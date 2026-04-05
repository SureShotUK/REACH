# Azure Private Link + VPN Gateway Research
## Zero Trust Remote Database Access Evaluation

**Research Date**: 2026-02-17
**Status**: Initial Research - Budget Viability Assessment
**Recommendation**: **NOT VIABLE within £300/month budget** ❌

---

## Executive Summary

**Azure Private Link + VPN Gateway** was evaluated as a potential Zero Trust solution for remote PostgreSQL access. This solution would require **migrating the on-premises PostgreSQL database to Azure** (either Azure VM or Azure Database for PostgreSQL managed service) and providing VPN connectivity through Azure VPN Gateway.

### Critical Finding: Budget Impact

**Estimated Monthly Cost**: **£250-450+** (minimum realistic configuration)
**Budget Requirement**: £300/month maximum (£10/user for 30 users)
**Verdict**: **Exceeds budget even at minimum viable configuration**

### Key Issues Identified

1. **Database Migration Required**: On-premises PostgreSQL must move to Azure (significant complexity)
2. **High VPN Gateway Costs**: VpnGw1AZ starts at ~£110-140/month alone
3. **Managed Database Costs**: Azure Database for PostgreSQL adds £80-150+/month
4. **Data Egress Charges**: Query results transmitted to UK users incur egress fees
5. **CCTV System Complexity**: Would require separate hybrid connectivity solution or migration
6. **No Native Azure AD for On-Premises**: Azure AD authentication only works with Azure-hosted PostgreSQL

### Alternative Recommendation

**WireGuard on Azure VM** (£12-25/month) or **Tailscale** (£180-300/month for 30 users) provide significantly lower costs while maintaining Zero Trust principles without database migration.

---

## Detailed Research Findings

### 1. Architecture Analysis

#### Required Components

To implement Azure Private Link + VPN Gateway for remote PostgreSQL access, the following Azure infrastructure would be required:

**Core Infrastructure:**
1. **Azure Virtual Network (VNet)** - Virtual network to host resources
2. **Azure VPN Gateway** - VpnGw1AZ or higher (zone-redundant)
3. **Private Endpoint** - For Private Link connectivity
4. **PostgreSQL Hosting** - Choice of:
   - **Option A**: Azure Database for PostgreSQL Flexible Server (managed service)
   - **Option B**: Azure Virtual Machine running PostgreSQL (IaaS)

**Supporting Components:**
5. **Public IP Address** - For VPN Gateway ($3-5/month)
6. **Storage** - For database data, backups
7. **Network Security Groups (NSG)** - Traffic filtering
8. **Azure Bastion** (optional) - For VM management if using IaaS approach

#### Architecture Flow

```
[User Laptop (Windows 11)]
         |
         | Azure VPN Client (P2S VPN)
         | TLS 1.2+ encrypted tunnel
         |
    [VPN Gateway - VpnGw1AZ]
         |
    [Azure VNet]
         |
    [Private Endpoint]
         |
[Azure Database for PostgreSQL]
or
[Azure VM running PostgreSQL]
```

**Critical Limitation**: This architecture **requires database migration to Azure**. There is no viable way to use Private Link with on-premises PostgreSQL without Azure Arc (discussed in Section 11).

---

### 2. Cost Analysis (2026 UK Pricing)

#### Unable to Obtain Exact UK Pricing

**Research Limitation**: Microsoft Azure pricing pages display "$-" placeholders instead of actual UK GBP pricing when accessed via web scraping. Pricing must be verified through:
- Azure Pricing Calculator (interactive tool)
- Direct Azure account sign-in
- Azure sales representative

#### Estimated Costs (Based on USD Conversion and Industry Data)

**Note**: These are estimates based on available data. Actual UK GBP pricing may vary.

##### Scenario A: Azure Database for PostgreSQL Flexible Server (Managed)

| Component | Specification | Est. Monthly Cost (GBP) |
|-----------|--------------|------------------------|
| **PostgreSQL Flexible Server** | Burstable B2s (2 vCore, 4GB RAM) | £80-120 |
| **Storage** | 32GB Premium SSD | £5-8 |
| **Backup Storage** | 32GB automated backups | £3-5 |
| **VPN Gateway** | VpnGw1AZ (zone-redundant) | £110-140 |
| **Private Endpoint** | 1 endpoint × 730 hours × $0.01/hr | £7-10 |
| **Public IP** | Standard SKU for VPN Gateway | £3-5 |
| **Data Egress** | ~50GB/month to UK users | £4-10 |
| **SUBTOTAL (PostgreSQL Managed)** | | **£212-298/month** |

**Additional Costs Not Included**:
- One-time migration costs (database transfer, testing, validation)
- Azure AD App Registration (included in M365 Business Premium)
- Azure Monitor logging (if enabled) - £5-15/month
- Additional storage if database >32GB
- Point-in-time restore storage (if enabled)

##### Scenario B: Azure Virtual Machine for PostgreSQL (IaaS)

| Component | Specification | Est. Monthly Cost (GBP) |
|-----------|--------------|------------------------|
| **Azure VM** | Standard_D2s_v5 (2 vCore, 8GB RAM) | £70-90 |
| **Managed Disk** | 128GB Premium SSD (P10) | £15-20 |
| **VPN Gateway** | VpnGw1AZ (zone-redundant) | £110-140 |
| **Private Endpoint** | 1 endpoint × 730 hours × $0.01/hr | £7-10 |
| **Public IP** | Standard SKU for VPN Gateway | £3-5 |
| **Data Egress** | ~50GB/month to UK users | £4-10 |
| **Backup Storage** (optional) | Azure Backup or manual snapshots | £10-20 |
| **SUBTOTAL (VM-hosted)** | | **£219-295/month** |

