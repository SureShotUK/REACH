# SSH Bastion Host vs ZTNA Solutions for PostgreSQL Database Access
## Custom SSH Bastion with Azure AD Authentication Research

**Research Date:** February 17, 2026
**Environment:** PostgreSQL 16.11 on Ubuntu 24.04 LTS, 30 Azure AD users, UK-based
**Budget:** £300/month total (£10/user/month maximum)
**Requirements:** Azure AD authentication, MFA enforcement, CCTV access, low maintenance

---

## Executive Summary

This research evaluates custom SSH bastion host implementations with Azure AD authentication compared to SaaS ZTNA solutions for providing secure remote access to an on-premises PostgreSQL database and CCTV system.

### Key Findings

| Solution Type | Monthly Cost (30 users) | Setup Complexity | Maintenance Overhead | User Experience | Recommendation |
|---------------|-------------------------|------------------|----------------------|-----------------|----------------|
| **Self-Hosted SSH Bastion** | £5-50 | ⚠️ **High** | ⚠️ **High** | ⚠️ **Poor** | ❌ **Not Recommended** |
| **Commercial SSH Bastion** (Teleport, Guacamole) | £0-500+ | Medium-High | Medium | Fair-Good | ⚠️ **Consider with caution** |
| **SaaS ZTNA** (Tailscale, Twingate, Cloudflare) | £150-250 | ✅ **Low** | ✅ **Very Low** | ✅ **Excellent** | ✅ **Strongly Recommended** |

### Bottom-Line Recommendation

**Do NOT build a custom SSH bastion host.** The operational complexity, maintenance burden, and poor user experience make self-hosted SSH bastions unsuitable for small IT teams. Instead, adopt a **SaaS ZTNA solution** (Tailscale, Twingate, or Cloudflare Zero Trust) that provides:

- Simpler setup and deployment (hours vs. weeks)
- Lower maintenance burden (vendor-managed infrastructure)
- Better user experience (transparent mesh networking vs. manual SSH tunnels)
- Lower total cost of ownership when factoring in IT labor
- Built-in Azure AD integration with MFA enforcement
- Continuous updates and security patches without IT intervention

---

## 1. Architecture Overview: SSH Bastion vs ZTNA Mesh

### 1.1 Traditional SSH Bastion Architecture

**How It Works:**

```
User Laptop                SSH Bastion Host              PostgreSQL Server
(Windows 11)  ----SSH---> (Ubuntu 24.04)    ----SSH---> (Ubuntu 24.04)
              Tunnel 5432  Azure AD Auth                Port 5432
                           Public IP
```

**Key Components:**
1. **SSH Bastion Host**: Ubuntu 24.04 VM exposed to internet
2. **Azure AD Integration**: PAM module or certificate-based authentication
3. **Port Forwarding**: SSH tunnel from user laptop → bastion → database
4. **Firewall Rules**: Open SSH port 22 on perimeter firewall (or bastion in DMZ)

**Access Flow:**
1. User initiates SSH connection to bastion host
2. Bastion authenticates user via Azure AD (PAM module or certificate)
3. User establishes SSH tunnel: `ssh -L 5432:postgres-server:5432 user@bastion`
4. Database client connects to `localhost:5432` on user laptop
5. Traffic tunneled through SSH to PostgreSQL server

**Sources:**
- <a href="https://www.postgresql.org/docs/current/ssh-tunnels.html" target="_blank">PostgreSQL: Secure TCP/IP Connections with SSH Tunnels</a>
- <a href="https://learn.microsoft.com/en-us/entra/identity/devices/howto-vm-sign-in-azure-ad-linux" target="_blank">Microsoft: Sign in to Linux VM using Microsoft Entra ID</a>

### 1.2 Modern ZTNA Mesh Architecture (e.g., Tailscale)

**How It Works:**

```
User Laptop                Coordination Server           PostgreSQL Server
(Windows 11)  <--Mesh--->  (Tailscale/Twingate)  <--Mesh--> (Ubuntu 24.04)
Tailscale Client           Azure AD Auth                   Tailscale Client
100.x.x.x      Direct P2P Connection (WireGuard)          100.y.y.y
```

**Key Components:**
1. **Mesh Network**: Peer-to-peer WireGuard tunnels (no central gateway)
2. **Coordination Server**: SaaS control plane (authentication, ACL distribution)
3. **Client Agents**: Lightweight software on user laptops and database server
4. **Azure AD SSO**: Native OAuth/SAML integration

**Access Flow:**
1. User authenticates once to Tailscale/Twingate via Azure AD SSO
2. Client agent establishes encrypted mesh connection to database server
3. User connects directly to database: `psql -h 100.y.y.y -p 5432`
4. No manual SSH tunnels or port forwarding required
5. ACL policies enforce least-privilege access (database only, not entire network)

**Sources:**
- <a href="https://tailscale.com/blog/how-tailscale-works" target="_blank">Tailscale: How it works</a>
- <a href="https://www.twingate.com/blog/bastion-host" target="_blank">Twingate: Bastion Host Servers Explained</a>
- <a href="https://hoop.dev/blog/ztna-vs-bastion-host-a-modern-solution-for-secure-network-access/" target="_blank">ZTNA vs. Bastion Host: A Modern Solution</a>

---

## 2. Self-Hosted SSH Bastion with Azure AD Integration

### 2.1 Implementation Options

#### Option A: Microsoft Entra ID SSH Extension (Azure VMs Only)

**Overview:**
Microsoft's official solution using the `AADSSHLoginForLinux` VM extension for certificate-based SSH authentication.

**Limitations:**
- ❌ **Azure VMs Only**: Designed for Azure-hosted Linux VMs, not on-premises servers
- ❌ **Requires Azure VNet**: Bastion must be in Azure, not on-premises
- ❌ **Not Applicable**: Your PostgreSQL server is on-premises behind SonicWall

**Verdict:** Not suitable for on-premises environments.

**Sources:**
- <a href="https://learn.microsoft.com/en-us/entra/identity/devices/howto-vm-sign-in-azure-ad-linux" target="_blank">Microsoft: Sign in to Linux VM using Entra ID and OpenSSH</a>
- <a href="https://ubuntu.com/tutorials/using-azure-ad-to-manage-ssh-logins-to-ubuntu" target="_blank">Ubuntu: Using Azure AD to manage SSH logins</a>

#### Option B: PAM Module Integration (pam-aad-oidc, pam_aad)

**Overview:**
Community-developed PAM (Pluggable Authentication Modules) libraries that integrate Linux SSH authentication with Azure AD using OAuth2/OIDC.

