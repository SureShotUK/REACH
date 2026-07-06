<div align="right"><img src="../NoxdownPortlandLogo.png" alt="Noxdown Portland Logo" height="60"></div>

# Risk Assessment QA Checklist

*Run every completed or revised risk assessment through this checklist before it is issued. A "No" on any starred (★) item means the assessment is not ready.*

---

## 1. Legal Sufficiency (MHSWR 1999, Reg 3 — "suitable and sufficient")

- [ ] ★ Identifies all significant hazards arising from the activity — cross-checked against the site hazard profile (combustible dust/ATEX Zone 22, LPG FLT, noise, lone working, manual handling, work at height as applicable)
- [ ] ★ States who might be harmed and how — covers employees by role, plus contractors, visitors, and drivers where relevant
- [ ] ★ Evaluates existing controls before proposing new ones
- [ ] ★ Written record exists (required — employer has 5+ employees)
- [ ] Proportionate to the actual risk — no enterprise-scale controls prescribed for a ~10-person business without justification

## 2. Method and Ratings

- [ ] ★ Likelihood and severity scored using `Risk_Assessments/Risk_Rating_Metrics.csv` — no ad-hoc scales
- [ ] Residual risk rated **after** controls, and the pre/post comparison shown
- [ ] ★ Control measures follow the hierarchy: Elimination → Substitution → Engineering → Administrative → PPE. PPE-first answers require explicit justification
- [ ] Site stated (`CITY` / `MFG`) and site-specific conditions reflected — no copy-paste from the other site
- [ ] No assets or substances assumed to exist without confirmation

## 3. Citations and References

- [ ] ★ Every regulation reference hyperlinked to legislation.gov.uk (HTML anchor, `target="_blank"`), link verified with WebFetch, and the linked text checked to contain the duty cited
- [ ] ★ HSE guidance paragraph-citation rule followed: no unnumbered paragraph cited under the nearest paragraph number — use `[L74, Guidance 3]` or `[L74, p.X]` style, verified against the source PDF
- [ ] Guidance documents current — superseded-publication check done (e.g. INDG455→LA455 pattern)
- [ ] Legal duties ("must") clearly distinguished from best practice ("should")

## 4. Actions and Review

- [ ] ★ Action table complete: every action has an owner, a priority, and a target date
- [ ] Actions are specific and verifiable ("install interlock on X by [date]", not "improve guarding")
- [ ] ★ Review date set (annual default) **and** trigger events listed (incident, process change, new equipment, regulation change)
- [ ] Assessor and date recorded; version/change note if this revises an earlier assessment

## 5. Document Standards

- [ ] Noxdown logo top-right, first line (hseea override — **not** Portland Long)
- [ ] Cross-references to related assessments in this repo instead of duplicating them (ATEX, FLT/DSEAR, noise, violence)
- [ ] Associated record/checklist forms carry the same logo and standards
- [ ] Plain English throughout — readable by the people doing the work, not just the assessor

---

*Related: `assessments/TEMPLATE-risk-assessment.md`, `assessments/TEMPLATE-coshh-assessment.md`, `Risk_Assessments/Risk_Rating_Metrics.csv`. The `hse-compliance-advisor` agent applies this checklist automatically when reviewing assessments.*
