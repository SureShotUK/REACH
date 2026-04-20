# IUCLID 6 — Article 26 Inquiry: Step-by-Step Guide

**Substance:** HVO (Renewable hydrocarbons, diesel type fraction)
**EC Number:** 700-571-2 | **CAS Number:** 928771-01-1
**Company:** Portland Fuel Ltd
**Prepared:** 2026-04-20
**Last updated:** 2026-04-20 — validated against live IUCLID 6 Cloud session

> This guide covers the complete process from logging into IUCLID 6 Cloud to exporting a validated `.i6z` inquiry dossier ready for submission to HSE. All field values are pre-filled for HVO. Steps reflect actual IUCLID behaviour encountered during submission.

---

## Before You Start — Checklist

- [ ] IUCLID 6 Cloud account created at iuclid6.echa.europa.eu
- [ ] Portland Fuel's registered company name, address and contact details to hand
- [ ] Working Context set to **UK REACH** (not ECHA/EU REACH) — see Part 1.2

---

## Part 1 — Log In and Set Working Context

### 1.1 Log In

1. Go to <a href="https://iuclid6.echa.europa.eu" target="_blank">iuclid6.echa.europa.eu</a>
2. Sign in with your ECHA account email and password

### 1.2 Set Working Context to UK REACH

This is the most commonly missed step — if the context is wrong, the Inquiry dossier type will not be available and the correct validation rules will not apply.

1. Look at the **top-right corner** of the screen after logging in
2. You will see a dropdown or label showing the current working context (it may say "ECHA" or be blank)
3. Click it and select **"UK REACH"**
4. The screen header should update to confirm UK REACH context

> If you do not see UK REACH as an option, your account may not have the UK REACH workspace activated. Contact ukreachitsupport@defra.gov.uk.

---

## Part 2 — Create the Legal Entity

The Legal Entity is Portland Fuel's company record in IUCLID. It must exist before you can create a dossier.

1. From the main menu, click **Legal Entities**
2. Check whether Portland Fuel already exists — if so, verify details and skip to Part 3
3. If not, click **+ New** or **Create Legal Entity**

| Field | Value |
|-------|-------|
| Legal Entity name | Portland Fuel Ltd (use exact registered company name) |
| Country | United Kingdom |
| Address | (registered address) |
| Postcode | (postcode) |
| Contact person — name | (your name) |
| Contact person — email | steve@portland-fuel.co.uk |
| Contact person — phone | (phone number) |

Click **Save**.

---

## Part 3 — Create the Substance Dataset

The substance dataset is the core record for HVO. All sections (1.1, 1.2, 1.4, 14) are added to it.

1. From the main menu, click **Substances**
2. Click **+ New** or **Create Substance**
3. Enter substance name: `Renewable hydrocarbons (diesel type fraction)`

Click **Create** or **Save**. The left-hand panel now shows all available sections.

---

## Part 4 — Create the Three Reference Substances

IUCLID requires separate Reference Substance records for the parent HVO substance and for each constituent group used in Section 1.2. Create all three before filling in any sections.

From the main menu, click **Reference Substances**.

---

### Reference Substance A — HVO Parent Substance

Click **+ New** and fill in:

| Field | Value |
|-------|-------|
| Reference substance name | Renewable hydrocarbons (diesel type fraction) |
| EC Number | 700-571-2 |
| CAS Number | 928771-01-1 |
| Molecular formula | (leave blank — justified in Remarks) |
| Molecular weight | (leave blank — justified in Remarks) |
| Structural formula | (leave blank — justified in Remarks) |

**Remarks field** (required to pass BR122):

> HVO (Hydrotreated Vegetable Oil) is a UVCB substance consisting of a complex mixture of paraffinic hydrocarbons in the C10-C20 carbon number range (molecular formula range C10-20 H22-42). A single molecular formula, molecular weight, or structural formula cannot be assigned due to the variable composition inherent in this substance type. Substance identity is established by the boundary composition defined in Section 1.2 and the analytical information in Section 1.4.

Click **Save**.

---

### Reference Substance B — C15-C18 n-Alkanes (Constituent Group 1)

Click **+ New** and fill in:

