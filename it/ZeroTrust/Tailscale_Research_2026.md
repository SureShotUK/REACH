# Tailscale Zero Trust Network Access Research (2026)

## Executive Summary

**Research Date**: February 17, 2026
**Environment**: PostgreSQL 16.11 on Ubuntu 24.04 LTS, 30 Azure AD users, UK-based
**Budget**: £10/user/month (£300/month total)

### Quick Assessment

| Criterion | Rating | Notes |
|-----------|--------|-------|
| **Budget Fit** | ⚠️ **Marginal** | Starter: £180/month, Premium: £540/month (exceeds budget), Enterprise: Unknown |
| **Technical Fit** | ✅ **Strong** | WireGuard mesh, ACL-based access control, subnet routing support |
| **Azure AD Integration** | ✅ **Excellent** | Native SSO, automatic user provisioning (Enterprise), MFA support |
| **Zero Trust Alignment** | ✅ **Strong** | Identity-based access, continuous verification, micro-segmentation |
| **GDPR Compliance** | ✅ **Compliant** | DPA available, EU representative, SCCs for US transfers |
| **Deployment Complexity** | ✅ **Low** | Intune deployment supported, MDM policies available |

**Recommendation**: Tailscale is technically excellent but **Premium plan exceeds budget** (£540/month vs £300/month). Consider Starter plan (£180/month) with limitations, negotiate Enterprise pricing, or explore alternatives.

---

## 1. Architecture & How Tailscale Works

### 1.1 Core Architecture

Tailscale implements a **peer-to-peer mesh network** using the WireGuard protocol with a centralized coordination server for control plane operations.

**Key Components**:

1. **Control Plane** (Centralized Coordination Server):
   - Handles user authentication via OAuth
   - Manages device authorization and registration
   - Distributes network maps to clients
   - Stores ACL policies
   - Facilitates peer discovery and connection information
   - Data stored: Public keys, metadata, ACLs, node IDs
   - Storage: SQLite + AWS S3 backups, analytics in Snowflake

2. **Data Plane** (Fully Distributed):
   - Each device pair establishes its own WireGuard tunnel
   - End-to-end encrypted (keys never leave devices)
   - Point-to-point connections when possible
   - No traffic passes through coordination server

**Sources**:
- <a href="https://tailscale.com/blog/how-tailscale-works" target="_blank">Tailscale: How it works</a>
- <a href="https://deepwiki.com/tailscale/tailscale/3-client-components" target="_blank">Control Plane Communication - DeepWiki</a>

### 1.2 Connection Methods (Priority Order)

1. **Direct Connection** (Preferred):
   - NAT traversal succeeds (90%+ success rate)
   - Lowest latency, highest throughput
   - Direct peer-to-peer WireGuard tunnel

2. **Peer Relay Connection**:
   - When direct fails but relay is reachable
   - Uses another Tailscale node as relay
   - Better performance than DERP

3. **DERP Relay** (Fallback):
   - Always-available baseline
   - Shared infrastructure for all Tailscale users
   - Optimized for availability, not raw throughput
   - Example latencies:
     - DERP relay: 452ms (initial)
     - Peer relay: 298-306ms
     - Direct: Best performance

**Sources**:
- <a href="https://tailscale.com/blog/peer-relays-beta" target="_blank">Tailscale Peer Relays: High-throughput relays</a>
- <a href="https://tailscale.com/blog/nat-traversal-improvements-pt-1" target="_blank">How Tailscale is improving NAT traversal</a>

### 1.3 UK/Europe Geographic Presence

**DERP Relay Locations** (measured from UK):
- London: 85.2ms
- Madrid: 64.7ms
- Warsaw: 66.1ms
- New York: 92.5ms (for occasional North America access)

Tailscale continues to expand DERP locations globally to reduce latency.

**Sources**:
- <a href="https://tailscale.com/blog/more-derp" target="_blank">Tailscale: More DERP</a>
- <a href="https://tailscale.com/blog/peer-relays-international-networks" target="_blank">Using Tailscale's Peer Relays internationally</a>

---

## 2. PostgreSQL Database Access

### 2.1 Direct TCP Access Model

Once a device joins the Tailscale mesh network:
- **Direct database connections** to PostgreSQL port 5432 via Tailscale IP
- No additional proxy required (devices on same virtual network)
- Configure `pg_hba.conf` to allow Tailscale IP range
- Use standard PostgreSQL clients (psql, pgAdmin, applications)

**Example Configuration**:
```conf
# pg_hba.conf - Allow Tailscale network
host    all    all    100.64.0.0/10    scram-sha-256
```

### 2.2 Enhanced Security with pgproxy

Tailscale provides **pgproxy** for identity-aware database access:

**Features**:
- Only accepts connections from Tailscale clients (uses tsnet library)
- Provides mTLS encryption
- Audit logging of all connections with Tailscale identity (machine + user)
- Works with cloud-hosted databases
- Minimal container footprint (hardened)

**How it Works**:
1. Client → pgproxy (secured by Tailscale mTLS)
2. pgproxy → PostgreSQL (TLS with full verification)
3. All connections logged with user identity

**Benefits**:
- Reduces attack surface
- No public database exposure
- Identity-aware audit trails
- Client configuration irrelevant (security handled by Tailscale)