**Additional Maintenance Burden**:
- Manual PostgreSQL updates and patching
- OS security updates (Ubuntu 24.04)
- Backup management
- High availability configuration (if needed)
- Monitoring and alerting setup

##### Cost Comparison: Budget Reality Check

| Cost Item | Amount (GBP) | % of Budget |
|-----------|--------------|-------------|
| **Minimum Azure Solution** | £212-298/month | **71-99% of budget** |
| **Total Budget Available** | £300/month | 100% |
| **Remaining for Other IT Costs** | £2-88/month | 1-29% |

**Verdict**: Even the most cost-optimized Azure configuration consumes **71-99% of the total IT budget**, leaving minimal room for:
- CCTV system connectivity (separate requirement)
- Future scaling
- Incident response
- Additional security tools
- Disaster recovery

---

### 3. Migration Complexity Assessment

#### Database Migration Requirements

Moving from on-premises PostgreSQL 16.11 to Azure requires:

**Pre-Migration Planning**:
1. **Database Assessment**:
   - Current database size (GB)
   - Number of tables, indexes, stored procedures
   - Extension dependencies (PostGIS, pg_cron, etc.)
   - Custom functions and triggers
   - Application connection strings

2. **Compatibility Validation**:
   - Azure Database for PostgreSQL supports PostgreSQL 11, 12, 13, 14, 15, 16
   - Extension availability check (some extensions not supported in managed service)
   - Performance tier sizing (CPU, RAM, IOPS)

3. **Downtime Planning**:
   - **Minimal downtime migration**: Use logical replication (requires setup, testing)
   - **Standard migration**: Dump/restore (requires full downtime window)
   - **Acceptable downtime**: Must be defined by business (hours? days? weekend?)

**Migration Methods**:

| Method | Downtime | Complexity | Risk |
|--------|----------|------------|------|
| **pg_dump + pg_restore** | 2-8 hours (depends on size) | Low | Low |
| **Azure Database Migration Service** | <1 hour (with replication) | Medium | Medium |
| **Logical Replication** | <15 minutes (cutover only) | High | Medium |
| **Physical backup restore** | Not supported in managed service | N/A | N/A |

**Post-Migration Tasks**:
- Application connection string updates (all apps accessing database)
- Performance tuning (different hardware characteristics)
- Backup validation
- Monitoring setup
- Disaster recovery testing

#### One-Time Migration Costs

| Task | Estimated Effort | Cost (if consultant required) |
|------|-----------------|------------------------------|
| Database assessment and planning | 8-16 hours | £800-1600 |
| Pre-migration testing | 4-8 hours | £400-800 |
| Migration execution | 4-8 hours | £400-800 |
| Post-migration validation | 4-8 hours | £400-800 |
| Application updates and testing | 8-16 hours | £800-1600 |
| **TOTAL** | **28-56 hours** | **£2,800-5,600** |

**Risk Assessment**:
- **Data Loss Risk**: Medium (mitigated by backups and validation)
- **Extended Downtime Risk**: Medium (depends on migration method)
- **Application Compatibility Risk**: Low-Medium (connection string changes required)
- **Rollback Complexity**: High (requires on-premises backup restoration)

---

### 4. Azure AD Integration (Azure-Hosted Only)

#### Native Azure AD Authentication Support

**Critical Finding**: Azure AD (Microsoft Entra ID) authentication for PostgreSQL is **ONLY supported for Azure Database for PostgreSQL** (cloud-hosted). There is **NO native Azure AD authentication for on-premises PostgreSQL**.

#### How Azure AD Authentication Works (Azure-Hosted)

Based on <a href="https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/concepts-azure-ad-authentication" target="_blank">Microsoft documentation</a>:

1. **Token-Based Authentication**:
   - User authenticates to Azure AD and receives OAuth 2.0 access token
   - Token is a Base64-encoded string containing user identity and claims
   - Token is used as the PostgreSQL password when connecting

2. **Configuration Options**:
   - **PostgreSQL and Microsoft Entra authentication** (dual mode)
   - **Microsoft Entra authentication only** (password-based auth disabled)

3. **Principal Types Supported**:
   - Azure AD users (individual accounts)
   - Azure AD groups (role-based access)
   - Service principals (application identities)
   - Managed identities (Azure resource identities)

4. **Administrator Management**:
   - Multiple Azure AD principals can be configured as administrators
   - Only Azure AD administrators can initially connect and create additional users
   - Active Directory administrator can configure subsequent database users

#### Integration with Conditional Access

With Azure AD authentication, you can enforce:
- **Multi-Factor Authentication (MFA)** - Require MFA for database access
- **Device Compliance** - Require Intune-managed, compliant devices
- **Trusted Locations** - Require connections from specific IP ranges or countries
- **Session Controls** - Limit session duration, require re-authentication
- **Risk-Based Access** - Block high-risk sign-ins automatically

**Example Conditional Access Policy**:
```
IF User = Database_Access_Group
AND Application = Azure Database for PostgreSQL
THEN REQUIRE:
  - Multi-Factor Authentication
  - Device Compliance (Intune)
  - Trusted Location (UK, Canada, or corporate VPN)
```

#### On-Premises PostgreSQL Limitation

As confirmed by <a href="https://www.postgresql.org/message-id/005301da5d3f$b9543f80$2bfcbe80$@gmail.com" target="_blank">PostgreSQL community discussion</a>:

