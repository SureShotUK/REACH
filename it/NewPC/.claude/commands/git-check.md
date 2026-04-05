# /git-check - Git Repository Sync and Check

**Usage:** `/git-check`

**What it does:** Automatically sync the repository with GitHub:
1. Shows current git status (modified files, branches)
2. Fetches latest from GitHub remote
3. Pulls in any updates if you're behind
4. Stages all local changes (`git add -A`)
5. Creates auto-commit with timestamp
6. Pushes changes to GitHub

**How it works:** Runs `.claude/scripts/git-sync.sh` which performs the sync operation.

**Output:** Git status before and after syncing, fetch/pull results, commit confirmation, push status.

**Note:** Requires write permissions for git operations. The script uses "Claude Code" as the author name for commits.
