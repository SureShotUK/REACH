# CLAUDE.md - Maintenance Administration System

This file provides project-specific guidance for the Maintenance project. It supplements the shared guidance in `/terminai/CLAUDE.md`. In any conflict, these instructions take precedence.

---

## Project Overview

An Excel-based maintenance administration system for a manufacturing/engineering business operating two sites. The system tracks statutory compliance, planned preventive maintenance, reactive repairs, and emergency repairs with full cost reporting.

**Primary Users:** Maintenance/facilities manager, admin/office staff
**Output format:** Microsoft Excel workbooks (`.xlsx`)

---

## Sites

| Ref | Name | Description |
|-----|------|-------------|
| `CITY` | City Centre Office | Office premises in city centre |
| `MFG` | Manufacturing/Warehouse Site | Manufacturing, warehouse, and separate offices on an out-of-town site |

Always ask for the site reference when raising or filtering jobs.

---

## Job Categories

| Code | Category | Description |
|------|----------|-------------|
| `STAT` | Statutory / Regulatory | Legally required under UK regulations; fixed maximum intervals |
| `PPM` | Planned Preventive Maintenance | Scheduled non-statutory maintenance to manufacturer/best-practice intervals |
| `REACT` | Reactive Maintenance | Response to reported defect; planned and scheduled |
| `EMERG` | Emergency Repair | Immediate safety-critical response; unplanned |

---

## Regulatory Framework

The following statutory maintenance regimes apply. Frequencies shown are **maximum legal intervals** unless stated otherwise. Always verify current HSE guidance before advising on specific intervals.

### Applicable at Both Sites

| Regulation / Standard | Requirement | Default Frequency |
|-----------------------|-------------|-------------------|
| Gas Safety (Installation & Use) Regs 1998 | Annual gas appliance/boiler service by Gas Safe registered engineer | 12 months |
| Electricity at Work Regs 1989 (EICR) | Fixed wiring installation inspection and test | 5 years (offices); 3 years (industrial) |
| PAT Testing | Portable appliance testing (risk-based frequency) | Risk-assessed |
| Fire Safety Order 2005 | Weekly fire alarm test; 6-monthly service by competent person | Weekly test; 6-monthly service |
| Fire Safety Order 2005 | Emergency lighting monthly function test; annual full duration test | Monthly / Annual |
| Fire Safety Order 2005 | Fire extinguisher annual service | 12 months |
| L8 / HSG274 (Legionella) | Temperature monitoring, showerhead disinfection, water risk assessment | Monthly TMC; Quarterly showerheads; 2-yearly RA |
| F-Gas Regulation (2024) | Refrigeration/AC leak checks (frequency depends on CO2e charge) | Risk-assessed by charge size |
| Asbestos Regulations 2012 | ACM condition review (if pre-2000 building) | Risk-assessed; typically annual |
| Lifts: LOLER 1998 | Thorough examination of passenger/goods lifts | 6 months (passenger); 12 months (goods) |

### Applicable at MFG Site Only

| Regulation / Standard | Requirement | Default Frequency |
|-----------------------|-------------|-------------------|
| LOLER 1998 | Thorough examination of lifting equipment (cranes, hoists, FLTs, slings, chains) | 6 months (personnel/accessories); 12 months (other) |
| PSSR 2000 | Written scheme of examination for pressure systems (compressors, vessels, autoclaves, boilers) | Per written scheme |
| PUWER 1998 | Work equipment inspection and maintenance records | Per manufacturer/risk assessment |
| HSG258 (LEV) | Thorough examination and test of local exhaust ventilation (dust, fume, mist) | 14 months maximum |
| SEMA / Storage Equipment | Racking inspections by SEMA-accredited inspector | 12 months recommended; expert inspection after damage |
| Dock Equipment | Loading dock levellers, dock seals, roller shutter doors | Per manufacturer; typically 6–12 months |

> **Note:** This list is a starting framework. Always confirm which specific assets exist on site before building the compliance schedule for that regulation. Do not assume equipment is present.

---

## System Architecture (Excel)

The system comprises the following workbooks/sheets. Each task should specify which workbook/sheet it concerns.

### Workbook 1: Asset Register (`Asset_Register.xlsx`)
Captures all maintainable assets across both sites.

**Key columns:**
- Asset ID (unique reference)
- Site (`CITY` / `MFG`)
- Location within site
- Asset description
- Asset category (Building, Mechanical, Electrical, Fire, Lifting, Pressure, HVAC, etc.)
- Make / Model / Serial number
- Installation date
- Regulatory regime(s) applicable
- In service (Yes/No)
- Notes

### Workbook 2: Compliance Schedule (`Compliance_Schedule.xlsx`)
One row per asset-per-regulatory-obligation. Drives the compliance calendar.

