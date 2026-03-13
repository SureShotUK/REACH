# RAG Setup — Local Document Q&A with Open WebUI + pgvector

**System**: Ubuntu 24.04 LTS Server, Dual RTX 3090 (48GB VRAM), Ollama + Open WebUI
**Last Updated**: March 2026

---

## Overview

RAG (Retrieval-Augmented Generation) allows you to upload your own documents and then ask questions about them. Instead of the model relying only on its training data, it searches your documents for relevant passages and includes them in its context before generating a response.

**What this setup adds to your existing stack:**

```
                    ┌─────────────────────────────────────┐
  Your documents →  │  Open WebUI (existing, port 3000)   │
                    │  + Built-in RAG engine              │
                    └──────────────┬──────────────────────┘
                                   │
                     ┌─────────────▼─────────────┐
                     │  nomic-embed-text (Ollama) │  ← converts text to vectors
                     └─────────────┬─────────────┘
                                   │
                     ┌─────────────▼─────────────┐
                     │  PostgreSQL 16 + pgvector  │  ← stores and searches vectors
                     │  (host service, port 5432) │
                     └───────────────────────────┘
```

Open WebUI has built-in RAG support. No additional application is needed. The changes are:
1. Install PostgreSQL + pgvector on the Ubuntu host
2. Pull an embedding model into Ollama
3. Reconfigure the Open WebUI Docker container with three extra environment variables

**What "embedding" means**: When you upload a document, Open WebUI breaks it into chunks and sends each chunk through the embedding model. The model converts each chunk into a list of numbers (a "vector") that represents its meaning. These vectors are stored in pgvector. When you ask a question, your question is also converted to a vector, and the database finds the stored vectors (document chunks) that are mathematically closest — meaning closest in meaning.

---

## Why pgvector over the default (ChromaDB)

Open WebUI uses ChromaDB (embedded, in-container) by default. Switching to pgvector gives you:

- **Data durability**: Vectors stored in PostgreSQL survive container rebuilds without needing to re-upload documents
- **SQL access**: You can query, inspect, and manage your vector data using standard SQL tools
- **Familiarity**: You already know PostgreSQL — pgvector is just an extension, not a new product
- **No extra containers**: pgvector runs inside your existing (new) PostgreSQL install

---

## Embedding Model Comparison

| Model | Dimensions | File Size | Context Window | Notes |
|-------|-----------|----------|----------------|-------|
| `nomic-embed-text` | 768 | 270MB | **8,192 tokens** | **Recommended** — best for long documents |
| `mxbai-embed-large` | 1024 | 670MB | 512 tokens | Higher raw accuracy, poor for long chunks |
| `bge-m3` | 1024 | 1.2GB | 8,192 tokens | Good accuracy, larger download |
| `all-minilm` | 384 | 45MB | 512 tokens | Very fast, lowest accuracy |

**Recommendation: `nomic-embed-text`** — its 8,192-token context window means longer document chunks can be embedded without losing context. The 512-token limit of `mxbai-embed-large` requires very aggressive chunking that can split sentences mid-thought. VRAM cost is negligible (~270MB on a 48GB system).

**Important**: Once you choose an embedding model and start uploading documents, do not change it. Changing the model means all existing embeddings become incompatible and all documents must be re-uploaded. Choose once and stick with it.

---

## Step 1 — Install PostgreSQL 16 and pgvector

Run these commands on the Ubuntu server (via SSH):

```bash
# Add the official PostgreSQL APT repository
sudo apt update && sudo apt install -y curl gnupg2 lsb-release
curl -fsSL https://www.postgresql.org/media/keys/ACCC4CF8.asc | \
  sudo gpg --dearmor -o /etc/apt/trusted.gpg.d/postgresql.gpg
sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" \
  > /etc/apt/sources.list.d/pgdg.list'

# Install PostgreSQL 16 and the pgvector extension
sudo apt update
sudo apt install -y postgresql-16 postgresql-contrib-16 postgresql-16-pgvector

# Enable and start PostgreSQL
sudo systemctl enable --now postgresql

# Verify it is running
sudo systemctl status postgresql
```

---

## Step 2 — Create the Database and User