**Available Solutions:**
1. **pam-aad-oidc** (Alan Turing Institute): OAuth2/OIDC PAM module
2. **pam_aad** (CyberNinjas): Azure AD PAM integration
3. **Himmelblau** (Microsoft-affiliated): Modern PAM/NSS suite for Azure AD

**Implementation Steps:**
1. Install PAM module on Ubuntu 24.04 bastion host
2. Configure `/etc/pam.d/sshd` to use Azure AD authentication
3. Register SSH bastion as Azure AD application
4. Configure OAuth2 redirect URIs and client credentials
5. Test SSH authentication flow with Azure AD credentials

**Complexity Assessment:**
- ⚠️ **High Complexity**: Manual PAM configuration, OAuth app registration, debugging authentication flows
- ⚠️ **Limited Documentation**: Community projects with sparse production deployment examples
- ⚠️ **Ongoing Maintenance**: Module updates, Azure AD API changes, compatibility testing
- ⚠️ **Troubleshooting Difficulty**: SSH authentication failures are notoriously difficult to debug

**Sources:**
- <a href="https://github.com/alan-turing-institute/pam-aad-oidc" target="_blank">GitHub: pam-aad-oidc (Alan Turing Institute)</a>
- <a href="https://github.com/CyberNinjas/pam_aad" target="_blank">GitHub: pam_aad (CyberNinjas)</a>
- <a href="https://www.puppeteers.net/blog/linux-azure-ad-authentication-options/" target="_blank">Puppeteers: Linux Azure AD authentication options</a>

#### Option C: SSH Certificate-Based Authentication with Azure AD

**Overview:**
Use Azure AD to issue short-lived SSH certificates instead of password-based authentication.

**Implementation Steps:**
1. Set up Azure AD as SSH Certificate Authority (CA)
2. Configure SSH server to accept certificates signed by Azure AD CA
3. Users authenticate to Azure AD and receive short-lived SSH certificates
4. Use certificates to SSH into bastion host

**Complexity Assessment:**
- ⚠️ **Very High Complexity**: Custom certificate issuance pipeline, key management infrastructure
- ⚠️ **Limited Tooling**: No official Microsoft tooling for on-premises SSH certificate issuance
- ⚠️ **Operational Burden**: Certificate rotation, revocation, monitoring

**Verdict:** Too complex for small IT teams without dedicated security engineers.

**Sources:**
- <a href="https://learn.microsoft.com/en-us/entra/architecture/auth-ssh" target="_blank">Microsoft: SSH authentication with Microsoft Entra ID</a>

### 2.2 SSH Hardening Requirements (CIS Benchmarks)

If implementing a self-hosted SSH bastion, you MUST apply CIS benchmark hardening to reduce attack surface.

**Required Security Configurations:**

1. **SSH Configuration (`/etc/ssh/sshd_config`)**:
   - Disable root login: `PermitRootLogin no`
   - Key-only authentication: `PasswordAuthentication no`, `PubkeyAuthentication yes`
   - Preferred key types: ED25519 (prioritize over RSA)
   - Strong ciphers: AES-256-GCM, ChaCha20-Poly1305
   - Strong MACs: hmac-sha2-512, hmac-sha2-256
   - Strong key exchange: diffie-hellman-group18-sha512, curve25519-sha256

2. **fail2ban**: Automatic IP blocking after repeated failed SSH attempts
   - Ban time: 1 hour minimum
   - Max retries: 5 attempts
   - Monitor `/var/log/auth.log` for SSH failures

3. **Firewall Rules**:
   - Restrict SSH access to known IP ranges (if possible)
   - Rate-limit SSH connections
   - Implement connection throttling

4. **System Hardening**:
   - Apply CIS Ubuntu 24.04 LTS Benchmark Level 1 (minimum)
   - Regular security patching (weekly minimum)
   - Kernel hardening (sysctl configurations)
   - Disable unnecessary services

5. **Monitoring and Logging**:
   - Centralized log forwarding (syslog to SIEM)
   - SSH session recording (if compliance required)
   - Alerting on failed authentication attempts
   - Regular log review

**Automation Tools:**
- Ubuntu Security Guide (USG) for automated CIS hardening
- OpenSCAP for compliance scanning
- Ansible playbooks for configuration management

**Sources:**
- <a href="https://ubuntu.com/blog/hardening-automation-for-cis-benchmarks-now-available-for-ubuntu-24-04-lts" target="_blank">Ubuntu: CIS Benchmark Hardening Automation for 24.04 LTS</a>
- <a href="https://www.cisecurity.org/benchmark/ubuntu_linux" target="_blank">CIS: Ubuntu Linux Benchmarks</a>
- <a href="https://www.sshaudit.com/hardening_guides.html" target="_blank">SSH Audit: SSH Hardening Guides</a>
- <a href="https://medium.com/@paulhoke/how-to-set-up-and-harden-a-new-ubuntu-24-04-server-1929ac72161f" target="_blank">Medium: How to Set Up and Harden Ubuntu 24.04 Server</a>

### 2.3 Windows 11 Client Setup (SSH Port Forwarding)

For users to access PostgreSQL via SSH tunnel, they must configure SSH port forwarding on Windows 11.

#### Manual SSH Tunnel (Command Line)

**Using Windows 11 Native OpenSSH Client:**

```powershell
# Establish SSH tunnel to bastion, forward local port 5432 to database server
ssh -L 5432:postgres-server.local:5432 azuread-user@bastion.example.com

# Keep terminal window open while using database connection
# Connect database client to localhost:5432
```

**Using PuTTY:**

1. Open PuTTY Configuration
2. Session → Host Name: `bastion.example.com`, Port: 22
3. Connection → SSH → Tunnels:
   - Source port: `5432`
   - Destination: `postgres-server.local:5432`
   - Click "Add"
4. Session → Save configuration
5. Open connection (keep PuTTY window open during database session)

**Database Client Configuration:**
- **Host**: `localhost` (or `127.0.0.1`)
- **Port**: `5432`
- **Username/Password**: PostgreSQL credentials

**Sources:**
- <a href="https://www.postgresql.org/docs/current/ssh-tunnels.html" target="_blank">PostgreSQL: Secure TCP/IP Connections with SSH Tunnels</a>
- <a href="http://postgresonline.com/article_pfriendly/38.html" target="_blank">PuTTY for SSH Tunneling to PostgreSQL Server</a>

#### User Experience Problems

❌ **Manual Tunnel Management**: Users must establish SSH tunnel before each database session
❌ **Terminal Window Required**: Must keep SSH terminal or PuTTY window open during entire session
❌ **Port Conflicts**: Local port 5432 conflicts if PostgreSQL installed locally (must use alternate port like 15432)
❌ **Connection Failures**: Tunnel drops if SSH connection interrupted (network change, laptop sleep)
❌ **Training Required**: Non-technical users struggle with SSH command-line syntax
❌ **No Auto-Reconnect**: Tunnel does not automatically restore after network interruption

