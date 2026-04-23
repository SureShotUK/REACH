# CLAUDE.md (Shared Context)

This file provides shared guidance to Claude Code (claude.ai/code) when working with any project in this repository structure.

## Repository Structure

This terminai directory contains multiple project folders, each with their own specific focus. Each project has its own CLAUDE.md file with project-specific instructions that supplement these shared guidelines.

## Projects

| Directory | Description |
|-----------|-------------|
| `hseea/` | Health, Safety, and Environmental Compliance knowledge |
| `it/` | IT infrastructure, virtualisation, and security documentation |
| `Maintenance/` | Maintenance administration system - statutory compliance, PPM, reactive and emergency repairs, cost tracking across two sites |
| `Canada/` | Canadian financial regulation research |
| `REACH/` | REACH chemical regulation documentation |
| `insurance/` | Insurance documentation |


## Core Principles (All Projects)

### Accuracy Through Verification
Before providing any answer or implementing any solution:
- Cross-reference information across multiple sources within the codebase
- Verify assumptions by reading relevant files rather than making educated guesses
- When uncertain about implementation details, search for similar patterns in the codebase
- Check for existing utilities, helpers, or patterns before creating new ones
- Validate that your understanding matches the actual code structure

### Always Ask Clarifying Questions First
Before providing any advice, calculations, or recommendations, ALWAYS ask questions to gather all necessary information to provide the most accurate answer possible. Never make assumptions about:
- Specific volumes, quantities, or measurements
- Operating schedules, frequencies, or durations
- Site-specific conditions or constraints
- Process details or technical specifications
- Regulatory context or applicable exemptions
- Budget constraints or timeline requirements
- The task or requirement if it's ambiguous or could be interpreted multiple ways
- Multiple valid approaches when the preferred one is unclear
- Context, priority, or scope of a request
- Potential trade-offs or implications the user should be aware of

Use the AskUserQuestion tool when multiple pieces of information are needed. Providing accurate, tailored advice based on complete information is more valuable than providing quick but potentially incorrect generic guidance.

