<img src="../../Portland Long.png" alt="Portland Long" style="width:40%; height:auto;" align="right">

# IUCLID 6 — NRES Member Registration Dossier: Step-by-Step Guide

**Substance:** HVO (Renewable hydrocarbons, diesel type fraction)
**EC Number:** 700-571-2 | **CAS Number:** 928771-01-1
**Pathway:** NRES — New Registration of an Existing Substance
**Dossier type:** Registration → Member dossier
**Prepared:** 2026-06-26
**Basis:** Article 26 Inquiry accepted — submission ref 0000025852-5

> This guide covers the complete process for preparing and submitting a UK REACH NRES
> member registration dossier. It is a companion to the Article 26 inquiry guide
> (`IUCLID6_Article26_Inquiry_Guide.md`) and builds on work already completed in IUCLID.

---

## Before You Start — Checklist

- ✅ Article 26 Inquiry accepted (ref 0000025852-5)
- [ ] Section 13.2 data waiver statement saved as PDF (`HVO_Section13_2_Data_Waiver.md`)
- ✅ IUCLID 6 Cloud account active at iuclid6.echa.europa.eu
- [   ] Working Context set to **UK REACH** (not ECHA/EU REACH) — see Part 1.2
- ✅ Existing HVO Substance record and Reference Substances are in place

---

## What Is Different From the Inquiry Dossier?

| | Article 26 Inquiry (done) | NRES Member Dossier (this guide) |
|---|---|---|
| **Dossier type** | Inquiry | Registration — Member Dossier |
| **Section 14** | Required (Type 3 Inquiry) | Not used |
| **Section 3** | Not required | Required — document our uses |
| **Section 13.2** | Not used | Required — data waiver statement |
| **Substance record** | Created from scratch | Reuse existing HVO record |
| **Reference Substances** | Created from scratch | Reuse all three existing records |
| **Sections 1.1, 1.2, 1.4** | Completed | Carry over as-is |

**The existing HVO substance record contains all the identity work already done. The
member dossier is a new dossier created from the same substance record — do not create
a new substance.**

---

## Part 1 — Log In and Set Working Context

### 1.1 Log In

1. Go to <a href="https://iuclid6.echa.europa.eu" target="_blank">iuclid6.echa.europa.eu</a>
2. Sign in with your ECHA account email and password

### 1.2 Set Working Context

The IUCLID 6 Cloud working context dropdown does not include a UK REACH option. This is
expected — the Article 26 Inquiry was also prepared using an EU REACH context and submitted
successfully to HSE via the Comply portal. The same approach applies here.

1. Look at the **top-right corner** of the screen after logging in
2. Click the working context selector
3. Under the **EU REACH** group, select **"REACH Registration member of a joint submission — general case"**

> Do not select "REACH Registration 10-100 tonnes" — that is a full standalone registration
> template requiring the complete data package. The member of joint submission template is
> correct for the NRES pathway.
>
> Do not use any option in the **"UK Test (do not use)"** group.

---

## Part 2 — Open the Existing HVO Substance Record

The substance identity work from the inquiry is already complete and does not need to be
redone. Simply open the existing record.

1. From the main menu, click **Substances**
2. Find and click **Renewable hydrocarbons (diesel type fraction)**
3. Verify sections 1.1, 1.2, and 1.4 are present — they should be from the inquiry work
4. Do not modify these sections unless correcting an error

---

## Part 2b — Section 2.3 PBT Assessment

This section must have a record created with a PBT status selected, or validation will fail.

1. In the section tree, click **2.3 PBT assessment**
2. Click **+ New** to create a summary record (look for the sigma Σ icon — summary documents are marked with it)
3. Set the **PBT status** field to: **Substance not fulfilling the PBT/vPvB criteria**
4. In the **Justification** field, enter:

> HVO (renewable hydrocarbons, diesel type fraction) is a paraffinic hydrocarbon fuel.
> Paraffinic hydrocarbons are readily biodegradable and do not meet the bioaccumulation
> criteria for PBT/vPvB assessment. Full PBT assessment data is held by the lead
> registrant and will be accessed via Letter of Access as part of the NRES registration
> process (UK REACH Article 26 Inquiry reference 0000025852-5).

