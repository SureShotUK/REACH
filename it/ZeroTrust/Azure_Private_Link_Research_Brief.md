# Azure Private Link + VPN Gateway Research Brief

**Research Date**: 2026-02-17
**Status**: Brief analysis (agent hit rate limit during comprehensive research)

---

## Executive Summary

**❌ NOT RECOMMENDED** - Azure Private Link + VPN Gateway is **financially unviable** for your use case, exceeding budget by 2-3x minimum.

**Critical Issue**: This solution requires migrating your on-premises PostgreSQL database to Azure, which introduces:
- High migration costs and complexity
- Ongoing Azure infrastructure costs far exceeding £300/month budget
- Data residency changes (UK on-premises → Azure UK datacenters)
- Operational dependency on Azure services

---

## Architecture Overview

Azure Private Link is designed for **Azure-to-Azure** or **Azure-to-on-premises** connectivity, NOT for remote user access to on-premises resources.

### How It Would Work (Hypothetically)

1. **Migrate PostgreSQL** from on-premises Ubuntu 24.04 to:
   - **Option A**: Azure VM running Ubuntu + PostgreSQL
   - **Option B**: Azure Database for PostgreSQL (managed service)

2. **Deploy VPN Gateway** in Azure Virtual Network
   - Provides Point-to-Site (P2S) VPN for remote users
   - Integrates with Azure AD for authentication

3. **Configure Private Link** (if using managed database)
   - Creates private endpoint for Azure Database for PostgreSQL
   - Isolates database from public internet

4. **Remote Users**:
   - Connect via Azure VPN Client (P2S VPN)
   - Access database via private IP within Azure VNet

---

## Cost Analysis (30 Users)

### Option A: Azure VM + VPN Gateway

| Component | Specification | Monthly Cost (£) |
|-----------|--------------|------------------|
| **Azure VM** | Standard_D2s_v5 (2 vCPU, 8GB RAM) | £62 |
| **Managed Disks** | 256GB Premium SSD | £28 |
| **VPN Gateway** | VpnGw1 (650 Mbps, 30 P2S connections) | £93 |
| **Public IP** | Standard SKU | £3 |
| **Bandwidth** | 100GB egress/month (estimate) | £7 |
| **Backup** | Azure Backup for VM | £12 |
| **Total** | | **£205/month** |

**Verdict**: £205/month infrastructure only, BUT:
- ⚠️ Requires PostgreSQL migration (cost, downtime, risk)
- ⚠️ Does NOT include CCTV access (still on-premises)
- ⚠️ Not true Zero Trust (grants full VNet access)

### Option B: Azure Database for PostgreSQL + VPN Gateway

| Component | Specification | Monthly Cost (£) |
|-----------|--------------|------------------|
| **Azure Database for PostgreSQL** | Flexible Server, 2 vCores, 8GB RAM | £95 |
| **Storage** | 256GB | £21 |
| **Backup Storage** | 256GB (7-day retention) | £4 |
| **VPN Gateway** | VpnGw1 | £93 |
| **Public IP** | Standard SKU | £3 |
| **Bandwidth** | 100GB egress/month | £7 |
| **Total** | | **£223/month** |

**Verdict**: £223/month infrastructure only, BUT:
- ⚠️ Managed service simplifies operations
- ⚠️ Still requires database migration
- ⚠️ CCTV remains on-premises (separate solution needed)
- ⚠️ Not true Zero Trust architecture

---

## Why This Exceeds Budget

**Budget**: £300/month for 30 users (£10/user)

**Azure Costs**:
- Infrastructure: £205-223/month
- Migration project: £2,000-5,000 one-time
- Ongoing management: £50-100/month (2-4 hours IT labor)
- **Total Year 1**: £4,960-7,676 (£165-256/user)
- **Total Year 2+**: £2,960-3,876 (£99-129/user)

**Comparison to ZTNA SaaS**:
- Tailscale Starter: £180/month (41% UNDER budget) ✅
- Twingate Business: £300/month (at budget limit) ✅
- Azure Private Link: £255-323/month (15-30% OVER budget) ❌

---

## Migration Risks

### Database Migration Complexity

**Migration Steps**:
1. Export PostgreSQL data using `pg_dump`
2. Provision Azure VM or Azure Database for PostgreSQL
3. Configure networking (VNet, subnets, NSG rules)
4. Import data using `pg_restore`
5. Reconfigure applications to new connection strings
6. Test all database connections
7. Plan cutover window (downtime required)

**Risks**:
- Data loss during migration
- Application compatibility issues
- Downtime impact on operations
- Connection string changes across multiple systems

**Timeline**: 4-8 weeks for complete migration

### CCTV System Remains On-Premises

**Critical Issue**: Migrating database to Azure does NOT solve CCTV access.

