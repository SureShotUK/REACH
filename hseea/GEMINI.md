# GEMINI.md - Project Context and Operational Guidelines

This document provides foundational context and operational guidelines for Gemini CLI's interaction with the current repository. It synthesizes information from various project documents to ensure a consistent, effective, and compliant approach to Health, Safety, and Environmental (HSE) management.

## 1. Project Overview

This repository serves as a comprehensive Health, Safety, and Environmental Compliance knowledge base for a UK-based business, **Noxdown Limited**, which operates both **industrial/manufacturing (including AdBlue production)** and **office/commercial** facilities in Pocklington. Its primary purpose is to store, organize, and manage regulatory guidance, best practice documents, and compliance resources to ensure adherence to UK HSE (Health and Safety Executive) and EA (Environment Agency) regulations. The project aims to balance regulatory compliance with practical, cost-effective solutions.

**Key characteristics:**
*   **Jurisdiction:** United Kingdom (primarily England and Wales).
*   **Operations:** Mixed Industrial (AdBlue production, warehousing) and Office/Commercial.
*   **Goal:** Maintain compliance with UK HSE and EA regulations while following cost-effective best practices.

## 2. Key Files and Directories

The repository is structured to categorize various HSE/EA-related documentation:

*   **`/regulations/`**: Contains official HSE and EA regulatory documents, guidance notes, and legal requirements.
*   **`/guidance/`**: Holds industry best practice guides, HSE guidance documents (HSG series), and INDG publications.
*   **`/assessments/`**: Stores risk assessments, COSHH assessments, environmental impact assessments, and related templates.
    *   `Risk_Assessments/GEMINI.md`: Provides specific guidelines for Gemini related to risk assessment best practices, including HSE's Five Steps and a best practice checklist.
    *   `Risk_Assessments/RA_Template.csv`: A structured template for recording risk assessments.
    *   `Risk_Assessments/Hierarchy_Of_Control.md`: Details the five levels of hazard control.
    *   `Risk_Assessments/Risk_Rating_Metrics.csv`: Standardized 1-5 scoring system for Likelihood and Severity.
*   **`/compliance/`**: Includes compliance checklists, audit records, and monitoring documentation.
*   **`/procedures/`**: Contains Standard Operating Procedures (SOPs), safe systems of work, and emergency response procedures.
*   **`/reference/`**: Provides quick reference materials, legislation summaries, and decision-making tools.
*   **`/Ladders/`**: A dedicated sub-project for comprehensive fixed ladder compliance documentation.
*   **`/Legionella/`**: Contains specific documentation related to Legionella control.
    *   `Legionella/Legionella_Action_Plan.md`: Details procedures for responding to a Legionella outbreak.
    *   `Legionella/Legionella_Prevention.md`: Outlines best practices and regulatory requirements for Legionella prevention, specifically addressing the site's AdBlue production.
    *   `Legionella/Legionella_Symptoms.md`: Lists common sources and symptoms of Legionnaires' disease.
*   **`/water/`**: Documents related to water discharge and treatment planning (e.g., EA permit applications).
*   **`README.md`**: The top-level project overview (this file provides more detailed context).
*   **`PROJECT_STATUS.md`**: Provides a detailed overview of the project's current state, completed tasks, active work areas, and next priorities.
*   **`CLAUDE.md`**: Contains HSE/EA-specific guidance for AI agents, including how to handle PDF documents, provide compliance advice, and format references.
*   **`SESSION_LOG.md`**: A chronological record of all work sessions.

## 3. Operational Guidelines for Gemini CLI

When interacting with this repository, Gemini CLI should adhere to the following principles:

### 3.1 Regulatory Compliance and Best Practices
*   **Prioritize UK Law:** Always distinguish between legal obligations ("must do") and best practices ("should do") based on UK HSE and EA regulations.
*   **Risk-Based Approach:** Apply HSE's five steps to risk assessment and the hierarchy of controls (Elimination > Substitution > Engineering > Administrative > PPE). Consider "so far as is reasonably practicable" (SFAIRP).
*   **Proportionate Advice:** Provide advice that is proportionate to the business size and risk profile.

### 3.2 Document Handling and Referencing
*   **Accurate Citations:** When providing compliance advice, always cite specific regulations, guidance document numbers (e.g., HSG65, INDG163), and relevant sections.
*   **HSE Guidance Paragraphs:** Exercise caution when citing HSE guidance. Cite as `[Document, Section]` or `[Document, p.X]` if no specific section is available. Verify that the cited text appears in the numbered paragraph attributed to it.
*   **Regulation Hyperlinks:** Convert all references to UK regulations (Acts of Parliament, Statutory Instruments) into HTML hyperlinks to `legislation.gov.uk` using `target="_blank"`. Always verify the URL before insertion.
*   **UK-Specific Context:** Avoid conflating UK regulations with those from other jurisdictions.
*   **Manufacturing vs. Office:** Be aware that different regulatory requirements and risk profiles apply to industrial vs. office settings.

### 3.3 AI Agent Interaction
*   This repository leverages AI agents (e.g., Claude) for specific tasks. Refer to `CLAUDE.md` for detailed guidance on how AI agents should interact with project content, including PDF reading, compliance advice formatting, and referencing standards. Gemini should align with these interaction patterns where applicable.
*   The project also maintains a multi-project structure (`/terminai/` parent directory) with shared context files and commands.

### 3.4 Session Highlights for Gemini

This section will be periodically updated to summarize significant interactions and developments involving Gemini, providing continuity for future sessions.

#### February 23, 2026
*   **Legionella Prevention Document:** Created `./Legionella/Legionella_Prevention.md`, outlining best practices and regulatory requirements for Legionella control specific to the site's AdBlue production, offices, and welfare units.
*   **Legionnaires' Disease Symptoms Document:** Created `./Legionella/Legionella_Symptoms.md`, listing common sources of Legionnaires' disease in a mixed-use site and the symptoms an infected person might exhibit.

---
**Note:** This `GEMINI.md` file will evolve as the project develops and as new instructions or insights emerge from interactions with Gemini CLI.