**Sources**:
- <a href="https://tailscale.com/blog/introducing-pgproxy" target="_blank">Protect Your PostgreSQL Database with Tailscale's pgproxy</a>
- <a href="https://www.crunchydata.com/blog/crunchy-bridge-with-tailscale" target="_blank">Postgres + Tailscale - Crunchy Data</a>
- <a href="https://petar.dev/notes/connect-to-postgresql-from-taiscale/" target="_blank">Connect to PostgreSQL from Tailscale</a>

### 2.3 Performance Considerations

**Connection Pooling**:
- Tailscale operates at network layer - transparent to application
- Use PgBouncer or application-level pooling as normal
- No additional overhead from Tailscale networking
- Sample implementations combine Tailscale + PgBouncer successfully

**Performance vs Traditional VPN**:
- **WireGuard Protocol**: Lower overhead than OpenVPN/IPSec
- **Direct Connections**: 90%+ success rate for peer-to-peer
- **User-space Implementation**: Slight overhead vs kernel-space WireGuard
- **Benchmarks**:
  - Kernel WireGuard: Up to 8Gb/s
  - Tailscale: 10Gb/s+ on Linux (optimized with UDP segmentation)
  - DERP relay: ~35.6 Mbits/sec (stress test worst case)

**For Database Access**: Direct connections (90%+ scenarios) provide excellent performance. Even DERP-relayed connections sufficient for typical database workloads (queries, not video streaming).

**Sources**:
- <a href="https://tailscale.com/compare/wireguard" target="_blank">WireGuard vs. Tailscale</a>
- <a href="https://contabo.com/blog/wireguard-vs-tailscale/" target="_blank">WireGuard vs Tailscale: Performance, Configuration, Costs</a>
- <a href="https://www.crunchydata.com/blog/crunchy-bridge-with-tailscale" target="_blank">Postgres + Tailscale performance</a>

---

## 3. Azure AD Integration & Authentication

### 3.1 Single Sign-On (SSO)

**Native Azure AD Support**:
- Default integration with Microsoft Entra ID (Azure AD)
- Users sign in with organizational Azure AD accounts
- OAuth-based authentication
- No username/password stored in Tailscale

**Sources**:
- <a href="https://azuremarketplace.microsoft.com/en/marketplace/apps/aad.tailscale" target="_blank">Tailscale - Microsoft Azure Marketplace</a>
- <a href="https://tailscale.com/blog/custom-oidc-enterprise" target="_blank">Log into Tailscale with OIDC providers</a>

### 3.2 User & Group Provisioning

**Automatic Provisioning** (Enterprise Plan):
- SCIM-based user/group sync from Azure AD
- Automatic user creation when added to Azure AD
- Automatic deprovisioning on offboarding
- Group membership sync for ACL rules
- Bidirectional synchronization

**Benefits**:
- No manual user management in Tailscale
- Immediate access revocation on Azure AD deletion
- Group-based access control aligned with Azure AD

**Configuration**:
- Microsoft Learn tutorial available
- Configure via Entra ID admin center
- SCIM connector setup

**Sources**:
- <a href="https://tailscale.com/blog/sync-azuread-groups" target="_blank">Sync Azure AD Users & Groups to Tailscale</a>
- <a href="https://learn.microsoft.com/en-us/entra/identity/saas-apps/tailscale-provisioning-tutorial" target="_blank">Configure Tailscale for automatic provisioning - Microsoft Learn</a>
- <a href="https://tailscale.com/blog/entra-id" target="_blank">Microsoft Entra ID Access Provisioning for Tailscale</a>

### 3.3 Multi-Factor Authentication (MFA)

**MFA Enforcement**:
- MFA handled entirely by Azure AD
- Tailscale respects all Azure AD authentication policies
- When Azure AD requires MFA, Tailscale automatically uses it
- Supports Azure AD Conditional Access policies

**How it Works**:
1. User attempts to connect Tailscale client
2. Redirected to Azure AD for authentication
3. Azure AD enforces MFA (if configured)
4. Upon successful MFA, Tailscale authorizes connection

**Best Practices**:
- Enable MFA in Azure AD tenant
- Use Conditional Access for context-aware policies
- Require MFA for all users or based on location/device

**Sources**:
- <a href="https://tailscale.com/kb/1005/multifactor-anything" target="_blank">Add multifactor authentication to any service - Tailscale</a>
- <a href="https://tailscale.com/learn/rolling-out-tailscale-for-your-team" target="_blank">Rolling Out Tailscale for Your Team</a>

---

## 4. Pricing Analysis (2026)

### 4.1 Plan Tiers

| Plan | Price per User/Month | Included Free Users | Your Cost (30 Users + 1 Server) |
|------|---------------------|---------------------|----------------------------------|
| **Personal** (Free) | £0 | 3 users max | ❌ Not suitable (30 users needed) |
| **Starter** | $6 (~£4.80) | 3 free users | **~£180/month** (27 paid users) |
| **Premium** | $18 (~£14.40) | 3 free users | **~£540/month** (27 paid users) |
| **Enterprise** | Contact Sales | Unknown | **Unknown - Requires quote** |

**Note**: Exchange rate approximation: $1.25 = £1 (verify current rates)

**Usage-Based Billing**:
- Only charged for users who **actively exchange data** over Tailscale
- Not charged for inactive users
- 3 free users included in paid plans
- Devices/servers typically don't count as "users" (verify with sales)

**Sources**:
- <a href="https://tailscale.com/pricing" target="_blank">Tailscale Pricing - Official</a>
- <a href="https://tailscale.com/blog/pricing-v3" target="_blank">Pricing v3, plans, packages</a>
- <a href="https://tailscale.com/blog/2021-06-new-pricing" target="_blank">New Pricing Model Makes Scaling Less Expensive</a>