Click **Save**.

---

## Part 3 — Add Section 3: Manufacture and Uses

Section 3 is required for a registration dossier. It documents who manufactures the
substance and how we use it.

1. In the section tree, click **3.1 Manufacture** (or **3. Manufacture and uses**)
2. Click **+ New** or **Add record**

### 3.1 Manufacturer/Importer Information

| Field | Value |
|-------|-------|
| Role | Importer |
| Legal entity | Portland Fuel Ltd |

### 3.2 Tonnage

| Field | Value |
|-------|-------|
| Tonnage band | 10–100 tonnes per year |
| Year | 2026 (first intended year of import) |
| Tonnage used as intermediate (transported) | 0 |
| Tonnage used as intermediate (on-site) | 0 |
| Tonnage directly exported | 0 |
| Tonnage for own use | 0 |

> Enter your anticipated annual import volume — the exact figure matters less than being
> within the 10–100 t/yr band. Since we have not yet started importing, use 2026 and an
> estimated volume based on business plans.
>
> **Resale is not "own use"** — own use means tonnage consumed in your own operations.
> All sub-fields above are 0 because all imported tonnage is placed on the market (sold
> to customers). There is no separate "placed on the market" field; add the following to
> the **Remarks / Additional Information** field to explain:
>
> *"All imported tonnage is placed on the market. We operate solely as an importer and
> distributor of HVO; the substance is sold in full to commercial end users for use as a
> drop-in replacement for fossil diesel in commercial road vehicles. No tonnage is consumed
> in our own operations, used as a chemical intermediate, or exported outside Great Britain."*

### 3.3 Sites

The site record must be linked to a Site entity with a full address, or validation will
fail (BR021 and TCC_0303_02). First create the Site entity, then link it here.

**Step 1 — Create the Site in IUCLID:**
1. From the IUCLID main menu, click **Legal Entities**
2. Open **Portland Fuel Ltd**
3. Find the **Sites** section and click **+ New site**
4. Fill in:

| Field | Value |
|-------|-------|
| Site name | Portland Fuel Ltd |
| Address | [office street address] |
| Postal code | [office postcode] |
| Town | [office town] |
| Country | United Kingdom |

> We import HVO and deliver directly to customers without storing it at any Portland Fuel
> premises. The office address is the correct site to use — it is the place of business
> from which the import activity is managed and where regulatory responsibility sits.

5. Click **Save**

**Step 2 — Link the Site to Section 3.3:**
1. Go back to the substance dataset → Section **3.3 Sites**
2. Click **+ New** or **Add record**
3. In the **Site** field, click the link/search icon and select the Portland Fuel Ltd site just created
4. Set **Role at site** to: Importer
5. In the **Remarks** field, enter:

> HVO is delivered directly from the point of import to customer sites. Portland Fuel Ltd
> does not store or physically handle the substance at its office premises. The site
> address given is the registered place of business from which import activities are
> managed.

Click **Save**.

### 3.4 Information on Mixtures

**Skip this section.** HVO is a UVCB substance (a pure substance category in REACH terms),
not deliberately mixed with other substances before supply. Leave blank.

### 3.5 Use and Exposure Information

Section 3.5 contains seven sub-sections. Only two apply to our circumstances.

| Sub-section | Action |
|---|---|
| 3.5.0 Use and exposure information relevant for all uses | **Complete** — see below |
| 3.5.1 Manufacture | Skip — we do not manufacture HVO |
| 3.5.2 Formulation or re-packing | Skip — we do not formulate or re-pack |
| 3.5.3 Uses at industrial sites | Skip — end use is not a factory/industrial process |
| 3.5.4 Widespread uses by professional workers | **Complete** — see below |
| 3.5.5 Consumer uses | Skip — customers are businesses, not general consumers |
| 3.5.6 Service life | Skip — not applicable to a consumed fuel |

#### 3.5.0 — Use and Exposure Information Relevant for All Uses

Click **+ New** and fill in the **Description** or **Remarks** field:

