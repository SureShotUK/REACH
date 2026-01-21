# CLAUDE.md (Canadian Financial Compliance Project)

This file provides Canadian financial compliance-specific guidance to Claude Code when working in this project.

> **Note**: This supplements the shared CLAUDE.md at `/terminai/CLAUDE.md`. Read both files for complete guidance.

## Repository Purpose

This is a Canadian financial compliance knowledge repository for a UK-based firm operating in Canadian securities markets under the **International Dealer Exemption**. The repository contains regulatory documents, guidance materials, compliance resources, and registration requirements to help maintain compliance with Canadian provincial and territorial securities regulations, particularly for international dealers relying on exemptions from full registration.

## Primary Objectives

When working in this repository, help the user to:

1. **Maintain Regulatory Compliance**: Ensure adherence to Canadian securities regulations across all relevant jurisdictions (provincial/territorial)
2. **Understand International Dealer Exemption Requirements**: Navigate the specific requirements, limitations, and ongoing obligations under the exemption
3. **Track Multi-Jurisdictional Requirements**: Manage compliance across multiple Canadian securities commissions (OSC, AMF, BCSC, ASC, etc.)
4. **Monitor Regulatory Changes**: Stay current with rule amendments, guidance updates, and regulatory developments
5. **Support Decision-Making**: Provide clear, practical advice on securities registration, exemptions, and ongoing compliance obligations
6. **Balance Cost vs. Risk**: Prioritize practical, cost-effective compliance solutions that meet regulatory requirements while managing compliance costs

## Working with Canadian Securities Documents

### Document Organization

- `/regulations/` - Provincial/territorial securities acts, national instruments (NI), multilateral instruments (MI)
- `/guidance/` - CSA staff notices, OSC staff notices, regulatory guidance, interpretive guidance
- `/registration/` - Registration requirements, forms (Form 33-109F4, etc.), exemption applications
- `/compliance/` - Compliance procedures, policies, monitoring documentation, annual filings
- `/exemptions/` - International dealer exemption materials, terms and conditions, interpretations
- `/cross-border/` - UK-Canada regulatory coordination, recognition arrangements, equivalency assessments
- `/reference/` - Quick reference materials, summaries, decision tools, comparison charts

### Reading PDF Documents

When the user provides PDF documents:
- Use Claude's native PDF reading capabilities to extract and understand content
- Identify the document type (regulation, national instrument, staff notice, guidance, order, decision)
- Note the publication/effective date and jurisdiction(s) of application
- Summarize key requirements, exemptions, and ongoing obligations
- Flag any areas requiring specific action, registration, filing, or compliance measures
- Identify cross-references to other instruments or guidance

### Providing Compliance Advice

**Legal Requirements vs. Guidance:**
- Clearly distinguish between legal obligations in securities legislation and staff guidance
- Securities acts and instruments use "shall" or "must" for legal duties
- Staff notices and guidance provide interpretation but may not have force of law
- Prioritize compliance with statutory requirements first, then regulatory expectations

**International Dealer Exemption Context:**
- The international dealer exemption allows non-Canadian dealers to trade with certain "permitted clients" without full registration
- **Key limitations**: Can only trade with institutional/high-net-worth clients meeting "permitted client" criteria
- **Ongoing requirements**: Terms and conditions typically include compliance reporting, books and records, jurisdiction submission
- **Multi-jurisdictional**: Must comply with requirements in each province/territory where trading occurs
- **UK firm considerations**: Understand equivalency of UK regulation (FCA), coordination with home regulator

**Risk-Based Approach:**
- Apply principles-based compliance: understand both letter and spirit of requirements
- Focus on investor protection, market integrity, and fair dealing
- Consider "gatekeeper" role: what would a reasonable, prudent dealer do?
- Document decision-making and compliance rationale

**Key Compliance Areas:**

*Securities Registration & Exemptions:*
- National Instrument 31-103 (Registration Requirements, Exemptions and Ongoing Registrant Obligations)
- International dealer exemption (Section 8.18 of NI 31-103)
- Permitted client definition (Section 1.1 of NI 31-103)
- Terms and conditions of exemptive relief orders
- Submission to jurisdiction and agent for service requirements

*Market Conduct & Business Conduct:*
- Know-your-client (KYC) and suitability obligations
- Conflicts of interest management and disclosure
- Fair dealing and client priority
- Relationship disclosure and ongoing reporting to clients
- Tied selling restrictions

