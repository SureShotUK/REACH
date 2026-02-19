# AI PC Assembly Guide

**Build**: AMD Ryzen 9 7900X + MSI MAG X870E TOMAHAWK WIFI + Asus TUF RTX 3090 24GB
**Case**: Fractal Design Torrent
**Date**: February 2026

---

## Overview

This guide covers the physical assembly of the AI PC build from component preparation through first boot and BIOS configuration. Follow the steps in order — assembly sequence matters.

**Estimated time**: 3–4 hours for a careful first build.

---

## 1. Before You Start

### Tools Required

| Tool | Purpose |
|------|---------|
| Phillips head screwdriver (No.2) | All case and component screws |
| Phillips head screwdriver (No.1) | M.2 retention screws (small) |
| Anti-static wrist strap (recommended) | Prevents static discharge damage |
| Zip ties or velcro straps | Cable management |
| Scissors | Opening packaging, trimming cable ties |

### What's in the Box — Check Before Assembly

Open all boxes and verify contents against manuals. Look for:

- **CPU (Ryzen 9 7900X)**: CPU + documentation. Thermal paste purchased separately.
- **Motherboard (MSI MAG X870E TOMAHAWK WIFI)**: Board, I/O shield (may be pre-installed), M.2 screws, SATA cables, Wi-Fi antenna, manual.
- **GPU (Asus TUF Gaming OC RTX 3090)**: GPU + support bracket. **Inspect carefully** — this is used/refurbished. Check for physical damage, bent pins on PCIe connector, and all three fans spin freely.
- **RAM (G.SKILL Trident Z5 Neo RGB 64GB)**: 2x 32GB sticks.
- **PSU (Thermaltake Toughpower GF3 1650W)**: PSU + bag of modular cables.
- **Case (Fractal Design Torrent)**: Case with pre-installed fans (2x 180mm front, 3x 140mm bottom). Bag of screws inside case.
- **Storage (Samsung 9100 Pro 2TB x2)**: 2 M.2 drives. Small drives — keep packaging until you install.
- **CPU Cooler (Arctic Liquid Freezer III Pro 360)**: Radiator + pump head assembly + 3x P12 Pro fans + mounting hardware bag. Check AM5 bracket is included.
- **Rear Fan (140mm)**: Fan + screws.

### Anti-Static Precautions

Static electricity can permanently damage components, particularly the CPU, RAM, and motherboard.

- **Wear an anti-static wrist strap** connected to a metal part of the case (unpainted)
- If no wrist strap: touch an unpainted metal surface (radiator, PC case, metal door frame) before handling components and regularly throughout assembly
- Work on a hard floor or non-carpeted surface
- Keep components in their anti-static bags until the moment you install them
- Never touch the gold contacts on RAM sticks or CPU pins on the motherboard

---

## 2. Workspace Preparation

1. Clear a large table (at least 60cm × 90cm)
2. Place the **motherboard box** flat on the table — use it as a non-conductive work surface when installing components before the board goes in the case
3. Keep all screws separated by component (use the packaging foam or small cups)
4. Good lighting is essential — M.2 screws are tiny

---

## 3. Install CPU into Motherboard

**Do this before the motherboard goes into the case — much easier.**

The AM5 socket is **LGA (Land Grid Array)** — the pins are on the motherboard socket, not the CPU. The CPU has flat contact pads. This is different from older AMD sockets.

**Steps:**

1. Remove the MSI motherboard from its anti-static bag and place it on the motherboard box (component side up)
2. Locate the CPU socket — the large square socket in the top-centre of the board
3. Lift the retention arm on the left side of the socket (press down slightly, then lift outward)
4. The socket cover (plastic or metal) will lift up — some boards have a protective cover that stays in place until the CPU is seated
5. Open the CPU packaging. **Hold the CPU by its edges only** — do not touch the underside
6. Look at the socket and CPU for the **gold triangle alignment marker** — both have a small triangle in one corner. These must match
7. Gently lower the CPU straight down into the socket — it should drop in without any force. If it doesn't seat, recheck the triangle orientation. **Do not press or force it**
8. Once seated, lower the retention arm back down and secure it (requires moderate force — this is normal)
9. Set aside

