# Changelog

All notable changes to the Nebosh NEBOSH National General Certificate study project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased] - 2025-11-17

### Added
- **Project Infrastructure**
  - Created complete Nebosh project folder structure (study-notes, assessments, reference)
  - Added specialized `gemini-nebosh-researcher` agent for NEBOSH-specific research
  - Created project-specific CLAUDE.md with NGC study guidance
  - Added README.md with project overview and study approach
  - Created SESSION_LOG.md for session tracking
  - Created PROJECT_STATUS.md for progress tracking
  - Created CHANGELOG.md for version tracking

- **Study Materials**
  - **LearningOutcomes.md** - Consolidated table of all NGC learning outcomes (Elements 1-9) extracted from NEBOSH PDF
  - **CadburyHistory.md** (35KB) - Comprehensive historical analysis of Cadbury's Bournville factory and village, Joseph Rowntree's welfare programs, Quaker business principles, and connection to modern H&S moral arguments (Element 1)
  - **Maslow.md** (45KB) - In-depth exploration of Maslow's hierarchy of needs theory applied to workplace health and safety, impact of poor H&S standards on employee behavior, connection to NEBOSH Element 3 human factors

### Documentation
- **CadburyHistory.md** includes:
  - Background on Cadbury brothers and move to Bournville (1879)
  - Joseph Rowntree's comprehensive welfare programs (1869-1925)
  - Quaker values and revolutionary working conditions
  - Timeline of innovations (8-hour day, pensions, healthcare, model villages)
  - Comparison with Victorian factory norms
  - Connection to HASAWA 1974 and modern H&S legislation
  - Full academic references and sources
  - Direct application to NEBOSH Element 1 moral arguments

- **Maslow.md** includes:
  - Complete theory overview (Abraham Maslow, 1943)
  - Detailed explanation of all five need levels
  - Workplace-specific applications
  - Impact of poor H&S standards on each need level
  - Resulting employee behaviors (rushing, normalization of risk, silence, disengagement)
  - Connection to NEBOSH Element 3 human factors
  - Three practical case studies (manufacturing, construction, healthcare)
  - Critical perspective including limitations and alternative theories
  - Application to safety culture development

- **gemini-nebosh-researcher** agent configured for:
  - Official NEBOSH resources (syllabus, examiner reports, specimen papers)
  - UK H&S regulations and HSE guidance documents
  - NGC exam technique and command words
  - NG2 risk assessment preparation
  - Study resources and practice materials

### Infrastructure
- Aligned project structure with existing terminai repository standards
- Created specialized agent following hseea project pattern
- Established session and progress tracking files
- Configured for UK-specific NEBOSH NGC content

## Notes

**Current Coverage**:
- Element 1 (Why We Should Manage H&S): Moral reasons comprehensively covered
- Element 3 (Managing Risk - People & Processes): Human factors and behavior covered

**Next Development Areas**:
- Element 2: H&S Management Systems (HSG65, PDCA)
- Element 4: Risk Assessment and Monitoring
- Elements 5-11: Specific hazards for NG2 practical
- Practice questions and exam technique
- NG2 risk assessment templates