**Verdict:** Poor user experience compared to transparent ZTNA mesh connections.

**Sources:**
- <a href="https://dbeaver.com/docs/dbeaver/SSH-Configuration/" target="_blank">DBeaver: SSH Configuration</a>
- <a href="https://www.enterprisedb.com/blog/ssh-tunneling-pgadmin-4" target="_blank">EnterpriseDB: SSH Tunneling in pgAdmin 4</a>

### 2.4 CCTV Access via SSH Tunneling

**CCTV System Requirements:**
- HTTPS web interface (port 443)
- RTSP video streaming (port 554)
- Different VLAN from database server

#### SSH Tunneling for CCTV

**Option 1: Multiple Local Port Forwards**

```bash
# Tunnel HTTPS web interface
ssh -L 8443:cctv.local:443 user@bastion

# Tunnel RTSP stream
ssh -L 8554:cctv.local:554 user@bastion

# Access:
# Web interface: https://localhost:8443
# RTSP stream: rtsp://localhost:8554/stream
```

**Option 2: Dynamic Port Forwarding (SOCKS Proxy)**

```bash
# Create SOCKS5 proxy on local port 1080
ssh -D 1080 user@bastion

# Configure browser to use SOCKS5 proxy: localhost:1080
# All traffic tunneled through SSH connection
```

**SOCKS Proxy Benefits:**
- ✅ Single SSH connection handles multiple services
- ✅ No need to specify individual port forwards
- ✅ Works with HTTPS, RTSP, and other protocols
- ✅ Can tunnel multiple VLANs through one connection

**SOCKS Proxy Drawbacks:**
- ⚠️ Requires browser/application configuration
- ⚠️ Not transparent (users must configure proxy settings)
- ⚠️ Some applications don't support SOCKS5

**RTSP Streaming Considerations:**
- RTSP streams work over SSH tunnels but may have latency issues
- Real-time video streaming sensitive to tunnel interruptions
- Bandwidth overhead from SSH encryption

**Sources:**
- <a href="https://zaiste.net/posts/ssh-port-forwarding/" target="_blank">Zaiste: SSH Port Forwarding</a>
- <a href="https://sumit-ghosh.com/posts/socks-proxy-ssh-tunneling-dynamic-port-forwarding/" target="_blank">Creating a SOCKS Proxy via SSH Tunneling</a>
- <a href="https://developer.ridgerun.com/wiki/index.php/How_to_use_local_port_forwarding_to_get_a_remote_RTSP_video_stream" target="_blank">RidgeRun: Local Port Forwarding for RTSP Video Streams</a>

### 2.5 Infrastructure Requirements and Costs

#### Self-Hosted Bastion (On-Premises)

**Hardware Requirements (30 Concurrent Users):**
- CPU: 2-4 cores
- RAM: 6-8 GB
- Storage: 50 GB SSD (OS + logs + backups)
- Network: 1 Gbps NIC

**Monthly Costs:**
- VM hosting (if cloud-based): £0 (on-premises) to £30-50 (Azure B2s VM)
- Electricity (on-premises): £5-10
- Backup storage: £5
- **Total Infrastructure**: £5-65/month

**Sources:**
- <a href="https://ubuntu.com/server/docs/reference/installation/system-requirements/" target="_blank">Ubuntu Server: System Requirements</a>
- <a href="https://saumiks.medium.com/configuring-a-linux-bastion-host-aka-jumpbox-3362f650dc5d" target="_blank">Configuring a Linux Bastion Host</a>

#### Cloud-Hosted Bastion (Azure)

**Azure VM Pricing (UK South Region):**
- **B1s** (1 vCPU, 1 GB RAM): £0.0104/hour ≈ £7.50/month (too small for 30 users)
- **B2s** (2 vCPU, 4 GB RAM): £0.0416/hour ≈ £30/month
- **B2ms** (2 vCPU, 8 GB RAM): £0.0832/hour ≈ £60/month (recommended)

**Additional Azure Costs:**
- Storage (50 GB SSD): £5/month
- Bandwidth egress: First 100 GB free, then £0.087/GB
- Public IP address: £3/month
- **Total Azure**: £38-68/month

**Bandwidth Estimates (30 Users):**
- SSH control traffic: ~1-2 GB/month
- PostgreSQL queries: Depends on usage (estimate 10-50 GB/month)
- CCTV HTTPS: 5-10 GB/month per user
- CCTV RTSP: 50-200 GB/month per active viewer
- **Total Bandwidth**: 50-500 GB/month (£0-35/month egress)

**Sources:**
- <a href="https://azure.microsoft.com/en-us/pricing/details/virtual-machines/linux/" target="_blank">Azure: Linux Virtual Machines Pricing</a>
- <a href="https://azure.microsoft.com/en-us/pricing/details/bandwidth/" target="_blank">Azure: Bandwidth Pricing</a>
- <a href="https://cloudprice.net/" target="_blank">CloudPrice: Azure VM Pricing Comparison</a>

### 2.6 Maintenance Overhead Assessment

#### Initial Setup Time

**Self-Hosted SSH Bastion with Azure AD PAM:**
- Ubuntu 24.04 installation and hardening: 4-8 hours
- PAM module installation and configuration: 4-8 hours
- Azure AD app registration and OAuth setup: 2-4 hours
- SSH hardening and fail2ban configuration: 2-4 hours
- Firewall rules and network configuration: 2-4 hours
- Testing and troubleshooting: 4-8 hours
- User documentation and training: 4-8 hours
- **Total Setup**: 22-44 hours (3-6 days)

**Ongoing Maintenance (Per Month):**
- Security patching (weekly): 2-4 hours/month
- Log review and monitoring: 2-4 hours/month
- User troubleshooting (SSH tunnel issues): 4-8 hours/month
- Azure AD token/certificate rotation: 1-2 hours/month
- Backup verification: 1-2 hours/month
- Performance monitoring: 1-2 hours/month
- **Total Maintenance**: 11-22 hours/month

**Annual Maintenance:**
- Ubuntu major version upgrades: 8-16 hours/year
- CIS benchmark re-assessment: 4-8 hours/year
- Security audit and penetration testing: 16-32 hours/year
- Disaster recovery testing: 4-8 hours/year
- **Total Annual**: 32-64 hours/year (2.5-5 hours/month average)

**Total Ongoing Effort**: 13.5-27 hours/month

**IT Labor Cost (£50/hour average):**
- Setup: £1,100-2,200 (one-time)
- Monthly maintenance: £675-1,350/month
- **Total First Year**: £9,200-18,400

