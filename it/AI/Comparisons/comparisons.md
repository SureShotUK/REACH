# AI Privacy Policy Comparison

**Topic:** Data Retention, Training Use, and Opt-Out Controls  
**Providers:** Anthropic Claude · OpenAI ChatGPT · Microsoft Copilot · Google Gemini  
**Date:** June 2026  
**Applies to:** Consumer / personal accounts unless otherwise noted

---

## Overview

The four dominant AI assistant providers each have materially different approaches to how they collect, retain, and use the conversations you have with their products. The most commercially significant questions are:

1. **How long is your data kept?**
2. **Is your data used to train the underlying AI model?**
3. **Can you opt out, and what does opting out actually cover?**

This document answers those questions for each provider and closes with a side-by-side comparison table.

---

## 1. Anthropic Claude (claude.ai)

### General Policy

Anthropic's privacy policy (effective July 8, 2026) covers all consumer products under claude.ai including Free, Pro, and Max plans. Enterprise, Government, and Education plans — and all API access — are governed by separate commercial agreements and are **not** subject to this consumer policy.

Anthropic collects: identity/contact data, payment information, and all inputs and outputs (your prompts, Claude's responses, and any files or connected services you interact with).

<a href="https://www.anthropic.com/legal/privacy" target="_blank">Anthropic Privacy Policy</a>

### Data Retention

Anthropic introduced a **two-tier retention model** in 2025:

| Scenario | Retention Period |
|---|---|
| You **opt in** to model improvement | **5 years** |
| You **opt out** (or do not actively opt in) | **30 days** |
| You delete a conversation | Excluded from training; removed from future use |
| Enterprise / commercial plans | 30 days minimum; custom controls available |

The 5-year option was introduced to allow Anthropic to use longer conversation histories for training. If you take no action on the opt-in prompt, the 30-day default applies.

<a href="https://support.anthropic.com/en/articles/7996866-how-long-do-you-store-personal-data" target="_blank">Anthropic: How long do you store my data?</a>

### Model Training

Anthropic **may** use your inputs and outputs to train its models. This applies to Free, Pro, and Max plan users by default unless you opt out. If you delete an individual conversation from your history, it is excluded from training even if you have not opted out globally.

### Opt-Out

Opt-out is available and straightforward:

1. Go to **Settings** in claude.ai
2. Navigate to **Privacy** or **Data Controls**
3. Disable the **"Improve models for everyone"** (or equivalent) toggle

When disabled:
- No previous or new conversations are used for training
- Data is retained for only 30 days
- Deleting a chat removes it from any future training use immediately

<a href="https://support.anthropic.com/en/articles/7996868-i-want-to-opt-out-of-my-prompts-and-results-being-used-for-training-models" target="_blank">Anthropic: Is my data used for model training?</a>

### Enterprise / API

Enterprise accounts and API customers are explicitly excluded from the consumer training pipeline. Customer data is not used to train models under commercial terms. Zero-data-retention (ZDR) is available for most API use cases, though not for Fable 5 and Mythos 5 models which require a minimum 30-day retention.

---

## 2. OpenAI ChatGPT (chat.openai.com)

### General Policy

OpenAI's privacy policy covers ChatGPT consumer accounts (Free and Plus plans). It covers data collected through chat interactions, API usage, and connected integrations. OpenAI collects: account identity data, conversation content (prompts and responses), usage metadata, and device/browser information.

<a href="https://openai.com/policies/privacy-policy" target="_blank">OpenAI Privacy Policy</a>

### Data Retention

| Scenario | Retention Period |
|---|---|
| Active conversation history | Retained indefinitely until deleted |
| Deleted conversations | Removed from OpenAI systems within **30 days** |
| Temporary Chat | Not saved to history; not used for training |

As of September 26, 2025, OpenAI is no longer subject to a prior legal order requiring indefinite retention of consumer ChatGPT content. Conversations are now removed within 30 days of deletion.

### Model Training

OpenAI **does** use your conversations to improve and train its models by default on Free and Plus plans. ChatGPT improves by training on real conversations unless you opt out.

**Temporary Chat** is an exception — these conversations are never saved to history and are never used for training.

<a href="https://help.openai.com/en/articles/5722486-how-your-data-is-used-to-improve-model-performance" target="_blank">OpenAI: How your data is used to improve model performance</a>

### Opt-Out

Opt-out is available and can be done at any time:

1. Go to **Your Profile** (top-right)
2. Select **Settings**
3. Navigate to **Data Controls**
4. Switch off **"Improve the model for everyone"**

Once disabled, new conversations will not be used for training. For a permanent opt-out, OpenAI also provides a privacy portal request mechanism.

**Alternative:** Use **Temporary Chat** for any session you do not want retained or used for training — no settings change required.

<a href="https://help.openai.com/en/articles/7730893-data-controls-faq" target="_blank">OpenAI: Data Controls FAQ</a>  
<a href="https://help.openai.com/en/articles/8983130-what-if-i-want-to-keep-my-history-on-but-disable-model-training" target="_blank">OpenAI: Keep history on but disable model training</a>

### Enterprise / API

ChatGPT Business, Enterprise, Edu, and API platform users are **not** subject to training by default. Their inputs and outputs are excluded from the model improvement pipeline without any action required.

---

## 3. Microsoft Copilot (copilot.microsoft.com)

### General Policy

Microsoft Copilot is covered by the main Microsoft Privacy Statement (updated March 2026), supplemented by Copilot-specific documentation. Microsoft collects: conversation content (prompts and responses), voice input where applicable, usage and diagnostic data, and account/identity information.

<a href="https://privacy.microsoft.com/en-us/privacystatement" target="_blank">Microsoft Privacy Statement</a>  
<a href="https://support.microsoft.com/en-us/microsoft-copilot/privacy-faq-for-microsoft-copilot" target="_blank">Microsoft Copilot Privacy FAQ</a>

### Data Retention

| Scenario | Retention Period |
|---|---|
| Default conversation storage | **18 months** |
| User-deleted conversations | Deleted on request |
| Entire conversation history | Can be cleared at any time |

Users can delete individual conversations or their entire conversation history at any time through their account settings.

### Model Training

Microsoft **does** use consumer Copilot conversation activity for AI model training by default. This includes both text conversations and voice conversations (where voice features are used).

**Important caveat:** Even after opting out of model training, Microsoft may still use your conversations for:
- General product or system improvements
- Advertising purposes
- Digital safety, security, and compliance

The opt-out **only** covers the specific model training pipeline, not all uses of your data.

### Opt-Out

Opt-out is available via the Copilot mobile app:

1. Open the **Copilot mobile app**
2. Tap the **menu** (top-left or hamburger icon)
3. Select your **profile icon**
4. Go to **Account > Privacy**
5. Select **"Training on conversation activity"**
6. Disable the toggle
7. Repeat for **"Training on voice conversations"** if applicable

Opting out excludes future conversation activity from training. It does not apply retroactively to conversations already processed.

<a href="https://support.microsoft.com/en-us/topic/microsoft-copilot-privacy-controls-8e479f27-6eb6-48c5-8d6a-c134062e2be6" target="_blank">Microsoft Copilot Privacy Controls</a>

### Microsoft 365 Copilot (Enterprise)

Enterprise users of Microsoft 365 Copilot have stronger protections: prompts, work content, and responses are **not used to train foundation models** by default. This applies without any configuration needed.

<a href="https://learn.microsoft.com/en-us/microsoft-365/copilot/microsoft-365-copilot-privacy" target="_blank">Data, Privacy, and Security for Microsoft 365 Copilot</a>

---

## 4. Google Gemini (gemini.google.com)

### General Policy

Google Gemini is governed by the Google Privacy Policy plus the Gemini Apps Privacy Hub, which is a dedicated addendum for Gemini-specific data handling. Google collects: conversation prompts and responses, uploaded files and images, location information (where applicable), subscription and account data, and usage signals.

A distinctive feature of Gemini's policy is that **human reviewers** may read a subset of conversations. These reviews are used to assess response quality, safety, and accuracy.

<a href="https://support.google.com/gemini/answer/13594961" target="_blank">Gemini Apps Privacy Hub</a>  
<a href="https://policies.google.com/privacy" target="_blank">Google Privacy Policy</a>

### Data Retention

| Scenario | Retention Period |
|---|---|
| Gemini Apps Activity **on** (default) | **18 months** auto-delete (user-adjustable) |
| Auto-delete setting options | 3 months, 18 months, or 36 months |
| Gemini Apps Activity **off** | **72 hours** (for service delivery), then deleted |
| Manually deleted conversations | Excluded from future training (if not already reviewed) |

The 18-month auto-delete is the consumer default. Users can adjust this or turn auto-delete off entirely via Google account settings.

### Model Training

Google **does** use Gemini conversation data for model training and improvement. This includes:
- Improving Gemini models
- Improving other generative AI models across Google products
- Human reviewer assessment of conversation quality and safety

Unlike OpenAI and Anthropic, Google does not offer a separate "keep history but opt out of training" toggle for personal consumer accounts. The main control is the **Gemini Apps Activity** setting, which is binary: on or off.

When **Keep Activity is on**: conversations are saved for 18 months (default) and used for training.  
When **Keep Activity is off**: conversations are saved for 72 hours only and are **not** used for training.

This means opting out of training on a personal Google account also means losing conversation history.

<a href="https://support.google.com/gemini/answer/13278892" target="_blank">Manage and delete your Gemini Apps activity</a>

### Opt-Out

To stop your conversations being used for training:

1. Go to <a href="https://myactivity.google.com/product/gemini" target="_blank">myactivity.google.com/product/gemini</a>
2. Select **"Gemini Apps Activity"** or **"Keep Activity"**
3. Turn it **off**

**Effect:** Future conversations are saved for 72 hours only (not retained long-term, not used for training). Any conversations not yet reviewed by human reviewers can also be deleted to prevent their use.

**Limitation:** There is no option to retain long-term conversation history *and* opt out of model training on a personal Google account. These two features are bundled together.

### Google Workspace (Enterprise)

Google Workspace customers have stronger protections: by default, Workspace data (emails, documents, calendar, chats) is not used to train Google AI models, including Gemini. This separation is enforced at the account level.

---

## Comparison Table

| | **Anthropic Claude** | **OpenAI ChatGPT** | **Microsoft Copilot** | **Google Gemini** |
|---|---|---|---|---|
| **Default retention** | 30 days (no opt-in) | Indefinite until deleted | 18 months | 18 months |
| **After deletion** | Excluded from training | Removed within 30 days | Removed on request | Removed on request |
| **Training on by default?** | Yes (Free/Pro/Max) | Yes (Free/Plus) | Yes (consumer) | Yes (personal accounts) |
| **Can you opt out?** | Yes | Yes | Partial | Partial |
| **Opt-out preserves history?** | Yes | Yes | Yes | **No** — history lost |
| **Opt-out scope** | Full training exclusion | Full training exclusion | Training only (not all uses) | Full training exclusion |
| **Other data uses after opt-out** | Limited | Limited | Advertising, safety, product improvement | N/A (activity off) |
| **Human review of chats?** | Not stated explicitly | Not stated explicitly | Not stated explicitly | **Yes** — subset reviewed |
| **Enterprise default** | No training | No training | No training | No training |
| **Opt-out mechanism** | Settings > Privacy toggle | Settings > Data Controls toggle | Copilot app > Account > Privacy | myactivity.google.com > off |
| **Temporary/private session** | Not available | Temporary Chat available | Not available | Not available |

---

## Key Observations

**Strongest consumer privacy controls:** OpenAI and Anthropic offer the cleanest opt-out experience — you can disable model training while retaining full conversation history, with a simple toggle in settings.

**Most transparency:** Anthropic's July 2026 policy is the most recently updated and explicitly describes the two-tier retention model linked to training consent.

**Most restrictive opt-out trade-off:** Google Gemini offers no way to retain conversation history while opting out of training for personal accounts. Disabling Gemini Apps Activity is all-or-nothing, and human review of a subset of conversations adds an additional layer of data exposure not explicitly present in the other providers' policies.

**Microsoft's limitations:** Opting out of Copilot model training does not remove your data from advertising, safety, security, or general product improvement pipelines. Read the Microsoft Privacy Statement carefully — the opt-out is narrower than it appears.

**Enterprise users are consistently better protected:** All four providers exclude enterprise/business customer data from the training pipeline by default. If data privacy is a core requirement, enterprise tiers offer materially stronger guarantees than consumer plans.

---

## Sources

<a href="https://www.anthropic.com/legal/privacy" target="_blank">Anthropic Privacy Policy</a>  
<a href="https://support.anthropic.com/en/articles/7996866-how-long-do-you-store-personal-data" target="_blank">Anthropic: How long do you store my data?</a>  
<a href="https://support.anthropic.com/en/articles/7996868-i-want-to-opt-out-of-my-prompts-and-results-being-used-for-training-models" target="_blank">Anthropic: Is my data used for model training?</a>  
<a href="https://www.anthropic.com/news/updates-to-our-consumer-terms" target="_blank">Anthropic: Updates to consumer terms and privacy policy</a>  
<a href="https://openai.com/policies/privacy-policy" target="_blank">OpenAI Privacy Policy</a>  
<a href="https://help.openai.com/en/articles/5722486-how-your-data-is-used-to-improve-model-performance" target="_blank">OpenAI: How your data is used to improve model performance</a>  
<a href="https://help.openai.com/en/articles/7730893-data-controls-faq" target="_blank">OpenAI: Data Controls FAQ</a>  
<a href="https://help.openai.com/en/articles/8983130-what-if-i-want-to-keep-my-history-on-but-disable-model-training" target="_blank">OpenAI: Keep history on but disable model training</a>  
<a href="https://privacy.microsoft.com/en-us/privacystatement" target="_blank">Microsoft Privacy Statement (March 2026)</a>  
<a href="https://support.microsoft.com/en-us/microsoft-copilot/privacy-faq-for-microsoft-copilot" target="_blank">Microsoft Copilot Privacy FAQ</a>  
<a href="https://support.microsoft.com/en-us/topic/microsoft-copilot-privacy-controls-8e479f27-6eb6-48c5-8d6a-c134062e2be6" target="_blank">Microsoft Copilot Privacy Controls</a>  
<a href="https://learn.microsoft.com/en-us/microsoft-365/copilot/microsoft-365-copilot-privacy" target="_blank">Data, Privacy, and Security for Microsoft 365 Copilot</a>  
<a href="https://support.google.com/gemini/answer/13594961" target="_blank">Gemini Apps Privacy Hub</a>  
<a href="https://policies.google.com/privacy" target="_blank">Google Privacy Policy</a>  
<a href="https://support.google.com/gemini/answer/13278892" target="_blank">Manage and delete your Gemini Apps activity</a>