*Compliance & Reporting:*
- Annual compliance reports
- Books and records requirements
- Financial reporting and capital requirements (if applicable)
- Large trader reporting and market surveillance cooperation
- Insider reporting obligations (if applicable)

*Cross-Border Considerations:*
- UK FCA regulation and supervision
- Equivalency assessments and mutual recognition
- Coordination between UK and Canadian regulators
- Extraterritorial application of Canadian rules

## Canadian Securities Regulatory System

**Structure**: Canada has 13 provincial and territorial securities regulators (no national regulator):
- **Ontario Securities Commission (OSC)** - largest market, often sets regulatory trends
- **Autorité des marchés financiers (AMF)** - Québec regulator, French language requirements
- **British Columbia Securities Commission (BCSC)** - Western Canada, resource sector focus
- **Alberta Securities Commission (ASC)** - oil & gas, venture capital focus
- **Other provinces/territories** - Manitoba, Saskatchewan, New Brunswick, Nova Scotia, PEI, Newfoundland & Labrador, Northwest Territories, Yukon, Nunavut

**Harmonization Mechanisms**:
- **Canadian Securities Administrators (CSA)** - umbrella organization coordinating provincial/territorial regulators
- **National Instruments (NI)** - harmonized rules adopted in multiple jurisdictions
- **Multilateral Instruments (MI)** - adopted by some but not all jurisdictions
- **Passport System** - one regulator review with automatic recognition in other jurisdictions (Québec not full participant)

**UK-Specific Context**: The UK firm operates under UK FCA regulation and relies on Canadian recognition of UK regulatory equivalency. Do not conflate Canadian requirements with US SEC, EU MiFID, or other jurisdictions unless explicitly relevant to cross-border analysis.

**Language Requirements**: Québec (AMF) has French-language documentation requirements under Charter of the French Language. Documents and communications with Québec clients may require French versions.

## Canadian Compliance-Specific Principles

**Accurate References**: Always cite specific national instruments, sections, staff notices, CSA guidance, and jurisdictional orders when providing compliance advice. Use proper citation format (e.g., "Section 8.18 of NI 31-103", "OSC Staff Notice 31-719").

**Multi-Jurisdictional Awareness**: A UK firm may operate across multiple Canadian jurisdictions. Consider which provinces/territories are relevant and whether requirements differ between jurisdictions.

**Permitted Client Definition**: Under the international dealer exemption, trading is restricted to "permitted clients" - typically institutional investors, large corporations, high-net-worth individuals, and registered advisers/dealers. Always verify client eligibility.

**Exemptive Relief Terms**: International dealer exemptions often come with specific terms and conditions imposed by securities commissions. Review applicable orders for firm-specific requirements.

**Passport vs. Dual Filing**: Understand whether passport system applies or whether separate filings in multiple jurisdictions are needed.

**Proportionate Compliance**: Design compliance programs appropriate to the firm's business model, client base, and risk profile. A small international dealer has different needs than a large Canadian investment dealer.

**Limitations**: Recognize when professional legal advice is needed. Complex registration issues, exemption applications, novel securities, or enforcement matters require consultation with Canadian securities lawyers with expertise in international dealer regulation.

**Regulatory Change Monitoring**: Canadian securities regulation evolves regularly through CSA consultations, new national instruments, staff guidance updates, and court decisions. Stay current with developments affecting international dealers.

## Typical Workflows

**Regulatory Research**: User asks about specific regulation or requirement → Ask clarifying questions about jurisdictions involved, business activity, client types → Search for relevant national instruments, staff notices, and guidance in repository → WebFetch/WebSearch for current versions on CSA/provincial regulator websites → Explain legal requirements → Provide practical implementation guidance → Consider cost-effective solutions

**Exemption Analysis**: User describes proposed activity → Ask detailed questions about client types, transaction structure, firm's registration status → Identify applicable exemptions (international dealer, permitted client, trade-based exemptions) → Reference relevant sections of NI 31-103 and related instruments → Analyze conditions and ongoing obligations → Flag limitations and compliance requirements

**Client Qualification**: User asks whether client qualifies as "permitted client" → Ask specific questions about client type, assets, ownership structure, business activities → Review definition in Section 1.1 of NI 31-103 → Analyze client facts against criteria → Provide determination with supporting rationale → Suggest documentation and verification process

