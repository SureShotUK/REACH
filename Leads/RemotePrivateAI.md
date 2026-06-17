# Private AI Hosting: Options for Business-Wide Access

*Last updated: June 2026 — prices are indicative and subject to change. USD costs converted at approximately £1 = $1.35.*

---

## What This Covers

Running a private AI server means your chosen language model runs on infrastructure you control — not on a third-party AI API like ChatGPT or Claude. Staff access it from any device on the business network (or via VPN from anywhere). Data never leaves your infrastructure.

**Core requirement:** a GPU-capable server running a model serving framework (e.g. <a href="https://ollama.com" target="_blank">Ollama</a> or <a href="https://github.com/vllm-project/vllm" target="_blank">vLLM</a>), accessible over the network via Tailscale or a VPN.

---

## Quick Comparison Matrix

| Option | Setup Cost | Approx. Monthly Cost | Data Stays Private | Technical Effort | Best For |
|--------|-----------|---------------------|-------------------|-----------------|----------|
| On-premise (own hardware) | £3,000–£8,000 | £50–£100 (power) | Yes — fully | Medium | Committed, long-term use |
| Dedicated GPU rental | £0 | £150–£1,200+ | Yes | Low | Hands-off, always-on |
| Cloud GPU (AWS/Azure/GCP) | £0 | £185–£2,100+ | Yes | Medium | Scalable, enterprise needs |
| Colocation (co-lo) | £3,000–£8,000 hardware | £100–£400 + hardware | Yes — fully | High | Best of both; power-user |
| AI-specialist platforms | £0 | £0–£800 (pay-as-you-go) | Partial* | Very low | Occasional or burst use |

*Data is on third-party GPU hardware but model runs privately — no AI company sees your prompts.*

---

## Option 1: On-Premise (Own Hardware, Office-Based)

You buy a GPU workstation or server and host it in your office. Staff access it via a VPN such as Tailscale. The machine runs 24/7 serving AI requests.

**Typical hardware:**

| Build | GPU | VRAM | Use Case | Approx. Cost (inc. VAT) |
|-------|-----|------|----------|------------------------|
| Entry workstation | RTX 4090 | 24 GB | Models up to ~13B params | £3,500–£5,000 |
| Mid-range workstation | 2× RTX 4090 | 48 GB | Models up to 70B params | £6,500–£9,000 |
| Professional server | NVIDIA L40S | 48 GB | 70B models, multi-user | £12,000–£20,000 |

**Ongoing costs:**
- Electricity: an RTX 4090 under load draws ~350W. At UK commercial rates (~£0.25/kWh) = roughly £60–£80/month running 24/7.
- Home broadband / business fibre: needs a static or dynamic-DNS address for inbound VPN.
- No rental or subscription fees.

**Access:** Install <a href="https://tailscale.com" target="_blank">Tailscale</a> on the server and all staff devices. Free for up to 6 users; Standard plan $8/user/month (~£5.90) for larger teams.

**Pros:**
- Lowest long-term cost if used heavily
- Complete data sovereignty — hardware never leaves the premises
- No ongoing rental or cloud bill surprises
- Full control over model selection and configuration

**Cons:**
- Large upfront capital cost
- Office internet upload speed limits access speed for remote users
- Hardware failure is your problem to fix
- Not suitable if the office has unreliable power or internet
- Requires someone technical to set up initially

---

## Option 2: Dedicated GPU Server Rental

You rent a dedicated bare-metal server with a GPU from a provider. The server is yours alone (not shared), running in a professional data centre. You install your own AI software stack.

### Hetzner (Germany — best value in Europe)

Hetzner's GPU-Line offers dedicated servers with NVIDIA GPUs at fixed monthly rates — no pay-per-hour metering.

| Server | GPU | Approx. Monthly | Notes |
|--------|-----|----------------|-------|
| GPU-Server GTX line | RTX 4000 Ada (20 GB) | ~€155–€200/month (~£130–£170) | Entry-level, suits 13B models |
| GPU-Server RTX A6000 | 48 GB VRAM | ~€350–€450/month (~£295–£380) | Solid for 70B models |

<a href="https://www.hetzner.com/dedicated-rootserver/gpu-servers/" target="_blank">Hetzner GPU Dedicated Servers</a>

### Lambda Labs

AI-focused cloud with dedicated and on-demand GPU instances.

| Instance | GPU | Hourly | Monthly (24/7) |
|----------|-----|--------|---------------|
| 1× A100 40GB | A100 | ~$1.29/hr | ~$929 (~£688) |
| 1× H100 80GB | H100 | ~$2.49/hr | ~$1,793 (~£1,328) |
| 1× RTX 4090 | RTX 4090 | ~$0.50/hr | ~$360 (~£267) |

<a href="https://lambdalabs.com/service/gpu-cloud" target="_blank">Lambda Labs GPU Cloud</a>

### RunPod

Competitive pay-as-you-go GPU cloud with both on-demand and spot pricing.

