---
description: Commit and push all session changes to GitHub
---

You are now executing the sync-session command to commit and push all changes made during this Claude Code session.

Follow these steps:

1. Run `git status` to check for any changes
2. If there are changes:
   - Run `git add .` to stage all changes
   - Check `git log -1 --format='%an %ae'` to see the last commit author
   - Create a descriptive commit message that:
     - Summarizes the work done in this session
     - Lists key files created or modified
     - Ends with: "🤖 Generated with [Claude Code](https://claude.com/claude-code)\n\nCo-Authored-By: Claude <noreply@anthropic.com>"
   - Commit the changes using a HEREDOC for proper formatting
   - Push to the remote repository with `git push origin master` (or main if that's the default branch)
3. If there are no changes, inform the user that everything is already up to date
4. Handle any errors gracefully (e.g., if push fails due to remote changes, suggest pulling first)

After completion, provide a summary of what was committed and pushed.
