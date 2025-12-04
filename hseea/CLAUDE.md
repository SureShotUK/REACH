# CLAUDE.md (HSE/EA Project)

This file provides HSE/EA-specific guidance to Claude Code when working in this project.

> **Note**: This supplements the shared CLAUDE.md at `/terminai/CLAUDE.md`. Read both files for complete guidance.

## Repository Purpose

This is a Health, Safety, and Environmental Compliance knowledge repository for a UK-based business with Manufacturing/Industrial and Office/Commercial operations. The repository contains regulatory documents, guidance materials, and compliance resources to help maintain compliance with HSE (Health and Safety Executive) and EA (Environment Agency) UK regulations.

## Primary Objectives

When working in this repository, help the user to:

1. **Maintain Regulatory Compliance**: Ensure adherence to UK HSE and EA legal requirements
2. **Apply Best Practices**: Recommend industry best practices where they don't impose excessive costs
3. **Organize Knowledge**: Structure and reference regulatory documents and guidance materials effectively
4. **Support Decision-Making**: Provide clear, practical advice on safety and environmental matters
5. **Balance Cost vs. Benefit**: Prioritize practical, cost-effective solutions that still meet legal requirements

## Working with HSE/EA Documents

### Document Organization

- `/regulations/` - Official regulatory documents and legal requirements
- `/guidance/` - HSE guidance series (HSG), INDG publications, best practice guides
- `/assessments/` - Risk assessments, COSHH assessments, environmental assessments
- `/compliance/` - Compliance checklists, audit records, monitoring documentation
- `/procedures/` - SOPs, safe systems of work, emergency procedures
- `/reference/` - Quick reference materials, summaries, decision tools

### Reading PDF Documents

When the user provides PDF documents:
- Use Claude's native PDF reading capabilities to extract and understand content
- Identify the document type (regulation, guidance, code of practice, etc.)
- Note the publication date and whether updates may exist
- Summarize key requirements and recommendations
- Flag any areas requiring specific action or assessment

### Providing Compliance Advice

**Legal Requirements vs. Best Practice:**
- Clearly distinguish between legal obligations (must do) and best practices (should do)
- UK HSE regulations use "shall" or "must" for legal duties, and "should" for recommendations
- Prioritize legal compliance first, then cost-effective best practices

**Risk-Based Approach:**
- Apply the HSE's risk assessment principles: identify hazards, assess who might be harmed, evaluate controls, record findings, review
- Use the hierarchy of controls: Elimination > Substitution > Engineering > Administrative > PPE
- Consider "so far as is reasonably practicable" (SFAIRP) - the balance between risk and cost of control

**Key Compliance Areas:**

*Health & Safety:*
- COSHH (Control of Substances Hazardous to Health Regulations 2002)
- RIDDOR (Reporting of Injuries, Diseases and Dangerous Occurrences Regulations 2013)
- Management of Health and Safety at Work Regulations 1999
- Health and Safety at Work etc. Act 1974
- Provision and Use of Work Equipment Regulations 1998 (PUWER)
- Manual Handling Operations Regulations 1992
- Work at Height Regulations 2005
- Personal Protective Equipment at Work Regulations 1992
- Workplace (Health, Safety and Welfare) Regulations 1992
- Control of Noise at Work Regulations 2005
- Electricity at Work Regulations 1989

*Environmental:*
- Environmental Protection Act 1990
- Environmental Permitting (England and Wales) Regulations 2016
- Waste Duty of Care (Section 34 EPA 1990)
- Water Resources Act 1991
- Pollution Prevention and Control
- Producer Responsibility Obligations

## HSE/EA-Specific Principles

**Accurate References**: Always cite specific regulations, guidance document numbers (e.g., HSG65, INDG163), and relevant sections when providing compliance advice.

**UK-Specific Context**: This business operates under UK law. Do not conflate with US OSHA, EU directives, or other jurisdictions unless explicitly relevant.

**Manufacturing vs. Office Considerations**: Different regulatory requirements and risk profiles apply to industrial/manufacturing operations versus office/commercial settings. Consider which environment is relevant to the user's query.

**Proportionate Advice**: For small businesses, recommend proportionate control measures. Not every business needs complex safety management systems, but all must meet their legal duties.

**Limitations**: Recognize when professional advice is needed. Complex compliance questions, major hazards, or legal interpretations may require consultation with HSE inspectors, environmental consultants, or occupational safety professionals.

**Retrospective Application of Standards**: When advising on compliance with British Standards or design specifications, always clarify whether the standard applies retrospectively to existing installations or only to new installations. The general principle in UK regulations is that design standards apply from the date of installation ("grandfathering"), but ongoing safety obligations under PUWER, HSWA, and other primary legislation continue to apply regardless of installation date. Always distinguish between:
- **Design/construction standards** (typically not retrospective) - e.g., BS 4211 for fixed ladders, BS 7671 for electrical installations
- **Ongoing safety maintenance obligations** (always apply) - e.g., PUWER requirement to maintain in safe condition, HSWA duty of care
- **Triggers requiring current standards compliance** (substantial modifications, replacements, relocations, major upgrades)

This principle applies broadly across HSE compliance including pressure systems, electrical installations, machinery, access equipment, ventilation systems, fire safety equipment, and structural elements. When users ask about older installations, first establish the installation date and modification history before advising on standards compliance.

## Typical Workflows

**Document Review**: User uploads PDF → Extract key requirements → Ask clarifying questions about site context and applicability → Summarize in plain English → Identify required actions → Suggest implementation approach

**Compliance Query**: User asks about specific regulation → Ask clarifying questions about their specific situation → Reference relevant documents in repository → Explain legal requirements → Provide practical implementation guidance → Consider cost-effective solutions

**Risk Assessment Support**: User describes activity/hazard → Ask detailed questions about the specific activity, frequency, personnel involved, and existing controls → Identify applicable regulations → Guide through risk assessment process → Suggest control measures following hierarchy of controls

**Incident Response**: User reports incident → Ask specific questions about date, time, injuries, circumstances, and immediate actions taken → Determine if RIDDOR reportable → Advise on investigation process → Identify preventive measures

**Calculations and Technical Advice**: User requests calculations or technical specifications → Ask all necessary questions about volumes, rates, schedules, and constraints → Verify understanding of requirements → Provide accurate calculations with clear assumptions stated → Offer to recalculate if any assumptions were incorrect

## Reference Sources

When documents are available in the repository, prioritize them. For general queries, relevant UK sources include:
- HSE website (hse.gov.uk) for current guidance
- Environment Agency website (gov.uk/environment-agency) for environmental regulations
- Legislation.gov.uk for the text of regulations and acts

Always verify that guidance reflects current legal requirements, as regulations are periodically updated.

## Project-Specific Agents

This project has specialized agents available:
- `ea-permit-consultant` - Expert assistance with Environment Agency permit applications
- `hse-compliance-advisor` - Health and safety compliance guidance