> We act as importer and distributor of HVO (Renewable hydrocarbons, diesel type
> fraction; EC 700-571-2) into Great Britain. The substance is imported in bulk as a
> finished fuel product and sold without further formulation or re-packing to commercial
> end users. All uses are as a drop-in replacement for fossil diesel fuel, compliant with
> EN 15940 (Automotive fuels — paraffinic diesel from synthesis or hydrotreatment).

There is a **Justification for no exposure assessment** tick-box field with four options.
**Leave all four unticked** — none apply to our circumstances:

| Option | Why it does not apply |
|---|---|
| Substance registered under REACH Article 17/18 only | Articles 17/18 cover isolated intermediates — HVO is a finished fuel |
| Total tonnage <10 tonnes/year per registrant | Our band is 10–100 t/yr |
| Only imported in mixture(s) below Article 14(2) | HVO is imported as a pure UVCB substance, not as a minor component in a mixture |
| Not hazardous / not PBT or vPvB per Article 14(4) | HVO IS classified as hazardous: H226 (Flammable Liquid Cat 3) and H304 (Aspiration Toxicity Cat 1) |

The actual reason — NRES pathway, data held by the lead registrant — is not represented in
this EU REACH picklist. Enter the explanation in the free-text field below instead. If
validation fails at submission because a tick box is required, contact
ukreachitsupport@defra.gov.uk for guidance.

In the free-text **Remarks** or **Further justification** field, enter:

> This registration is submitted under the New Registration of an Existing Substance
> (NRES) pathway (UK REACH Article 26 Inquiry reference 0000025852-5, accepted June
> 2026). As an NRES registrant we are not currently in a position to provide a full
> exposure assessment, as the relevant data is held by the lead registrant within the
> established substance group for this substance (EC 700-571-2). A data waiver statement
> explaining this is attached to Section 13.2 of this dossier. We will obtain a Letter
> of Access to the substance group's data upon joining the joint registration, and a full
> exposure assessment compliant with the information requirements for the 10–100 tonnes
> per year tonnage band will be submitted before midnight on 27 October 2030.

Click **Save**.

#### 3.5.4 — Widespread Uses by Professional Workers

This covers commercial vehicle operators (hauliers, fleet operators, delivery businesses)
using HVO as fuel in their vehicles in the course of their work.

Click **+ New** and fill in:

| Field | Value |
|-------|-------|
| Use number | 1 (assign sequentially — this is your first use entry, so enter 1) |
| Use name | Use of HVO as drop-in diesel fuel in commercial road vehicles |
| Use descriptor | SU22 (Widespread uses by professional workers) |
| Product category | PC13 (Fuels) |
| Sector of end use | SU22 (Widespread uses by professional workers) |
| Registration / Report Status | Registration according to REACH Article 10; total tonnage manufactured/imported ≥10 tonnes/year per registrant |
| Technical function of the substance during use | Fuel |
| Subsequent service life relevant to this use | No — HVO is combusted as fuel and is not incorporated into an article with a service life |

In the **Description of use** field, enter:

> HVO is supplied to commercial operators for use as a drop-in replacement for fossil
> diesel fuel in commercial road vehicles (trucks, vans, buses). End users are
> professional businesses and fleet operators. The substance is combusted in vehicle
> engines compliant with EN 590 (conventional diesel) and EN 15940 (paraffinic diesel).
> No mixing, formulation, or further processing is carried out by the end user.

**Contributing activities — Environment:**
Click **+ Add contributing activity / technique for the environment** and set:

| Field | Value |
|---|---|
| Environmental release category (ERC) | ERC8a — Wide dispersive outdoor use of processing aids in open systems |

**Contributing activities — Workers:**
Click **+ Add contributing activity / technique for workers** and set:

| Field | Value |
|---|---|
| Process category (PROC) | PROC8a — Transfer of substance or mixture (charging and discharging) at non-dedicated facilities |

> These cover the two main exposure routes: combustion emissions to outdoor air (ERC8a)
> and worker exposure during vehicle refuelling (PROC8a).

Click **Save**.

### 3.6 Uses Advised Against

Leave blank. No entries needed for a straightforward fuel import and distribution
registration — there are no uses we need to specifically exclude.

### 3.7 Environmental Assessment from Aggregated Sources

