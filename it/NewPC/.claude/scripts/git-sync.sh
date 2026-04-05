#!/bin/bash
# git-sync.sh - Git Repository Sync and Check
# Usage: ./git-sync.sh [options]
# Options:
#   --status  : Only show git status, don't sync
#   --verbose : Show more detail

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
WORKING_DIR="$(dirname "$SCRIPT_DIR")/.."

cd "$WORKING_DIR"

echo "=== Git Repository Status ==="
git status --short
echo ""

if [[ "$1" == "--status" ]]; then
    exit 0
fi

echo "Checking for updates from GitHub..."
REMOTE_HEAD=$(git ls-remote origin HEAD 2>/dev/null | cut -f1)
LOCAL_HEAD=$(git rev-parse HEAD 2>/dev/null)

if [[ -n "$REMOTE_HEAD" && "$REMOTE_HEAD" != "$LOCAL_HEAD" ]]; then
    echo "Remote has updates. Fetching..."
    git fetch origin
fi

echo ""
echo "=== Pulling updates (if any)... ==="
git pull --ff-only 2>&1 || true

echo ""
echo "=== Local changes: ==="
if [[ -n "$(git status --porcelain)" ]]; then
    echo "Staging changes..."
    git add -A

    TIMESTAMP=$(date +"%Y-%m-%d %H:%M")
    git commit --author="Claude Code <claude@example.com>" -m "Auto-sync: $TIMESTAMP" 2>&1 && {
        echo ""
        echo "=== Pushing to GitHub... ==="
        git push origin main 2>&1 || echo "Push failed (may be protected branch)"
    }
else
    echo "No local changes to sync."
fi

echo ""
echo "=== Sync complete ==="
git status --short
