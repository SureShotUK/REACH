---
description: Sync local files with GitHub — commit local changes, pull remote changes, then push. Safe to run on any machine.
---

You are executing the sync-files command to synchronise this repository with GitHub. This command is safe to run on any machine (Windows or Linux) and handles the common cases automatically.

Follow these steps exactly:

## Step 1 — Check current state

Run these commands to understand the current state:
```
git status
git fetch origin
git status -sb
```

The `git fetch origin` updates your local knowledge of the remote without changing any files. After fetching, `git status -sb` will show whether you are ahead, behind, or diverged.

Note the output for use in later steps.

## Step 2 — Commit any local changes

If `git status` shows any modified, new, or deleted files:

1. Stage all changes: `git add -A`
2. Create a commit with a message in this format:
   ```
   Sync: <brief summary of what changed>

   Machine: <hostname from running: hostname>
   Date: <current date and time>

   Co-Authored-By: Claude <noreply@anthropic.com>
   ```
3. Use a HEREDOC to pass the commit message to avoid formatting issues

If there are no local changes, skip this step.

## Step 3 — Determine sync direction and act

After committing (or if there was nothing to commit), check the relationship between local and remote:

**Case A — Local is up to date with remote** (no ahead/behind shown):
- Nothing to do. Report "Already in sync with GitHub."

**Case B — Local is only behind remote** (remote has commits you don't have, you have none they don't):
- Run: `git pull --rebase origin main`
- Report what was pulled.

**Case C — Local is only ahead of remote** (you have commits the remote doesn't):
- Run: `git push origin main`
- Report what was pushed.

**Case D — Diverged** (both local and remote have commits the other doesn't — this includes cases where you just committed in Step 2 and the remote also has new commits):
- Run: `git pull --rebase origin main`
- If the rebase succeeds (no conflicts): run `git push origin main`
- If the rebase reports conflicts: stop and tell the user which files have conflicts and that they need to be resolved manually before the push can complete. Show the output of `git status` and `git diff --name-only --diff-filter=U`.

## Step 4 — Report outcome

Provide a clear summary:
- Whether local changes were committed (and what)
- Whether remote changes were pulled (and what commits)
- Whether the push succeeded
- Current state: "Repository is now in sync" or "Manual conflict resolution needed for: <files>"

## Error handling

- If `git push` is rejected (not fast-forward after the rebase somehow): report the exact error and suggest the user check whether someone else has pushed since the rebase started
- If `git pull --rebase` is interrupted by conflicts: do NOT run `git rebase --abort` automatically — leave it for the user to resolve. Show exactly which files conflict.
- If there is no remote configured: report this clearly with the message "No remote repository configured. Run `git remote add origin <URL>` to connect this repository to GitHub."

## Notes on "newest version wins"

This command uses rebase, which means: if the same file was changed both locally and on the remote, your local version is applied on top of the remote version. In a true conflict (same line changed in both places), your local edits take priority. This approximates "local machine's most recent changes win" for the common case of working on one machine at a time.