> "There is currently no native way to do Azure AD/Entra ID authentication with on-premises PostgreSQL server databases. This remains a feature request."

**Alternative for On-Premises**:
- **Kerberos/GSSAPI authentication** with Windows Active Directory (on-premises AD)
- **LDAP authentication** with Azure AD Connect (hybrid identity sync)
- **Certificate-based authentication** (client certificates)

**Conclusion**: To use native Azure AD authentication with PostgreSQL, **database must be hosted in Azure**.

---

### 5. VPN Client Deployment (Windows 11)

#### Azure VPN Client Options

**Option 1: Azure VPN Client (Recommended for Azure AD Auth)**

The **Azure VPN Client** (<a href="https://learn.microsoft.com/en-us/azure/vpn-gateway/point-to-site-vpn-client-cert-windows" target="_blank">Microsoft docs</a>) supports:
- **Azure AD authentication** (seamless integration)
- **Certificate-based authentication** (PKI)
- **RADIUS authentication** (MFA via third-party)

**Deployment via Intune**:
1. Download Azure VPN Client from Microsoft Store or direct installer
2. Create Intune application deployment policy
3. Deploy VPN profile XML to devices
4. Users authenticate with Azure AD credentials on first connect

**User Experience**:
- Install Azure VPN Client (one-time)
- Click "Connect" when database access needed
- Authenticate with Azure AD (MFA enforced if configured)
- Client maintains connection until manually disconnected

**Option 2: Native Windows VPN Client (Legacy)**

The built-in Windows 11 VPN client supports:
- **IKEv2** protocol
- **Certificate-based authentication** only (no Azure AD)

**Limitation**: Native Windows VPN client does **NOT** support Azure AD authentication for Point-to-Site VPN. You must use the Azure VPN Client for Azure AD integration.

#### VPN Client Configuration Challenges

**Intune Deployment Considerations**:
- VPN profile must include VPN Gateway public IP and certificate information
- Azure VPN Client configuration file (`.azureauth`) must be distributed
- Users may need local admin rights to install VPN client (depending on organization policy)
- ThreatLocker policies must allow Azure VPN Client execution

**User Training Requirements**:
- When to connect VPN (only when database access needed)
- How to authenticate (Azure AD credentials + MFA)
- Troubleshooting connectivity issues
- What to do if VPN connection fails

**Compared to ZTNA Solutions**:
- **Traditional VPN (Azure VPN Gateway)**: User manually connects VPN → Accesses database
- **ZTNA (Cloudflare, Twingate, Tailscale)**: User accesses database → Solution handles auth/tunneling transparently

**Verdict**: Azure VPN Gateway is **more complex for end users** than transparent ZTNA solutions.

---

### 6. Zero Trust Alignment Assessment

#### Does Azure Private Link + VPN Gateway Achieve Zero Trust?

**Zero Trust Principles Scorecard**:

| Principle | Azure Private Link + VPN | Assessment |
|-----------|-------------------------|------------|
| **Verify Explicitly** | ✅ Partial | Azure AD auth + MFA supported, but VPN grants network access (not resource-specific) |
| **Least Privilege Access** | ⚠️ Limited | VPN grants access to entire Azure VNet, not just database. NSG rules can restrict, but not granular. |
| **Assume Breach** | ✅ Yes | Private Link isolates traffic, encryption in transit, audit logging available |

**Traditional VPN vs. True ZTNA**:

| Characteristic | Azure VPN Gateway (P2S) | True ZTNA (Twingate, Cloudflare Access) |
|----------------|------------------------|----------------------------------------|
| **Access Model** | Network access (Layer 3) | Application access (Layer 7) |
| **Default Access** | All resources in VNet (unless NSG blocks) | No access (explicit grant required) |
| **User Experience** | Manual VPN connection | Transparent (no VPN client) |
| **Granularity** | Subnet/IP-based | Resource/application-based |
| **Attack Surface** | VPN endpoint exposed (albeit encrypted) | No exposed endpoints |

**Verdict**: Azure VPN Gateway provides **VPN-based security** with Azure AD integration, but it is **NOT pure Zero Trust Network Access (ZTNA)**. It is a **hybrid approach**—better than legacy VPN, but not as granular as true ZTNA solutions like Twingate or Cloudflare Access.

**Why Azure VPN Gateway Falls Short of ZTNA**:
1. **Network-Centric**: Grants access to Azure VNet, not specific PostgreSQL database
2. **Manual Connection**: User must manually connect VPN (not transparent)
3. **Broad Access**: Once connected, user can reach all VNet resources (unless NSG restricts)
4. **VPN Endpoint**: Exposed VPN Gateway (though encrypted and authenticated)

**How to Improve Zero Trust Alignment**:
- Deploy **Network Security Groups (NSG)** to restrict VPN users to PostgreSQL port 5432 only
- Enable **Azure AD Conditional Access** with MFA, device compliance, location restrictions
- Implement **Just-In-Time (JIT) access** with Azure AD Privileged Identity Management
- Use **Azure Monitor** for audit logging and anomaly detection

---

### 7. Performance Analysis

#### Latency Considerations

**Expected Latency for UK Users**:

| User Location | VPN Gateway Region | Expected Latency |
|---------------|-------------------|------------------|
| **UK (London)** | UK South | 10-30ms (VPN overhead) + 1-5ms (Private Link) = **15-35ms** |
| **Europe (Paris)** | UK South | 20-50ms (VPN overhead) + 1-5ms (Private Link) = **25-55ms** |
| **Canada (Toronto)** | UK South | 80-120ms (VPN overhead) + 1-5ms (Private Link) = **85-125ms** |

