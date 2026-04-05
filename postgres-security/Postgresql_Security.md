# PostgreSQL Security Configuration: Best Practices

This document outlines key concepts for securing a PostgreSQL database and compares an existing configuration with a best-practice approach, focusing on client authentication and connection security.

## 1. Concepts of Configuring a Secure PostgreSQL Setup

A secure PostgreSQL setup hinges on several fundamental principles, ensuring that only authorized users can access data, that data in transit is protected, and that the attack surface is minimized.

### 1.1. Principle of Least Privilege
This core security concept dictates that users, processes, and programs should be granted only the minimum necessary permissions to perform their intended function. For PostgreSQL, this means:
*   Granting specific database and table permissions to users, rather than broad `ALL PRIVILEGES`.
*   Creating distinct database users for different applications or roles, each with limited access.
*   Restricting superuser (`postgres`) access to only when absolutely necessary, and preferably only from highly secured, local connections.

### 1.2. Encrypted Connections (SSL/TLS)
Protecting data as it travels between the client and the PostgreSQL server is paramount. SSL/TLS (Secure Sockets Layer/Transport Layer Security) encryption ensures that communications cannot be intercepted or tampered with.
*   **`ssl=on` in `postgresql.conf`**: Enables SSL support on the server.
*   **`hostssl` in `pg_hba.conf`**: Explicitly requires SSL for specified connections. This is crucial as `host` rules allow both SSL and non-SSL connections, potentially exposing unencrypted traffic.
*   **Trusted Certificates**: For production, use certificates issued by a trusted Certificate Authority (CA) and ensure clients verify server certificates to prevent man-in-the-middle attacks.

### 1.3. Strong Authentication Mechanisms
Choosing robust authentication methods is critical to prevent unauthorized access. PostgreSQL offers several options, with varying levels of security:
*   **`trust`**: **Highly Insecure**. Allows connections without a password. Should almost never be used, especially for remote access.
*   **`password`**: **Insecure**. Sends passwords in cleartext. Do not use.
*   **`md5`**: **Deprecated/Weak**. Sends hashed passwords but is vulnerable to dictionary attacks and rainbow tables. Avoid if stronger options are available.
*   **`scram-sha-256`**: **Recommended**. A strong, modern, challenge-response authentication mechanism that prevents password sniffing and brute-force attacks. It is the preferred password-based authentication method.
*   **`peer`**: **Recommended for local Unix socket connections**. Authenticates clients based on their operating system user name. Very secure for local access as it relies on OS-level user management.
*   **`ident`**: Similar to `peer` but for TCP/IP connections, relying on an ident server. Less commonly used today.
*   **`cert`**: Authenticates clients using SSL client certificates. Provides very strong authentication but requires more setup and management.

### 1.4. Network Access Control
Limiting where connections can originate from is a primary defense.
*   **`listen_addresses` in `postgresql.conf`**: Controls which network interfaces PostgreSQL listens on.
    *   `localhost`: Only allows connections from the same machine.
    *   `*`: Listens on all available interfaces. If `*` is used, stringent `pg_hba.conf` rules are absolutely critical.
    *   Specific IP addresses: Listens only on specified interfaces.
*   **`ADDRESS` in `pg_hba.conf`**: Defines the allowed source IP addresses or networks for a given rule.
    *   `0.0.0.0/0` or `::/0`: **Highly Insecure** for broad access, meaning "any IP address." Restrict these to specific client IPs (`192.168.1.10/32`) or tightly controlled subnets (`10.0.0.0/24`).

### 1.5. Database and User Specificity
Avoid blanket `all` rules in `pg_hba.conf` wherever possible. Define rules that apply only to specific databases and specific users to adhere to the principle of least privilege.

### 1.6. Regular Auditing and Logging
While not directly part of `pg_hba.conf`, configuring robust logging in `postgresql.conf` (`log_connections`, `log_disconnections`, `log_min_messages`, etc.) and regularly reviewing these logs is vital for detecting and responding to security incidents.

### 1.7. Server Hardening
Beyond PostgreSQL configuration, the underlying operating system and network infrastructure must also be secured (e.g., firewall rules, timely OS patches, secure file permissions for PostgreSQL data directory and configuration files).

## 2. Comparison: Original vs. Best Practice Configuration

Let's compare the provided original configuration files (`pg_hba.conf` and `postgresql.conf`) against best practice recommendations.

### 2.1. `pg_hba.conf` Differences

The `pg_hba.conf` file controls client authentication and access. The original configuration contained several significant security vulnerabilities.

