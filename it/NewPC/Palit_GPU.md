# Palit GeForce RTX 3090 GamingPro - Compatibility & Assessment

**Date**: February 27, 2026
**Context**: Evaluation of a used Palit RTX 3090 GamingPro at £300 as a second GPU to add to the existing AI PC build.
**Existing primary GPU**: Asus TUF Gaming OC RTX 3090 24GB

---

## Card Specifications

| Specification | Details |
|---------------|---------|
| **Model** | Palit GeForce RTX 3090 GamingPro |
| **Architecture** | NVIDIA Ampere (GA102) |
| **VRAM** | 24GB GDDR6X |
| **Memory Bandwidth** | 936 GB/s |
| **Boost Clock** | 1,695 MHz (reference speed — not factory overclocked) |
| **TDP** | 350W (measured ~365W under full load) |
| **Power Connectors** | 2x 8-pin PCIe |
| **PCIe Interface** | PCIe 4.0 x16 |
| **Length** | 294mm |
| **Width** | 2.7 slots |
| **Height** | 112mm |
| **Fans** | 3x (TurboFan 3.0 blade design, semi-passive at idle) |

**Sources**: <a href="https://www.palit.com/palit/vgapro.php?id=3731&lang=en&pn=NED3090019SB-132BA&tab=sp" target="_blank">Palit Official Specifications</a> | <a href="https://www.guru3d.com/review/palit-geforce-rtx-3090-gamingpro-oc-review/" target="_blank">Guru3D Review</a>

---

## Compatibility Check

### Physical Fit

| Check | Detail | Result |
|-------|--------|--------|
| Case length clearance | Palit: 294mm — Fractal Torrent supports 420mm+ | ✅ Fits easily |
| Slot 1 card (ASUS TUF) | 2.9 slots wide | ✅ Fine |
| Slot 2 card (Palit) | 2.7 slots wide | ✅ Fine |
| **Slot spacing** | ASUS TUF (2.9 slots) in Slot 1 — must not block Slot 2 | ⚠️ **Verify before purchase** |

The slot spacing requires a physical check. The MSI X870E TOMAHAWK has two PCIe x16 slots spaced 2-3 positions apart. The ASUS TUF at 2.9 slots overhangs slightly into the gap below. Confirm the Palit card can seat fully in Slot 2 without the ASUS TUF's shroud or backplate physically obstructing it. Check the MSI X870E TOMAHAWK manual for the exact slot separation in mm.

### Power

| Component | Estimated Draw | Connectors Required |
|-----------|---------------|---------------------|
| ASUS TUF RTX 3090 (Slot 1) | ~350W | 2x 8-pin |
| Palit RTX 3090 GamingPro (Slot 2) | ~365W | 2x 8-pin |
| AMD Ryzen 9 7900X | ~170W | — |
| Other (RAM, storage, fans, motherboard) | ~80W | — |
| **Total estimated** | **~965W** | **4x 8-pin PCIe** |
| **Super Flower Leadex Titanium 1600W** | 1,600W capacity | Should have 8+ PCIe connectors |

✅ Power headroom is comfortable — the system uses approximately 60% of the PSU's capacity at full load.

**Important**: Both GPUs use 2x 8-pin connectors each. At ~365W actual draw on the Palit, two cables plus the slot power is within specification on a quality PSU. On the Super Flower Leadex Titanium this is not a concern.

**Cable check required**: Verify you have **4 separate 8-pin PCIe cables** from the Super Flower. Do not use daisy-chained cables (two connectors on one cable) on either GPU — route one dedicated cable per connector.

### PCIe Interface

✅ No issue. RTX 3090 uses PCIe 4.0 x16. The MSI X870E TOMAHAWK automatically bifurcates to PCIe 5.0 x8/x8 when a second GPU is installed — fully backwards compatible and more than sufficient bandwidth for AI workloads.

### Mixed Card Configuration

✅ Compatible for AI use. The ASUS TUF runs at 1,860 MHz boost (factory overclocked); the Palit GamingPro runs at 1,695 MHz (reference). For LLM inference via PyTorch or Ollama, frameworks handle heterogeneous GPU configurations without issue — each card operates independently on its assigned layers or workload. NVLink is not required for this use case.

---

## Card Quality Assessment

### Cooling Performance

The GamingPro is Palit's mid-tier cooler — not their flagship (the GameRock). Its 2.7-slot heatsink is thinner than premium variants, which forces the fans to work harder at sustained load.

| Card | Slots | Load Noise | Cooling Tier |
|------|-------|-----------|--------------|
| ASUS ROG Strix | 3.5 | ~38 dBA | Tier 1 — best |
| EVGA FTW3 Ultra | 3+ | ~36 dBA | Tier 1 — quietest |
| MSI Suprim X | 3.5 | ~37 dBA | Tier 1 |
| MSI Gaming X Trio | 3.0 | ~39 dBA | Tier 1-2 |
| **ASUS TUF (your card)** | **2.9** | **~40 dBA** | **Tier 2** |
| **Palit GamingPro** | **2.7** | **~43-44 dBA** | **Tier 2-3** |
| Gigabyte Eagle | 2.5 | ~41 dBA | Tier 3 |

Under sustained AI workloads the Palit will be audibly louder than your ASUS TUF. GPU core temperatures of 75-76°C are acceptable; hotspot temperatures can reach 105-108°C on degraded thermal paste — within NVIDIA's 110°C limit but worth monitoring.

### Performance