**Key columns:**
- Asset ID (linked to Asset Register)
- Site
- Regulation / Standard
- Job category (`STAT` or `PPM`)
- Last completed date
- Certificate / report reference
- Next due date (calculated)
- Status (Current / Due Soon / Overdue / Not Applicable)
- Contractor
- Notes

### Workbook 3: Job Log (`Job_Log.xlsx`)
Records all maintenance and repair jobs raised, in progress, or completed.

**Key columns:**
- Job ID (sequential, e.g. JOB-0001)
- Raised date
- Site
- Asset ID (if applicable)
- Location / description
- Job category (`STAT` / `PPM` / `REACT` / `EMERG`)
- Regulation / Standard (if STAT)
- Description of work
- Assigned contractor
- Priority (Critical / High / Medium / Low)
- Status (Open / In Progress / Pending Invoice / Closed / Cancelled)
- Target completion date
- Actual completion date
- Quoted cost (£)
- Actual cost (£)
- Invoice reference
- Certificate / report received (Yes/No/NA)
- Compliance Schedule updated (Yes/No/NA)
- Closed by
- Notes

### Workbook 4: Contractor Register (`Contractor_Register.xlsx`)
Approved contractor list with competency and insurance records.

**Key columns:**
- Contractor ID
- Company name
- Trade / discipline
- Contact name / phone / email
- Gas Safe number (if applicable)
- NICEIC / ECA number (if applicable)
- SEMA accreditation (if applicable)
- Public liability insurance expiry
- Employers liability insurance expiry
- Last used date
- Approved (Yes/No)
- Notes

### Sheet: Dashboard / Reports
Summary views, pivot tables, and KPIs within a reporting workbook:
- Open jobs by site and category
- Jobs overdue or due within 30/60/90 days
- YTD spend by site, category, and contractor
- Compliance status summary (Current / Due Soon / Overdue counts)

---

## Data Entry Standards

- **Dates:** Always use `DD/MM/YYYY` format in Excel cells formatted as Date
- **Costs:** Always enter as numeric values (no £ symbol in cell); apply currency formatting to columns
- **Job IDs:** Sequential format `JOB-NNNN` (e.g. JOB-0001, JOB-0042)
- **Asset IDs:** Format `SITE-CAT-NNN` (e.g. `MFG-LIFT-001`, `CITY-ELEC-001`)
- **Status values:** Use exact strings from the defined lists above to enable filtering/pivot tables
- **Contractor references:** Always use the Contractor ID from the Contractor Register

---

## Working Instructions for Claude

### Before Designing Any Sheet or Formula
1. Ask which workbook/sheet the task relates to
2. Confirm the Excel version in use (affects feature availability - e.g. XLOOKUP requires Excel 2019+)
3. Confirm whether Power Query is available (Excel 365 / 2019+) - preferred for cross-workbook lookups
4. Do not assume assets exist at a site without confirmation from the user

### When Advising on Regulatory Intervals
1. State the regulation and the source of the interval
2. Flag if the interval is risk-assessed rather than fixed
3. Advise the user to verify current HSE guidance, as regulations and ACoPs are updated
4. Never advise that a statutory inspection is "not required" without confirmation of the specific circumstances

### When Building Excel Formulas
1. State which cell the formula goes in and which columns it references
2. Use named ranges or structured table references (`Table[Column]`) rather than raw cell references where possible
3. Prefer `XLOOKUP` over `VLOOKUP` for new work (if Excel version supports it)
4. Provide formulas with explanation of logic

### When Advising on Costs or Budgets
1. Ask for actual contractor quotes rather than estimating market rates
2. If the user asks for benchmarks, clearly label them as indicative only

---

## Out of Scope

The following are explicitly out of scope for this project unless later agreed:
- Production/manufacturing process maintenance (machinery under production control) - unless the user confirms otherwise
- Vehicle fleet maintenance
- IT equipment maintenance
- HR or personnel records

---

## File Storage Convention

All project files to be stored in `/terminai/Maintenance/`:
- `/terminai/Maintenance/CLAUDE.md` - this file
- `/terminai/Maintenance/Asset_Register.xlsx`
- `/terminai/Maintenance/Compliance_Schedule.xlsx`
- `/terminai/Maintenance/Job_Log.xlsx`
- `/terminai/Maintenance/Contractor_Register.xlsx`
- `/terminai/Maintenance/docs/` - supporting documentation, certificates, inspection reports

---

## Related Projects

- `hseea/` - HSE compliance; overlaps with statutory maintenance (fire safety, COSHH, risk assessments)
- `it/` - IT infrastructure; out of scope for this project's maintenance tracking