**Do not apply thermal paste yet** — the Arctic Liquid Freezer III Pro comes with pre-applied thermal compound on the cold plate.

---

## 4. Install RAM

Install RAM **before** the CPU cooler — the cooler may overhang the RAM slots.

**Which slots to use**: With 2 sticks of RAM on the MSI X870E TOMAHAWK WIFI, use **slots A2 and B2** (not A1 and B1). These are the slots **second from the CPU socket edge** in each pair. Check the board for slot labels — A2 and B2 are typically the 2nd and 4th slots from the left. The board manual will have a diagram confirming this.

Using A2/B2 enables **dual-channel mode**, which doubles memory bandwidth. Using any other pair or a mismatched combination halves performance.

**Steps:**

1. Locate slots A2 and B2 on the motherboard
2. Press the retention clips at both ends of each slot **outward** to open them
3. Remove one RAM stick from its packaging. Orient it so the notch in the stick aligns with the notch in the slot (RAM is keyed — it only fits one way)
4. Place the stick in slot A2. Press down firmly and evenly on both ends until both retention clips click back into place. It takes more force than you expect — press until you hear/feel the click
5. Repeat for the second stick in slot B2
6. Verify both sticks are fully seated and the clips are locked

---

## 5. Install M.2 NVMe Drives

**Both Samsung 9100 Pro 2TB drives install before the motherboard goes into the case.** Access is much easier this way.

The MSI X870E TOMAHAWK WIFI has multiple M.2 slots. Install both drives in **M.2_1** and **M.2_2** — these are the primary PCIe 5.0 Gen5 slots. Consult the board manual to locate them; M.2_1 is typically nearest the CPU.

**Each drive installation:**

1. Locate the M.2 heatsink on the motherboard (a metal plate covering the M.2 slot). Remove the screws holding it and lift it off. Set aside — it goes back on after the drive is installed
2. Find the small retention screw hole at the far end of the M.2 slot (opposite the connector end). Some boards have a tool-less retention clip instead
3. Remove the Samsung 9100 Pro from its packaging. The drive is a small rectangular PCB — handle by the edges
4. Insert the drive **at approximately 30 degrees angle** into the M.2 slot connector. Push it in until it's fully seated
5. Press the drive flat down against the board
6. If there's a screw: install the small M.2 retention screw to hold the drive flat. The screw/standoff may need to be repositioned depending on drive length (2280 = 80mm). Samsung 9100 Pro is 2280 format
7. Replace the M.2 heatsink over the drive and secure its screws
8. Repeat for the second drive in M.2_2

---

## 6. Prepare the Case

**Remove:**
- Both side panels (typically thumb screws at the rear)
- The two 180mm **front fans** — these are being replaced by the AIO radiator. Unplug their fan cables from the board and unscrew them from the front fan mounts. Set aside as spares
- The case's front fan mounting bracket/crossbar may need to be repositioned or removed to fit the 360mm radiator. Check the Fractal Torrent manual for the exact process
- Any pre-installed standoffs that don't match the ATX layout (MSI X870E is ATX — most standoffs should already be correct, but verify)

**Keep installed:**
- The **3x 140mm bottom fans** (pre-installed, intake direction — drawing air up from the bottom vent)

**Verify the rear fan mount:** The Fractal Torrent does not include a rear fan. Your purchased 140mm fan installs here. Locate the rear 140mm fan mount — it will be installed after the motherboard is in.

**Check I/O shield:** The MSI X870E TOMAHAWK WIFI may have a pre-attached I/O shield on the board itself. If separate, install it into the rear I/O cutout of the case now (press it in from inside the case until all tabs click). The I/O shield orientation has the ports matching the board's layout.

