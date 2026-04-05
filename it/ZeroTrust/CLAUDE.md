# CLAUDE.md (Zero Trust Remote Access Project)

This file provides Zero Trust project-specific guidance to Claude Code when working in this project.

> **Note**: This supplements the shared CLAUDE.md at `/terminai/CLAUDE.md` and the IT-specific CLAUDE.md at `/terminai/it/CLAUDE.md`. Read all three files for complete guidance.

## Project Overview

This project documents the implementation of **Zero Trust Network Access (ZTNA)** for secure remote database access without opening firewall ports. The goal is to enable controlled remote access to a PostgreSQL database running in a small office environment while maintaining a zero-trust security posture.

### Core Problem Statement
Traditional VPN and port-forwarding solutions require:
- Opening ports on the perimeter firewall (attack surface expansion)
- Broad network access once authenticated (lateral movement risk)
- Complex firewall rule management
- VPN client deployment and maintenance

### Zero Trust Solution Approach
Implement secure remote access using:
- **Software-Defined Perimeter (SDP)** or **ZTNA architecture**
- **Identity-centric access** (not network-centric)
- **Least-privilege access** (database only, not entire network)
- **No inbound firewall ports** (outbound-only connections)
- **Continuous verification** (not "trust once, access always")

## Environment Details

### Network Infrastructure
- **Perimeter Firewall**: SonicWall TZ 270
  - Current configuration: No inbound ports opened for database access
  - Management: SonicOS interface
  - Goal: Keep all inbound ports closed (except essential services)

### Database Server
- **Operating System**: Ubuntu 24.04 LTS (Server)
- **Database**: PostgreSQL (version TBD - document actual version)
- **Primary Purpose**: Data collection and controlled access for business users
- **Network Position**: Inside office LAN, behind SonicWall firewall
- **Current Access**: Local network only

### Client Devices
- **Device Type**: User-owned laptops
- **Operating Systems**: Windows 11 (assumed - verify actual deployment)
- **Identity Management**: Azure AD domain-joined
  - Users authenticate with Azure AD credentials
  - Conditional Access policies available
  - MFA enforcement (assumed - verify)

### Security Stack (Defense in Depth)
Current enterprise security tools deployed:

1. **ThreatLocker** (Application Allowlisting & Ringfencing)
   - Application whitelisting and control
   - Ringfencing to prevent lateral movement
   - Elevation control
   - Storage control

2. **Huntress** (Managed EDR/MDR)
   - Endpoint detection and response
   - Managed threat hunting
   - Persistent footholds detection
   - Ransomware canaries

3. **Microsoft Defender for Business**
   - Next-generation antivirus
   - Endpoint protection
   - Integration with Microsoft 365 Business Premium

4. **Microsoft 365 Business Premium**
   - Azure AD Premium P1 (conditional access, MFA)
   - Microsoft Defender for Office 365
   - Azure Information Protection
   - Intune device management

### Licensing and Available Features
With **M365 Business Premium**, the following Azure AD/Entra ID features are available:
- Conditional Access policies
- Multi-Factor Authentication (MFA)
- Azure AD Application Proxy (potential solution component)
- Self-service password reset
- Azure AD Connect (hybrid identity)
- Security defaults

## Zero Trust Principles (Applied to This Project)

### 1. Verify Explicitly
- Authenticate and authorize based on all available data points:
  - User identity (Azure AD)
  - Device compliance (Intune)
  - Location
  - Application
  - Data classification

### 2. Use Least Privilege Access
- Just-in-time (JIT) access where possible
- Just-enough-access (JEA) - database only, not entire network
- Risk-based adaptive policies
- Time-based access (if applicable)

### 3. Assume Breach
- Minimize blast radius with segmentation
- End-to-end encryption
- Analytics for threat detection
- Automated threat response

## Project Goals

### Primary Objectives
1. **Eliminate Firewall Port Exposure**: No inbound ports opened on SonicWall TZ 270
2. **Enable Secure Remote Database Access**: Authorized users can access PostgreSQL from anywhere
3. **Maintain Zero Trust Posture**: Identity-centric, least-privilege access
4. **Integrate with Existing Security Stack**: Work with ThreatLocker, Huntress, Defender
5. **Leverage Azure AD Identity**: Use existing Azure AD authentication

### Secondary Objectives
1. **Audit and Compliance**: Log all database access attempts and queries
2. **Conditional Access**: Enforce MFA, device compliance, location policies
3. **User Experience**: Transparent access without complex VPN clients
4. **Scalability**: Solution should scale as business grows
5. **Cost-Effectiveness**: Leverage existing M365 Business Premium licenses where possible

## Solution Architecture Candidates

### Option 1: Azure AD Application Proxy + Private Endpoint
- **Pros**: Included with M365 Business Premium, integrates with Azure AD
- **Cons**: Requires Azure-hosted resources, may need VNet configuration
- **Complexity**: Medium
- **Cost**: Potentially low (leverages existing licenses + Azure hosting costs)

### Option 2: Cloudflare Tunnel (Zero Trust)
- **Pros**: No inbound ports, free tier available, simple setup
- **Cons**: Third-party dependency, data flows through Cloudflare
- **Complexity**: Low
- **Cost**: Free tier available, paid plans for advanced features

