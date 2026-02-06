# Zero Trust Network Access (ZTNA) Provider Research
## For Small Business Multi-Site Environment (35 Users)

**Research Date:** February 6, 2026
**Environment:** 3 offices, 35 total users, mixed Windows/Linux, existing SonicWall/Draytek infrastructure

---

## Executive Summary

This research evaluates Zero Trust Network Access (ZTNA) solutions suitable for a small business environment with specific requirements for ODBC database access, RDP connectivity, and site-to-site networking across three office locations. The analysis focuses on solutions under $10/user/month that can integrate with existing firewall infrastructure.

**Key Finding:** Several ZTNA providers offer solutions within budget, with **Tailscale**, **Twingate**, **Cloudflare Zero Trust**, and **ZeroTier** emerging as top candidates for this use case.

---

## Environment Requirements Summary

### Office Infrastructure
- **Office1 (24 users):** Linux database server (ODBC access), 3 RDP-accessible PCs, SonicWall firewall with VLANs
- **Office2 (8 users):** Access to Office1 database, SonicWall firewall, IPSec tunnel to Office1
- **Office3 (3 users):** Local database on user machine, Draytek router, traditional VPN

### Technical Requirements
1. ODBC database access from Microsoft Excel (all 35 users to Office1 database)
2. Office1 users need access to Office3 local database
3. RDP access to 3 PCs in Office1 (manual + automated scheduled tasks)
4. Site-to-site mesh networking capability
5. Integration with SonicWall Gen 7+ and Draytek routers
6. Support for both user-to-resource and site-to-site connections

### Budget & Constraints
- **Target Budget:** Under $10/user/month
- **Total Users:** 35
- **Monthly Budget Target:** ~$350/month or less
- **Annual Budget Target:** ~$4,200/year

---

## ZTNA Provider Comparison Matrix

### Budget-Friendly Solutions (Under $10/User/Month)

| Provider | Pricing | Free Tier | User Limit | Annual Cost (35 users) |
|----------|---------|-----------|------------|------------------------|
| **Tailscale** | $6/user/month (Starter) | 3 users, 100 devices | No limit | $2,520/year |
| **Twingate** | $5-6/user/month (Teams) | 5 users | No stated limit | $2,100-2,520/year |
| **ZeroTier** | $5/month flat + usage | 10 devices, 3 networks | Usage-based | ~$2,100-3,000/year (est.) |
| **Cloudflare Access** | $3-7/user/month | 50 users | No limit | $1,260-2,940/year |
| **NordLayer** | $9/user/month (Core) | None | No limit | $3,780/year |
| **Kitecyber Infra Shield** | $5-7/user/month | 10 users | Tiered | $2,100-2,940/year |

### Enterprise Solutions (Over $10/User/Month)

| Provider | Pricing | Notes | Annual Cost (35 users) |
|----------|---------|-------|------------------------|
| **Check Point Harmony SASE** | $30/user/month | 50-license minimum | $12,600/year (exceeds budget) |
| **Palo Alto Prisma Access** | $12-20/user/month | Mid-range option | $5,040-8,400/year |
| **Zscaler ZPA** | $140-375/user/year | Complex pricing | $4,900-13,125/year |
| **StrongDM** | $50-70/user/month | 20-user minimum | $12,000-16,800/year (exceeds budget) |
| **SonicWall Cloud Secure Edge** | Custom pricing | Integrated with SonicWall | Contact vendor |

---

## Detailed Provider Analysis

### 1. Tailscale ⭐ **RECOMMENDED**

**Pricing:**
- **Personal Free:** 3 users, 100 devices
- **Personal Plus:** $5/month (6 users, 100 devices)
- **Starter:** $6/user/month (best for 35 users)
- **Premium:** $18/user/month
- **Enterprise:** Custom pricing

**Total Cost for 35 Users:** $2,520/year (within budget)

**Key Features:**
- Zero-config VPN based on WireGuard protocol
- Subnet routing for site-to-site networking
- Direct peer-to-peer mesh connections
- Granular network segmentation with ACLs
- No firewall port forwarding required
- Infrastructure agnostic (Windows, Linux, macOS, mobile)
- 100+ technology integrations
- Web-based admin console
- Integration with major identity providers

**Database & RDP Support:**
- ✅ Subnet routers enable access to any network resource (including databases)
- ✅ RDP explicitly supported
- ✅ ODBC connections work transparently over Tailscale network
- ✅ Direct connections reduce latency vs. traditional VPN

**Site-to-Site Configuration:**
- Deploy Tailscale subnet router at each office location
- Office1 router advertises database server subnet
- Office3 router advertises local database subnet
- Users connect to Tailscale network, access resources via subnet routes
- No VPN client management needed for users

**Integration with Existing Infrastructure:**
- Install Tailscale on a Linux VM or dedicated device at each site
- Configure subnet routes for each office network
- No changes required to SonicWall or Draytek firewall rules (outbound only)
- Coexist with existing IPSec tunnel or replace it

**Pros:**
- ✅ Best-in-class user experience ("it just works")
- ✅ Mature ecosystem and excellent documentation
- ✅ Low latency peer-to-peer connections
- ✅ Simple deployment and management
- ✅ No minimum user requirement
- ✅ Strong security with WireGuard encryption

**Cons:**
- ⚠️ Coordination server is closed-source (Tailscale-controlled)
- ⚠️ Less granular application-level controls than pure ZTNA solutions
- ⚠️ Requires Tailscale agent on user devices

**Best For:** Organizations prioritizing ease of use, low latency, and simple site-to-site networking.

**Sources:**
- <a href="https://tailscale.com/pricing" target="_blank">Tailscale Pricing</a>
- <a href="https://tailscale.com/kb/1019/subnets" target="_blank">Tailscale Subnet Routers Documentation</a>
- <a href="https://tailscale.com/use-cases/remote-access" target="_blank">Tailscale Remote Access Use Cases</a>

