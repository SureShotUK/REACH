# Draytek Vigor 2865 VPN Connection Guide

**Purpose:** Secure VPN connection from Windows 11 PC to Draytek Vigor 2865 router at remote location
**Last Updated:** October 2025
**Security Level:** High (using L2TP over IPsec with AES encryption)

---

## Table of Contents

1. [Overview and Prerequisites](#overview)
2. [VPN Protocol Selection](#vpn-protocols)
3. [Part 1: Draytek Vigor 2865 Configuration](#part1)
4. [Part 2: Windows 11 VPN Client Setup](#part2)
5. [Testing and Verification](#testing)
6. [Troubleshooting](#troubleshooting)
7. [Security Hardening](#security-hardening)
8. [Maintenance and Monitoring](#maintenance)

---

## Overview and Prerequisites {#overview}

### What This Guide Covers

This guide will help you establish a secure VPN connection from your office Windows 11 PC to a remote network protected by a Draytek Vigor 2865 router. You will be able to:

- Access devices on the remote network as if you were physically there
- Securely transfer files to/from the remote location
- Manage remote devices and systems
- Maintain end-to-end encryption for all traffic

### Network Scenario

```
[Your Office PC] -----> Internet -----> [Draytek Vigor 2865] -----> [Remote Network]
   Windows 11                              Remote Location            (192.168.x.x)
```

### Prerequisites

#### At the Remote Location (Draytek Side):

- ✅ Draytek Vigor 2865 router installed and operational
- ✅ Internet connection active and stable
- ✅ Public IP address (static preferred) or DDNS hostname
- ✅ Administrator access to the Draytek web interface
- ✅ Firmware version 4.3.2 or newer (recommended for security)

**To check your public IP:**
- Log into the Draytek: Look at the WAN1/WAN2 status on the dashboard
- Or visit: https://whatismyip.com from a device on the remote network

**If you don't have a static public IP:**
- Use Draytek's built-in DDNS service or a third-party service like DynDNS, No-IP
- This guide will cover DDNS setup

#### At Your Office (Client Side):

- ✅ Windows 11 PC with administrator rights
- ✅ Internet connection
- ✅ The following information from the Draytek (you'll configure these in Part 1):
  - Public IP address or DDNS hostname
  - VPN username and password
  - Pre-shared key (PSK)
  - Remote network subnet (e.g., 192.168.1.0/24)

---

## VPN Protocol Selection {#vpn-protocols}

The Draytek Vigor 2865 supports multiple VPN protocols. Here's a comparison:

| Protocol | Security | Ease of Setup | Native Windows Support | Recommendation |
|----------|----------|---------------|----------------------|----------------|
| **L2TP over IPsec** | High | Medium | Yes (Built-in) | **Recommended** - Best balance |
| **IPsec (IKEv2)** | Very High | Complex | Yes (PowerShell) | Maximum security |
| **SSL VPN** | High | Easy | No (needs client) | Good alternative |
| **PPTP** | Low | Easy | Yes | ❌ **NOT RECOMMENDED** - Insecure |

### Why L2TP over IPsec?

This guide focuses on **L2TP over IPsec** because it:

- ✅ Provides strong encryption (AES-256 available)
- ✅ Works natively in Windows 11 (no additional software needed)
- ✅ Offers good compatibility and reliability
- ✅ Is well-documented and supported
- ✅ Balances security and ease of configuration

**Security Note:** Never use PPTP - it has known security vulnerabilities and can be cracked easily.

---

## Part 1: Draytek Vigor 2865 Configuration {#part1}

### Step 1.1: Access the Draytek Web Interface

1. From a device on the remote network, open a web browser
2. Navigate to the Draytek's IP address (default: `https://192.168.1.1`)
3. Log in with administrator credentials:
   - Default username: `admin`
   - Default password: `admin` (if not changed)
   - **⚠️ IMPORTANT:** Change the default password immediately if you haven't already

4. You should see the Draytek dashboard

**Cannot access the router?**
- Verify you're connected to the remote network
- Check if the router IP was changed from default
- Ensure your device is on the same subnet
- Try HTTP instead: `http://192.168.1.1`

### Step 1.2: Check and Update Firmware (Recommended)

**Why:** Firmware updates contain critical security patches and bug fixes.

1. Go to **System Maintenance > Firmware Upgrade**
2. Note your current firmware version
3. Click "Check Latest Firmware"
4. If an update is available:
   - Download it or use the online upgrade feature
   - ⚠️ **Warning:** Do not interrupt the upgrade process or power off the router
   - Wait 5-10 minutes for the upgrade and reboot to complete

**Current Recommended Version:** 4.3.2 or newer (as of October 2025)

### Step 1.3: Configure Dynamic DNS (If No Static IP)

**Skip this step if you have a static public IP address.**

If your remote location has a dynamic (changing) IP address, you need DDNS:

1. Go to **WAN > Dynamic DNS**
2. Enable **Dynamic DNS**
3. Choose a DDNS provider:
   - **DrayDDNS** (Draytek's free service) - Recommended for simplicity
   - Or use: DynDNS, No-IP, etc. (requires account registration)

**Using DrayDDNS:**

4. Select **DrayDDNS** from the provider dropdown
5. In the **Domain Name** field, create your hostname:
   - Example: `mycompany-remote.dray-dns.com`
   - Choose something memorable and professional
6. Click **Apply**
7. Wait 2-3 minutes for DNS propagation
8. Test by visiting: `https://[your-hostname].dray-dns.com` from an external network
9. **Record this hostname** - you'll need it for Windows 11 setup

**Using Third-Party DDNS (e.g., No-IP):**

4. First, create an account at https://www.noip.com (or your chosen provider)
5. Create a hostname (e.g., `mycompany-remote.ddns.net`)
6. Return to the Draytek DDNS page
7. Select your provider from the dropdown
8. Enter your DDNS account username and password
9. Enter the hostname you created
10. Click **Apply**

### Step 1.4: Configure L2TP/IPsec VPN Server

#### 1.4a: Enable L2TP Server

1. Go to **VPN and Remote Access > Remote Access Control**
2. Find the **L2TP / PPTP / PPPoE** section
3. Check the box for **Enable L2TP Server**
4. Configure the following settings:

**L2TP Server Settings:**

| Setting | Value | Notes |
|---------|-------|-------|
| **Enable L2TP Server** | ✅ Checked | Enables the VPN server |
| **CHAP Authentication** | ✅ Enabled | Required for Windows |
| **MS-CHAPv2 Authentication** | ✅ Enabled | Required for Windows |
| **PAP Authentication** | ❌ Disabled | Less secure |
| **IPsec Encryption** | ✅ Enabled | **CRITICAL - Must enable** |
| **IPsec Pre-Shared Key** | [Create strong key] | See below for guidelines |

**Creating a Strong Pre-Shared Key (PSK):**

The PSK is a shared password that secures your VPN. Create a strong one:

- **Minimum 20 characters** (longer is better)
- Mix of uppercase, lowercase, numbers, and symbols
- Random characters (don't use dictionary words)
- **Example generator:** Use Windows PowerShell on your office PC:

```powershell
-join ((48..57) + (65..90) + (97..122) + (33..47) | Get-Random -Count 32 | ForEach-Object {[char]$_})
```

This generates a 32-character random PSK. **Copy and save it securely** - you'll need it later.

**Example PSK:** `K9mP2$xL5nQ#7wR@8jT4vY&3zB6cN1fH`

5. Paste your PSK into the **IPsec Pre-Shared Key** field
6. Set **Encryption Algorithm** to **AES-256** (if available) or **AES-128** (minimum)
7. Set **Authentication Algorithm** to **SHA-256** or **SHA-1** (SHA-256 preferred)

#### 1.4b: Configure Client IP Pool

The Draytek will assign IP addresses to VPN clients from this pool.

1. Still on the **Remote Access Control** page
2. Find **Client IP Address Pool** section
3. Configure the following:

| Setting | Example Value | Notes |
|---------|---------------|-------|
| **Starting IP** | `192.168.1.200` | Choose IPs outside your DHCP range |
| **IP Pool Count** | `10` | Number of simultaneous VPN connections allowed |

**How to choose the IP range:**

- Check your current DHCP scope: **LAN > General Setup**
- Example: If DHCP range is `192.168.1.2` to `192.168.1.199`, use `192.168.1.200` onwards for VPN
- Ensure VPN pool doesn't overlap with DHCP or static IPs

4. Click **Apply** to save settings

#### 1.4c: Create VPN User Account

1. Go to **VPN and Remote Access > Remote Dial-In User**
2. Click **Add** to create a new user
3. Configure the user account:

| Field | Example Value | Recommendations |
|-------|---------------|-----------------|
| **User Name** | `office-admin` | Your username for connecting |
| **Password** | [Create strong password] | Min. 16 chars, mixed case, numbers, symbols |
| **Active** | ✅ Enabled | Must be checked |
| **Privilege** | **Admin** or **User** | Admin for full network access |
| **Allow Client IP** | Leave blank | Restricts by source IP (optional) |
| **VPN Type** | **L2TP** | Select L2TP only |

**Creating a Strong Password:**

Use a password manager or generate one:

```powershell
# PowerShell command to generate 16-char password
-join ((48..57) + (65..90) + (97..122) + (33..47) | Get-Random -Count 16 | ForEach-Object {[char]$_})
```

**Example:** `P9mL$2xK5nQ#7wR@`

4. Click **OK** to save the user
5. **Record these credentials securely** - you'll need them for Windows 11 setup

**💡 Pro Tip:** Create separate accounts for each user/device. This allows you to:
- Track who's connecting (via VPN logs)
- Revoke access individually if needed
- Audit connection activity

### Step 1.5: Configure Firewall Rules

The Draytek's firewall must allow VPN traffic to reach the router.

#### 1.5a: Create Firewall Rule for L2TP/IPsec

1. Go to **Firewall > Filter Setup**
2. Select the **WAN** interface (usually WAN1)
3. Find an empty rule slot (or click **Add**)
4. Configure the rule:

**Rule 1: Allow IPsec (UDP 500)**

| Field | Value |
|-------|-------|
| **Active** | ✅ Enabled |
| **Rule Name** | `Allow IPsec UDP 500` |
| **Source IP** | `Any` (or specific office IP if known) |
| **Destination IP** | `Default` |
| **Protocol** | `UDP` |
| **Source Port** | `Any` |
| **Destination Port** | `500` |
| **Action** | `Pass` |
| **Log** | ✅ Enabled (for troubleshooting) |

5. Click **OK**

**Rule 2: Allow IPsec NAT Traversal (UDP 4500)**

6. Add another rule with these settings:

| Field | Value |
|-------|-------|
| **Active** | ✅ Enabled |
| **Rule Name** | `Allow IPsec NAT-T UDP 4500` |
| **Source IP** | `Any` |
| **Destination IP** | `Default` |
| **Protocol** | `UDP` |
| **Source Port** | `Any` |
| **Destination Port** | `4500` |
| **Action** | `Pass` |
| **Log** | ✅ Enabled |

7. Click **OK**

**Rule 3: Allow L2TP (UDP 1701)**

8. Add a third rule:

| Field | Value |
|-------|-------|
| **Active** | ✅ Enabled |
| **Rule Name** | `Allow L2TP UDP 1701` |
| **Source IP** | `Any` |
| **Destination IP** | `Default` |
| **Protocol** | `UDP` |
| **Source Port** | `Any` |
| **Destination Port** | `1701` |
| **Action** | `Pass` |
| **Log** | ✅ Enabled |

9. Click **OK**
10. Click **Apply** to activate the firewall rules

**⚠️ Security Note:** If you have a static office IP, you can improve security by entering it in the **Source IP** field instead of `Any`. This restricts VPN access to only your office.

#### 1.5b: Check Default Firewall Policy

1. Still in **Firewall > Filter Setup**
2. Scroll to the bottom
3. Ensure the **Default Action** for WAN is **Deny** (blocks all except allowed traffic)

### Step 1.6: Enable VPN Pass-Through (If Behind Another Router/NAT)

**Skip this step if the Draytek has a direct public IP on its WAN port.**

If the Draytek is behind another router or ISP modem:

1. Go to **NAT > VPN Pass-Through**
2. Enable the following:
   - ✅ **L2TP Pass-Through**
   - ✅ **IPsec Pass-Through**
3. Click **Apply**

**Additional requirement:** You may need to configure port forwarding on the upstream router:
- Forward UDP ports 500, 4500, 1701 to the Draytek's IP

### Step 1.7: Verify and Record Configuration Details

Before moving to Windows 11 setup, verify and record these details:

**Connection Information Checklist:**

```
☐ Public IP Address or DDNS Hostname: _________________________________
☐ VPN Username: _________________________________
☐ VPN Password: _________________________________
☐ Pre-Shared Key (PSK): _________________________________
☐ Remote Network Subnet: _________________________________ (e.g., 192.168.1.0/24)
☐ VPN Client IP Pool Range: _________________________________ (e.g., 192.168.1.200-209)
```

**How to find your remote network subnet:**

1. In the Draytek, go to **LAN > General Setup**
2. Note the **IP Address** (e.g., `192.168.1.1`) and **Subnet Mask** (e.g., `255.255.255.0`)
3. The subnet is the IP with `.0` at the end: `192.168.1.0/24`

### Step 1.8: Optional - Enable VPN Logs for Troubleshooting

1. Go to **System Maintenance > Syslog/Mail Alert**
2. Enable **Syslog**
3. Under **Event Categories**, check:
   - ✅ **VPN** (logs all VPN connections and errors)
   - ✅ **Security** (logs authentication attempts)
4. Click **Apply**
5. View logs at: **System Maintenance > View Logs**

---

## Part 2: Windows 11 VPN Client Setup {#part2}

Now that the Draytek is configured, set up your Windows 11 PC to connect.

### Step 2.1: Gather Your Connection Information

You'll need the information you recorded in Step 1.7:

- [ ] Public IP address or DDNS hostname
- [ ] VPN username
- [ ] VPN password
- [ ] Pre-shared key (PSK)

### Step 2.2: Configure Registry for L2TP Behind NAT

**Why this is needed:** By default, Windows 11 won't connect to L2TP/IPsec VPNs if the server is behind NAT. This registry change enables it.

**⚠️ Warning:** You're editing the Windows registry. Follow these steps exactly.

1. Press **Windows Key + R** to open Run dialog
2. Type `regedit` and press **Enter**
3. Click **Yes** when User Account Control prompts
4. Navigate to this key (copy and paste into address bar at top):
   ```
   HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\PolicyAgent
   ```
5. Right-click on **PolicyAgent** folder > **New** > **DWORD (32-bit) Value**
6. Name it: `AssumeUDPEncapsulationContextOnSendRule`
7. Double-click the new value
8. Set **Value data** to: `2`
9. Click **OK**
10. Close Registry Editor
11. **Restart your computer** for the change to take effect

**Alternative method using PowerShell (Administrator):**

```powershell
# Run PowerShell as Administrator
Set-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Services\PolicyAgent" -Name "AssumeUDPEncapsulationContextOnSendRule" -Value 2 -Type DWord
Restart-Computer -Confirm
```

### Step 2.3: Create VPN Connection in Windows 11

After restarting:

1. Click the **Start button** or press **Windows Key**
2. Go to **Settings** (gear icon)
3. Select **Network & Internet** from the left sidebar
4. Click **VPN** in the right panel
5. Click **Add VPN**

Configure the VPN connection with these settings:

**VPN Connection Settings:**

| Field | Value | Notes |
|-------|-------|-------|
| **VPN provider** | Windows (built-in) | Select from dropdown |
| **Connection name** | `Remote Office VPN` | Choose a descriptive name |
| **Server name or address** | [Your DDNS hostname or IP] | e.g., `mycompany-remote.dray-dns.com` or `203.0.113.50` |
| **VPN type** | L2TP/IPsec with pre-shared key | **Important:** Select this exact option |
| **Pre-shared key** | [Your PSK from Step 1.4a] | Copy and paste carefully |
| **Type of sign-in info** | User name and password | Select from dropdown |
| **User name** | [VPN username from Step 1.4c] | e.g., `office-admin` |
| **Password** | [VPN password from Step 1.4c] | Enter carefully |
| **Remember my sign-in info** | ✅ Checked (optional) | Convenience vs. security trade-off |

6. Click **Save**

**Security Note:** If multiple people use this PC, don't save the password. They should only connect with authorized credentials.

### Step 2.4: Configure Advanced VPN Settings

The VPN connection needs additional settings for optimal security and functionality.

1. Still in **Settings > Network & Internet > VPN**
2. Find your VPN connection (**Remote Office VPN**)
3. Click the **three dots** (...) next to it
4. Select **Advanced options**

**Configure these settings:**

#### Connection Properties:

5. Scroll to **Related settings** section
6. Click **More adapter options**
7. This opens the Network Connections window
8. Find your VPN connection (**Remote Office VPN**)
9. Right-click on it > **Properties**

#### Security Tab:

10. Click the **Security** tab
11. Configure as follows:

| Setting | Value | Notes |
|---------|-------|-------|
| **Type of VPN** | L2TP/IPsec with pre-shared key | Should already be set |
| **Data encryption** | Require encryption (disconnect if server declines) | Maximum security |
| **Authentication** | Use Extensible Authentication Protocol (EAP) | Uncheck this |
| **Allow these protocols** | ✅ Microsoft CHAP Version 2 (MS-CHAP v2) | **Check ONLY this option** |

12. Click the **Advanced settings** button
13. Verify the **Pre-shared key** is correct
14. Click **OK**

#### Networking Tab:

15. Click the **Networking** tab
16. Under "This connection uses the following items", verify these are checked:
    - ✅ **Internet Protocol Version 4 (TCP/IPv4)**
    - ❌ **Internet Protocol Version 6 (TCP/IPv6)** - Uncheck unless you specifically use IPv6 at the remote site

17. Select **Internet Protocol Version 4 (TCP/IPv4)**
18. Click **Properties**
19. Click **Advanced**
20. **Uncheck** the option: **Use default gateway on remote network**

**Why uncheck "Use default gateway"?**
- If checked: All your internet traffic routes through the VPN (slower)
- If unchecked: Only traffic to the remote network uses the VPN; other internet traffic uses your local connection (split tunneling)

**Exception:** If you want all traffic encrypted through the remote site (e.g., for security), leave it checked.

21. Click **OK** on all open dialog boxes to save

### Step 2.5: Add Static Route (For Split Tunneling)

If you unchecked "Use default gateway" (split tunneling), you need to tell Windows which IPs should use the VPN.

**Add a static route for the remote network:**

1. Open **PowerShell as Administrator**:
   - Right-click **Start button** > **Terminal (Admin)** or **PowerShell (Admin)**
   - Click **Yes** when UAC prompts

2. Run this command (replace values with your remote network details):

```powershell
# Syntax: route -p add [remote_network] mask [subnet_mask] [vpn_gateway]
# Example for remote network 192.168.1.0/24:
route -p add 192.168.1.0 mask 255.255.255.0 0.0.0.0 metric 1
```

**How to determine the correct command:**

Your remote network subnet (from Step 1.7): `192.168.1.0/24`

| Network | Subnet Mask | Command |
|---------|-------------|---------|
| `192.168.1.0/24` | `255.255.255.0` | `route -p add 192.168.1.0 mask 255.255.255.0 0.0.0.0 metric 1` |
| `192.168.0.0/24` | `255.255.255.0` | `route -p add 192.168.0.0 mask 255.255.255.0 0.0.0.0 metric 1` |
| `10.0.0.0/24` | `255.255.255.0` | `route -p add 10.0.0.0 mask 255.255.255.0 0.0.0.0 metric 1` |

**Explanation:**
- `-p` flag makes the route persistent (survives reboots)
- `0.0.0.0` means "use the VPN interface"
- `metric 1` gives this route high priority

3. Press **Enter** to execute
4. You should see: `OK!`

**Verify the route was added:**

```powershell
route print 0.0.0.0*
```

You should see your remote network in the list.

**To remove a route (if you made a mistake):**

```powershell
route delete 192.168.1.0
```

### Step 2.6: Configure Windows Firewall for VPN

Ensure Windows Firewall allows VPN traffic:

1. Search for **Windows Defender Firewall** in the Start menu
2. Click **Allow an app or feature through Windows Defender Firewall**
3. Click **Change settings** button (requires admin)
4. Find **Routing and Remote Access** in the list
5. Check both **Private** and **Public** columns
6. Click **OK**

### Step 2.7: Test the VPN Connection

**First connection test:**

1. Go to **Settings > Network & Internet > VPN**
2. Click on **Remote Office VPN**
3. Click **Connect**
4. Wait 10-30 seconds for the connection to establish
5. If successful, you'll see **Connected** status

**If it fails**, see the [Troubleshooting](#troubleshooting) section.

---

## Testing and Verification {#testing}

Once connected, verify the VPN is working properly.

### Test 1: Check VPN Connection Status

**In Windows 11:**

1. Go to **Settings > Network & Internet > VPN**
2. Look for **Connected** status under your VPN connection
3. Note the **IP address assigned** (should be in the VPN client pool range)

**Example:** `192.168.1.200` (if your pool started at .200)

### Test 2: Ping the Remote Router

1. Open **Command Prompt** or **PowerShell**
2. Ping the Draytek's LAN IP:

```cmd
ping 192.168.1.1
```

**Expected result:**
```
Reply from 192.168.1.1: bytes=32 time=25ms TTL=64
Reply from 192.168.1.1: bytes=32 time=24ms TTL=64
Reply from 192.168.1.1: bytes=32 time=26ms TTL=64
```

If you see replies, the VPN is working!

**If you see "Request timed out"**, see troubleshooting.

### Test 3: Access Remote Devices

Try to access a device on the remote network:

**Test with ping:**

```cmd
# Replace with an actual device IP on the remote network
ping 192.168.1.50
```

**Test with Remote Desktop (if applicable):**

1. Press **Windows Key + R**
2. Type `mstsc` and press **Enter**
3. Enter a remote device's IP address (e.g., `192.168.1.50`)
4. Click **Connect**

**Test with file sharing (if applicable):**

1. Open **File Explorer**
2. In the address bar, type: `\\192.168.1.50` (replace with remote device IP)
3. Press **Enter**
4. You should see shared folders if available

### Test 4: Verify Split Tunneling (If Configured)

If you set up split tunneling, verify that non-VPN traffic still uses your local internet:

1. While connected to the VPN, visit: https://whatismyip.com
2. You should see **your local office IP** (not the remote site's IP)
3. This confirms only remote network traffic uses the VPN

**To test VPN traffic:**

```cmd
# This should show the VPN tunnel's IP
tracert 192.168.1.1
```

First hop should be your VPN gateway IP.

### Test 5: Check Draytek VPN Logs

On the Draytek side:

1. Log into the Draytek web interface (from the remote location or via VPN)
2. Go to **System Maintenance > View Logs**
3. Filter by **VPN** category
4. You should see entries like:
   - `L2TP tunnel established from [your office IP]`
   - `User 'office-admin' authenticated successfully`
   - `VPN connection established`

---

## Troubleshooting {#troubleshooting}

### Connection Fails - Error 809

**Error message:** "The network connection between your computer and the VPN server could not be established..."

**Causes and solutions:**

#### Solution 1: Registry setting not applied

- Verify you completed Step 2.2 (registry edit)
- Restart your computer after making the registry change
- Verify the registry value exists:

```powershell
Get-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Services\PolicyAgent" -Name "AssumeUDPEncapsulationContextOnSendRule"
```

Should return value of `2`.

#### Solution 2: Firewall blocking VPN traffic

**On Windows 11 side:**

1. Temporarily disable Windows Firewall to test:
   - Settings > Privacy & Security > Windows Security > Firewall & network protection
   - Turn off for "Public network"
2. Try connecting again
3. If it works, the firewall is the issue - add VPN ports to allowed list (see Step 2.6)

**On Draytek side:**

- Verify firewall rules from Step 1.5 are active and correct
- Check the rule order - VPN allow rules should be before deny rules

**On your office network:**

- Your office firewall/router may be blocking outbound VPN
- Ensure UDP ports 500, 4500, 1701 are allowed outbound

#### Solution 3: ISP blocking VPN ports

Some ISPs block VPN traffic. Test:

1. Try connecting from a different network (e.g., mobile hotspot)
2. If it works on mobile but not your office network, the office ISP/firewall is blocking VPN

**Workaround:** Use SSL VPN instead of L2TP (requires Draytek SSL VPN client software)

### Connection Fails - Error 789

**Error message:** "The L2TP connection attempt failed because the security layer encountered a processing error..."

**Causes and solutions:**

#### Solution 1: Pre-shared key mismatch

- Verify the PSK on Draytek matches exactly what you entered in Windows
- PSKs are case-sensitive
- Check for extra spaces or characters

**Fix:**

1. On Draytek: **VPN and Remote Access > Remote Access Control** - note the PSK exactly
2. On Windows: VPN properties > Security tab > Advanced settings - re-enter PSK
3. Try connecting again

#### Solution 2: Incorrect encryption or authentication settings

**On Draytek:**

- Ensure IPsec encryption is **Enabled**
- Encryption: AES-256 or AES-128
- Authentication: SHA-256 or SHA-1

**On Windows:**

- VPN properties > Security tab
- Data encryption: "Require encryption"
- Allow only: MS-CHAP v2

### Connection Fails - Error 691

**Error message:** "The remote connection was denied because the user name and password combination you provided is not recognized..."

**Causes and solutions:**

#### Solution 1: Incorrect username or password

- Verify credentials are correct (case-sensitive)
- Check for typos

#### Solution 2: User account not enabled or expired

**On Draytek:**

1. Go to **VPN and Remote Access > Remote Dial-In User**
2. Find your user account
3. Verify:
   - ✅ **Active** is checked
   - ✅ **VPN Type** includes "L2TP"
   - Account hasn't expired (if expiration was set)

#### Solution 3: Authentication protocol mismatch

**On Draytek:**

- Ensure **MS-CHAPv2 Authentication** is enabled

**On Windows:**

- VPN properties > Security tab
- Ensure **Microsoft CHAP Version 2 (MS-CHAP v2)** is checked

### Can Connect But Can't Access Remote Devices

**Symptom:** VPN shows "Connected" but you can't ping or access devices on the remote network.

#### Solution 1: Check VPN client IP assignment

```cmd
ipconfig /all
```

Look for "PPP adapter Remote Office VPN" - it should show an IP in your VPN pool range.

**If no IP or wrong IP:**
- Draytek's client IP pool may be exhausted
- Check Draytek: **VPN and Remote Access > Remote Access Control** - increase IP pool count

#### Solution 2: Routing issue

**Verify static route (if using split tunneling):**

```powershell
route print | findstr "192.168.1.0"
```

You should see an entry for your remote network.

**If missing, add it again:**

```powershell
route -p add 192.168.1.0 mask 255.255.255.0 0.0.0.0 metric 1
```

#### Solution 3: Remote device firewall

- The device you're trying to reach may have its own firewall blocking your VPN IP
- Try pinging the Draytek itself (`192.168.1.1`) - if that works, the issue is with the end device

#### Solution 4: Subnet conflict

**Issue:** Your office network uses the same subnet as the remote network.

**Example:** Both use `192.168.1.0/24`

**Symptoms:**
- VPN connects but routing is confused
- Can't access remote devices

**Solution:** Change the subnet on one of the networks:

**Option A - Change remote network (Draytek side):**

1. On Draytek: **LAN > General Setup**
2. Change IP address to a different subnet:
   - Example: `192.168.50.1` with subnet `255.255.255.0`
3. Update DHCP scope to match
4. This requires reconfiguring devices on the remote network

**Option B - Change office network** (if possible)

### VPN Connection Drops Frequently

#### Solution 1: Check internet stability

- Unstable internet on either side will drop the VPN
- Run a continuous ping test:

```cmd
# From office PC to remote router (while VPN connected)
ping -t 192.168.1.1
```

Monitor for packet loss or high latency spikes.

#### Solution 2: Enable VPN keepalive

**On Draytek:**

1. Go to **VPN and Remote Access > Remote Access Control**
2. Look for **LCP Echo** settings (may vary by firmware version)
3. Enable LCP keepalive to prevent idle disconnections

**On Windows:**

Configure idle timeout:

1. VPN properties > Options tab
2. Uncheck "Prompt for name and password, certificate, etc."
3. Set Idle time before hanging up: **Never**

#### Solution 3: Power management

Windows may disable the network adapter to save power:

1. Open **Device Manager** (right-click Start > Device Manager)
2. Expand **Network adapters**
3. Find your primary network adapter (Ethernet or WiFi)
4. Right-click > **Properties**
5. Go to **Power Management** tab
6. **Uncheck** "Allow the computer to turn off this device to save power"
7. Click **OK**

### Slow VPN Performance

#### Solution 1: Check encryption overhead

- VPN encryption adds overhead (10-20% speed reduction is normal)
- Run speed test without VPN, then with VPN to compare

#### Solution 2: Optimize MTU (Maximum Transmission Unit)

**Find optimal MTU:**

```cmd
# Test different MTU sizes (start with 1400)
ping -f -l 1400 [remote-ip]
```

If you get "Packet needs to be fragmented but DF set", lower the value.
Keep lowering until ping succeeds.

**Set MTU on VPN adapter:**

```powershell
# Find VPN adapter name
Get-NetAdapter

# Set MTU (replace "Remote Office VPN" with your actual adapter name)
Set-NetIPInterface -InterfaceAlias "Remote Office VPN" -NlMtuBytes 1400
```

#### Solution 3: QoS on Draytek

**On Draytek:**

1. Go to **Bandwidth Management > Quality of Service**
2. Create a QoS rule giving VPN traffic priority
3. This helps if the remote connection has limited bandwidth

### Can't Access Draytek Web Interface via VPN

**Symptom:** VPN connects, but you can't open the Draytek admin page at `https://192.168.1.1`

#### Solution: Enable remote management

**On Draytek:**

1. Go to **System Maintenance > Management**
2. Under **Access Control**:
   - Allow remote management from: **LAN and VPN**
3. If using HTTPS, ensure the certificate is valid or add exception in your browser
4. Try HTTP instead: `http://192.168.1.1`

---

## Security Hardening {#security-hardening}

After your VPN is working, implement these additional security measures.

### 1. Change Default Draytek Admin Password

If you haven't already:

1. Draytek web interface > **System Maintenance > Administrator Password**
2. Create a strong password (16+ characters, mixed case, numbers, symbols)
3. Store it securely in your password manager

### 2. Restrict VPN Access by Source IP (If Possible)

If your office has a static public IP:

**On Draytek:**

1. Go to **VPN and Remote Access > Remote Dial-In User**
2. Edit your user account
3. In **Allow Client IP** field, enter your office public IP
4. Click **OK**

This prevents VPN connections from any other IP address.

### 3. Enable Two-Factor Authentication (If Supported)

Some Draytek firmware versions support RADIUS authentication with 2FA:

1. Set up a RADIUS server (e.g., FreeRADIUS, Windows NPS) with 2FA
2. Configure Draytek to use RADIUS:
   - **VPN and Remote Access > Remote Access Control > RADIUS Settings**
3. User must provide both password and OTP code when connecting

### 4. Implement Connection Logging and Monitoring

**On Draytek:**

1. **System Maintenance > Syslog/Mail Alert**
2. Enable **Email Alert** for security events:
   - Configure SMTP settings
   - Add recipient email
   - Check **VPN** and **Security** event categories
3. You'll receive emails when VPN connections occur or fail

**Review logs regularly:**
- **System Maintenance > View Logs**
- Look for:
  - Failed authentication attempts (potential brute force)
  - Connections from unexpected IPs
  - Connection errors

### 5. Limit Simultaneous VPN Connections

**On Draytek:**

1. **VPN and Remote Access > Remote Access Control**
2. Set **IP Pool Count** to the minimum you need
3. This limits how many devices can connect simultaneously

### 6. Regularly Update Firmware

- Check for Draytek firmware updates monthly
- Subscribe to Draytek security advisories
- Apply updates promptly (especially security patches)

### 7. Use Certificate-Based Authentication (Advanced)

For maximum security, upgrade to certificate-based IPsec:

- This removes the shared PSK vulnerability
- Each client gets a unique certificate
- More complex to set up but significantly more secure
- Refer to Draytek's certificate management documentation

### 8. Disable Unused Remote Access Protocols

**On Draytek:**

1. **VPN and Remote Access > Remote Access Control**
2. Ensure only **L2TP Server** is enabled
3. Disable:
   - ❌ **PPTP Server** (if present)
   - ❌ **PPPoE Server** (unless specifically needed)

### 9. Configure Connection Timeout

**On Draytek:**

1. **VPN and Remote Access > Remote Access Control**
2. Set **Idle Timeout**: 30 minutes (or appropriate for your use case)
3. Connections will automatically disconnect after idle period

### 10. Implement Network Segmentation

**For advanced security:**

- Create a separate VLAN for VPN clients
- Implement firewall rules controlling what VPN users can access
- Example: Allow access to servers but not other workstations

**On Draytek:**

1. **LAN > VLAN** - Create VPN client VLAN
2. **Firewall > Filter Setup** - Create inter-VLAN rules
3. **VPN and Remote Access > Remote Access Control** - Assign VPN clients to VLAN

---

## Maintenance and Monitoring {#maintenance}

### Regular Maintenance Tasks

**Weekly:**

- [ ] Review VPN connection logs for anomalies
- [ ] Verify VPN connectivity with a test connection

**Monthly:**

- [ ] Check for Draytek firmware updates
- [ ] Review and rotate VPN passwords (if following strict security policy)
- [ ] Verify backup of Draytek configuration

**Quarterly:**

- [ ] Audit VPN user accounts (disable unused accounts)
- [ ] Review firewall rules for VPN access
- [ ] Test VPN connection from different locations/networks
- [ ] Review and update this documentation

**Annually:**

- [ ] Change the Pre-Shared Key (PSK)
- [ ] Review and update VPN security policies
- [ ] Consider upgrading to certificate-based authentication

### Backup Draytek Configuration

**Create a configuration backup:**

1. Log into Draytek web interface
2. Go to **System Maintenance > Config Backup/Restore**
3. Click **Backup Configuration to File**
4. Save the file securely (encrypted storage)
5. Store off-site or in cloud backup

**When to backup:**
- Before any configuration changes
- After successful VPN setup
- Before firmware updates
- Monthly (automated if possible)

**Restore configuration:**

1. **System Maintenance > Config Backup/Restore**
2. Click **Choose File** and select your backup
3. Click **Restore**
4. Router will reboot with restored settings

### Monitoring VPN Health

**Key metrics to monitor:**

**Connection Logs:**
- Number of connection attempts vs. successful connections
- Failed authentication attempts (watch for brute force)
- Connection duration

**Performance:**
- VPN latency (ping times)
- Throughput (speed tests)
- Packet loss

**View real-time VPN status on Draytek:**

1. **Online Status > VPN Connection**
2. Shows:
   - Currently connected users
   - Connection duration
   - Data transferred
   - Assigned IP addresses

### Setting Up Email Alerts

Get notified of VPN issues automatically:

**On Draytek:**

1. **System Maintenance > Syslog/Mail Alert**
2. Configure SMTP settings:
   - SMTP server: `smtp.gmail.com` (or your email provider)
   - Port: `587` (TLS) or `465` (SSL)
   - Authentication: Enter email username and password
   - Sender email: Your email address
3. Add recipient email addresses
4. Under **Event Categories**, select:
   - ✅ **VPN** - Connection events
   - ✅ **Security** - Failed auth, intrusion attempts
   - ✅ **System** - Router reboots, errors
5. Click **Apply**

**Test the alert:**

1. Click **Send Test Mail**
2. Check your inbox for the test message

### Documentation Updates

Keep this document updated with:

- Configuration changes (IP addresses, subnets, passwords)
- Firmware version updates
- Issues encountered and solutions
- Additional user accounts created
- Network topology changes

**Version control:**

Consider using Git to track changes to this document:

```bash
# Initialize git repository in documentation folder
cd /path/to/documentation
git init
git add Draytek_Connect.md
git commit -m "Initial VPN configuration documentation"

# After making changes:
git add Draytek_Connect.md
git commit -m "Updated VPN pool range and added new user"
```

---

## Additional Resources

### Official Documentation

- **Draytek Vigor 2865 User Manual:** https://www.draytek.com/products/vigor2865/
- **Draytek Support Portal:** https://www.draytek.com/support/
- **Draytek Firmware Downloads:** https://www.draytek.com/support/downloads/

### Microsoft Documentation

- **Windows VPN Settings:** https://support.microsoft.com/en-us/windows/connect-to-a-vpn-in-windows-3d29aeb1-f497-f6b7-7633-115722c1009c
- **Advanced VPN Configuration:** https://learn.microsoft.com/en-us/windows-server/remote/remote-access/vpn/vpn-top

### Security Resources

- **VPN Security Best Practices (NIST):** https://csrc.nist.gov/publications
- **IPsec Security (SANS):** https://www.sans.org/reading-room/whitepapers/vpns/

### Troubleshooting Resources

- **Draytek Community Forum:** https://www.draytek.com/support/resources/community-forum/
- **Windows VPN Troubleshooting:** https://learn.microsoft.com/en-us/troubleshoot/windows-client/networking/troubleshoot-vpn-issues

---

## Appendix: Quick Reference

### Draytek VPN Server Settings Summary

| Setting | Location | Value |
|---------|----------|-------|
| L2TP Server | VPN > Remote Access Control | Enabled |
| IPsec Encryption | VPN > Remote Access Control | Enabled |
| Pre-Shared Key | VPN > Remote Access Control | [Your PSK] |
| Encryption Algorithm | VPN > Remote Access Control | AES-256 |
| Authentication | VPN > Remote Access Control | SHA-256 |
| Client IP Pool | VPN > Remote Access Control | [Start IP + Count] |
| Firewall Rules | Firewall > Filter Setup | UDP 500, 4500, 1701 allowed |

### Windows 11 VPN Settings Summary

| Setting | Location | Value |
|---------|----------|-------|
| VPN Type | VPN Properties > Security | L2TP/IPsec with PSK |
| Data Encryption | VPN Properties > Security | Require encryption |
| Authentication | VPN Properties > Security | MS-CHAP v2 only |
| Pre-Shared Key | VPN Properties > Security > Advanced | [Your PSK] |
| Default Gateway | VPN Properties > Networking > IPv4 > Advanced | Unchecked (for split tunnel) |
| Registry Setting | PolicyAgent | AssumeUDPEncapsulationContextOnSendRule = 2 |

### Port Reference

| Protocol | Port | Purpose |
|----------|------|---------|
| IPsec IKE | UDP 500 | Internet Key Exchange |
| IPsec NAT-T | UDP 4500 | NAT Traversal |
| L2TP | UDP 1701 | L2TP tunnel |

### Common IP Ranges

| Purpose | Default | Example |
|---------|---------|---------|
| Draytek LAN IP | 192.168.1.1 | Router management address |
| Remote Network | 192.168.1.0/24 | Full remote subnet |
| VPN Client Pool | 192.168.1.200-209 | IPs assigned to VPN clients |

### Useful Commands

**Windows PowerShell (as Administrator):**

```powershell
# Add registry key for L2TP behind NAT
Set-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Services\PolicyAgent" -Name "AssumeUDPEncapsulationContextOnSendRule" -Value 2 -Type DWord

# Add persistent route for remote network (example: 192.168.1.0/24)
route -p add 192.168.1.0 mask 255.255.255.0 0.0.0.0 metric 1

# View all routes
route print

# Test VPN connection
ping 192.168.1.1

# Check VPN adapter status
Get-NetAdapter | Where-Object {$_.InterfaceDescription -like "*WAN*"}

# View VPN IP configuration
ipconfig /all | Select-String -Pattern "PPP adapter"
```

**Command Prompt:**

```cmd
# Ping remote router
ping 192.168.1.1

# Trace route to remote device
tracert 192.168.1.50

# View current routes
route print

# Release and renew IP (if VPN adapter has issues)
ipconfig /release "Remote Office VPN"
ipconfig /renew "Remote Office VPN"
```

### Emergency Contact Information

**Document your support contacts:**

| Role | Name | Contact |
|------|------|---------|
| Draytek Remote Site Contact | _____________ | _____________ |
| Network Administrator | _____________ | _____________ |
| ISP Support (Remote) | _____________ | _____________ |
| ISP Support (Office) | _____________ | _____________ |
| IT Support | _____________ | _____________ |

---

## Document Information

**Document Title:** Draytek Vigor 2865 VPN Connection Guide
**Version:** 1.0
**Created:** October 2025
**Last Updated:** October 2025
**Author:** IT Documentation Team
**Review Cycle:** Quarterly

### Change Log

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | October 2025 | Initial document creation | IT Team |

---

**End of Document**

For questions or issues not covered in this guide, contact your network administrator or Draytek technical support.