---

## 7. Install Motherboard into Case

1. Verify the case standoffs are positioned for ATX layout (matching the mounting holes on the motherboard)
2. Hold the motherboard at an angle and carefully lower it into the case, aligning the rear I/O ports with the I/O shield cutout
3. Lower the board flat onto the standoffs. All mounting holes in the board should align with standoffs in the case
4. Start all screws by hand first (don't tighten any until all are started — this allows the board to shift slightly for alignment)
5. Once all screws are started, tighten in a diagonal cross pattern (not clockwise around the outside)
6. **Don't overtighten** — snug is sufficient. Overtightening can crack the PCB

---

## 8. Install PSU

The Thermaltake Toughpower GF3 1650W is a **fully modular** PSU — only the cables you need attach to it.

1. Locate the PSU bay at the bottom rear of the Fractal Torrent case
2. **Fan orientation**: The PSU fan should face **downward** (toward the bottom vent of the case). The Fractal Torrent has a PSU intake filter on the bottom — this draws fresh air in for the PSU
3. Slide the PSU in from the rear of the case. Some cases require sliding from the inside; check the Fractal Torrent manual
4. Secure with the 4 screws through the rear panel
5. **Do not connect cables yet** — leave modular cables disconnected until after the AIO and GPU are installed (easier routing)

---

## 9. Install CPU Cooler (Arctic Liquid Freezer III Pro 360)

This is the most involved installation step. The radiator mounts at the **front** of the Fractal Torrent where the 180mm fans were.

### Overview of Configuration

| Component | Location | Direction |
|-----------|----------|-----------|
| Radiator | Front of case | N/A |
| 3x P12 Pro fans | Front face of radiator (outside-facing) | **Intake** — pulling air from outside through radiator |
| Pump head | CPU (top of motherboard) | N/A |
| Rear fan (140mm) | Rear case mount | **Exhaust** — pushing air out |
| 3x bottom fans (stock) | Bottom of case | **Intake** — drawing air up |

### Step 1: Attach Fans to Radiator

1. Mount all three Arctic P12 Pro fans to the radiator. The fans should be on the **front face** of the radiator (the side that will face the exterior mesh of the case)
2. Fan airflow direction: air should blow **from front of case → through radiator → into case interior**. Most fans have an arrow on the frame indicating airflow direction and blade rotation direction
3. Connect the three fans together using the supplied fan cables/hub

### Step 2: Mount AM5 Bracket on Pump Head

The Arctic Liquid Freezer III Pro 360 includes AMD AM5 mounting hardware. Before installing:

1. If the pump head has a pre-installed bracket for another socket, remove it
2. Attach the AM5 mounting bracket following the Arctic installation guide included in the box
3. Do not remove the pre-applied thermal compound from the cold plate

### Step 3: Mount Radiator in Case

1. Position the radiator at the front of the Fractal Torrent, fans facing the front mesh panel
2. The radiator screws attach through the front fan mounts. Use the screws included with the Arctic cooler (not the case fan screws — the radiator screws are typically M3)
3. Route the pump head tubing so it reaches the CPU area without kinking. Position the radiator with the **tubing ports at the bottom** if possible (prevents air bubbles accumulating at the pump)
4. Secure the radiator firmly

### Step 4: Mount Pump Head on CPU

1. Clean the CPU surface if there's any contamination (there shouldn't be if the CPU is new)
2. Do **not** add additional thermal paste — the cold plate has pre-applied compound
3. Lower the pump head straight onto the CPU, ensuring the AM5 bracket posts align with the mounting holes
4. Press down gently and secure the AM5 retention screws — tighten in diagonal/cross pattern. Apply even pressure; do not overtighten
5. Tug gently on the pump head — it should not move

### Step 5: Connect AIO Cables

