# IPSec Site-to-Site + SonicWall Mobile Connect Deployment Guide
## Traditional VPN Approach for Multi-Site Connectivity and Remote Access

**Document Version:** 1.0
**Date:** February 6, 2026
**Environment:** Office1 (SonicWall TZ270), Office2 (SonicWall TZ270), Office3 (Draytek Router)
**Target:** Site-to-site connectivity with remote access for hybrid workers

---

## Executive Summary

This guide deploys a traditional VPN solution using:
- **IPSec site-to-site tunnels** for transparent inter-office connectivity
- **SonicWall Mobile Connect** VPN client for remote worker access

**Key Benefits:**
- ✅ **$0 additional cost** - Uses existing SonicWall/Draytek infrastructure
- ✅ **Office users: completely transparent** - No software installation needed
- ✅ **Proven, mature technology** - IPSec is industry-standard
- ✅ **Works with existing hardware** - TZ270s and Draytek already have IPSec support

**Trade-offs:**
- ⚠️ **Remote workers must manually connect VPN** - Not automatic like Tailscale
- ⚠️ **Traditional hub-and-spoke architecture** - Higher latency than mesh networking
- ⚠️ **Hybrid workers must remember** - "Connect VPN at home, not at office"

**User Experience:**
- **In office:** Completely transparent - resources work seamlessly, zero software needed
- **At home:** Manual VPN connection required - click "Connect" in SonicWall Mobile Connect app
- **Daily interaction:** Office users: ZERO | Remote users: Click "Connect" when working from home

