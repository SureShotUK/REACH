# tmux — Terminal Multiplexer

**Platform**: Linux / macOS / WSL2
**Install**: `sudo apt install tmux` (Ubuntu/Debian)

---

## What is tmux?

tmux is a **terminal multiplexer** — it lets you run multiple terminal sessions inside a single SSH connection or terminal window, and crucially, **sessions survive disconnection**. If your SSH connection drops or you close your terminal window, anything running inside tmux keeps running on the server.

### Core concepts

| Concept | What it is |
|---|---|
| **Session** | A top-level container. Survives disconnection. You can have multiple sessions. |
| **Window** | A tab inside a session. Each window is a full-screen terminal. |
| **Pane** | A split section of a window. One window can have multiple panes side by side or stacked. |

Think of it as: **Session → Windows → Panes**

---

## The Prefix Key

Every tmux keyboard shortcut starts with the **prefix key**: `Ctrl+B`

Press `Ctrl+B`, release both keys, then press the command key. Do not hold them all at once.

---

## Sessions

### Starting and stopping

| Command | What it does |
|---|---|
| `tmux` | Start a new unnamed session |
| `tmux new-session -s my-name` | Start a new session named `my-name` |
| `tmux list-sessions` | List all running sessions |
| `tmux attach -t my-name` | Attach to session named `my-name` |
| `tmux kill-session -t my-name` | Kill a session and everything in it |

### Leaving a session without killing it

This is the most important tmux skill — **detach** from the session. The session keeps running in the background:

**Inside tmux**: `Ctrl+B`, then `D`

You are returned to your normal terminal. The session and everything running inside it continues on the server. You can safely close your SSH connection or terminal window.

To come back later: `tmux attach -t my-name`

### Switching between sessions (when inside tmux)

| Shortcut | What it does |
|---|---|
| `Ctrl+B, S` | Show interactive session list — use arrow keys to select, Enter to switch |
| `Ctrl+B, $` | Rename the current session |
| `tmux switch-client -t my-name` | Switch to named session from command line |

---

## Windows (tabs)

| Shortcut | What it does |
|---|---|
| `Ctrl+B, C` | Create a new window |
| `Ctrl+B, N` | Switch to next window |
| `Ctrl+B, P` | Switch to previous window |
| `Ctrl+B, 0–9` | Switch to window by number |
| `Ctrl+B, W` | Show interactive window list |
| `Ctrl+B, ,` | Rename the current window |
| `Ctrl+B, &` | Close the current window (prompts for confirmation) |

Windows are shown in the status bar at the bottom of the screen. The current window is marked with `*`.

---

## Panes (split screen)

| Shortcut | What it does |
|---|---|
| `Ctrl+B, %` | Split pane vertically (left and right) |
| `Ctrl+B, "` | Split pane horizontally (top and bottom) |
| `Ctrl+B, Arrow key` | Move focus to pane in that direction |
| `Ctrl+B, Z` | Zoom current pane to full screen (press again to unzoom) |
| `Ctrl+B, X` | Close current pane (prompts for confirmation) |
| `Ctrl+B, Q` | Show pane numbers briefly — press the number to jump to it |

---

## Scrolling and Copy Mode

By default you cannot scroll up in tmux with the mouse wheel. To scroll through output:

| Shortcut | What it does |
|---|---|
| `Ctrl+B, [` | Enter scroll/copy mode — use arrow keys or PageUp/PageDown to scroll |
| `Q` | Exit scroll mode |

---

## Practical Workflows

### Running a long job (e.g. AI training) and leaving it unattended

```bash
# On the server, start a named session
tmux new-session -s lora-training

# Run your job inside tmux
bash examples/qwen_image/model_training/lora/stage2_train.sh

# When you want to leave — detach (session keeps running)
# Press: Ctrl+B, then D

# Close your SSH connection or terminal — job continues on server
# Later, reattach to check progress:
tmux attach -t lora-training
```

### Monitoring a job in a split view

```bash
# Inside tmux, split horizontally
# Press: Ctrl+B, then "

# In the bottom pane, run a monitoring command
watch -n 5 'free -h && echo "---" && nvidia-smi --query-gpu=memory.used --format=csv,noheader'

# Switch between panes: Ctrl+B, Arrow key
# Zoom in on the training output pane: Ctrl+B, Z
```

### Checking if a session is still running after reconnecting

```bash
tmux list-sessions
# Output: lora-training: 1 windows (created Tue Mar 18 22:41:00 2026) [220x50]

tmux attach -t lora-training
```

If `tmux list-sessions` returns `no server running on /tmp/tmux-1000/default`, all sessions have ended (server was rebooted, or sessions were killed).

---

## Closing Things Cleanly

| What you want to do | How to do it |
|---|---|
| Leave session running, disconnect | `Ctrl+B, D` (detach) |
| Close current pane | `exit` or `Ctrl+D` in the shell, or `Ctrl+B, X` |
| Close current window | `Ctrl+B, &` |
| Kill entire session | `tmux kill-session -t my-name` |
| Kill all sessions | `tmux kill-server` |

**Never** just close your SSH/terminal window without detaching first if you care about the session. Closing the terminal without detaching will kill the session in some terminal emulators (though not all). Detaching with `Ctrl+B, D` is always safe.

---

## Quick Reference Card

```
Start session:      tmux new-session -s name
Attach:             tmux attach -t name
List sessions:      tmux list-sessions
Kill session:       tmux kill-session -t name

--- Inside tmux (all start with Ctrl+B) ---

Detach (leave running):   Ctrl+B, D
New window:               Ctrl+B, C
Switch window:            Ctrl+B, N / P / 0-9
Split vertical:           Ctrl+B, %
Split horizontal:         Ctrl+B, "
Move between panes:       Ctrl+B, Arrow key
Zoom pane:                Ctrl+B, Z
Scroll mode:              Ctrl+B, [  (Q to exit)
Session list:             Ctrl+B, S
```