**Latency Breakdown**:
1. **User to VPN Gateway**: Dependent on geographic distance and ISP routing
2. **VPN Encryption/Decryption**: 5-15ms overhead
3. **Private Link**: <5ms within Azure region
4. **PostgreSQL Query Processing**: Depends on query complexity

**Performance Factors**:
- **VPN Gateway SKU**: VpnGw1AZ supports up to 650 Mbps, VpnGw2AZ supports up to 1 Gbps
- **User Internet Connection**: Home broadband (50-500 Mbps typical)
- **Azure Region**: UK South provides lowest latency for UK users
- **Database Performance**: B2s tier (Burstable) has limited IOPS (640 IOPS max)

**Real-World Performance Expectations**:
- **Interactive Queries**: 50-200ms total (query + network latency)
- **Large Result Sets**: Limited by VPN bandwidth and user internet speed
- **Concurrent Users**: VpnGw1AZ supports 250 P2S connections (adequate for 30 users)

**Comparison to Alternatives**:
- **Direct Internet Connection (no VPN)**: 5-15ms to Azure (fastest, but no security)
- **WireGuard VPN**: 10-25ms overhead (lower than IPsec-based Azure VPN Gateway)
- **Cloudflare Tunnel**: 15-40ms overhead (depends on Cloudflare edge location)
- **Tailscale**: 10-30ms overhead (direct peer-to-peer where possible)

**Verdict**: Performance is **acceptable for interactive database access** from UK/Europe, but **Canada users may experience noticeable latency** (100ms+).

---

### 8. Security Features

#### Azure Security Capabilities

**Network Security**:
1. **Private Link Isolation**:
   - Database is NOT exposed to public internet
   - Traffic flows through Azure backbone (not public internet)
   - Private IP addressing within VNet

2. **Network Security Groups (NSG)**:
   - Inbound/outbound traffic filtering
   - Restrict VPN users to PostgreSQL port 5432 only
   - Deny all other traffic by default

3. **VPN Gateway Security**:
   - IKEv2 or OpenVPN protocols (TLS 1.2+)
   - Azure AD authentication (OAuth 2.0)
   - Certificate-based authentication (PKI)
   - Perfect Forward Secrecy (PFS)

**Data Encryption**:
1. **In-Transit Encryption**:
   - VPN tunnel: IKEv2 with AES-256 or OpenVPN with TLS 1.2+
   - Private Link: TLS 1.2+ to PostgreSQL
   - PostgreSQL: SSL/TLS enforced connections

2. **At-Rest Encryption**:
   - Azure Database for PostgreSQL: Transparent Data Encryption (TDE) enabled by default
   - Azure VM disks: Azure Disk Encryption (ADE) with BitLocker (Windows) or dm-crypt (Linux)
   - Backup encryption: Encrypted with Microsoft-managed keys or customer-managed keys

**Identity and Access Management**:
1. **Azure AD Integration**:
   - Multi-Factor Authentication (MFA)
   - Conditional Access policies (device compliance, location, risk-based)
   - Privileged Identity Management (PIM) for JIT admin access
   - Azure AD logs (sign-ins, audit logs)

2. **Database-Level Security**:
   - Row-Level Security (RLS) in PostgreSQL
   - Column-level encryption (application-managed)
   - Audit logging (Azure Database for PostgreSQL supports pgAudit)

**Monitoring and Logging**:
1. **Azure Monitor**:
   - VPN Gateway metrics (connections, bandwidth, packet drops)
   - PostgreSQL metrics (CPU, memory, IOPS, connections)
   - Alert rules (connection failures, high CPU, failed logins)

2. **Azure Log Analytics**:
   - VPN diagnostic logs (authentication, connection events)
   - PostgreSQL audit logs (queries, schema changes, failed logins)
   - Retention: 30-730 days (configurable, affects cost)

3. **Microsoft Sentinel** (SIEM):
   - Optional: Ingest logs for threat detection
   - Correlate VPN and database events
   - Automated incident response
   - Additional cost: ~£1-3/GB ingested

**Compliance Certifications**:
- Azure is certified for: ISO 27001, SOC 2 Type II, PCI DSS, GDPR, HIPAA, UK G-Cloud

---

### 9. CCTV System Access Challenge

#### Hybrid Connectivity Complexity

The requirement to access both PostgreSQL database and CCTV system creates significant architectural complexity.

**Problem Statement**:
- **PostgreSQL**: Would be migrated to Azure
- **CCTV System**: Remains on-premises (different VLAN)
- **Current Access**: Local network only

**Architecture Options**:

##### Option 1: Separate Solutions (Hybrid Approach)

- **Database Access**: Azure Private Link + VPN Gateway (as described)
- **CCTV Access**: Different solution (Cloudflare Tunnel, Tailscale, or hardware VPN)

**Pros**:
- Separation of concerns
- Best tool for each job

**Cons**:
- **Two separate systems to manage**
- **Two separate VPN clients** (confusing for users)
- **Two separate authentication flows**
- **Higher total cost**

##### Option 2: Site-to-Site VPN to Azure

Establish **Site-to-Site (S2S) VPN** between on-premises network and Azure VNet:

```
[On-Premises Network]
   |
   |- PostgreSQL (migrated to Azure)
   |- CCTV System (on-premises)
   |
[SonicWall TZ 270]
   |
   | S2S VPN Tunnel
   |
[Azure VPN Gateway]
   |
[Azure VNet]
   |
   |- Azure Database for PostgreSQL
   |- Jump Box VM (for CCTV access)
```