**Cost:** $0 (licenses included with SonicWall TZ270)

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Prerequisites](#prerequisites)
3. [Phase 1: IPSec Tunnel Configuration (Office1 ↔ Office3)](#phase-1-ipsec-tunnel-configuration-office1--office3)
4. [Phase 2: Optional IPSec Tunnel (Office2 ↔ Office3)](#phase-2-optional-ipsec-tunnel-office2--office3)
5. [Phase 3: Routing Configuration](#phase-3-routing-configuration)
6. [Phase 4: SonicWall Mobile Connect Setup](#phase-4-sonicwall-mobile-connect-setup)
7. [Phase 5: Client Deployment](#phase-5-client-deployment)
8. [PostgreSQL ODBC Configuration](#postgresql-odbc-configuration)
9. [RDP Configuration](#rdp-configuration)
10. [Testing & Validation](#testing--validation)
11. [User Guide for Remote Workers](#user-guide-for-remote-workers)
12. [Troubleshooting](#troubleshooting)
13. [Maintenance & Operations](#maintenance--operations)

---

## Architecture Overview

### Network Topology

```
┌──────────────────────────────────────────────────────────────┐
│                   Site-to-Site IPSec Tunnels                 │
└──────────────────────────────────────────────────────────────┘
           │                                    │
    ┌──────▼──────┐                      ┌──────▼──────┐
    │   Office1   │◄────────────────────►│   Office3   │
    │ SonicWall   │   IPSec Tunnel       │   Draytek   │
    │   TZ270     │                      │   Router    │
    │ 10.1.0.0/24 │                      │ 10.3.0.0/24 │
    └──────┬──────┘                      └──────┬──────┘
           │                                    │
           │◄──────────────┐                   │
    ┌──────▼──────┐        │            ┌──────▼──────┐
    │   Office2   │        │            │   3 Users   │
    │ SonicWall   │        │            │  Local DB   │
    │   TZ270     │        │            │             │
    │ 10.2.0.0/24 │        │            └─────────────┘
    └──────┬──────┘        │
           │               │
           ▼               │
    Existing IPSec    (Optional
    Tunnel            IPSec Tunnel
    Office1↔Office2)  Office2↔Office3)

Remote Workers:
┌─────────────────┐     ┌─────────────────┐     ┌─────────────┐
│  User Laptop    │────►│  SonicWall      │────►│  Office1    │
│  (Home/Travel)  │     │  Mobile Connect │     │  Resources  │
│                 │     │  VPN Client     │     │             │
│ Manual Connect  │     │  (SSL VPN)      │     │             │
└─────────────────┘     └─────────────────┘     └─────────────┘
```

### Components

**1. IPSec Site-to-Site Tunnels:**
- **Office1 ↔ Office3:** Primary tunnel for Office1 users to access Office3 database
- **Office1 ↔ Office2:** Existing tunnel (already configured)
- **Office2 ↔ Office3:** Optional tunnel for redundancy

**2. SonicWall Mobile Connect (Remote Access):**
- SSL VPN client for remote workers
- Connects to Office1 TZ270 VPN gateway
- Provides access to all office resources (Office1, Office2, Office3 via tunnels)

**3. Routing:**
- Static routes on each firewall/router to direct inter-office traffic through IPSec tunnels
- Users require no local routing configuration (transparent)

### Traffic Flows

**Scenario 1: Office1 user accessing local PostgreSQL database**
```
User Laptop (10.1.0.50) → Local LAN → PostgreSQL (10.1.0.100)
(Direct local access, no VPN involved)
```

**Scenario 2: Office2 user accessing Office1 PostgreSQL database**
```
User Laptop (10.2.0.50) → Office2 LAN → Office2 TZ270 → IPSec Tunnel → Office1 TZ270 → PostgreSQL
(Transparent via existing Office1↔Office2 IPSec tunnel)
```

**Scenario 3: Office1 user accessing Office3 database**
```
User Laptop (10.1.0.50) → Office1 LAN → Office1 TZ270 → IPSec Tunnel → Office3 Draytek → Office3 DB (10.3.0.50)
(Transparent via new Office1↔Office3 IPSec tunnel)
```

**Scenario 4: Remote user accessing Office1 PostgreSQL database**
```
User Laptop (Home WiFi) → SonicWall Mobile Connect → Office1 TZ270 VPN Gateway → Office1 LAN → PostgreSQL
(Manual VPN connection required, then access is transparent)
```

**Scenario 5: Remote user accessing Office3 database**
```
User Laptop (Home WiFi) → SonicWall Mobile Connect → Office1 TZ270 → IPSec Tunnel → Office3 Draytek → Office3 DB
(Manual VPN connection to Office1, then Office3 accessible via tunnel)
```

---

## Prerequisites

### Hardware & Firmware

**Office1:**
- [ ] SonicWall TZ270 with latest firmware
  - Minimum firmware: SonicOS 7.0 or later
  - Check: <a href="https://www.sonicwall.com/support/product-support/tz-series/" target="_blank">SonicWall Support</a>
- [ ] Active SonicWall support/license (for firmware updates)
- [ ] Public static IP address (or Dynamic DNS if dynamic IP)

**Office2:**
- [ ] SonicWall TZ270 with latest firmware
- [ ] Public static IP address

**Office3:**
- [ ] Draytek router (model: ___________)
  - Most Draytek models support IPSec (Vigor 2865, 2962, etc.)
- [ ] Latest firmware installed
- [ ] Public static IP address (or Dynamic DNS)

### Network Information Required

Document the following before starting:

**Office1:**
- Public IP address: `____________`
- LAN subnet: `____________` (e.g., 10.1.0.0/24)
- PostgreSQL server IP: `____________` (e.g., 10.1.0.100)
- RDP host IPs: `____________, ____________, ____________`
- TZ270 LAN interface IP: `____________` (e.g., 10.1.0.1)
- TZ270 WAN interface IP: `____________` (public IP)

**Office2:**
- Public IP address: `____________`
- LAN subnet: `____________` (e.g., 10.2.0.0/24)
- TZ270 LAN interface IP: `____________` (e.g., 10.2.0.1)
- TZ270 WAN interface IP: `____________` (public IP)

**Office3:**
- Public IP address: `____________`
- LAN subnet: `____________` (e.g., 10.3.0.0/24)
- Local database IP: `____________` (e.g., 10.3.0.50)
- Draytek LAN interface IP: `____________` (e.g., 10.3.0.1)
- Draytek WAN interface IP: `____________` (public IP)

### Access Required

- [ ] Admin access to Office1 SonicWall TZ270 (web interface or SSH)
- [ ] Admin access to Office2 SonicWall TZ270
- [ ] Admin access to Office3 Draytek router
- [ ] Credentials for SonicWall admin console
- [ ] Credentials for Draytek admin console

### Firewall Rules Required

**No inbound firewall changes needed** - IPSec and SSL VPN use standard ports:
- **UDP 500** (IKE - Internet Key Exchange)
- **UDP 4500** (NAT-T - NAT Traversal for IPSec)
- **TCP 443** (HTTPS for SSL VPN / Mobile Connect)

These ports should already be open on WAN interfaces for VPN functionality.

---

## Phase 1: IPSec Tunnel Configuration (Office1 ↔ Office3)

**Objective:** Create IPSec site-to-site VPN tunnel between Office1 and Office3.

**Duration:** 1-2 hours
**Prerequisites:** Admin access to both firewalls, network information documented

### Step 1.1: Configure Office1 SonicWall (Side A)

**Log into Office1 SonicWall TZ270:**
1. Open web browser: `https://10.1.0.1` (or SonicWall LAN IP)
2. Enter admin credentials
3. Navigate to **VPN > Settings**

**Create VPN Policy:**

1. Go to **VPN > Settings > Add Policy**

2. **General Settings:**
   - **Policy Type:** Site to Site
   - **Authentication Method:** IKE using Preshared Secret
   - **Name:** `Office1-to-Office3`
   - **IPsec Primary Gateway Name or Address:** (Office3 public IP)
   - **Shared Secret:** (Create strong passphrase, 20+ characters)
     - Example: `Y7#mK9$pL3@vN8!qR2&zT6^wX4` (save this securely!)

3. **Network Settings:**
   - **Local Networks:**
     - Click **Add**
     - **Type:** Subnet
     - **Network:** `10.1.0.0`
     - **Netmask:** `255.255.255.0` (or /24)
     - Click **Add**

   - **Peer Networks:**
     - Click **Add**
     - **Type:** Subnet
     - **Network:** `10.3.0.0`
     - **Netmask:** `255.255.255.0` (or /24)
     - Click **Add**

4. **Proposals (Phase 1 - IKE):**
   - **IKE Version:** IKEv2 (recommended) or IKEv1 (if Draytek only supports v1)
   - **Exchange:** Main Mode
   - **DH Group:** Group 14 (2048-bit) or Group 19 (256-bit ECP)
   - **Encryption:** AES-256
   - **Authentication:** SHA-256
   - **Life Time:** 28800 seconds (8 hours)

5. **Proposals (Phase 2 - IPsec):**
   - **Protocol:** ESP
   - **Encryption:** AES-256
   - **Authentication:** SHA-256
   - **Enable Perfect Forward Secrecy:** Yes
   - **DH Group:** Group 14 or Group 19
   - **Life Time:** 3600 seconds (1 hour)

6. **Advanced Settings:**
   - **Enable Keep Alive:** Yes
   - **Keep Alive Interval:** 30 seconds
   - **Dead Peer Detection:** Enable

7. Click **Save**

**Configure NAT Exemption (Important!):**

Traffic between Office1 and Office3 should NOT be NAT'd. Create NAT exemption rule:

1. Go to **Firewall > NAT Policies**

2. Click **Add**

3. Configure:
   - **Original Source:** Address Object: `10.1.0.0/24`
   - **Translated Source:** Original
   - **Original Destination:** Address Object: `10.3.0.0/24`
   - **Translated Destination:** Original
   - **Original Service:** Any
   - **Translated Service:** Original
   - **Inbound Interface:** X1 (LAN)
   - **Outbound Interface:** X0 (WAN)
   - **Comment:** NAT Exemption for Office1-Office3 VPN

4. **Move rule to TOP of NAT policy list** (critical - must be evaluated before other NAT rules)

5. Click **Save**

**Create Access Rule (Firewall Policy):**

Allow traffic between Office1 and Office3 subnets:

1. Go to **Firewall > Access Rules**

2. Click **Add**

3. Configure:
   - **Action:** Allow
   - **Service:** Any
   - **Source:** Address Object: `10.1.0.0/24`
   - **Destination:** Address Object: `10.3.0.0/24`
   - **Comment:** Allow Office1 to Office3 via VPN

4. Click **Add** (create reciprocal rule)

5. Configure:
   - **Action:** Allow
   - **Service:** Any
   - **Source:** Address Object: `10.3.0.0/24`
   - **Destination:** Address Object: `10.1.0.0/24`
   - **Comment:** Allow Office3 to Office1 via VPN

6. Click **Save**

### Step 1.2: Configure Office3 Draytek (Side B)

**Log into Office3 Draytek Router:**
1. Open web browser: `https://10.3.0.1` (or Draytek LAN IP)
2. Enter admin credentials
3. Navigate to **VPN and Remote Access**

**Create VPN Profile:**

1. Go to **VPN and Remote Access > LAN to LAN**

2. **Select an unused VPN profile** (e.g., Profile 1)

3. Click **Edit**

4. **General Setup:**
   - **Enable this profile:** ✓
   - **Profile Name:** `Office3-to-Office1`
   - **Call Direction:** Dial-Out
   - **Dial-Out Settings:**
     - **Remote Gateway IP:** (Office1 public IP)
     - **Always On:** ✓ (keep tunnel always connected)

5. **IKE Authentication:**
   - **Pre-Shared Key:** (Same as Office1 shared secret)
     - Example: `Y7#mK9$pL3@vN8!qR2&zT6^wX4`

6. **Local/Remote Policy:**
   - **Local Network:**
     - **IP Address:** `10.3.0.0`
     - **Subnet Mask:** `255.255.255.0`

   - **Remote Network:**
     - **IP Address:** `10.1.0.0`
     - **Subnet Mask:** `255.255.255.0`

7. **IKE Phase 1 Proposal:**
   - **Authentication:** Pre-Shared Key
   - **Encryption:** AES-256
   - **Authentication Algorithm:** SHA-256
   - **DH Group:** 14 (2048-bit) or 19 (256-bit ECP)
   - **Key Life:** 28800 seconds

8. **IKE Phase 2 Proposal:**
   - **Encryption:** AES-256
   - **Authentication Algorithm:** SHA-256
   - **PFS:** Enable
   - **PFS Group:** 14 or 19
   - **Key Life:** 3600 seconds

9. **Dead Peer Detection (DPD):**
   - **Enable DPD:** ✓
   - **DPD Interval:** 30 seconds

10. Click **OK** to save

**Configure Firewall Rules:**

1. Go to **Firewall > General Setup**

2. Ensure VPN traffic is allowed:
   - Default rule: Allow VPN traffic (usually enabled by default)
   - If not, create rule: Allow `10.1.0.0/24` ↔ `10.3.0.0/24`

3. Click **OK**

### Step 1.3: Verify Tunnel Establishment

**On Office1 SonicWall:**

1. Go to **Dashboard > VPN**

2. Check VPN status:
   - Should show: `Office1-to-Office3` - **Connected** (green)
   - If not connected, check **VPN > Logs** for errors

3. Test connectivity from Office1 subnet:
   - From Office1 PC, ping Office3 device:
     ```powershell
     ping 10.3.0.50
     ```
   - Should receive replies

**On Office3 Draytek:**

1. Go to **VPN and Remote Access > Connection Management**

2. Check tunnel status:
   - Should show: `Office3-to-Office1` - **Connected**
   - Connection time, bytes transferred

3. Test connectivity from Office3 subnet:
   - From Office3 PC, ping Office1 device:
     ```cmd
     ping 10.1.0.100
     ```
   - Should receive replies

**If tunnel doesn't connect, see [Troubleshooting](#troubleshooting) section.**

---

## Phase 2: Optional IPSec Tunnel (Office2 ↔ Office3)

**Objective:** Create redundant path for Office2 users to access Office3 resources.

**Note:** This is optional. Office2 users can access Office3 via Office1 tunnel (Office2 → Office1 → Office3). However, a direct Office2 ↔ Office3 tunnel provides:
- Lower latency
- Redundancy (if Office1 tunnel fails)
- Load balancing

**Decision:** Skip this phase if Office2 traffic volume to Office3 is low.

### Step 2.1: Configure Office2 SonicWall

**Repeat Step 1.1 process on Office2 SonicWall:**

1. Log into Office2 SonicWall: `https://10.2.0.1`

2. Create VPN Policy: `Office2-to-Office3`
   - **IPsec Gateway:** Office3 public IP
   - **Shared Secret:** (Create new secret for Office2↔Office3 tunnel)
   - **Local Networks:** `10.2.0.0/24`
   - **Peer Networks:** `10.3.0.0/24`
   - **Proposals:** Same as Office1 configuration

3. Configure NAT exemption: `10.2.0.0/24` ↔ `10.3.0.0/24`

4. Create firewall access rules: Allow `10.2.0.0/24` ↔ `10.3.0.0/24`

5. Save configuration

### Step 2.2: Configure Office3 Draytek (Second Profile)

**On Office3 Draytek:**

1. Go to **VPN and Remote Access > LAN to LAN**

2. Select **Profile 2** (next unused profile)

3. Click **Edit**

4. Configure:
   - **Profile Name:** `Office3-to-Office2`
   - **Remote Gateway IP:** Office2 public IP
   - **Pre-Shared Key:** (Same as Office2 shared secret)
   - **Local Network:** `10.3.0.0/24`
   - **Remote Network:** `10.2.0.0/24`
   - **Proposals:** Same as Office1 configuration

5. Save

### Step 2.3: Verify Tunnel

**Test connectivity:**

From Office2 PC:
```powershell
ping 10.3.0.50
```

Should receive replies directly via Office2↔Office3 tunnel.

---

## Phase 3: Routing Configuration

**Objective:** Ensure proper routing for inter-office traffic.

**Duration:** 30 minutes

### Step 3.1: Office1 Routing

**On Office1 SonicWall:**

1. Go to **Network > Routing**

2. Verify route to Office3 exists:
   - **Destination:** `10.3.0.0/24`
   - **Gateway:** VPN Tunnel `Office1-to-Office3`
   - **Metric:** 1

3. If route doesn't exist, add manually:
   - Click **Add**
   - **Destination:** `10.3.0.0/24`
   - **Gateway:** Select VPN tunnel interface
   - **Metric:** 1
   - Click **OK**

4. Verify route to Office2 (existing):
   - **Destination:** `10.2.0.0/24`
   - **Gateway:** VPN Tunnel `Office1-to-Office2`

### Step 3.2: Office2 Routing

**On Office2 SonicWall:**

1. Go to **Network > Routing**

2. Add route to Office3 (if direct tunnel configured):
   - **Destination:** `10.3.0.0/24`
   - **Gateway:** VPN Tunnel `Office2-to-Office3`
   - **Metric:** 1

3. **OR (if no direct tunnel):** Add route via Office1:
   - **Destination:** `10.3.0.0/24`
   - **Gateway:** VPN Tunnel `Office2-to-Office1` (Office1 will forward to Office3)
   - **Metric:** 2 (higher metric, lower priority)

4. Verify route to Office1 (existing):
   - **Destination:** `10.1.0.0/24`
   - **Gateway:** VPN Tunnel `Office2-to-Office1`

### Step 3.3: Office3 Routing

**On Office3 Draytek:**

1. Go to **LAN > General Setup**

2. Check routing table:
   - Route to Office1 should be automatic via VPN profile
   - Route to Office2 (if direct tunnel configured)

3. If routes don't appear:
   - Go to **Route > Static Route**
   - Add:
     - **Destination:** `10.1.0.0/24`
     - **Gateway:** `0.0.0.0` (VPN tunnel)
     - **Interface:** VPN (Profile 1)

   - Add (if Office2 tunnel exists):
     - **Destination:** `10.2.0.0/24`
     - **Gateway:** `0.0.0.0`
     - **Interface:** VPN (Profile 2)

### Step 3.4: Verify Routing

**From Office1 PC, test reachability:**
```powershell
# Test Office3
ping 10.3.0.50

# Trace route to Office3 (should show tunnel path)
tracert 10.3.0.50
```

**From Office2 PC, test reachability:**
```powershell
# Test Office1
ping 10.1.0.100

# Test Office3
ping 10.3.0.50
```

**From Office3 PC, test reachability:**
```powershell
# Test Office1
ping 10.1.0.100

# Test Office2
ping 10.2.0.1
```

All pings should succeed. If not, check routing tables and firewall rules.

---

## Phase 4: SonicWall Mobile Connect Setup

**Objective:** Configure SSL VPN for remote workers to access office resources.

**Duration:** 1-2 hours
**Prerequisites:** SonicWall TZ270 with SSL VPN licenses (included with TZ270)

### Step 4.1: Enable SSL VPN on Office1 SonicWall

**Log into Office1 SonicWall:**

1. Go to **VPN > Settings > Client Settings**

2. **Enable SSL VPN:**
   - **Enable SSL VPN:** ✓
   - **SSL VPN Server Port:** 443 (default, uses HTTPS)
   - **Enable NetExtender:** ✓ (for full tunnel mode)
   - **SSL VPN Portal:** Customize (optional, see below)

3. **Configure IP Address Range for VPN Clients:**
   - **IP Assignment:** Use DHCP server or Static Range
   - **Static Range Example:**
     - **Start IP:** `10.1.100.1`
     - **End IP:** `10.1.100.50` (enough for 50 remote workers)
     - **Subnet Mask:** `255.255.255.0`

4. Click **Accept**

### Step 4.2: Create SSL VPN User Accounts

**Option A: Local Users (Simple, Small Deployments):**

1. Go to **Users > Local Users**

2. For each remote worker, click **Add**:
   - **User Name:** `user1` (or email: `user1@yourdomain.com`)
   - **Password:** (Strong password, or user sets on first login)
   - **VPN Access:** ✓ Enable SSL VPN
   - **Group:** Create group "Remote Workers" (for policy management)

3. Click **OK**

4. Repeat for all 35 users

**Option B: Azure AD / SAML SSO (Recommended for M365 Environments):**

1. Go to **Users > Settings > Authentication**

2. Click **Add** under **Authentication Servers**

3. Select **SAML**

4. Configure Azure AD SAML:
   - **Identity Provider Name:** `Azure AD`
   - **Entity ID:** (from Azure AD app)
   - **Single Sign-On URL:** (from Azure AD)
   - **Certificate:** Upload Azure AD signing certificate

5. Enable **Use this server for SSL VPN authentication**

6. Click **OK**

**Result:** Users authenticate with Microsoft 365 credentials (same as office login).

### Step 4.3: Configure SSL VPN Access Policy

**Create policy for remote workers:**

1. Go to **VPN > Settings > Client Routes**

2. **Configure Split Tunneling (Recommended):**
   - **Split Tunnel:** Enable
   - **Split Tunnel Routes:**
     - Add `10.1.0.0/24` (Office1)
     - Add `10.2.0.0/24` (Office2)
     - Add `10.3.0.0/24` (Office3)
   - **Result:** Only office traffic routes through VPN, internet traffic goes direct

   **Alternative: Full Tunnel:**
   - **Full Tunnel:** Enable (all traffic routes through VPN, including internet)
   - **Use case:** Maximum security, but slower internet for remote users

3. Click **Accept**

**Configure Access Rules for VPN Users:**

1. Go to **Firewall > Access Rules**

2. Click **Add**

3. Configure:
   - **Action:** Allow
   - **Service:** Any
   - **Source:** SSLVPN Users
   - **Destination:** Address Object: `10.1.0.0/24`, `10.2.0.0/24`, `10.3.0.0/24`
   - **Comment:** Allow SSL VPN users to access office resources

4. Click **OK**

### Step 4.4: Test SSL VPN Access

**From a test laptop (outside office network):**

1. Download SonicWall Mobile Connect client:
   - <a href="https://www.sonicwall.com/support/downloads/" target="_blank">SonicWall Downloads</a>
   - Or connect via web browser: `https://<Office1-Public-IP>/`

2. Install Mobile Connect client (Windows/Mac/iOS/Android)

3. Launch Mobile Connect

4. Configure connection:
   - **Server Address:** `<Office1-Public-IP>` or `vpn.yourcompany.com` (if DNS configured)
   - **Username:** `user1@yourdomain.com`
   - **Password:** (user password or Azure AD credentials)

5. Click **Connect**

6. VPN should connect (green checkmark or "Connected" status)

7. Test connectivity:
   ```powershell
   # Ping Office1 PostgreSQL
   ping 10.1.0.100

   # Ping Office3 database
   ping 10.3.0.50
   ```

Both should work via VPN tunnel.

---

## Phase 5: Client Deployment

**Objective:** Deploy SonicWall Mobile Connect client to all user laptops.

**Duration:** 1-2 weeks (phased rollout)

### Option A: Intune Deployment

**Prepare Mobile Connect Package:**

1. Download **SonicWall Mobile Connect MSI**:
   - <a href="https://www.sonicwall.com/support/downloads/" target="_blank">SonicWall Support Downloads</a>
   - File: `SonicWall-Mobile-Connect-x.x.x.msi`

2. Upload to Microsoft Intune:
   - Go to **Apps > Windows > Add**
   - Select **Line-of-business app**
   - Upload MSI file

3. Configure app information:
   - **Name:** SonicWall Mobile Connect VPN
   - **Description:** Remote access VPN for hybrid workers
   - **Publisher:** SonicWall

4. Assign to user groups:
   - **Required:** All employees (auto-install)

5. Save and publish

**Create Connection Profile (Optional):**

Pre-configure VPN connection for users:

Create XML configuration file (`MobileConnect-Config.xml`):

```xml
<?xml version="1.0" encoding="UTF-8"?>
<MobileConnect>
  <Connection>
    <Name>Company VPN</Name>
    <ServerAddress>vpn.yourcompany.com</ServerAddress>
    <Protocol>SSL</Protocol>
  </Connection>
</MobileConnect>
```

Deploy via Intune configuration policy:
1. **Devices > Configuration profiles > Create**
2. Upload XML file
3. Assign to all devices

### Option B: Group Policy (GPO) Deployment

**On domain controller:**

1. Download Mobile Connect MSI to network share:
   ```
   \\fileserver\software\sonicwall\SonicWall-Mobile-Connect.msi
   ```

2. Create GPO: **"SonicWall Mobile Connect Deployment"**

3. **Computer Configuration > Policies > Software Settings > Software Installation**
   - Add package: `\\fileserver\software\sonicwall\SonicWall-Mobile-Connect.msi`
   - Deployment: Assigned

4. Link GPO to user OUs

5. Force GPO update: `gpupdate /force`

### Option C: Manual Installation (Small Deployments)

**Provide users with download link:**

**Email template:**

```
Subject: Install SonicWall Mobile Connect for Remote Access

Dear Team,

To access company resources when working from home, please install the SonicWall Mobile Connect VPN client:

1. Download: https://www.sonicwall.com/support/downloads/
   (Select: Mobile Connect > Windows/Mac)

2. Install the application

3. Launch Mobile Connect

4. Connect using:
   - Server: vpn.yourcompany.com
   - Username: your-email@yourcompany.com
   - Password: your-regular-password

5. Click "Connect" whenever you need to access office resources from home

Questions? Contact IT Helpdesk: helpdesk@yourcompany.com

Best regards,
IT Department
```

---

## PostgreSQL ODBC Configuration

**Objective:** Configure Excel ODBC connections to PostgreSQL database.

**Note:** ODBC configuration is identical whether users are in office or connected via VPN. Database IP address remains the same (`10.1.0.100`).

### Step 6.1: Install PostgreSQL ODBC Driver

**On all user laptops (via Intune/GPO or manual):**

1. Download psqlODBC driver (64-bit):
   - <a href="https://www.postgresql.org/ftp/odbc/versions/msi/" target="_blank">PostgreSQL ODBC Downloads</a>
   - File: `psqlodbc_x64.msi`

2. Install (silent install for deployment):
   ```powershell
   msiexec /i psqlodbc_x64.msi /quiet /norestart
   ```

### Step 6.2: Configure ODBC Data Source

**Manual configuration (for each user):**

1. Open **ODBC Data Source Administrator (64-bit)**
   - Press `Win + R`, type `odbcad32`, press Enter

2. Go to **System DSN** tab

3. Click **Add**

4. Select **PostgreSQL Unicode(x64)**, click **Finish**

5. Configure:
   - **Data Source:** `Office1_PostgreSQL`
   - **Server:** `10.1.0.100`
   - **Port:** `5432`
   - **Database:** `your_database_name`
   - **User Name:** `db_username`
   - **SSL Mode:** `prefer`

6. Click **Test**:
   - **In office:** Should succeed immediately
   - **Remote (VPN disconnected):** Will fail
   - **Remote (VPN connected):** Should succeed

7. Click **Save**

**Automated configuration (via GPO registry import):**

See Tailscale guide Appendix for registry script example.

### Step 6.3: Excel Connection

**In Microsoft Excel:**

1. **Data** tab → **Get Data > From Other Sources > From ODBC**

2. Select: `Office1_PostgreSQL`

3. Enter credentials

4. Select tables/views

5. **Load** or **Transform Data**

**Connection works identically in office or via VPN** - Excel doesn't know the difference.

---

## RDP Configuration

**Objective:** Enable RDP access to Office1 PCs for remote workers.

**Note:** RDP configuration is identical for office users and VPN users.

### Step 7.1: Enable RDP on Office1 PCs

**On each of the 3 RDP PCs:**

1. Open **System Properties** (`sysdm.cpl`)

2. **Remote** tab → Enable **"Allow remote connections to this computer"**

3. Click **Select Users** → Add authorized users

4. Click **OK**

5. Configure Windows Firewall:
   ```powershell
   Enable-NetFirewallRule -DisplayGroup "Remote Desktop"
   ```

### Step 7.2: User RDP Instructions

**For remote workers:**

**Before connecting to RDP, ensure VPN is connected:**

1. Launch **SonicWall Mobile Connect**

2. Click **Connect** (wait for "Connected" status)

3. Open **Remote Desktop Connection** (`mstsc.exe`)

4. Enter: `10.1.0.10` (or RDP PC hostname)

5. Click **Connect**

6. Enter Windows credentials

7. RDP session opens

**Connection works identically to in-office access** once VPN is connected.

---

## Testing & Validation

### Step 8.1: Office User Testing (Transparent Access)

**Office1 user (in office, no VPN needed):**

- [ ] Ping Office3 database: `ping 10.3.0.50` (via IPSec tunnel)
- [ ] Excel ODBC to PostgreSQL: Should work seamlessly
- [ ] RDP to Office1 PC: Should work locally

**Office2 user (in office):**

- [ ] Ping Office1 database: `ping 10.1.0.100` (via existing Office1↔Office2 tunnel)
- [ ] Ping Office3 database: `ping 10.3.0.50` (via Office2→Office1→Office3)
- [ ] Excel ODBC to Office1 PostgreSQL: Should work

**Office3 user (in office):**

- [ ] Ping Office1 database: `ping 10.1.0.100` (via IPSec tunnel)
- [ ] Excel ODBC to Office1 PostgreSQL: Should work

### Step 8.2: Remote User Testing (VPN Required)

**Remote worker (at home, VPN disconnected):**

- [ ] Attempt to ping Office1: `ping 10.1.0.100` → **Should FAIL** (expected)
- [ ] Attempt Excel ODBC connection → **Should FAIL** (expected)

**Remote worker (at home, VPN connected):**

- [ ] Connect SonicWall Mobile Connect → Should show "Connected"
- [ ] Ping Office1 database: `ping 10.1.0.100` → **Should SUCCEED**
- [ ] Ping Office3 database: `ping 10.3.0.50` → **Should SUCCEED** (via tunnel)
- [ ] Excel ODBC to Office1 PostgreSQL → **Should SUCCEED**
- [ ] RDP to Office1 PC: `mstsc /v:10.1.0.10` → **Should SUCCEED**

### Step 8.3: Hybrid Worker Testing

**User alternates between office and home:**

**Day 1: In office**
- [ ] User does NOT need to connect VPN
- [ ] All resources accessible directly via LAN

**Day 2: At home**
- [ ] User launches Mobile Connect, clicks "Connect"
- [ ] After VPN connects, all resources accessible identically
- [ ] User may forget VPN is connected (transparent after connection)

**Day 3: Returns to office**
- [ ] User should disconnect VPN (not required, but good practice)
- [ ] Resources accessible via LAN again

**Potential issue:** User forgets to connect VPN at home → Excel/RDP fails → User must remember to connect VPN.

**Training emphasis:** "At home, always connect VPN first before accessing company resources."

### Step 8.4: Performance Testing

**Measure latency:**

**In office:**
```powershell
# Office1 to Office3 via IPSec tunnel
ping 10.3.0.50

# Expected: 1-10ms (depending on internet connection quality)
```

**Remote (via VPN):**
```powershell
# Home → VPN → Office1 → Office3
ping 10.3.0.50

# Expected: 30-100ms (depends on home internet and distance to office)
```

**PostgreSQL query performance:**

**In office:**
- Simple queries: < 1 second
- Medium queries: 1-5 seconds

**Remote (VPN):**
- Simple queries: 1-3 seconds (slight increase due to VPN latency)
- Medium queries: 3-10 seconds

**Expected performance is acceptable for typical Excel use cases.**

---

## User Guide for Remote Workers

### SonicWall Mobile Connect User Guide

**Working from Home? Connect to the VPN First!**

---

#### Important: You MUST Connect VPN to Access Company Resources

When working from home or traveling, you need to connect to the company VPN **before** you can access:
- Office databases (Excel ODBC connections)
- Remote Desktop (RDP to office PCs)
- Office file shares
- Other company resources

**Think of VPN as your "key" to unlock access to company resources when you're not in the office.**

---

#### Connecting to VPN (Every Time You Work from Home)

**1. Find SonicWall Mobile Connect**

Look for the SonicWall icon on your desktop or Start Menu:
- Icon: SonicWall Mobile Connect
- Or search: Press `Win` key, type "SonicWall", press Enter

**2. Launch Mobile Connect**

The SonicWall window opens

**3. Enter Connection Details (First Time Only)**

If this is your first time:
- **Server:** `vpn.yourcompany.com` (or IP address provided by IT)
- **Username:** `your-email@yourcompany.com`
- **Password:** (your regular company password)

Click **Save** to remember these settings

**4. Click "Connect"**

- Button changes to "Connecting..."
- Wait 5-10 seconds
- Status changes to **"Connected"** (green checkmark)

**You're now connected! All company resources are accessible.**

**5. Work Normally**

- Open Excel → Connect to database (works normally)
- Open Remote Desktop → Connect to office PC (works normally)
- Access file shares (works normally)

**Everything works just like you're in the office.**

---

#### Disconnecting from VPN

**When you're done working:**

1. Open SonicWall Mobile Connect

2. Click **"Disconnect"**

3. Status changes to "Disconnected"

**You can now close your laptop or continue using it for personal internet browsing.**

---

#### Troubleshooting

**"I can't connect to the database"**

→ **Did you connect to VPN?**
- Check SonicWall Mobile Connect
- If it says "Disconnected", click "Connect"
- Wait for "Connected" status
- Try accessing database again

**"VPN won't connect - error message"**

→ **Check your internet connection:**
- Make sure your home WiFi is connected
- Try opening a website (Google.com) to verify internet works
- If internet works but VPN doesn't, contact IT Helpdesk

**"VPN is connected but I still can't access resources"**

→ **Try disconnecting and reconnecting:**
1. Click "Disconnect" in Mobile Connect
2. Wait 10 seconds
3. Click "Connect" again
4. Try accessing resources again

→ **If still not working, contact IT Helpdesk**

**"I forgot to connect VPN and my Excel file won't refresh"**

→ **No problem!**
1. Connect to VPN (Mobile Connect → Connect)
2. Wait for "Connected" status
3. Go back to Excel
4. Refresh your data (Data → Refresh All)
5. Should work now

---

#### Important Notes

**✅ DO:**
- Connect VPN every time you work from home
- Disconnect VPN when you're done working (saves company resources)
- Contact IT if VPN won't connect

**❌ DON'T:**
- Try to access company resources without VPN (it won't work)
- Share your VPN username/password with anyone
- Panic if you forget to connect VPN - just connect and try again

---

#### Quick Reference Card

**VPN Connection Steps (Print and Keep Handy):**

1. **Open:** SonicWall Mobile Connect (desktop icon or Start Menu)
2. **Enter:** vpn.yourcompany.com, your-email@yourcompany.com, password
3. **Click:** "Connect"
4. **Wait:** For "Connected" status (green checkmark)
5. **Work:** Open Excel, RDP, file shares - everything works normally
6. **Done:** Click "Disconnect" when finished

**Helpdesk:** helpdesk@yourcompany.com | Phone: (555) 123-4567

---

## Troubleshooting

### Issue: IPSec Tunnel Won't Connect

**Symptoms:**
- VPN status shows "Disconnected" or "Down"
- Can't ping remote office subnet

**Solutions:**

1. **Check Phase 1 mismatch:**
   - Verify shared secret matches on both sides (case-sensitive)
   - Verify encryption/authentication algorithms match
   - Check SonicWall VPN logs: **VPN > Logs**

2. **Check firewall rules:**
   - Ensure UDP 500 and UDP 4500 are allowed on WAN interface
   - Check if ISP blocks VPN ports (some residential ISPs do)

3. **Verify public IP addresses:**
   - Confirm public IPs haven't changed (if using DHCP)
   - Update VPN configuration with correct IPs

4. **Check NAT-T:**
   - If both sides are behind NAT, enable NAT-T
   - SonicWall: Enable "NAT Traversal" in VPN settings

5. **Restart VPN tunnel:**
   - SonicWall: Right-click tunnel → "Reset Connection"
   - Draytek: Disable/enable VPN profile

6. **Check DPD settings:**
   - Ensure Dead Peer Detection settings match on both sides

**Debug commands:**

**SonicWall:**
- View VPN logs: **VPN > Logs**
- Diagnostic: **System > Diagnostics > Packet Capture**

**Draytek:**
- View logs: **System Maintenance > Syslog/Mail Alert**
- VPN status: **VPN and Remote Access > Connection Management**

---

### Issue: Remote User Can't Connect VPN

**Symptoms:**
- Mobile Connect shows "Connection failed" or "Authentication failed"

**Solutions:**

1. **Check credentials:**
   - Username: Should be `user@yourdomain.com` (full email, not just username)
   - Password: Check Caps Lock, verify correct password

2. **Check SSL VPN is enabled:**
   - SonicWall: **VPN > Settings > Client Settings** → "Enable SSL VPN" should be checked

3. **Check user VPN access:**
   - **Users > Local Users** → Select user → Verify "SSL VPN" is enabled

4. **Check firewall allows TCP 443:**
   - User's home/hotel firewall may block port 443
   - Try different network (mobile hotspot) to test

5. **Check SonicWall SSL VPN logs:**
   - **VPN > Logs** → Filter by SSL VPN
   - Look for authentication failures, errors

6. **Check client version:**
   - Ensure Mobile Connect client is latest version
   - Download from SonicWall support site

7. **Try web-based NetExtender:**
   - User navigates to: `https://<Office1-Public-IP>/`
   - Login via web browser
   - NetExtender launches in browser (no client install needed)

---

### Issue: Slow VPN Performance

**Symptoms:**
- Excel queries take > 30 seconds
- RDP is very laggy
- File transfers are slow

**Solutions:**

1. **Check user's home internet speed:**
   - Run speed test: speedtest.net
   - VPN can't be faster than slowest link (home internet or office internet)

2. **Check VPN tunnel utilization:**
   - SonicWall: **Dashboard > Bandwidth Management**
   - If tunnel is saturated, consider bandwidth upgrade or QoS policies

3. **Enable split tunneling:**
   - **VPN > Settings > Client Routes** → Enable "Split Tunnel"
   - Only office traffic routes through VPN, internet traffic goes direct
   - Reduces VPN load, improves performance

4. **Check IPSec tunnel bandwidth:**
   - IPSec tunnels may saturate if many users accessing Office3 simultaneously
   - Monitor bandwidth: **Dashboard > Bandwidth**

5. **Optimize RDP settings:**
   - Reduce display quality: 16-bit color, disable desktop background
   - Use RemoteFX (if supported) for better compression

6. **Check for VPN server overload:**
   - Too many concurrent VPN users can overload TZ270
   - Consider upgrading to higher-end SonicWall model (TZ370, TZ470)

---

### Issue: Office3 Database Not Accessible from Office1

**Symptoms:**
- IPSec tunnel shows "Connected"
- But ping to Office3 subnet fails

**Solutions:**

1. **Check routing:**
   - Verify route exists: **Network > Routing**
   - Destination: `10.3.0.0/24`, Gateway: VPN tunnel
   - If route missing, add manually

2. **Check NAT exemption:**
   - Traffic may be getting NAT'd incorrectly
   - Verify NAT exemption rule is at TOP of NAT policy list
   - **Firewall > NAT Policies** → Move exemption rule to position #1

3. **Check firewall rules:**
   - Ensure traffic from `10.1.0.0/24` to `10.3.0.0/24` is allowed
   - **Firewall > Access Rules** → Verify "Allow" rule exists

4. **Test from Office1 firewall itself:**
   - SonicWall: **System > Diagnostics > Ping**
   - Ping Office3 device: `10.3.0.50`
   - If firewall can ping but users can't, routing issue on user subnet

5. **Check Office3 firewall rules:**
   - Draytek may be blocking return traffic
   - Verify Office3 firewall allows `10.1.0.0/24` → `10.3.0.0/24`

---

## Maintenance & Operations

### Daily Operations

**No daily maintenance required for IPSec tunnels** - they maintain themselves.

**Monitor VPN status:**
- Check SonicWall dashboard daily
- Verify IPSec tunnels show "Connected"
- Verify SSL VPN users can connect

### Weekly Tasks

- [ ] Review SonicWall VPN logs
  - Check for failed authentication attempts
  - Check for tunnel disconnections/reconnections
  - Investigate any errors

- [ ] Check tunnel uptime
  - **VPN > Status** → Verify tunnels have been up continuously
  - If tunnels are flapping (connecting/disconnecting), investigate

- [ ] Review remote user connections
  - **Users > Active Users** → Check who is connected via SSL VPN
  - Verify only authorized users

### Monthly Tasks

- [ ] Update SonicWall firmware
  - Check for latest SonicOS updates
  - Schedule maintenance window (evening/weekend)
  - Apply firmware updates, test VPN connectivity after

- [ ] Update Draytek firmware
  - Check Draytek support for firmware updates
  - Apply updates during maintenance window

- [ ] Review user accounts
  - Remove accounts for departed employees
  - Disable unused VPN access

- [ ] Test disaster recovery
  - Simulate Office1 SonicWall failure
  - Verify failover procedures (if redundancy configured)

### Quarterly Tasks

- [ ] Rotate IPSec shared secrets
  - Generate new pre-shared keys for IPSec tunnels
  - Update on both sides, test connectivity

- [ ] Review SSL VPN user passwords
  - Enforce password changes (if not using Azure AD SSO)
  - Disable weak passwords

- [ ] Performance review
  - Review VPN bandwidth utilization
  - Plan capacity upgrades if needed

### Annual Tasks

- [ ] SonicWall support renewal
  - Renew SonicWall support contract for firmware updates
  - Review licensing (SSL VPN user count)

- [ ] Security audit
  - Review VPN logs for security incidents
  - Audit user access (principle of least privilege)

- [ ] Disaster recovery test
  - Full test of VPN infrastructure failover
  - Document recovery procedures

---

## Appendix A: SonicWall CLI Commands

**Useful CLI commands for troubleshooting (SSH to SonicWall):**

```bash
# Show VPN status
show vpn-status

# Show IPsec SA (Security Associations)
show vpn ipsec-sa

# Show SSL VPN users
show vpn ssl-users

# Show routing table
show ip route

# Test connectivity
ping <IP-address>

# Show firewall rules
show access-rules

# Show NAT policies
show nat-policies

# Restart VPN service
restart vpn

# View real-time logs
show log type vpn
```

---

## Appendix B: Draytek CLI/Web Commands

**Draytek diagnostics (web interface):**

- **VPN Status:** VPN and Remote Access > Connection Management
- **Routing Table:** Route > Static Route
- **Ping Test:** Diagnostics > Ping/Trace Route
- **Logs:** System Maintenance > Syslog/Mail Alert
- **Restart VPN:** VPN and Remote Access > Connection Management > Disconnect/Connect

---

## Appendix C: Cost Summary

**Total Cost: $0 (No Additional Licensing Required)**

**Existing infrastructure:**
- Office1 SonicWall TZ270: SSL VPN included (up to 10 users standard, more with licenses)
- Office2 SonicWall TZ270: IPSec VPN included (unlimited tunnels)
- Office3 Draytek: IPSec VPN included

**Optional costs:**
- Additional SSL VPN user licenses (if > 10 concurrent users): ~$15-30/user/year
- SonicWall support renewal: ~$300-500/year (for firmware updates, support)

**Comparison to Tailscale:**
- Tailscale: $2,520/year
- IPSec + Mobile Connect: $0-500/year
- **Savings: $2,000-2,500/year**

**Trade-off:** Cost savings vs. user experience (manual VPN connection required for remote workers)

---

## Appendix D: Comparison to Tailscale

| Feature | IPSec + Mobile Connect | Tailscale |
|---------|----------------------|-----------|
| **Cost** | $0-500/year | $2,520/year |
| **Office Users** | Completely transparent (no software) | Subnet router (transparent) or client (visible) |
| **Remote Users** | Manual VPN connection required | Auto-connect (transparent) |
| **Hybrid Workers** | Must remember to connect VPN at home | Seamless (auto-connects everywhere) |
| **Performance** | Good (IPSec overhead ~5-10ms) | Excellent (P2P mesh, ~1-5ms) |
| **Latency** | Office1→Office3: 5-15ms | Office1→Office3: 1-10ms |
| **Remote Latency** | 50-150ms (hub-and-spoke) | 10-50ms (peer-to-peer) |
| **Setup Complexity** | Moderate (firewall configuration) | Simple (install clients, configure subnet routers) |
| **Maintenance** | Low (stable, mature technology) | Very Low (auto-updates, cloud-managed) |
| **User Complaints** | "I forgot to connect VPN" | None (it just works) |
| **Best For** | Budget-conscious, office-first users | Hybrid workers, best user experience |

**Recommendation:**
- Choose **IPSec + Mobile Connect** if: Budget is priority, users are mostly office-based
- Choose **Tailscale** if: Hybrid workers, user experience is priority, budget allows $2,520/year

---

**End of IPSec Site-to-Site + SonicWall Mobile Connect Deployment Guide**

**Document Version:** 1.0
**Last Updated:** February 6, 2026
**Next Review:** After deployment completed and user feedback gathered
