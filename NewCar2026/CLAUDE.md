# CLAUDE.md (NewCar2026 Project)

This file provides NewCar2026-specific guidance to Claude Code when working in this project.

> **Note**: This supplements the shared CLAUDE.md at `/terminai/CLAUDE.md`. Read both files for complete guidance.

## Repository Purpose

This is a vehicle research and decision-making repository for evaluating and selecting a new car purchase planned for 2026. The repository contains research materials, vehicle comparisons, pricing analysis, and decision criteria to support an informed vehicle purchase decision.

## Primary Objectives

When working in this repository, help the user to:

1. **Research Vehicle Options**: Gather comprehensive information on potential vehicles including specs, pricing, reviews, and ratings
2. **Compare Alternatives**: Create structured comparisons across vehicles based on user priorities
3. **Analyze Total Cost**: Evaluate purchase price, ownership costs, depreciation, and long-term value
4. **Track Safety & Reliability**: Monitor safety ratings, reliability data, and owner experiences
5. **Support Decision-Making**: Organize findings and provide objective analysis to inform the purchase decision

## Document Organization

Suggested structure (create as needed):
- `/research/` - Vehicle research, specifications, and reviews
- `/comparisons/` - Side-by-side vehicle comparisons
- `/pricing/` - Pricing data, quotes, and incentives
- `/safety/` - Safety ratings and crash test results
- `/reliability/` - Reliability ratings and long-term ownership data
- `/test-drives/` - Test drive notes and impressions
- `/decisions/` - Decision criteria, scoring, and final recommendations

## Working with Automotive Information

### Key Information Categories

**Vehicle Specifications:**
- Engine options and performance
- Transmission types
- Fuel economy or EV range
- Dimensions and cargo capacity
- Towing capacity
- Available trims and packages

**Safety:**
- IIHS crash test ratings
- NHTSA safety ratings
- Standard and optional safety features
- ADAS (Advanced Driver Assistance Systems)

**Reliability:**
- Consumer Reports reliability ratings
- J.D. Power quality scores
- Common issues and recalls
- Warranty coverage

**Cost Analysis:**
- MSRP and typical transaction prices
- Lease vs. buy comparison
- Insurance estimates
- Fuel/electricity costs
- Maintenance costs
- Depreciation rates
- Available incentives and rebates

**Reviews & Opinions:**
- Professional automotive journalist reviews
- Owner reviews and experiences
- Expert recommendations
- Awards and recognitions

### Research Best Practices

**Multiple Sources**: Cross-reference information across manufacturer data, professional reviews, and owner experiences.

**Current Data**: Prioritize recent information, especially for pricing and incentives which change frequently.

**Regional Awareness**: Be aware that specs, pricing, and availability can vary significantly by market.

**Model Year Clarity**: Clearly distinguish between model years, as features and specifications often change annually.

**Total Cost Focus**: Look beyond purchase price to total cost of ownership over expected ownership period.

## NewCar2026-Specific Principles

**Objectivity**: Present balanced information including both strengths and weaknesses of each vehicle option.

**User Priorities**: Always consider the user's stated priorities (safety, reliability, cost, performance, technology, etc.) when researching and comparing vehicles.

**Evidence-Based**: Ground recommendations in data from authoritative sources, not marketing materials.

**Practical Considerations**: Consider real-world factors like local dealer availability, service network, resale value.

**Budget Awareness**: Respect the user's budget constraints and provide options across price ranges if requested.

## Typical Workflows

**Vehicle Research**: User identifies vehicle of interest → Use gemini-car-researcher to gather comprehensive information → Summarize key specs, pricing, safety, reliability → Organize findings in research folder → Identify any concerns or standout features

**Vehicle Comparison**: User requests comparison of multiple vehicles → Research each vehicle → Create structured comparison across key dimensions → Highlight differentiators → Provide objective analysis based on data

**Cost Analysis**: User requests ownership cost analysis → Gather pricing data → Research insurance, fuel, maintenance costs → Calculate total cost over ownership period → Compare financing/leasing options → Present clear cost breakdown

**Safety Evaluation**: User asks about safety → Research IIHS and NHTSA ratings → Review available safety features → Compare against alternatives → Provide clear safety assessment

**Decision Support**: User ready to decide → Review all research and comparisons → Apply user's stated priorities and criteria → Provide objective recommendation with clear rationale → Suggest next steps (test drive specific models, get quotes, etc.)

## Project-Specific Agents

This project has specialized agents available:
- `gemini-car-researcher` - Expert web research for automotive information, reviews, pricing, and specifications

## Key Considerations for 2026 Purchase

**Timing**: 2026 model year vehicles typically arrive in late 2025. Research may need to reference 2025 models until 2026 information becomes available.

**Electric Vehicles**: If considering EVs, pay special attention to:
- Real-world range vs. EPA estimates
- Charging infrastructure in user's area
- Home charging requirements and costs
- Federal and state EV incentives
- Battery warranty and long-term degradation

**Market Conditions**: Auto market conditions (inventory, pricing, incentives) can change rapidly. Recent data is critical.

**Test Drives**: No amount of research replaces actual driving experience. Always recommend test driving finalists before purchase.

## Limitations

- Cannot predict future incentives or pricing changes
- Cannot assess subjective factors like driving experience without test drive
- Regional dealer practices and negotiations vary significantly
- Used vehicle specific history requires VIN-specific research

Always encourage the user to verify critical information with dealers, test drive vehicles, and consider their specific needs and circumstances when making the final decision.