**Sources:**
- <a href="https://www.centreon.com/saas-it-monitoring-vs-self-hosted-can-tco-help-make-the-best-choice/" target="_blank">Centreon: SaaS vs. Self-Hosted TCO Analysis</a>
- <a href="https://www.bastionzero.com/resources/to-self-host-or-not-to-self-host" target="_blank">BastionZero: Self-Hosted vs. SaaS Decision Framework</a>

---

## 3. Commercial SSH Bastion Solutions

### 3.1 Teleport

**Overview:**
Enterprise SSH/Kubernetes access platform with certificate-based authentication and session recording.

**Pricing (2026):**
- **Community Edition (Free)**: Companies <100 employees, <$10M revenue
  - ⚠️ **Commercial License Restriction**: Not freely usable by most businesses as of v16
- **Starter**: Custom pricing (estimated $10-20/user/month)
- **Premium**: Custom pricing (estimated $30-50/user/month)
- **Enterprise**: Custom pricing

**30 Users Cost Estimate**: £300-500/month (exceeds budget)

**Key Features:**
- Azure AD SSO via SAML/OIDC
- Certificate-based SSH authentication
- Session recording and audit logs
- Role-based access control (RBAC)
- Web-based terminal access
- Native Windows/macOS/Linux clients

**Pros:**
- ✅ Built for enterprise SSH access management
- ✅ Strong audit and compliance features
- ✅ Native Azure AD integration
- ✅ Session recording for compliance

**Cons:**
- ❌ **Exceeds Budget**: Likely £300-500+/month for 30 users
- ❌ Community Edition no longer free for most businesses
- ⚠️ Requires dedicated infrastructure (self-hosted) or use cloud service
- ⚠️ Complex setup and management

**User Experience:**
- Better than manual SSH tunnels (dedicated client apps)
- Still requires users to launch Teleport client
- Web-based terminal available but limited for database clients

**Sources:**
- <a href="https://goteleport.com/pricing/" target="_blank">Teleport: Pricing</a>
- <a href="https://github.com/gravitational/teleport/discussions/39158" target="_blank">GitHub: Teleport Community Edition License Change</a>
- <a href="https://goteleport.com/docs/feature-matrix/" target="_blank">Teleport: Feature Matrix</a>

### 3.2 HashiCorp Boundary

**Overview:**
Identity-based access management for dynamic infrastructure with OIDC integration.

**Pricing (2026):**
- **Community Edition (Free)**: Open-source, self-hosted
- **HCP Boundary (Cloud)**: Custom pricing (no public rates)
- **Enterprise**: Custom pricing

**30 Users Cost Estimate**: Free (Community) to £200-400+/month (Enterprise)

**Key Features:**
- Azure AD OIDC authentication
- Identity-based access (no network credentials)
- Dynamic host catalogs
- Session management
- Integration with HashiCorp Vault for dynamic credentials

**Pros:**
- ✅ Free Community Edition available
- ✅ Azure AD OIDC integration supported
- ✅ Modern identity-based architecture
- ✅ No bastion host maintenance (uses connectors)

**Cons:**
- ⚠️ **Community Edition Limitations**:
  - No SCIM integration
  - Basic audit logging only (no session recording)
  - No enterprise support
- ⚠️ Complex setup with multiple components (Boundary + Consul)
- ⚠️ Limited documentation and community resources
- ⚠️ Primarily designed for cloud infrastructure (not on-premises focus)

**User Experience:**
- Requires Boundary CLI or desktop client
- Not as user-friendly as ZTNA mesh solutions
- More suited to DevOps teams than general business users

**Sources:**
- <a href="https://developer.hashicorp.com/boundary/docs/community" target="_blank">HashiCorp: Boundary Community Edition</a>
- <a href="https://developer.hashicorp.com/boundary/tutorials/identity-management/oidc-azure" target="_blank">HashiCorp: OIDC Authentication with Azure</a>
- <a href="https://www.hashicorp.com/products/boundary/pricing" target="_blank">HashiCorp: Boundary Pricing</a>

### 3.3 Apache Guacamole

**Overview:**
Clientless remote desktop gateway supporting RDP, SSH, VNC via HTML5 browser interface.

**Pricing:**
- **Open Source (Free)**: Self-hosted
- **Managed Services**: Various providers (£50-200+/month depending on provider)

**30 Users Cost Estimate**: £0-10/month (infrastructure only for self-hosted)

**Key Features:**
- Azure AD SAML SSO integration
- Browser-based access (no client installation)
- Supports SSH, RDP, VNC, Kubernetes
- Session recording
- Two-factor authentication

**Pros:**
- ✅ Free and open source
- ✅ Azure AD SAML integration available
- ✅ Browser-based (no client software required)
- ✅ Well-documented with active community
- ✅ Supports both database (SSH) and CCTV (HTTPS) access

**Cons:**
- ⚠️ **High Maintenance Overhead**:
  - Requires PostgreSQL database for Guacamole configuration
  - Manual schema upgrades when updating versions
  - Tomcat application server management
  - Web server (Nginx/Apache) configuration with SSL
  - All maintenance responsibilities on your team
- ⚠️ Requires bastion/jump host architecture (additional VM)
- ⚠️ Session interruptions disconnect all users
- ⚠️ PostgreSQL database access requires SSH passthrough (not direct connection)

**User Experience:**
- Good for RDP/SSH terminal access via browser
- Poor for database client tools (must use browser-based SQL client or SSH tunnel)
- Not ideal for CCTV RTSP streaming

**Database Access Pattern:**
Users → Guacamole web UI → SSH session → `psql` or SSH tunnel setup → Database

**Sources:**
- <a href="https://guacamole.apache.org/doc/gug/saml-auth.html" target="_blank">Apache Guacamole: SAML Authentication</a>
- <a href="https://nathancatania.com/posts/deploy-guacamole-ssl-saml/" target="_blank">Nathan Catania: Deploy Guacamole with SSL & SAML (Azure AD)</a>
- <a href="https://guacamole.apache.org/doc/1.6.0/gug/postgresql-auth.html" target="_blank">Apache Guacamole: PostgreSQL Database Setup</a>

---

## 4. Total Cost of Ownership (TCO) Comparison

### 4.1 Self-Hosted SSH Bastion

| Cost Category | One-Time | Monthly | Annual |
|---------------|----------|---------|--------|
| **Infrastructure** | | | |
| Azure B2ms VM (UK South) | - | £60 | £720 |
| Storage (50 GB SSD) | - | £5 | £60 |
| Public IP | - | £3 | £36 |
| Bandwidth (100 GB/month avg) | - | £0-10 | £0-120 |
| **Labor** | | | |
| Initial setup (30 hours @ £50/hr) | £1,500 | - | - |
| Monthly maintenance (15 hours) | - | £750 | £9,000 |
| Annual tasks (40 hours) | - | - | £2,000 |
| **Total Year 1** | £1,500 | £818 | **£11,316** |
| **Total Year 2+** | - | £818 | **£9,816** |

