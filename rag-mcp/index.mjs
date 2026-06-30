import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import { CallToolRequestSchema, ListToolsRequestSchema } from '@modelcontextprotocol/sdk/types.js';
import pkg from 'pg';
const { Pool } = pkg;
import { execSync } from 'child_process';

const pool = new Pool({
  host: process.env.PG_HOST || 'localhost',
  port: parseInt(process.env.PG_PORT || '5432'),
  database: process.env.PG_DATABASE || 'openwebui_vectors',
  user: process.env.PG_USER || 'openwebui',
  password: process.env.PGPASS,
  ssl: false,
});

const OLLAMA_BASE_URL = process.env.OLLAMA_BASE_URL || 'http://localhost:11434';
const EMBED_MODEL = process.env.EMBED_MODEL || 'nomic-embed-text';
const RAG_TOP_K = parseInt(process.env.RAG_TOP_K || '5');
// Open WebUI v0.10.1 stores 768-dim nomic-embed-text vectors zero-padded to 1536
const STORED_DIMS = 1536;

async function getEmbedding(text) {
  const res = await fetch(`${OLLAMA_BASE_URL}/api/embed`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ model: EMBED_MODEL, input: text }),
  });
  if (!res.ok) throw new Error(`Ollama embed failed: ${res.status} ${await res.text()}`);
  const data = await res.json();
  const vec = data.embeddings[0];
  const padded = new Array(STORED_DIMS).fill(0);
  vec.forEach((v, i) => { padded[i] = v; });
  return padded;
}

function getKbNames() {
  // Tries to resolve collection UUIDs to human-readable names via the Open WebUI
  // container SQLite database. Only works when running on Amelai itself.
  // Returns an empty map gracefully when called from a remote machine.
  try {
    const cmd = `docker exec open-webui python3 -c "import sqlite3,json; conn=sqlite3.connect('/app/backend/data/webui.db'); rows=conn.execute('SELECT id, name FROM knowledge').fetchall(); print(json.dumps({r[0]:r[1] for r in rows}))"`;
    const output = execSync(cmd, { timeout: 5000 }).toString().trim();
    return JSON.parse(output);
  } catch {
    return {};
  }
}

const server = new Server(
  { name: 'rag', version: '1.0.0' },
  { capabilities: { tools: {} } }
);

server.setRequestHandler(ListToolsRequestSchema, async () => ({
  tools: [
    {
      name: 'rag_search',
      description: "Search Amelai's local knowledge base (Portland Fuel internal documents: HSE regulations, DSEAR, PUWER, Legionella ACOP, maintenance procedures, REACH compliance, etc.) for information relevant to a query. Call this automatically when answering questions that may be covered by stored documents.",
      inputSchema: {
        type: 'object',
        properties: {
          query: {
            type: 'string',
            description: 'Natural language search query',
          },
          collection_name: {
            type: 'string',
            description: 'Optional: UUID of a specific knowledge base collection to restrict the search. If omitted, searches all collections.',
          },
        },
        required: ['query'],
      },
    },
    {
      name: 'rag_list_collections',
      description: "List all available knowledge base collections in Amelai's RAG database, with their names and document chunk counts.",
      inputSchema: { type: 'object', properties: {} },
    },
  ],
}));

server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const { name, arguments: args } = request.params;

  if (name === 'rag_list_collections') {
    const kbNames = getKbNames();
    const result = await pool.query(
      'SELECT collection_name, COUNT(*) AS chunks FROM document_chunk GROUP BY collection_name ORDER BY chunks DESC'
    );
    if (result.rows.length === 0) {
      return { content: [{ type: 'text', text: 'No collections found. Upload documents via Open WebUI (Workspace → Knowledge) first.' }] };
    }
    const lines = result.rows.map(r => {
      const displayName = kbNames[r.collection_name] || r.collection_name;
      return `${displayName} — ${r.chunks} chunks [id: ${r.collection_name}]`;
    });
    return { content: [{ type: 'text', text: lines.join('\n') }] };
  }

  if (name === 'rag_search') {
    const embedding = await getEmbedding(args.query);
    const vectorStr = `[${embedding.join(',')}]`;

    let result;
    if (args.collection_name) {
      result = await pool.query(
        `SELECT text, vmetadata, collection_name, 1 - (vector <=> $1::vector) AS score
         FROM document_chunk
         WHERE collection_name = $2
         ORDER BY vector <=> $1::vector
         LIMIT $3`,
        [vectorStr, args.collection_name, RAG_TOP_K]
      );
    } else {
      result = await pool.query(
        `SELECT text, vmetadata, collection_name, 1 - (vector <=> $1::vector) AS score
         FROM document_chunk
         ORDER BY vector <=> $1::vector
         LIMIT $2`,
        [vectorStr, RAG_TOP_K]
      );
    }

    if (result.rows.length === 0) {
      return { content: [{ type: 'text', text: 'No relevant documents found in the knowledge base.' }] };
    }

    const kbNames = getKbNames();
    const formatted = result.rows.map((r, i) => {
      const colDisplay = kbNames[r.collection_name] || r.collection_name;
      const source = r.vmetadata?.source || r.vmetadata?.file_name || r.vmetadata?.name || '';
      const header = `[${i + 1}] Score: ${parseFloat(r.score).toFixed(3)} | KB: ${colDisplay}${source ? ` | File: ${source}` : ''}`;
      return `${header}\n${r.text}`;
    }).join('\n\n---\n\n');

    return { content: [{ type: 'text', text: formatted }] };
  }

  throw new Error(`Unknown tool: ${name}`);
});

const transport = new StdioServerTransport();
await server.connect(transport);
