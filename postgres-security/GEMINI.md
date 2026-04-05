# PostgreSQL Security Hardening: Access Rules (pg_hba.conf)

## Project Goal
To enhance the security of PostgreSQL access rules by implementing best practices for client authentication, focusing on the `pg_hba.conf` configuration file. This project aims to minimize potential attack vectors and ensure secure connections to the PostgreSQL database.

## Key Areas of Focus
1.  **`pg_hba.conf` Analysis:** Review and understand the structure and implications of current (or default) `pg_hba.conf` entries.
2.  **Authentication Methods:** Evaluate and recommend secure authentication methods (e.g., `scram-sha-256`, `cert`, `peer`) over less secure ones (`trust`, `md5`).
3.  **Connection Security:** Prioritize encrypted connections using SSL/TLS.
4.  **Least Privilege:** Implement access rules based on the principle of least privilege, ensuring users and applications only have the necessary access.
5.  **Network Restrictions:** Configure host-based access to restrict connections from untrusted networks or hosts.

## Core Principles
*   **Principle of Least Privilege:** Granting only the minimum necessary permissions to users and roles.
*   **Encrypted Connections:** Enforcing SSL/TLS for all or most connections to protect data in transit.
*   **Strong Authentication:** Utilizing robust authentication mechanisms like SCRAM-SHA-256 or client certificates.
*   **Explicit Deny:** Defaulting to deny access and explicitly allowing only what is required.

## Action Plan
1.  **Understand `pg_hba.conf`:**
    *   Familiarize with the format and options of the `pg_hba.conf` file.
    *   Reference: <a href="https://www.postgresql.org/docs/current/auth-pg-hba-conf.html" target="_blank">PostgreSQL Documentation on pg_hba.conf</a>
2.  **Review Existing Configuration (or Template):**
    *   Examine a typical `pg_hba.conf` file to identify common patterns and potential weaknesses.
3.  **Identify Insecure Practices:**
    *   Rules using `trust` authentication.
    *   Rules allowing `all` databases or `all` users from broad IP ranges (`0.0.0.0/0`, `::/0`).
    *   Use of `md5` authentication when stronger methods are available.
4.  **Propose Secure Alternatives:**
    *   **Authentication:**
        *   Replace `trust` with `scram-sha-256` or `cert` (for client certificate authentication).
        *   Migrate `md5` to `scram-sha-256`.
        *   Use `peer` or `ident` for local connections where appropriate.
    *   **Host Restrictions:**
        *   Specify exact IP addresses or narrow IP ranges (`192.168.1.10/32`, `10.0.0.0/8`) instead of broad ranges.
        *   Leverage hostnames if DNS resolution is reliable and secure.
    *   **Database/User Specificity:**
        *   Define rules for specific databases and users rather than `all`.
    *   **SSL/TLS Enforcement:**
        *   Add `hostssl` entries and configure `ssl=on` in `postgresql.conf`.
        *   Reference: <a href="https://www.postgresql.org/docs/current/runtime-config-connection.html#RUNTIME-CONFIG-CONNECTION-SSL" target="_blank">PostgreSQL SSL Support Documentation</a>
5.  **Provide Examples:**
    *   Illustrate "good" and "bad" `pg_hba.conf` entries with explanations.

## Deliverables
*   This `GEMINI.md` file, detailing the project scope and plan.
*   An example `pg_hba.conf` configuration demonstrating secure access rules.
*   (Optional) A script to validate or apply the new configuration.

## Next Steps
Proceed with a detailed analysis of a sample `pg_hba.conf` file, then generate a proposed secure configuration.