**How it works**:
1. User connects to Azure via Point-to-Site (P2S) VPN
2. Azure VNet has Site-to-Site (S2S) VPN to on-premises network
3. User can access both Azure PostgreSQL and on-premises CCTV

**Cons**:
- **Requires opening VPN port on SonicWall** (defeats zero-trust goal of "no inbound ports")
- **S2S VPN adds complexity** and cost
- **CCTV traffic traverses VPN twice** (user → Azure → on-premises)
- **Increased latency** for CCTV access

##### Option 3: Keep PostgreSQL On-Premises, Use ZTNA for Both

**Alternative Recommendation**:
- Keep PostgreSQL on-premises (Ubuntu 24.04 server)
- Deploy **single ZTNA solution** (Tailscale, Cloudflare Tunnel, Twingate) for both database and CCTV

**Pros**:
- **No database migration** required
- **Single solution** for both systems
- **Unified user experience** (one authentication, one client/agent)
- **Lower cost** (no Azure hosting fees)
- **No inbound firewall ports** (true zero-trust)

**Cons**:
- No native Azure AD authentication for on-premises PostgreSQL
- Must use alternative auth methods (Kerberos, LDAP, certificates)

**Verdict**: Keeping PostgreSQL on-premises with a ZTNA solution is **significantly simpler and cheaper** than hybrid Azure + on-premises architecture.

---

### 10. Azure Bastion vs VPN Gateway Comparison

#### Use Case Differences

| Feature | Azure Bastion | Azure VPN Gateway (Point-to-Site) |
|---------|--------------|----------------------------------|
| **Primary Use Case** | RDP/SSH to Azure VMs via browser | Remote access to entire Azure VNet |
| **Client Required** | None (browser-based) | Yes (Azure VPN Client or native VPN) |
| **Protocols Supported** | RDP (3389), SSH (22) | IKEv2, OpenVPN, SSTP |
| **Access Scope** | Individual VMs only | All resources in VNet (IP-based) |
| **PostgreSQL Access** | ❌ No (only RDP/SSH) | ✅ Yes (via VPN tunnel) |
| **Azure AD Integration** | ✅ Yes (RBAC for VM access) | ✅ Yes (P2S VPN authentication) |
| **Pricing** | ~£110/month (Basic) | ~£110-140/month (VpnGw1AZ) |

#### When to Use Each

**Use Azure Bastion When**:
- Need secure RDP/SSH access to Azure VMs
- Want browser-based access (no client installation)
- Managing Azure infrastructure (not application access)
- Single-VM jump box scenario

**Use Azure VPN Gateway When**:
- Need access to multiple resources in Azure VNet
- Application access (databases, web apps, file shares)
- Client-based connectivity required
- Support for standard database tools (pgAdmin, DBeaver)

**For This Project**:
- **Azure Bastion**: ❌ Not suitable (PostgreSQL access requires direct TCP connection, not RDP/SSH)
- **VPN Gateway**: ✅ Suitable (but expensive and requires database migration)

**Hybrid Approach**:
- Deploy VPN Gateway for database access (P2S VPN)
- Optionally add Azure Bastion for VM management (if using IaaS approach)
- Total cost: £220-280/month (both services)

**Verdict**: Azure Bastion is **NOT a replacement** for VPN Gateway in this scenario. It serves a different purpose (VM management, not application access).

---

### 11. Azure Arc Alternative (On-Premises Database)

#### Azure Arc Enabled PostgreSQL

**What is Azure Arc?**

<a href="https://learn.microsoft.com/en-us/azure/azure-arc/data/what-is-azure-arc-enabled-postgresql" target="_blank">Azure Arc</a> extends Azure management capabilities to on-premises and multi-cloud environments. It allows you to:
- Manage on-premises servers as Azure resources
- Use Azure services (Azure Policy, Azure Monitor) on-premises
- Hybrid connectivity via Private Link

**Azure Arc for PostgreSQL**:
- Deploy PostgreSQL on-premises or in other clouds
- Manage via Azure portal (unified management)
- Hybrid connectivity options

#### Azure Arc + Private Link Connectivity

According to <a href="https://learn.microsoft.com/en-us/azure/azure-arc/servers/private-link-security" target="_blank">Microsoft documentation</a>:

> "You can connect your on-premises or multicloud servers with Azure Arc and send all traffic over Azure ExpressRoute or a site-to-site virtual private network (VPN) connection instead of using public networks."

**Architecture**:

```
[On-Premises PostgreSQL Server]
   |
   | Azure Arc Agent (outbound HTTPS)
   |
[Azure Arc Private Link Scope]
   |
[Azure Private Endpoint]
   |
[Azure VNet]
   |
[VPN Gateway - Point-to-Site]
   |
[User Laptop (Windows 11)]
```

**How It Works**:
1. **Azure Arc Agent** installed on on-premises PostgreSQL server (Ubuntu 24.04)
2. Agent establishes **outbound HTTPS connection** to Azure Arc service (no inbound ports)
3. Azure Arc Private Link Scope creates **Private Endpoint** in Azure VNet
4. Users connect via **P2S VPN** to Azure VNet
5. Traffic flows from user → VPN Gateway → Private Endpoint → Arc Agent → PostgreSQL

#### Complexity Assessment

**Advantages**:
- PostgreSQL remains on-premises (no migration)
- No inbound firewall ports required (Arc agent initiates outbound)
- Unified Azure management (Azure portal, RBAC, policies)

**Disadvantages**:
1. **Extreme Complexity**:
   - Azure Arc agent installation and configuration
   - Private Link Scope setup
   - VPN Gateway configuration
   - Certificate management
   - Hybrid identity configuration