| Field | Value |
|-------|-------|
| Reference substance name | C15-C18 n-alkanes (normal paraffins) |
| IUPAC name | pentadecane; hexadecane; heptadecane; octadecane |
| EC Number | (leave blank) |
| CAS Number | (leave blank) |
| Molecular formula | (leave blank — justified in Remarks) |
| Molecular weight | (leave blank — justified in Remarks) |

**Remarks field** (required to pass BR122 and BR186):

> This entry represents a group of straight-chain paraffinic hydrocarbons in the C15-C18 range. A single molecular formula and molecular weight cannot be assigned as this group encompasses multiple molecular species: n-pentadecane (C15H32, MW 212.4), n-hexadecane (C16H34, MW 226.4), n-heptadecane (C17H36, MW 240.5), n-octadecane (C18H38, MW 254.5).

Click **Save**.

---

### Reference Substance C — C15-C18 iso-Alkanes (Constituent Group 2)

Click **+ New** and fill in:

| Field | Value |
|-------|-------|
| Reference substance name | C15-C18 iso-alkanes (branched paraffins) |
| IUPAC name | C15-C18 branched-chain alkanes |
| EC Number | (leave blank) |
| CAS Number | (leave blank) |
| Molecular formula | (leave blank — justified in Remarks) |
| Molecular weight | (leave blank — justified in Remarks) |

**Remarks field** (required to pass BR122 and BR186):

> This entry represents a group of branched-chain paraffinic hydrocarbons in the C15-C18 range, produced by the isomerisation step during HVO manufacture. A single molecular formula and weight cannot be assigned as this group encompasses multiple isomeric molecular species. Molecular formula range: C15H32 to C18H38.

Click **Save**.

---

## Part 5 — Section 1.1: General Information

1. Return to your HVO substance dataset (click **Substances**, then click on "Renewable hydrocarbons (diesel type fraction)")
2. In the section tree, click **1.1 Identification**
3. Click **+ New** or **Add record**

| Field | Value |
|-------|-------|
| Substance name | Renewable hydrocarbons (diesel type fraction) |
| **Type of substance** | **UVCB** |
| EC number | 700-571-2 |
| CAS number | 928771-01-1 |

> **Type of substance must be set to UVCB** — leaving it blank causes validation failure BR121.

**Link the Reference Substance:**
1. Find the **Reference substance** field
2. Search for and select **Renewable hydrocarbons (diesel type fraction)** (Reference Substance A)

**Set the Legal Entity:**
1. Find the **Legal Entity** field
2. Select **Portland Fuel Ltd**

Click **Save**.

---

## Part 6 — Section 1.2: Composition

Section 1.2 requires: the correct composition type, a manufacturing process description, a degree of purity, and two constituent groups each linked to their own reference substance.

### 6.1 Create the Composition Record

1. In the section tree, click **1.2 Composition**
2. Click **+ New** or **Add record**

### 6.2 Set Composition Type

| Field | Value |
|-------|-------|
| Name | HVO — legal entity composition |
| **Type of composition** | **legal entity composition of the substance** |
| State/form | liquid |

### 6.3 Add Manufacturing Process Description

Find the **Description** field and enter the following (required to pass BR195):

> HVO (Hydrotreated Vegetable Oil) is produced by catalytic hydrotreatment of vegetable oils and/or animal fats (triglycerides). Starting materials include rapeseed oil, soy oil, palm oil, tallow, and/or used cooking oil (feedstock ratios vary by batch and supplier).
>
> Manufacturing process:
> 1. Deoxygenation — triglycerides are reacted with hydrogen at 250-380 degrees C and 30-130 bar over a NiMo or CoMo alumina catalyst. Oxygen is removed as water (H2O), carbon monoxide (CO) and carbon dioxide (CO2), yielding straight-chain C15-C18 paraffins.
> 2. Isomerisation/hydrocracking — straight-chain paraffins undergo selective isomerisation to produce branched-chain (iso-paraffinic) hydrocarbons, improving cold-flow properties.
> 3. Fractionation/stripping — light ends, water, CO and CO2 are removed by distillation and gas stripping to yield the final diesel-range paraffinic product.
>
> The resulting product consists predominantly of C15-C18 normal and iso-paraffinic hydrocarbons. Total aromatics less than 1.0% w/w; sulphur content less than 1 mg/kg; FAME content zero.