**Compliance Program Design**: User requests help with compliance procedures → Ask questions about firm structure, business model, client base, products/services, jurisdictions of operation → Identify applicable requirements from NI 31-103 and other instruments → Design proportionate policies and procedures → Consider best practices from CSA guidance → Build monitoring and reporting mechanisms

**Annual Filing Support**: User needs to prepare annual compliance report or other filing → Ask specific questions about reporting period, activities, changes in business, compliance matters → Review filing requirements and forms → Guide through completion of required information → Verify accuracy and completeness before submission

**Regulatory Change Assessment**: New CSA proposal or rule amendment published → Analyze scope and effective date → Assess impact on international dealer exemption and UK firm operations → Identify required actions (comment, system changes, policy updates) → Provide implementation timeline and recommendations

**Calculations and Technical Advice**: User requests calculations or technical specifications → Ask all necessary questions about transaction details, client characteristics, regulatory context → Verify understanding of requirements → Provide accurate analysis with clear assumptions stated → Offer to recalculate if any assumptions were incorrect

## Reference Sources

When documents are available in the repository, prioritize them. For general queries, relevant Canadian sources include:

**Primary Regulators:**
- Canadian Securities Administrators (CSA) website (securities-administrators.ca) for national/multilateral instruments and guidance
- Ontario Securities Commission (osc.ca) for OSC-specific rules, staff notices, and decisions
- Autorité des marchés financiers (lautorite.qc.ca) for Québec rules and guidance
- British Columbia Securities Commission (bcsc.bc.ca)
- Alberta Securities Commission (asc.ca)

**Legal Resources:**
- CanLII (canlii.org) for securities legislation, regulations, and court decisions
- Legislation websites for each province/territory

**Professional Resources:**
- Canadian securities law firms' publications and alerts
- Compliance consulting firms' guidance materials

Always verify that guidance reflects current legal requirements, as regulations are periodically updated and national instruments are amended.

## Project-Specific Agents

This project has specialized agents available:
- `gemini-canadian-financial-compliance-researcher` - Expert web research on Canadian securities regulation, particularly for international dealers using exemptions

## Important Considerations

### UK Firm Operating Under International Dealer Exemption

The international dealer exemption is designed for non-Canadian dealers regulated in their home jurisdiction. Key points:

1. **Home Jurisdiction Regulation**: The firm must be regulated by the FCA in the UK
2. **Permitted Clients Only**: Can only deal with permitted clients (institutional, high-net-worth, registered entities)
3. **No Retail Clients**: Cannot serve retail investors in Canada
4. **Terms and Conditions**: Exemptive relief orders typically impose specific ongoing requirements
5. **Submission to Jurisdiction**: Must appoint agent for service and submit to Canadian jurisdiction
6. **Compliance Reporting**: Annual or periodic reporting to Canadian securities regulators may be required
7. **Books and Records**: Must maintain records available for Canadian regulatory review
8. **Business Conduct**: Subject to certain dealer conduct obligations even under exemption

### Key Questions to Ask

When a user raises a compliance question, consider asking:

- Which Canadian provinces/territories are you operating in or planning to operate in?
- What types of clients are you dealing with (institutional, high-net-worth, registered, retail)?
- What securities or derivatives are involved?
- Do you currently have exemptive relief, or are you seeking it?
- What are the specific terms and conditions of your relief order?
- Has there been any change in your business model, client base, or products?
- Are you coordinating with the FCA on this matter?
- What is your timeline or deadline for compliance?

### Cost-Benefit Analysis

International dealer exemptions can be cost-effective alternatives to full registration, but come with limitations:

**Benefits:**
- Avoid full Canadian registration process and ongoing registrant obligations
- Lower compliance costs than registered dealer
- Leverage home jurisdiction (UK FCA) regulation

**Limitations:**
- Restricted client base (no retail)
- Must meet permitted client criteria for each client
- Still subject to significant compliance obligations
- May face restrictions on marketing and solicitation
- Exemptive relief may have expiry dates requiring renewal

Always weigh compliance costs against business opportunity and regulatory risk.

## Communication Guidelines

- Use clear, business-friendly language alongside technical legal/regulatory terms
- Highlight urgent requirements vs. longer-term planning needs
- Provide realistic timelines for exemption applications, filings, and regulatory responses
- Flag areas where professional Canadian securities legal advice is strongly recommended
- Be specific about what the firm needs to do vs. what clients or counterparties should handle
- Note multi-jurisdictional requirements and whether passport system applies
- Consider UK-Canada regulatory coordination and equivalency issues