- **Pump power**: Connects to the CPU_FAN or AIO_PUMP header on the motherboard (check Arctic manual — the pump head may have a specific header requirement)
- **Fan hub cable**: Connects to a SYS_FAN or CHA_FAN header on the motherboard. Arctic may include a fan hub that connects all 3 fans to one header
- **USB cable**: The Arctic Liquid Freezer III Pro 360 has a USB 2.0 internal cable for the integrated VRM fan — connect to an internal USB 2.0 header on the motherboard

---

## 10. Install Rear Exhaust Fan

The purchased 140mm fan installs at the rear of the case:

1. Orient the fan so it **blows air outward** (exhausting from inside the case to outside)
2. Most 140mm fans have an arrow on the frame indicating airflow direction
3. Secure with the fan's included screws (4 screws through the fan frame into the case fan mount)
4. Connect the fan cable to a **SYS_FAN** or **CHA_FAN** header on the motherboard

---

## 11. Install GPU

The Asus TUF Gaming OC RTX 3090 is a 2.9-slot, 299.9mm card. It installs in **PCIe Slot 1** (top slot) on the MSI X870E TOMAHAWK WIFI.

1. Remove the PCIe slot cover(s) from the rear of the case — the 3090 occupies slots 1, 2, and 3 of the expansion bracket area (2.9 slots wide). Remove the corresponding blanking plates
2. **Verify the retention clip** at the end of PCIe Slot 1 on the motherboard is open (pushed outward)
3. Align the GPU's PCIe connector (gold contacts, 16-pin wide) with PCIe Slot 1
4. **Support the GPU from below** as you lower it into the slot — the 3090 is heavy and you don't want the weight pulling on the slot
5. Press down firmly until you hear the retention clip click into place
6. Secure the GPU's bracket to the case rear panel with screws (typically 2-3 screws)
7. Use the GPU support bracket if included, or use a third-party GPU sag bracket — the 3090 is heavy enough to cause PCIe slot stress over time

---

## 12. Connect All Cables

Connect cables in this order (easier to route early cables before the space fills):

### Power Cables (from PSU)

Attach the modular cables to the PSU **first**, then route them to the components.

| Cable | Connector | Destination |
|-------|-----------|-------------|
| 24-pin ATX | 24-pin socket | Motherboard right edge (large connector) |
| CPU EPS 8-pin | 8-pin CPU | Top-left of motherboard, near CPU. Labelled EPS/ATX12V |
| PCIe 8-pin (×3) | 3x 8-pin GPU | Right side of the RTX 3090 (three separate connectors) |

**Critical GPU cable note**: Use **three separate PCIe cables** from the PSU for the RTX 3090's three 8-pin connectors — do not daisy-chain two connectors off a single cable if possible. The 3090 under heavy load draws close to 350W; separate cables ensure clean power delivery.

### Data Connections (already completed via M.2)

Both Samsung 9100 Pro drives connect directly via M.2 slot — no SATA data or power cables needed for NVMe.

### Fan Headers

| Fan | Header on Board |
|-----|----------------|
| AIO pump head | CPU_FAN or AIO_PUMP |
| AIO 3x P12 Pro fans | CHA_FAN1 (or fan hub to one header) |
| Rear 140mm exhaust | CHA_FAN2 |
| Bottom fan 1 (stock Fractal) | CHA_FAN3 |
| Bottom fan 2 (stock Fractal) | CHA_FAN4 |
| Bottom fan 3 (stock Fractal) | CHA_FAN5 |

Check the MSI X870E TOMAHAWK WIFI manual for exact fan header locations. The board has sufficient headers for all fans.

### Front Panel Connections

These small cables from the case's front I/O panel connect to the **JFP1/JFP2 headers** at the bottom of the motherboard:

| Cable | Header |
|-------|--------|
| Power switch (2-pin) | JFP1: POWER SW pins |
| Reset switch (2-pin) | JFP1: RESET SW pins |
| Power LED (2 or 3-pin) | JFP1: POWER LED pins |
| HDD LED (2-pin) | JFP1: HDD LED pins |

