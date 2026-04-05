# IUCLID Project - Claude Code Instructions

## Project Overview

This project contains technical guidance, procedures, and documentation for IUCLID 6 Cloud software used in UK REACH chemical registration submissions.

**IUCLID Context:**
- Software: IUCLID 6 Cloud (International Uniform ChemicaL Information Database)
- Purpose: Preparing and submitting chemical registration dossiers for UK REACH compliance
- Authority: HSE (Health and Safety Executive) via Comply with UK REACH portal
- Complexity: High - requires technical expertise in regulatory data submission

## Project Objectives

1. **Technical Guidance**: Create step-by-step procedures for IUCLID 6 Cloud operations
2. **Troubleshooting Documentation**: Document common issues and solutions
3. **Process Workflows**: Map out complete submission workflows from data entry to HSE submission
4. **Best Practices**: Capture lessons learned and efficient approaches

## Key Focus Areas

### IUCLID 6 Cloud Operations
- Creating and managing legal entities
- Substance registration workflows
- Inquiry dossier preparation (Article 26 enquiries)
- Joint submission procedures
- Data validation and completeness checks
- Dossier export and submission processes

### Integration Points
- **Comply with UK REACH Portal**: HSE submission system integration
- **REACH Compliance**: Links to parent REACH project documentation
- **Reference Documents**: Connection to existing IUCLID guides in REACH project

## Related Projects

This IUCLID project supports chemical registration work in:
- **REACH Project** (`/terminai/REACH/`) - UK REACH compliance for Urea and HVO
  - Existing guide: `/REACH/IUCLID_Inquiry_Dossier_Guide.md` (550+ lines, comprehensive)
  - HVO consortium registration guidance
  - Urea ATRm monitoring

## Documentation Approach

### Technical Procedures
When creating IUCLID procedures:
- **Screenshot references**: Describe UI elements clearly (e.g., "Main menu → Legal Entities")
- **Step numbering**: Use clear sequential steps with expected outcomes
- **Validation points**: Include checkpoints to verify progress
- **Error handling**: Document common error messages and solutions
- **Prerequisites**: Clearly state what must be completed before each step

### Troubleshooting Guides
When documenting IUCLID issues:
- **Symptom description**: What the user sees/experiences
- **Root cause**: Why the issue occurs
- **Solution steps**: Clear resolution procedure
- **Prevention**: How to avoid the issue in future
- **HSE support escalation**: When to contact ukreachitsupport@defra.gov.uk

### Workflow Diagrams
When mapping IUCLID processes:
- Use text-based flowcharts or clear sequential lists
- Identify decision points and alternative paths
- Mark system transitions (IUCLID → Comply portal)
- Note typical timeframes for each stage

## Information Sources

### Primary Sources
- **IUCLID 6 Cloud**: <a href="https://iuclid6.echa.europa.eu/" target="_blank">ECHA IUCLID Portal</a>
- **HSE UK REACH**: <a href="https://www.hse.gov.uk/reach/" target="_blank">UK REACH Guidance</a>
- **Comply Portal**: <a href="https://comply-chemical-regulations.service.gov.uk/" target="_blank">UK REACH Submission System</a>
- **HSE IT Support**: ukreachitsupport@defra.gov.uk

### Reference Documentation
- `/REACH/IUCLID_Inquiry_Dossier_Guide.md` - Comprehensive Article 26 inquiry guide
- `/REACH/HVO/HVO_Consortium_Registration_Guide.md` - IUCLID workflow for SME consortium registration
- ECHA user manuals and training materials (verify links before using)

## Communication Guidelines

### Technical Accuracy
- **Software versions matter**: Always specify "IUCLID 6 Cloud" (not just "IUCLID")
- **UI element names**: Use exact terminology from the software interface
- **System names**: Distinguish between IUCLID 6 Cloud and Comply with UK REACH portal
- **Accounts**: Clarify which system (IUCLID account vs. Government Gateway account)

### User Skill Levels
Consider different user expertise when writing:
- **Beginners**: May need explanation of chemical regulatory terminology
- **Intermediate**: Understand REACH concepts, need IUCLID technical guidance
- **Advanced**: Need optimization tips and complex scenario handling

### Procedure Formatting
Use this structure for step-by-step procedures:

```markdown
## [Procedure Name]

**Prerequisites:**
- [Required accounts/data/preparation]

**Estimated Time:** [X minutes/hours]

**Steps:**

1. **[Action]**
   - Navigate to: [Location in UI]
   - Expected result: [What should happen]
   - Screenshot reference: [Description if needed]

2. **[Next action]**
   - [Details]
   - ⚠️ **Common issue**: [If applicable]
   - ✓ **Validation**: [How to confirm success]

**Completion Checklist:**
- [ ] [Verification item 1]
- [ ] [Verification item 2]

**If Something Goes Wrong:**
- [Troubleshooting guidance]
- Contact HSE IT Support if [conditions]
```

## Known IUCLID Issues

### Common Problems
Based on existing REACH project experience:

1. **"Create Dossier" Button Unavailable**
   - Cause: Working context not set or legal entity incomplete
   - Solution: Set working context (upper-right) to "REACH" or "UK REACH"
   - Prevention: Always verify legal entity sections 1.1, 1.2, 1.4 complete

2. **Validation Failures**
   - Use Validation Assistant to identify issues
   - Check mandatory fields completion
   - Verify data format requirements

3. **Export/Submission Errors**
   - Run validation before export
   - Check file size limits
   - Confirm correct IUCLID format for UK REACH (not EU REACH)

## Document Organization

Organize IUCLID documentation into:
- `/procedures/` - Step-by-step operational guides
- `/troubleshooting/` - Issue resolution guides
- `/workflows/` - End-to-end process documentation
- `/templates/` - Reusable checklists and data templates
- `/reference/` - Quick reference guides and lookup tables

## Important Notes

- **Two-System Architecture**: IUCLID 6 Cloud is for dossier preparation; Comply with UK REACH portal is for submission to HSE. These are separate systems requiring separate accounts.
- **Data Validation**: Always run IUCLID Validation Assistant before attempting export/submission
- **Save Frequently**: IUCLID 6 Cloud is web-based; connection issues can cause data loss
- **HSE IT Support**: For technical IUCLID/portal issues, contact ukreachitsupport@defra.gov.uk (24-hour response time)
- **Legal Entity First**: Must create and complete legal entity profile before creating dossiers

## Cross-References

When IUCLID guidance relates to broader compliance topics, reference:
- REACH project compliance strategies
- Substance-specific registration guides (Urea, HVO)
- Consortium membership procedures
- Article 26 enquiry processes

Maintain links between IUCLID technical procedures and business compliance decisions.
