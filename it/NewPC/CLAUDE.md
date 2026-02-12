# CLAUDE.md (NewPC Project)

This file provides project-specific guidance to Claude Code when working on the AI PC build project.

> **Note**: This supplements the shared CLAUDE.md files at:
> - `/terminai/CLAUDE.md` (Repository-wide shared guidance)
> - `/terminai/it/CLAUDE.md` (IT project-specific guidance)
>
> Read all three files for complete guidance.

## Project Purpose

This project documents the research, planning, and decision-making process for building a new computer system specifically designed to run AI workloads locally. The goal is to balance system capabilities with cost-effectiveness to create an optimal AI-capable machine.

## Target Audience

Documentation should be aimed at users who are:
- Competent with computers and general technical concepts
- Familiar with basic hardware components (RAM, VRAM, DDR, HDD, NVME)
- **NOT experts** in AI hardware or cutting-edge specifications
- Capable of building or configuring their own system with guidance

## Documentation Requirements

### Technical Terminology
- **Define all specialized terms** except for common components:
  - **Common terms** (no definition needed): RAM, VRAM, DDR4/DDR5, HDD, SSD, NVME
  - **Define these**: TDP, PCIe lanes, tensor cores, CUDA cores, inference, quantization, model parameters, fp16/fp32, context window, tokens per second, etc.
- Include brief explanations inline when first introducing a term
- Use analogies or real-world comparisons where helpful

### Research Standards
All research and recommendations must follow strict quality standards:

**Link Verification (CRITICAL)**
- **Every hyperlink must be tested** before inclusion using the WebFetch tool
- If a URL returns 404 or fails to load, use WebSearch to find the current correct URL
- **Never include broken links** - verify first, include second
- All links must use HTML anchor format with `target="_blank"`: `<a href="URL" target="_blank">Link Text</a>`
- Check for product page updates, moved documentation, or replaced resources

**Source Requirements**
- Cite authoritative sources:
  - Manufacturer specifications (NVIDIA, AMD, Intel)
  - Independent benchmark databases (PassMark, UserBenchmark, Tom's Hardware)
  - Technical reviews from reputable publications
  - Academic papers or whitepapers for AI-specific capabilities
- Include publication dates or last-updated dates where possible
- Cross-reference information across multiple sources
- Verify current pricing and availability (specify date checked)

**Reference Format**
- Include inline citations with working links
- Create a "Sources" or "References" section at the end of documents
- For benchmarks, specify the test conditions and date performed
- For pricing, include retailer, date checked, and currency

### Decision-Making Methodology

**Funnel Approach**
Use a systematic narrowing process to move from broad options to final recommendations:

1. **Define Requirements**
   - Identify AI workloads (LLM inference, image generation, training, etc.)
   - Establish budget constraints
   - Determine must-have vs nice-to-have features

2. **Establish Criteria**
   - Performance metrics (tokens per second, VRAM capacity, memory bandwidth)
   - Cost considerations (price-to-performance ratio)
   - Compatibility requirements
   - Power and cooling considerations
   - Future upgrade path

3. **Initial Research Phase** (Broad)
   - Survey current market landscape
   - Identify all viable options in each component category
   - Gather specifications and pricing

4. **Filtering Phase** (Medium)
   - Apply must-have criteria to eliminate non-viable options
   - Compare price-to-performance ratios
   - Identify standout options in each category

5. **Deep Analysis Phase** (Narrow)
   - Detailed comparison of top 2-3 options per component
   - Real-world benchmark analysis
   - Compatibility verification across chosen components
   - Calculate total system cost for different configurations

6. **Final Recommendation** (Specific)
   - Present 2-3 complete system configurations (e.g., "Budget", "Balanced", "Performance")
   - Justify each recommendation with data
   - Include upgrade paths and future-proofing considerations

### Cost-Capability Balance

When analyzing options, explicitly address:
- **Price-to-performance ratio**: Cost per unit of relevant metric (e.g., $ per GB VRAM, $ per TFLOP)
- **Diminishing returns**: Identify where additional cost provides minimal benefit
- **Budget tiers**: Present options at different price points (e.g., $1500, $2500, $4000)
- **Used vs new**: Consider refurbished or previous-generation options where viable
- **Bottleneck analysis**: Ensure balanced component selection (don't overspend on GPU while CPU is inadequate)

### AI-Specific Considerations

When researching and documenting AI workload requirements:

**GPU Requirements**
- VRAM capacity (most critical for LLMs)
- Memory bandwidth
- Tensor core performance (NVIDIA) or equivalent (AMD)
- Power consumption and thermal design
- Driver support for AI frameworks (PyTorch, TensorFlow, CUDA, ROCm)

**CPU Requirements**
- Core count and multi-threading for parallel processing
- PCIe lane configuration (for multiple GPUs or high-speed storage)
- Compatibility with chosen GPU and motherboard

**Memory and Storage**
- System RAM capacity (model loading, data preprocessing)
- RAM speed and timing impact on AI workloads
- Fast storage for model loading (NVME preferred)
- Storage capacity for model files and datasets

**Software Ecosystem**
- Operating system considerations (Linux vs Windows for AI frameworks)
- Container support (Docker, Podman for reproducible environments)
- Development tool compatibility

## Documentation Structure

When creating project documents, follow this structure:

### Component Research Documents
```markdown
# [Component Name] Research

## Overview
Brief description of the component's role in AI workloads

## Key Specifications for AI
What specs matter most and why

## Market Survey
Current options with specifications and pricing (verified links)

## Top Options Analysis
Detailed comparison of 2-3 best choices

## Recommendation
Final recommendation with justification

## Sources
Verified links to all references
```

### System Configuration Documents
```markdown
# AI PC Build - [Configuration Name]

## Target Use Case
What AI workloads this configuration is optimized for

## Component List
Complete parts list with prices and links

## Total Cost Breakdown

## Performance Expectations
Estimated capabilities (models it can run, inference speed, etc.)

## Assembly Considerations

## Sources
Verified links to all references
```

## File Naming Conventions

Use clear, descriptive names:
- `GPU_Research.md` - Graphics card analysis
- `CPU_Options.md` - Processor comparison
- `Build_Config_Budget.md` - Budget-tier complete build
- `Build_Config_Performance.md` - High-performance complete build
- `AI_Workload_Requirements.md` - Analysis of AI software needs
- `Cost_Analysis.md` - Price-to-performance comparisons

## Research Workflow

When beginning research on a component or topic:

1. **Use the appropriate research agent**:
   - `gemini-it-security-researcher` for security-related aspects
   - General web research for hardware specifications and pricing

2. **Verify all information**:
   - Test every link with WebFetch before including
   - Cross-reference specifications across multiple sources
   - Verify current pricing (include date checked)

3. **Document thoroughly**:
   - Include all sources with working links
   - Show your reasoning and trade-off analysis
   - Present data in comparison tables where appropriate

4. **Apply the funnel method**:
   - Start broad, narrow systematically
   - Document why options were eliminated
   - Show the decision-making path clearly

## User Preferences

Based on parent CLAUDE.md files, maintain:
- Detailed, comprehensive documentation
- Practical, actionable guidance
- Honest assessment of trade-offs
- Authoritative source citation
- Verification of all external references

## Success Criteria

This project will be successful when we have:
- ✅ Thoroughly researched all component options with verified sources
- ✅ Applied systematic filtering to narrow choices
- ✅ Created 2-3 complete system configurations at different price points
- ✅ Provided clear justification for each recommendation
- ✅ Documented in a way that is accessible to competent non-experts
- ✅ Verified all links and references are current and working
- ✅ Addressed AI-specific requirements with appropriate detail