---

### 2. Twingate ⭐ **RECOMMENDED**

**Pricing:**
- **Starter:** Free (up to 5 users, 1 admin, 10 remote networks)
- **Teams:** $5/user/month (yearly) or $6/user/month (monthly)
- **Business:** $10/user/month (yearly) or $12/user/month (monthly)
- **Enterprise:** Custom pricing

**Total Cost for 35 Users:** $2,100/year (Teams plan, yearly) - **LOWEST COST**

**Key Features:**
- Zero Trust architecture with no open ports
- Application-level access control (more granular than Tailscale)
- Traffic routed through encrypted relays (not pure peer-to-peer)
- Connector-based architecture (deploy connectors at each site)
- Seamless identity provider integration (Azure AD, Okta, Google)
- Device posture checking
- Comprehensive access logs for compliance
- Resource-level access policies

**Database & RDP Support:**
- ✅ Explicit support for database access
- ✅ RDP explicitly supported
- ✅ ODBC connections work through Twingate connectors
- ✅ More controlled connection path than mesh VPN

**Site-to-Site Configuration:**
- Deploy Twingate Connector at each office location (VM or container)
- Create Remote Networks for each site in Twingate Admin Console
- Define Resources (Office1 database, Office3 database, RDP hosts)
- Assign access policies per user/group
- Connectors automatically load-balance and provide failover

**Integration with Existing Infrastructure:**
- Connectors require only outbound internet access (no inbound firewall rules)
- Deploy behind SonicWall and Draytek without firewall changes
- Can coexist with existing IPSec tunnel
- No routing changes required

**Pros:**
- ✅ Lowest pricing option at $5/user/month (yearly)
- ✅ Application-level security and granular access control
- ✅ No exposed ports (better security than traditional VPN)
- ✅ Excellent for compliance-heavy environments
- ✅ Free tier suitable for testing with Office3 (3 users)
- ✅ Automatic connector redundancy and load-balancing
- ✅ Easy integration with identity providers

**Cons:**
- ⚠️ Not true peer-to-peer (traffic routes through relays)
- ⚠️ May have slightly higher latency than Tailscale mesh
- ⚠️ Requires connector deployment and management
- ⚠️ More complex initial setup than Tailscale

**Best For:** Organizations prioritizing security, compliance, and granular access control with the lowest cost.

**Sources:**
- <a href="https://www.twingate.com/pricing" target="_blank">Twingate Pricing</a>
- <a href="https://www.twingate.com/docs/connector-deployment" target="_blank">Twingate Connector Deployment</a>
- <a href="https://www.twingate.com/docs/site-2-site" target="_blank">Twingate Site-to-Site Connections</a>

---

### 3. Cloudflare Zero Trust

**Pricing:**
- **Free Tier:** Up to 50 users
- **Pay-as-you-go:** $7/user/month
- **Access (ZTNA):** Starting at $3/month (separate component)
- **Gateway:** Part of Zero Trust suite

**Total Cost for 35 Users:** Free to $2,940/year (depending on features needed)

**Key Features:**
- Cloudflare's global network for low-latency access
- Access secures internal resources with per-user authentication
- Supports SSH, VNC, RDP, and arbitrary L4-L7 TCP/UDP traffic
- Integration with Cloudflare Tunnel for exposing resources
- Strong DDoS protection and security filtering
- Can protect self-hosted, SaaS, and non-web apps

**Database & RDP Support:**
- ✅ RDP explicitly supported
- ✅ TCP/UDP traffic support enables ODBC connections
- ⚠️ May require Cloudflare Tunnel setup for database access
- ⚠️ Less clear documentation on ODBC-specific scenarios

**Site-to-Site Configuration:**
- Deploy Cloudflare Tunnel connectors at each site
- Configure Access policies for resources
- Users authenticate through Cloudflare Access
- Traffic routes through Cloudflare's global network

**Integration with Existing Infrastructure:**
- Cloudflare Tunnel requires outbound HTTPS (port 443)
- No inbound firewall rules required
- Can work alongside existing VPN infrastructure

**Pros:**
- ✅ **FREE for up to 50 users** (covers your 35-user environment)
- ✅ Leverages Cloudflare's massive global network
- ✅ Strong security and DDoS protection
- ✅ Mature enterprise platform
- ✅ Good for organizations already using Cloudflare

**Cons:**
- ⚠️ More complex setup than Tailscale
- ⚠️ Database access configuration less straightforward
- ⚠️ Requires understanding of Cloudflare ecosystem
- ⚠️ May require paid tier for advanced features

**Best For:** Organizations already using Cloudflare or wanting enterprise-grade security with potential for free tier usage.

**Sources:**
- <a href="https://www.cloudflare.com/plans/zero-trust-services/" target="_blank">Cloudflare Zero Trust Pricing</a>
- <a href="https://www.cloudflare.com/zero-trust/products/access/" target="_blank">Cloudflare Access Product Overview</a>

---

### 4. ZeroTier

**Pricing:**
- **Basic:** 10 devices, 3 networks (free)
- **Essential:** $5/month flat + usage-based for devices beyond 10
- **Custom:** 500+ devices (custom pricing)

**Total Cost for 35 Users:** Estimated $2,100-3,000/year (depending on device count)

**Key Features:**
- Software-defined network overlay (SD-WAN approach)
- Device-centric architecture optimized for IoT
- Peer-to-peer mesh networking
- Unique cryptographic device ID
- Vendor-agnostic platform
- Works with various routers and gateways
- Multi-cloud environment support

**Database & RDP Support:**
- ✅ Layer 2/3 networking supports all protocols (ODBC, RDP)
- ✅ Devices appear on same virtual LAN
- ✅ Transparent for all applications
- ✅ No special configuration needed