### Option 3: Tailscale (WireGuard-based Mesh VPN)
- **Pros**: Peer-to-peer mesh, simple setup, ACL-based access control
- **Cons**: Still VPN-like (though more zero-trust), limited free tier
- **Complexity**: Low
- **Cost**: Free for personal use, paid for business (verify pricing)

### Option 4: Twingate (ZTNA Platform)
- **Pros**: Purpose-built ZTNA, granular access, no network changes
- **Cons**: Third-party SaaS, cost, requires deployment
- **Complexity**: Medium
- **Cost**: Paid service (pricing TBD)

### Option 5: Custom SSH Bastion + Azure AD Auth
- **Pros**: Full control, leverages SSH, can integrate Azure AD
- **Cons**: Requires bastion host maintenance, complex setup
- **Complexity**: High
- **Cost**: Low (self-hosted)

### Option 6: Azure Private Link + VPN Gateway
- **Pros**: Azure-native, secure, enterprise-grade
- **Cons**: Requires Azure hosting, VPN client still needed, costly
- **Complexity**: High
- **Cost**: High (VPN Gateway, Private Link pricing)

> **Research Required**: Detailed evaluation of each option against requirements, cost analysis, security assessment

## Technical Requirements

### Security Requirements
- [ ] No inbound firewall ports opened
- [ ] Azure AD authentication required
- [ ] MFA enforced for remote access
- [ ] Device compliance check (if feasible)
- [ ] Encrypted connections (TLS 1.3 preferred)
- [ ] Audit logging of all access attempts
- [ ] Compatible with ThreatLocker application control
- [ ] Compatible with Huntress EDR monitoring
- [ ] Least-privilege access (database only, specific tables if possible)

### Operational Requirements
- [ ] User-friendly access method (minimal training)
- [ ] Connection from Windows 11 laptops
- [ ] Support standard database tools (pgAdmin, DBeaver, psql, etc.)
- [ ] Reliable connectivity (99%+ uptime)
- [ ] Acceptable latency (<200ms for queries)
- [ ] Disaster recovery plan
- [ ] Documented troubleshooting procedures

### Compliance Requirements
- [ ] Log retention (define retention period)
- [ ] Access review process (quarterly/annual)
- [ ] Data residency (verify if applicable)
- [ ] GDPR compliance (if applicable)
- [ ] Change management process
- [ ] Security incident response plan

## Documentation Standards

### When Researching Solutions
Use the `gemini-it-security-researcher` agent for:
- ZTNA platform evaluations
- Security architecture best practices
- Azure AD integration patterns
- PostgreSQL security hardening
- Zero Trust implementation guides

### Cross-Reference Authoritative Sources
- **NIST SP 800-207**: Zero Trust Architecture
- **CISA Zero Trust Maturity Model**
- **NSA guidance on Zero Trust security**
- **Microsoft Zero Trust deployment guide**
- **PostgreSQL Security Documentation**
- **SonicWall TZ 270 Administration Guide**

### Documentation Structure
Each solution evaluation should include:
1. **Architecture Overview**: How it works
2. **Security Analysis**: Threat model, attack surface
3. **Integration Steps**: Detailed implementation guide
4. **Configuration Examples**: Real configs for Ubuntu, SonicWall, clients
5. **Cost Breakdown**: Licensing, hosting, operational costs
6. **Pros/Cons Analysis**: Honest assessment
7. **Testing Plan**: How to validate security and functionality
8. **Troubleshooting Guide**: Common issues and fixes

## Current Project Status

See `PROJECT_STATUS.md` for current phase and progress tracking.

## Project-Specific Agents

This project has specialized agents available:
- `gemini-it-security-researcher` - Expert research agent for Zero Trust, ZTNA, security architectures
- `gemini-researcher` - General web research for product comparisons, pricing, vendor analysis

## User Preferences

Based on IT project patterns:
- Prefers comprehensive technical documentation with practical implementation steps
- Values security-first approach with honest risk assessment
- Appreciates cost-benefit analysis with verified pricing
- Wants both theoretical understanding (Zero Trust principles) and hands-on guides
- Environment: Windows 11 clients, Ubuntu 24.04 server, Azure AD identity

## Next Steps

1. **Environment Discovery**:
   - Document actual PostgreSQL version and current configuration
   - Document current SonicWall TZ 270 configuration
   - Document Azure AD tenant configuration (conditional access policies, MFA status)
   - Document current database access patterns (who, from where, how often)

2. **Solution Research**:
   - Deep-dive research on top 3 solution candidates
   - Security audit reports for selected solutions
   - Cost analysis with actual pricing
   - Proof-of-concept planning

3. **Implementation Planning**:
   - Select solution based on requirements matrix
   - Create detailed implementation plan
   - Design testing and validation procedures
   - Document rollback procedures

## Related Documentation

- `/terminai/CLAUDE.md` - Shared guidance for all projects
- `/terminai/it/CLAUDE.md` - IT-specific guidance
- `/terminai/it/VPN_Benefits.md` - VPN security analysis (contrast with ZTNA)
- `/terminai/it/L2TP_over_IPsec.md` - Traditional VPN approach (what we're moving away from)