**Skip — not mandatory** (no asterisk). Leave blank.

---

## Part 4 — Prepare and Attach the Section 13.2 Data Waiver

This is the critical new element for the NRES pathway.

### 4.1 Prepare the Waiver Document

1. Open `HVO_Section13_2_Data_Waiver.md`
2. Fill in the `[insert date of dossier submission]` field with today's date
3. Save as **PDF** (copy text into Word or Google Docs → File → Print → Save as PDF)
4. Name the file: `Portland_Fuel_HVO_NRES_DataWaiver_2026.pdf`

### 4.2 Attach in IUCLID Section 13.2

1. In the section tree, scroll to **Section 13** (may be labelled "Assessment reports",
   "Risk assessment" or similar)
2. Expand Section 13 and click **13.2**
3. If no record exists, click **+ New** or **Add**
4. Look for an **Attachments** or **Supporting documents** area within the section
5. Click **+ Add attachment** and upload `Portland_Fuel_HVO_NRES_DataWaiver_2026.pdf`

> If Section 13.2 shows sub-sections for each information endpoint, add the waiver as a
> general attachment to the Section 13.2 header record. If you cannot locate Section 13.2,
> check that your Working Context is UK REACH and the dossier template is Registration type.
> Contact ukreachitsupport@defra.gov.uk if the section is not visible.

Click **Save**.

---

## Part 5 — Run Validation

1. From within the substance dataset, find the **Validation Assistant** (under Tools or
   a shield/checkmark icon)
2. Click **Validate** or **Run validation**

| Result type | Action |
|-------------|--------|
| **FAILURE** (red) | Must fix before submission |
| **WARNING** (amber) | Does not block submission — note only |
| **INFO** (blue) | No action required |

### Expected validation rules for a member registration dossier

A registration dossier has stricter validation than the inquiry dossier. The following
failures are likely and must be resolved:

| Code | Likely cause | Fix |
|------|-------------|-----|
| BR075 | Degree of purity missing in 1.2 | Already set to 100% w/w from inquiry work |
| BR121 | Type of substance not UVCB | Already set from inquiry work |
| BR195 | Manufacturing process missing in 1.2 | Already populated from inquiry work |
| BR196 | Constituent reference substances missing | Already created from inquiry work |

New failures specific to registration dossiers may also appear. Work through each red
failure in turn. If a failure relates to a data endpoint (e.g., toxicology, ecotox) that
we cannot supply, the data waiver in Section 13.2 addresses this — leave the endpoint
sections empty and note any related warnings.

### Known non-applicable validation failures

| Field | Why it fires | Action |
|---|---|---|
| On-site isolated intermediates tonnage band (Article 17) | EU REACH template flags this as mandatory for all registrations | Leave blank — Article 17 covers chemicals used in on-site synthesis; HVO is a finished fuel, not an intermediate |
| Transported isolated intermediates tonnage band (Article 18) | Same reason | Leave blank — Article 18 covers chemicals transported between sites for synthesis; does not apply to fuel distribution |

If either of these returns as a red failure that blocks submission, email
ukreachitsupport@defra.gov.uk and explain that HVO is not used as a chemical
intermediate and these fields are not applicable. Quote the intermediate tonnage
values of 0 already declared in Section 3.2 as supporting context.

---

## Part 6 — Create and Export the Member Dossier

1. From within the substance dataset, click **Create Dossier**