| GPU | On-Demand (per hr) | Monthly (24/7) |
|-----|--------------------|---------------|
| RTX 4090 | from $0.34/hr | ~$245 (~£181) |
| A100 80GB | from $1.64/hr | ~$1,181 (~£875) |
| H100 80GB | from $1.99/hr | ~$1,433 (~£1,061) |

<a href="https://www.runpod.io/gpu-instance/pricing" target="_blank">RunPod GPU Pricing</a>

**Pros:**
- No upfront hardware cost
- Professional data centre — redundant power, cooling, connectivity
- Easy to upgrade GPU tier
- Hetzner offers flat monthly pricing (no meter anxiety)

**Cons:**
- Monthly fee is ongoing indefinitely (no break-even vs. ownership after ~2 years)
- You are responsible for server OS, software stack, and security
- Data is in a third-party data centre (though you control encryption)
- Hetzner is in Germany — EU data residency, not UK

---

## Option 3: Cloud GPU Instances (AWS / Azure / GCP)

The major hyperscalers offer GPU virtual machines billed by the hour. These are best for scaling up and down rather than always-on inference.

**Important:** running a GPU VM 24/7 on a major cloud is expensive. These providers are better suited to burst workloads or where you already use their other services.

### AWS EC2

| Instance | GPU | vCPU | On-Demand (London) | Monthly (24/7) |
|----------|-----|------|-------------------|---------------|
| g4dn.xlarge | T4 16GB | 4 | ~$0.71/hr | ~$511 (~£378) |
| g5.xlarge | A10G 24GB | 4 | ~$1.21/hr | ~$871 (~£645) |
| p3.2xlarge | V100 16GB | 8 | ~$3.82/hr | ~$2,751 (~£2,038) |

<a href="https://aws.amazon.com/ec2/pricing/on-demand/" target="_blank">AWS EC2 On-Demand Pricing</a> — reserved instances offer ~40% discount for 1-year commitments.

### Microsoft Azure

| Instance | GPU | On-Demand (UK South) | Monthly (24/7) |
|----------|-----|---------------------|---------------|
| NC4as T4 v3 | T4 16GB | ~$0.56/hr | ~$403 (~£299) |
| NC6s v3 | V100 16GB | ~$3.06/hr | ~$2,203 (~£1,632) |
| NC A100 v4 | A100 80GB | ~$3.67/hr | ~$2,642 (~£1,957) |

<a href="https://azure.microsoft.com/en-gb/pricing/details/virtual-machines/linux/" target="_blank">Azure VM Pricing (Linux)</a>

### Google Cloud Platform

| GPU Add-on | VRAM | Per Hour | Monthly (24/7) |
|------------|------|----------|---------------|
| NVIDIA T4 | 16GB | ~$0.35/hr | ~$252 (~£187) |
| NVIDIA V100 | 16GB | ~$2.48/hr | ~$1,786 (~£1,323) |
| NVIDIA A100 40GB | 40GB | ~$2.93/hr | ~$2,110 (~£1,563) |

*(VM compute costs are additional to GPU costs on GCP.)*

<a href="https://cloud.google.com/compute/gpus-pricing" target="_blank">GCP GPU Pricing</a>

**Pros:**
- No upfront cost
- Pay only for what you use (good for occasional/burst loads)
- Enterprise SLAs, UK data residency available
- Integrates with other cloud services if already in AWS/Azure

**Cons:**
- Extremely expensive if left running 24/7
- Complex billing and cost management
- Overkill for a small business unless cloud services are already in use
- GPU availability can be limited in peak periods

---

## Option 4: Colocation (Co-lo)

You buy your own GPU server hardware but rent space, power, and internet connectivity in a professional UK data centre. You physically place (or ship) the server there.

**Typical UK colocation costs:**

| Space | Included | Approx. Monthly |
|-------|----------|----------------|
| 1U rack space | 1–2 amps power, 1Gbps port | £80–£200/month |
| 2U rack space | 2–4 amps power, 1Gbps port | £150–£350/month |
| Quarter rack | 2–5kW power, 1Gbps | £300–£700/month |

UK providers include <a href="https://www.ukfast.co.uk/colocation.html" target="_blank">ANS (formerly UKFast)</a>, <a href="https://www.pulsant.com" target="_blank">Pulsant</a>, and <a href="https://www.custodiandc.com" target="_blank">Custodian Data Centres</a>. Most require a minimum contract (typically 12 months) and prices are generally quoted on request.

**Total cost example:** Own workstation (£5,000) + 2U co-lo (£250/month) = hardware paid off in ~20 months vs. dedicated GPU rental at £250/month.

**Pros:**
- You own the hardware — long-term it's cheaper than rental
- Professional power, cooling, and multi-gigabit connectivity
- Data physically in UK, under your control
- Low latency access for staff across the UK via dedicated internet

**Cons:**
- High upfront hardware purchase
- Requires a visit to install/maintain hardware (or remote hands fee ~£50–£150/hr)
- 12-month minimum contracts typical
- Technical setup is the most complex of all options
- Hardware repair means shipping to/from the data centre

---

## Option 5: AI-Specialist Platforms (Marketplace / Serverless)