**Per-User Cost**: £377/user/year (Year 1), £327/user/year (Year 2+)
**38x over budget** (vs. £120/user/year target)

### 4.2 Teleport (Estimated Enterprise Pricing)

| Cost Category | One-Time | Monthly | Annual |
|---------------|----------|---------|--------|
| Teleport licenses (30 users @ £15/user) | - | £450 | £5,400 |
| Infrastructure (self-hosted) | - | £50 | £600 |
| Initial setup (8 hours @ £50/hr) | £400 | - | - |
| Monthly maintenance (2 hours) | - | £100 | £1,200 |
| **Total Year 1** | £400 | £600 | **£7,600** |
| **Total Year 2+** | - | £600 | **£7,200** |

**Per-User Cost**: £253/user/year (Year 1), £240/user/year (Year 2+)
**2x over budget**

### 4.3 HashiCorp Boundary Community (Free)

| Cost Category | One-Time | Monthly | Annual |
|---------------|----------|---------|--------|
| Boundary Community (free) | - | £0 | £0 |
| Infrastructure (VM + Consul) | - | £50-80 | £600-960 |
| Initial setup (16 hours @ £50/hr) | £800 | - | - |
| Monthly maintenance (4 hours) | - | £200 | £2,400 |
| **Total Year 1** | £800 | £250-280 | **£3,800-4,160** |
| **Total Year 2+** | - | £250-280 | **£3,000-3,360** |

**Per-User Cost**: £127-139/user/year (Year 1), £100-112/user/year (Year 2+)
**Near budget but high complexity**

### 4.4 Apache Guacamole (Self-Hosted)

| Cost Category | One-Time | Monthly | Annual |
|---------------|----------|---------|--------|
| Guacamole (free) | - | £0 | £0 |
| Infrastructure (VM) | - | £50 | £600 |
| PostgreSQL for Guacamole config | - | £10 | £120 |
| Initial setup (20 hours @ £50/hr) | £1,000 | - | - |
| Monthly maintenance (6 hours) | - | £300 | £3,600 |
| **Total Year 1** | £1,000 | £360 | **£5,320** |
| **Total Year 2+** | - | £360 | **£4,320** |

**Per-User Cost**: £177/user/year (Year 1), £144/user/year (Year 2+)
**20-40% over budget**

### 4.5 SaaS ZTNA Solutions (Tailscale, Twingate, Cloudflare)

| Cost Category | One-Time | Monthly | Annual |
|---------------|----------|---------|--------|
| **Tailscale Starter** (30 users @ £6/user) | - | £180 | £2,160 |
| Initial setup (2 hours @ £50/hr) | £100 | - | - |
| Monthly maintenance (0.5 hours) | - | £25 | £300 |
| **Total Year 1** | £100 | £205 | **£2,560** |
| **Total Year 2+** | - | £205 | **£2,460** |

**Per-User Cost**: £85/user/year (Year 1), £82/user/year (Year 2+)
**30% UNDER budget** ✅

| Cost Category | One-Time | Monthly | Annual |
|---------------|----------|---------|--------|
| **Cloudflare Zero Trust** (30 users @ £7/user) | - | £210 | £2,520 |
| Initial setup (2 hours @ £50/hr) | £100 | - | - |
| Monthly maintenance (0.5 hours) | - | £25 | £300 |
| **Total Year 1** | £100 | £235 | **£2,920** |
| **Total Year 2+** | - | £235 | **£2,820** |

**Per-User Cost**: £97/user/year (Year 1), £94/user/year (Year 2+)
**20% UNDER budget** ✅

**Sources:**
- <a href="https://www.centreon.com/saas-it-monitoring-vs-self-hosted-can-tco-help-make-the-best-choice/" target="_blank">Centreon: SaaS vs. Self-Hosted TCO</a>
- <a href="https://www.bastionzero.com/resources/to-self-host-or-not-to-self-host" target="_blank">BastionZero: Self-Hosted vs. SaaS Cost Analysis</a>

---

## 5. Comparison: SSH Bastion vs SaaS ZTNA

### 5.1 Feature Comparison Matrix

| Feature | Self-Hosted SSH Bastion | Commercial SSH Bastion (Teleport) | SaaS ZTNA (Tailscale/Twingate) |
|---------|------------------------|-----------------------------------|--------------------------------|
| **Setup Complexity** | ⚠️ High (22-44 hours) | ⚠️ Medium (8-16 hours) | ✅ Low (1-4 hours) |
| **Azure AD Integration** | ⚠️ Manual (PAM modules) | ✅ Native SAML/OIDC | ✅ Native OAuth/SAML |
| **MFA Enforcement** | ⚠️ Azure AD dependent | ✅ Built-in + Azure AD | ✅ Native Azure AD MFA |
| **User Experience** | ❌ Poor (manual SSH tunnels) | ⚠️ Fair (client required) | ✅ Excellent (transparent mesh) |
| **Maintenance Burden** | ❌ High (15+ hours/month) | ⚠️ Medium (4-8 hours/month) | ✅ Very Low (<1 hour/month) |
| **Database Access** | ⚠️ SSH tunnel required | ⚠️ Proxy/tunnel required | ✅ Direct TCP connection |
| **CCTV Access** | ⚠️ Complex (SOCKS proxy) | ⚠️ Limited support | ✅ Direct HTTPS/RTSP |
| **Disaster Recovery** | ⚠️ Manual failover | ⚠️ HA setup required | ✅ Automatic mesh healing |
| **Security Patching** | ❌ Manual weekly updates | ⚠️ Vendor updates | ✅ Vendor-managed |
| **Session Recording** | ⚠️ Manual setup | ✅ Built-in | ⚠️ Limited (connection logs only) |
| **Cost (30 users, Year 1)** | ❌ £11,316 | ❌ £7,600 | ✅ £2,560-2,920 |
| **Per-User Cost/Year** | ❌ £377 | ❌ £253 | ✅ £85-97 |
| **Scalability** | ⚠️ Manual capacity planning | ⚠️ Manual scaling | ✅ Automatic |
| **Vendor Support** | ❌ None (DIY) | ✅ Enterprise support | ✅ Business support |
| **GDPR Compliance** | ⚠️ Your responsibility | ✅ Vendor DPA | ✅ Vendor DPA |

### 5.2 User Experience Comparison

