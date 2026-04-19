# Wake-on-LAN via Alexa

## Overview

This documents the setup to wake the Windows 11 PC (Steve's personal desktop) by saying **"Alexa, open wol machine"** followed by a PIN to any Amazon Echo device on the home network.

The solution uses:
- A Python webhook service on **amelai** (Ubuntu 24.04) that sends WOL magic packets
- **Tailscale Funnel** to expose the webhook to the public internet
- An **AWS Lambda function** as the Alexa skill backend with PIN verification
- A **Published Custom Alexa Skill** (certification required — Alexa+ blocks development skills on real devices)

---

## Architecture

```
"Alexa, open wol machine"
        ↓
Amazon Alexa Cloud
        ↓
AWS Lambda (eu-west-2) — SteveOP_WOL_Skill
        ↓
Tailscale Funnel (https://amelai.tail926601.ts.net:8443)
        ↓
wol-webhook Python service on amelai (127.0.0.1:9999)
        ↓
wakeonlan -i 192.168.1.255 34:5A:60:BD:B1:5D
        ↓
Windows 11 PC wakes up
```

**Voice flow:**
1. "Alexa, open wol machine" → Alexa asks "What's your PIN?"
2. Say "wake up [PIN]" → Alexa verifies PIN → PC wakes up

---

## Prerequisites

- amelai (192.168.1.192) running Ubuntu 24.04 with Tailscale installed
- Windows 11 PC with WOL enabled in BIOS and network adapter
- Windows 11 PC MAC address: `34:5A:60:BD:B1:5D`
- AWS account
- Amazon Developer account (same Amazon account as Alexa devices)

### Windows 11 WOL Configuration
1. BIOS → enable Wake-on-LAN for the ethernet adapter
2. Device Manager → Network Adapter → Properties → Power Management → tick **Allow this device to wake the computer**
3. Control Panel → Power Options → **Turn off fast startup** (required for WOL from full shutdown)

---

## Part 1 — wol-webhook Service on amelai

### Install wakeonlan
```bash
sudo apt update && sudo apt install -y wakeonlan
```

Test manually (PC must be off):
```bash
wakeonlan -i 192.168.1.255 34:5A:60:BD:B1:5D
```

> **Note:** The `-i 192.168.1.255` flag is required. The default broadcast address (255.255.255.255) does not reliably reach the PC on this network.

### Create the webhook script
```bash
sudo mkdir -p /opt/wol-webhook

sudo tee /opt/wol-webhook/wol_webhook.py > /dev/null << 'EOF'
#!/usr/bin/env python3
import subprocess
import os
from http.server import HTTPServer, BaseHTTPRequestHandler

SECRET = os.environ.get("WOL_SECRET", "")
MAC = "34:5A:60:BD:B1:5D"
PORT = 9999

class WOLHandler(BaseHTTPRequestHandler):
    def do_POST(self):
        if not SECRET or self.path != f"/wol/{SECRET}":
            self.send_response(403)
            self.end_headers()
            return
        subprocess.run(["wakeonlan", "-i", "192.168.1.255", MAC])
        self.send_response(200)
        self.end_headers()
        self.wfile.write(b"OK")

    def log_message(self, format, *args):
        pass

if __name__ == "__main__":
    server = HTTPServer(("127.0.0.1", PORT), WOLHandler)
    server.serve_forever()
EOF
```

### Generate a secret token
```bash
openssl rand -hex 16
```
Save this token securely — it is used in the systemd service and the Lambda function.

### Create the systemd service
```bash
sudo tee /etc/systemd/system/wol-webhook.service > /dev/null << 'EOF'
[Unit]
Description=WOL Webhook Server
After=network-online.target
Wants=network-online.target

[Service]
ExecStart=/usr/bin/python3 /opt/wol-webhook/wol_webhook.py
Environment=WOL_SECRET=YOUR_SECRET_TOKEN
Restart=on-failure
RestartSec=5

[Install]
WantedBy=multi-user.target
EOF

sudo systemctl daemon-reload
sudo systemctl enable --now wol-webhook
sudo systemctl status wol-webhook
```

---

## Part 2 — Tailscale Funnel

Expose the webhook publicly so AWS Lambda can reach it. Port 8443 is used to keep the WOL endpoint isolated from other services (all others remain tailnet-only).

```bash
sudo tailscale serve --https=8443 --bg http://localhost:9999
sudo tailscale funnel --https=8443 --bg 9999
tailscale serve status
```

Expected output:
```
# Funnel on:
#     - https://amelai.tail926601.ts.net:8443

https://amelai.tail926601.ts.net:8443 (Funnel on)
|-- / proxy http://127.0.0.1:9999
```

### Test the webhook
With the PC off, run from any machine:
```bash
curl -X POST https://amelai.tail926601.ts.net:8443/wol/YOUR_SECRET_TOKEN
```
Expected response: `OK` and the PC should begin booting.

---

## Part 3 — AWS Lambda Function

1. AWS Console → **Lambda** → **Create function**
2. Name: `SteveOP_WOL_Skill`, Runtime: **Python 3.12**, Architecture: x86_64
3. Click **Create function**
4. Paste the following code (replace `YOUR_SECRET_TOKEN` and `YOUR_PIN`):

```python
import urllib.request
import os

WEBHOOK_URL = "https://amelai.tail926601.ts.net:8443/wol/YOUR_SECRET_TOKEN"
PIN = os.environ.get("WOL_PIN", "")

def lambda_handler(event, context):
    request_type = event.get('request', {}).get('type', '')

    if request_type == 'LaunchRequest':
        return ask("What's your PIN?")

    elif request_type == 'IntentRequest':
        intent_name = event.get('request', {}).get('intent', {}).get('name', '')

        if intent_name == 'WakeComputerIntent':
            slots = event.get('request', {}).get('intent', {}).get('slots', {})
            spoken_pin = slots.get('pin_number', {}).get('value', '') or ''
            if spoken_pin == PIN:
                trigger_wol()
                return tell("Switching on your computer.")
            elif spoken_pin == '':
                return ask("What's your PIN?")
            else:
                return tell("Incorrect PIN.")

        elif intent_name in ('AMAZON.StopIntent', 'AMAZON.CancelIntent'):
            return tell("Goodbye.")

        elif intent_name == 'AMAZON.HelpIntent':
            return ask("Say wake up followed by your PIN to switch on your computer.")

    return tell("Sorry, I didn't understand that.")

def ask(text):
    return {
        "version": "1.0",
        "response": {
            "outputSpeech": {"type": "PlainText", "text": text},
            "shouldEndSession": False
        }
    }

def tell(text):
    return {
        "version": "1.0",
        "response": {
            "outputSpeech": {"type": "PlainText", "text": text},
            "shouldEndSession": True
        }
    }

def trigger_wol():
    try:
        req = urllib.request.Request(WEBHOOK_URL, method='POST')
        urllib.request.urlopen(req, timeout=8)
    except Exception:
        pass
```

5. Click **Deploy**
6. **Configuration** → **General configuration** → **Edit** → set **Timeout** to **10 seconds** → Save
7. **Configuration** → **Environment variables** → **Edit** → add:
   - Key: `WOL_PIN` / Value: your chosen PIN (digits only, e.g. `7421`)
8. Note the **Function ARN**: `arn:aws:lambda:eu-west-2:974228379033:function:SteveOP_WOL_Skill`

---

## Part 4 — Alexa Custom Skill

> **Important:** Alexa+ blocks development/unreviewed skills on real devices. The skill must be published and certified by Amazon before it works on real Echo hardware. It works correctly in the Developer Console test tab at all stages.

### Create the skill
1. Go to <a href="https://developer.amazon.com/alexa/console/ask" target="_blank">Alexa Developer Console</a>
2. **Create Skill** → Name: `WOL Machine` → Locale: **English (UK)**
3. Model: **Custom** → Hosting: **Provision your own**
4. Template: **Start from scratch** → **Create Skill**

### Configure the skill

**Invocation Name**
- Left menu → **Invocations** → **Skill Invocation Name** → `wol machine` → Save

**WakeComputerIntent**
- Left menu → **Intents** → **Add Intent** → Name: `WakeComputerIntent`
- Add slot: **+ Add Slot** → name: `pin_number` → type: `AMAZON.NUMBER`
- Add sample utterances (type `{` to trigger slot autocomplete):
  - `wake up {pin_number}`
  - `{pin_number}`
- Save

**Built-in Intents** (required for certification)
- Add `AMAZON.StopIntent`, `AMAZON.CancelIntent`, `AMAZON.HelpIntent`

**Endpoint**
- Left menu → **Endpoint** → select **AWS Lambda ARN**
- Default Region: `arn:aws:lambda:eu-west-2:974228379033:function:SteveOP_WOL_Skill`
- Save Endpoints → note the **Skill ID** shown at the top

### Add Alexa trigger to Lambda
1. AWS Lambda → `SteveOP_WOL_Skill` → **Add trigger**
2. Select **Alexa Skills Kit**
3. Paste the Skill ID: `amzn1.ask.skill.d1357b39-a05e-490a-a5d0-3c702eaee152`
4. Click **Add**

### Build the skill
- Click **Build Skill** (top right) — wait for completion

---

## Part 5 — Publishing for Certification

Required because Alexa+ blocks development skills on real devices.

### Distribution section
- **Public Name**: `WOL Machine`
- **One Sentence Description**: `Wake your home computer using a voice PIN.`
- **Full Description**: `WOL Machine lets you wake your home computer using a secure voice PIN. Simply open the skill and say your PIN to send a Wake-on-LAN signal to your computer.`
- **Example Phrases**: `Alexa, open wol machine`
- **Category**: `Smart Home`
- **Keywords**: `wake on lan, computer, WOL, smart home`
- **Privacy Policy URL**: `https://gist.github.com/SureShotUK/0bbe7d527c26dd76f2279e8d8b7f1913`
- **Icons**: 108x108 and 512x512 PNG required

### Certification submission
- **Certification** → **Validation** → **Run** → fix any errors
- **Submission** → add reviewer note explaining the skill purpose and test PIN
- **Submit for Certification** — Amazon reviews within 3-5 business days

---

## Testing

**Test tab** (Alexa Developer Console — works at any stage):
1. Enable testing → type `open wol machine`
2. Type `wake up 7421` (your PIN) → "Switching on your computer"

**Real Alexa device** (requires certified/published skill):
1. Say: **"Alexa, open wol machine"**
2. Say: **"wake up [your PIN]"**
3. PC should wake within a few seconds

**Manual webhook test** (from any machine):
```bash
curl -X POST https://amelai.tail926601.ts.net:8443/wol/YOUR_SECRET_TOKEN
```

**Check service logs on amelai**:
```bash
sudo journalctl -u wol-webhook -f
```

---

## Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| curl returns 403 | Wrong secret token in URL | Check token matches `WOL_SECRET` in systemd service |
| curl returns OK but PC doesn't wake | WOL not reaching PC | Confirm `-i 192.168.1.255` flag; check BIOS, NIC Power Management, disable Fast Startup |
| Alexa says "not supported on this device" | Alexa+ blocking unreviewed skill | Skill must be certified — works in test tab only until then |
| Alexa says "there was a problem" | Lambda timeout or error | Check Lambda timeout is 10 seconds; check CloudWatch logs |
| Stuck asking "What's your PIN?" | Slot not captured | Ensure utterance `wake up {pin_number}` has slot highlighted; rebuild skill |
| Tailscale Funnel not active after reboot | Config not persisted | Re-run: `sudo tailscale serve --https=8443 --bg http://localhost:9999` then `sudo tailscale funnel --https=8443 --bg 9999` |

---

## Notes

- **fauxmo** is installed at `/opt/fauxmo/` but disabled — UPnP/SSDP multicast does not work between wired (amelai) and WiFi (Alexa) on the Linksys Velop WHW03v2 mesh router
- **Alexa+** blocks all development/unreviewed custom skills on real Echo devices; the test simulator is unaffected
- The Tailscale Funnel exposes **only port 8443** publicly — all other services remain tailnet-only
- The secret token in the webhook URL and the voice PIN together provide two layers of security
- The `WOL_PIN` environment variable in Lambda stores the PIN — update it there if the PIN needs changing