```bash
sudo -i -u postgres psql <<'EOF'
CREATE USER openwebui WITH PASSWORD 'choose_a_strong_password_here';
CREATE DATABASE openwebui_vectors OWNER openwebui;
\c openwebui_vectors
CREATE EXTENSION IF NOT EXISTS vector;
\q
EOF
```

Replace `choose_a_strong_password_here` with a real password. Note it down — you will need it in Step 5.

This creates a dedicated database (`openwebui_vectors`) for RAG data only. The Open WebUI application's own data (chat history, user accounts, settings) stays in its Docker volume using SQLite, unchanged.

Verify the extension is enabled:

```bash
sudo -u postgres psql -d openwebui_vectors -c "\dx"
# Should list: vector | 0.x.x | public | vector data type and ivfflat and hnsw access methods
```

---

## Step 3 — Allow Docker Containers to Connect

PostgreSQL by default only accepts connections from localhost. Docker containers have their own network addresses (typically in the `172.x.x.x` range), so PostgreSQL needs to be configured to accept those connections.

### 3a. Make PostgreSQL listen on all interfaces

```bash
sudo sed -i "s/#listen_addresses = 'localhost'/listen_addresses = '*'/" \
  /etc/postgresql/16/main/postgresql.conf

# Verify the change
grep "listen_addresses" /etc/postgresql/16/main/postgresql.conf
```

### 3b. Allow the openwebui user to connect from Docker

```bash
# Add a rule to accept connections from all IPs for this specific user/database
# (still requires password authentication)
echo "host  openwebui_vectors  openwebui  0.0.0.0/0  scram-sha-256" | \
  sudo tee -a /etc/postgresql/16/main/pg_hba.conf
```

### 3c. Allow PostgreSQL through the firewall

```bash
# Allow Docker's bridge network to reach PostgreSQL on the host
# Find your Docker bridge subnet:
docker network inspect ai-network --format '{{range .IPAM.Config}}{{.Subnet}}{{end}}'
# Typical output: 172.18.0.0/16 — use whatever your network shows

# Allow that subnet to reach port 5432
sudo ufw allow from 172.18.0.0/16 to any port 5432
```

> If the `docker network inspect` command returns a different subnet, use that value in the `ufw allow` command.

### 3d. Restart PostgreSQL

```bash
sudo systemctl restart postgresql
```

### 3e. Test connectivity from a Docker container

```bash
# Confirm PostgreSQL is reachable from within Docker
docker run --rm --network ai-network \
  postgres:16 \
  psql "postgresql://openwebui:your_password@192.168.1.192:5432/openwebui_vectors" \
  -c "SELECT version();"
```

You should see a PostgreSQL version string. If you see a connection refused error, check Steps 3a–3c again.

---

## Step 4 — Pull the Embedding Model

```bash
ollama pull nomic-embed-text
```

This downloads approximately 270MB. Verify it pulled correctly:

```bash
ollama list | grep nomic
```

---

## Step 5 — Reconfigure the Open WebUI Container

The existing Open WebUI container needs three extra environment variables. The approach is to stop and remove the current container, then recreate it with the original settings plus the new ones. **All chat history and settings are preserved in the Docker volume — only the container itself is replaced.**

### Current docker run command (from Software_Setup.md):

```bash
docker run -d \
  --name open-webui \
  --restart always \
  --gpus all \
  -p 3000:8080 \
  -v open-webui:/app/backend/data \
  -e OLLAMA_BASE_URL=http://192.168.1.192:11434 \
  ghcr.io/open-webui/open-webui:main
```

### Updated docker run command (with RAG additions):

```bash
# Stop and remove the existing container (data is safe in the Docker volume)
docker stop open-webui && docker rm open-webui

# Recreate with RAG environment variables added
docker run -d \
  --name open-webui \
  --restart always \
  --gpus all \
  --network ai-network \
  -p 3000:8080 \
  -v open-webui:/app/backend/data \
  -e OLLAMA_BASE_URL=http://192.168.1.192:11434 \
  -e VECTOR_DB=pgvector \
  -e PGVECTOR_DB_URL=postgresql://openwebui:your_password@192.168.1.192:5432/openwebui_vectors \
  -e RAG_EMBEDDING_ENGINE=ollama \
  -e RAG_EMBEDDING_MODEL=nomic-embed-text \
  ghcr.io/open-webui/open-webui:main
```