### 4.2 Feature Comparison Relevant to Your Requirements

| Feature | Starter | Premium | Enterprise | Your Need |
|---------|---------|---------|------------|-----------|
| **Azure AD SSO** | ✅ | ✅ | ✅ | Required |
| **User/Group Provisioning** | ❌ | ❌ | ✅ | Desired |
| **ACLs** | ✅ Basic | ✅ Advanced | ✅ Full | Required |
| **Subnet Routing** | ✅ | ✅ | ✅ | Required (CCTV) |
| **MFA Support** | ✅ | ✅ | ✅ | Required |
| **Audit Logs** | ❌ | ❌ | ✅ Configuration | Desired |
| **Network Flow Logs** | ❌ | ✅ | ✅ | Desired |
| **Session Recording** | ❌ | ❌ | ✅ SSH only | Not critical |
| **SOC 2 Type II** | ❌ | ✅ | ✅ | Desired (compliance) |
| **SLA** | ❌ | ❌ | ✅ | Desired |

**Budget Analysis**:
- **Starter Plan**: Fits budget (£180 < £300) but lacks user provisioning, audit logs
- **Premium Plan**: Exceeds budget significantly (£540 > £300)
- **Enterprise Plan**: Unknown cost, includes all features but likely exceeds budget

**Recommendation**:
1. Try Starter plan (fits budget, test functionality)
2. Request Enterprise quote (negotiate for 30 users)
3. Consider if manual user management acceptable (no auto-provisioning on Starter)

**Sources**:
- <a href="https://tailscale.com/pricing" target="_blank">Tailscale Pricing</a>
- <a href="https://www.saasworthy.com/product/tailscale/pricing" target="_blank">Tailscale Pricing - SaaSworthy</a>

### 4.3 Free Tier Limitations

**Personal Plan** (Free):
- **3 users maximum**
- **100 devices maximum**
- All core features included (ACLs, subnet routing, etc.)
- No audit logging
- Community support only

**Not Suitable** for 30-user deployment, but useful for:
- Proof of concept testing
- Lab environment validation
- Training administrators before purchase

**Sources**:
- <a href="https://tailscale.com/blog/free-plan" target="_blank">How Tailscale's free plan stays free</a>
- <a href="https://tailscale.com/pricing" target="_blank">Tailscale Pricing</a>

---

## 5. Access Control & Zero Trust Features

### 5.1 Zero Trust Architecture Alignment

**Traditional VPN vs Tailscale ZTNA**:

| Aspect | Traditional VPN | Tailscale ZTNA |
|--------|-----------------|----------------|
| **Access Model** | Network-level (all resources once connected) | Application-level (identity-based) |
| **Network Topology** | Centralized hub-and-spoke | Distributed peer-to-peer mesh |
| **Authentication** | One-time at connection | Continuous verification |
| **Trust Model** | Network location-based | Identity + device posture |
| **Data Plane** | Traffic through central server | Direct peer-to-peer tunnels |
| **Segmentation** | Subnet-based | Micro-segmentation per resource |

**Zero Trust Principles Implemented**:
1. ✅ **Verify explicitly**: Azure AD authentication + continuous device verification
2. ✅ **Least privilege access**: ACL-based per-resource authorization
3. ✅ **Assume breach**: End-to-end encryption, no implicit trust
4. ✅ **Identity-centric**: All access tied to user/device identity
5. ✅ **Micro-segmentation**: Granular rules per device/port/protocol

**Verdict**: Tailscale is **genuine ZTNA**, not just a "modern VPN".

**Sources**:
- <a href="https://tailscale.com/use-cases/zero-trust-networking" target="_blank">Zero Trust Networking with Tailscale</a>
- <a href="https://oneuptime.com/blog/post/2026-01-27-tailscale-zero-trust-networking/view" target="_blank">How to Use Tailscale for Zero-Trust Networking</a>
- <a href="https://tailscale.com/compare/zscaler" target="_blank">Zscaler vs. Tailscale</a>

### 5.2 Access Control Lists (ACLs)

**Capabilities**:
- **Deny-by-default**: No access unless explicitly granted
- **Group-based policies**: Define groups in Azure AD, reference in ACLs
- **Tag-based policies**: Assign tags to devices (e.g., `tag:db-server`)
- **Port/protocol filtering**: Restrict to specific ports (e.g., TCP:5432)
- **User-based rules**: Grant access to specific users or groups
- **Advanced grants**: Application-layer capabilities (SSH file editing, etc.)

**Example ACL for PostgreSQL Access**:
```json
{
  "groups": {
    "group:db-admins": ["user1@company.com", "user2@company.com"],
    "group:db-users": ["user3@company.com", "user4@company.com"]
  },
  "tagOwners": {
    "tag:db-server": ["group:db-admins"]
  },
  "acls": [
    // DB admins: full PostgreSQL access
    {
      "action": "accept",
      "src": ["group:db-admins"],
      "dst": ["tag:db-server:5432"],
      "proto": "tcp"
    },
    // DB users: read-only port access (enforce at DB level)
    {
      "action": "accept",
      "src": ["group:db-users"],
      "dst": ["tag:db-server:5432"],
      "proto": "tcp"
    }
  ]
}
```