2. **Limited Documentation**:
   - Azure Arc for PostgreSQL is **primarily designed for Arc-enabled data services** (managed PostgreSQL deployed by Arc)
   - Using Arc **just for connectivity** to existing on-premises PostgreSQL is **not a common pattern**
   - Lack of step-by-step guides for this specific scenario

3. **Cost**:
   - **VPN Gateway**: £110-140/month (same as full Azure migration)
   - **Private Endpoint**: £7-10/month
   - **Azure Arc**: Free for on-premises servers (management plane only)
   - **Data Egress**: Query results from Azure to users (£4-10/month)
   - **Total**: £121-160/month (less than full migration, but still high)

4. **Azure AD Authentication Still Not Supported**:
   - Azure Arc does NOT enable Azure AD authentication for on-premises PostgreSQL
   - You would still need Kerberos/LDAP/certificate auth

5. **Latency**:
   - User → VPN Gateway → Private Endpoint → Arc Agent → PostgreSQL
   - Additional hops increase latency vs. direct connection

**Verdict**: Azure Arc + Private Link is **theoretically possible** but **extremely complex**, **poorly documented**, and **still expensive** (£121-160/month). It provides **minimal benefit** over simpler ZTNA solutions while adding significant operational complexity.

**Not Recommended**: The complexity and cost do not justify the benefits. Simpler ZTNA solutions (Tailscale, Cloudflare Tunnel, WireGuard) provide better cost/complexity tradeoffs.

---

### 12. Final Verdict: Budget Viability

#### Cost Summary Table

| Solution Approach | Monthly Cost (GBP) | Budget Fit | Migration Required | Complexity |
|-------------------|-------------------|------------|-------------------|------------|
| **Azure Database for PostgreSQL + VPN Gateway** | £212-298 | ❌ 71-99% of budget | ✅ Yes (high effort) | High |
| **Azure VM PostgreSQL + VPN Gateway** | £219-295 | ❌ 73-98% of budget | ✅ Yes (high effort) | Very High |
| **Azure Arc + Private Link + VPN Gateway** | £121-160 | ❌ 40-53% of budget | ❌ No | Extreme |
| **WireGuard on Azure VM** | £12-25 | ✅ 4-8% of budget | ❌ No | Low |
| **Tailscale (30 users, Starter plan)** | £180 | ✅ 60% of budget | ❌ No | Very Low |
| **Cloudflare Tunnel (Free tier)** | £0 | ✅ 0% of budget | ❌ No | Low |

#### Budget Reality Check

**Total IT Budget**: £300/month (£10/user × 30 users)

**Azure Private Link + VPN Gateway Consumption**:
- **Minimum Configuration**: £212/month (71% of budget)
- **Realistic Configuration**: £250-300/month (83-100% of budget)
- **With CCTV Hybrid Connectivity**: £300-400+/month (**exceeds budget**)

**Remaining Budget for Other IT Needs**: £0-88/month

**Other IT Costs Not Included**:
- Microsoft 365 Business Premium licenses (~£16/user/month = £480/month total)
- SonicWall TZ 270 licensing (if applicable)
- ThreatLocker licensing
- Huntress EDR licensing
- Backup solutions
- Other Azure services (if any)

**Conclusion**: Azure Private Link + VPN Gateway is **NOT financially viable** within the stated budget of £300/month for IT infrastructure.

---

### 13. Alternative Recommendations

Given the budget constraints and requirements, the following solutions are recommended for further evaluation:

#### Option 1: WireGuard VPN on Azure VM (Lowest Cost)

**Architecture**:
- Deploy Standard_B1ms Azure VM (£12/month) in UK South
- Install WireGuard VPN server (Ubuntu 24.04)
- Configure peer-to-peer VPN for each user
- PostgreSQL remains on-premises

**Pros**:
- **Extremely low cost**: £12-25/month (4-8% of budget)
- **No database migration** required
- **No inbound firewall ports** (WireGuard initiates outbound from on-premises)
- **Modern cryptography** (Noise protocol framework)
- **Simple configuration** (WireGuard config files)

**Cons**:
- **No native Azure AD authentication** (must use WireGuard keys or third-party auth)
- **Manual user management** (add/remove WireGuard peers)
- **Self-managed** (VM patching, monitoring, backups)

**References**:
- <a href="https://moriyama.co.uk/about-us/news/blog-switching-to-wireguard-and-reducing-costs-in-azure/" target="_blank">Switching to WireGuard and reducing costs in Azure</a>
- <a href="https://github.com/vijayshinva/AzureWireGuard" target="_blank">Azure WireGuard Bicep Template</a>

#### Option 2: Tailscale (Best User Experience)

**Architecture**:
- Install Tailscale client on Windows 11 laptops
- Install Tailscale on on-premises PostgreSQL server (Ubuntu 24.04)
- Configure ACLs for database access only

**Pros**:
- **Transparent connectivity** (no manual VPN connection)
- **Native Azure AD integration** (Tailscale supports OIDC/SAML)
- **Zero-config networking** (mesh VPN, NAT traversal)
- **Granular ACLs** (restrict users to PostgreSQL port 5432 only)
- **Easy deployment via Intune**

**Cons**:
- **Cost**: £6/user/month (Starter) = £180/month (60% of budget)
- **Third-party dependency** (Tailscale SaaS service)
- **Data flows through Tailscale coordination servers** (though E2E encrypted)

**References**:
- <a href="https://tailscale.com/pricing" target="_blank">Tailscale Pricing</a>
- <a href="https://tailscale.com/use-cases/infrastructure-access" target="_blank">Tailscale Infrastructure Access</a>