**Site-to-Site Configuration:**
- Install ZeroTier on devices/servers at each site
- Create virtual network in ZeroTier Central
- Authorize devices to join network
- All devices appear on same Layer 2 network

**Integration with Existing Infrastructure:**
- Can bridge ZeroTier virtual network to physical LANs
- No firewall configuration required (outbound only)
- Works alongside existing infrastructure

**Pros:**
- ✅ Simple Layer 2 virtual networking
- ✅ Excellent for IoT and hybrid environments
- ✅ Low cost for small deployments
- ✅ Direct peer-to-peer connections
- ✅ Open-source client

**Cons:**
- ⚠️ Less enterprise-focused than Twingate/Tailscale
- ⚠️ Usage-based pricing can be unpredictable
- ⚠️ Fewer identity provider integrations
- ⚠️ Less granular access control than pure ZTNA

**Best For:** Organizations managing IoT devices or preferring Layer 2 networking with open-source components.

**Sources:**
- <a href="https://www.zerotier.com/pricing/" target="_blank">ZeroTier Pricing</a>
- <a href="https://www.zerotier.com/blog/zerotier-review-everything-you-need-to-know-about-zerotier-in-2023/" target="_blank">ZeroTier Overview</a>

---

### 5. NordLayer

**Pricing:**
- **Lite:** $9/user/month
- **Core:** $9+/user/month (adds site-to-site VPN)
- **Premium:** Higher tier (API access, SSO)

**Total Cost for 35 Users:** $3,780+/year (exceeds $10/user budget)

**Key Features:**
- Business VPN with Zero Trust features
- Site-to-site network connectors
- Biometric authentication
- Auto-connect capability
- Third-party SSO integrations (Azure AD, Okta)
- Network segmentation (Premium tier)

**Database & RDP Support:**
- ✅ VPN-based approach supports all protocols
- ✅ Site-to-site connectors enable office interconnection
- ✅ Standard VPN functionality for all applications

**Site-to-Site Configuration:**
- Deploy site-to-site connectors at each office
- Configure secure VPN tunnels between sites
- Users connect via NordLayer VPN client

**Pros:**
- ✅ Site-to-site VPN explicitly supported
- ✅ User-friendly interface
- ✅ Good for organizations wanting traditional VPN with modern features

**Cons:**
- ❌ Exceeds $10/user budget at $9/user minimum
- ⚠️ More traditional VPN approach vs. true ZTNA
- ⚠️ Core tier required for site-to-site (higher cost)

**Best For:** Organizations wanting traditional VPN replacement with Zero Trust features and willing to spend more.

**Sources:**
- <a href="https://nordlayer.com/pricing/" target="_blank">NordLayer Pricing</a>
- <a href="https://www.security.org/vpn/nordlayer/" target="_blank">NordLayer Review</a>

---

### 6. SonicWall Cloud Secure Edge (Banyan Security)

**Pricing:** Custom (contact vendor)

**Key Features:**
- Native integration with SonicWall Gen 7+ firewalls
- Built-in ZTNA connector in SonicWall firewalls
- Part of SonicWall's unified security platform
- Previously Banyan Security (acquired by SonicWall)
- Free for up to 20 users (Banyan legacy pricing)

**Database & RDP Support:**
- ✅ Full protocol support through ZTNA connector
- ✅ Tight integration with SonicWall firewall infrastructure
- ✅ Least privilege access with continuous authorization

**Integration with Existing Infrastructure:**
- ⭐ **NATIVE INTEGRATION** with existing SonicWall firewalls at Office1 and Office2
- Built-in connector in Gen 7+ SonicWalls (just configure, no separate deployment)
- Unified management across firewall and ZTNA
- Seamless for existing SonicWall customers

**Pros:**
- ✅ **Native SonicWall integration** (major advantage for Office1 & Office2)
- ✅ Single pane of glass management
- ✅ No additional hardware/VMs needed at SonicWall sites
- ✅ Continuous device posture checking
- ✅ Enterprise security features

**Cons:**
- ⚠️ Custom pricing (may exceed budget)
- ⚠️ Office3 (Draytek) would need separate connector deployment
- ⚠️ Pricing not transparent

**Best For:** Organizations already invested in SonicWall infrastructure wanting unified management.

**Sources:**
- <a href="https://www.sonicguard.com/secure-private-access.asp" target="_blank">SonicWall Secure Private Access</a>
- <a href="https://blog.sonicwall.com/en-us/2024/07/give-a-ztna-boost-to-your-sonicwall-firewall/" target="_blank">SonicWall ZTNA Boost</a>
- <a href="https://www.banyansecurity.io/" target="_blank">Banyan Security (Now SonicWall)</a>

---

## Performance Considerations

### ZTNA vs Traditional VPN Performance

**Traditional VPN Performance Issues:**
- **Hairpinning:** Traffic routes from user → corporate gateway → internet/datacenter → back to user
- **Central bottleneck:** All traffic funnels through single gateway
- **High latency:** Backhaul to central location adds delays
- **Bandwidth constraints:** Corporate gateway becomes congestion point
- **Poor user experience:** Especially for cloud/SaaS applications

**ZTNA Performance Advantages:**
- **Direct connections:** Users connect directly to applications (mesh networking)
- **Distributed architecture:** No central gateway bottleneck
- **Lower latency:** Avoids unnecessary backhaul
- **Better for remote users:** Optimal routing to cloud resources
- **Faster load times:** Direct paths reduce hops

**Source:** <a href="https://www.twingate.com/blog/ztna-reduces-network-latency" target="_blank">How ZTNA Reduces Network Latency</a>

---

### ODBC Database Access Performance

**Key Considerations for Excel ODBC Queries:**

1. **Network Latency Impact:**
   - ODBC queries are latency-sensitive (multiple round-trips)
   - Direct peer-to-peer connections (Tailscale, ZeroTier) minimize latency
   - Relay-based solutions (Twingate) may add ~10-50ms per query
   - Traditional VPN with hairpinning adds significant latency

