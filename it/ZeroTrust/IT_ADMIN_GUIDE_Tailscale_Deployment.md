# IT Admin Guide: Tailscale Deployment and Management

**For**: IT Administrators
**Version**: 1.0
**Last Updated**: 2026-02-17

---

## Overview

This guide provides IT administrators with complete instructions for deploying, configuring, and managing Tailscale for secure remote database access.

**Environment:**
- **Solution**: Tailscale Starter Plan
- **Users**: 30 staff (Azure AD-joined Windows 11 laptops)
- **Resources**: PostgreSQL 16.11 on Ubuntu 24.04 LTS, CCTV system (separate VLAN)
- **Authentication**: Microsoft Entra ID (Azure AD) with MFA
- **Device Management**: Microsoft Intune
- **Cost**: £180/month (£6/user × 30 users)

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Prerequisites](#prerequisites)
3. [Phase 1: Tailscale Account Setup](#phase-1-tailscale-account-setup)
4. [Phase 2: Azure AD Integration](#phase-2-azure-ad-integration)
5. [Phase 3: Ubuntu Server Configuration](#phase-3-ubuntu-server-configuration)
6. [Phase 4: Windows 11 Client Deployment](#phase-4-windows-11-client-deployment)
7. [Phase 5: Access Control Lists (ACLs)](#phase-5-access-control-lists-acls)
8. [Phase 6: Monitoring and Maintenance](#phase-6-monitoring-and-maintenance)
9. [Troubleshooting](#troubleshooting)
10. [Security Hardening](#security-hardening)

---

## Architecture Overview

### High-Level Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                     Tailscale Coordination Server            │
│                    (tailscale.com - SaaS)                    │
│          • Azure AD authentication                           │
│          • Access control policies                           │
│          • Connection coordination                           │
└──────────────┬───────────────────────────────┬───────────────┘
               │                               │
               │ Encrypted tunnel              │ Encrypted tunnel
               │ (WireGuard)                   │ (WireGuard)
               │                               │
       ┌───────▼────────┐              ┌───────▼───────┐
       │  Windows 11    │              │ Ubuntu 24.04  │
       │    Laptop      │◄────────────►│  PostgreSQL   │
       │                │  Direct P2P  │    Server     │
       │  Tailscale     │  Connection  │  Tailscale    │
       │   Client       │  (90% time)  │   Daemon      │
       └────────────────┘              └───────┬───────┘
                                               │
                                               │ Subnet routing
                                               │
                                       ┌───────▼────────┐
                                       │  CCTV VLAN     │
                                       │  192.168.10.0  │
                                       │  /24           │
                                       └────────────────┘
```

### Key Components

**1. Tailscale Coordination Server (SaaS)**
- Hosted by Tailscale, Inc.
- Handles authentication (Azure AD SSO)
- Distributes access control policies (ACLs)
- Coordinates peer discovery (STUN/TURN)
- Does NOT route data traffic (except as relay fallback)

**2. Ubuntu Server (PostgreSQL + Tailscale Daemon)**
- Runs `tailscaled` daemon (background service)
- Advertises subnet routes (CCTV VLAN)
- Accepts connections from authorized clients
- Connection: `100.64.0.1:5432` (PostgreSQL)

**3. Windows 11 Client (Tailscale Client)**
- Runs Tailscale Windows client (GUI + service)
- Deployed via Intune (MSI package)
- Authenticates via Azure AD SSO
- Direct P2P connection to server (90%+ of time)

**4. Azure AD (Identity Provider)**
- Authenticates users (email + MFA)
- Enforces Conditional Access policies
- Provides device compliance status (Intune)

---

## Prerequisites

### Subscriptions & Licenses

- [ ] **Tailscale account** - Sign up at <a href="https://login.tailscale.com/start" target="_blank">login.tailscale.com/start</a>
- [ ] **Tailscale Starter plan** - £180/month for 30 users (upgrade from free trial)
- [ ] **Microsoft 365 Business Premium** (includes Azure AD Premium P1, Intune)
- [ ] **Azure AD Global Administrator access** (for app registration)

### Technical Requirements

**Server:**
- [ ] Ubuntu 24.04 LTS server with PostgreSQL 16.11
- [ ] Static internal IP address or hostname
- [ ] Outbound internet access (port 443 HTTPS)
- [ ] Sudo/root access for installation

**Clients:**
- [ ] Windows 11 Pro/Enterprise (Azure AD-joined)
- [ ] Microsoft Intune enrolled (device compliance policies configured)
- [ ] Outbound internet access (port 443, UDP 41641)

**Network:**
- [ ] SonicWall TZ 270 firewall (no inbound ports required)
- [ ] VLAN configuration documented (PostgreSQL VLAN, CCTV VLAN)

### Admin Access

- [ ] Azure AD Global Administrator or Application Administrator
- [ ] Intune Administrator (for app deployment)
- [ ] PostgreSQL DBA access (for user provisioning)
- [ ] Ubuntu server sudo/root access

---

## Phase 1: Tailscale Account Setup

**Estimated Time**: 30 minutes

### Step 1.1: Create Tailscale Account

1. Go to <a href="https://login.tailscale.com/start" target="_blank">login.tailscale.com/start</a>

2. Click **"Sign up"**

3. **Select "Microsoft"** as identity provider

4. **Sign in with your work email** (Azure AD admin account)

5. Complete MFA authentication

6. You'll be redirected to Tailscale admin console: <a href="https://login.tailscale.com/admin" target="_blank">login.tailscale.com/admin</a>

**Note**: This creates your "tailnet" (your company's private Tailscale network).

---

### Step 1.2: Configure Tailnet Settings

**Navigate to Settings** → **General**

**Tailnet name:**
- Set to: `yourcompany` (lowercase, no spaces)
- This becomes your network identifier

**Tailnet lock:**
- **Enable Tailnet Lock** (prevents unauthorized node addition)
- Save signing key securely (store in password manager)
- **IMPORTANT**: Without signing key, you cannot add nodes if admin account is compromised

**Magic DNS:**
- **Enable MagicDNS** (provides DNS names like `postgres-server`)
- This allows users to connect via `postgres-server.yourcompany.ts.net` instead of IP

**Key expiry:**
- Default: 180 days (devices re-authenticate automatically)
- **Set to**: 90 days (more secure, aligns with compliance requirements)

---

### Step 1.3: Upgrade to Starter Plan

**Navigate to Settings** → **Billing**

1. Click **"Upgrade plan"**

2. Select **"Starter"** plan
   - Price: £6/user/month
   - Users: 30
   - Total: £180/month

3. Enter billing information

4. Confirm subscription

**Features unlocked:**
- Up to 100 users (room for growth)
- Support for multiple admins
- Increased DNS query limits
- Email support (vs. community-only on free tier)

---

## Phase 2: Azure AD Integration

**Estimated Time**: 45 minutes

### Step 2.1: Create Azure AD App Registration

**In Azure Portal** (<a href="https://portal.azure.com" target="_blank">portal.azure.com</a>):

1. Navigate to **Microsoft Entra ID** (formerly Azure AD)

2. Click **App registrations** → **New registration**

3. Enter details:
   - **Name**: `Tailscale Zero Trust`
   - **Supported account types**: "Accounts in this organizational directory only"
   - **Redirect URI**: `https://login.tailscale.com/admin/oidc/callback`

4. Click **Register**

5. **Note down**:
   - **Application (client) ID**: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
   - **Directory (tenant) ID**: `yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy`

---

### Step 2.2: Create Client Secret

1. In your app registration, go to **Certificates & secrets**

2. Click **New client secret**

3. Description: `Tailscale Integration`

4. Expires: **24 months** (maximum allowed)

5. Click **Add**

6. **Copy the secret value** (visible only once!) → Save securely

**IMPORTANT**: Store client secret in password manager. If lost, must create new secret.

---

### Step 2.3: Configure API Permissions

1. Go to **API permissions**

2. Click **Add a permission** → **Microsoft Graph** → **Delegated permissions**

3. Add these permissions:
   - `openid`
   - `profile`
   - `email`
   - `User.Read`
   - `Group.Read.All` (for group-based access)

4. Click **Grant admin consent for [Your Organization]**

5. Verify all permissions show green checkmark under "Status"

---

### Step 2.4: Configure Tailscale to Use Azure AD

**In Tailscale Admin Console** (<a href="https://login.tailscale.com/admin/settings/auth" target="_blank">login.tailscale.com/admin/settings/auth</a>):

1. Navigate to **Settings** → **Identity provider**

2. Click **Add identity provider**

3. Select **Microsoft Entra ID** (Azure AD)

4. Enter credentials:
   - **Client ID**: `[Application ID from Step 2.1]`
   - **Client Secret**: `[Secret value from Step 2.2]`
   - **Tenant ID**: `[Directory ID from Step 2.1]`

5. Click **Save**

6. Click **Make primary** (makes Azure AD the default login method)

---

### Step 2.5: Test Azure AD Integration

1. Open new incognito browser window

2. Go to <a href="https://login.tailscale.com" target="_blank">login.tailscale.com</a>

3. Click **"Log in"**

4. Verify **"Microsoft"** appears as login option

5. Sign in with test user account (Azure AD + MFA)

6. Should successfully authenticate and see Tailscale dashboard

**If login fails:**
- Verify redirect URI matches exactly
- Check admin consent was granted
- Verify client secret is correct (not expired)

---

### Step 2.6: Configure Conditional Access (Optional)

**To enforce device compliance and MFA:**

**In Azure Portal** → **Microsoft Entra ID** → **Security** → **Conditional Access**:

1. Click **New policy**

2. **Name**: `Tailscale - Require Compliant Device + MFA`

3. **Assignments**:
   - **Users**: Select group (e.g., "Database_Users")
   - **Cloud apps**: Select "Tailscale Zero Trust" app
   - **Conditions**: Configure as needed

4. **Access controls** → **Grant**:
   - ☑ Require multi-factor authentication
   - ☑ Require device to be marked as compliant
   - Grant access: "Require all selected controls"

5. **Enable policy**: On

6. **Create**

**Result**: Users must have MFA + compliant device to authenticate to Tailscale.

---

## Phase 3: Ubuntu Server Configuration

**Estimated Time**: 30 minutes

### Step 3.1: Install Tailscale on Ubuntu Server

**SSH to PostgreSQL server:**

```bash
ssh user@postgres-server
```

**Install Tailscale:**

```bash
# Add Tailscale repository
curl -fsSL https://pkgs.tailscale.com/stable/ubuntu/noble.noarmor.gpg | sudo tee /usr/share/keyrings/tailscale-archive-keyring.gpg >/dev/null
curl -fsSL https://pkgs.tailscale.com/stable/ubuntu/noble.tailscale-keyring.list | sudo tee /etc/apt/sources.list.d/tailscale.list

# Update and install
sudo apt-get update
sudo apt-get install tailscale

# Start and enable service
sudo systemctl enable --now tailscaled
```

**Verify installation:**

```bash
tailscale version
# Should show: tailscale 1.x.x
```

---

### Step 3.2: Authenticate Server to Tailnet

**Generate auth key** (in Tailscale Admin Console):

1. Go to **Settings** → **Keys**

2. Click **Generate auth key**

3. Configure:
   - **Reusable**: No (one-time use)
   - **Ephemeral**: No (keep device after disconnect)
   - **Pre-approved**: Yes (skip manual approval)
   - **Tags**: Add `tag:servers` (for ACL policies)
   - **Expiration**: 1 hour (sufficient for setup)

4. Click **Generate key**

5. Copy key: `tskey-auth-xxxxxxxxxxxxxxxxxxxx`

**Authenticate server** (on Ubuntu server):

```bash
sudo tailscale up --auth-key=tskey-auth-xxxxxxxxxxxxxxxxxxxx --advertise-tags=tag:servers
```

**Verify connection:**

```bash
tailscale status
# Should show: connected
```

---

### Step 3.3: Configure Shared IP for PostgreSQL

**Generate shareable address:**

```bash
sudo tailscale set --advertise-address 100.64.0.1
```

**Verify:**

```bash
tailscale ip -4
# Should show both:
# 100.x.x.x (actual Tailscale IP)
# 100.64.0.1 (shared IP)
```

**Result**: All clients will connect to `100.64.0.1` (simplifies configuration).

---

### Step 3.4: Configure Subnet Routing (for CCTV VLAN)

**Enable IP forwarding** (required for subnet routing):

```bash
echo 'net.ipv4.ip_forward = 1' | sudo tee -a /etc/sysctl.conf
echo 'net.ipv6.conf.all.forwarding = 1' | sudo tee -a /etc/sysctl.conf
sudo sysctl -p
```

**Advertise CCTV subnet** (example: CCTV VLAN is 192.168.10.0/24):

```bash
sudo tailscale up --advertise-routes=192.168.10.0/24
```

**Approve subnet routes** (in Tailscale Admin Console):

1. Go to **Machines**

2. Find your PostgreSQL server

3. Click **...** (three dots) → **Edit route settings**

4. Check **Approved** next to `192.168.10.0/24`

5. Click **Save**

**Verify routing:**

```bash
tailscale status
# Should show: "Subnets: 192.168.10.0/24"
```

**Test from Windows client** (after client deployment):
```powershell
ping 192.168.10.100  # CCTV device IP
```

---

### Step 3.5: Configure PostgreSQL Access

**Update pg_hba.conf** to allow Tailscale subnet:

```bash
sudo nano /etc/postgresql/16/main/pg_hba.conf
```

**Add this line** (before other rules):

```
# Tailscale remote access
hostssl    all    all    100.64.0.0/10    scram-sha-256
```

**Reload PostgreSQL:**

```bash
sudo systemctl reload postgresql
```

**Security note**: This allows connections from Tailscale IP range (`100.64.0.0/10`) with strong authentication (`scram-sha-256`) and SSL encryption.

---

### Step 3.6: Test Local Connection

**From Ubuntu server:**

```bash
psql -h 100.64.0.1 -U postgres -d postgres
# Enter password when prompted
# Should connect successfully
```

**If connection fails:**
- Verify PostgreSQL is listening on all interfaces: `listen_addresses = '*'` in postgresql.conf
- Check firewall: `sudo ufw status` (allow port 5432 if needed)
- Verify SSL is enabled: `ssl = on` in postgresql.conf

---

## Phase 4: Windows 11 Client Deployment

**Estimated Time**: 2-3 hours (including testing)

### Step 4.1: Download Tailscale MSI

**Download official MSI** package:

1. Go to <a href="https://pkgs.tailscale.com/stable/#windows" target="_blank">pkgs.tailscale.com/stable/#windows</a>

2. Download **tailscale-setup-x.x.x-amd64.msi** (latest stable version)

3. Save to secure location (for Intune upload)

**Verify hash** (optional but recommended):

```powershell
Get-FileHash tailscale-setup-*.msi -Algorithm SHA256
# Compare with hash on Tailscale download page
```

---

### Step 4.2: Prepare Intune Deployment

**In Microsoft Intune Admin Center** (<a href="https://endpoint.microsoft.com" target="_blank">endpoint.microsoft.com</a>):

1. Navigate to **Apps** → **All apps** → **Add**

2. Select **App type**: "Windows app (Win32)"

3. Click **Select**

**Upload app package:**
1. Click **Select app package file**
2. Upload: `tailscale-setup-x.x.x-amd64.msi`
3. Wait for upload (may take 2-5 minutes)

---

### Step 4.3: Configure App Information

**App information page:**

- **Name**: `Tailscale Zero Trust Client`
- **Description**: `Secure remote access to company database and CCTV`
- **Publisher**: `Tailscale Inc.`
- **App version**: `1.x.x` (match downloaded version)
- **Category**: Productivity
- **Show as featured app**: Yes (helps users find it in Company Portal)
- **Information URL**: `https://tailscale.com`
- **Privacy URL**: `https://tailscale.com/privacy-policy`

**Click Next**

---

### Step 4.4: Configure Installation Settings

**Program page:**

- **Install command**:
  ```
  msiexec /i "tailscale-setup-x.x.x-amd64.msi" /quiet TS_AUTHKEY=""
  ```
  *(Leave TS_AUTHKEY empty - users will authenticate interactively)*

- **Uninstall command**:
  ```
  msiexec /x "{GUID}" /quiet
  ```
  *(Intune fills GUID automatically)*

- **Install behavior**: System
- **Device restart behavior**: No specific action
- **Return codes**: (defaults OK)

**Click Next**

---

### Step 4.5: Configure Requirements

**Requirements page:**

- **Operating system architecture**: 64-bit
- **Minimum operating system**: Windows 11 21H2
- **Disk space required**: 100 MB
- **Physical memory required**: (leave blank)
- **Number of processors required**: (leave blank)
- **CPU speed required**: (leave blank)

**Click Next**

---

### Step 4.6: Configure Detection Rules

**Detection rules page:**

**Rules format**: Manually configure detection rules

**Add rule:**
1. Click **Add**
2. **Rule type**: File
3. **Path**: `C:\Program Files\Tailscale`
4. **File or folder**: `tailscale.exe`
5. **Detection method**: File or folder exists
6. **Associated with a 32-bit app on 64-bit clients**: No

**Click OK** → **Next**

---

### Step 4.7: Configure Dependencies & Assignments

**Dependencies**: (None needed) → Click Next

**Supersedence**: (None) → Click Next

**Assignments**:

1. Click **Add group** under "Required"

2. Select Azure AD group: `Database_Users` (or appropriate group)

3. **End user notifications**: Show all toast notifications

4. **Delivery optimization**: Not configured

**Click Next**

---

### Step 4.8: Review and Deploy

**Review + create page:**

1. Review all settings

2. Click **Create**

3. Wait for deployment (Intune processes assignment)

**Deployment will:**
- Install Tailscale on all laptops in "Database_Users" group
- Installation happens at next device check-in (usually within 8 hours)
- Users see toast notification during installation

---

### Step 4.9: Monitor Deployment

**Check deployment status:**

1. Go to **Apps** → **Tailscale Zero Trust Client** → **Device install status**

2. Monitor:
   - **Installed**: Successful installs
   - **Failed**: Installation errors
   - **Pending**: Waiting for device check-in

**Troubleshoot failures:**
- Click on failed device → View error log
- Common issues: Insufficient disk space, pending Windows updates

**Expected timeline:**
- **Day 1**: 30-50% devices installed
- **Day 3**: 80-90% devices installed
- **Day 7**: 95%+ devices installed

---

## Phase 5: Access Control Lists (ACLs)

**Estimated Time**: 45 minutes

### Understanding Tailscale ACLs

**ACLs control**:
- Who can access which resources
- Which ports are accessible
- Tag-based access (servers, users, admins)

**Default policy**: Deny all (except same user's devices)

**Goal**: Create least-privilege access (users access only database + CCTV, nothing else)

---

### Step 5.1: Define ACL Policy

**In Tailscale Admin Console** → **Access Controls** → **Edit ACL**

**Replace default ACL with:**

```json
{
  // Define tags for organizing devices
  "tagOwners": {
    "tag:servers": ["autogroup:admin"],
    "tag:users": ["autogroup:admin"]
  },

  // Define groups (Azure AD groups synced)
  "groups": {
    "group:database-users": ["user1@yourcompany.com", "user2@yourcompany.com"],
    "group:database-admins": ["admin@yourcompany.com"]
  },

  // Access control rules
  "acls": [
    // Rule 1: Database users can access PostgreSQL
    {
      "action": "accept",
      "src": ["autogroup:member"],
      "dst": ["tag:servers:5432"]
    },

    // Rule 2: Database users can access CCTV subnet
    {
      "action": "accept",
      "src": ["autogroup:member"],
      "dst": ["192.168.10.0/24:443", "192.168.10.0/24:554"]
    },

    // Rule 3: Admins can SSH to servers
    {
      "action": "accept",
      "src": ["group:database-admins"],
      "dst": ["tag:servers:22"]
    },

    // Rule 4: Allow DNS
    {
      "action": "accept",
      "src": ["autogroup:member"],
      "dst": ["tag:servers:53"]
    }
  ],

  // SSH rules (for admin access)
  "ssh": [
    {
      "action": "accept",
      "src": ["group:database-admins"],
      "dst": ["tag:servers"],
      "users": ["root", "autogroup:nonroot"]
    }
  ]
}
```

**Click Save** → **Test rules** (validates syntax)

---

### Step 5.2: ACL Rule Explanation

**Rule 1 (PostgreSQL access)**:
```json
{
  "action": "accept",
  "src": ["autogroup:member"],
  "dst": ["tag:servers:5432"]
}
```

- **Source**: All Tailscale users in your tailnet
- **Destination**: Devices tagged `tag:servers` on port `5432` (PostgreSQL)
- **Effect**: All users can connect to database

**To restrict to specific users:**
- Replace `"autogroup:member"` with `["group:database-users"]`
- Requires syncing Azure AD groups (see Step 5.3)

---

**Rule 2 (CCTV access)**:
```json
{
  "action": "accept",
  "src": ["autogroup:member"],
  "dst": ["192.168.10.0/24:443", "192.168.10.0/24:554"]
}
```

- **Source**: All Tailscale users
- **Destination**: CCTV subnet on port 443 (HTTPS web interface) and 554 (RTSP streaming)
- **Effect**: All users can access CCTV cameras

---

### Step 5.3: Sync Azure AD Groups (Optional)

**For granular group-based access:**

1. In Tailscale Admin Console → **Settings** → **Users & Groups**

2. Click **Sync groups from Azure AD**

3. Select groups to sync:
   - `Database_Users`
   - `Database_Admins`
   - `CCTV_Access`

4. Click **Sync**

**Update ACL to reference Azure AD groups:**

```json
"groups": {
  "group:database-users": ["azuread-group:Database_Users"],
  "group:cctv-users": ["azuread-group:CCTV_Access"]
}
```

**Benefit**: Group membership managed in Azure AD, automatically reflected in Tailscale ACLs.

---

### Step 5.4: Test ACL Policy

**From test user's laptop:**

```powershell
# Test PostgreSQL connectivity
Test-NetConnection -ComputerName 100.64.0.1 -Port 5432
# Should show: TcpTestSucceeded : True

# Test CCTV web interface
Test-NetConnection -ComputerName 192.168.10.100 -Port 443
# Should show: TcpTestSucceeded : True
```

**If connection fails:**
- Verify ACL rule syntax (JSON validator)
- Check device has correct tag (`tag:servers`)
- Verify subnet routes are approved
- Check firewall on destination device

---

## Phase 6: Monitoring and Maintenance

### Step 6.1: Set Up Monitoring Dashboard

**In Tailscale Admin Console** → **Machines**:

**Monitor:**
- **Online devices**: Should match number of deployed laptops
- **Offline devices**: Investigate if offline > 48 hours
- **Last seen**: Identify inactive devices
- **OS version**: Track Windows 11 version compliance

**Weekly review:**
- Remove devices for departed employees
- Approve new devices (if pre-approval disabled)
- Review audit logs for suspicious activity

---

### Step 6.2: Review Audit Logs

**Tailscale Admin Console** → **Logs**:

**Monitor for:**
- Failed authentication attempts (may indicate compromise)
- Unusual connection times (3 AM connections from user laptops?)
- Geographic anomalies (UK user suddenly in Asia?)
- Excessive connection frequency (script/bot activity?)

**Retention**: 90 days on Starter plan (download periodically for longer retention)

**Export logs** (for SIEM integration):

```bash
# Use Tailscale API to fetch logs
curl -u "tskey-api-xxxx:" https://api.tailscale.com/api/v2/tailnet/yourcompany.com/logs
```

---

### Step 6.3: Patch Management

**Tailscale updates:**
- **Windows clients**: Auto-update via Intune (update MSI package quarterly)
- **Ubuntu server**: Auto-update via unattended-upgrades

**Configure auto-updates on Ubuntu:**

```bash
# Install unattended-upgrades
sudo apt-get install unattended-upgrades

# Enable Tailscale updates
echo 'Unattended-Upgrade::Origins-Pattern {
  "origin=Tailscale";
};' | sudo tee /etc/apt/apt.conf.d/52tailscale-updates

# Enable service
sudo dpkg-reconfigure --priority=low unattended-upgrades
```

**Verify auto-updates:**

```bash
sudo unattended-upgrades --dry-run --debug
```

---

### Step 6.4: User Offboarding

**When employee leaves:**

1. **Azure AD account disabled** (HR triggers)

2. **Tailscale access automatically revoked** (Azure AD integration)

3. **Optional**: Manually remove device from Tailscale:
   - **Admin Console** → **Machines** → Find user's laptop → **...** → **Remove**

**Result**: User cannot authenticate to Tailscale, loses database access immediately.

---

### Step 6.5: Backup and Disaster Recovery

**Critical data to backup:**

1. **ACL policy**: Copy from Admin Console → Save to Git repository

2. **Tailnet signing key**: Store in secure password manager

3. **Azure AD app credentials**: Document client ID, secret (encrypted)

4. **PostgreSQL server Tailscale key**: Backup server configuration

**Recovery procedure** (if Tailscale server dies):

1. Provision new Ubuntu server

2. Install Tailscale

3. Generate new auth key (reusable)

4. Authenticate: `sudo tailscale up --auth-key=tskey-xxxx --advertise-tags=tag:servers`

5. Re-configure subnet routes

6. Test client connectivity

**RTO**: 30 minutes (Recovery Time Objective)

---

## Troubleshooting

### Issue: User Can't Connect to Database

**Symptoms:**
- User reports "Can't connect to database"
- Tailscale shows green (connected)

**Diagnosis steps:**

1. **Verify user is on Tailnet:**
   ```
   Admin Console → Users → Find user → Check "Last seen"
   ```

2. **Check ACL allows access:**
   ```
   Admin Console → Access Controls → Test user access to 100.64.0.1:5432
   ```

3. **Test from user's laptop:**
   ```powershell
   Test-NetConnection -ComputerName 100.64.0.1 -Port 5432
   ```

4. **Check PostgreSQL logs** (on Ubuntu server):
   ```bash
   sudo tail -f /var/log/postgresql/postgresql-16-main.log
   ```

**Common causes:**
- ACL denies access (update policy)
- PostgreSQL not listening on Tailscale IP (check pg_hba.conf)
- Database credentials incorrect (reset password)
- SSL error (verify sslmode=require in client)

---

### Issue: Tailscale Won't Connect on Windows Client

**Symptoms:**
- Tailscale icon grey (disconnected)
- Won't authenticate

**Diagnosis steps:**

1. **Check Tailscale service status:**
   ```powershell
   Get-Service Tailscale
   # Should show: Running
   ```

2. **Check firewall:**
   ```powershell
   Test-NetConnection -ComputerName login.tailscale.com -Port 443
   # Should succeed
   ```

3. **Check event logs:**
   ```
   Event Viewer → Applications and Services Logs → Tailscale
   ```

4. **Reinstall Tailscale:**
   - Intune → Apps → Tailscale → Select device → Uninstall
   - Wait 15 minutes
   - Trigger sync (device will reinstall)

**Common causes:**
- Firewall blocking UDP 41641 (corporate firewall issue)
- Azure AD authentication failed (MFA timeout)
- Tailscale service crashed (reinstall fixes)

---

### Issue: Subnet Routing Not Working (CCTV Inaccessible)

**Symptoms:**
- Can access database (100.64.0.1) but not CCTV (192.168.10.x)

**Diagnosis steps:**

1. **Check subnet route approval:**
   ```
   Admin Console → Machines → PostgreSQL server → Route settings
   # Verify 192.168.10.0/24 is "Approved"
   ```

2. **Verify IP forwarding on server:**
   ```bash
   sysctl net.ipv4.ip_forward
   # Should output: net.ipv4.ip_forward = 1
   ```

3. **Check server can reach CCTV:**
   ```bash
   ping 192.168.10.100
   # Should succeed from PostgreSQL server
   ```

4. **Test from client:**
   ```powershell
   Test-NetConnection -ComputerName 192.168.10.100 -Port 443
   ```

**Common causes:**
- Subnet route not approved (approve in Admin Console)
- IP forwarding disabled (re-run sysctl commands)
- Firewall blocking between PostgreSQL VLAN and CCTV VLAN
- ACL missing rule for 192.168.10.0/24

---

## Security Hardening

### Harden PostgreSQL Access

**1. Use strong authentication (scram-sha-256):**

```bash
# pg_hba.conf
hostssl    all    all    100.64.0.0/10    scram-sha-256
```

**2. Enforce SSL/TLS:**

Replace self-signed certificates with proper TLS certificates:

```bash
# Generate Let's Encrypt cert (if server has public domain)
sudo certbot certonly --standalone -d postgres.yourcompany.com

# Update postgresql.conf
ssl_cert_file = '/etc/letsencrypt/live/postgres.yourcompany.com/fullchain.pem'
ssl_key_file = '/etc/letsencrypt/live/postgres.yourcompany.com/privkey.pem'
```

**3. Enable query logging (audit trail):**

```bash
# postgresql.conf
log_connections = on
log_disconnections = on
log_statement = 'all'  # WARNING: Logs ALL queries (performance impact)
```

**4. Least-privilege database accounts:**

```sql
-- Create read-only user
CREATE ROLE readonly_user WITH LOGIN PASSWORD 'strong_password';
GRANT CONNECT ON DATABASE mydb TO readonly_user;
GRANT USAGE ON SCHEMA public TO readonly_user;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO readonly_user;
```

---

### Enable Device Posture Checks

**Enforce device compliance:**

1. **In Tailscale Admin Console** → **Access Controls** → **Device Posture**

2. Enable checks:
   - ☑ **OS version** (Windows 11 22H2 or newer)
   - ☑ **Disk encryption** (BitLocker enabled)
   - ☑ **Antivirus running** (Windows Defender active)
   - ☑ **Firewall enabled**

3. **Configure Intune compliance policy:**
   - **Devices** → **Compliance policies** → **Create policy**
   - **Platform**: Windows 11
   - **Requirements**: BitLocker required, Windows Defender enabled, OS up-to-date

4. **Link to Conditional Access:**
   - Azure AD → Conditional Access → Require compliant device for Tailscale app

**Result**: Non-compliant devices cannot authenticate to Tailscale.

---

### Implement MFA Enforcement

**Ensure MFA is required:**

1. **Azure AD** → **Security** → **Conditional Access**

2. Create policy: **"Require MFA for Tailscale"**

3. Assignments:
   - Users: All database users
   - Cloud apps: Tailscale app
   - Conditions: Any location

4. Access controls:
   - Grant: Require multi-factor authentication

5. Enable policy

**Result**: All Tailscale authentications require MFA.

---

### Regular Security Reviews

**Monthly:**
- [ ] Review Tailscale audit logs for anomalies
- [ ] Check for offline devices (> 30 days)
- [ ] Verify ACL policy still matches business needs
- [ ] Review database access logs (who accessed what)

**Quarterly:**
- [ ] Rotate Azure AD client secret (before expiry)
- [ ] Review user permissions (remove unnecessary access)
- [ ] Test disaster recovery procedure
- [ ] Update documentation with any changes

**Annually:**
- [ ] Full security audit (penetration test if budget allows)
- [ ] Review Tailscale SOC 2 report (request from Tailscale support)
- [ ] Update ACL policy based on lessons learned
- [ ] Review and update user training materials

---

## Summary Checklist

**Deployment Checklist:**

- [ ] Tailscale account created and upgraded to Starter plan (£180/month)
- [ ] Azure AD app registration configured with correct permissions
- [ ] Tailscale integrated with Azure AD (SSO working)
- [ ] Conditional Access policy enforces MFA + compliant device
- [ ] Ubuntu server running Tailscale daemon with subnet routing
- [ ] PostgreSQL configured to accept Tailscale connections (SSL + strong auth)
- [ ] Windows 11 clients deployed via Intune (all 30 users)
- [ ] ACL policy configured (least-privilege access)
- [ ] Subnet routes approved (CCTV VLAN accessible)
- [ ] Monitoring dashboard configured (weekly reviews)
- [ ] User documentation distributed (User Guide, Quick Start, FAQ)
- [ ] IT team trained on troubleshooting procedures
- [ ] Backup of critical configuration (ACLs, keys, Azure AD app details)
- [ ] Disaster recovery tested (can rebuild server in < 1 hour)

---

**Success Criteria:**

- ✅ All 30 users can access database from any location
- ✅ Zero inbound firewall ports opened on SonicWall TZ 270
- ✅ Azure AD MFA enforced for all Tailscale authentications
- ✅ CCTV accessible via subnet routing (no agent on cameras)
- ✅ Average connection latency < 50ms (UK users)
- ✅ User satisfaction > 80% (ease of use)
- ✅ Zero security incidents in first 90 days
- ✅ Total deployment time < 2 weeks
- ✅ Monthly cost within budget (£180/month = 40% under £300 budget)

---

**Document Version**: 1.0
**Last Updated**: 2026-02-17
**Next Review**: 2026-05-17 (Quarterly)

---

*For questions or clarifications, contact the IT Security team or refer to Tailscale official documentation: <a href="https://tailscale.com/kb/" target="_blank">tailscale.com/kb/</a>*