### 6.4 Set Degree of Purity

Find the **Degree of purity** field (required to pass BR075):

| Field | Value |
|-------|-------|
| Value | 100 |
| Units | % (w/w) |

> For a UVCB substance, purity is reported as 100% w/w. All components are reported as constituents, not impurities.

### 6.5 Add Constituent Group 1 — n-Alkanes

Scroll to the **Constituents** section and click **+ Add**:

| Field | Value |
|-------|-------|
| Reference substance | C15-C18 n-alkanes (normal paraffins) |
| Typical concentration | 50 |
| Lower concentration | 30 |
| Upper concentration | 70 |
| Units | % (w/w) |

**Save this sub-entry** using the small Save/tick button within the constituent row before proceeding.

### 6.6 Add Constituent Group 2 — iso-Alkanes

Click **+ Add** for a second constituent:

| Field | Value |
|-------|-------|
| Reference substance | C15-C18 iso-alkanes (branched paraffins) |
| Typical concentration | 45 |
| Lower concentration | 25 |
| Upper concentration | 65 |
| Units | % (w/w) |

**Save this sub-entry** before saving the parent record.

> The two ranges do not sum to exactly 100% — this is correct for a UVCB. The remaining percentage accounts for trace aromatics and minor components.

Click **Save** on the main Composition record.

---

## Part 7 — Section 1.4: Analytical Information

### 7.1 Create the Record

1. In the section tree, click **1.4 Analytical information**
2. Click **+ New** or **Add record**
3. Name the record: `HVO identity — chromatographic analysis`

### 7.2 Add an Analytical Determination Entry

Inside the record, find the **Analytical determination** repeating block and click **+ Add**.

| Field | Value |
|-------|-------|
| **Purpose of analysis** | **identification and quantification** |
| **Type of information provided** | **methods and results** |
| **Analysis type** | **gas chromatography [GC]** |
| **Rationale for no results** | **analysis scientifically not necessary (other information available)** |

Enter the following in the **Justification** field:

> Substance identity and composition are established by reference to existing supplier Safety Data Sheet data. The Phillips 66 Renewable Diesel SDS (February 2024) contains GC chromatographic analysis confirming C15-C18 isoalkane-predominant composition, total aromatics less than 1.0% by weight, zero sulphur content, and zero FAME content (fatty acid methyl esters). This is consistent with EC 700-571-2 registered under UK REACH number UK-01-9638319484-0-0004. Full analytical package is held by the Lead Registrant. Portland Fuel will obtain access to this data via a Letter of Access upon joining the consortium.

> **Critical — saving sub-entries in IUCLID:** The Analytical determination entry has its own Save button separate from the main record Save. Look for a small floppy disk icon, tick, or OK button within the sub-entry row. Click this **first** to commit the sub-entry, then click Save on the main record. If the entry disappears after saving the parent, the sub-entry was not committed — click elsewhere on screen after filling in the fields to commit it before saving the parent record.

Click **Save** on the main Analytical information record.

---

## Part 8 — Section 14: Inquiry

### 8.1 Open Section 14

1. In the section tree, scroll to **Section 14 — Inquiry**
2. Open the existing **Inquiry.001** document, or click **+ New** if none exists

### 8.2 Set Type of Inquiry

| Field | Value |
|-------|-------|
| **Type of inquiry** | **Type 3 — inquiry for a substance before its registration** |

The options available under UK REACH are:
- Type 3 — inquiry for a substance before its registration **(select this)**
- Type 4 — inquiry for tonnage band increase (not applicable)
- Type 5 — inquiry for read-across studies (not applicable)

### 8.3 Set New Studies Required

| Field | Value |
|-------|-------|
| **Information requirements requiring new studies to be conducted?** | **No** |

> This field must be explicitly set to **No** — leaving it blank is not the same as No and causes a validation failure.

Confirm that no individual study types are ticked "Yes" in the sub-section below.

### 8.4 Tonnage Band

If this field is present:

| Field | Value |
|-------|-------|
| Tonnage band for inquiry | 10-100 tonnes per year |

Click **Save**.

---

## Part 9 — Run Validation

1. From within the substance dataset, find the **Validation Assistant** — under a **Tools** menu or a shield/checkmark icon
2. Click **Validate** or **Run validation**