2. **Excel ODBC Optimization:**
   - Use bound columns (SQLBindCol) instead of SQLGetData
   - Implement bulk operations for inserts/updates
   - Restrict query result sets to only needed fields and rows
   - Avoid RefreshAll (30+ minutes) in favor of targeted query refresh (<1 minute)
   - ODBC connections default to read-only (include ReadOnly=0 for write access)

3. **Connection Architecture:**
   - **Persistent connections:** ZTNA maintains persistent tunnels (better than on-demand VPN)
   - **Split tunneling:** Some ZTNA solutions allow split tunneling (local traffic doesn't route through VPN)
   - **Compression:** Some solutions offer compression (improves large result sets)

4. **Microsoft Office Considerations:**
   - Use 64-bit ODBC drivers with 64-bit Office
   - Click-to-Run Office (Microsoft 365) has virtualization limitations (resolved in Version 2009+)
   - Consider using Get & Transform (Power Query) for better performance than legacy ODBC connections

**Expected Performance:**
- **Tailscale/ZeroTier (peer-to-peer):** Near-native LAN performance (1-5ms additional latency)
- **Twingate (relay-based):** 10-50ms additional latency depending on relay location
- **Traditional VPN:** 50-200ms+ additional latency with hairpinning
- **Cloudflare:** Latency depends on proximity to Cloudflare PoP (typically 5-30ms)

**Sources:**
- <a href="https://learn.microsoft.com/en-us/office/troubleshoot/access/optimize-odbc-data-sources" target="_blank">Microsoft: Optimize ODBC Data Sources</a>
- <a href="https://medium.com/@ngpiesco/performance-considerations-orms-odbc-and-database-drivers-5baf3b03922a" target="_blank">ODBC Performance Considerations</a>

---

### RDP Performance Over ZTNA

**RDP Protocol Characteristics:**
- Sensitive to latency (100ms+ latency degrades user experience)
- Moderate bandwidth requirements (50-200 Kbps for typical use)
- Benefits from persistent connections
- Compression helps (RDP includes built-in compression)

**ZTNA RDP Performance:**
- **Tailscale/ZeroTier:** Excellent RDP performance (direct peer-to-peer)
- **Twingate:** Good RDP performance (relay adds minimal latency)
- **Cloudflare:** Good performance through global network
- All solutions support persistent connections (better than on-demand VPN)

**Automated Tasks Consideration:**
- RDP-based automation (scheduled tasks) requires persistent connectivity
- ZTNA solutions with auto-reconnect are essential
- Site-to-site connectors (Tailscale subnet router, Twingate connector) ensure always-on connectivity
- Better than user-initiated VPN connections for automation

---

## Integration Scenarios

### Scenario 1: Minimal Change (Coexist with Current Infrastructure)

**Approach:** Deploy ZTNA alongside existing IPSec tunnel and VPN

**Configuration:**
1. Keep IPSec tunnel between Office1 and Office2 for existing services
2. Deploy Tailscale or Twingate for:
   - Office3 access (replace traditional VPN)
   - Remote/mobile users
   - ODBC database access
3. Users install ZTNA client on workstations
4. Office1 and Office2 can use ZTNA or existing IPSec tunnel

**Pros:**
- ✅ Low risk (existing infrastructure unchanged)
- ✅ Can migrate gradually
- ✅ Immediate benefit for Office3 users

**Cons:**
- ⚠️ Maintains two separate systems
- ⚠️ More complex management

---

### Scenario 2: Full ZTNA Migration

**Approach:** Replace IPSec tunnel and VPN with ZTNA mesh network

**Configuration:**
1. Deploy ZTNA solution at all three offices:
   - Office1: Connector/subnet router + database server subnet
   - Office2: Connector/subnet router (if needed) or direct user clients
   - Office3: Connector/subnet router + local database access
2. All users install ZTNA client
3. Configure access policies for:
   - Office1 database → all 35 users
   - Office3 database → Office1 users (24 users)
   - RDP hosts → authorized users
4. Decommission IPSec tunnel after testing
5. Remove traditional VPN

**Pros:**
- ✅ Unified security and access control
- ✅ Simpler long-term management
- ✅ Better performance (no hairpinning)
- ✅ Consistent user experience

**Cons:**
- ⚠️ Higher initial effort
- ⚠️ Requires testing and validation
- ⚠️ Potential disruption during migration

---

### Scenario 3: Hybrid with SonicWall Native ZTNA

**Approach:** Use SonicWall Cloud Secure Edge for Office1/Office2, separate solution for Office3

**Configuration:**
1. Enable built-in ZTNA connector on Office1 and Office2 SonicWall firewalls (Gen 7+)
2. Configure SonicWall Cloud Secure Edge for 32 users (Office1 + Office2)
3. Deploy lightweight ZTNA client (Tailscale/Twingate) for Office3 access
4. Unified access policies in SonicWall admin console

**Pros:**
- ✅ Leverages existing SonicWall investment
- ✅ Unified management for Office1/Office2
- ✅ No additional hardware at SonicWall sites

**Cons:**
- ⚠️ Requires SonicWall Gen 7+ firewalls (verify model compatibility)
- ⚠️ Custom pricing (may exceed budget)
- ⚠️ Office3 requires separate solution

---

## Implementation Checklist

### Pre-Implementation (Planning Phase)

- [ ] **Verify firewall models:** Confirm SonicWall models (Gen 7+ for native ZTNA), Draytek model
- [ ] **Identity provider:** Determine if using Azure AD, Google Workspace, Okta, or local AD
- [ ] **Database details:** Document database type (MySQL, PostgreSQL, SQL Server, etc.) and version
- [ ] **Network mapping:** Document IP subnets for each office
- [ ] **User access matrix:** Map which users need access to which resources
- [ ] **Pilot group:** Select 3-5 users for pilot testing
- [ ] **Budget approval:** Confirm budget authority for selected solution
- [ ] **Stakeholder buy-in:** Get management approval for migration approach

### Evaluation Phase (Proof of Concept)

- [ ] **Free tier testing:** Deploy free tier of top 2-3 solutions:
  - Tailscale (3 users free)
  - Twingate (5 users free)
  - Cloudflare (50 users free)
- [ ] **Test ODBC connectivity:** Excel ODBC queries to Office1 database
- [ ] **Test RDP access:** RDP to Office1 PCs
- [ ] **Test site-to-site:** Office1 users accessing Office3 database
- [ ] **Performance testing:** Measure query latency and RDP responsiveness
- [ ] **User experience feedback:** Gather feedback from pilot users
- [ ] **Administrative testing:** Evaluate admin console, policy management, logging
- [ ] **Documentation review:** Review vendor documentation and support resources

### Deployment Phase (Recommended: Phased Approach)

**Phase 1: Office3 (Lowest Risk - 3 Users)**
- [ ] Deploy ZTNA connector/subnet router at Office3
- [ ] Install client on 3 Office3 user devices
- [ ] Configure access to Office3 local database
- [ ] Test Office1 users connecting to Office3 database
- [ ] Decommission Office3 traditional VPN after validation

**Phase 2: Office1 Database Access (All 35 Users)**
- [ ] Deploy ZTNA connector/subnet router at Office1
- [ ] Configure access to Office1 Linux database server
- [ ] Rollout client to Office2 users (8 users)
- [ ] Rollout client to Office1 users (24 users)
- [ ] Test Excel ODBC connections from all users
- [ ] Monitor performance and user feedback

**Phase 3: RDP and IPSec Replacement**
- [ ] Configure RDP access policies for Office1 PCs
- [ ] Test automated scheduled tasks over ZTNA RDP
- [ ] Monitor RDP performance and reliability
- [ ] If replacing IPSec: Deploy ZTNA connector at Office2
- [ ] Validate all Office2 services accessible
- [ ] Decommission IPSec tunnel after validation period

**Phase 4: Decommission Legacy Infrastructure**
- [ ] Remove traditional VPN infrastructure
- [ ] Update documentation
- [ ] Provide final user training
- [ ] Archive old VPN configuration for reference

### Post-Deployment

- [ ] **Monitoring:** Set up alerts for connector downtime, user issues
- [ ] **Documentation:** Document configuration, access policies, troubleshooting
- [ ] **User training:** Create quick-start guides for common tasks
- [ ] **Regular reviews:** Monthly review of access logs and policies
- [ ] **Backup access:** Maintain emergency backup access method (admin VPN)

---

## Specific Recommendations

### Primary Recommendation: **Tailscale Starter Plan**

**Rationale:**
- ✅ **Best user experience:** "It just works" simplicity
- ✅ **Within budget:** $6/user/month = $2,520/year
- ✅ **Low latency:** Peer-to-peer mesh networking ideal for ODBC queries
- ✅ **Simple deployment:** Subnet routers easy to configure
- ✅ **No minimum users:** Pay only for what you need
- ✅ **Excellent documentation:** Strong community and support
- ✅ **Mature platform:** Proven at scale
- ✅ **Works with SonicWall and Draytek:** No firewall changes required

**Deployment Approach:**
1. Install Tailscale subnet router at each office (Linux VM or dedicated device)
2. Office1 subnet router advertises database server subnet + RDP hosts
3. Office3 subnet router advertises local database machine
4. Users install Tailscale client on their workstations
5. Configure ACLs in Tailscale admin console for access control
6. Test ODBC and RDP connectivity
7. Decommission traditional VPN

**Cost Breakdown:**
- 35 users × $6/month = $210/month
- Annual cost: $2,520
- Subnet routers: Free (use existing hardware or VMs)

---

### Budget-Conscious Alternative: **Twingate Teams Plan**

**Rationale:**
- ✅ **Lowest cost:** $5/user/month = $2,100/year (saves $420 vs. Tailscale)
- ✅ **Better access control:** Application-level policies
- ✅ **Compliance-friendly:** Detailed access logs
- ✅ **Free Office3 testing:** 5-user free tier covers Office3 initially
- ✅ **Works with SonicWall and Draytek:** No firewall changes required

**Deployment Approach:**
1. Start with free tier for Office3 (3 users) to validate
2. Deploy Twingate Connector at each office (VM or container)
3. Define Remote Networks and Resources in Twingate admin console
4. Users install Twingate client on their workstations
5. Configure granular access policies per user/group
6. Upgrade to Teams plan when ready to onboard all 35 users

**Cost Breakdown:**
- 35 users × $5/month (yearly) = $175/month
- Annual cost: $2,100
- Connectors: Free (use existing hardware or VMs)

---

### Free Option: **Cloudflare Zero Trust**

**Rationale:**
- ✅ **FREE for up to 50 users** (covers all 35 users with room to grow)
- ✅ **Enterprise-grade security:** Cloudflare's global network
- ✅ **Scalable:** Grow without immediate cost concerns
- ✅ **Strong security features:** DDoS protection, filtering

**Considerations:**
- ⚠️ More complex setup than Tailscale
- ⚠️ ODBC database access less documented
- ⚠️ Requires learning Cloudflare ecosystem
- ⚠️ May need paid tier for advanced features later

**When to Choose:**
- Already using Cloudflare for DNS/CDN
- IT team comfortable with Cloudflare platform
- Want to minimize immediate costs
- Need enterprise-grade security with free tier

---

### SonicWall Customers: **SonicWall Cloud Secure Edge**

**Rationale:**
- ✅ **Native integration** with existing SonicWall Gen 7+ firewalls
- ✅ **Unified management:** Single admin console
- ✅ **No additional hardware:** Built-in connector in firewalls
- ✅ **Enterprise features:** Continuous authorization, device posture

**Considerations:**
- ⚠️ Custom pricing (may exceed budget)
- ⚠️ Requires SonicWall Gen 7+ (verify model compatibility)
- ⚠️ Office3 needs separate connector deployment

**When to Choose:**
- Already invested in SonicWall infrastructure
- Office1 and Office2 have compatible SonicWall models
- Willing to evaluate custom pricing
- Value unified management over lowest cost

---

## Risk Mitigation Strategies

### Technical Risks

**Risk:** ODBC query performance degradation
- **Mitigation:** Pilot test with realistic Excel queries, measure latency
- **Mitigation:** Choose peer-to-peer solution (Tailscale/ZeroTier) for lowest latency
- **Mitigation:** Optimize Excel ODBC queries (use bound columns, restrict result sets)

**Risk:** RDP automated tasks fail over ZTNA
- **Mitigation:** Test scheduled tasks during pilot phase
- **Mitigation:** Use site-to-site connectors for always-on connectivity
- **Mitigation:** Configure auto-reconnect on ZTNA clients

**Risk:** Connector/subnet router failure
- **Mitigation:** Deploy redundant connectors at critical sites (Office1)
- **Mitigation:** Maintain backup VPN access during migration
- **Mitigation:** Monitor connector health with alerts

**Risk:** User devices incompatible with ZTNA client
- **Mitigation:** Verify OS compatibility during pilot
- **Mitigation:** Update older workstations before full rollout
- **Mitigation:** Web-based access as fallback (if supported)

### Operational Risks

**Risk:** User resistance to new technology
- **Mitigation:** Include users in pilot phase, gather feedback
- **Mitigation:** Provide clear training and documentation
- **Mitigation:** Emphasize benefits (better performance, simpler access)
- **Mitigation:** Offer hands-on support during rollout

**Risk:** Disruption during migration
- **Mitigation:** Phased rollout starting with lowest-risk site (Office3)
- **Mitigation:** Maintain parallel systems during migration
- **Mitigation:** Schedule migration during low-usage periods
- **Mitigation:** Have rollback plan ready

**Risk:** Budget overruns
- **Mitigation:** Start with free tiers for evaluation (Tailscale 3 users, Twingate 5 users, Cloudflare 50 users)
- **Mitigation:** Get written quotes from vendors before commitment
- **Mitigation:** Monitor usage-based pricing (ZeroTier) carefully
- **Mitigation:** Choose yearly billing for cost savings (Twingate: 16% savings)

### Security Risks

**Risk:** Misconfigured access policies
- **Mitigation:** Follow principle of least privilege
- **Mitigation:** Start with restrictive policies, expand as needed
- **Mitigation:** Regular access policy audits
- **Mitigation:** Use identity provider integration for role-based access

**Risk:** Compromised user credentials
- **Mitigation:** Require MFA for all users
- **Mitigation:** Integrate with identity provider (Azure AD, Okta)
- **Mitigation:** Device posture checking (verify device health before access)
- **Mitigation:** Monitor access logs for anomalies

**Risk:** Connector compromise
- **Mitigation:** Deploy connectors on hardened VMs/hosts
- **Mitigation:** Keep connectors updated
- **Mitigation:** Segment connector network from sensitive resources
- **Mitigation:** Regular security audits

---

## Next Steps

### Immediate Actions (This Week)

1. **Answer clarifying questions** (see top of document) to refine recommendation
2. **Verify infrastructure compatibility:**
   - Check SonicWall models at Office1 and Office2 (Gen 7+ for native ZTNA?)
   - Check Draytek model at Office3
   - Confirm identity provider (Azure AD, Google Workspace, etc.)
3. **Get budget approval** for $2,100-2,520/year ZTNA solution
4. **Select 3-5 pilot users** for initial testing (include mix from all offices)

### Short-Term Actions (Next 2 Weeks)

1. **Deploy free tier testing:**
   - **Tailscale:** 3-user free tier at Office3
   - **Twingate:** 5-user free tier across offices
   - **Cloudflare:** Full 35 users in free tier (if comfortable with setup)
2. **Test critical use cases:**
   - Excel ODBC queries from Office2/Office3 to Office1 database
   - Office1 users accessing Office3 local database
   - RDP to Office1 PCs (manual and automated tasks)
3. **Measure performance:**
   - Query latency before and after ZTNA
   - RDP responsiveness
   - User feedback on experience
4. **Evaluate administration:**
   - Test access policy management
   - Review logging and monitoring capabilities
   - Assess deployment complexity

### Medium-Term Actions (Next Month)

1. **Select final solution** based on pilot results
2. **Create detailed deployment plan:**
   - Phase 1: Office3 migration
   - Phase 2: Office1 database access
   - Phase 3: RDP and IPSec replacement
3. **Prepare infrastructure:**
   - Provision VMs for connectors/subnet routers
   - Configure network access for connectors
   - Prepare user workstation inventory
4. **Develop user training materials:**
   - Quick-start guide for client installation
   - ODBC connection guide for Excel users
   - RDP connection guide
   - Troubleshooting FAQs

### Long-Term Actions (Next Quarter)

1. **Execute phased rollout** per deployment checklist
2. **Monitor and optimize:**
   - User satisfaction surveys
   - Performance metrics
   - Access log reviews
3. **Decommission legacy infrastructure:**
   - Traditional VPN
   - IPSec tunnel (if replaced)
4. **Document lessons learned** and refine policies

---

## Comparison Summary Table

| Criterion | **Tailscale** ⭐ | **Twingate** ⭐ | **Cloudflare** | **ZeroTier** | **NordLayer** |
|-----------|-----------------|-----------------|----------------|--------------|---------------|
| **Cost (35 users/year)** | $2,520 | $2,100 | FREE-$2,940 | $2,100-3,000 | $3,780+ |
| **Within Budget ($10/user)** | ✅ Yes | ✅ Yes | ✅ Yes | ✅ ~Yes | ❌ No |
| **ODBC Support** | ✅ Excellent | ✅ Good | ⚠️ Good | ✅ Excellent | ✅ Good |
| **RDP Support** | ✅ Explicit | ✅ Explicit | ✅ Explicit | ✅ Transparent | ✅ Explicit |
| **Site-to-Site** | ✅ Subnet routers | ✅ Connectors | ✅ Tunnels | ✅ Virtual LAN | ✅ Native |
| **Performance (Latency)** | ⭐ Excellent (P2P) | ✅ Good (Relay) | ✅ Good (Global) | ⭐ Excellent (P2P) | ✅ Good (VPN) |
| **Ease of Use** | ⭐ Excellent | ✅ Good | ⚠️ Moderate | ✅ Good | ✅ Good |
| **Security/Access Control** | ✅ Good (ACLs) | ⭐ Excellent (App-level) | ⭐ Excellent | ✅ Good | ✅ Good |
| **SonicWall Integration** | ⚠️ No | ⚠️ No | ⚠️ No | ⚠️ No | ⚠️ No |
| **Draytek Integration** | ⚠️ No | ⚠️ No | ⚠️ No | ⚠️ No | ⚠️ No |
| **Free Tier for Testing** | 3 users | 5 users | 50 users | 10 devices | None |
| **Documentation** | ⭐ Excellent | ✅ Good | ✅ Good | ✅ Good | ✅ Good |
| **Ideal For** | Ease of use, performance | Low cost, compliance | Free tier, security | IoT, open source | Traditional VPN users |

**Legend:**
- ⭐ = Exceptional
- ✅ = Good/Supported
- ⚠️ = Caution/Moderate
- ❌ = Not recommended/Unsupported

---

## Additional Considerations

### ODBC Driver and Excel Configuration

**Critical Setup Requirements:**
1. **Matching architecture:** 64-bit ODBC drivers for 64-bit Excel (most Microsoft 365 installations)
2. **ReadOnly setting:** Clear ReadOnly checkbox or include "ReadOnly=0" in connection string for write access
3. **Click-to-Run Office:** Ensure Microsoft 365 Apps Version 2009+ for proper ODBC access outside virtualization
4. **Admin rights:** Users need local admin rights to configure ODBC data sources (one-time setup)
5. **Performance:** Use Get & Transform (Power Query) instead of legacy ODBC for better performance

**ZTNA-Specific Considerations:**
- ODBC connection strings may need hostname/IP updates if ZTNA uses different addressing
- Test Excel ODBC queries with realistic data volume during pilot
- Some ZTNA solutions assign virtual IPs (e.g., 100.x.x.x for Tailscale) - update connection strings accordingly
- Consider creating DSN (Data Source Name) entries to simplify user configuration

---

### Identity Provider Integration

**Recommended Approach:**
- If using Microsoft 365 for email: Integrate with **Azure AD** (best user experience)
- If using Google Workspace: Integrate with **Google Workspace**
- If no cloud identity provider: Consider setting up **Okta** or using ZTNA provider's built-in auth

**Benefits of Identity Provider Integration:**
- Single Sign-On (SSO) for users
- Centralized user management
- Automatic user provisioning/deprovisioning
- Role-based access control via groups
- Stronger authentication (leverage existing MFA)

**Provider Support:**
- **Tailscale:** Supports Azure AD, Google, Okta, and 100+ IdPs
- **Twingate:** Native integration with Azure AD, Okta, Google, OneLogin
- **Cloudflare:** Supports major IdPs (Azure AD, Okta, Google, etc.)
- **ZeroTier:** Basic SSO support (less mature than others)

---

### Compliance and Logging

**Compliance Requirements:**
If your organization has compliance requirements (GDPR, SOC 2, ISO 27001, etc.), consider:
- **Twingate:** Best logging and audit trails (resource-level access logs)
- **Cloudflare:** Enterprise compliance certifications
- **Tailscale:** Good logging, but less granular than Twingate

**Logging Capabilities:**
- **Twingate:** Who accessed what resource, when, from which device, for how long
- **Tailscale:** Connection logs, ACL enforcement logs
- **Cloudflare:** Comprehensive Access logs, authentication events

**Retention:**
- Most ZTNA providers retain logs for 30-90 days on standard plans
- Extended retention may require Enterprise tier
- Consider exporting logs to SIEM for long-term retention

---

## Frequently Asked Questions

### Q: Can we keep our existing IPSec tunnel during migration?
**A:** Yes, ZTNA solutions can coexist with existing infrastructure. Start by adding ZTNA for specific use cases (Office3 access, remote users) while maintaining the IPSec tunnel. Migrate gradually and decommission the tunnel only after thorough testing.

### Q: Do all users need to install a client?
**A:** It depends on the solution:
- **Tailscale/ZeroTier:** Yes, all users need client installed
- **Twingate:** Yes, client required for user devices
- **Cloudflare:** Client required for most use cases, but some resources can use browser-based access
- **Site-to-site:** With connectors/subnet routers, office-based users can access local resources without ZTNA client, but still need client for remote access

### Q: What happens if a connector fails?
**A:**
- Deploy **redundant connectors** at critical sites (Office1 with database server)
- ZTNA solutions automatically fail over to backup connector
- Monitor connector health with alerts
- Maintain emergency backup VPN during migration phase

### Q: How long does deployment take?
**A:**
- **Pilot testing:** 1-2 weeks
- **Single office (Office3):** 1-2 days
- **Full deployment (all 3 offices, 35 users):** 2-4 weeks (phased approach)
- **Total project timeline:** 6-8 weeks from decision to full production

### Q: Can automated RDP tasks work over ZTNA?
**A:** Yes, but requires proper configuration:
- Use **site-to-site connectors** (not user VPN clients) for always-on connectivity
- Ensure connector auto-reconnects after network interruptions
- Test scheduled tasks thoroughly during pilot phase
- Consider using **service accounts** with persistent ZTNA connections

### Q: How do we handle the Office3 local database?
**A:** Multiple options:
1. **Subnet router approach:** Deploy ZTNA subnet router at Office3, advertise local subnet, Office1 users access via ZTNA
2. **Host-based approach:** Install ZTNA client on the Office3 machine with local database, advertise that host
3. **Migrate database:** Consider migrating Office3 local database to Office1 server (long-term solution)

### Q: What if our SonicWalls are older models (not Gen 7+)?
**A:** Older SonicWalls don't have built-in ZTNA connector, so:
- Deploy separate ZTNA solution (Tailscale, Twingate, etc.)
- Install connector/subnet router on VM behind SonicWall
- No inbound firewall rules required (ZTNA uses outbound only)
- Consider SonicWall upgrade when budget allows for native ZTNA

### Q: How do we handle MFA?
**A:** Best approach:
- Integrate ZTNA with identity provider (Azure AD, Okta, Google)
- Leverage identity provider's MFA (already configured for Microsoft 365, Google Workspace)
- ZTNA inherits MFA settings from identity provider
- Alternative: Use ZTNA provider's built-in MFA (less seamless)

---

## Conclusion

For your 35-user, three-office environment with requirements for ODBC database access, RDP connectivity, and site-to-site networking, **Tailscale** and **Twingate** emerge as the top two recommendations:

**Choose Tailscale if:**
- Best user experience is priority
- Low latency is critical (peer-to-peer mesh)
- You value simplicity and "it just works" approach
- Excellent documentation and community support are important
- Budget allows $6/user/month ($2,520/year)

**Choose Twingate if:**
- Lowest cost is priority ($5/user/month = $2,100/year)
- Granular application-level access control is needed
- Compliance and detailed audit logs are required
- You want to test Office3 with free 5-user tier first

**Choose Cloudflare if:**
- Free tier (50 users) covers your needs
- Already using Cloudflare for other services
- Enterprise-grade security is priority
- IT team comfortable with learning Cloudflare ecosystem

**Investigate SonicWall Cloud Secure Edge if:**
- Office1 and Office2 have SonicWall Gen 7+ firewalls
- Native integration and unified management are priorities
- Custom pricing fits within budget

All recommended solutions support your technical requirements (ODBC, RDP, site-to-site) and integrate with existing infrastructure without requiring firewall changes. Start with free tier testing to validate performance with your specific use cases before committing to a paid plan.

---

## Research Sources

This research compiled information from the following authoritative sources:

**General ZTNA Market:**
- <a href="https://aimultiple.com/ztna-solutions" target="_blank">Top +10 ZTNA Solutions in 2026</a>
- <a href="https://cybersecuritynews.com/best-ztna-solutions/" target="_blank">10 Best ZTNA Solutions 2026</a>
- <a href="https://www.itbusinessedge.com/security/smb-zero-trust-solutions/" target="_blank">Top 8 Zero Trust Network Access Products for Small Businesses</a>
- <a href="https://expertinsights.com/zero-trust/the-top-zero-trust-security-solutions" target="_blank">Top 11 Zero Trust Security Solutions 2026</a>

**Provider-Specific:**
- <a href="https://tailscale.com/pricing" target="_blank">Tailscale Pricing</a>
- <a href="https://tailscale.com/kb/1019/subnets" target="_blank">Tailscale Subnet Routers</a>
- <a href="https://www.twingate.com/pricing" target="_blank">Twingate Pricing</a>
- <a href="https://www.twingate.com/docs/connector-deployment" target="_blank">Twingate Connector Deployment</a>
- <a href="https://www.cloudflare.com/plans/zero-trust-services/" target="_blank">Cloudflare Zero Trust Pricing</a>
- <a href="https://www.zerotier.com/pricing/" target="_blank">ZeroTier Pricing</a>
- <a href="https://nordlayer.com/pricing/" target="_blank">NordLayer Pricing</a>
- <a href="https://blog.sonicwall.com/en-us/2024/07/give-a-ztna-boost-to-your-sonicwall-firewall/" target="_blank">SonicWall ZTNA Integration</a>

**Performance and Best Practices:**
- <a href="https://www.twingate.com/blog/ztna-reduces-network-latency" target="_blank">How ZTNA Reduces Network Latency</a>
- <a href="https://learn.microsoft.com/en-us/office/troubleshoot/access/optimize-odbc-data-sources" target="_blank">Microsoft: Optimize ODBC Data Sources</a>
- <a href="https://medium.com/@ngpiesco/performance-considerations-orms-odbc-and-database-drivers-5baf3b03922a" target="_blank">ODBC Performance Considerations</a>
- <a href="https://blog.openvpn.net/7-ztna-best-practices/" target="_blank">Zero Trust Best Practices: 7 ZTNA Tips for SMBs</a>
- <a href="https://versa-networks.com/blog/9-easy-steps-to-follow-when-migrating-from-a-legacy-vpn-to-a-ztna-solution/" target="_blank">Migrate From VPN to ZTNA in 9 Easy Steps</a>

**Comparisons:**
- <a href="https://www.twingate.com/blog/comparisons/tailscale-vs-zerotier" target="_blank">Tailscale vs ZeroTier Comparison</a>
- <a href="https://tailscale.com/compare/twingate" target="_blank">Tailscale vs Twingate</a>
- <a href="https://sourceforge.net/software/compare/Tailscale-vs-Twingate-vs-ZeroTier/" target="_blank">Multi-Provider Comparison</a>

---

**Document Version:** 1.0
**Last Updated:** February 6, 2026
**Next Review:** After clarifying questions answered and pilot testing completed
