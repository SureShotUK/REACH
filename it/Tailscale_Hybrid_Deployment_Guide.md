# Tailscale Hybrid Deployment Guide
## Zero-Touch ZTNA for Hybrid Workers (35 Users, 3 Offices)

**Document Version:** 1.0
**Date:** February 6, 2026
**Environment:** Office1 (24 users), Office2 (8 users), Office3 (3 users)
**Target:** Seamless access for hybrid workers (office + remote)

---

## Executive Summary

This guide deploys Tailscale as a transparent, zero-touch connectivity solution for hybrid workers. Users experience seamless access to Office1 PostgreSQL database and RDP hosts whether working from office or remotely, with zero daily interaction required.

**Key Benefits:**
- ✅ **"Stupid Simple" for users:** Auto-connects, works everywhere, zero interaction after setup
- ✅ **Hybrid-worker optimized:** Same experience in office or at home
- ✅ **Best performance:** Peer-to-peer mesh networking (1-5ms overhead for PostgreSQL)
- ✅ **Azure AD SSO:** One-time Microsoft sign-in, inherits existing MFA
- ✅ **Cost-effective:** $2,520/year for all 35 users

**User Experience:**
- **In office:** Laptop auto-connects, resources work transparently
- **At home:** Same - auto-connects, resources work identically
- **On the road:** Same - works from any network
- **Daily interaction:** ZERO (small system tray icon, completely automatic)

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Prerequisites](#prerequisites)
3. [Phase 1: Pilot Deployment (Office3 Free Tier)](#phase-1-pilot-deployment-office3-free-tier)
4. [Phase 2: Subnet Router Deployment (All Offices)](#phase-2-subnet-router-deployment-all-offices)
5. [Phase 3: Azure AD SSO Integration](#phase-3-azure-ad-sso-integration)
6. [Phase 4: Client Deployment via Intune/GPO](#phase-4-client-deployment-via-intunegpo)
7. [PostgreSQL ODBC Configuration](#postgresql-odbc-configuration)
8. [RDP Configuration](#rdp-configuration)
9. [Testing & Validation](#testing--validation)
10. [User Quick-Start Guide](#user-quick-start-guide-1-page)
11. [Troubleshooting](#troubleshooting)
12. [Maintenance & Operations](#maintenance--operations)

---

## Architecture Overview

### Network Topology

```
┌─────────────────────────────────────────────────────────────────┐
│                     Tailscale Mesh Network                      │
│                        (100.x.x.x/10)                          │
└─────────────────────────────────────────────────────────────────┘
         │                      │                      │
    ┌────▼─────┐          ┌────▼─────┐          ┌────▼─────┐
    │ Office1  │          │ Office2  │          │ Office3  │
    │ Subnet   │◄────────►│ Subnet   │          │ Subnet   │
    │ Router   │  IPSec   │ Router   │          │ Router   │
    └────┬─────┘          └────┬─────┘          └────┬─────┘
         │                      │                      │
    ┌────▼──────────┐     ┌────▼─────┐          ┌────▼─────┐
    │ SonicWall     │     │SonicWall │          │ Draytek  │
    │ TZ270         │     │ TZ270    │          │ Router   │
    │ 10.1.0.0/24   │     │10.2.0.0/24│         │10.3.0.0/24│
    └───────────────┘     └──────────┘          └──────────┘
         │                      │                      │
    ┌────▼──────────┐     ┌────▼─────┐          ┌────▼─────┐
    │ PostgreSQL    │     │ 8 Users  │          │ 3 Users  │
    │ 24 Users      │     │          │          │ Local DB │
    │ 3 RDP PCs     │     │          │          │          │
    └───────────────┘     └──────────┘          └──────────┘

Remote Workers:
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│ User Laptop  │────►│   Tailscale  │────►│ Office1      │
│ (Home/Travel)│     │   P2P Mesh   │     │ Subnet Router│
│ Auto-connect │     │   (Direct)   │     │              │
└──────────────┘     └──────────────┘     └──────────────┘
```

### Components

**1. Tailscale Subnet Routers (3 total):**
- **Office1 Subnet Router:** Advertises 10.1.0.0/24 (PostgreSQL server, RDP hosts)
- **Office2 Subnet Router:** Advertises 10.2.0.0/24 (if needed for local resources)
- **Office3 Subnet Router:** Advertises 10.3.0.0/24 (local database)

**2. Tailscale Clients (35 laptops):**
- Installed on all user laptops
- Auto-connect enabled
- Azure AD SSO configured
- Runs in background, minimal visibility

**3. Tailscale Control Plane:**
- Cloud-hosted coordination server (Tailscale SaaS)
- Manages ACLs, routing, device authorization
- No user data passes through control plane (only coordination)

### Traffic Flows

**Scenario 1: Office1 user accessing local PostgreSQL database (in office)**
```
User Laptop (10.1.0.50) → Local LAN → PostgreSQL (10.1.0.100)
(Tailscale not in path - direct local access)
```

**Scenario 2: Office2 user accessing Office1 PostgreSQL database (in office)**
```
User Laptop → Office2 Subnet Router → Tailscale Mesh → Office1 Subnet Router → PostgreSQL
OR (if IPSec tunnel exists):
User Laptop → Local LAN → IPSec tunnel → Office1 LAN → PostgreSQL
(Tailscale coexists, routes optimize automatically)
```

**Scenario 3: Remote user accessing Office1 PostgreSQL database (from home)**
```
User Laptop (Home WiFi) → Tailscale Client → P2P Mesh → Office1 Subnet Router → PostgreSQL
(Direct peer-to-peer connection, no central VPN gateway)
```

**Scenario 4: Office1 user accessing Office3 local database**
```
User Laptop → Office1 Subnet Router → Tailscale Mesh → Office3 Subnet Router → Office3 DB
```

### Key Features

- **Automatic routing:** Tailscale clients auto-discover optimal routes
- **Split tunneling:** Only traffic destined for Tailscale network routes through VPN
- **MagicDNS:** Access resources by hostname (e.g., `office1-db` vs `100.64.1.10`)
- **ACL-based security:** Granular access control via Tailscale admin console
- **Zero-config NAT traversal:** Direct connections even behind firewalls/NAT

---

## Prerequisites

### Infrastructure Requirements

**Office1:**
- [ ] SonicWall TZ270 with internet access
- [ ] VM or dedicated device for subnet router (Linux preferred)
  - **Minimum specs:** 1 vCPU, 512MB RAM, 5GB disk
  - **Recommended:** 2 vCPU, 1GB RAM, 10GB disk
  - **OS:** Ubuntu 24.04 LTS (or similar Linux)
- [ ] Static IP for subnet router on Office1 LAN (e.g., 10.1.0.250)
- [ ] PostgreSQL server accessible on LAN (10.1.0.100 assumed)
- [ ] 3 RDP-accessible PCs on LAN

**Office2:**
- [ ] SonicWall TZ270 with internet access
- [ ] VM or dedicated device for subnet router
- [ ] Static IP for subnet router on Office2 LAN (e.g., 10.2.0.250)

**Office3:**
- [ ] Draytek router with internet access
- [ ] VM or dedicated device for subnet router
- [ ] Static IP for subnet router on Office3 LAN (e.g., 10.3.0.250)
- [ ] Local database machine accessible on LAN

**Client Devices (35 laptops):**
- [ ] Windows 10/11 (64-bit)
- [ ] Microsoft 365 with Azure AD accounts
- [ ] Internet connectivity (office and remote)
- [ ] Microsoft Intune OR Active Directory GPO access for deployment

### Network Information Needed

Document the following before starting:

**Office1:**
- LAN subnet: `____________` (e.g., 10.1.0.0/24)
- PostgreSQL server IP: `____________` (e.g., 10.1.0.100)
- RDP host IPs: `____________, ____________, ____________`
- Subnet router IP: `____________` (e.g., 10.1.0.250)

**Office2:**
- LAN subnet: `____________` (e.g., 10.2.0.0/24)
- Subnet router IP: `____________` (e.g., 10.2.0.250)

**Office3:**
- LAN subnet: `____________` (e.g., 10.3.0.0/24)
- Local database IP: `____________` (e.g., 10.3.0.50)
- Subnet router IP: `____________` (e.g., 10.3.0.250)

### Accounts & Access

- [ ] Microsoft 365 Global Administrator access (for Azure AD app registration)
- [ ] Tailscale account (create at <a href="https://login.tailscale.com/start" target="_blank">login.tailscale.com</a>)
- [ ] Admin access to SonicWall firewalls
- [ ] Admin access to Draytek router
- [ ] Intune or GPO permissions for software deployment

### Firewall Requirements

**Outbound access required (all offices):**
- HTTPS (TCP 443) to `*.tailscale.com`
- UDP 3478 (STUN) for NAT traversal
- UDP 41641 (default Tailscale port, fallback to random high ports)

**No inbound ports required** - Tailscale initiates outbound connections only.

---

## Phase 1: Pilot Deployment (Office3 Free Tier)

**Objective:** Validate Tailscale with 3 Office3 users using free tier before full deployment.

**Duration:** 1-2 weeks
**Cost:** $0 (free tier: 3 users, 100 devices)

### Step 1.1: Create Tailscale Account

1. Go to <a href="https://login.tailscale.com/start" target="_blank">https://login.tailscale.com/start</a>
2. Click **"Sign up"**
3. Choose **"Sign up with Microsoft"** (use your Microsoft 365 admin account)
4. Complete Microsoft authentication
5. Verify email if prompted
6. You're now in the Tailscale admin console

### Step 1.2: Deploy Office3 Subnet Router

**On Office3 network, create a Linux VM or use dedicated device:**

**Install Ubuntu 24.04 on VM/device:**
- Assign static IP: 10.3.0.250 (or your chosen IP)
- Ensure internet access through Draytek router

**Install Tailscale:**

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install Tailscale
curl -fsSL https://tailscale.com/install.sh | sh

# Start Tailscale
sudo tailscale up
```

**Authenticate:**
- Command will output URL: `https://login.tailscale.com/a/xxxxxxxxxxxx`
- Open URL in browser
- Sign in with your Microsoft account (same as admin account)
- Approve device

**Enable subnet routing:**

```bash
# Enable IP forwarding
echo 'net.ipv4.ip_forward = 1' | sudo tee -a /etc/sysctl.conf
echo 'net.ipv6.conf.all.forwarding = 1' | sudo tee -a /etc/sysctl.conf
sudo sysctl -p

# Advertise Office3 subnet (adjust subnet to match your network)
sudo tailscale up --advertise-routes=10.3.0.0/24 --accept-routes
```

**Approve subnet routes in admin console:**

1. Go to <a href="https://login.tailscale.com/admin/machines" target="_blank">Tailscale Admin > Machines</a>
2. Find the Office3 subnet router device
3. Click **"..."** menu → **"Edit route settings"**
4. Toggle **ON** the subnet route (10.3.0.0/24)
5. Click **"Save"**

**Verify subnet router:**

```bash
# Check Tailscale status
sudo tailscale status

# Should show:
# - Device name and IP (100.x.x.x)
# - "Subnet routes: 10.3.0.0/24"
```

### Step 1.3: Install Tailscale Client on 3 Office3 User Laptops

**Manual installation (for pilot testing):**

**On each Office3 user laptop:**

1. Download Tailscale Windows client:
   - <a href="https://tailscale.com/download/windows" target="_blank">https://tailscale.com/download/windows</a>

2. Run installer: `tailscale-setup-latest.exe`
   - Click **"Install"**
   - Allow firewall prompts

3. Click **"Sign in"** in Tailscale tray icon
   - Choose **"Sign in with Microsoft"**
   - User enters their Microsoft 365 credentials
   - Approve device

4. Verify connection:
   - Tailscale tray icon shows green checkmark
   - Open PowerShell: `tailscale status`
   - Should show connected devices and subnet routes

### Step 1.4: Test Office3 Local Database Access from Office1

**From an Office1 user laptop (needs Tailscale client installed):**

1. Install Tailscale client (same process as Step 1.3)

2. Test connectivity to Office3 database:

```powershell
# Test ping to Office3 database (adjust IP)
ping 10.3.0.50

# Should receive replies via Tailscale mesh
```

3. Configure Excel ODBC connection to Office3 database:
   - Use Office3 database IP: `10.3.0.50`
   - Connection should work transparently through Tailscale

4. Verify traffic route:

```powershell
# Check Tailscale status
tailscale status

# Should show Office3 subnet router in peer list
# Should show subnet route: 10.3.0.0/24 via Office3 router
```

### Step 1.5: Test Remote Access (Office3 User from Home)

**Office3 user works from home:**

1. Connect laptop to home WiFi
2. Tailscale client auto-connects (should see green checkmark in tray)
3. Test database access:

```powershell
# Ping Office3 local database
ping 10.3.0.50

# Should work identically to in-office experience
```

4. Open Excel, test ODBC connection to Office3 database
   - Should work seamlessly

5. Test Office1 PostgreSQL access:

```powershell
# Ping Office1 PostgreSQL server
ping 10.1.0.100

# Should work (routes through Tailscale mesh to Office1 subnet router)
```

### Step 1.6: Gather Pilot Feedback

**User survey questions:**

- [ ] Did you notice any difference working from office vs. home?
- [ ] Did you need to manually connect to VPN? (Answer should be: No)
- [ ] How was PostgreSQL/database performance? (Excel query speed)
- [ ] Did you experience any connectivity issues?
- [ ] On a scale of 1-10, how "simple" was the experience?

**Performance benchmarks:**

- [ ] Measure Excel ODBC query time (in office vs. remote)
- [ ] Test RDP responsiveness
- [ ] Check Tailscale peer latency: `tailscale ping <peer-ip>`

**Decision point:** If pilot is successful, proceed to Phase 2 (full deployment).

---

## Phase 2: Subnet Router Deployment (All Offices)

**Objective:** Deploy subnet routers at Office1 and Office2 for full mesh network.

**Duration:** 1 week
**Prerequisites:** Phase 1 pilot completed successfully

### Step 2.1: Deploy Office1 Subnet Router

**On Office1 network (behind SonicWall TZ270):**

**Create Linux VM or dedicated device:**
- OS: Ubuntu 24.04 LTS
- Static IP: 10.1.0.250 (adjust to your network)
- CPU: 2 vCPU
- RAM: 1GB
- Disk: 10GB

**Install and configure Tailscale:**

```bash
# Install Tailscale
curl -fsSL https://tailscale.com/install.sh | sh

# Enable IP forwarding
echo 'net.ipv4.ip_forward = 1' | sudo tee -a /etc/sysctl.conf
sudo sysctl -p

# Start Tailscale and advertise Office1 subnet
sudo tailscale up --advertise-routes=10.1.0.0/24 --accept-routes

# Authenticate via browser URL provided
```

**Approve subnet routes in Tailscale admin console:**
1. Go to Tailscale Admin > Machines
2. Find Office1 subnet router
3. Edit route settings → Enable 10.1.0.0/24
4. Save

**Verify:**

```bash
sudo tailscale status

# Should show:
# - Connected to Tailscale network
# - Subnet routes: 10.1.0.0/24
# - Peer list includes Office3 subnet router
```

**Test connectivity:**

```bash
# From Office1 subnet router, ping Office3 database
ping 10.3.0.50

# Should work via Tailscale mesh
```

### Step 2.2: Deploy Office2 Subnet Router

**On Office2 network (behind SonicWall TZ270):**

Repeat Step 2.1 process with Office2-specific configuration:

```bash
# Install Tailscale
curl -fsSL https://tailscale.com/install.sh | sh

# Enable IP forwarding
echo 'net.ipv4.ip_forward = 1' | sudo tee -a /etc/sysctl.conf
sudo sysctl -p

# Start Tailscale and advertise Office2 subnet
sudo tailscale up --advertise-routes=10.2.0.0/24 --accept-routes
```

**Approve routes in admin console** (10.2.0.0/24)

**Verify mesh connectivity:**

```bash
# Test connectivity to all offices
ping 10.1.0.100  # Office1 PostgreSQL
ping 10.3.0.50   # Office3 database

# Both should work
```

### Step 2.3: Configure Subnet Router High Availability (Optional)

For critical sites (Office1), deploy redundant subnet router:

**Deploy second subnet router at Office1:**
- Install Tailscale on second VM/device
- Advertise same subnet (10.1.0.0/24)
- Tailscale automatically load-balances and provides failover

**Verify failover:**
1. Shut down primary subnet router
2. Test connectivity from remote user
3. Should automatically fail over to secondary router (may take 10-30 seconds)

---

## Phase 3: Azure AD SSO Integration

**Objective:** Enable seamless single sign-on with Microsoft 365 accounts.

**Duration:** 30-60 minutes
**Prerequisites:** Microsoft 365 Global Administrator access

### Step 3.1: Configure Tailscale for Azure AD

1. Go to <a href="https://login.tailscale.com/admin/settings/oauth" target="_blank">Tailscale Admin > Settings > OAuth</a>

2. Under **"Identity Provider"**, click **"Microsoft"**

3. Click **"Configure Microsoft"**

4. Follow wizard:
   - Sign in as Microsoft 365 Global Admin
   - Consent to Tailscale app permissions
   - Complete registration

5. Copy the **Tenant ID** and **Client ID** (save for reference)

### Step 3.2: Configure User Auto-Provisioning

**In Tailscale admin console:**

1. Go to **Settings > Users**

2. Enable **"Auto-approve new users from your organization"**

3. Set **Domain:** `yourdomain.com` (your Microsoft 365 domain)

4. Configure **User roles:**
   - Default role: **Member** (can access resources)
   - Admin role: Assign to IT staff manually

5. Save settings

**Result:** Any user with `user@yourdomain.com` Microsoft 365 account can auto-join Tailscale network by signing in with Microsoft.

### Step 3.3: Configure Multi-Factor Authentication (MFA)

**MFA is inherited from Azure AD** - no additional Tailscale configuration needed.

**Verify MFA works:**
1. Have test user sign in to Tailscale client
2. User should be prompted for MFA (via Microsoft Authenticator, SMS, etc.)
3. After MFA approval, Tailscale client connects

**Best practice:** Enforce MFA in Azure AD Conditional Access policies for all users.

### Step 3.4: Test SSO

**On a test laptop:**

1. Install Tailscale client
2. Click **"Sign in"**
3. Choose **"Sign in with Microsoft"**
4. User enters Microsoft 365 credentials
5. User completes MFA (if enabled)
6. Tailscale client connects automatically
7. User is now authenticated and authorized

**Verify:**
- User appears in Tailscale Admin > Machines
- User can access subnet routes (10.1.0.0/24, 10.3.0.0/24)

---

## Phase 4: Client Deployment via Intune/GPO

**Objective:** Auto-deploy Tailscale client to all 35 user laptops with zero-touch configuration.

**Duration:** 1-2 weeks (phased rollout)
**Methods:** Microsoft Intune (preferred) OR Group Policy (GPO)

### Option A: Deployment via Microsoft Intune (Recommended)

#### Step 4A.1: Prepare Tailscale Installation Package

1. Download Tailscale MSI installer:
   - <a href="https://tailscale.com/download/windows/msi" target="_blank">https://tailscale.com/download/windows/msi</a>
   - File: `tailscale-setup-x.x.x.msi`

2. Create deployment configuration file (`tailscale-config.json`):

```json
{
  "ServerURL": "https://controlplane.tailscale.com",
  "AuthKey": "tskey-auth-xxxxxxxxxxxxx-xxxxxxxxxxxxxxxxxxxxxxxx",
  "ExitNodeID": "",
  "AdvertiseRoutes": "",
  "AcceptRoutes": true,
  "AllowIncomingConnections": false,
  "ShieldsUp": false,
  "ExitNodeAllowLANAccess": true,
  "UnattendedMode": true,
  "ForceReauth": false,
  "Locked": []
}
```

**Generate Auth Key:**
1. Go to <a href="https://login.tailscale.com/admin/settings/keys" target="_blank">Tailscale Admin > Settings > Keys</a>
2. Click **"Generate auth key"**
3. Configure:
   - **Reusable:** Yes
   - **Ephemeral:** No
   - **Pre-approved:** Yes
   - **Expires:** 90 days (or longer)
   - **Tags:** Add tag `tag:employee` (for ACL management)
4. Copy auth key: `tskey-auth-xxxxxxxxxxxxx`
5. Paste into `tailscale-config.json` → `AuthKey` field

**Note:** Auth key allows auto-authentication without user sign-in. For Azure AD SSO instead, omit AuthKey and users will be prompted to sign in with Microsoft on first launch.

**Recommended approach for hybrid:** Omit AuthKey, use Azure AD SSO (better security, user-based access control).

#### Step 4A.2: Upload to Intune

**In Microsoft Intune admin center:**

1. Go to **Apps > Windows > Add**

2. Select app type: **Line-of-business app**

3. Upload `tailscale-setup-x.x.x.msi`

4. Configure app information:
   - **Name:** Tailscale VPN Client
   - **Description:** Zero-touch secure network access for hybrid workers
   - **Publisher:** Tailscale Inc.
   - **Command-line arguments:** (leave blank for Azure AD SSO)

5. Configure requirements:
   - **Operating system:** Windows 10/11 64-bit
   - **Minimum OS:** Windows 10 1809 or later

6. Configure detection rules:
   - **Manually configure detection rules**
   - **Rule type:** MSI
   - **MSI product code:** (auto-detected from MSI)

7. Assign to user groups:
   - **Required:** All employees (auto-install on all devices)
   - **Available:** (optional) Self-service install from Company Portal

8. Save and publish

#### Step 4A.3: Configure Auto-Start Policy

**Create configuration profile:**

1. Go to **Devices > Configuration profiles > Create profile**

2. Platform: **Windows 10 and later**

3. Profile type: **Templates > Administrative Templates**

4. Configure settings:
   - **Computer Configuration > Windows Components > Task Scheduler > Task Creation**
   - Create startup task:
     ```
     Program: C:\Program Files\Tailscale\tailscale.exe
     Arguments: up --accept-routes
     Run: At system startup
     ```

5. Assign to all devices

6. Save

**Alternative (simpler):** Tailscale auto-starts after installation by default. No additional configuration needed.

#### Step 4A.4: Phased Rollout

**Week 1: Pilot group (5-10 users)**
- Assign app to pilot user group
- Monitor for issues
- Gather feedback

**Week 2: Office3 (remaining users)**
- Assign to Office3 users
- Verify seamless transition from pilot

**Week 3: Office2 (8 users)**
- Assign to Office2 users
- Monitor connectivity

**Week 4: Office1 (remaining users)**
- Complete rollout to all 35 users
- Decommission old VPN solutions

**Monitoring:**
- Check Intune app installation status
- Verify devices appear in Tailscale admin console
- Monitor helpdesk tickets for issues

### Option B: Deployment via Group Policy (GPO)

#### Step 4B.1: Prepare GPO Package

**On a domain controller or GPO management workstation:**

1. Download Tailscale MSI installer to network share:
   ```
   \\fileserver\software\tailscale\tailscale-setup-x.x.x.msi
   ```

2. Create startup script for auto-authentication:

**File: `tailscale-startup.bat`**

```batch
@echo off
REM Start Tailscale and auto-authenticate
"C:\Program Files\Tailscale\tailscale.exe" up --accept-routes
```

**Save to:** `\\fileserver\scripts\tailscale-startup.bat`

#### Step 4B.2: Create GPO

**In Group Policy Management Console:**

1. Create new GPO: **"Tailscale Client Deployment"**

2. Edit GPO:

**Computer Configuration > Policies > Software Settings > Software Installation:**
- Right-click → **New > Package**
- Browse to: `\\fileserver\software\tailscale\tailscale-setup-x.x.x.msi`
- Deployment method: **Assigned**
- Installation type: **Install this application at computer startup**

**Computer Configuration > Policies > Windows Settings > Scripts > Startup:**
- Click **Add**
- Script Name: `\\fileserver\scripts\tailscale-startup.bat`
- Script Parameters: (leave blank)

3. Link GPO to appropriate OUs:
   - Link to **"Company Computers"** OU
   - Or specific OUs for phased rollout

4. Enforce GPO and trigger update:

```powershell
# On client machines, force GPO update
gpupdate /force
```

5. Reboot client machines (or wait for next reboot)

**Verification:**
- Check if Tailscale installed: `C:\Program Files\Tailscale\`
- Check if running: `tailscale status`
- Check Tailscale admin console for new devices

### Step 4.5: Configure Auto-Approved Routes

**In Tailscale admin console:**

1. Go to **Settings > Route settings**

2. Enable **"Accept routes from devices automatically"**
   - This allows user clients to auto-accept subnet routes without approval

3. Configure **Auto-approval** for subnet routers:
   - Tag subnet routers with `tag:subnet-router`
   - Create ACL rule (see Phase 5) to auto-approve routes from these tags

4. Save

---

## Phase 5: Access Control Lists (ACLs)

**Objective:** Configure granular access control for security and compliance.

**Duration:** 1-2 hours
**Prerequisites:** All subnet routers and clients deployed

### Step 5.1: Understanding Tailscale ACLs

Tailscale ACLs define:
- **Who** can access **what** resources
- **From where** (tags, groups, users)
- **On which ports** (PostgreSQL 5432, RDP 3389, etc.)

**ACL Format:** JSON-based policy file in Tailscale admin console

### Step 5.2: Basic ACL Configuration

**In Tailscale admin console:**

1. Go to **Settings > Access Controls**

2. Edit ACL policy:

```json
{
  "tagOwners": {
    "tag:subnet-router": ["autogroup:admin"],
    "tag:employee": ["autogroup:admin"]
  },

  "acls": [
    // Allow all employees to access Office1 PostgreSQL database
    {
      "action": "accept",
      "src": ["tag:employee"],
      "dst": ["10.1.0.100:5432"]
    },

    // Allow all employees to RDP to Office1 PCs
    {
      "action": "accept",
      "src": ["tag:employee"],
      "dst": ["10.1.0.10:3389", "10.1.0.11:3389", "10.1.0.12:3389"]
    },

    // Allow Office1 users to access Office3 local database
    {
      "action": "accept",
      "src": ["user@yourdomain.com", "admin@yourdomain.com"],
      "dst": ["10.3.0.50:5432"]
    },

    // Allow subnet routers to communicate with each other
    {
      "action": "accept",
      "src": ["tag:subnet-router"],
      "dst": ["tag:subnet-router:*"]
    },

    // Allow all employees to access all office subnets (broad access)
    {
      "action": "accept",
      "src": ["tag:employee"],
      "dst": ["10.1.0.0/24:*", "10.2.0.0/24:*", "10.3.0.0/24:*"]
    }
  ],

  "autoApprovers": {
    "routes": {
      "10.1.0.0/24": ["tag:subnet-router"],
      "10.2.0.0/24": ["tag:subnet-router"],
      "10.3.0.0/24": ["tag:subnet-router"]
    }
  }
}
```

3. Click **"Save"**

4. Tailscale validates ACL syntax (fix any errors)

5. ACL is immediately enforced

### Step 5.3: Advanced ACL - Restrict by User Groups

**If you want more granular control (e.g., only Office1 users access Office3 database):**

**In Azure AD:**
1. Create security groups:
   - `Tailscale-Office1-Users`
   - `Tailscale-Office2-Users`
   - `Tailscale-Office3-Users`

2. Add users to appropriate groups

**In Tailscale ACL:**

```json
{
  "groups": {
    "group:office1-users": ["user1@yourdomain.com", "user2@yourdomain.com"],
    "group:office2-users": ["user3@yourdomain.com"],
    "group:office3-users": ["user4@yourdomain.com"]
  },

  "acls": [
    // Only Office1 users can access Office3 database
    {
      "action": "accept",
      "src": ["group:office1-users"],
      "dst": ["10.3.0.50:5432"]
    },

    // All users can access Office1 PostgreSQL
    {
      "action": "accept",
      "src": ["tag:employee"],
      "dst": ["10.1.0.100:5432"]
    }
  ]
}
```

**Note:** Manually maintaining group membership in ACL is tedious. Better to sync with Azure AD groups via API (advanced configuration).

### Step 5.4: Test ACL Enforcement

**Test allowed access:**
```powershell
# From Office1 user laptop, test PostgreSQL access
Test-NetConnection -ComputerName 10.1.0.100 -Port 5432

# Should succeed
```

**Test denied access:**
```powershell
# From Office2 user laptop, test Office3 database (if ACL denies)
Test-NetConnection -ComputerName 10.3.0.50 -Port 5432

# Should fail if ACL blocks this access
```

**Check ACL logs:**
- Go to Tailscale Admin > Logs
- Review allowed/denied connections
- Adjust ACL as needed

---

## PostgreSQL ODBC Configuration

**Objective:** Configure Excel ODBC connections to PostgreSQL database via Tailscale.

### Step 6.1: Install PostgreSQL ODBC Driver

**On all Windows laptops (via Intune or GPO):**

1. Download psqlODBC driver (64-bit):
   - <a href="https://www.postgresql.org/ftp/odbc/versions/msi/" target="_blank">https://www.postgresql.org/ftp/odbc/versions/msi/</a>
   - File: `psqlodbc_x64.msi`

2. Install via Intune or GPO (same process as Tailscale client)

3. Verify installation:
   - Open **ODBC Data Source Administrator (64-bit)**
   - Go to **Drivers** tab
   - Should see: **PostgreSQL Unicode(x64)**

### Step 6.2: Configure ODBC Data Source

**Option A: User configures manually (simple environments):**

**On user laptop:**

1. Open **ODBC Data Source Administrator (64-bit)**
   - Press `Win + R`, type `odbcad32`, press Enter

2. Go to **System DSN** tab (or User DSN)

3. Click **Add**

4. Select **PostgreSQL Unicode(x64)**, click **Finish**

5. Configure Data Source:
   - **Data Source:** `Office1_PostgreSQL`
   - **Description:** `Office1 Database via Tailscale`
   - **Server:** `10.1.0.100` (Office1 PostgreSQL server IP)
   - **Port:** `5432`
   - **Database:** `your_database_name`
   - **User Name:** `db_username`
   - **Password:** `db_password` (or leave blank for prompt)
   - **SSL Mode:** `prefer`

6. Click **Test** to verify connection
   - Should succeed if Tailscale is connected

7. Click **Save**

**Option B: Auto-configure via registry (GPO deployment):**

**Create registry import file (`postgres-odbc-config.reg`):**

```reg
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SOFTWARE\ODBC\ODBC.INI\Office1_PostgreSQL]
"Driver"="C:\\Program Files\\psqlODBC\\bin\\psqlodbc35w.dll"
"Description"="Office1 Database via Tailscale"
"Servername"="10.1.0.100"
"Port"="5432"
"Database"="your_database_name"
"Username"="db_username"
"SSLmode"="prefer"

[HKEY_LOCAL_MACHINE\SOFTWARE\ODBC\ODBC.INI\ODBC Data Sources]
"Office1_PostgreSQL"="PostgreSQL Unicode(x64)"
```

**Deploy via GPO:**
1. Save `.reg` file to network share
2. Create GPO startup script to import registry settings
3. Link GPO to user OUs

### Step 6.3: Configure Excel Connection

**In Microsoft Excel:**

1. Go to **Data** tab

2. Click **Get Data > From Other Sources > From ODBC**

3. Select Data Source: **Office1_PostgreSQL**

4. Enter credentials (if not saved in DSN)

5. Select tables/views to query

6. Click **Load** or **Transform Data** (Power Query)

**Connection String (for VBA or direct connections):**

```vba
Connection String:
DSN=Office1_PostgreSQL;
UID=db_username;
PWD=db_password;
```

**Alternative: Connection string without DSN:**

```vba
Driver={PostgreSQL Unicode(x64)};
Server=10.1.0.100;
Port=5432;
Database=your_database_name;
Uid=db_username;
Pwd=db_password;
SSLMode=prefer;
```

### Step 6.4: Performance Optimization for Excel ODBC

**Best Practices:**

1. **Use Power Query instead of legacy ODBC connections:**
   - Better performance, more robust
   - `Data > Get Data > From Other Sources > From ODBC`

2. **Limit result sets with WHERE clauses:**
   - Fetch only needed rows (avoid `SELECT *`)

3. **Use bound columns (SQLBindCol):**
   - More efficient than SQLGetData

4. **Enable query caching in Excel:**
   - Right-click connection > **Properties**
   - Set **Refresh control** to manual or scheduled

5. **Avoid RefreshAll on large datasets:**
   - Refresh specific queries instead of entire workbook

**Expected Performance:**
- **Local (in-office):** 1-10ms query overhead (Tailscale subnet router adds minimal latency)
- **Remote (from home):** 10-50ms query overhead (peer-to-peer mesh)
- **Baseline (no Tailscale):** 1-5ms (direct LAN)

**Performance is excellent for typical Excel use cases (< 100ms total query time).**

---

## RDP Configuration

**Objective:** Enable RDP access to Office1 PCs via Tailscale for hybrid workers.

### Step 7.1: Configure RDP Hosts

**On Office1 RDP PCs (3 machines):**

**Verify RDP is enabled:**

1. Open **System Properties** (`sysdm.cpl`)
2. Go to **Remote** tab
3. Enable **"Allow remote connections to this computer"**
4. Uncheck **"Allow connections only from computers running Remote Desktop with Network Level Authentication"** (if needed for compatibility)
5. Click **Select Users**
6. Add authorized users (all employees or specific groups)
7. Click **OK**

**Configure Windows Firewall:**
```powershell
# Allow RDP through firewall
Enable-NetFirewallRule -DisplayGroup "Remote Desktop"
```

**Document RDP host IPs:**
- RDP PC 1: `10.1.0.10` - `office1-rdp1`
- RDP PC 2: `10.1.0.11` - `office1-rdp2`
- RDP PC 3: `10.1.0.12` - `office1-rdp3`

### Step 7.2: Configure Tailscale MagicDNS (Optional)

**Enable MagicDNS for easy hostname access:**

**In Tailscale admin console:**

1. Go to **DNS** settings

2. Enable **MagicDNS**

3. Devices are now accessible by hostname:
   - `office1-rdp1.tailnet-xxxx.ts.net`
   - OR set custom names in Tailscale admin

**Rename devices for clarity:**
1. Go to **Machines**
2. Click device → **Edit machine name**
3. Rename:
   - `office1-rdp1`
   - `office1-rdp2`
   - `office1-rdp3`

**Now users can RDP to:** `office1-rdp1` (instead of `10.1.0.10`)

### Step 7.3: User RDP Instructions

**For users connecting to RDP PCs:**

**From Windows laptop (in office or remote):**

1. Ensure Tailscale is connected (green checkmark in tray)

2. Open **Remote Desktop Connection** (`mstsc.exe`)

3. Enter computer name:
   - By IP: `10.1.0.10`
   - OR by MagicDNS name: `office1-rdp1`

4. Click **Connect**

5. Enter Windows credentials:
   - Username: `DOMAIN\username` or `username@yourdomain.com`
   - Password: (Active Directory password)

6. Click **OK**

7. RDP session opens

**Connection works identically from office or remote** - Tailscale routes traffic automatically.

### Step 7.4: RDP Performance Optimization

**For best RDP experience over Tailscale:**

**On user laptops, configure RDP settings:**

1. Open Remote Desktop Connection
2. Click **Show Options**
3. **Experience** tab:
   - Connection speed: **LAN (10 Mbps or higher)**
   - Uncheck unnecessary features:
     - Desktop background (for slower connections)
     - Font smoothing
     - Desktop composition
4. **Display** tab:
   - Remote desktop size: **Full Screen** or custom
   - Colors: **True Color (24 bit)** or **Highest Quality (32 bit)**
5. **Local Resources** tab:
   - Audio: **Play on this computer** (or **Do not play**)
   - Printers: Uncheck if not needed
6. Save settings

**Expected RDP Latency:**
- **In-office:** 1-5ms (near-native LAN performance)
- **Remote (home):** 10-30ms (excellent for interactive work)
- **Remote (international):** 50-100ms (still usable)

**Troubleshooting slow RDP:**
- Check Tailscale peer latency: `tailscale ping 10.1.0.10`
- Verify direct peer-to-peer connection (not relayed)
- Reduce RDP display quality settings

---

## Testing & Validation

### Step 8.1: Connectivity Testing Checklist

**Test from each office location:**

**Office1 User (in office):**
- [ ] Tailscale connected (green checkmark)
- [ ] Ping Office1 PostgreSQL: `ping 10.1.0.100` (should work)
- [ ] Ping Office3 database: `ping 10.3.0.50` (should work via mesh)
- [ ] Excel ODBC to Office1 PostgreSQL (should work)
- [ ] RDP to Office1 PCs: `mstsc /v:10.1.0.10` (should work)

**Office2 User (in office):**
- [ ] Tailscale connected
- [ ] Ping Office1 PostgreSQL: `ping 10.1.0.100` (should work via subnet router or IPSec)
- [ ] Excel ODBC to Office1 PostgreSQL (should work)
- [ ] RDP to Office1 PCs (should work)

**Office3 User (in office):**
- [ ] Tailscale connected
- [ ] Ping Office3 local database: `ping 10.3.0.50` (should work locally)
- [ ] Ping Office1 PostgreSQL: `ping 10.1.0.100` (should work via mesh)
- [ ] Excel ODBC to Office1 PostgreSQL (should work)

**Remote User (from home):**
- [ ] Tailscale auto-connects on home WiFi
- [ ] Ping Office1 PostgreSQL: `ping 10.1.0.100` (should work)
- [ ] Excel ODBC to Office1 PostgreSQL (should work)
- [ ] RDP to Office1 PCs (should work)
- [ ] Ping Office3 database: `ping 10.3.0.50` (should work)

### Step 8.2: Performance Testing

**Measure latency:**

```powershell
# Check Tailscale peer latency
tailscale ping 10.1.0.100

# Expected:
# - In-office: 1-5ms
# - Remote: 10-50ms
```

**Test PostgreSQL query performance:**

**In Excel:**
1. Open Power Query connection to PostgreSQL
2. Run sample query (e.g., `SELECT * FROM table LIMIT 100`)
3. Measure time to completion
4. Compare in-office vs. remote performance

**Expected:**
- Simple queries (< 100 rows): < 1 second
- Medium queries (100-1000 rows): 1-5 seconds
- Large queries (1000+ rows): 5-30 seconds (depending on data volume)

**If performance is poor:**
- Check Tailscale status: `tailscale status`
- Verify direct peer-to-peer connection (not relayed)
- Check network quality: `tailscale netcheck`

**Test RDP responsiveness:**
1. Connect via RDP to Office1 PC
2. Open applications, type in text fields
3. Assess latency and responsiveness
4. Expected: < 50ms latency for smooth interactive experience

### Step 8.3: Failover Testing

**Test subnet router failover (if redundant routers deployed):**

1. Identify primary subnet router (Office1)
2. Shut down primary router VM
3. From remote user laptop, test connectivity:
   ```powershell
   ping 10.1.0.100
   ```
4. Should automatically fail over to secondary router (10-30 seconds)
5. Verify connectivity restored
6. Restart primary router
7. Verify automatic return to primary

**Test user client auto-reconnect:**

1. Disconnect user laptop from internet (unplug Ethernet or disable WiFi)
2. Wait 30 seconds
3. Reconnect to internet
4. Tailscale should auto-reconnect (5-10 seconds)
5. Verify connectivity restored: `tailscale status`

### Step 8.4: Security Testing

**Verify ACL enforcement:**

**Test allowed access:**
```powershell
# User should have access to PostgreSQL
Test-NetConnection -ComputerName 10.1.0.100 -Port 5432
# Should succeed (TcpTestSucceeded: True)
```

**Test denied access (if ACL configured to deny):**
```powershell
# If ACL denies access to a specific resource
Test-NetConnection -ComputerName 10.1.0.50 -Port 3389
# Should fail (TcpTestSucceeded: False) if ACL blocks
```

**Verify Azure AD SSO:**
1. Install Tailscale client on test laptop
2. Sign in with Microsoft account
3. Verify MFA prompt (if Azure AD MFA enabled)
4. Confirm auto-authorization and access to resources

**Test device de-authorization:**
1. In Tailscale admin console, remove a test device
2. On that device, verify connectivity is lost
3. Attempt to access resources (should fail)
4. Re-authorize device, verify access restored

---

## User Quick-Start Guide (1-Page)

### Tailscale Quick-Start for Hybrid Workers

**Welcome! Your IT team has deployed Tailscale to give you seamless access to company resources from anywhere.**

---

#### First-Time Setup (One-Time, 2 Minutes)

**1. Sign In to Tailscale**

Your Tailscale client has been automatically installed on your laptop.

**Find the Tailscale icon in your system tray** (bottom-right corner of Windows taskbar):
- Look for the Tailscale icon: ![Tailscale Icon]

**Click the Tailscale icon** and select **"Sign in"**

**Choose "Sign in with Microsoft"**

**Enter your company email and password** (same as your Microsoft 365 login)

**Complete Multi-Factor Authentication** if prompted (via Microsoft Authenticator app)

**That's it!** Tailscale will now connect automatically whenever you use your laptop.

---

#### Daily Use (Zero Interaction Required)

**You don't need to do anything!**

- Tailscale starts automatically when you turn on your laptop
- It connects automatically to the company network (no "Connect VPN" button)
- All your applications work identically whether you're in the office or at home

**You'll see a small Tailscale icon in your system tray showing you're connected (green checkmark).**

---

#### Accessing Company Resources

**Everything works exactly the same from office or home:**

**PostgreSQL Database (Excel):**
- Open Excel
- Go to Data → Get Data → From Other Sources → From ODBC
- Select "Office1_PostgreSQL"
- Your data loads normally

**Remote Desktop (RDP to Office PCs):**
- Press `Win + R`, type `mstsc`, press Enter
- Enter computer name: `10.1.0.10` (or `office1-rdp1`)
- Click Connect
- Enter your Windows username and password

**No VPN button to click. No extra steps. It just works.**

---

#### Troubleshooting

**"I can't access the database"**

1. Check if Tailscale is connected:
   - Look for Tailscale icon in system tray
   - Icon should have a green checkmark
   - If not connected, click icon → "Connect"

2. If still not working, restart Tailscale:
   - Right-click Tailscale icon
   - Select "Quit Tailscale"
   - Wait 10 seconds
   - Tailscale will restart automatically

3. If issue persists, contact IT helpdesk

**"Tailscale icon is missing"**

- Tailscale may be running in background
- Open Start Menu, search for "Tailscale"
- Click "Tailscale" app to open

**"I'm working from a coffee shop and it's slow"**

- Public WiFi can be slow - this is normal
- Tailscale adds minimal latency (10-30ms)
- If very slow, try moving closer to WiFi router or using mobile hotspot

---

#### Important Notes

**✅ DO:**
- Keep Tailscale running (leave it in system tray)
- Connect to any WiFi network - Tailscale works on all networks
- Contact IT if you have connection issues

**❌ DON'T:**
- Uninstall Tailscale (you need it to access company resources remotely)
- Share your Tailscale login with anyone
- Disable Tailscale auto-start

---

**Questions? Contact IT Helpdesk: [helpdesk@yourcompany.com]**

---

## Troubleshooting

### Common Issues and Solutions

#### Issue: Tailscale won't connect

**Symptoms:**
- Tailscale icon shows "Disconnected" or gray icon
- `tailscale status` shows "Stopped" or "No state"

**Solutions:**

1. **Restart Tailscale service:**
   ```powershell
   # In PowerShell (as Administrator)
   Restart-Service Tailscale
   ```

2. **Check internet connectivity:**
   ```powershell
   ping 8.8.8.8
   # Should succeed
   ```

3. **Check firewall rules:**
   - Ensure Windows Firewall allows Tailscale
   - Check corporate firewall allows outbound HTTPS (443) and UDP (3478, 41641)

4. **Re-authenticate:**
   ```powershell
   tailscale up
   # Follow authentication URL in browser
   ```

5. **Check Tailscale service status:**
   ```powershell
   Get-Service Tailscale
   # Should show "Running"
   ```

6. **Reinstall Tailscale client (last resort):**
   - Uninstall via Control Panel
   - Reboot
   - Reinstall from <a href="https://tailscale.com/download/windows" target="_blank">tailscale.com</a>

---

#### Issue: Can't access PostgreSQL database

**Symptoms:**
- Excel ODBC connection fails
- `ping 10.1.0.100` fails
- Error: "Connection timed out" or "Host unreachable"

**Solutions:**

1. **Verify Tailscale is connected:**
   ```powershell
   tailscale status
   # Should show connected devices and subnet routes
   ```

2. **Check subnet routes:**
   ```powershell
   tailscale status
   # Look for: 10.1.0.0/24 via [Office1 subnet router]
   ```

3. **Test connectivity to subnet router:**
   ```powershell
   # Find Office1 subnet router IP (100.x.x.x)
   tailscale status

   # Ping subnet router
   ping 100.64.1.1  # (example Tailscale IP)
   ```

4. **Verify ACL allows access:**
   - Check Tailscale admin console → Access Controls
   - Ensure your user/device is allowed to access 10.1.0.100:5432

5. **Test direct PostgreSQL connectivity:**
   ```powershell
   Test-NetConnection -ComputerName 10.1.0.100 -Port 5432
   # Should show: TcpTestSucceeded: True
   ```

6. **Check PostgreSQL server is running:**
   - On Office1 subnet router, verify PostgreSQL server is online
   - Test local connectivity from subnet router: `ping 10.1.0.100`

---

#### Issue: RDP connection fails

**Symptoms:**
- Remote Desktop Connection shows "Can't connect to computer"
- Error: "Remote Desktop can't find the computer"

**Solutions:**

1. **Verify Tailscale connectivity (same as above)**

2. **Test RDP port accessibility:**
   ```powershell
   Test-NetConnection -ComputerName 10.1.0.10 -Port 3389
   # Should show: TcpTestSucceeded: True
   ```

3. **Check RDP is enabled on target PC:**
   - Remote into Office1 network (if possible)
   - Verify RDP is enabled in System Properties → Remote
   - Verify Windows Firewall allows RDP

4. **Try MagicDNS hostname instead of IP:**
   ```
   mstsc /v:office1-rdp1
   ```

5. **Check ACL allows RDP access:**
   - Tailscale admin → Access Controls
   - Ensure ACL allows your device to access 10.1.0.10:3389

---

#### Issue: Slow performance (high latency)

**Symptoms:**
- Excel queries take > 10 seconds
- RDP is laggy
- `tailscale ping` shows > 100ms latency

**Solutions:**

1. **Check if connection is relayed (not direct):**
   ```powershell
   tailscale status
   # Look for "relay" or "derp" in connection info
   # Ideally should show direct connection
   ```

2. **Force direct connection (disable relay):**
   ```powershell
   tailscale up --accept-routes --no-derp
   ```

3. **Check network quality:**
   ```powershell
   tailscale netcheck
   # Shows latency to DERP servers and NAT traversal info
   ```

4. **Test baseline internet speed:**
   - Use speedtest.net
   - If home internet is slow, Tailscale will be slow

5. **Optimize RDP settings:**
   - Reduce display quality (see RDP Configuration section)
   - Disable desktop background, font smoothing

6. **Check for ISP throttling:**
   - Some ISPs throttle VPN traffic
   - Try mobile hotspot to compare performance

---

#### Issue: Subnet router offline

**Symptoms:**
- Tailscale status shows subnet router as offline
- Can't access office resources even though Tailscale is connected

**Solutions:**

1. **Check subnet router VM/device is powered on:**
   - Log into hypervisor (VMware, Hyper-V, etc.)
   - Verify subnet router VM is running

2. **Check subnet router internet connectivity:**
   - SSH into subnet router: `ssh admin@10.1.0.250`
   - Test internet: `ping 8.8.8.8`

3. **Restart Tailscale service on subnet router:**
   ```bash
   sudo systemctl restart tailscaled
   sudo tailscale up --advertise-routes=10.1.0.0/24 --accept-routes
   ```

4. **Check subnet router Tailscale status:**
   ```bash
   sudo tailscale status
   # Should show connected to control plane and advertising routes
   ```

5. **Verify routes are approved in admin console:**
   - Tailscale admin → Machines
   - Find subnet router → Edit route settings
   - Ensure subnet routes are enabled

6. **If redundant subnet router exists, traffic should auto-failover:**
   - Check if secondary subnet router is online
   - Failover should occur within 10-30 seconds

---

#### Issue: Azure AD authentication fails

**Symptoms:**
- "Sign in with Microsoft" fails
- Error: "Authentication failed" or "User not found"

**Solutions:**

1. **Verify user has Microsoft 365 account:**
   - User should have active M365 license
   - Email: `user@yourdomain.com`

2. **Check Tailscale tenant configuration:**
   - Tailscale admin → Settings → OAuth
   - Verify Microsoft Azure AD is configured
   - Verify correct tenant ID

3. **Re-consent Tailscale app in Azure AD:**
   - Azure AD admin portal → Enterprise Applications
   - Find "Tailscale"
   - Grant admin consent for all users

4. **Clear browser cache and retry authentication:**
   - Open authentication URL in incognito/private browser
   - Sign in with Microsoft credentials

5. **Check Azure AD Conditional Access policies:**
   - Ensure Tailscale app is not blocked by CA policies
   - Add Tailscale to trusted apps if needed

---

#### Issue: Device not appearing in Tailscale admin console

**Symptoms:**
- User installed client and signed in
- Device doesn't appear in Machines list

**Solutions:**

1. **Check if user authenticated correctly:**
   ```powershell
   tailscale status
   # Should show "Logged in as: user@yourdomain.com"
   ```

2. **Verify auto-approval is enabled:**
   - Tailscale admin → Settings → Users
   - Enable "Auto-approve new users from your organization"

3. **Manually approve device:**
   - Tailscale admin → Machines
   - Look for unapproved devices
   - Click "Approve"

4. **Check if device is tagged correctly:**
   - Device might be approved but not visible due to filtering
   - Check all device filters in admin console

---

## Maintenance & Operations

### Daily Operations

**No daily maintenance required.** Tailscale is designed for zero-touch operation.

**Optional monitoring:**
- Check Tailscale admin console for device status
- Review logs for connectivity issues
- Monitor subnet router uptime

### Weekly Tasks

- [ ] Review Tailscale admin console → Machines
  - Verify all devices are online
  - Check for unapproved devices
  - Remove old/decommissioned devices

- [ ] Review Access Logs (if compliance required)
  - Tailscale admin → Logs
  - Export logs for audit (if needed)

- [ ] Check subnet router health:
  ```bash
  # SSH to each subnet router
  sudo tailscale status
  uptime
  df -h  # Check disk space
  ```

### Monthly Tasks

- [ ] Update Tailscale clients (auto-update should handle this)
  - Verify clients are on latest version
  - Manually update if needed

- [ ] Review and update ACLs
  - Add/remove users as needed
  - Adjust access policies based on changes

- [ ] Test failover procedures (if redundant subnet routers)
  - Simulate subnet router failure
  - Verify automatic failover

- [ ] Review auth key expiration
  - Regenerate auth keys if expired
  - Update deployment packages with new keys

### Quarterly Tasks

- [ ] Security audit
  - Review all authorized devices
  - Remove inactive users/devices
  - Review ACL policies for least-privilege access

- [ ] Performance review
  - Gather user feedback on connectivity experience
  - Review latency metrics
  - Optimize routing if needed

- [ ] Disaster recovery test
  - Simulate complete subnet router failure
  - Verify recovery procedures
  - Document lessons learned

### Annual Tasks

- [ ] Review Tailscale subscription and pricing
  - Verify user count matches billing
  - Consider plan upgrades if needed (Premium for advanced features)

- [ ] Security compliance review
  - Audit access logs for compliance (GDPR, SOC2, etc.)
  - Document security controls

- [ ] Infrastructure upgrade planning
  - Plan subnet router hardware refresh
  - Consider redundancy improvements

---

## Appendix A: Tailscale CLI Reference

**Common commands:**

```powershell
# Show Tailscale status
tailscale status

# Connect to Tailscale network
tailscale up

# Connect with auto-accept routes
tailscale up --accept-routes

# Disconnect from Tailscale network
tailscale down

# Check network diagnostics
tailscale netcheck

# Ping a peer by Tailscale IP
tailscale ping 100.64.1.1

# Show Tailscale IP address
tailscale ip

# Show detailed status (JSON)
tailscale status --json

# Log out (de-authenticate device)
tailscale logout

# Show version
tailscale version

# Enable/disable SSH access (Tailscale SSH feature)
tailscale up --ssh
```

**Subnet router-specific commands (Linux):**

```bash
# Advertise subnet routes
sudo tailscale up --advertise-routes=10.1.0.0/24 --accept-routes

# Advertise multiple subnets
sudo tailscale up --advertise-routes=10.1.0.0/24,10.2.0.0/24 --accept-routes

# Check if IP forwarding is enabled
sysctl net.ipv4.ip_forward
# Should show: net.ipv4.ip_forward = 1

# Enable IP forwarding (persistent)
echo 'net.ipv4.ip_forward = 1' | sudo tee -a /etc/sysctl.conf
sudo sysctl -p
```

---

## Appendix B: Cost Summary

**Tailscale Starter Plan:**
- **35 users × $6/month = $210/month**
- **Annual (12 months): $2,520/year**

**Infrastructure costs:**
- **3 subnet router VMs:** $0 (use existing infrastructure)
- **Optional dedicated devices:** $100-300 per device (one-time)

**Total Annual Cost: $2,520** (software licensing only)

**Cost per user per month: $6**

**Comparison to alternatives:**
- Traditional VPN: $0 (included with SonicWall), but poor performance and complex for hybrid workers
- Twingate: $2,100/year (cheaper by $420/year, but slightly higher latency)
- SonicWall Cloud Secure Edge: Custom pricing (need quote)

---

## Appendix C: Support & Resources

**Official Tailscale Resources:**
- Documentation: <a href="https://tailscale.com/kb/" target="_blank">https://tailscale.com/kb/</a>
- Support: <a href="https://tailscale.com/contact/support" target="_blank">https://tailscale.com/contact/support</a>
- Community Forum: <a href="https://forum.tailscale.com/" target="_blank">https://forum.tailscale.com/</a>
- Status Page: <a href="https://status.tailscale.com/" target="_blank">https://status.tailscale.com/</a>

**Key Knowledge Base Articles:**
- Subnet Routers: <a href="https://tailscale.com/kb/1019/subnets" target="_blank">https://tailscale.com/kb/1019/subnets</a>
- Azure AD SSO: <a href="https://tailscale.com/kb/1347/azure-ad" target="_blank">https://tailscale.com/kb/1347/azure-ad</a>
- ACLs: <a href="https://tailscale.com/kb/1018/acls" target="_blank">https://tailscale.com/kb/1018/acls</a>
- MagicDNS: <a href="https://tailscale.com/kb/1081/magicdns" target="_blank">https://tailscale.com/kb/1081/magicdns</a>

**Internal Contacts:**
- IT Helpdesk: [helpdesk@yourcompany.com]
- Tailscale Admin: [it-admin@yourcompany.com]
- Network Team: [network@yourcompany.com]

---

**End of Tailscale Hybrid Deployment Guide**

**Document Version:** 1.0
**Last Updated:** February 6, 2026
**Next Review:** After Phase 4 deployment completed
