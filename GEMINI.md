# Gemini CLI Session Parameters

These parameters are to be applied to all sessions to ensure thoroughness, accuracy, and a consistent user experience.

## 1. Research and Verification
- **Thorough Research:** When given an instruction, always research the subject matter comprehensively.
- **Cross-checking:** Verify all information and answers by cross-referencing multiple reliable sources before committing to a final response.
- **Links to references** When a reference is given in response file the link to it should also be given as an HTML hyperlink that opens in a new tab (using target="_blank").

## 2. Information Gathering
- **Ask for Clarification:** If more information is needed to make a better assessment or provide a more accurate response, proactively ask clarifying questions.

## 3. Assumption Verification
- **Read Relevant Files:** Always verify assumptions by reading relevant files and documentation within the project context. Avoid making educated guesses.

## 4. Writing Style and Content
- **Thoroughness:** Adopt a thorough writing style.
- **Theoretical Background:** Provide theoretical background where relevant.
- **Practical Instructions:** Include practical, step-by-step instructions when applicable.

## 5. Formatting and Structure
- **Clear Headings:** Use clear and descriptive headings to structure content.
- **Structured Formatting:** Employ structured formatting for readability.

## 6. Markdown Consistency
- **Consistent Markdown:** Use Markdown formatting consistently throughout all responses.

## 7. Hyperlink Handling
- **Valid Hyperlinks:** Before including any hyperlink, first check that it is valid and accessible.
- **HTML Formatting:** Format valid hyperlinks using HTML `<a>` tags with the `target="_blank"` attribute to ensure they open in a new tab.
  Example: <a href="https://www.example.com" target="_blank">Example Link</a>

---

## Session Highlights: February 17, 2026

This session involved several key software engineering tasks:

### 1. PostgreSQL Security Hardening
- **Objective**: Improve PostgreSQL `pg_hba.conf` access rules for enhanced security.
- **Outcome**: Analyzed existing `pg_hba.conf` and `postgresql.conf`, identified vulnerabilities (e.g., `md5` authentication, broad `0.0.0.0/0` access, `trust` for superuser). Proposed and documented a secure `pg_hba.conf.new` using `scram-sha-256`, `peer` authentication, and `hostssl`.
- **Reference**: `postgres-security/GEMINI.md`, `postgres-security/Postgresql_Security.md`

### 2. PostgreSQL Setup on Windows Subsystem for Linux (WSL2)
- **Objective**: Provide a step-by-step guide for installing and configuring PostgreSQL on WSL2 with access from Windows.
- **Outcome**: Created a detailed guide covering installation, `postgresql.conf` (listen_addresses), user/database creation, `pg_hba.conf` configuration, and Windows client access.
- **Reference**: `wsl-postgresql-setup/PostgreSQL_WSL_Setup_Guide.md`

### 3. Troubleshooting PostgreSQL on WSL2
- **Objective**: Diagnose and resolve PostgreSQL startup issues (`active (exited)` status) and `listen_addresses` configuration for WSL2 access.
- **Outcome**: Identified misleading `sudo service postgresql status` in WSL. Successfully used `pg_ctlcluster` to manage the service. Diagnosed and resolved a `pg_hba.conf` entry mismatch due to a dynamic client IP address from Windows. Documented troubleshooting steps and solutions.
- **Reference**: `wsl-postgresql-setup/WSL_PostgreSQL_Troubleshooting_and_Config.md`

### 4. Gemini CLI Setup Guide
- **Objective**: Create a comprehensive how-to document for setting up and using the Gemini CLI in the terminal.
- **Outcome**: Developed a step-by-step guide covering prerequisites (Node.js, npm), installation (`npm install -g @google/gemini-cli`), authentication (`gemini configure`), basic usage (`gemini generate`, `gemini chat`, `gemini models list`, `gemini skill activate`), important considerations (API key security, rate limits), and further resources.
- **Reference**: `gemini-cli-guide/Gemini_CLI_Setup_Guide.md`

### 5. Troubleshooting Gemini CLI Installation Errors
- **Objective**: Troubleshoot and resolve Gemini CLI installation errors on a user's machine.
- **Outcome**: Identified the root cause of errors like `EBADENGINE` and `EINVALIDTAGNAME` as a Node.js version mismatch (requiring Node.js >=20) and a typo in the package name (`@googlegemini-cli` instead of `@google/gemini-cli`). Guided the user through updating Node.js, clearing npm cache, reinstalling, and correcting the package name typo, leading to a successful installation.