**Best Practices**:
- Avoid `autogroup:self:*` (allows all ports - too permissive)
- Use specific ports (e.g., `:5432` not `:*`)
- Combine ACLs with PostgreSQL RBAC (defense in depth)
- Sync groups from Azure AD for consistency

**Sources**:
- <a href="https://tailscale.com/kb/1337/acl-syntax" target="_blank">ACL syntax reference - Tailscale Docs</a>
- <a href="https://tailscale.com/blog/acl-grants" target="_blank">Manage network access with Tailscale Grants</a>
- <a href="https://medium.com/@blabber_ducky/managing-tailscale-network-access-with-acls-e2989b550e27" target="_blank">Managing Tailscale Network Access with ACLs</a>
- <a href="https://vulnerx.com/mastering-tailscale-acl/" target="_blank">Master Tailscale ACLs: Enterprise Security Guide</a>

### 5.3 Least Privilege Implementation

**Approach**:
1. Define user groups aligned with job roles
2. Tag resources by function (e.g., `tag:db-server`, `tag:cctv-system`)
3. Create ACL rules granting minimal required access
4. Evaluate with most restrictive rules first

**For Your Scenario**:
- **Database access**: Only users needing database work
- **CCTV access**: Separate group (likely subset of 30 users)
- **Port restrictions**: PostgreSQL (5432), CCTV (web/RTSP ports only)

**Sources**:
- <a href="https://www.conductorone.com/blog/implementing-least-privilege-access-tailscale-conductorone/" target="_blank">Implementing Least Privilege Access: Tailscale + ConductorOne</a>
- <a href="https://tailscale.com/security-policies/access-control" target="_blank">Access control policy - Tailscale</a>

---

## 6. Subnet Routing for CCTV Access

### 6.1 How Subnet Routing Works

**Concept**:
- Install Tailscale on a device within your network (subnet router)
- Advertise local subnets (e.g., CCTV VLAN 192.168.10.0/24)
- Remote Tailscale clients access subnet resources without installing Tailscale on them

**Use Case for CCTV**:
- CCTV cameras on separate VLAN (e.g., 192.168.10.0/24)
- Cameras cannot run Tailscale client (embedded devices)
- Deploy subnet router on same network/VLAN
- Users access CCTV web interface via Tailscale

**Sources**:
- <a href="https://tailscale.com/blog/subnet-router-video" target="_blank">Subnet routers: how do they work?</a>
- <a href="https://docs.railway.com/guides/set-up-a-tailscale-subnet-router" target="_blank">Set up a Tailscale Subnet Router - Railway</a>

### 6.2 Configuration Steps

**On Subnet Router Device** (Linux recommended):
```bash
# Install Tailscale
curl -fsSL https://tailscale.com/install.sh | sh

# Enable IP forwarding
echo 'net.ipv4.ip_forward = 1' | sudo tee -a /etc/sysctl.conf
echo 'net.ipv6.conf.all.forwarding = 1' | sudo tee -a /etc/sysctl.conf
sudo sysctl -p

# Advertise routes
sudo tailscale up --advertise-routes=192.168.10.0/24
```

**In Tailscale Admin Console**:
1. Navigate to Machines
2. Find subnet router device
3. Click "Edit route settings"
4. Enable advertised routes

**ACL for CCTV Access**:
```json
{
  "groups": {
    "group:cctv-viewers": ["user5@company.com", "user6@company.com"]
  },
  "acls": [
    {
      "action": "accept",
      "src": ["group:cctv-viewers"],
      "dst": ["192.168.10.0/24:80", "192.168.10.0/24:443", "192.168.10.0/24:554"],
      "proto": "tcp"
    }
  ]
}
```
**Ports**: 80/443 (web), 554 (RTSP for video streams)

**Sources**:
- <a href="https://docs.gl-inet.com/router/en/4/interface_guide/tailscale/" target="_blank">Tailscale - GL.iNet Router Docs</a>
- <a href="https://community.home-assistant.io/t/tailscale-subnets-how-to-access-a-different-subnet/574372" target="_blank">Tailscale + Subnets - Home Assistant Community</a>

### 6.3 Advanced Considerations

**Challenges**:
- Requires IP forwarding enabled on subnet router
- May need NAT configuration on some Linux systems
- Ensure subnet router has access to CCTV VLAN (routing/firewall rules)
- For Docker-based subnet routers: Use `userspace_networking` mode

**Best Practices**:
- Use dedicated Linux device/VM as subnet router (not end-user laptop)
- Tag subnet router device (e.g., `tag:subnet-router`)
- Monitor subnet router availability (single point of failure for CCTV access)
- Consider redundant subnet routers if CCTV access is critical

**Sources**:
- <a href="https://github.com/tailscale/tailscale/issues/8370" target="_blank">Site-to-site networking using subnet router nodes</a>
- <a href="https://github.com/tailscale/tailscale/issues/14971" target="_blank">Subnet Router NAT Configuration</a>

---

## 7. Windows 11 Client Deployment via Intune

### 7.1 Installation Options

**Available Installers**:
- `.exe` installer (standard)
- `.msi` installer (enterprise deployment)

**Deployment Methods**:
- Microsoft Intune (MDM)
- Group Policy (GPO)
- Manual installation

**Sources**:
- <a href="https://github.com/tailscale/tailscale/issues/595" target="_blank">Windows: add MSI installer - GitHub Issue</a>
- <a href="https://yearofinvention.com/blog/how-does-tailscale-vpn-work-on-windows/" target="_blank">How Tailscale VPN Works with Windows</a>

