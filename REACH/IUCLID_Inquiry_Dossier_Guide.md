# GB REACH Article 26 Inquiry Dossier Creation Guide
## Complete Step-by-Step Process for IUCLID 6 Cloud

Based on comprehensive research of official HSE and ECHA documentation (December 2025), here is a practical guide for creating an Article 26 Inquiry dossier for Urea (CAS 57-13-6) registration under GB REACH.

## Research Summary

**Key Findings:**
1. **Mandatory First Step**: Article 26 Inquiry is mandatory before any GB REACH registration - there is no pre-registration option under UK REACH
2. **Test Account Limitation**: IUCLID 6 Cloud trial accounts CAN create dossiers, but submissions from trial accounts are not legally valid
3. **Common Issue**: The "Create Dossier" option being unavailable is typically due to missing Legal Entity setup or incomplete Working Context
4. **GB REACH System**: Uses separate "Comply with UK REACH" portal (not ECHA's system), requires Defra account
5. **Deadline**: Full registration deadline for >1000 tonnes/year is October 27, 2026 (inquiry has no deadline but must be completed first)

---

## PART 1: Prerequisites and Account Setup

### 1.1 Required Accounts

**You Need TWO Separate Accounts:**

1. **ECHA Account** (for IUCLID 6 Cloud access)
   - Create at: <a href="https://echa.europa.eu/trial-service" target="_blank">ECHA Trial Service</a>
   - Role required: "IUCLID trial Full Access"
   - Note: Trial account has 100MB storage limit, no backups, limited support
   - **Important**: For actual submission, you'll need a full IUCLID Cloud subscription

2. **Defra Account** (for UK REACH submission)
   - Create when accessing: <a href="https://www.gov.uk/guidance/how-to-comply-with-reach-chemical-regulations" target="_blank">Comply with UK REACH</a>
   - This is your UK Government Gateway ID
   - Required to submit dossiers to HSE

**Critical Understanding**: You prepare the dossier in IUCLID 6 Cloud (ECHA's system), then upload the exported .i6z file to the HSE's "Comply with UK REACH" portal. The systems are completely separate.

### 1.2 Software Access

**IUCLID 6 Cloud Trial Account Capabilities:**
- ✓ Can create substance datasets
- ✓ Can create dossiers
- ✓ Can validate dossiers
- ✓ Can export .i6z files
- ✗ Cannot make legally valid official submissions
- ✗ No data backups
- ✗ Limited to 100MB storage

**For Production Use**: Consider upgrading to full IUCLID Cloud Services subscription for official submissions, dedicated support, and data backups.

---

## PART 2: Legal Entity Setup (CRITICAL STEP)

**This is the #1 reason "Create Dossier" is not available.**

### 2.1 Understanding Legal Entities in IUCLID

- **Legal Entity** = Your company/organization in IUCLID
- All datasets and dossiers must be linked to a Legal Entity
- Without a proper Legal Entity, you cannot create dossiers

### 2.2 Create Official Legal Entity

**For GB REACH submissions, you need an "Official Legal Entity" (LEOX file):**

**Option A: For Trial/Testing** (what you likely have now)
- IUCLID trial accounts come with a test Legal Entity
- This is sufficient for learning and validation
- NOT valid for official submissions

**Option B: For Official Submissions**
1. Create Official Legal Entity on ECHA website (even for GB REACH)
2. Download the LEOX file
3. Import LEOX file into your IUCLID 6 Cloud instance
4. Use this Legal Entity as owner for all submission dossiers

**Important**: The HSE guidance confirms that ECHA's Legal Entity system is still used for GB REACH technical preparation.

### 2.3 Verify Legal Entity in IUCLID

1. Log into IUCLID 6 Cloud
2. Navigate to: **Main menu → Legal Entities**
3. Verify you have at least one Legal Entity listed
4. Note the Legal Entity name - you'll need this when creating substance datasets

---

## PART 3: Creating the Substance Dataset

### 3.1 Create New Substance Dataset

**Navigation:**
1. Log into IUCLID 6 Cloud
2. Main menu → **Substance**
3. Click **"Create new"** (or similar button)
4. Select **"Substance"** (not Mixture or Product)

**Required Information:**
- **Owner**: Select your Legal Entity from dropdown
- **Substance Type**: Mono-constituent substance
- **Working Context**: This is CRITICAL - see below

### 3.2 Set Working Context (CRITICAL)

**The Working Context determines what type of dossier you can create.**

**Location**: Upper-right corner of the substance dataset view

**For GB REACH Inquiry, select:**
- **"REACH"** or **"UK REACH"** (depending on IUCLID version)

**Common Error**: If no Working Context is selected, the "Create Dossier" option will be greyed out or unavailable.

**Note**: Recent IUCLID versions may have migrated Working Context settings. If you don't see this option, ensure you're using the latest IUCLID 6 version (should be auto-updated in Cloud).

### 3.3 Complete Essential IUCLID Sections

**For an Article 26 Inquiry, you must complete these minimum sections:**

#### **Section 1.1: Identification**

Required fields:
- **Reference Substance**: Link to IUCLID Reference Substance for Urea
  - EC Number: 200-315-5
  - CAS Number: 57-13-6
  - IUPAC Name: Urea
  - Molecular Formula: CH₄N₂O

**How to add Reference Substance:**
1. In Section 1.1, click "Add Reference Substance"
2. Search IUCLID substance inventory for "Urea" or "57-13-6"
3. Select the official IUCLID reference substance
4. This auto-populates EC/CAS numbers

#### **Section 1.2: Composition**

**Critical Section** - Must be complete for dossier creation:

1. **Add composition entry:**
   - Type: "Legal entity composition of the substance"
   - Click "Add block"

2. **Define constituents:**
   - Click "Add constituent"
   - Reference substance: Urea (same as Section 1.1)
   - Typical concentration: 100% (or your actual purity, e.g., ≥99%)
   - Concentration range: e.g., 99-100%

3. **Degree of purity**: Specify typical purity of your commercial Urea

**For Mono-constituent Substances**: One main constituent at ≥80% concentration

#### **Section 1.4: Analytical Information**

**Purpose**: Confirm substance identity and composition

Required:
- **Appearance**: Physical state (solid), color (white), form (crystalline or prilled)
- **Analytical methods**: Methods used to characterize your substance
  - Common for Urea: HPLC, Karl Fischer (moisture), nitrogen content

**You must provide analytical data** such as:
- Certificate of Analysis (attach as sanitized PDF)
- Spectroscopic data (IR, NMR if available)
- Chromatographic data (HPLC/GC)

**How to attach documents:**
1. Click "Add attachment"
2. Upload sanitized files (remove confidential manufacturing details)
3. Describe the analytical technique

#### **Section 1.3: Identifiers** (Optional but Recommended)

Additional identifiers:
- SMILES notation
- InChI/InChI Key
- Other trade names

#### **Section 14: Information Requirements**

**Purpose**: Indicate which information requirements would necessitate new studies (especially vertebrate animal testing)

This section helps facilitate data sharing discussions.

**What to include:**
- Indicate standard information requirements for your tonnage band (>1000 tonnes = Annexes VII-XI)
- Flag any studies that would require new vertebrate animal testing
- This section is less critical for the initial inquiry but should be addressed

---

## PART 4: Validate the Substance Dataset

**Before attempting to create a dossier, ALWAYS validate first.**

### 4.1 Run Validation Assistant

**Navigation:**
1. Open your substance dataset
2. Click **"Validate"** button (usually in top toolbar or right-side panel)
3. Select validation type:
   - **Submission Checks**: Critical errors that will block submission
   - **Quality Checks**: Warnings about data inconsistencies

### 4.2 Review Validation Results

**Validation Report shows:**
- **Failures (Red)**: MUST be fixed before dossier creation
- **Warnings (Amber)**: Should be addressed but won't block submission

**Common Failures:**
- Missing Reference Substance in Section 1.1
- Incomplete composition in Section 1.2
- Missing Legal Entity information
- Analytical data not provided in Section 1.4

### 4.3 Fix All Failures

**Click on each failure** - the report provides hyperlinks directly to the problem section.

Make corrections in the substance dataset and re-validate until all submission check failures are resolved.

**Important**: Dossiers are read-only snapshots. All corrections must be made in the original substance dataset, then you create a new dossier.

---

## PART 5: Create the Inquiry Dossier

**Prerequisites Checklist:**
- ✓ Legal Entity exists and is correctly set up
- ✓ Substance dataset created with correct Owner (Legal Entity)
- ✓ Working Context is set (REACH or UK REACH)
- ✓ Sections 1.1, 1.2, and 1.4 are complete
- ✓ Validation Assistant shows no critical failures

### 5.1 Initiate Dossier Creation

**Step-by-step:**
1. **Locate your Substance Dataset**
   - Navigate to: Main menu → Substances
   - Find "Urea" in your substance list
   - Click to open the substance dataset

2. **Right-click on the substance**
   - Right-click anywhere on the substance dataset name/header
   - OR look for a "Create Dossier" button in the toolbar

3. **Select "Create Dossier"**
   - If this option is greyed out, review prerequisites above
   - Common issues:
     - No Legal Entity selected as Owner
     - No Working Context set
     - Substance dataset not saved
     - Missing critical validation data

### 5.2 Dossier Creation Wizard

**The wizard will launch and guide you through several steps:**

#### **Step 1: Select Submission Type**

Choose: **"Article 26 Inquiry"** or **"REACH Inquiry"**

(Exact wording depends on IUCLID version)

#### **Step 2: Complete Dossier Header**

Required information:
- **Submitting Legal Entity**: Your company (auto-filled from dataset Owner)
- **Contact Person**: Your details or regulatory contact
- **Substance Information**: Auto-populated from Section 1
- **Tonnage Band**: Select ">1000 tonnes per year"
- **Roles**: Manufacturer and/or Importer (select your role)

**Additional Information:**
- Planned manufacture/import date
- Whether this is for GB only (Yes for UK REACH)

#### **Step 3: Review and Finalize**

The wizard will:
- Pull all relevant data from your substance dataset
- Create a read-only snapshot (.i6z format)
- Assign a local dossier identifier

**Click "Finish" or "Create Dossier"**

### 5.3 Review Created Dossier

Once created:
1. The dossier appears in your **Dossiers** list
2. You can open and review it (read-only)
3. You cannot edit it (must edit original dataset and create new dossier if changes needed)

---

## PART 6: Validate the Final Dossier

**Even though you validated the dataset, you must validate the dossier itself.**

### 6.1 Run Validation on Dossier

1. Open the created inquiry dossier
2. Click **"Validate"** button
3. Select: **Submission Checks** (critical errors)
4. Review results

### 6.2 Address Any New Issues

Sometimes the dossier creation process introduces new validation issues.

**If failures occur:**
1. Note the specific issues
2. Go back to original substance dataset
3. Make corrections
4. Create a NEW dossier (dossiers cannot be edited)
5. Re-validate until clean

**Target**: Zero failures in Submission Checks

---

## PART 7: Export Dossier for Submission

### 7.1 Export as .i6z File

**Navigation:**
1. Open your validated inquiry dossier
2. Look for **"Export"** button (usually top-right corner of dossier view)
3. Click "Export"

**Export Options:**
- Format: IUCLID 6 format (.i6z)
- Include attachments: Yes (ensure analytical data PDFs are included)

**File size note**: Trial accounts limited to 100MB total storage

### 7.2 Save the .i6z File

- Download the .i6z file to your computer
- Recommended naming: `Urea_GB-REACH_Inquiry_YYYY-MM-DD.i6z`
- Keep this file secure - it contains your submission data

**File Format**: The .i6z file is a compressed archive containing all substance data, dossier header, and attachments in standardized XML format.

---

## PART 8: Submit to HSE via Comply with UK REACH

### 8.1 Access the Submission Portal

1. Navigate to: <a href="https://www.gov.uk/guidance/how-to-comply-with-reach-chemical-regulations" target="_blank">Comply with UK REACH</a>
2. Click the link to access the service
3. Log in using your **Defra account** (UK Government Gateway ID)

**First-time users**: You'll be prompted to create a Defra account during your first access attempt.

### 8.2 Create Account in Comply with UK REACH

Follow the portal's account creation process:
- Verify your email address
- Provide company details
- Accept terms and conditions

### 8.3 Upload Inquiry Dossier

**Steps in the portal** (exact interface may vary):
1. Select **"Submit new inquiry"** or similar option
2. **Upload your .i6z file** from Step 7.2
3. The system will automatically validate the file
4. Provide any additional metadata requested

**Upload Validation**: The portal performs its own validation checks on the .i6z file.

### 8.4 Review and Submit

1. Review all information in the portal
2. Confirm accuracy
3. Click **"Submit to HSE"**

**Submission Confirmation**: You'll receive:
- Submission receipt/acknowledgment
- Reference number for tracking
- Email confirmation

### 8.5 Wait for Inquiry Number

**Timeline**: HSE aims to process inquiries promptly, but no specific SLA is published.

**Upon successful processing, you'll receive:**
- **Inquiry Number**: Your unique identifier for this substance
- Access to **Substance Group**: List of other GB REACH registrants/inquirers for Urea
- Ability to proceed with data sharing negotiations

**Next Step After Inquiry**: Use the Substance Group to identify and contact other registrants to negotiate data sharing (Letter of Access) for full registration data.

---

## PART 9: Troubleshooting Common Issues

### Issue 1: "Create Dossier" Button is Greyed Out/Missing

**Possible Causes & Solutions:**

1. **No Working Context Set**
   - Solution: Click upper-right corner of substance dataset, select "REACH" or "UK REACH"

2. **No Legal Entity**
   - Solution: Create or import Legal Entity, then edit substance dataset to set Legal Entity as Owner

3. **Substance Dataset Not Saved**
   - Solution: Save the dataset (Ctrl+S or Save button) before attempting to create dossier

4. **Trial Account Permissions**
   - Solution: Verify your ECHA account has "IUCLID trial Full Access" role

5. **IUCLID Version Issue**
   - Solution: IUCLID Cloud auto-updates, but check you're using latest version

6. **Missing Critical Data**
   - Solution: Run Validation Assistant, fix all failures in Sections 1.1, 1.2, 1.4

### Issue 2: Validation Failures

**Common Submission Check Failures:**

1. **"Reference substance not linked"**
   - Fix: In Section 1.1, search and add Urea reference substance from IUCLID inventory

2. **"Composition not provided"**
   - Fix: Complete Section 1.2 with legal entity composition including constituent(s) and concentrations

3. **"Analytical data missing"**
   - Fix: Add analytical information in Section 1.4, attach sanitized analytical reports

4. **"Legal Entity incomplete"**
   - Fix: Verify Legal Entity has complete company details

### Issue 3: Upload Fails at Comply with UK REACH Portal

**Possible Causes:**

1. **File size too large**
   - Solution: Check attachments - remove or compress large files
   - Consider sanitizing analytical reports to reduce size

2. **Validation errors in dossier**
   - Solution: Re-validate in IUCLID, ensure zero failures before export

3. **Incorrect file format**
   - Solution: Ensure you exported as .i6z format (not .i6d or other formats)

4. **Portal technical issues**
   - Solution: Contact HSE IT support: ukreachitsupport@defra.gov.uk

### Issue 4: Cannot Export Dossier

1. **Dossier not finalized**
   - Solution: Ensure dossier creation wizard completed fully

2. **Trial account limitations**
   - Solution: Check you haven't exceeded 100MB storage limit

3. **Browser/connection issues**
   - Solution: Try different browser, check internet connection

---

## PART 10: Key Differences - Inquiry vs. Full Registration

**Understanding what you're submitting now vs. later:**

| Aspect | Article 26 Inquiry (Now) | Full Registration (By Oct 2026) |
|--------|-------------------------|--------------------------------|
| **Purpose** | Notice of intent to register | Complete safety assessment |
| **Content** | Company + substance identity | All hazard/risk data |
| **IUCLID Sections** | 1.1, 1.2, 1.4, (14) | All Annexes VII-XI sections |
| **Data Requirements** | Minimal - analytical data | Extensive - physico-chem, toxicity, eco-tox, exposure |
| **Testing Required** | None | May require new studies |
| **File Size** | Small (<10MB typical) | Large (can be >100MB) |
| **Timeline** | No deadline | October 27, 2026 (>1000 t/y) |
| **Outcome** | Inquiry number + substance group access | Registration number + legal permission to manufacture/import |
| **Cost** | No fee mentioned | Registration fee applies |

---

## PART 11: Important Notes and Warnings

### Legal and Compliance Notes

1. **Trial Account Status**
   - Your IUCLID trial account can create practice dossiers
   - For official submission, verify your account status
   - Consider full IUCLID Cloud subscription for production use

2. **Data Ownership**
   - Ensure you have legal right to use analytical data
   - Sanitize all attachments to remove confidential manufacturing info

3. **Third Party Representative**
   - If you're not GB-based, you may need a TPR (Third Party Representative)
   - TPR must be a legal entity established in Great Britain

4. **Data Sharing Obligations**
   - After inquiry, you MUST attempt to share existing data
   - "One Substance, One Registration" principle applies
   - Negotiate Letter of Access (LoA) with other registrants
   - Costs for data access can be significant - budget accordingly

### Timeline Considerations

**Your Registration Pathway:**

1. **Now → Q1 2026**: Submit Article 26 Inquiry
2. **Q1 2026**: Receive inquiry number, access Substance Group
3. **Q1-Q3 2026**: Negotiate data sharing, obtain Letters of Access
4. **Q3-Q4 2026**: Prepare full registration dossier (all Annexes VII-XI)
5. **By October 27, 2026**: Submit complete registration

**Critical**: Don't wait until late 2026 - data sharing negotiations can take months.

### Data Sharing for Urea

**Urea is a high-volume commodity chemical** - expect:
- Multiple existing registrants in the Substance Group
- Established data packages available
- Potential consortium or data sharing framework
- Need to negotiate and pay for data access (LoA)

**Consider**:
- Joining existing registrant consortia if available
- Coordinating with other inquirers to share costs
- Budget for potentially significant data access fees

---

## PART 12: Resources and Support

### Official Documentation

1. **ECHA Manual: "How to prepare an inquiry dossier"**
   - <a href="https://echa.europa.eu/documents/10162/22308542/manual_inquiry_en.pdf/65b360f7-1f37-4dbd-ba29-22940273cd32" target="_blank">ECHA Inquiry Manual PDF</a>
   - Most comprehensive technical guide
   - Valid for GB REACH preparation (HSE confirmed)

2. **HSE UK REACH Guidance**
   - <a href="https://www.hse.gov.uk/reach/new-registration.htm" target="_blank">Guidance for New Registrants</a>
   - <a href="https://www.hse.gov.uk/reach/using-comply-with-uk-reach.htm" target="_blank">Using Comply with UK REACH</a>

3. **IUCLID 6 Resources**
   - <a href="https://iuclid6.echa.europa.eu/faq" target="_blank">IUCLID 6 FAQ</a>
   - <a href="https://iuclid6.echa.europa.eu/videos" target="_blank">IUCLID Video Tutorials</a> (YouTube channel)
   - <a href="https://iuclid6.echa.europa.eu/training-material" target="_blank">Training Materials</a>

4. **Submission Portal**
   - <a href="https://www.gov.uk/guidance/how-to-comply-with-reach-chemical-regulations" target="_blank">Comply with UK REACH Portal</a>

### Technical Support Contacts

1. **HSE UK REACH IT Support**
   - Email: ukreachitsupport@defra.gov.uk
   - Response time: Within 24 hours (target)
   - Scope: Account issues, portal problems, file upload errors

2. **IUCLID 6 Cloud Support**
   - Trial accounts: Limited support via online resources
   - Full subscription: Dedicated helpdesk support

3. **HSE General REACH Queries**
   - Check HSE website for regulatory guidance
   - Note: IT support (ukreachitsupport@defra.gov.uk) is for technical issues only

### Video Tutorials

While specific URLs change, search for these on IUCLID's YouTube channel:
- "IUCLID 6 Cloud getting started"
- "Creating substance datasets"
- "Dossier creation wizard"
- "Validation assistant"
- "Article 26 inquiry preparation"

---

## PART 13: Quick Reference Checklist

**Use this checklist to track your progress:**

### Pre-Dossier Preparation
- [ ] ECHA account created with "IUCLID trial Full Access" role
- [ ] Defra account created for Comply with UK REACH
- [ ] Legal Entity exists in IUCLID 6 Cloud
- [ ] DUIN registered with HSE (if you had DUIN obligations)

### Substance Dataset
- [ ] New substance created in IUCLID 6 Cloud
- [ ] Owner set to Legal Entity
- [ ] Working Context set to "REACH" or "UK REACH"
- [ ] Section 1.1: Reference substance linked (Urea, CAS 57-13-6)
- [ ] Section 1.2: Composition defined (mono-constituent, ~100%)
- [ ] Section 1.4: Analytical information provided
- [ ] Attachments sanitized and uploaded (CoA, analytical data)
- [ ] Validation Assistant run - zero failures

### Dossier Creation
- [ ] Right-clicked on substance dataset
- [ ] Selected "Create Dossier"
- [ ] Chose "Article 26 Inquiry" submission type
- [ ] Completed dossier header (company, tonnage >1000 t/y)
- [ ] Dossier creation wizard completed successfully
- [ ] Dossier appears in Dossiers list

### Validation and Export
- [ ] Opened created dossier
- [ ] Ran Validation Assistant on dossier
- [ ] All Submission Check failures resolved
- [ ] Clicked "Export"
- [ ] Downloaded .i6z file to computer
- [ ] File size reasonable (<100MB)

### Submission to HSE
- [ ] Logged into Comply with UK REACH portal
- [ ] Uploaded .i6z file
- [ ] Portal validation passed
- [ ] Submission completed
- [ ] Received submission confirmation/reference number
- [ ] Waiting for inquiry number from HSE

### Post-Inquiry
- [ ] Inquiry number received
- [ ] Substance Group accessed
- [ ] Other registrants identified
- [ ] Data sharing negotiations initiated
- [ ] Planning full registration dossier preparation

---

## PART 14: Research Sources

This guide was compiled from the following official sources:

**UK HSE Sources:**
- <a href="https://www.hse.gov.uk/reach/new-registration.htm" target="_blank">HSE: Guidance for new registrants under UK REACH</a>
- <a href="https://www.hse.gov.uk/reach/using-comply-with-uk-reach.htm" target="_blank">HSE: Using the Comply with UK REACH service</a>
- <a href="https://www.gov.uk/guidance/how-to-comply-with-reach-chemical-regulations" target="_blank">GOV.UK: Comply with UK REACH service</a>

**ECHA/IUCLID Sources:**
- <a href="https://echa.europa.eu/support/registration/creating-your-registration-dossier/how-to-create-your-registration-dossier-with-iuclid-cloud" target="_blank">ECHA: How to create your registration dossier with IUCLID Cloud</a>
- <a href="https://echa.europa.eu/support/registration/creating-your-registration-dossier/how-to-create-your-registration-dossier-with-iuclid" target="_blank">ECHA: How to create your registration dossier with IUCLID</a>
- <a href="https://echa.europa.eu/trial-service" target="_blank">ECHA: Trial Service</a>
- <a href="https://echa.europa.eu/support/dossier-submission-tools/echa-cloud-services" target="_blank">ECHA: Cloud Services</a>
- <a href="https://iuclid6.echa.europa.eu/faq" target="_blank">IUCLID 6: FAQ</a>
- <a href="https://iuclid6.echa.europa.eu/videos" target="_blank">IUCLID 6: Video Tutorials</a>

**Reference Documents:**
- ECHA Manual: "How to prepare an inquiry dossier" (Version 7, 2021) - Referenced by HSE as valid for GB REACH

**Technical Support:**
- HSE UK REACH IT Support: ukreachitsupport@defra.gov.uk

**Regulatory Status:**
All information current as of December 2025. Regulatory requirements may change - always verify with official HSE guidance before submission.

---

## Summary

The most likely reason you cannot create a dossier in your IUCLID 6 Cloud test account is **missing Legal Entity setup** or **no Working Context selected**. Follow the steps in Part 2 and Part 3.2 carefully.

For official submission, you'll need to:
1. Prepare the dossier in IUCLID 6 Cloud (following this guide)
2. Export as .i6z file
3. Upload to separate HSE "Comply with UK REACH" portal (requires Defra account)

The inquiry itself requires minimal data (substance identity and analytical confirmation), but it's critical to get right as it's your gateway to the Substance Group and full registration.

**Timeline**: With proper preparation, you should be able to complete and submit your inquiry within 1-2 weeks. Budget several months for post-inquiry data sharing negotiations before preparing your full registration dossier for the October 2026 deadline.