#### Option 3: Cloudflare Tunnel (Zero Trust Access)

**Architecture**:
- Install `cloudflared` daemon on on-premises PostgreSQL server
- Create Cloudflare Tunnel (outbound HTTPS to Cloudflare edge)
- Configure Cloudflare Access policies (Azure AD SSO)
- Users access via web browser or Cloudflare WARP client

**Pros**:
- **Free tier available** (50 users free)
- **Native Azure AD integration** (SAML/OIDC SSO)
- **No inbound firewall ports** (cloudflared initiates outbound)
- **True Zero Trust** (policy-based access control)
- **Cloudflare global network** (low latency)

**Cons**:
- **Database access may require WARP client** (for TCP connections)
- **Third-party dependency** (Cloudflare SaaS service)
- **Limited free tier features** (advanced policies require paid plan)

**References**:
- <a href="https://www.cloudflare.com/plans/zero-trust-services/" target="_blank">Cloudflare Zero Trust Pricing</a>
- <a href="https://www.cloudflare.com/products/zero-trust/access/" target="_blank">Cloudflare Access</a>

#### Option 4: Twingate (Enterprise ZTNA)

**Architecture**:
- Deploy Twingate Connector on on-premises network (VM or Docker)
- Install Twingate client on Windows 11 laptops
- Configure Twingate Resources (PostgreSQL access only)
- Integrate with Azure AD (SAML/OIDC)

**Pros**:
- **Purpose-built ZTNA** (application-level access)
- **Native Azure AD integration**
- **Granular access policies** (user, device, resource, MFA)
- **Split tunneling** (only database traffic goes through Twingate)
- **Device posture checks** (integration with Intune, CrowdStrike, etc.)

**Cons**:
- **Cost**: Pricing not publicly disclosed (likely £5-15/user/month = £150-450/month)
- **Third-party SaaS** dependency

**References**:
- Contact Twingate for pricing quote

---

### 14. Comparison to Zero Trust Project Candidates

#### How Azure Private Link + VPN Gateway Ranks

From the ZeroTrust project `README.md`, the original solution candidates were:

| Solution | Complexity | Cost | Zero Trust Alignment | **Budget Fit** |
|----------|-----------|------|---------------------|----------------|
| Azure AD App Proxy | Medium | Low (included in M365) | High | ✅ Yes |
| Cloudflare Tunnel | Low | Free/Paid tiers | High | ✅ Yes |
| Tailscale | Low | Paid for business | Medium | ⚠️ 60% of budget |
| Twingate | Medium | Paid | Very High | ⚠️ Unknown |
| SSH Bastion + Azure AD | High | Low (self-hosted) | Medium | ✅ Yes |
| **Azure Private Link** | **High** | **High** | **High** | **❌ 71-99% of budget** |

**Updated Assessment**:

| Solution | Monthly Cost | Budget % | Migration Required | Azure AD Native | Verdict |
|----------|--------------|----------|-------------------|----------------|---------|
| **Azure Private Link + VPN Gateway** | **£212-298** | **71-99%** | **✅ Yes** | **✅ Yes** | **❌ Not Viable** |
| **WireGuard on Azure VM** | **£12-25** | **4-8%** | **❌ No** | **❌ No** | **✅ Recommended** |
| **Tailscale** | **£180** | **60%** | **❌ No** | **✅ Yes (OIDC)** | **✅ Recommended** |
| **Cloudflare Tunnel** | **£0-84** | **0-28%** | **❌ No** | **✅ Yes (SAML)** | **✅ Recommended** |

---

## Conclusion

### Is Azure Private Link + VPN Gateway Viable?

**NO**—Azure Private Link + VPN Gateway is **NOT financially viable** within the £300/month budget constraint.

### Key Findings

1. **Cost Exceeds Budget**: Minimum configuration consumes **71-99% of total IT budget** (£212-298/month)
2. **Migration Required**: On-premises PostgreSQL must migrate to Azure (high complexity, downtime, one-time costs)
3. **CCTV System Complexity**: Requires separate solution or complex Site-to-Site VPN (adds cost)
4. **Not True ZTNA**: VPN-based approach grants network access (Layer 3), not application-specific access (Layer 7)
5. **Azure AD Auth Requires Cloud**: Native Azure AD authentication only works for Azure-hosted PostgreSQL

### What Azure Private Link + VPN Gateway DOES Provide

✅ **Enterprise-grade security** (Private Link isolation, Azure AD MFA, Conditional Access)
✅ **Managed database service** option (Azure Database for PostgreSQL with automated backups, HA)
✅ **Azure integration** (unified management, monitoring, compliance)
✅ **Scalability** (can scale to hundreds of users if needed)

### What It DOES NOT Provide

❌ **Budget fit** (£212-298/month exceeds £300 total budget)
❌ **On-premises database support** (requires migration to Azure)
❌ **True ZTNA** (VPN grants network access, not application-specific)
❌ **Simple CCTV integration** (requires separate solution or complex S2S VPN)
❌ **Cost-effective scaling** (costs increase with usage, storage, bandwidth)

---

## Recommended Next Steps

### 1. Evaluate Lower-Cost Alternatives

**Priority 1: WireGuard on Azure VM**
- **Cost**: £12-25/month (4-8% of budget)
- **Action**: Deploy proof-of-concept WireGuard server in Azure UK South
- **Testing**: Validate connectivity, performance, ThreatLocker compatibility
- **Timeline**: 1-2 weeks for POC