### 7.2 Intune Deployment Process

**Step 1: Package Tailscale MSI**:
1. Download Tailscale MSI from official website
2. In Intune admin center → Apps → Windows → Add
3. Select "Line-of-business app"
4. Upload Tailscale MSI

**Step 2: Configure Deployment Settings**:
- **Assignment**: Assign to Azure AD user/device groups
- **Install context**: System context (all users on device)
- **Installation command**: `msiexec /i Tailscale.msi /quiet /norestart`

**Step 3: Policy Configuration** (Optional):
- Configure MDM policies for Tailscale behavior
- Set login server, exit nodes, key expiry

**Sources**:
- <a href="https://tailscale.com/blog/mdm-ga" target="_blank">Customize Tailscale Using MDM Policies</a>
- <a href="https://learn.microsoft.com/en-us/entra/identity/saas-apps/tailscale-provisioning-tutorial" target="_blank">Configure Tailscale with Microsoft Entra ID</a>

### 7.3 MDM Policies for Tailscale

**Available Policies**:
- `LoginURL`: Set custom coordination server (if using Headscale)
- `AlwaysOn`: Force Tailscale to stay connected (replaces `ForceEnabled`)
- `ReconnectAfter`: Auto-reconnect after user disconnects
- `ExitNodeID`: Force specific exit node
- `LogTarget`: Configure logging destination

**Example Registry Policy** (via Intune):
```
HKLM\Software\Tailscale\IPN
- AlwaysOn: DWORD = 1 (enabled)
- ReconnectAfter: DWORD = 300 (seconds)
```

**Always-On Behavior**:
- Tailscale connects at user sign-in
- Stays connected regardless of GUI state
- Enables early access to network resources (e.g., mapped drives)
- Works on headless Windows systems

**Sources**:
- <a href="https://tailscale.com/blog/mdm-ga" target="_blank">MDM Policies for Secure Access</a>
- <a href="https://github.com/tailscale/tailscale/issues/7055" target="_blank">Automatically set unattended on Windows</a>
- <a href="https://github.com/tailscale/tailscale/issues/12483" target="_blank">Windows client auto-connection options</a>

### 7.4 User Experience Considerations

**Default Behavior**:
- Manual connection required (tray icon)
- Disconnects when user logs off

**With Always-On Enabled**:
- Automatic connection at login
- Seamless experience (users may not notice VPN active)
- Potential user confusion ("Am I connected?")

**Recommendations**:
- Enable Always-On for database users (seamless access)
- Provide user training (tray icon status)
- Configure system tray icon visibility
- Consider conditional Always-On (based on user group)

**Sources**:
- <a href="https://github.com/tailscale/tailscale/issues/7212" target="_blank">Windows client connects automatically - Issue</a>

---

## 8. Security Audits & Compliance

### 8.1 Security Certifications

**SOC 2 Type II**:
- ✅ Completed certification
- Meets AICPA Trust Services Criteria:
  - Security
  - Availability
  - Confidentiality
- Annual audits required to maintain

**Security Audits**:
- Conducted by **Latacora** (specialized security firm)
- Services include:
  - Traditional security assessments
  - Continuous monitoring
  - Maturity model reviews
  - Design review and advisory

**Sources**:
- <a href="https://tailscale.com/security" target="_blank">Security | Tailscale</a>

### 8.2 Recent Vulnerabilities (2025-2026)

| CVE/Bulletin | Severity | Impact | Status | Date |
|--------------|----------|--------|--------|------|
| **TS-2025-008**: Tailnet Lock Signing | Medium | Signing checks in Tailnet Lock | ✅ Patched | 2025-11 |
| **Subnet Router ACL Issue** | Medium | Protocol filters not enforced on shared subnet routers | ✅ Fixed (coordination server) | 2025-11-07 |
| **Grafana Proxy Header Vuln** | Medium | Privilege escalation via HTTP header forgery | ✅ Patched | 2025-05-15 |

**Vulnerability Disclosure**:
- Tailscale publishes security bulletins transparently
- Timely patches deployed to coordination server
- Client updates distributed automatically (or via MDM)

**Sources**:
- <a href="https://tailscale.com/security-bulletins" target="_blank">Security Bulletins - Tailscale</a>
- <a href="https://app.opencve.io/cve/?vendor=tailscale" target="_blank">Tailscale CVEs - OpenCVE</a>
- <a href="https://www.cvedetails.com/vendor/28799/Tailscale.html" target="_blank">Tailscale Vulnerabilities - CVE Details</a>

### 8.3 GDPR Compliance

**GDPR Representation**:
- ✅ European Data Protection Office (EDPO) appointed as GDPR Representative (Article 27)
- EU-based contact for data protection inquiries

**Data Processing Addendum (DPA)**:
- ✅ Available to all customers
- Published subprocessor list
- Standard Contractual Clauses (SCCs) for EU→US transfers

**Data Residency**:
- Coordination data stored in: **Canada, Germany, United States, United Kingdom**
- May be processed in other jurisdictions
- EU/EEA/UK → US transfers via SCCs

**Data Minimization**:
- Tailscale stores **metadata only** (not customer data/traffic)
- No traffic inspection
- End-to-end encryption (keys never leave devices)
- Minimal PII stored (coordination purposes only)

**Data Collected**:
- Device hardware information
- Hostnames
- IP addresses
- Public keys
- Network maps