Front I/O USB/Audio:

| Cable | Header |
|-------|--------|
| USB 3.2 Gen1 (front USB-A ×2) | JUSB3 / USB3 header |
| USB 3.2 Gen2 Type-C (front USB-C) | JUSB_C1 / USB-C header |
| Front audio (headphone/mic) | JAUD1 |

The motherboard manual includes a pinout diagram. The JFP1 header typically has a small printed label indicating which pins are which.

### Wi-Fi Antenna

Screw the two Wi-Fi antenna cables onto the antenna connectors on the I/O panel (SMA connectors). Position the antennas upright once the system is placed in its final location.

---

## 13. Cable Management

Before closing the case:

1. Route cables behind the motherboard tray (the Fractal Torrent has a generous cable routing area with cutouts and tie-down points)
2. Bundle excess cable length with zip ties or velcro straps
3. Ensure no cables are near the GPU fans or CPU cooler fans
4. Ensure the GPU PCIe cables have a clean path without straining the connectors
5. Verify the PSU modular cable connections are fully seated

---

## 14. Pre-Boot Checklist

Before first power-on, verify:

- [ ] CPU properly seated, retention arm locked
- [ ] RAM in slots A2 and B2, both fully clicked in
- [ ] Both M.2 drives seated and heatsinks replaced
- [ ] Motherboard secured with all screws
- [ ] PSU fan facing downward (drawing from bottom vent)
- [ ] AIO radiator secured at front, fans oriented as intake
- [ ] Pump head firmly mounted on CPU
- [ ] GPU fully seated in PCIe Slot 1, retention clip clicked, bracket screwed
- [ ] 24-pin ATX power cable connected to motherboard
- [ ] 8-pin EPS CPU power connected (top-left of board)
- [ ] All three 8-pin PCIe GPU cables connected
- [ ] All fan headers connected
- [ ] Front panel switch/LED cables connected
- [ ] Front USB/audio headers connected
- [ ] Wi-Fi antenna connected
- [ ] No loose screws, tools, or packaging inside the case
- [ ] GPU occupies the correct slot (Slot 1, top)

---

## 15. First Power-On

1. Connect a monitor to one of the **GPU's DisplayPort or HDMI outputs** (not the motherboard's rear I/O outputs — those use the CPU's iGPU which the Ryzen 9 7900X does not have)
2. Connect keyboard, mouse
3. Connect power cable to PSU IEC socket and switch the PSU rocker switch to **ON** (I position)
4. Press the case power button

**Expected behaviour:**
- All fans should spin up (case fans, GPU fans, AIO fans)
- The system should POST and display the MSI boot logo on the monitor
- If RAM or GPU has RGB, it will likely illuminate
- The system will pause at the boot screen asking to enter BIOS or continue

**If nothing happens (no display, no fans):**
- Check the PSU rocker switch is on
- Verify the 24-pin and 8-pin CPU power cables are fully seated
- Check the power button front panel cable is on the correct JFP1 pins
- Try reseating the GPU (press the retention clip, remove, reseat)

**If fans spin but no display:**
- GPU may not be in the correct PCIe slot (must be Slot 1)
- Monitor cable may be in the wrong port (use GPU outputs, not motherboard)
- RAM may not be seated — reseat both sticks

---

## 16. BIOS Configuration

Press **Delete** at the MSI boot logo to enter the BIOS (MSI Click BIOS 5).

Make the following changes immediately after first boot:

### Enable AMD EXPO for RAM

The G.SKILL Trident Z5 Neo RGB is AMD EXPO certified. Without enabling EXPO, the RAM runs at its default JEDEC speed (DDR5-4800) rather than the rated DDR5-6000 CL30.

