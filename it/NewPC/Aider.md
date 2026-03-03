# Aider — AI Pair Programming in the Terminal

**Version**: Current as of March 2026
**Reference**: <a href="https://aider.chat/docs/" target="_blank">Official Aider Documentation</a>

---

## What is Aider?

Aider is a terminal-based AI coding assistant. You run it inside any project directory and
describe what you want in plain English. Aider reads your codebase, makes the requested
changes directly to your files, and automatically commits each change to git with a
descriptive commit message.

Think of it as Claude Code but running locally against your Ollama server.

**What Aider can do:**
- Create new files and write code from scratch
- Edit existing files to add features, fix bugs, or refactor
- Add tests, documentation, or comments
- Explain what existing code does
- Work across multiple files simultaneously

**What Aider cannot do:**
- Search the web (use Open WebUI for that)
- Execute code (unless you run it separately)
- Work with PDFs directly (convert to text first — see below)

---

## Prerequisites

| Requirement | How to check |
|---|---|
| Aider installed | `aider --version` |
| OLLAMA_API_BASE set | `echo $OLLAMA_API_BASE` (should show server URL) |
| In a git repository | `git status` (should not say "not a git repo") |

If you are not in a git repo, initialise one first:

```bash
git init
git commit -m "Initial commit" --allow-empty
```

---

## Starting Aider

Navigate to your project directory and run:

```bash
aider
```

Aider will show the current model and a `>` prompt. You are now in a session.

**Start with specific files** (Aider focuses on these and ignores the rest):

```bash
aider src/main.py src/utils.py
```

**Override the model for this session:**

```bash
aider --model ollama/qwen3.5:27b      # general reasoning / writing
aider --model ollama/devstral         # coding (default)
aider --model ollama/qwen3.5:35b      # highest quality, slower
```

---

## Giving Instructions

Type your request at the `>` prompt in plain English:

```
> Add a function that validates email addresses using regex
> Fix the bug in the login function — it's not handling empty passwords
> Refactor the database class to use a connection pool
> Add docstrings to all functions in this file
> Explain what the parse_csv function does
```

Aider will show you the changes it plans to make, apply them, and commit to git.

---

## In-Session Commands

Type these at the `>` prompt during a session:

| Command | What it does |
|---|---|
| `/add filename.py` | Add a file to the session context |
| `/drop filename.py` | Remove a file from the session context |
| `/read document.txt` | Include a file as read-only context (e.g. a spec) |
| `/model ollama/qwen3.5:27b` | Switch to a different model mid-session |
| `/undo` | Undo the last AI-made change and its git commit |
| `/diff` | Show what has changed in the current session |
| `/git log --oneline -5` | Run any git command from inside Aider |
| `/help` | List all available commands |
| `/exit` | Exit Aider |

---

## Common Workflows

### Create a new script from scratch

```bash
cd ~/my-project
aider
> Create a Python script that reads a CSV file and outputs a summary of each column
```

### Edit an existing file

```bash
aider src/auth.py
> The login function doesn't handle the case where the user account is locked. Add that check.
```

### Work across multiple files

```bash
aider src/models.py src/views.py tests/test_views.py
> Add a new User model with email and created_at fields, update the views to use it, and add tests
```

### Use a document as context

```bash
pdftotext specification.pdf specification.txt
aider --read specification.txt src/main.py
> Implement the API endpoints described in the specification
```

### Switch model mid-task

```
> Analyse this code and identify any security issues
  [review the analysis]
> /model ollama/devstral
> Fix all the security issues you identified
```

---

## Model Reference

| Model | Command | Best for |
|---|---|---|
| devstral | `ollama/devstral` | Default coding — fast, agentic, file-aware |
| qwen3.5:27b | `ollama/qwen3.5:27b` | General reasoning, writing, analysis |
| qwen3.5:35b | `ollama/qwen3.5:35b` | High-quality output when accuracy matters |
| qwen3.5:9b | `ollama/qwen3.5:9b` | Fast fallback when server is under load |

---

## Per-Project Model Default

To set a different default model for a specific project, create `.aider.conf.yml` in that
project's root directory:

```yaml
# /path/to/project/.aider.conf.yml
model: ollama/qwen3.5:27b   # override for this project only
```

This overrides `~/.aider.conf.yml` for that project only.

---

## Troubleshooting

**"No git repository found"**
Initialise git first: `git init && git commit -m "Initial commit" --allow-empty`

**"Connection error" / model not responding**
Check the Ollama server is reachable: `curl http://100.79.83.113:11434/api/tags`
Check the env var is set: `echo $OLLAMA_API_BASE`

**Aider made a change I don't want**
Type `/undo` to revert the last change and its commit.

**Aider is including too many files**
Use `/drop filename` to remove files from the context, or start Aider with only the files
you need: `aider specific-file.py`

**Changes are not being committed**
Check `auto-commits: true` is in `~/.aider.conf.yml` and `git: true` is set.

---

## Further Reading

- <a href="https://aider.chat/docs/usage/commands.html" target="_blank">Aider — In-chat commands reference</a>
- <a href="https://aider.chat/docs/config/aider_conf.html" target="_blank">Aider — Configuration file reference</a>
- <a href="https://aider.chat/docs/usage/tips.html" target="_blank">Aider — Tips for effective use</a>
- <a href="https://aider.chat/docs/leaderboards/" target="_blank">Aider — Model leaderboards (benchmark scores)</a>