**Sources**:
- <a href="https://tailscale.com/privacy-policy" target="_blank">Privacy Policy - Tailscale</a>
- <a href="https://tailscale.com/dpa" target="_blank">Data Processing Addendum - Tailscale</a>
- <a href="https://tailscale.com/security" target="_blank">Security - Tailscale</a>

### 8.4 Audit Logging Capabilities

| Log Type | Availability | Content | Use Case |
|----------|-------------|---------|----------|
| **Configuration Audit Logs** | ✅ Enterprise | Admin actions, ACL changes, settings updates | Compliance, change tracking |
| **Network Flow Logs** | ✅ Premium/Enterprise | Node-to-node connections, timestamps | Network monitoring, troubleshooting |
| **pgproxy Connection Logs** | ✅ All plans (if using pgproxy) | Database connections with Tailscale user/machine identity | Database access auditing |
| **SSH Session Recording** | ✅ Enterprise (SSH only) | Recorded SSH sessions | Privileged access monitoring |

**Integration Options**:
- Export logs to SIEM (Datadog, Panther, RunReveal)
- API access for log retrieval
- Real-time log streaming

**For PostgreSQL Access Auditing**:
1. **Option 1**: Use pgproxy for Tailscale-level audit logs
2. **Option 2**: Enable PostgreSQL native audit logging (`pgaudit` extension)
3. **Option 3**: Combine both for defense-in-depth

**Sources**:
- <a href="https://tailscale.com/blog/config-audit-logging-ga" target="_blank">Configuration Audit Logs GA</a>
- <a href="https://tailscale.com/blog/network-flow-logs-is-generally-available" target="_blank">Network Flow Logs GA</a>
- <a href="https://docs.runreveal.com/sources/source-types/tailscale/audit" target="_blank">Tailscale Audit Logs - RunReveal</a>
- <a href="https://tailscale.com/blog/introducing-pgproxy" target="_blank">pgproxy audit logging</a>

---

## 9. Limitations & Considerations

### 9.1 Known Limitations

**Performance**:
- User-space WireGuard implementation: Slight overhead vs kernel-space
- DERP relay fallback: Slower than direct connections (but rare - 90%+ direct)
- Not optimized for bandwidth-intensive use (streaming, large file transfers)

**Geographic Restrictions**:
- Limited reliability in restrictive countries (e.g., China)
- WireGuard protocol doesn't obfuscate traffic (detectable)
- Coordination server dependency

**Public Access**:
- Tailscale Funnel (public exposure feature) limited platform availability
- Not designed for general public internet access (internal networking focus)

**Exit Nodes**:
- Exit node functionality limited to Linux devices currently
- Not a replacement for commercial VPNs for streaming/geo-unblocking

**Sources**:
- <a href="https://www.vpnmentor.com/reviews/tailscale/" target="_blank">Tailscale VPN Review 2026 - VPNMentor</a>
- <a href="https://privacysavvy.com/reviews/vpn/tailscale/" target="_blank">Tailscale VPN Review 2026 - PrivacySavvy</a>
- <a href="https://leetsaber.com/tailscale-risks" target="_blank">Tailscale Risks</a>

### 9.2 Privacy Considerations

**Data Collection**:
- Device hardware details
- Hostnames
- IP addresses (coordination purposes)

**Jurisdiction**:
- Tailscale headquartered in **Canada**
- Canada = Five Eyes Alliance member
- Concern for users requiring non-Five-Eyes jurisdiction

**Mitigation**:
- End-to-end encryption (Tailscale cannot decrypt traffic)
- No traffic inspection
- Self-hosted coordination server option (Headscale open-source)

**Sources**:
- <a href="https://tailscale.com/privacy-policy" target="_blank">Privacy Policy - Tailscale</a>

### 9.3 Support & Troubleshooting

**Support Tiers**:
- **Personal (Free)**: Community support only
- **Starter**: Email support
- **Premium**: Priority email support
- **Enterprise**: Dedicated support, SLA

**Potential Challenges**:
- Large teams: May need more robust support (Enterprise)
- Complex network setups: Requires networking expertise
- Subnet routing: Additional configuration complexity

**Sources**:
- <a href="https://tailscale.com/pricing" target="_blank">Tailscale Pricing</a>

---

## 10. Fit Assessment for Your Requirements

### 10.1 Requirements Matrix

| Requirement | Tailscale Support | Rating | Notes |
|-------------|------------------|--------|-------|
| **PostgreSQL TCP Access** | ✅ Direct access via Tailscale mesh | ⭐⭐⭐⭐⭐ | Native support, optional pgproxy for enhanced auditing |
| **30 Users** | ✅ Supported (all plans) | ⭐⭐⭐⭐⭐ | Usage-based billing, 3 free users |
| **Azure AD Integration** | ✅ Native SSO + provisioning (Enterprise) | ⭐⭐⭐⭐⭐ | Seamless integration, automatic sync |
| **MFA Support** | ✅ Via Azure AD | ⭐⭐⭐⭐⭐ | Leverages existing Azure AD MFA policies |
| **CCTV VLAN Access** | ✅ Subnet routing | ⭐⭐⭐⭐ | Requires subnet router setup, works well |
| **Budget (£300/month)** | ⚠️ Starter fits, Premium exceeds | ⭐⭐⭐ | Starter: £180 ✅, Premium: £540 ❌ |
| **Least Privilege ACLs** | ✅ Granular ACL policies | ⭐⭐⭐⭐⭐ | Port/protocol/user-based rules |
| **Intune Deployment** | ✅ MSI installer + MDM policies | ⭐⭐⭐⭐⭐ | Well-documented, tested |
| **GDPR Compliance** | ✅ DPA, EU rep, SCCs | ⭐⭐⭐⭐⭐ | Fully compliant |
| **Audit Logging** | ⚠️ Premium/Enterprise only | ⭐⭐⭐ | Not available on Starter plan |
| **UK/Europe Performance** | ✅ DERP in London, Madrid, Warsaw | ⭐⭐⭐⭐⭐ | Low latency, good coverage |
| **Zero Trust Alignment** | ✅ True ZTNA architecture | ⭐⭐⭐⭐⭐ | Identity-based, micro-segmentation |