#### Self-Hosted SSH Bastion User Flow

**Database Access:**
1. Open terminal or PuTTY
2. Authenticate to Azure AD (via PAM or certificate)
3. Establish SSH tunnel: `ssh -L 5432:postgres:5432 user@bastion`
4. Keep terminal window open
5. Open database client (pgAdmin, DBeaver)
6. Connect to `localhost:5432`
7. If tunnel drops, restart from step 1

**Steps Required**: 7 steps, manual tunnel management
**User Training**: 30-60 minutes, moderate technical skill required
**Failure Points**: Network interruption, laptop sleep, port conflicts

#### SaaS ZTNA User Flow (Tailscale/Twingate)

**Database Access:**
1. Authenticate to Tailscale/Twingate via Azure AD (one-time setup)
2. Open database client (pgAdmin, DBeaver)
3. Connect to `100.x.x.x:5432` (Tailscale IP)

**Steps Required**: 2 steps (after initial setup), zero manual tunnel management
**User Training**: 5-10 minutes, no technical skill required
**Failure Points**: Automatic reconnection on network changes

**User Experience Verdict**: SaaS ZTNA is 70% simpler with near-zero ongoing user friction.

**Sources:**
- <a href="https://hoop.dev/blog/ztna-vs-bastion-host-a-modern-solution-for-secure-network-access/" target="_blank">ZTNA vs. Bastion Host: User Experience</a>
- <a href="https://goteleport.com/blog/do-we-still-need-a-bastion/" target="_blank">Teleport: Do You Still Need a Bastion?</a>

### 5.3 Security Posture Comparison

| Security Control | SSH Bastion | SaaS ZTNA |
|------------------|-------------|-----------|
| **Authentication** | Azure AD (via PAM/cert) | Azure AD (native SSO) |
| **MFA Enforcement** | Azure AD MFA | Azure AD MFA |
| **Device Trust** | ❌ No (unless custom script) | ✅ Device posture checking |
| **Network Segmentation** | ⚠️ Firewall rules | ✅ Application-level ACLs |
| **Least Privilege** | ⚠️ Broad SSH access | ✅ Resource-level policies |
| **Continuous Verification** | ❌ No (trust after auth) | ✅ Per-connection auth |
| **Encryption** | ✅ SSH (AES-256) | ✅ WireGuard (ChaCha20) |
| **Audit Logging** | ⚠️ SSH logs (manual setup) | ✅ Centralized audit logs |
| **Vulnerability Surface** | ⚠️ Exposed SSH port 22 | ✅ Zero inbound ports |
| **Zero Trust Alignment** | ⚠️ Partial | ✅ Full |

**Sources:**
- <a href="https://nordlayer.com/blog/bastion-host/" target="_blank">NordLayer: What is a Bastion Host?</a>
- <a href="https://www.netskope.com/blog/leaving-bastion-hosts-behind-part-1-gcp" target="_blank">Netskope: Leaving Bastion Hosts Behind</a>

### 5.4 Operational Complexity Assessment

#### Self-Hosted SSH Bastion

**Day 1 (Initial Deployment):**
- Ubuntu installation and hardening: 4-8 hours
- PAM module configuration: 4-8 hours
- Azure AD app registration: 2-4 hours
- SSH hardening and fail2ban: 2-4 hours
- Testing and troubleshooting: 4-8 hours
- **Total**: 16-32 hours

**Week 1-4 (User Onboarding):**
- User training (30 min per user × 30 users): 15 hours
- Troubleshooting SSH tunnel issues: 8-12 hours
- Firewall rule adjustments: 2-4 hours
- **Total**: 25-31 hours

**Month 2+ (Ongoing):**
- Weekly security patching: 1 hour/week = 4 hours/month
- User support (SSH issues): 4-8 hours/month
- Log review and monitoring: 2-4 hours/month
- Azure AD token rotation: 1-2 hours/month
- **Total**: 11-18 hours/month

**Annual Events:**
- Ubuntu major upgrade: 8-16 hours/year
- Security audit: 16-32 hours/year
- DR testing: 4-8 hours/year
- **Total**: 28-56 hours/year (2-5 hours/month average)

**Total Ongoing Effort**: 13-23 hours/month (£650-1,150/month in labor)

#### SaaS ZTNA (Tailscale/Twingate)

**Day 1 (Initial Deployment):**
- Sign up and configure Azure AD SSO: 1 hour
- Deploy client agents (Intune or manual): 1-2 hours
- Configure ACL policies: 0.5-1 hour
- Testing: 0.5-1 hour
- **Total**: 3-5 hours

**Week 1-4 (User Onboarding):**
- User training (5 min per user × 30 users): 2.5 hours
- Troubleshooting (minimal): 1-2 hours
- **Total**: 3.5-4.5 hours

**Month 2+ (Ongoing):**
- User support (rare): 0.5-1 hour/month
- Policy adjustments: 0.5 hour/month
- Log review: 0.5 hour/month
- **Total**: 1.5-2.5 hours/month

**Annual Events:**
- Access policy review: 2-4 hours/year
- Vendor invoice review: 0.5 hour/year
- **Total**: 2.5-4.5 hours/year (0.2-0.4 hours/month average)

**Total Ongoing Effort**: 1.7-2.9 hours/month (£85-145/month in labor)

**Operational Complexity Verdict:** SaaS ZTNA is **88% less operational effort** than self-hosted SSH bastion.

---

## 6. Recommendation and Implementation Guidance

### 6.1 Bottom-Line Recommendation

**❌ DO NOT build a custom SSH bastion host.**

**✅ ADOPT a SaaS ZTNA solution: Tailscale, Twingate, or Cloudflare Zero Trust.**

### 6.2 Why NOT SSH Bastion?

1. **Operational Burden Unsustainable for Small IT Teams**:
   - 13-23 hours/month ongoing maintenance (£650-1,150/month labor)
   - Requires Linux security expertise (CIS hardening, PAM configuration, SSH debugging)
   - 24/7 monitoring and patching responsibility
   - Single point of failure (bastion host downtime = no remote access)

2. **Poor User Experience**:
   - Manual SSH tunnel setup for every database connection
   - Terminal window must remain open during entire session
   - Port conflicts and connection drops
   - High support burden (4-8 hours/month troubleshooting user issues)
   - Extensive training required (30+ minutes per user)

3. **Total Cost Exceeds Budget by 3-9x**:
   - Self-hosted: £377/user/year (38x over £120/user target)
   - Teleport: £253/user/year (2x over budget)
   - Guacamole: £177/user/year (47% over budget)
   - vs. SaaS ZTNA: £85-97/user/year (30% UNDER budget)

