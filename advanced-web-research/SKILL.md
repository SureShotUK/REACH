---
name: advanced-web-research
description: Performs iterative and refined web searches to answer user queries, evaluate responses, ask clarifying questions when needed, and always provides source references. Use when complex or nuanced information retrieval from the web is required.
---

# Advanced Web Research

## Overview
This skill guides Gemini CLI through a comprehensive web research process, ensuring accurate, well-sourced, and progressively refined answers to user queries.

## Workflow

To perform advanced web research, follow these steps:

### 1. Initial Broad Search
   - Use the `google_web_search` tool with a broad query to gather initial information.
   - Review the search results for relevance and general understanding of the topic.

### 2. Evaluate and Refine Results
   - Analyze the initial search results. Identify key terms, concepts, and potential gaps in information.
   - If a satisfactory answer can be formulated directly from the results, construct the answer and proceed to "5. Provide Final Answer and References".
   - If the initial results are too broad, insufficient, or contradictory, formulate a more specific or targeted query for the next search iteration based on identified key terms or areas needing deeper investigation.

### 3. Iterative Targeted Search
   - Use the `google_web_search` tool again with the refined query.
   - Repeat the "2. Evaluate and Refine Results" step. Continue this iterative process, narrowing down the search until sufficient information is gathered or a clear path forward is established.

### 4. Ask Clarifying Questions (If Needed)
   - If, after several iterations of web search, the information is still insufficient or ambiguous, use the `ask_user` tool to ask clarifying questions to the user.
   - Based on the user's response, return to "1. Initial Broad Search" or "3. Iterative Targeted Search" with the new information.

### 5. Provide Final Answer and References
   - Synthesize all gathered information into a comprehensive, accurate, and direct answer to the user's original query.
   - **Crucially, always include all relevant URLs from the web searches as references to support the answer.** Each piece of information should be traceable to its source.