**Overall Rating**: ⭐⭐⭐⭐ (4/5 stars)

**Strengths**:
- Excellent technical fit for PostgreSQL + CCTV access
- Strong Azure AD integration
- True Zero Trust architecture
- Easy deployment and management
- GDPR compliant

**Weaknesses**:
- **Premium plan exceeds budget** (£540 vs £300 target)
- Audit logging requires Premium/Enterprise
- Starter plan lacks user provisioning automation

### 10.2 Budget Scenarios

**Scenario 1: Starter Plan (£180/month)**
- ✅ **Fits budget** with £120/month headroom
- ✅ All core functionality (ACLs, subnet routing, Azure AD SSO)
- ❌ Manual user management (no auto-provisioning)
- ❌ No network flow logs
- ⚠️ Acceptable if manual user management tolerable

**Scenario 2: Premium Plan (£540/month)**
- ❌ **Exceeds budget** by £240/month (80% over)
- ✅ Network flow logs for auditing
- ✅ SOC 2 Type II certification
- ✅ Priority support
- ❌ Still lacks user provisioning (Enterprise only)

**Scenario 3: Enterprise Plan (Unknown Cost)**
- ❓ **Request quote** for 30 users
- ✅ All features (user provisioning, config audit logs, SLA)
- ❓ May negotiate volume discount
- ❓ Potentially within budget if discounted

**Scenario 4: Free Tier Testing**
- ✅ **£0 cost** for proof-of-concept
- ⚠️ Limited to 3 users (test with admin + 2 users)
- ✅ Validate PostgreSQL access, CCTV subnet routing
- ✅ Test Azure AD integration
- ✅ Verify Windows 11 client experience

### 10.3 Recommendations

**Immediate Action**:
1. **Deploy Free Tier Proof of Concept**:
   - Test with 3 users (2 admins + 1 regular user)
   - Validate PostgreSQL direct access
   - Test subnet routing for CCTV
   - Confirm Azure AD SSO + MFA
   - Measure performance and latency

2. **Request Enterprise Quote**:
   - Contact Tailscale sales for 30-user Enterprise pricing
   - Mention budget constraints (£300/month target)
   - Negotiate based on annual commitment
   - Ask about nonprofit/startup discounts (if applicable)

**Decision Path**:

**If Enterprise Quote ≤ £300/month**:
- ✅ **Proceed with Enterprise**
- Gain full feature set (provisioning, audit logs, SLA)
- Best long-term solution

**If Enterprise Quote > £300/month**:
- **Option A**: Accept Starter Plan (£180/month)
  - Manual user management acceptable?
  - Use PostgreSQL audit logs for database access tracking
  - Upgrade to Premium/Enterprise later if budget increases

- **Option B**: Negotiate Starter + Add-ons
  - Ask if audit logging available as paid add-on to Starter

- **Option C**: Explore Alternatives
  - Compare with competitors (Twingate, Cloudflare Tunnels, OpenZiti)
  - See companion document: `ZTNA_Provider_Research_2026.md`

**Long-Term Considerations**:
- Tailscale pricing may decrease with competition
- Your budget may increase as business grows
- Start with Starter, upgrade when audit logging becomes mandatory

---

## 11. Comparison with Traditional VPN

| Factor | Traditional VPN (IPSec/OpenVPN) | Tailscale ZTNA |
|--------|--------------------------------|----------------|
| **Setup Complexity** | High (server config, certs, routing) | Low (install client, authenticate) |
| **Performance** | Centralized bottleneck | Peer-to-peer (90%+ direct) |
| **Security Model** | Network-level trust | Identity + device verification |
| **Access Control** | Broad network access | Granular per-resource |
| **Scalability** | Requires VPN server scaling | Mesh architecture scales naturally |
| **User Experience** | Obvious VPN connection | Seamless (always-on available) |
| **Maintenance** | Manual cert renewal, updates | Automatic updates, key rotation |
| **Cost** | Infrastructure + licensing | Per-user subscription |
| **Azure AD Integration** | Complex (RADIUS, etc.) | Native SSO |
| **Audit Logging** | VPN server logs | Network flow logs + pgproxy logs |

**Verdict**: Tailscale significantly easier to deploy and manage than traditional VPN, with better Zero Trust alignment.

**Sources**:
- <a href="https://tailscale.com/use-cases/zero-trust-networking" target="_blank">Zero Trust Networking - Tailscale</a>
- <a href="https://www.shellfire.net/blog/tailscale-vs-vpn/" target="_blank">Tailscale vs VPN - Shellfire</a>

---

## 12. Conclusion & Next Steps

### 12.1 Summary