### Task Management
- Use the TodoWrite tool to plan and track multi-step tasks
- Break down complex requests into manageable steps
- Verify each step before moving to the next
- Update task status in real-time as work progresses
- Mark todos as completed immediately after finishing (don't batch completions)
- Keep exactly ONE task in_progress at any time

### Warnings Before Instructions
When providing instructions that include destructive, irreversible, or high-risk actions (e.g. deleting files, discarding changes, dropping databases, force-pushing, running `rm -rf`, `git clean`, `git checkout -- .`, etc.):
- **Always place the warning BEFORE the commands**, not after
- The user must see the risk before they can act on the instructions — a warning after the commands is useless

### Professional Objectivity
- Prioritize technical accuracy and truthfulness over validating the user's beliefs
- Focus on facts and problem-solving
- Provide direct, objective technical info without unnecessary superlatives or praise
- Apply rigorous standards to all ideas and disagree when necessary
- Investigate to find the truth first rather than instinctively confirming beliefs
- Avoid over-the-top validation or excessive praise

## Communication Style

### Tone and Format
- Output will be displayed on a command line interface - be concise
- Only use emojis if the user explicitly requests it
- Use Github-flavored markdown for formatting (monospace font, CommonMark spec)
- Output text to communicate with the user; all text outside of tool use is displayed
- Never use tools like Bash or code comments as means to communicate with the user
- Avoid using over-the-top validation or excessive praise

### One-Page Summaries and Brief Overviews
When the user requests a "one page" or "brief overview":
- Produce a summary containing ONLY the most important information
- **Maximum limit: 80 lines of text**
- **Always include** any information the user has explicitly marked as "important" or "critical"
- Prioritize actionable information, key decisions, and critical deadlines
- Omit supporting details, background context, and explanatory material unless essential
- Use concise bullet points and clear headings for scanability

### Code References
When referencing specific functions or code, include the pattern `file_path:line_number` to allow easy navigation:
```
Example: Clients are marked as failed in the `connectToServer` function in src/services/process.ts:712.
```

## File Operations

### Creating vs. Editing Files
- NEVER create files unless absolutely necessary for achieving your goal
- ALWAYS prefer editing an existing file to creating a new one
- This includes markdown files
- NEVER proactively create documentation files (*.md) or README files unless explicitly requested

### Tool Usage
- Use specialized tools instead of bash commands when possible
- For file operations:
  - Read for reading files (NOT cat/head/tail)
  - Edit for editing (NOT sed/awk)
  - Write for creating files (NOT cat with heredoc or echo redirection)
- Reserve bash tools exclusively for actual system commands and terminal operations
- NEVER use bash echo or command-line tools to communicate with the user

### Hyperlinks and External References

When including hyperlinks to external resources in markdown documents:

**Link Format Requirements:**
- **ALWAYS use HTML anchor format** with `target="_blank"` attribute
- Links must open in a new tab: `<a href="URL" target="_blank">Link Text</a>`
- **NEVER use markdown link format** `[text](URL)` in documentation files

**Link Verification:**
- **Test all URLs before including them** in documents using the WebFetch tool
- If a URL returns 404 or fails to load, search for the correct current URL using WebSearch
- **Do not include broken links** - verify first, include second
- Pay attention to URL changes, redirects, and updated publication references
- Check for superseded publications (e.g., INDG455 replaced by LA455)

**Examples:**

✓ **Correct:**
```html
<a href="https://www.hse.gov.uk/work-at-height/ladders/index.htm" target="_blank">HSE: Safe Use of Ladders</a>
```

✗ **Incorrect:**
```markdown
[HSE: Safe Use of Ladders](https://www.hse.gov.uk/work-at-height/ladders/index.htm)
```

**Verification Process:**
1. Before adding any link, use WebFetch to verify the URL exists and loads correctly
2. If WebFetch returns 404, use WebSearch to find the current correct URL
3. Only after successful verification, add the link in HTML format with `target="_blank"`
4. For government/regulatory links, check if publications have been superseded or updated

**Scope — applies to all documents:** This requirement applies to every document in this repository — briefings, internal reference documents, analysis papers, and any other written output. Every statistic or factual claim drawn from an external source must have a verified HTML anchor link. This is not limited to formal briefings in the `/Briefings/` folder.

## Session Management

### Shared Commands
Common slash commands available across all projects are stored in `/terminai/.claude/commands/`:
- `/end-session` - Document session progress and update context files
- `/sync-session` - Commit and push all session changes to GitHub

### Project-Specific Commands
Each project may have its own additional commands in `<project>/.claude/commands/`

### Project-Specific Agents
Each project has specialized agents in `<project>/.claude/agents/` that are specific to that project's domain.

## Briefings

Standalone intelligence/research briefing papers live in `/docs/terminai/Briefings/`. All future briefings must be created there.

### Briefing Format Standards

Every briefing must follow this structure and format:

1. **Header image** — Always place the Portland Long logo at the very top, before any heading:
   ```html
   <img src="../Portland Long.png" alt="Alt Text" style="width:40%; height:auto;" align="right">
   ```
   *(The path uses `../` because briefings are one level below the root where the image lives.)*

2. **Title and date** — `# Title: Strategic Briefing` followed by an italicised date line.

3. **Horizontal rules** (`---`) between every major section.

4. **Tables** for key statistics wherever possible (cleaner than prose lists).

5. **Sources** — Every section that uses statistics or factual claims must end with a `Sources:` line using verified HTML anchor links with `target="_blank"`. No unverified links.

6. **Closing sources note** — A final italicised `*Sources: ...*` line listing all major references.

### Briefing Creation Blueprint (Process)

When creating a new briefing:

1. **Clarify before researching** — Ask the user: audience/purpose, depth required, any specific angle or concern, and whether any suggested additional sections make sense.
2. **Agree the outline** — Confirm section headings with the user before writing content.
3. **Research first** — Use WebSearch/WebFetch to gather current data; verify all statistics and URLs before inclusion.
4. **Draft section by section** — Write each section, cite sources inline, verify every link with WebFetch.
5. **Place in `/docs/terminai/Briefings/`** — Named `<Topic>_Briefing.md` (e.g. `Canada_Oil_Briefing.md`).
6. **Update the Projects table** in this CLAUDE.md if the briefing introduces a new subject area.

## Important Notes

- When working in a specific project folder, always refer to that project's CLAUDE.md for project-specific guidance
- These shared principles apply universally but may be supplemented by project-specific instructions
- If there's a conflict between shared and project-specific instructions, the project-specific instructions take precedence
