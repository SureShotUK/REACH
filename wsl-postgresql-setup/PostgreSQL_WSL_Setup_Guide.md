# Setting up PostgreSQL on Windows Subsystem for Linux (WSL2) and Configuring Windows Access

This guide provides step-by-step instructions for installing PostgreSQL on WSL2, creating a new user and database, and configuring access from your Windows environment.

## 1. Prerequisites

*   **Windows 10/11 with WSL2 installed and configured**: Ensure you have WSL2 enabled and a Linux distribution (e.g., Ubuntu) installed.
    *   <a href="https://learn.microsoft.com/en-us/windows/wsl/install" target="_blank">Microsoft Docs: Install WSL</a>
*   **Basic familiarity with Linux command line**: This guide assumes you can execute commands in your WSL terminal.

## 2. Install PostgreSQL on WSL2

Open your WSL terminal (e.g., Ubuntu) to perform these steps.

1.  **Update package lists**:
    ```bash
    sudo apt update
    ```
2.  **Install PostgreSQL and contrib modules**:
    ```bash
    sudo apt install postgresql postgresql-contrib
    ```
    This command installs the PostgreSQL server, client utilities, and commonly used modules.
3.  **Check PostgreSQL service status**:
    ```bash
    sudo service postgresql status
    ```
    You should see output indicating that the service is `active (exited)` or `active (running)`. If it's not running, you can start it.
4.  **Basic service control commands**:
    *   To start PostgreSQL: `sudo service postgresql start`
    *   To stop PostgreSQL: `sudo service postgresql stop`
    *   To restart PostgreSQL: `sudo service postgresql restart`

## 3. Configure PostgreSQL for Network Access within WSL

By default, PostgreSQL might only listen for connections on `localhost` (127.0.0.1) within your WSL environment. To allow connections from your Windows host, you need to configure PostgreSQL to listen on all available network interfaces or a specific IP address.