For AI/LLM inference, **all RTX 3090 variants perform identically**. Every 3090 has the same 24GB GDDR6X VRAM and 936 GB/s memory bandwidth. LLM inference is memory-bandwidth bound, not compute-clock bound. The Palit running at reference clocks (1,695 MHz vs ASUS TUF's 1,860 MHz) makes no measurable difference to token generation speed.

The Palit GamingPro is not disadvantaged for this use case.

### Palit as a Manufacturer

Palit is a legitimate, long-established GPU manufacturer (founded 1988, Taiwan) and one of the world's largest NVIDIA AIB partners. They also own the Gainward brand.

- **Tier**: Budget-to-mid — not competing with ASUS ROG/TUF or MSI at the premium end
- **Reliability**: Generally acceptable on modern cards; reliability has improved significantly over the past decade
- **UK warranty support**: Reported as poor to mixed by UK users — notably harder to deal with than ASUS or the old EVGA standard
- **UK retail presence**: Limited (less visible on Scan, Overclockers UK) compared to ASUS, MSI, Gigabyte

Warranty support is less relevant on a used purchase, but worth noting as context for the brand's standing.

### Known Issues on Used Units

| Issue | Detail |
|-------|--------|
| **Dried thermal paste** | After 4+ years, expect degraded paste. Core temps 86-88°C and hotspot 105-108°C are reported on unserviced used cards. A repaste resolves this. |
| **VRAM thermal pads** | 2mm pads front, 1mm rear. These degrade over time and can cause elevated VRAM junction temperatures. Replacement with Thermalright Odyssey or Fujipoly pads is a common fix. |
| **Fan bearings** | If the card has significant mining hours, fan bearings may be worn. Spin each fan manually before accepting the card — should rotate freely with no grinding. |
| **Coil whine** | Some reports exist; not unique to Palit, affects multiple 3090 variants. |

---

## Price Assessment

### UK Used Market (February 2026)

| Source | Typical Used RTX 3090 Price |
|--------|----------------------------|
| eBay UK average | ~£625-670 |
| Recent eBay UK low | ~£476 (October 2025) |
| **This offer** | **£300** |

At £300, this card is priced approximately **50-55% below the current used market average**. That discount warrants scrutiny — it is either a genuine bargain or indicates an issue with the card.

**Source**: <a href="https://bestvaluegpu.com/en-gb/history/new-and-used-rtx-3090-price-history-and-specs/" target="_blank">Best Value GPU: RTX 3090 UK Price History</a>

### Value for AI Workloads

A second RTX 3090 would bring the combined VRAM to **48GB**, enabling:
- 70B parameter models (e.g. Llama 3 70B) to run fully in VRAM at Q4 quantization (~40GB)
- Faster inference on large models via tensor parallelism across both GPUs
- The 48GB VRAM target originally planned for this build's upgrade path

At £300, even accounting for servicing costs, this represents strong value for an AI inference card.

---

## Pre-Purchase Checklist

Before committing to the purchase, carry out the following:

- [ ] **Mining history**: Ask seller for GPU-Z screenshots showing VRAM error rates. Extended Ethereum mining is common on 3090s and degrades VRAM.
- [ ] **Fan inspection**: Spin each of the three fans manually — should rotate freely with no grinding or resistance.
- [ ] **Stress test**: Run <a href="https://benchmark.unigine.com/superposition" target="_blank">Unigine Superposition</a> or <a href="https://www.3dmark.com/" target="_blank">3DMark</a> for 30+ minutes. Watch for visual artefacts, screen corruption, or crashes.
- [ ] **Temperature monitoring**: During stress test, monitor GPU core temp (target <85°C), hotspot temp (target <105°C), and VRAM junction temp (target <90°C) using GPU-Z or HWiNFO64.
- [ ] **Return policy**: Insist on a minimum 30-day return window. A seller confident in the card's condition should agree without hesitation.
- [ ] **Slot spacing**: Physically verify the Palit card seats fully in PCIe Slot 2 of the MSI X870E TOMAHAWK with the ASUS TUF installed in Slot 1.
- [ ] **PSU cables**: Confirm the Super Flower Leadex Titanium 1600W has at least 4 separate 8-pin PCIe cables (not daisy-chained).

### Budget for Servicing

Regardless of condition, plan to service the card before sustained AI workloads:

| Item | Estimated Cost |
|------|---------------|
| Thermal paste (e.g. Thermal Grizzly Kryonaut) | ~£8-12 |
| VRAM thermal pads (Thermalright Odyssey 2mm + 1mm) | ~£10-15 |
| **Total servicing cost** | **~£18-27** |

**Effective all-in cost**: ~£320-330 for a fully serviced second RTX 3090 24GB.

---

## Summary Verdict

| Question | Answer |
|----------|--------|
| Compatible with build? | ✅ Yes — one physical check needed (slot spacing) |
| Case fit? | ✅ 294mm well within Fractal Torrent clearance |
| Power supply sufficient? | ✅ 1600W handles dual 3090 at ~60% capacity |
| Cable requirements met? | ⚠️ Verify 4 separate 8-pin PCIe cables from Super Flower |
| Good card for AI workloads? | ✅ Yes — VRAM and bandwidth identical to all 3090 variants |
| Cooling quality? | Mid-tier — louder than ASUS TUF but adequate for the task |
| Palit brand? | Legitimate but budget-to-mid tier; limited UK support |
| £300 a good price? | ✅ Yes — ~50% below market if condition is verified |
| Overall recommendation? | **Buy, subject to pre-purchase checks and servicing budget** |

At £300 (plus ~£20-25 in servicing materials), this card delivers the 48GB combined VRAM the build was designed to eventually reach — at roughly half the expected used market price.

---

**Document Version**: 1.0
**Last Updated**: February 27, 2026