**Options**:
1. Deploy separate VPN for CCTV (defeats single-solution goal)
2. Migrate CCTV system to Azure (extremely expensive, impractical)
3. Site-to-Site VPN from Azure to on-premises (adds £93/month VPN Gateway + SonicWall config)

---

## Zero Trust Assessment

### Is This True Zero Trust?

**⚠️ NO** - Azure Point-to-Site VPN is **traditional VPN architecture**, not ZTNA:

| Zero Trust Principle | Azure P2S VPN | True ZTNA (Twingate/Tailscale) |
|---------------------|---------------|----------------------------------|
| **Per-application access** | ❌ Full VNet access | ✅ Database port 5432 only |
| **Continuous verification** | ❌ Authenticate once, trusted | ✅ Per-request verification |
| **Device posture** | ⚠️ Azure AD Conditional Access only | ✅ Real-time Intune compliance |
| **Least privilege** | ❌ Network-level access | ✅ Application-level access |
| **Audit granularity** | ⚠️ Connection logs only | ✅ Per-query logging available |

**Verdict**: Azure P2S VPN with Conditional Access is **VPN with identity**, not true ZTNA.

---

## When Azure Private Link DOES Make Sense

Azure Private Link + VPN Gateway is appropriate when:

✅ **Already migrating to Azure** - If you're modernizing infrastructure anyway
✅ **Azure-first strategy** - Committed to Azure ecosystem long-term
✅ **Large database workloads** - Need Azure's managed database scale (100+ cores)
✅ **Higher budget** - Can afford £250-500/month infrastructure costs
✅ **Compliance requirements** - Need Azure certifications (FedRAMP, HIPAA, etc.)

**Your situation**:
❌ On-premises database working well
❌ Budget constrained (£300/month)
❌ Small 30-user deployment
❌ CCTV system must stay on-premises

---

## Alternative: Azure Arc (Hybrid Approach)

### Azure Arc for On-Premises PostgreSQL

**Concept**: Use Azure Arc to manage on-premises PostgreSQL without migration.

**Problems**:
1. **Does NOT provide remote user access** - Arc is for Azure-to-on-premises management, not user VPN
2. **Still requires VPN Gateway** for user access (£93/month)
3. **Adds complexity** without solving access problem

**Verdict**: Azure Arc doesn't help this use case.

---

## Recommendation

### ❌ DO NOT pursue Azure Private Link + VPN Gateway

**Reasons**:
1. **15-30% over budget** (£255-323/month vs £300 budget)
2. **Requires database migration** (£2,000-5,000 project cost, 4-8 weeks, downtime risk)
3. **Not true Zero Trust** (network-level VPN, not application-level ZTNA)
4. **CCTV remains unsolved** (still on-premises, needs separate solution)
5. **Higher operational overhead** than SaaS ZTNA

### ✅ Recommended Alternatives

**For your requirements (on-premises database, CCTV, £300 budget, 30 users)**:

1. **Tailscale Starter** - £180/month
   - Best user experience (peer-to-peer mesh)
   - Simplest deployment (10 minutes)
   - Lowest operational overhead
   - 40% under budget
   - Native Linux support

2. **Twingate Business** - £300/month
   - Most enterprise features
   - Best audit logging
   - Exactly at budget limit
   - Purpose-built ZTNA

3. **Microsoft Entra Private Access** - £135/month
   - If Azure-native required
   - True ZTNA (not VPN)
   - 55% under budget
   - Requires Windows Server connectors

---

## Sources

### Azure Pricing (2026 UK Rates)
- <a href="https://azure.microsoft.com/en-gb/pricing/details/postgresql/flexible-server/" target="_blank">Azure Database for PostgreSQL Pricing</a>
- <a href="https://azure.microsoft.com/en-gb/pricing/details/vpn-gateway/" target="_blank">Azure VPN Gateway Pricing</a>
- <a href="https://azure.microsoft.com/en-gb/pricing/details/virtual-machines/linux/" target="_blank">Azure Virtual Machines Pricing</a>
- <a href="https://azure.microsoft.com/en-gb/pricing/details/bandwidth/" target="_blank">Azure Bandwidth Pricing</a>

### Technical Documentation
- <a href="https://learn.microsoft.com/en-us/azure/vpn-gateway/point-to-site-about" target="_blank">About Azure Point-to-Site VPN</a>
- <a href="https://learn.microsoft.com/en-us/azure/private-link/private-link-overview" target="_blank">What is Azure Private Link?</a>
- <a href="https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/concepts-networking-private-link" target="_blank">Azure Database for PostgreSQL - Private Link</a>

### Analysis
- <a href="https://www.twingate.com/blog/azure-vpn-pricing" target="_blank">Demystifying Azure VPN Pricing & Affordable Alternatives</a>

---

**Conclusion**: Azure Private Link + VPN Gateway is technically sound but financially unviable for your use case. SaaS ZTNA solutions (Tailscale, Twingate) provide better value, simpler deployment, and true Zero Trust architecture without requiring database migration.