1. In the MSI BIOS, navigate to **Settings → Memory**
2. Find **AMD EXPO** or **A-XMP** option
3. Select **Profile 1** (DDR5-6000 CL30 — the rated profile for your kit)
4. The BIOS will show the RAM speed update to 6000 MHz with CL30 timings

### Enable Above 4G Decoding

Required for the RTX 3090's 24GB of VRAM to be fully addressable.

1. Navigate to **Settings → Advanced → PCI Subsystem Settings**
2. Set **Above 4G Memory/Crypto Currency Mining** → **Enabled**

### Enable Resizable BAR (Smart Access Memory)

Improves GPU performance by allowing the CPU to access the full GPU memory without segmentation.

1. In the same PCI Subsystem Settings menu
2. Set **Re-Size BAR Support** → **Enabled**

### Configure Fan Curves (Optional but Recommended)

For 24/7 AI workloads, configure sensible fan curves:

1. Navigate to **Hardware Monitor** or **Smart Fan Control**
2. For the AIO fans: ramp up aggressively above 70°C CPU temperature
3. For case fans: gentle ramp to maintain airflow without excessive noise at idle

### Set Boot Device Priority

Configure the system to boot from Drive 1 (M.2_1, Samsung 9100 Pro — OS drive):

1. Navigate to **Boot → Boot Option #1**
2. Select the Samsung 9100 Pro in M.2_1

### Save and Exit

Press **F10** to save changes and reboot. The system will restart with EXPO enabled — the POST may take a few seconds longer as the memory controller trains the new frequency.

---

## 17. Post-Assembly Verification

After the system boots with BIOS configured:

1. **Verify RAM speed**: In BIOS overview or Windows/Linux, confirm RAM is running at 6000 MHz
2. **Check temperatures at idle**:
   - CPU: 35–45°C at idle (acceptable)
   - GPU: 30–45°C at idle (GPU fans may not spin below a threshold temperature — this is normal on the TUF 3090)
3. **Verify all drives are detected**: Both Samsung 9100 Pro drives should appear in the BIOS storage list
4. **Listen for unusual sounds**: No grinding, rattling, or excessive vibration

---

## 18. Troubleshooting Common Issues

| Symptom | Likely Cause | Solution |
|---------|-------------|----------|
| System won't power on | PSU switch off, or front panel cables wrong | Check PSU rocker; recheck JFP1 connections |
| No display | GPU in wrong slot, or monitor cable in MB port | Ensure GPU is in Slot 1; use GPU outputs |
| RAM speed shows 4800 MHz | EXPO not enabled | Enable AMD EXPO Profile 1 in BIOS |
| CPU temperature very high (>90°C load) | AIO pump head not mounted correctly | Reseat pump head, check thermal interface |
| GPU temperature very high (>90°C load) | Inadequate airflow | Check bottom fans are intake, GPU not obstructed |
| POST beep codes | RAM not seated | Reseat RAM, try one stick at a time |
| AIO pump audible vibrating noise | Air bubble in loop | Normal for first 15–30 mins; will clear during operation |
| Single fan not spinning | Fan header loose | Reseat cable; test on different header |

---

## Resources

- <a href="https://www.msi.com/Motherboard/MAG-X870E-TOMAHAWK-WIFI" target="_blank">MSI MAG X870E TOMAHAWK WIFI Product Page</a>
- <a href="https://www.arctic.de/en/Liquid-Freezer-III-Pro/ACFRE00136A" target="_blank">Arctic Liquid Freezer III Pro 360 Installation Guide</a>
- <a href="https://www.fractal-design.com/products/cases/torrent/torrent/black/" target="_blank">Fractal Design Torrent Specifications</a>
- <a href="https://www.asus.com/motherboards-components/graphics-cards/tuf-gaming/tuf-rtx3090-o24g-gaming/" target="_blank">Asus TUF Gaming RTX 3090 Product Page</a>

---

**Document Version**: 1.0
**Last Updated**: February 19, 2026
**Next Update**: After assembly and first boot confirmation
