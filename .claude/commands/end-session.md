---
description: Update context files and document session progress
---

You are now executing the end-session command to update project tracking files and document the work completed in this Claude Code session.

## Session Documentation Process

Follow these steps systematically:

### 1. Gather Session Information

Run these commands in parallel to collect session data:
- `git log --oneline -20 --no-merges` - Recent commits
- `git log -1 --stat` - Last commit details with file changes
- `git diff HEAD~5..HEAD --name-status` - Files changed in recent commits (if more than one commit)

### 2. Analyze Session Activity

Based on the git history and your memory of this session, identify:
- **Key accomplishments**: What was built, fixed, or improved?
- **Files created/modified**: Which files were added or changed?
- **Important decisions**: What architectural or implementation choices were made?
- **Reference documents**: Were any new documents added or existing ones referenced?
- **Next actions**: What tasks remain or should be done next?

### 3. Update SESSION_LOG.md

Append a new session entry to SESSION_LOG.md (create the file if it doesn't exist):

```markdown
## Session [YYYY-MM-DD HH:MM]

### Summary
[2-3 sentence summary of what was accomplished]

### Work Completed
- [Bullet point list of key accomplishments]
- [Include links to reference documents if applicable]

### Files Changed
- `path/to/file.ext` - [Brief description of changes]
- `path/to/another.ext` - [Brief description]

### Git Commits
- `commit-hash` - Commit message summary

### Key Decisions
- [Any important architectural or implementation decisions]
- [Trade-offs considered]

### Reference Documents
- [Links to any documents uploaded, created, or referenced]
- [External resources consulted]

### Next Actions
- [ ] [Outstanding task 1]
- [ ] [Outstanding task 2]

---
```

### 4. Update PROJECT_STATUS.md

Update or create PROJECT_STATUS.md with the current project state:

```markdown
# Project Status

**Last Updated**: [YYYY-MM-DD]

## Current State
[Brief description of where the project stands now]

## Active Work Areas
- [Area 1]: [Status and progress]
- [Area 2]: [Status and progress]

## Recently Completed
- [Recent accomplishment 1]
- [Recent accomplishment 2]

## Blocked/Pending
- [Any blockers or items waiting on decisions]

## Next Priorities
1. [Priority 1]
2. [Priority 2]
3. [Priority 3]

## Key Files & Structure
- `/important/path/` - [What it contains]
- `important-file.md` - [Purpose]
```

### 5. Update CHANGELOG.md

Add an entry to CHANGELOG.md (create if it doesn't exist) in version-style format:

```markdown
## [Unreleased] - YYYY-MM-DD

### Added
- [New features or files added]

### Changed
- [Modifications to existing features or files]

### Fixed
- [Bug fixes or corrections]

### Documentation
- [Documentation updates or new documents]
```

### 6. Review CLAUDE.md for Updates

Check if the session revealed any patterns or learnings that should be added to CLAUDE.md:
- New workflows discovered
- Common patterns or processes
- Lessons learned about the codebase structure
- Updates to working practices

If updates are warranted, suggest specific additions to CLAUDE.md for the user to review.

### 7. Review gemini.md for Updates (if applicable)

If the session involved research, web searches, or gathering external information, check if gemini.md needs updates:
- New research patterns
- Useful external resources discovered
- Search strategies that worked well

Suggest specific additions if relevant.

### 8. Commit and Push

After updating all files:
- Run `git add SESSION_LOG.md PROJECT_STATUS.md CHANGELOG.md` (and any other updated files)
- Create a commit: `End of session documentation update - [brief session summary]`
- Include the standard Claude Code co-authorship footer
- Push to remote: `git push origin main`

## Important Guidelines

- **Be specific**: Use actual file paths, commit hashes, and concrete details
- **Be concise**: Summaries should be clear but not verbose
- **Link references**: Include full paths to any documents referenced or created
- **Maintain chronology**: SESSION_LOG.md should be in reverse chronological order (newest first)
- **Keep status current**: PROJECT_STATUS.md should reflect the actual current state
- **Use proper formatting**: Maintain markdown formatting and structure
- **Ask if unclear**: If you don't have enough context about the session, ask the user for clarification

## Output

After completing all updates, provide the user with:
1. A summary of what was documented
2. Confirmation that files were updated
3. The git commit hash for the documentation update
4. Any suggested CLAUDE.md or gemini.md updates for their review