2. The working context selected in Part 1.2 ("REACH Registration member of a joint submission
   — general case") determines the dossier template. IUCLID will create a dossier in that
   format. Confirm the dossier type shown matches a joint submission / member registration.

3. Click **Advanced** or expand **Advanced Options**
4. Tick **"Include Legal Entity"** — required for UK REACH submissions
5. Confirm Portland Fuel Ltd is selected

6. Look for **"Information provided by the lead on behalf of the member(s)"** — tick both:
   - **Chemical safety report** — the CSR is held by the lead registrant (resolves TCC_13_02)
   - **Guidance on safe use** — held by the lead registrant (resolves TCC_11_04)

   > If these checkboxes are not visible at this step, look for them in the dossier header
   > after the dossier is created (a field called "Information provided by lead registrant").

7. Name the dossier: `Portland_Fuel_HVO_NRES_MemberDossier_2026`
8. Click **Create** or **Finish**
9. Click **Export** or **Download** to save the `.i6z` file to your computer

> **Format version:** When exporting, IUCLID may offer a version selector or an **"Export
> to previous major version"** option. The Article 26 Inquiry required this — apply the
> same choice here. If unsure, try the default first; if the Comply portal rejects the
> `.i6z`, re-export using the previous major version option.

---

## Part 7 — Submit via the Comply with UK REACH Portal

1. Go to <a href="https://comply-chemical-regulations.service.gov.uk/" target="_blank">comply-chemical-regulations.service.gov.uk</a>
2. Sign in with Government Gateway credentials

> **Navigation note:** Registration submission is NOT under "Make a New Submission"
> (those options are: Inquiry, PPORD, DUR, SIAN — none are for registrations).
> Look instead for a **"Registrations"**, **"My Registrations"**, or **"Joint Submissions"**
> section on the dashboard. If the correct location is not clear, email
> ukreachitsupport@defra.gov.uk and ask where to upload a member registration dossier
> (.i6z) for an NRES substance. Update this guide with the correct path once confirmed.

3. Navigate to the registration/joint submission section
4. Upload the `.i6z` file
5. Review the submission summary and click **Submit**
6. **Record the submission reference number** in `HVO_NRES_Action_Tracker.md`
7. Pay HSE fee: **£399** (Small SME rate — <50 employees, ≤€10m turnover)
8. Email acknowledgement will be sent to steve@portland-fuel.co.uk

---

## What Happens Next

| Timeframe | Event |
|-----------|-------|
| Immediately | Email acknowledgement from HSE/Defra |
| Up to 15 working days | HSE processes the member dossier and issues a UK REACH registration number |
| On registration number issued | **We can begin importing HVO** — the member dossier registration satisfies the "register before importing" requirement |
| Shortly after | HSE places us in the HVO substance group |
| After placement | HSE facilitates contact with the substance group; negotiate LoA with lead registrant |
| By 27 Oct 2030 | Full registration dossier with all tonnage-band data submitted — required to **continue** importing beyond this date |

> Before the first import shipment, email ukreachitsupport@defra.gov.uk to confirm
> our registration number from the member dossier is sufficient to begin importing
> under the NRES pathway. Quote inquiry ref 0000025852-5.

---

## Troubleshooting

| Problem | Cause | Fix |
|---------|-------|-----|
| "Registration" not in dossier type list | Working Context not UK REACH | Switch to UK REACH in top-right corner |
| Section 13.2 not visible | Wrong dossier template or context | Confirm Working Context is UK REACH; contact HSE IT if missing |
| Member option not available at dossier creation | IUCLID version or context issue | Try selecting Registration first, then look for member option in advanced settings; alternatively, contact ukreachitsupport@defra.gov.uk |
| Portal rejects `.i6z` | Legal Entity not included | Re-export with Advanced Options — tick "Include Legal Entity" |
| BR validation failure on data endpoints | Data not held by us (expected for NRES) | Leave endpoint sections empty; the Section 13.2 waiver addresses the gap |
| "Create Dossier" button unavailable | Legal Entity not set or wrong context | Ensure Portland Fuel Ltd Legal Entity exists and Working Context is UK REACH |

---

## Key Reference Data

| Field | Value |
|-------|-------|
| Substance name | Renewable hydrocarbons (diesel type fraction) |
| EC Number | 700-571-2 |
| CAS Number | 928771-01-1 |
| Substance type | UVCB |
| Existing UK REACH registration | UK-01-9638319484-0-0004 |
| Tonnage band | 10–100 tonnes/year |
| Article 26 Inquiry ref | 0000025852-5 |
| Pathway | NRES — member dossier with data waiver |
| Registration deadline | 27 October 2030 |
| Company contact | steve@portland-fuel.co.uk |
| HSE IT Support | ukreachitsupport@defra.gov.uk |
| Comply with UK REACH portal | comply-chemical-regulations.service.gov.uk |
| IUCLID 6 Cloud | iuclid6.echa.europa.eu |