4. **Security Risks**:
   - Exposed SSH port 22 on perimeter firewall (attack surface)
   - PAM module complexity increases misconfiguration risk
   - Manual patching delays expose vulnerabilities
   - No continuous verification after initial authentication

5. **Scalability Limitations**:
   - Manual capacity planning and VM resizing
   - User onboarding requires individual SSH key management
   - No automatic load distribution

### 6.3 Recommended Solution: SaaS ZTNA (Tailscale or Cloudflare Zero Trust)

#### Option 1: Tailscale (Preferred for Database-Heavy Use)

**Why Tailscale:**
- ✅ **Best User Experience**: Direct peer-to-peer mesh connections (lowest latency)
- ✅ **Simplest Database Access**: Direct TCP connection to `100.x.x.x:5432` (no tunnels)
- ✅ **CCTV Friendly**: HTTPS and RTSP work transparently over mesh
- ✅ **Zero Inbound Ports**: All connections outbound-only
- ✅ **Granular ACLs**: Per-user, per-resource access control
- ✅ **Under Budget**: £180/month (£40% under £300 budget)
- ✅ **Azure AD MFA**: Native integration with Conditional Access

**Deployment Plan:**
1. Sign up for Tailscale Starter plan (30 users)
2. Configure Azure AD SSO (OAuth2)
3. Deploy Tailscale client via Intune to Windows 11 laptops
4. Install Tailscale on PostgreSQL server (Ubuntu 24.04)
5. Configure ACLs: Grant database access only to authorized users
6. Test connectivity: Users authenticate to Azure AD once, connect directly to database
7. Deploy subnet router for CCTV VLAN access

**Implementation Time**: 3-5 hours
**User Training**: 5-10 minutes per user
**Monthly Maintenance**: 1-2 hours

**Tailscale Pricing:**
- Starter: £6/user/month = £180/month for 30 users
- 3-year commitment discount available (10-20% off)

**Sources:**
- <a href="https://tailscale.com/pricing" target="_blank">Tailscale Pricing</a>
- Previous research: `/mnt/c/Users/SteveIrwin/terminai/it/ZeroTrust/Tailscale_Research_2026.md`

#### Option 2: Cloudflare Zero Trust (Alternative)

**Why Cloudflare:**
- ✅ **Enterprise Infrastructure**: Global edge network (low latency)
- ✅ **BastionZero Acquisition**: Native infrastructure access features
- ✅ **Azure AD SSO**: Native SAML/OIDC integration
- ✅ **Under Budget**: £210/month (£30% under budget)
- ✅ **Web-Based Access**: Optional browser-based access (no client required)

**Limitations:**
- ⚠️ More application-focused (less optimized for raw TCP than Tailscale)
- ⚠️ Requires Cloudflare Tunnel connector on database server side

**Deployment Plan:**
1. Sign up for Cloudflare Zero Trust
2. Configure Azure AD SSO
3. Install Cloudflare WARP client on user laptops (or browser-based access)
4. Install Cloudflare Tunnel connector on PostgreSQL server
5. Configure access policies for database and CCTV resources
6. Test connectivity

**Implementation Time**: 4-6 hours
**User Training**: 10-15 minutes per user
**Monthly Maintenance**: 1-2 hours

**Cloudflare Pricing:**
- Zero Trust: £7/user/month = £210/month for 30 users

**Sources:**
- <a href="https://www.cloudflare.com/plans/zero-trust-services/" target="_blank">Cloudflare Zero Trust Pricing</a>
- <a href="https://blog.cloudflare.com/cloudflare-acquires-bastionzero/" target="_blank">Cloudflare Acquires BastionZero</a>

### 6.4 Migration Plan from Current State

**Current State:** No remote database access (users on-premises only)

**Target State:** Secure remote access via Tailscale for 30 Azure AD users

**Migration Steps:**

**Phase 1: Planning and Procurement (Week 1)**
1. Sign up for Tailscale Starter plan
2. Configure Azure AD OAuth application for Tailscale SSO
3. Document Tailscale architecture and access policies
4. Prepare user communication and training materials

**Phase 2: Pilot Deployment (Week 2)**
1. Deploy Tailscale client to 3-5 pilot users (IT team)
2. Install Tailscale on PostgreSQL server (Ubuntu 24.04)
3. Configure initial ACL policies (pilot users only)
4. Test PostgreSQL connectivity from pilot users
5. Test CCTV access (if applicable to pilot users)
6. Gather feedback and refine policies

**Phase 3: Full Rollout (Week 3-4)**
1. Deploy Tailscale client via Intune to all 30 Windows 11 laptops
2. Update ACL policies for all authorized users
3. Conduct brief user training (5-10 min per user, group sessions)
4. Provide written quick-start guide
5. Monitor connections and troubleshoot issues

**Phase 4: Validation and Documentation (Week 5)**
1. Verify all 30 users can access database remotely
2. Validate Azure AD MFA enforcement
3. Review Tailscale audit logs
4. Document final configuration for future reference
5. Conduct access policy review

**Total Migration Time**: 5 weeks (part-time effort)
**Total Migration Cost**: £100-200 in IT labor + £180 first month subscription

### 6.5 GDPR Compliance Notes

**Tailscale GDPR Compliance:**
- ✅ **Data Processing Agreement (DPA)**: Available upon request
- ✅ **EU Representative**: Tailscale has EU representative for GDPR
- ✅ **Standard Contractual Clauses (SCCs)**: For US-EU data transfers
- ✅ **Data Residency**: Coordination plane data stored in US, but user data never transits Tailscale servers (peer-to-peer mesh)
- ✅ **Encryption**: End-to-end encryption via WireGuard (keys never leave devices)
- ✅ **User Consent**: Azure AD authentication provides consent mechanism

**Data Flows:**
- **Control Plane**: User authentication metadata sent to Tailscale coordination server (US)
- **Data Plane**: PostgreSQL queries flow peer-to-peer (UK laptop → UK database server)
- **No Database Data Exposure**: Database content never flows through Tailscale infrastructure

**Sources:**
- <a href="https://tailscale.com/privacy-policy" target="_blank">Tailscale Privacy Policy</a>
- <a href="https://tailscale.com/blog/how-tailscale-works" target="_blank">Tailscale Architecture: Data Separation</a>

---

## 7. Frequently Asked Questions

### Q1: Why not just use Azure Bastion?

**A:** Azure Bastion is designed for Azure-hosted VMs, not on-premises infrastructure. Your PostgreSQL server is on-premises behind a SonicWall firewall, so Azure Bastion is not applicable. Azure Bastion also costs ~£140/month minimum, exceeding your budget.

### Q2: Can I use Microsoft Entra ID SSH extension on-premises?