**Priority 2: Tailscale**
- **Cost**: £180/month (60% of budget)
- **Action**: Sign up for Tailscale free trial (30 days)
- **Testing**: Test Azure AD OIDC integration, Intune deployment, ACL policies
- **Timeline**: 1 week for POC

**Priority 3: Cloudflare Tunnel**
- **Cost**: £0 (free tier for 50 users)
- **Action**: Deploy `cloudflared` on PostgreSQL server
- **Testing**: Test WARP client for TCP database access, Azure AD SAML integration
- **Timeline**: 1 week for POC

### 2. Document Environment (Phase 1 Completion)

Before proceeding with any solution, complete **Phase 1: Environment Discovery**:

- [ ] PostgreSQL version, database size, current configuration
- [ ] Database access patterns (users, frequency, query types)
- [ ] SonicWall TZ 270 current ruleset
- [ ] Azure AD tenant configuration (MFA status, Conditional Access policies)
- [ ] Intune enrollment status for laptops
- [ ] ThreatLocker policies affecting database access
- [ ] CCTV system details (manufacturer, access method, bandwidth requirements)

### 3. Reassess Budget Allocation

**Current Budget**: £300/month for "IT infrastructure" (assumed)

**Question for User**: Does the £300/month budget include:
- Microsoft 365 Business Premium licenses (~£480/month for 30 users)?
- ThreatLocker licensing?
- Huntress EDR licensing?
- Other security tools?

**If £300/month is ONLY for remote access solution**:
- Azure Private Link + VPN Gateway becomes viable (71-99% of £300)
- Tailscale is comfortable (60% of £300)
- WireGuard is extremely cost-effective (4-8% of £300)

**If £300/month is TOTAL IT budget**:
- Only WireGuard or Cloudflare Tunnel are viable
- Azure Private Link + VPN Gateway is not feasible

### 4. Consider Hybrid Approach

**Scenario**: If database migration to Azure is desired for OTHER reasons (managed service, HA, disaster recovery), consider:

1. **Migrate PostgreSQL to Azure Database for PostgreSQL Flexible Server** (£80-120/month)
2. **Use WireGuard on B1ms VM for VPN access** (£12/month)
3. **Total Cost**: £92-132/month (31-44% of budget)

**Benefit**: Managed database service + low-cost VPN access
**Drawback**: Still requires database migration effort

---

## Sources

### Azure Pricing References
- <a href="https://azure.microsoft.com/en-us/pricing/details/postgresql/flexible-server/" target="_blank">Azure Database for PostgreSQL Pricing</a>
- <a href="https://azure.microsoft.com/en-us/pricing/details/vpn-gateway/" target="_blank">Azure VPN Gateway Pricing</a>
- <a href="https://azure.microsoft.com/en-us/pricing/details/private-link/" target="_blank">Azure Private Link Pricing</a>
- <a href="https://azure.microsoft.com/en-us/pricing/details/bandwidth/" target="_blank">Azure Bandwidth Pricing</a>
- <a href="https://instances.vantage.sh/azure/vm/d2s-v5" target="_blank">Azure VM D2s v5 Pricing (Vantage)</a>

### Azure Documentation
- <a href="https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/concepts-azure-ad-authentication" target="_blank">Microsoft Entra Authentication - Azure Database for PostgreSQL</a>
- <a href="https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/how-to-configure-sign-in-azure-ad-authentication" target="_blank">Use Microsoft Entra ID Authentication - PostgreSQL</a>
- <a href="https://learn.microsoft.com/en-us/azure/azure-arc/data/what-is-azure-arc-enabled-postgresql" target="_blank">Azure Arc-enabled PostgreSQL</a>
- <a href="https://learn.microsoft.com/en-us/azure/azure-arc/servers/private-link-security" target="_blank">Azure Arc Private Link Security</a>
- <a href="https://learn.microsoft.com/en-us/azure/vpn-gateway/point-to-site-vpn-client-cert-windows" target="_blank">Azure VPN Gateway Point-to-Site Configuration</a>

### PostgreSQL and On-Premises Azure AD
- <a href="https://www.postgresql.org/message-id/005301da5d3f$b9543f80$2bfcbe80$@gmail.com" target="_blank">PostgreSQL: Feature request - MS Entra ID Authentication from On-premises</a>

### Alternative Solutions
- <a href="https://moriyama.co.uk/about-us/news/blog-switching-to-wireguard-and-reducing-costs-in-azure/" target="_blank">Switching to WireGuard and reducing costs in Azure</a>
- <a href="https://github.com/vijayshinva/AzureWireGuard" target="_blank">Azure WireGuard Bicep Template (GitHub)</a>
- <a href="https://tailscale.com/pricing" target="_blank">Tailscale Pricing</a>
- <a href="https://tailscale.com/use-cases/infrastructure-access" target="_blank">Tailscale Infrastructure Access</a>
- <a href="https://www.cloudflare.com/plans/zero-trust-services/" target="_blank">Cloudflare Zero Trust Pricing</a>
- <a href="https://www.cloudflare.com/products/zero-trust/access/" target="_blank">Cloudflare Access</a>

### Industry Analysis
- <a href="https://learn.microsoft.com/en-us/answers/questions/51871/azure-bastion-vs-azure-vpn-point-on-site" target="_blank">Azure Bastion vs Azure VPN Point-to-Site (Microsoft Q&A)</a>
- <a href="https://www.twingate.com/blog/azure-vpn-pricing" target="_blank">Demystifying Azure VPN Pricing & Affordable Alternatives (Twingate)</a>

---

**Document Version**: 1.0
**Last Updated**: 2026-02-17
**Next Review**: After Phase 1 environment discovery completion