**Tailscale is an excellent technical fit** for your Zero Trust PostgreSQL access requirements:
- Strong Zero Trust architecture with identity-based access
- Native Azure AD integration with SSO and MFA
- Supports PostgreSQL direct access and CCTV subnet routing
- Easy Windows 11 deployment via Intune
- GDPR compliant with EU data protection representation
- Low administrative overhead

**Primary concern**: **Premium plan exceeds budget** (£540 vs £300/month target).

**Viable path forward**: Start with **Starter plan** (£180/month) or negotiate **Enterprise pricing**.

### 12.2 Recommended Next Steps

**Phase 1: Proof of Concept (Week 1-2)**
1. ✅ Sign up for Tailscale Free tier (3 users)
2. ✅ Configure Azure AD SSO integration
3. ✅ Test PostgreSQL access from Windows 11 laptop
4. ✅ Deploy subnet router for CCTV access
5. ✅ Create sample ACL policies (database, CCTV)
6. ✅ Measure connection performance and latency
7. ✅ Validate MFA enforcement via Azure AD
8. ✅ Test Intune deployment (single device)

**Phase 2: Pricing Evaluation (Week 3)**
1. ✅ Request Enterprise quote for 30 users
2. ✅ Compare Starter vs Premium vs Enterprise features vs needs
3. ✅ Evaluate if manual user management acceptable (Starter)
4. ✅ Consider alternative ZTNA providers (Twingate, Cloudflare, OpenZiti)

**Phase 3: Pilot Deployment (Week 4-6)**
1. ✅ Select plan based on pricing negotiation
2. ✅ Deploy to 5-10 pilot users
3. ✅ Refine ACL policies based on real usage
4. ✅ Create user documentation and training
5. ✅ Monitor performance and gather feedback
6. ✅ Test failover scenarios (subnet router failure, DERP relay)

**Phase 4: Full Rollout (Week 7-8)**
1. ✅ Deploy to all 30 users via Intune
2. ✅ Enable Always-On policy (if desired)
3. ✅ Configure PostgreSQL `pg_hba.conf` for Tailscale network
4. ✅ Implement audit logging (pgproxy or PostgreSQL native)
5. ✅ Document procedures for onboarding/offboarding users
6. ✅ Establish monitoring and alerting

### 12.3 Open Questions to Resolve

Before final decision, clarify:

1. **Pricing**:
   - What is actual Enterprise cost for 30 users?
   - Any discounts for annual commitment?
   - Does subnet router device count as a "user"?

2. **Audit Requirements**:
   - Is network flow logging mandatory, or can you use PostgreSQL audit logs only?
   - Does manual user management (Starter) meet compliance needs?

3. **User Access Patterns**:
   - How many users need simultaneous database access?
   - What applications will connect to PostgreSQL?
   - How many users need CCTV access (subset of 30)?

4. **Technical Details**:
   - Where will subnet router run (dedicated device, existing server)?
   - Current CCTV system details (brand, access method)?
   - Is PostgreSQL server on same network as CCTV, or different location?

### 12.4 Alternative Solutions to Consider

If Tailscale pricing doesn't work:

1. **Twingate**: Similar ZTNA, competitive pricing, may fit budget better
2. **Cloudflare Tunnels**: Free tier generous, but different architecture
3. **OpenZiti**: Open-source ZTNA, self-hosted (no subscription cost)
4. **Headscale**: Open-source Tailscale coordination server (removes subscription, but requires hosting)

See: `/mnt/c/Users/SteveIrwin/terminai/it/ZTNA_Provider_Research_2026.md` for comparisons.

---

## 13. Additional Resources

### Official Documentation
- <a href="https://tailscale.com/kb" target="_blank">Tailscale Knowledge Base</a>
- <a href="https://tailscale.com/kb/1337/acl-syntax" target="_blank">ACL Syntax Reference</a>
- <a href="https://learn.microsoft.com/en-us/entra/identity/saas-apps/tailscale-provisioning-tutorial" target="_blank">Microsoft Entra ID Integration Guide</a>

### Security & Compliance
- <a href="https://tailscale.com/security-bulletins" target="_blank">Security Bulletins</a>
- <a href="https://tailscale.com/dpa" target="_blank">Data Processing Addendum</a>
- <a href="https://tailscale.com/privacy-policy" target="_blank">Privacy Policy</a>

### Community Resources
- <a href="https://tailscale.com/blog" target="_blank">Tailscale Blog</a>
- <a href="https://github.com/tailscale/tailscale" target="_blank">Tailscale GitHub Repository</a>
- <a href="https://www.reddit.com/r/Tailscale/" target="_blank">r/Tailscale Reddit Community</a>

### PostgreSQL-Specific
- <a href="https://tailscale.com/blog/introducing-pgproxy" target="_blank">pgproxy Documentation</a>
- <a href="https://www.crunchydata.com/blog/crunchy-bridge-with-tailscale" target="_blank">Postgres + Tailscale - Crunchy Data</a>
- <a href="https://petar.dev/notes/connect-to-postgresql-from-taiscale/" target="_blank">Connect to PostgreSQL from Tailscale - Tutorial</a>

---

## Document Control

**Author**: IT Security Researcher (Claude Code)
**Created**: February 17, 2026
**Last Updated**: February 17, 2026
**Version**: 1.0
**Status**: Draft for Review

**Disclaimer**: This research is based on publicly available information as of February 2026. Pricing, features, and availability subject to change. Always verify current details with Tailscale sales and official documentation before making purchasing decisions.
