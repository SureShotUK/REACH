# Zero Trust Remote Database Access

## Overview

This project documents the implementation of **Zero Trust Network Access (ZTNA)** to enable secure remote access to a PostgreSQL database without opening inbound firewall ports.

## Environment

- **Firewall**: SonicWall TZ 270
- **Database Server**: Ubuntu 24.04 LTS running PostgreSQL
- **Client Devices**: Azure AD-joined Windows 11 laptops
- **Security Stack**: ThreatLocker, Huntress, Microsoft Defender for Business
- **Licensing**: Microsoft 365 Business Premium

## Problem Statement

Traditional remote database access requires:
- Opening firewall ports (security risk)
- VPN configuration (complex, broad network access)
- Attack surface expansion

## Zero Trust Approach

Implement identity-centric, least-privilege access:
- **No inbound firewall ports** opened
- **Azure AD authentication** (existing identity provider)
- **Database-only access** (not entire network)
- **Continuous verification** (MFA, device compliance)

## Solution Candidates

| Solution | Complexity | Cost | Zero Trust Alignment |
|----------|-----------|------|---------------------|
| Azure AD App Proxy | Medium | Low (included in M365) | High |
| Cloudflare Tunnel | Low | Free/Paid tiers | High |
| Tailscale | Low | Paid for business | Medium |
| Twingate | Medium | Paid | Very High |
| SSH Bastion + Azure AD | High | Low (self-hosted) | Medium |
| Azure Private Link | High | High | High |

> See `CLAUDE.md` for detailed evaluation criteria and requirements.

## Project Goals

1. **Eliminate firewall port exposure** on SonicWall TZ 270
2. **Enable secure remote database access** for authorized users
3. **Integrate with Azure AD** for authentication
4. **Maintain least-privilege access** (database only)
5. **Audit all access** for compliance and security

## Getting Started

1. **Read the documentation**:
   - `CLAUDE.md` - Comprehensive project guidance
   - `PROJECT_STATUS.md` - Current project phase
   - `SESSION_LOG.md` - Work history

2. **Environment discovery** (first phase):
   - Document PostgreSQL version and configuration
   - Document SonicWall current ruleset
   - Document Azure AD tenant configuration
   - Document current database access patterns

3. **Solution research** (second phase):
   - Evaluate top 3 solutions against requirements
   - Security analysis and threat modeling
   - Cost-benefit analysis
   - Proof-of-concept planning

## Key Principles

### Zero Trust Pillars
1. **Verify Explicitly**: Authenticate and authorize using all available data
2. **Least Privilege Access**: Just-in-time, just-enough-access
3. **Assume Breach**: Minimize blast radius, segment access, encrypt

### Security Requirements
- Azure AD authentication (MFA enforced)
- No inbound firewall ports
- Encrypted connections (TLS 1.3)
- Audit logging
- Compatible with existing security stack

## Documentation

All documentation follows IT project standards:
- Comprehensive technical explanations
- Step-by-step implementation guides
- Configuration examples
- Troubleshooting procedures
- Security considerations
- Cost analysis

## Resources

### Authoritative Guidance
- <a href="https://csrc.nist.gov/publications/detail/sp/800-207/final" target="_blank">NIST SP 800-207: Zero Trust Architecture</a>
- <a href="https://www.cisa.gov/zero-trust-maturity-model" target="_blank">CISA Zero Trust Maturity Model</a>
- <a href="https://learn.microsoft.com/en-us/security/zero-trust/" target="_blank">Microsoft Zero Trust Guidance</a>
- <a href="https://www.postgresql.org/docs/current/security.html" target="_blank">PostgreSQL Security Documentation</a>

### Related IT Documentation
- `../VPN_Benefits.md` - Traditional VPN analysis (for comparison)
- `../L2TP_over_IPsec.md` - Traditional VPN approach
- `../WIFI_Best_Practices_for_Laptops_and_Mobiles.md` - Client security

## Status

**Current Phase**: Project initialization and environment discovery

See `PROJECT_STATUS.md` for detailed status tracking.

## Contact

For questions or clarifications about requirements, use the AskUserQuestion tool to gather complete information before providing guidance.