| Type | Database | User | Address | Method | Options | Original Configuration Notes | Best Practice Configuration (`pg_hba.conf.new`) | Security Improvement |
|---|---|---|---|---|---|---|---|---|
| `local` | `all` | `postgres` | | `trust` | | **Insecure:** Allows password-less superuser access via Unix sockets. | `local all all peer` <br> `local postgres postgres peer` | Replaced `trust` with `peer` authentication. `peer` maps the OS user to the database user, ensuring OS-level authentication is required. This adheres to strong authentication and least privilege. |
| `host` | `all` | `all` | `0.0.0.0/0` | `md5` | | **Highly Insecure:** Allows connections from *any* IPv4 address (`0.0.0.0/0`) to *any* database for *any* user using weak `md5` authentication. No encryption. | `hostssl all all 0.0.0.0/0 scram-sha-256` <br> *(**WARNING**: `0.0.0.0/0` should be replaced with specific client IPs/subnets in production.)* | Enforces SSL encryption (`hostssl`) and upgrades authentication to `scram-sha-256`. This protects data in transit and uses a much stronger password hashing algorithm, preventing password sniffing and making brute-force attacks significantly harder. Adheres to encrypted connections, strong authentication, and moves towards network access control (with the warning). |
| `host` | `all` | `all` | `::/0` | `md5` | | **Highly Insecure:** Similar to the IPv4 rule, allows connections from *any* IPv6 address (`::/0`) with weak `md5` authentication. No encryption. | `hostssl all all ::/0 scram-sha-256` <br> *(**WARNING**: `::/0` should be replaced with specific client IPs/subnets in production.)* | Same security improvements as for IPv4 connections, applied to IPv6. |
| `local` | `replication` | `all` | | `peer` | | (Already good) | `local replication all peer` | This rule is already a good practice for local replication. |
| `host` | `replication` | `all` | `127.0.0.1/32` | `scram-sha-256` | | (Already good) | `host replication all 127.0.0.1/32 scram-sha-256` | This rule is already a good practice for local replication via TCP/IP. |
| `host` | `replication` | `all` | `::1/128` | `scram-sha-256` | | (Already good) | `host replication all ::1/128 scram-sha-256` | This rule is already a good practice for local replication via TCP/IP (IPv6). |
| `host` | `replication` | `all` | `20.254.117.127/32` | `scram-sha-256` | | (Already good) | `host replication all 20.254.117.127/32 scram-sha-256` | This rule is already a good practice, using specific IP and strong authentication for remote replication. |

### 2.2. `postgresql.conf` Differences/Recommendations

The `postgresql.conf` file contains general server settings. While the original provided a good base for SSL, some areas can be enhanced.

| Parameter | Original Configuration | Best Practice Recommendation | Security Improvement |
|---|---|---|---|
| `listen_addresses` | `*` | Keep `*` if remote access is required. If database access should be restricted to the local machine only, change to `localhost`. | If `*` is necessary, it *must* be combined with highly restrictive `pg_hba.conf` rules. Changing to `localhost` eliminates external network exposure. |
| `ssl` | `on` | Keep `on`. | Ensures SSL/TLS encryption is enabled server-side, a foundational step for secure connections. |
| `ssl_cert_file`<br>`ssl_key_file` | `/etc/ssl/certs/ssl-cert-snakeoil.pem`<br>`/etc/ssl/private/ssl-cert-snakeoil.key` | In production environments, replace "snakeoil" (self-signed or generic) certificates with certificates issued by a trusted Certificate Authority (CA) and ensure proper permissions (`root` only for key file). | Trusted certificates allow clients to verify the server's identity, preventing man-in-the-middle attacks. |
| `ssl_min_protocol_version` | (Commented out/Default) | Uncomment and set to `'TLSv1.2'` or `'TLSv1.3'` to ensure only modern, secure SSL/TLS protocols are used. | Prevents the use of older, vulnerable TLS protocol versions, enhancing connection security. |
| `password_encryption` | (Commented out/Default) | Ensure `scram-sha-256` is enabled implicitly by `pg_hba.conf` rules, or explicitly set `password_encryption = scram-sha-256` if managing passwords via SQL. | Ensures that new passwords are hashed using the strongest available algorithm. |

## 3. Implementation Steps and Verification

To apply these security enhancements, follow these critical steps:

1.  **Backup Existing Configuration**: Always make a backup of your current `pg_hba.conf` and `postgresql.conf` files before making any changes.
2.  **Update `postgresql.conf`**:
    *   Modify `listen_addresses` if needed (e.g., to `localhost`).
    *   Replace snakeoil SSL certificates with trusted, CA-signed certificates (and update `ssl_cert_file`, `ssl_key_file`).
    *   Uncomment and set `ssl_min_protocol_version = 'TLSv1.2'` (or higher).
    *   (Optional) Uncomment `password_encryption = scram-sha-256`.
3.  **Replace `pg_hba.conf`**: Replace your current `pg_hba.conf` file with the contents of the best-practice `pg_hba.conf.new` file. Remember to customize the `ADDRESS` fields (`0.0.0.0/0`, `::/0`) to reflect your specific client IP ranges for production.
4.  **Reload PostgreSQL Configuration**:
    *   After modifying the configuration files, instruct PostgreSQL to reload them.
    *   You can do this by running `pg_ctl reload` (as the PostgreSQL user) or by sending a `SIGHUP` signal to the PostgreSQL master process, or by executing `SELECT pg_reload_conf();` within a PostgreSQL client.
5.  **Update User Passwords**:
    *   **This is a critical step**: If you change the authentication method in `pg_hba.conf` to `scram-sha-256`, existing users whose passwords were created using `md5` will no longer be able to connect.
    *   You **must** update the passwords for all relevant database users (e.g., `ALTER USER your_user PASSWORD 'new_strong_password';`). PostgreSQL will then store these passwords using SCRAM-SHA-256.
6.  **Test Thoroughly**:
    *   After applying changes and updating passwords, test all client connections (applications, replication, administration tools) to ensure they can connect correctly and securely.
    *   Verify that non-authorized connections are indeed rejected.

By following these steps, you will significantly enhance the security posture of your PostgreSQL database, protecting it against common vulnerabilities related to authentication and access.

---
**References:**

*   PostgreSQL Documentation on `pg_hba.conf`: <a href="https://www.postgresql.org/docs/current/auth-pg-hba-conf.html" target="_blank">Client Authentication</a>
*   PostgreSQL Documentation on SSL Support: <a href="https://www.postgresql.org/docs/current/runtime-config-connection.html#RUNTIME-CONFIG-CONNECTION-SSL" target="_blank">SSL Support</a>
*   PostgreSQL Documentation on `postgresql.conf`: <a href="https://www.postgresql.org/docs/current/runtime-config.html" target="_blank">Server Configuration</a>
