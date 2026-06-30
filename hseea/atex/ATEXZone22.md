# ATEX Zone 22: Electronics, Drones and Cameras in Grain/Flour/Starch Warehouses

*Research reference — Portland Fuel | May 2026*

---

## Contents

1. [Legal Framework](#1-legal-framework)
2. [Zone Classification](#2-zone-classification)
3. [Grain/Flour/Starch Dust — Hazard Characterisation](#3-grainflourpowder-dust--hazard-characterisation)
4. [ATEX Equipment Categories](#4-atex-equipment-categories)
5. [Ignition Sources — Why Standard Electronics Are Prohibited](#5-ignition-sources--why-standard-electronics-are-prohibited)
6. [Mobile Phones and Tablets in Zone 22](#6-mobile-phones-and-tablets-in-zone-22)
7. [Cameras in Zone 22](#7-cameras-in-zone-22)
8. [Drones in Zone 22 — The Core Challenge](#8-drones-in-zone-22--the-core-challenge)
9. [Explosion Protection Document](#9-explosion-protection-document)
10. [Practical Guidance — One-Off Insurance/Compliance Survey](#10-practical-guidance--one-off-insurancecompliance-survey)
11. [Key Standards and References](#11-key-standards-and-references)

---

## 1. Legal Framework

Two sets of UK regulations govern ATEX Zone 22 compliance. They implement what were formerly EU ATEX Directives and remain in force post-Brexit.

### Dangerous Substances and Explosive Atmospheres Regulations 2002 (DSEAR)

<a href="https://www.legislation.gov.uk/uksi/2002/2776/contents" target="_blank">SI 2002/2776 — Dangerous Substances and Explosive Atmospheres Regulations 2002</a>

DSEAR places duties on **employers** to manage risks from explosive atmospheres in the workplace. Key employer duties under DSEAR:

- **Eliminate or reduce** the risk from dangerous substances
- **Carry out a risk assessment** of all dangerous substances and explosive atmosphere hazards
- **Classify hazardous areas** into zones based on likelihood and persistence of explosive atmosphere
- **Prepare an Explosion Protection Document** (EPD) — see section 9
- **Ensure equipment** used in classified zones is appropriate for the zone
- **Provide information, instruction and training** to employees working in or near hazardous zones
- **Coordinate** with any contractors working in the zone

DSEAR applies throughout Great Britain. The equivalent in Northern Ireland is the Dangerous Substances and Explosive Atmospheres Regulations (Northern Ireland) 2003.

The HSE's Approved Code of Practice (ACOP) L138 provides statutory guidance:
<a href="https://www.hse.gov.uk/pubns/books/l138.htm" target="_blank">HSE L138: Dangerous Substances and Explosive Atmospheres — ACOP and Guidance</a>

### Equipment and Protective Systems Intended for Use in Potentially Explosive Atmospheres Regulations 2016

<a href="https://www.legislation.gov.uk/uksi/2016/1107/contents" target="_blank">SI 2016/1107 — EPS Regulations 2016</a>

These regulations govern the **certification and marking of equipment** for use in explosive atmospheres. They apply to manufacturers and suppliers placing equipment on the market. Equipment used in ATEX zones must bear the appropriate conformity marking.

### Post-Brexit: UKCA vs CE ATEX Marking

| Situation | Marking Required |
|-----------|-----------------|
| Equipment placed on GB market from 1 Jan 2021 | UKCA marking required |
| Equipment with EU CE ATEX marking placed on market before 1 Jan 2021 | CE marking remains valid |
| Equipment sold into EU market | EU CE ATEX marking (ATEX Directive 2014/34/EU) |

In practice, most currently available certified equipment carries both CE and UKCA markings. Either is acceptable for equipment placed on the GB market within the relevant transition window.

Further detail on DSEAR requirements:
<a href="https://www.hse.gov.uk/fireandexplosion/dsear.htm" target="_blank">HSE: The Dangerous Substances and Explosive Atmospheres Regulations</a>

---

## 2. Zone Classification

### Dust Zones (20, 21, 22)

For combustible dust atmospheres, three zone classifications apply. These mirror the gas/vapour zone system (0, 1, 2) but are designated with the prefix 2.

| Zone | Definition | Typical Duration | Dust Example |
|------|-----------|-----------------|--------------|
| **Zone 20** | Explosive dust cloud present **continuously** or for **long periods** | >1,000 hrs/yr | Inside grain silos, inside flour hoppers during operation |
| **Zone 21** | Explosive dust cloud **likely** in normal operation | 10–1,000 hrs/yr | Immediate vicinity of filling/emptying points, conveyor transfer points |
| **Zone 22** | Explosive dust cloud **not likely** in normal operation, or if it occurs will **persist for only a short period** | <10 hrs/yr | Storage warehouse interiors, roof structure areas where dust accumulates |

**Important:** Zone 22 also covers locations where dust layers accumulate and **could be disturbed into an explosive cloud**. A warehouse interior where grain or flour dust has settled on beams, purlins, and roof cladding is a Zone 22 environment — even if airborne dust is rarely seen during normal operations.

### Comparison with Gas/Vapour Zones

| Gas Zone | Dust Zone | Explosive Atmosphere Likelihood |
|----------|-----------|--------------------------------|
| Zone 0 | Zone 20 | Continuous / long periods |
| Zone 1 | Zone 21 | Likely in normal operation |
| Zone 2 | Zone 22 | Unlikely or short periods only |

### Applicable Standard

Area classification for explosive dust atmospheres is governed by:
**BS EN 60079-10-2**: Explosive atmospheres — Classification of areas — Explosive dust atmospheres

<a href="https://www.hse.gov.uk/fireandexplosion/atex.htm" target="_blank">HSE: ATEX and Explosive Atmospheres — Overview</a>

---

## 3. Grain/Flour/Starch Dust — Hazard Characterisation

### ATEX Dust Group

ATEX equipment groups for surface industry applications:

| Group | Dust Type | Examples |
|-------|-----------|---------|
| **IIIA** | Combustible flyings | Cotton fibres, wood shavings, hemp |
| **IIIB** | Non-conductive dust | **Grain, flour, starch, sugar, cocoa, dried milk, sawdust, plastic powder** |
| **IIIC** | Conductive dust | Aluminium, magnesium, zinc, carbon |

**Grain, flour and starch = Group IIIB (non-conductive dust)**

This determines which equipment can be used: equipment certified for Group IIIC (conductive dust) can be used with IIIB dusts, but IIIB-only equipment must not be used with IIIC dusts.

### Explosive Properties of Grain/Flour/Starch Dusts

| Property | Wheat Flour | Grain Dust | Cornstarch |
|----------|-------------|------------|------------|
| Minimum Explosible Concentration (MEC) | ~50 g/m³ | ~40–60 g/m³ | ~40 g/m³ |
| Minimum Ignition Energy (MIE) | ~50 mJ | ~100 mJ | ~30–40 mJ |
| Minimum Ignition Temperature — MIT (dust cloud) | ~380–420°C | ~380°C | ~410°C |
| Minimum Ignition Temperature — MIT (dust layer) | ~250–290°C | ~240–270°C | ~270°C |
| Temperature Class | T3 (max 200°C) | T3 (max 200°C) | T3 (max 200°C) |

The **Minimum Ignition Temperature (MIT) of the dust layer (~240–290°C)** is the most critical figure for electronics assessment — it is the lowest surface temperature at which settled dust will ignite on contact. A device whose surface can exceed this temperature (e.g., an overheating processor or battery) could ignite a dust layer even in Zone 22.

### Why Grain/Flour Warehouses Are Typically Zone 22

In a grain or flour storage warehouse:

- **Active loading/unloading and conveying areas** are typically Zone 20 or Zone 21 (dust cloud likely during operation)
- **The storage warehouse interior, particularly the roof structure and upper levels**, is typically Zone 22 — dust settles on horizontal surfaces (beams, purlins, ledges) and is not normally airborne, but could be disturbed into an explosive cloud by air movement, vibration, mechanical impact, or cleaning activities

The key risk during a roof structure survey is that drone **rotor wash** or other disturbance (e.g., dislodging dust with survey equipment) could briefly elevate the local condition from Zone 22 to Zone 21.

HSE guidance on grain dust:
<a href="https://www.hse.gov.uk/agriculture/topics/grain-dust.htm" target="_blank">HSE: Grain Dust</a>

HSE guidance on dust explosions in the food industry:
<a href="https://www.hse.gov.uk/food/safety-hazards/dustexplosion.htm" target="_blank">HSE: Prevention of Dust Explosions in the Food Industry</a>

---

## 4. ATEX Equipment Categories

Equipment certified for use in explosive dust atmospheres is categorised as 1D, 2D, or 3D. The category determines which zones the equipment may be used in.

| Category | Zone Permitted | Protection Level | Description |
|----------|---------------|-----------------|-------------|
| **1D** | Zone 20, 21, 22 | Very High | Two independent protective measures; safe even if two faults occur simultaneously |
| **2D** | Zone 21, 22 | High | One protective measure; safe even if equipment develops a fault |
| **3D** | Zone 22 only | Normal | Does not produce ignition sources during normal operation |

**Minimum category for Zone 22: Category 3D**

Category 2D and 1D equipment may also be used in Zone 22 — they offer a higher level of protection and are interchangeable where the zone permits.

### Understanding the ATEX Equipment Marking

A typical ATEX marking for Zone 22 equipment appears as:

```
⬡ II 3 D Ex tc IIIB T135°C Dc
```

| Mark | Meaning |
|------|---------|
| ⬡ | Explosion protection symbol |
| II | Equipment Group II (surface industry — not mining) |
| 3 | Category 3 (Zone 22 minimum) |
| D | Dust atmosphere |
| Ex | Explosion protected |
| tc | Protection by enclosure (dust-tight) |
| IIIB | Dust group (non-conductive dust — suitable for grain/flour) |
| T135°C | Maximum permitted surface temperature (must be below dust layer ignition temperature) |
| Dc | Protection level (normal, for Category 3D) |

Where a device is dual-rated for gas and dust zones, it may show e.g. **II 3G/3D** — suitable for both Zone 2 (gas) and Zone 22 (dust).

For grain/flour (IIIB, MIT layer ~240°C), the equipment surface temperature rating must be **below 240°C** — equipment marked T135°C or T200°C is suitable; T300°C would not be appropriate where the layer ignition temperature is lower.

---

## 5. Ignition Sources — Why Standard Electronics Are Prohibited

Standard consumer electronic devices present multiple ignition hazard mechanisms in Zone 22. They are **not tested, designed, or certified** for use in explosive atmospheres.

| Ignition Mechanism | Detail | Relevance |
|-------------------|--------|-----------|
| **Electrical spark** | Battery contacts, charging ports, SIM card trays, switches and connectors can produce sparks | Grain/flour MIE is 30–100 mJ — within range of switch/connector spark energy |
| **Hot surface** | Processor chips, battery packs and charger circuits can reach 100–150°C+ under load | Close to or exceeding dust layer ignition temperatures |
| **Battery thermal runaway** | LiPo battery failure produces sparks, molten metal, and high-temperature gases | Catastrophic ignition source |
| **Static discharge** | Plastic housings accumulate static charge; a discharge to metal structure can release 5–20 mJ | MIE of cornstarch ~30–40 mJ; combined with accumulated charge in dusty air, risk is significant |
| **Screen fracture** | A broken touchscreen exposes internal circuits | Creates uncontrolled spark/hot surface risk |

The HSE's guidance on electrical equipment in ATEX environments is clear: **all electrical equipment used in zoned areas must be appropriately selected for that zone**. Taking an uncertified device into Zone 22 is a breach of DSEAR and the EPS Regulations 2016.

<a href="https://www.hse.gov.uk/electricity/atex/issues.htm" target="_blank">HSE: General Issues — Electrical Safety in ATEX Environments</a>

---

## 6. Mobile Phones and Tablets in Zone 22

### Legal Position

Under Regulation 7 of DSEAR 2002, all equipment used in classified hazardous zones must be appropriate for that zone. **Standard (non-certified) mobile phones, smartphones and tablets must not be taken into Zone 22.** This applies to all brands and models, regardless of how robust or well-sealed they appear.

The only exception is where the zone has been **temporarily declassified** under a permit to work (see section 10).

### ATEX-Certified Smartphones — Zone 2/22

A range of ATEX-certified smartphones are available commercially. These carry **II 3G/3D** (or higher) ATEX marking, confirming suitability for Zone 2 and Zone 22.

| Device | Certification | Key Features |
|--------|--------------|--------------|
| **BARTEC SP9EX2** | ATEX II 3G/3D, IIIC T135°C | 5G, Android 15, 6.1" AMOLED, 48 MP camera, IP64 |
| **Ecom Smart-Ex 203** | ATEX Zone 2/22, IECEx | 5G, Samsung-based, rugged Android |
| **i.SAFE IS730.2** | ATEX Zone 2/22 | Android smartphone, Zone 2/22 rated |

<a href="https://intrinsicallysafestore.com/product/bartec-sp9ex2-5g-smartphone-for-zone-2-division-2/" target="_blank">BARTEC SP9EX2 — Product Page</a>

<a href="https://www.ecom-ex.com/products/communication/cell-phones/smart-ex-203/" target="_blank">Ecom Smart-Ex 203 — Product Page</a>

<a href="https://www.airacom.com/devices/intrinsically-safe-mobile-devices/zone-2-smartphones/" target="_blank">Airacom: Zone 2/22 Smartphone Guide</a>

<a href="https://intrinsicallysafestore.com/blog/intrinsically-safe-smartphone-guide-2025/" target="_blank">Best Intrinsically Safe Smartphones 2025 — Buyer's Guide</a>

### Key Requirements When Using ATEX-Certified Phones in Zone 22

- The device **must carry the correct ATEX marking** for Group IIIB dust and temperature class below the dust layer MIT
- **No non-certified cases, screen protectors or accessories** may be attached while in the zone — these can compromise certification
- **Charging must take place outside the hazardous zone** — chargers are not typically ATEX certified
- The device must be **in good physical condition** — cracked screens, damaged ports or modified housings void the certification
- The **battery must be the OEM-supplied battery** — replacement with a third-party battery may void ATEX certification

---

## 7. Cameras in Zone 22

### Legal Position

Standard cameras — DSLR, mirrorless, compact, action cameras (GoPro etc.) and phone cameras — are not ATEX certified and **must not be taken into Zone 22**. The same principles apply as for mobile phones. Camera-flash units present a particular spark risk.

### ATEX-Certified Cameras

A range of ATEX Zone 2/22 certified cameras is available for industrial inspection and survey work.

**Handheld inspection cameras (most relevant for survey work):**

| Device | Zone Rating | Key Features |
|--------|------------|--------------|
| **Armadex OZC 3** | ATEX Zone 2/22, IECEx | 4K, 12 MP, 4× optical zoom, IP68, drop-rated 2.1 m, construction mode for dusty environments |

<a href="https://armadex.com/product/atex-camera/" target="_blank">Armadex ATEX Zone 2/22 Camera</a>

<a href="https://www.exloc.com/products/armadex-ex-m-ozc-3-explosion-proof-camera-atex-zone-2" target="_blank">Armadex OZC 3 — Explosion Proof Camera Zone 2/22</a>

**Other ATEX camera suppliers:**

<a href="https://www.atexshop.com/cameras/digital-cameras.html" target="_blank">ATEXshop: Intrinsically Safe & Explosion Proof Digital Cameras</a>

### Key Specifications to Verify Before Purchase

When selecting a camera for use in a grain/flour Zone 22 environment:

1. **Zone rating**: Must include Zone 22 (dust) not just Zone 2 (gas)
2. **Dust group**: Must be rated for Group IIIB (non-conductive) dust or IIIC (which covers IIIB)
3. **Surface temperature class**: Must be below the dust layer MIT for flour/grain (~240–290°C); T135°C or T200°C marking is appropriate
4. **IP rating**: IP6X (dust-tight) as minimum
5. **Camera flash**: If the camera has a flash unit, it must also be ATEX rated — or flash disabled in zone

---

## 8. Drones in Zone 22 — The Core Challenge

### Why Standard Drones Are Prohibited

A standard commercial drone (DJI, Parrot, Autel, Skydio, or any consumer/commercial UAV) presents multiple ignition hazards in Zone 22 and **must not be operated** in an unmitigated Zone 22 environment.

| Hazard | Mechanism | Significance |
|--------|-----------|-------------|
| **Motor sparking** | Brushless motors can produce sparks from bearing failure or winding fault | Direct ignition risk in dust cloud |
| **Battery thermal** | LiPo battery packs operate at elevated temperature; thermal runaway releases high-temperature gases | Worst-case ignition source |
| **ESC heat** | Electronic Speed Controllers (power electronics) generate significant heat under load | Surface temperature may exceed T3 class limits |
| **Static discharge** | Rotor blades moving through dusty air generate static charge; discharge to grounded metalwork can release energy | MIE of grain dust ~100 mJ; energetic static events can exceed this |
| **Crash/impact** | A crashed drone creates battery rupture, sparking and potentially fire | High consequence event in Zone 22 |
| **Rotor wash** | Downwash from rotors disturbs settled dust layers on beams and structure | Can temporarily elevate local zone from 22 to 21; a Category 3D drone would then be operating in a zone requiring Category 2D |
| **GPS-denied operation** | Inside a steel-framed warehouse, GPS signal is absent; flight relies on optical flow and barometric sensing — less stable, higher crash probability | Amplifies all crash-related risks |

### Current State of ATEX-Certified Drones (2025)

As of mid-2025, **very few commercially available drones carry full ATEX product certification** for explosive dust atmospheres. The fundamental engineering challenge is that drone motors and batteries are inherently high-energy components that are difficult to certify under the ATEX equipment categories.

**Flyability Elios 3** — the most widely referenced indoor inspection drone — is **not ATEX certified**. It is purpose-built for confined spaces and GPS-denied environments, with a protective cage to absorb impacts. It can be equipped with an optional flammable gas sensor (for awareness only — not a low-explosive-limit safety monitor). However, the drone itself has no ATEX certification and should not be operated in an unmitigated Zone 22 environment.

<a href="https://www.flyability.com/elios-3" target="_blank">Flyability Elios 3 — Indoor Inspection Drone</a>

Specialist ATEX drone inspection services do exist, but these typically operate on the basis of a **site-specific risk assessment** and temporary zone mitigation rather than a fully certified product. The equipment is assessed by a competent ATEX person as being suitable for a specific controlled scenario, not generically certified.

<a href="https://intrinsicallysafestore.com/blog/atex-certified-inspection-drones-industrial-safety-standards/" target="_blank">ATEX Certified Inspection Drones: Industrial Use Cases and Safety Standards</a>

### Three Practical Options for the Survey

**Option 1: ATEX-Assessed Drone (Specialist Contractor)**

Engage a specialist UAV inspection contractor who holds a site-specific risk assessment for drone use in Zone 22 dust environments. This is appropriate where:
- The roof structure cannot easily be inspected by other means
- Budget allows for specialist hire
- The timeline allows for the contractor's assessment and method statement approval process

This is the most technically comprehensive approach but also the most expensive and logistically complex for a one-off survey.

**Option 2: Temporary Zone Mitigation (Recommended for one-off survey)**

Temporarily remove the explosive atmosphere rather than bringing compliant equipment to it. This is the most commonly adopted approach for one-off inspections.

Process:
1. Cease all grain/flour/starch operations in the warehouse
2. Thoroughly clean the survey area: use an **ATEX-certified industrial vacuum** (not a standard vacuum — using a standard vacuum in Zone 22 is itself a DSEAR breach) to remove settled dust from the survey zone, or wet the dust down
3. Ventilate the warehouse: open doors and use fans to clear any residual airborne dust
4. Have a **competent person** (ATEX-qualified) confirm that the area can be temporarily treated as unclassified
5. Issue a **Permit to Work** covering the survey period, with clear stop criteria (e.g. if dusting resumes or airborne dust is observed, suspend drone operations immediately)
6. Conduct the survey with the standard drone under the permit
7. On completion, restore normal zone controls and re-establish operational status
8. Document the temporary declassification in the Explosion Protection Document

This approach requires a competent ATEX assessor to be involved and all steps documented. The DSEAR requirement for an Explosion Protection Document is not suspended — the temporary change must be reflected in it.

**Option 3: Alternative Survey Method (No drone)**

For a one-off insurance/compliance survey, alternatives to the drone include:

| Method | Notes |
|--------|-------|
| **Telescopic pole camera** | ATEX-rated camera on a carbon-fibre pole; low risk, no moving parts, no batteries in zone; limited height reach |
| **Rope access team** | Trained access technicians using ATEX-rated equipment; suitable for detailed structural inspection; requires rope access plan and ATEX equipment selection |
| **MEWP** (cherry picker / scissor lift) | Platform access to roof structure; the MEWP itself must be assessed for Zone 22 suitability; ATEX-certified lighting and tools required |
| **Scaffold (temporary)** | Most conservative option; eliminates all ATEX equipment complexity; practical for a defined survey area; higher cost and setup time |

For a routine one-off insurance survey, **Option 2 or Option 3** will be most practical. Option 3 is the lowest-risk approach from a regulatory compliance standpoint.

---

## 9. Explosion Protection Document

### Legal Requirement

The term "Explosion Protection Document" does not appear in DSEAR 2002 itself — it is an industry/practitioner term derived from Article 8 of the underlying EU Directive (99/92/EC). The substantive legal requirement in DSEAR is <a href="https://www.legislation.gov.uk/uksi/2002/2776/regulation/5/made" target="_blank">Regulation 5(4)(c)</a>, which requires employers with five or more employees to record the significant findings of the risk assessment, including where an explosive atmosphere may occur, sufficient information to show:

- **(i)** those places which have been classified into zones pursuant to Regulation 7(1)
- **(ii)** equipment required for, or helping to ensure, the safe operation of equipment in hazardous places
- **(iii)** that any verification of overall explosion safety required by Regulation 7(4) has been carried out
- **(iv)** the aim of any co-ordination required by Regulation 11 and the measures for implementing it

What practitioners call the "Explosion Protection Document" (EPD) is the written record that satisfies **Regulation 5(4)(c)(i)–(iv)**. It must be:

- Prepared as soon as practicable after the risk assessment is made
- Kept up to date when the workplace, work processes, or organisation changes significantly
- Made available to HSE inspectors on request

### What the EPD Must Include

1. **Identification of hazards**: Dangerous substances present, quantities, physical form
2. **Zone classification**: Maps or drawings showing the extent and classification of each zone
3. **Statement of compliance**: Confirmation that the workplace design, equipment, and working practices meet DSEAR requirements
4. **Control measures**: Technical and organisational measures in place to prevent or mitigate explosive atmosphere risks
5. **Equipment inventory**: List of electrical and non-electrical equipment used in each zone, with ATEX certification details
6. **Coordination arrangements**: Where contractors or other employers work in the zone, arrangements for safe coordination
7. **Emergency procedures**: Actions to be taken in the event of an explosion, fire, or dangerous substance release

### Relevance to the Survey

Before undertaking the roof structure survey:

1. **Review the existing EPD** — confirm the warehouse roof structure area is classified as Zone 22 (or check whether a zone classification assessment has been done at all — if not, one is required before the survey)
2. **Confirm the zone extent** — verify the Zone 22 boundary includes the roof structure levels to be surveyed
3. **Check for a permit to work procedure** — the EPD or associated management system should include a procedure for non-routine work in the zone
4. **Update the EPD** — document any temporary declassification and the permit to work issued for the survey; the EPD must reflect actual workplace conditions

If no such record exists (i.e. no EPD), this is itself a breach of **Regulation 5(4)(c) of DSEAR 2002** and should be addressed before any survey work is planned.

<a href="https://www.hse.gov.uk/fireandexplosion/dsear-background.htm" target="_blank">HSE: DSEAR In Detail — Explosion Protection Document</a>

---

## 10. Practical Guidance — One-Off Insurance/Compliance Survey

The following step-by-step process applies to a one-off survey of the internal roof structure of a grain/flour/starch Zone 22 warehouse using temporary zone mitigation (Option 2 from section 8), combined with ATEX-certified handheld equipment.

### Pre-Survey

| Step | Action | Responsibility |
|------|--------|---------------|
| 1 | Review current Explosion Protection Document; confirm Zone 22 extent covers roof structure | Site Manager / ATEX Assessor |
| 2 | If no EPD exists, commission one before proceeding | Employer |
| 3 | Select survey method (drone under PTW / pole camera / scaffold) and confirm equipment list | Survey Team Leader |
| 4 | Procure or hire ATEX-certified devices (phone, camera, drone operator if applicable) | Survey Team Leader |
| 5 | Prepare written Method Statement covering all activities, equipment, and emergency procedures | Survey Team Leader |
| 6 | Issue Permit to Work — specify: scope, duration, equipment list, stop criteria, emergency contacts | Site Manager |

### Day of Survey

| Step | Action |
|------|--------|
| 7 | Cease all grain/flour operations in warehouse; confirm no active conveying or filling |
| 8 | ATEX-certified vacuum the survey area or wet/suppress settled dust; ventilate warehouse |
| 9 | Competent person inspects and confirms area is suitable for survey (records findings) |
| 10 | Brief all survey personnel on Zone 22 risks, PTW conditions, emergency stop criteria |
| 11 | Verify all devices are ATEX-certified or that zone has been confirmed safe for uncertified equipment under PTW |
| 12 | Conduct survey; photograph roof structure, steelwork, cladding, and fixings |
| 13 | Note any areas of significant dust accumulation — these should be flagged for EPD review |

### Post-Survey

| Step | Action |
|------|--------|
| 14 | Close Permit to Work; confirm area returned to normal zone status |
| 15 | Compile survey report with photographic evidence for insurer/compliance record |
| 16 | Update EPD to reflect any new observations (e.g. dust accumulation areas, zone extent changes) |
| 17 | Retain all records (PTW, method statement, survey report, EPD update) for audit trail |

### Equipment Checklist

Before entering the Zone 22 warehouse, verify each item:

- [ ] Mobile phone: ATEX II 3G/3D or better, Group IIIB, surface temp class appropriate
- [ ] Camera: ATEX Zone 2/22, Group IIIB, IP6X dust-tight
- [ ] Drone (if used under PTW): covered by site-specific risk assessment and method statement
- [ ] Torches/lighting: ATEX Zone 2/22 rated (no standard battery torches)
- [ ] Radios: ATEX-rated two-way radios if required
- [ ] Personal protective equipment: anti-static footwear and clothing (reduces static discharge risk)

---

## 11. Key Standards and References

### UK Legislation

<a href="https://www.legislation.gov.uk/uksi/2002/2776/contents" target="_blank">Dangerous Substances and Explosive Atmospheres Regulations 2002 (SI 2002/2776)</a>

<a href="https://www.legislation.gov.uk/uksi/2016/1107/contents" target="_blank">Equipment and Protective Systems Intended for Use in Potentially Explosive Atmospheres Regulations 2016 (SI 2016/1107)</a>

### HSE Guidance

<a href="https://www.hse.gov.uk/fireandexplosion/atex.htm" target="_blank">HSE: ATEX and Explosive Atmospheres — Overview</a>

<a href="https://www.hse.gov.uk/fireandexplosion/dsear.htm" target="_blank">HSE: Dangerous Substances and Explosive Atmospheres Regulations</a>

<a href="https://www.hse.gov.uk/fireandexplosion/dsear-background.htm" target="_blank">HSE: DSEAR In Detail (background and detailed guidance)</a>

<a href="https://www.hse.gov.uk/pubns/books/l138.htm" target="_blank">HSE L138: Dangerous Substances and Explosive Atmospheres — ACOP and Guidance</a>

<a href="https://www.hse.gov.uk/pubns/books/hsg103.htm" target="_blank">HSE HSG103: Safe Handling of Combustible Dusts — Precautions Against Explosions</a>

<a href="https://www.hse.gov.uk/food/safety-hazards/dustexplosion.htm" target="_blank">HSE: Prevention of Dust Explosions in the Food Industry</a>

<a href="https://www.hse.gov.uk/agriculture/topics/grain-dust.htm" target="_blank">HSE: Grain Dust</a>

<a href="https://www.hse.gov.uk/electricity/atex/issues.htm" target="_blank">HSE: Electrical Safety in ATEX Environments — General Issues</a>

<a href="https://www.hse.gov.uk/comah/sragtech/techmeasareaclas.htm" target="_blank">HSE: Hazardous Area Classification and Control of Ignition Sources</a>

### ATEX-Certified Smartphones

<a href="https://intrinsicallysafestore.com/product/bartec-sp9ex2-5g-smartphone-for-zone-2-division-2/" target="_blank">BARTEC SP9EX2 — 5G ATEX Zone 2/22 Smartphone</a>

<a href="https://www.ecom-ex.com/products/communication/cell-phones/smart-ex-203/" target="_blank">Ecom Smart-Ex 203 — 5G Zone 2/22 Smartphone</a>

<a href="https://www.airacom.com/devices/intrinsically-safe-mobile-devices/zone-2-smartphones/" target="_blank">Airacom: Zone 2/22 ATEX Smartphones Guide</a>

<a href="https://intrinsicallysafestore.com/blog/intrinsically-safe-smartphone-guide-2025/" target="_blank">Best Intrinsically Safe Smartphones 2025 — Buyer's Guide</a>

### ATEX-Certified Cameras

<a href="https://armadex.com/product/atex-camera/" target="_blank">Armadex — ATEX Zone 2/22 Inspection Camera</a>

<a href="https://www.exloc.com/products/armadex-ex-m-ozc-3-explosion-proof-camera-atex-zone-2" target="_blank">Armadex OZC 3 — Explosion Proof 4K Camera Zone 2/22</a>

<a href="https://www.atexshop.com/cameras/digital-cameras.html" target="_blank">ATEXshop: ATEX Digital Cameras for Hazardous Areas</a>

### Drones in Hazardous Environments

<a href="https://www.flyability.com/elios-3" target="_blank">Flyability Elios 3 — Confined Space Indoor Inspection Drone (not ATEX certified)</a>

<a href="https://intrinsicallysafestore.com/blog/atex-certified-inspection-drones-industrial-safety-standards/" target="_blank">ATEX Certified Inspection Drones: Industrial Use Cases and Safety Standards</a>

### British Standards (BS)

- **BS EN 60079-10-2** — Explosive atmospheres: Classification of areas — Explosive dust atmospheres
- **BS EN 60079-31** — Explosive atmospheres: Equipment dust ignition protection by enclosure "t"
- **BS EN 60079-0** — Explosive atmospheres: General requirements for equipment

---

*Document prepared: May 2026 | Next review: when EPD is updated or following any change in warehouse operations or dust classification*