1.  **Edit the `postgresql.conf` file**:
    Locate the `postgresql.conf` file. The exact path may vary slightly by PostgreSQL version, but it's typically `/etc/postgresql/<version>/main/postgresql.conf`. You can find your version by checking the directory structure or looking for the `data_directory` in the output of `psql -c 'SHOW data_directory;'`.
    ```bash
    sudo nano /etc/postgresql/$(pg_lsclusters -h | awk 'NR>1 {print $1}')/main/postgresql.conf
    ```
    (Replace `$(pg_lsclusters -h | awk 'NR>1 {print $1}')` with your actual version if the command doesn't work or returns multiple versions, e.g., `16`)

    Find the line `listen_addresses = 'localhost'` (it might be commented out with `#`). Change it to:
    ```
    listen_addresses = '*'
    ```
    This tells PostgreSQL to listen on all available network interfaces. Save the file (Ctrl+O, Enter) and exit nano (Ctrl+X).
2.  **Restart PostgreSQL for changes to take effect**:
    ```bash
    sudo service postgresql restart
    ```

## 4. Create a New Database User and Database

By default, PostgreSQL creates a `postgres` user in the database that corresponds to the `postgres` operating system user. We'll create a new user and database for your application.

1.  **Switch to the `postgres` Linux user**:
    ```bash
    sudo -i -u postgres
    ```
    This command switches you to the `postgres` user account on your WSL Linux system, which has administrative privileges for PostgreSQL.
2.  **Access the PostgreSQL command-line interface (psql)**:
    ```bash
    psql
    ```
3.  **Create a new database user**:
    ```sql
    CREATE USER myuser WITH PASSWORD 'mysecurepassword';
    ```
    *   Replace `myuser` with your desired username.
    *   Replace `mysecurepassword` with a strong, unique password.
    *   Remember to end all SQL commands with a semicolon `;`.
4.  **Create a new database and assign ownership**:
    ```sql
    CREATE DATABASE mydatabase OWNER myuser;
    ```
    *   Replace `mydatabase` with your desired database name.
    *   The `OWNER myuser` clause makes the `myuser` the owner of this new database.
5.  **Grant all privileges on the database to the new user (optional, but common for application users)**:
    ```sql
    GRANT ALL PRIVILEGES ON DATABASE mydatabase TO myuser;
    ```
    This allows `myuser` full control over `mydatabase`. For production, you might want to grant more specific privileges.
6.  **Exit psql**:
    ```sql
    \q
    ```
7.  **Exit the `postgres` Linux user session**:
    ```bash
    exit
    ```

## 5. Configure `pg_hba.conf` for Windows Access

The `pg_hba.conf` file controls client authentication. You need to add a rule that allows your Windows machine to connect to the PostgreSQL instance in WSL.

1.  **Get your WSL Linux distribution's IP address**:
    Open a *new WSL terminal window* (do not close your current one where you're configuring PostgreSQL). Run the following command to find the IP address that Windows uses to communicate with your WSL instance:
    ```bash
    ip addr show eth0 | grep inet | awk '{ print $2 }' | cut -d/ -f1
    ```
    This will output an IP address (e.g., `172.20.128.3`). Note this down; it's your `<WSL_IP_ADDRESS>`. This IP address can change after restarting Windows or your WSL distribution.
2.  **Edit the `pg_hba.conf` file**:
    Using the original WSL terminal, open `pg_hba.conf` for editing.
    ```bash
    sudo nano /etc/postgresql/$(pg_lsclusters -h | awk 'NR>1 {print $1}')/main/pg_hba.conf
    ```
    (Again, replace with your actual version if needed)

3.  **Add a new host entry**:
    Scroll to the end of the file or find the section where `host` rules are defined. Add the following line *above any very broad default `host` rules* (e.g., above `host all all 0.0.0.0/0 md5`) to ensure your specific rule is matched first:

    ```
    # Allow access from Windows to WSL PostgreSQL for myuser on mydatabase
    host    mydatabase      myuser          <WSL_IP_ADDRESS>/32            scram-sha-256
    ```
    *   Replace `mydatabase` and `myuser` with the names you created.
    *   Replace `<WSL_IP_ADDRESS>` with the IP address you noted in step 1. The `/32` indicates a single host IP address.
    *   **Alternative (Less Secure but Broader Access)**: If you want to allow any user to connect to any database from your WSL IP, you could use `host all all <WSL_IP_ADDRESS>/32 scram-sha-256`.
    *   **Considerations for IP Address**:
        *   If you're using `0.0.0.0/0` in `postgresql.conf` for `listen_addresses`, and want to restrict `pg_hba.conf` further, you can specify your Windows host's IP address (if it's static) or a subnet.
        *   For a development environment, allowing access from your `<WSL_IP_ADDRESS>/32` is generally the safest approach for Windows-to-WSL connections.
    *   **Recommendation**: Always use `scram-sha-256` for password authentication for better security.

    Save the file (Ctrl+O, Enter) and exit nano (Ctrl+X).
4.  **Restart PostgreSQL for changes to take effect**:
    ```bash
    sudo service postgresql restart
    ```

## 6. Access PostgreSQL from Windows

Now that PostgreSQL is configured, you can connect to your WSL-hosted database from your Windows environment using your preferred PostgreSQL client.

Common clients include:
*   <a href="https://www.pgadmin.org/" target="_blank">pgAdmin</a> (GUI tool)
*   <a href="https://dbeaver.io/" target="_blank">DBeaver</a> (Universal database tool)
*   `psql` command-line client (if you have it installed on Windows, e.g., via a PostgreSQL for Windows installation or Git Bash).

When configuring your client, use the following connection details:

*   **Host/Server Address**: `<WSL_IP_ADDRESS>` (the IP you noted in Section 5, Step 1)
*   **Port**: `5432` (default PostgreSQL port)
*   **Database**: `mydatabase` (the name you created)
*   **Username**: `myuser` (the user you created)
*   **Password**: `mysecurepassword` (the password you set for `myuser`)

### Example using Windows `psql` (if installed):

Open your Windows Command Prompt, PowerShell, or Git Bash and execute:

```bash
psql -h <WSL_IP_ADDRESS> -p 5432 -U myuser -d mydatabase
```
You will be prompted for the password.

## 7. Important Considerations

*   **WSL IP Address Volatility**: The IP address of your WSL instance can change when you restart Windows or your WSL distribution.
    *   **Solution**: You will need to re-run `ip addr show eth0 | grep inet | awk '{ print $2 }' | cut -d/ -f1` in WSL to get the current IP and update your Windows client connection settings and potentially your `pg_hba.conf` if you used a specific IP address there.
    *   **Advanced**: For more persistent solutions, you might look into configuring `netsh interface portproxy` on Windows or using tools designed to resolve dynamic WSL IPs, but these are outside the scope of this basic guide.
*   **Windows Firewall**: Ensure your Windows Firewall is not blocking the connection. Typically, WSL handles network routing transparently, but if you encounter connectivity issues, temporarily disabling the Windows Firewall (for testing purposes only!) can help diagnose. Re-enable it immediately after testing.
*   **Security for Production**:
    *   For production environments, never use broad `0.0.0.0/0` or `::/0` IP ranges in `pg_hba.conf`. Restrict access to known, static IP addresses or VPN subnets.
    *   Always enforce SSL/TLS encryption (`hostssl` in `pg_hba.conf` and `ssl=on` in `postgresql.conf`).
    *   Consider implementing `sslmode=verify-full` on your client connections to ensure both encryption and server identity verification.
    *   Use strong, unique passwords and consider password rotation policies.
    *   Regularly review PostgreSQL logs for unusual activity.

By following this guide, you should have a functional PostgreSQL setup on WSL2 accessible from your Windows environment for development purposes.
