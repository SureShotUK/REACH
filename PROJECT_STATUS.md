# Project Status

**Last Updated**: 2025-10-31

## Current State
The terminai repository contains multiple specialized project folders for domain-specific knowledge management. Currently focused on the HSEEA (Health, Safety, and Environmental) project, which has been enhanced with custom Claude Code agents for compliance guidance and research.

## Active Work Areas
- **HSEEA Agents**: Three specialized agents now available
  - `hse-compliance-advisor` - Health and safety compliance guidance (opus model)
  - `ea-permit-consultant` - Environment Agency permit assistance (sonnet model)
  - `gemini-hseea-researcher` - Web research for HSE/EA topics (sonnet model) ✨ NEW
- **IT Project**: Contains windows-virtual-assistant-security agent
- **Session Management**: Slash commands configured for /end-session and /sync-session

## Recently Completed
- Created gemini-hseea-researcher agent for specialized UK HSE/EA web research
- Configured agent with 6-step research methodology
- Established research source hierarchy prioritizing official UK government sources
- Set up comprehensive agent description with usage examples

## Blocked/Pending
- None currently

## Next Priorities
1. Test the new gemini-hseea-researcher agent with real research queries
2. Verify agent loads correctly after Claude Code restart
3. Consider documentation updates if agent proves effective

## Key Files & Structure
- `/terminai/CLAUDE.md` - Shared guidance for all projects
- `/terminai/hseea/` - Health, Safety, and Environmental compliance knowledge
  - `/hseea/CLAUDE.md` - HSEEA-specific project guidance
  - `/hseea/.claude/agents/` - Custom agents for HSEEA domain
- `/terminai/it/` - IT infrastructure and security documentation
  - `/it/.claude/agents/` - Custom agents for IT domain
- `/terminai/.claude/commands/` - Shared slash commands across projects