These platforms let you deploy private model endpoints on shared GPU infrastructure, billed per use. Your prompts don't go to a public AI company, but they do run on the provider's GPU hardware.

### Vast.ai

Peer-to-peer GPU marketplace — rent from individual data centres and operators. Very low prices due to market competition.

| GPU | Interruptible (cheapest) | On-Demand | Reserved |
|-----|------------------------|-----------|----------|
| RTX 3090 | from $0.10–0.20/hr | from $0.20–0.35/hr | from $0.15/hr |
| RTX 4090 | from $0.20–0.35/hr | from $0.35–0.50/hr | from $0.25/hr |
| A100 40GB | from $0.70–1.00/hr | from $1.00–1.50/hr | from $0.80/hr |

<a href="https://vast.ai/pricing" target="_blank">Vast.ai GPU Pricing</a> — prices are live/dynamic; interruptible instances can be 50%+ cheaper than on-demand.

### Together AI

Dedicated inference endpoints — you deploy a model exclusively, billed per hour.

| Tier | Hourly Rate | Notes |
|------|------------|-------|
| Dedicated Inference | $6.49–$11.95/hr | Dedicated GPU endpoint for your model |
| GPU Clusters (on-demand) | $5.49–$9.95/hr per GPU | H100 / B200 clusters |
| GPU Clusters (reserved) | $3.99–$9.65/hr per GPU | 7–180+ day commitments |

<a href="https://www.together.ai/pricing" target="_blank">Together AI Pricing</a>

**Pros:**
- Zero setup — deploy a model in minutes
- No hardware responsibility
- Pay only when actively using
- Good for testing before committing to own infrastructure

**Cons:**
- Data is processed on third-party hardware (acceptable risk for most, not for highly sensitive data)
- Costs escalate quickly if used continuously
- Less control over hardware configuration and model performance
- Vast.ai interruptible instances can be reclaimed mid-use

---

## Networking: How Staff Access the AI Server

Regardless of which hosting option is chosen, staff need a way to reach the server securely from any device.

### Tailscale (Recommended)

<a href="https://tailscale.com" target="_blank">Tailscale</a> creates an encrypted peer-to-peer mesh VPN using WireGuard. The AI server and all staff devices join the same private network regardless of location.

| Plan | Cost | Users |
|------|------|-------|
| Personal | Free | Up to 6 users |
| Standard | $8/user/month (~£5.90) | Unlimited users |

- Install on the AI server, and on each staff laptop/phone/desktop
- Staff browse to `http://<server-tailscale-ip>:11434` to access Ollama, for example
- No port forwarding, no static IP, no firewall changes required
- Works over any internet connection including home broadband and mobile

### Alternative: WireGuard (self-hosted)

WireGuard is free and open-source. More effort to configure than Tailscale but no per-user cost. Suitable if you already have a network admin or want full control.

---

## Recommendation for Portland Fuel

For a small business wanting privacy-first access with low management overhead:

**Short-term / trial:**
- Start with <a href="https://www.runpod.io/gpu-instance/pricing" target="_blank">RunPod</a> (RTX 4090 on-demand, ~$0.34/hr) or <a href="https://www.hetzner.com/dedicated-rootserver/gpu-servers/" target="_blank">Hetzner GPU dedicated</a> (~£130–£170/month flat). Run <a href="https://ollama.com" target="_blank">Ollama</a> on the server, connect all staff via Tailscale (free plan).
- Models to run: **Llama 3.1 8B** (fast, very capable) on RTX 4090; **Llama 3.1 70B** requires at least 48GB VRAM (2× RTX 4090 or A100 80GB).

**Long-term / committed:**
- An on-premise workstation with 1–2× RTX 4090 (£5,000–£8,000 once) + Tailscale becomes the cheapest option after ~18 months compared to rental.
- Alternatively, a Hetzner dedicated GPU server at ~£150/month keeps IT overhead minimal with no upfront cost.

**Avoid** AWS/Azure/GCP for always-on inference — the hourly billing makes them 3–5× more expensive than specialist GPU providers for the same hardware.

---

*All prices are indicative as of June 2026. Hardware prices from UK retailers; cloud prices from provider documentation. Exchange rate used: £1 = $1.35.*

*Sources: <a href="https://www.runpod.io/gpu-instance/pricing" target="_blank">RunPod Pricing</a> · <a href="https://lambdalabs.com/service/gpu-cloud" target="_blank">Lambda Labs</a> · <a href="https://vast.ai/pricing" target="_blank">Vast.ai</a> · <a href="https://www.together.ai/pricing" target="_blank">Together AI</a> · <a href="https://www.hetzner.com/dedicated-rootserver/gpu-servers/" target="_blank">Hetzner GPU Servers</a> · <a href="https://tailscale.com/pricing" target="_blank">Tailscale</a> · <a href="https://aws.amazon.com/ec2/pricing/on-demand/" target="_blank">AWS EC2</a> · <a href="https://azure.microsoft.com/en-gb/pricing/details/virtual-machines/linux/" target="_blank">Azure VMs</a> · <a href="https://cloud.google.com/compute/gpus-pricing" target="_blank">GCP GPU Pricing</a>*