| Result type | Action |
|-------------|--------|
| **FAILURE** (red) | Must fix before submission |
| **WARNING** (amber) | Does not block submission — note only |
| **INFO** (blue) | No action required |

The following warnings are expected and can be left for an inquiry dossier:

| Warning code | Reason safe to ignore |
|---|---|
| QLT052 | Degree of purity for UVCB — addressed by 100% w/w entry |
| QLT057 | IUPAC name — populated for constituents; amber only |
| QLT065 | Optical activity — not applicable to a hydrocarbon mixture |

---

## Part 10 — Create and Export the Inquiry Dossier

1. From within the substance dataset, click **Create Dossier**

2. Select dossier type:

| Field | Value |
|-------|-------|
| Dossier type | Inquiry |
| Submission type | UK REACH |

> If "Inquiry" is not listed, the Working Context is not UK REACH — switch it in the top-right corner.

3. Click **Advanced** or expand **Advanced Options**
4. Tick **"Include Legal Entity"** — this is a UK REACH requirement and is not enabled by default
5. Confirm Portland Fuel Ltd is selected

6. Name the dossier: `Portland_Fuel_HVO_Article26_Inquiry_2026`
7. Click **Create** or **Finish**
8. Click **Export** or **Download** to save the `.i6z` file to your computer

---

## Part 11 — Submit via the Comply with UK REACH Portal

1. Go to <a href="https://comply-chemical-regulations.service.gov.uk/" target="_blank">comply-chemical-regulations.service.gov.uk</a>
2. Sign in with **Government Gateway** credentials (create account on first use)
3. Click **New submission** and upload the `.i6z` file
4. Review the submission summary and click **Submit**
5. **Note the submission reference number** — needed for any HSE follow-up
6. Email acknowledgement will be sent to steve@portland-fuel.co.uk

---

## What Happens Next

| Timeframe | Event |
|-----------|-------|
| Immediately | Email acknowledgement from HSE/Defra |
| Up to 15 working days | HSE processes the inquiry |
| On completion | Inquiry number issued; Lead Registrant identity visible in portal; your contact details shared with existing registrants |
| After response | Contact Lead Registrant to negotiate consortium membership and Letter of Access |

---

## Troubleshooting

| Error code | Cause | Fix |
|------------|-------|-----|
| BR020 | Reference substance has no EC, CAS or IUPAC name | Add IUPAC name to the reference substance |
| BR075 | Degree of purity missing in Section 1.2 | Set to 100 % (w/w) in the composition record |
| BR121 | Type of substance not set in Section 1.1 | Set to UVCB |
| BR122 / BR186 | Molecular formula missing without justification | Add UVCB justification text to Remarks field of the reference substance |
| BR175 | Type of composition not set | Set to "legal entity composition of the substance" |
| BR195 | Manufacturing process description missing | Add description text to the Description field of the composition record |
| BR196 | Constituents not defined individually for UVCB | Create separate reference substances for each constituent group and link them |
| BR228 | Analytical determination sub-entry not saved | Commit the sub-entry with its own Save/tick button before saving the parent record |
| BR232 / BR086 | Type of inquiry not set in Section 14 | Select Type 3 |
| BR167 | New studies field blank | Set explicitly to No — blank is not the same as No |
| "Create Dossier" unavailable | Wrong Working Context | Switch to UK REACH in top-right corner |
| Portal rejects .i6z | Legal Entity not included in export | Re-export with Advanced Options — tick "Include Legal Entity" |

---

## Key Reference Data

| Field | Value |
|-------|-------|
| Substance name | Renewable hydrocarbons (diesel type fraction) |
| EC Number | 700-571-2 |
| CAS Number | 928771-01-1 |
| Substance type | UVCB |
| Existing UK REACH registration | UK-01-9638319484-0-0004 |
| Tonnage band | 10-100 tonnes/year |
| Inquiry type | Type 3 — inquiry for a substance before its registration |
| New studies required | No |
| Company contact | steve@portland-fuel.co.uk |
| HSE IT Support | ukreachitsupport@defra.gov.uk |
| Comply with UK REACH portal | comply-chemical-regulations.service.gov.uk |
| IUCLID 6 Cloud | iuclid6.echa.europa.eu |
