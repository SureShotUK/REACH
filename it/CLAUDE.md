# CLAUDE.md (IT Project)

This file provides IT-specific guidance to Claude Code when working in this project.

> **Note**: This supplements the shared CLAUDE.md at `/terminai/CLAUDE.md`. Read both files for complete guidance.

## Repository Purpose

This repository contains comprehensive technical documentation focused on:
- Virtualization technologies (Type 1/Type 2 hypervisors, VMs, containers)
- Security considerations for virtual environments
- Homelab setup guides and best practices
- Cross-platform development documentation

## Documentation Standards

### Writing Style
- Comprehensive and thorough explanations
- Include practical examples and step-by-step guides
- Provide both theoretical background and hands-on instructions
- Use clear headings and structured formatting
- Include comparison tables where appropriate

### Technical Documentation Requirements
When creating technical guides, include:
- **Overview section**: What it is and why it matters
- **Prerequisites**: Hardware/software requirements with specific versions
- **Step-by-step instructions**: Detailed, numbered steps with commands
- **Configuration examples**: Real, copy-pasteable code/configs
- **Troubleshooting section**: Common issues and solutions
- **Alternatives**: Other approaches or tools when applicable
- **Cost considerations**: Where relevant (hardware, licensing, etc.)
- **Resources**: Links to official docs, communities, tutorials

### Security and Legal Considerations
Always address when relevant:
- Legal implications (license agreements, EULA violations)
- Security risks and mitigations
- Best practices for safe implementation
- Ethical considerations
- Disclaimer when activities are in legal gray areas

### Formatting Guidelines
- Use markdown formatting consistently
- Include code blocks with appropriate syntax highlighting
- Use tables for comparisons
- Include visual separators (---) for major sections
- Use emoji sparingly and only when explicitly requested
- Provide file path references when discussing specific configurations

## Current Repository Structure

```
/it/
├── CLAUDE.md                    # This file
├── virtual_virus_test.md        # VM security and virus isolation guide
├── virtual_machine_types.md     # Overview of VM types and differences
├── type1_hypervisors.md         # Homelab Type 1 hypervisor setup guide
└── mac_on_windows.md            # macOS on Windows for iOS development
```

## User Preferences

Based on previous sessions, the user:
- Prefers detailed, comprehensive documentation over brief summaries
- Values practical, hands-on guides with real commands and configurations
- Appreciates honest assessment of legal/ethical implications
- Is interested in homelab and virtualization technologies
- Wants both theoretical understanding and practical implementation
- Environment: Windows 11 with WSL2 access

## Project-Specific Agents

This project has specialized agents available:
- `windows-virtual-assistant-security` - Specialized assistance for Windows virtualization and security topics
- `gemini-it-security-researcher` - Expert research agent for IT security topics, vulnerabilities, standards, and best practices

## Project-Specific Commands

This project has additional commands:
- `/update-claude` - Update this CLAUDE.md file with session learnings and user preferences