Replace `your_password` with the password you chose in Step 2.

> **Note on PGVECTOR_DB_URL vs DATABASE_URL**: Open WebUI uses `PGVECTOR_DB_URL` for the vector database connection. If after starting the container you see errors about pgvector in the logs mentioning `DATABASE_URL`, try replacing `-e PGVECTOR_DB_URL=...` with `-e DATABASE_URL=...` in the command above. The variable name shifted between Open WebUI releases. Check `docker logs open-webui` to diagnose.

Verify the container started cleanly:

```bash
docker logs open-webui --tail 50
```

Look for a startup message confirming pgvector is in use (e.g., `"Using pgvector"` or similar). No error messages about database connection failures should appear.

---

## Step 6 — Configure Embedding in Open WebUI UI

The environment variables tell Open WebUI which embedding engine to use, but you should also confirm the settings in the UI:

1. Open Open WebUI in your browser (http://192.168.1.192:3000 or your Tailscale URL)
2. Go to **Admin Panel** (top-right menu) → **Settings** → **Documents**
3. Set **Embedding Model Engine** → `Ollama`
4. Set **Embedding Model** → `nomic-embed-text`
5. Click **Save**

You should see a green confirmation. If the model field shows an error, verify that `ollama list` shows `nomic-embed-text` on the server.

---

## Step 7 — Adding and Using Documents

### Method 1: Knowledge Bases (Persistent, Recommended)

Knowledge Bases are document collections that persist across chat sessions and can be used in any conversation.

**Creating a Knowledge Base:**

1. In Open WebUI, go to **Workspace** → **Knowledge** (in the left sidebar)
2. Click **+ Create** (top-right)
3. Give it a name (e.g., "IT Documentation", "Work Contracts", "Research Papers")
4. Click **Create**
5. Drag and drop files onto the upload area, or click to browse
6. Wait for the green tick — processing is complete when it appears
7. Repeat for additional files

**Supported file types**: PDF, Word (.docx), Excel (.xlsx), PowerPoint (.pptx), plain text (.txt), Markdown (.md), CSV, HTML, and more.

**Using a Knowledge Base in a chat:**

1. Start a new chat
2. Type `#` in the message box — a dropdown appears listing your Knowledge Bases
3. Select the Knowledge Base you want to search
4. Type your question and send
5. The response will include a **Sources** section at the bottom citing the specific document chunks used

### Method 2: Single-session Document Upload

For one-off document questions without creating a persistent Knowledge Base:

1. In a chat, click the **paperclip icon** (📎) in the message box
2. Select a file from your computer
3. The file is uploaded, processed, and attached to this conversation only
4. Ask questions about it directly — no `#` prefix needed

---

## Step 8 — Verification

### Verify vectors are being stored

After uploading at least one document, check that tables were created in PostgreSQL:

```bash
sudo -u postgres psql -d openwebui_vectors -c "\dt"
```

You should see tables created by Open WebUI (the exact names depend on the version, but will be vector storage tables).

Check that embeddings exist:

```bash
sudo -u postgres psql -d openwebui_vectors -c "SELECT COUNT(*) FROM (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public') t;"
```

### End-to-end test

1. Create a Knowledge Base called "Test"
2. Upload a PDF or text file you know the contents of (e.g., a technical document with specific facts)
3. In a new chat, type `#Test` to attach the Knowledge Base
4. Ask a specific factual question whose answer is in the document
5. Confirm the answer is correct AND that a **Sources** citation appears below the response

If the answer is correct but Sources do not appear, check that retrieval is enabled:
- Admin Panel → Settings → Documents → **RAG** should be enabled

---

## Environment Variable Reference

| Variable | Value | Purpose |
|----------|-------|---------|
| `VECTOR_DB` | `pgvector` | Switches from ChromaDB (default) to pgvector |
| `PGVECTOR_DB_URL` | `postgresql://user:pass@host:5432/db` | pgvector connection string |
| `DATABASE_URL` | (same as above) | Fallback if `PGVECTOR_DB_URL` not recognised |
| `RAG_EMBEDDING_ENGINE` | `ollama` | Use local Ollama for generating embeddings |
| `RAG_EMBEDDING_MODEL` | `nomic-embed-text` | Which embedding model to use |
| `RAG_TOP_K` | `5` | Number of document chunks retrieved per query (default: 4) |
| `RAG_RELEVANCE_THRESHOLD` | `0.0` | Minimum similarity score — raise to filter low-quality matches |
| `CHUNK_SIZE` | `1500` | Characters per document chunk (default ~1000) |
| `CHUNK_OVERLAP` | `150` | Overlap between consecutive chunks to preserve context |

To add optional variables, include additional `-e KEY=VALUE` flags in the `docker run` command.

---

## Troubleshooting

### Container fails to start / pgvector connection errors

```bash
docker logs open-webui | grep -i "error\|pgvector\|database"
```

Common issues:
- **Wrong password**: Check the password in `PGVECTOR_DB_URL` matches what you set in Step 2
- **Wrong IP**: Confirm PostgreSQL is reachable at `192.168.1.192:5432` — test with Step 3e
- **PGVECTOR_DB_URL not recognised**: Try `DATABASE_URL` instead (see Step 5 note)

### Document upload hangs or fails

```bash
docker logs open-webui --tail 100 -f
# Upload a document while watching the logs
```

Common causes:
- **nomic-embed-text not pulled**: Run `ollama list` and confirm it appears
- **Ollama unreachable from container**: Confirm `OLLAMA_BASE_URL` is correct
- **File too large**: Very large PDFs (>200MB) may time out — split into smaller files

### Retrieval returns wrong results

- Try increasing `RAG_TOP_K` (more chunks retrieved = more context for the model)
- The quality of retrieval depends heavily on the chunk size vs the specificity of your question
- For very long documents, consider splitting them into smaller files by topic before uploading

### Chat model ignores the retrieved context

Ensure the LLM you are using has sufficient context window configured. In Open WebUI:

1. Click the model settings icon (⚙️) next to the model name in the chat
2. Find **Context Length** (`num_ctx`) — set it to at least `8192`
3. This ensures the retrieved document chunks plus your question fit within the model's context

### PostgreSQL connection test fails (Step 3e)

```bash
# Check PostgreSQL is listening on all interfaces
sudo ss -tlnp | grep 5432
# Expected: 0.0.0.0:5432 (not 127.0.0.1:5432)

# Check pg_hba.conf was updated
sudo tail -5 /etc/postgresql/16/main/pg_hba.conf
# Expected: the line added in Step 3b

# Check UFW allows the Docker subnet
sudo ufw status | grep 5432
```

---

## Updating Open WebUI (with RAG config preserved)

When updating Open WebUI in the future, use the full command from Step 5 (with all RAG environment variables) — not the shorter command from `Software_Setup.md`. The RAG variables must be included each time the container is recreated:

```bash
docker stop open-webui && docker rm open-webui
docker pull ghcr.io/open-webui/open-webui:main

docker run -d \
  --name open-webui \
  --restart always \
  --gpus all \
  --network ai-network \
  -p 3000:8080 \
  -v open-webui:/app/backend/data \
  -e OLLAMA_BASE_URL=http://192.168.1.192:11434 \
  -e VECTOR_DB=pgvector \
  -e PGVECTOR_DB_URL=postgresql://openwebui:your_password@192.168.1.192:5432/openwebui_vectors \
  -e RAG_EMBEDDING_ENGINE=ollama \
  -e RAG_EMBEDDING_MODEL=nomic-embed-text \
  ghcr.io/open-webui/open-webui:main
```

**Tip**: Copy this full command into a shell script (e.g., `/home/steve/scripts/start-open-webui.sh`) so you only have to maintain one canonical version.

---

## Resources

- <a href="https://docs.openwebui.com/tutorials/features/rag" target="_blank">Open WebUI RAG Documentation</a>
- <a href="https://github.com/pgvector/pgvector" target="_blank">pgvector GitHub Repository</a>
- <a href="https://ollama.com/library/nomic-embed-text" target="_blank">nomic-embed-text on Ollama Library</a>
- <a href="https://www.postgresql.org/docs/16/index.html" target="_blank">PostgreSQL 16 Documentation</a>

---

**Document Version**: 1.0
**Created**: March 2026