**A:** No. The `AADSSHLoginForLinux` VM extension is only supported on Azure-hosted Linux VMs. It requires Azure VNet integration and is not designed for on-premises servers.

### Q3: What if we already have a server we can use as an SSH bastion?

**A:** Even with free infrastructure, the operational cost is prohibitive:
- 13-23 hours/month maintenance (£650-1,150/month in IT labor)
- Poor user experience (manual SSH tunnels)
- Security risks (exposed SSH port, manual patching)
- Total Year 1 cost: £9,800+ (vs. £2,560 for Tailscale)

The labor cost alone exceeds SaaS ZTNA pricing by 3-5x.

### Q4: Can SaaS ZTNA solutions access resources on different VLANs?

**A:** Yes. Tailscale uses "subnet routers" that can advertise multiple subnets:
- Deploy one subnet router for database VLAN
- Deploy one subnet router for CCTV VLAN (or use same router)
- Users connect to Tailscale mesh, access both VLANs via ACL policies

No cross-VLAN routing required on your network infrastructure.

### Q5: What happens if Tailscale/Cloudflare goes down?

**A:** If the coordination server is down, **existing connections continue working** (data plane is independent). New connections cannot be established until service resumes. Typical SLA: 99.9% uptime (43 minutes downtime/month).

For comparison, a self-hosted bastion has no SLA and depends on your maintenance practices.

### Q6: How do we migrate users from Tailscale to another solution later?

**A:** Tailscale uses standard WireGuard protocol. Migration options:
1. Switch to Twingate or Cloudflare Zero Trust (similar deployment process)
2. Deploy self-hosted WireGuard (if you later have dedicated security resources)
3. Use traditional VPN as fallback

No vendor lock-in for user devices (standard OAuth/SAML authentication).

### Q7: Can Tailscale enforce device compliance (managed device only)?

**A:** Yes, with Tailscale's Device Posture feature:
- Intune-enrolled devices can be tagged as "managed"
- ACL policies require "managed" tag for database access
- Unmanaged devices (personal laptops) denied access

This is not available with self-hosted SSH bastion without extensive custom scripting.

### Q8: How do we audit who accessed the database?

**A:** Tailscale provides:
- Connection audit logs (who connected to what, when, from where)
- Export logs to SIEM (Splunk, ELK, Azure Sentinel)
- Network flow logs (source/destination IPs, timestamps)

For query-level auditing, enable PostgreSQL's built-in audit logging (`pgAudit` extension).

### Q9: What about session recording for compliance?

**A:** Tailscale does not provide session recording (database query logs). For compliance:
- Use PostgreSQL query logging (`log_statement = 'all'`)
- Deploy database audit tool (pgAudit extension, free and open source)
- Commercial solutions: Teleport (session recording, but £500+/month)

If session recording is mandatory, consider Teleport instead of Tailscale (at higher cost).

---

## 8. Conclusion and Next Steps

### 8.1 Final Verdict

**Custom SSH bastion hosts are NOT recommended for small IT teams.** The operational complexity, maintenance burden, and total cost of ownership make self-hosted SSH bastions a poor choice compared to modern SaaS ZTNA solutions.

**Adopt Tailscale or Cloudflare Zero Trust** for:
- 88% less operational effort
- 70% simpler user experience
- 70% lower total cost
- Better security posture (zero inbound ports, continuous verification)
- Native Azure AD integration with MFA

### 8.2 Recommended Action Plan

1. **Week 1**: Sign up for Tailscale Starter plan, configure Azure AD SSO
2. **Week 2**: Deploy pilot with 3-5 users, test database and CCTV access
3. **Week 3-4**: Full rollout to 30 users via Intune
4. **Week 5**: Validate, document, and conduct access policy review

**Total Implementation**: 5 weeks, £100-200 setup cost, £180/month ongoing

### 8.3 If You Absolutely Must Build SSH Bastion (Not Recommended)

If organizational policy requires on-premises SSH bastion despite cost and complexity:

**Use Apache Guacamole instead of custom PAM integration:**
- Browser-based access (no SSH tunnel training required)
- Azure AD SAML integration is well-documented
- Active open-source community
- Total cost: £177/user/year (vs. £377 for custom SSH+PAM)

**Do NOT attempt:**
- Custom PAM module integration (unreliable, complex, poorly documented)
- SSH certificate-based authentication with Azure AD (no tooling available)
- Self-managed Teleport Community Edition (license restrictions as of v16)

---

## 9. Additional Resources

### Official Documentation
- <a href="https://learn.microsoft.com/en-us/entra/identity/devices/howto-vm-sign-in-azure-ad-linux" target="_blank">Microsoft: Sign in to Linux VM using Entra ID</a>
- <a href="https://tailscale.com/kb/" target="_blank">Tailscale Knowledge Base</a>
- <a href="https://developers.cloudflare.com/cloudflare-one/" target="_blank">Cloudflare Zero Trust Documentation</a>
- <a href="https://www.postgresql.org/docs/current/ssh-tunnels.html" target="_blank">PostgreSQL: SSH Tunnels</a>

### Security Hardening Guides
- <a href="https://www.cisecurity.org/benchmark/ubuntu_linux" target="_blank">CIS Ubuntu Linux Benchmarks</a>
- <a href="https://www.sshaudit.com/hardening_guides.html" target="_blank">SSH Audit: Hardening Guides</a>
- <a href="https://ubuntu.com/blog/hardening-automation-for-cis-benchmarks-now-available-for-ubuntu-24-04-lts" target="_blank">Ubuntu: CIS Benchmark Hardening for 24.04 LTS</a>

### ZTNA vs Bastion Analysis
- <a href="https://hoop.dev/blog/ztna-vs-bastion-host-a-modern-solution-for-secure-network-access/" target="_blank">ZTNA vs. Bastion Host: Modern Solution Analysis</a>
- <a href="https://goteleport.com/blog/do-we-still-need-a-bastion/" target="_blank">Teleport: Do We Still Need a Bastion?</a>
- <a href="https://www.netskope.com/blog/leaving-bastion-hosts-behind-part-1-gcp" target="_blank">Netskope: Leaving Bastion Hosts Behind</a>

### Cost Analysis
- <a href="https://www.bastionzero.com/resources/to-self-host-or-not-to-self-host" target="_blank">BastionZero: Self-Hosted vs. SaaS Decision Framework</a>
- <a href="https://www.centreon.com/saas-it-monitoring-vs-self-hosted-can-tco-help-make-the-best-choice/" target="_blank">Centreon: SaaS vs. Self-Hosted TCO Analysis</a>

---

**Report Prepared By:** IT Security Research Agent
**Report Date:** February 17, 2026
**Last Updated:** February 17, 2026
**Version:** 1.0
