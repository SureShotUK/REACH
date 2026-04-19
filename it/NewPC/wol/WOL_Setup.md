# Wake-on-LAN via Alexa

## Overview

This documents the setup to wake the Windows 11 PC (Steve's personal desktop) by saying **"Alexa, open steve wol"** to any Amazon Echo device on the home network.

The solution uses:
- A Python webhook service on **amelai** (Ubuntu 24.04) that sends WOL magic packets
- **Tailscale Funnel** to expose the webhook to the public internet
- An **AWS Lambda function** as the Alexa skill backend
- A **Custom Alexa Skill** to handle the voice command

---

## Architecture

```
"Alexa, open steve wol"
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

Expose the webhook publicly so AWS Lambda can reach it.

```bash
sudo tailscale serve --https=8443 --bg http://localhost:9999
sudo tailscale funnel --https=8443 --bg 9999
tailscale serve status
```

Expected output should show:
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
4. Paste the following code (replace `YOUR_SECRET_TOKEN`):

```python
import urllib.request

WEBHOOK_URL = "https://amelai.tail926601.ts.net:8443/wol/YOUR_SECRET_TOKEN"

def lambda_handler(event, context):
    trigger_wol()
    return {
        "version": "1.0",
        "response": {
            "outputSpeech": {
                "type": "PlainText",
                "text": "Switching on your computer."
            },
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
7. Note the **Function ARN**: `arn:aws:lambda:eu-west-2:974228379033:function:SteveOP_WOL_Skill`

---

## Part 4 — Alexa Custom Skill

### Create the skill
1. Go to <a href="https://developer.amazon.com/alexa/console/ask" target="_blank">Alexa Developer Console</a>
2. **Create Skill** → Name: `steve wol` → Locale: **English (UK)**
3. Model: **Custom** → Hosting: **Provision your own**
4. Template: **Start from scratch** → **Create Skill**

### Configure the skill

**Invocation Name**
- Left menu → **Invocations** → **Skill Invocation Name** → `steve wol` → Save

**Intent**
- Left menu → **Intents** → **Add Intent** → Name: `WakeComputerIntent`
- Add sample utterances: `switch on`, `wake up`, `turn on`, `start`
- Save

**Endpoint**
- Left menu → **Endpoint** → select **AWS Lambda ARN**
- Default Region: `arn:aws:lambda:eu-west-2:974228379033:function:SteveOP_WOL_Skill`
- Save Endpoints
- Note the **Skill ID** shown at the top of the Endpoint page

### Add Alexa trigger to Lambda
1. AWS Lambda → `SteveOP_WOL_Skill` → **Add trigger**
2. Select **Alexa Skills Kit**
3. Paste the Skill ID: `amzn1.ask.skill.d1357b39-a05e-490a-a5d0-3c702eaee152`
4. Click **Add**

### Build the skill
- Click **Build Skill** (top right) — wait for completion

### Enable the skill on Alexa devices
- Alexa app → **More** → **Skills & Games** → **Your Skills** → **Dev** tab
- Find **steve wol** → enable it

---

## Testing

**Test tab** (Alexa Developer Console):
- Enable testing → type `open steve wol`
- Expected response: "Switching on your computer"

**Real Alexa device**:
- Say: **"Alexa, open steve wol"**
- PC should wake within a few seconds

**Manual webhook test** (from amelai):
```bash
curl -X POST https://amelai.tail926601.ts.net:8443/wol/YOUR_SECRET_TOKEN
```

**Check service logs**:
```bash
sudo journalctl -u wol-webhook -f
```

---

## Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| curl returns 403 | Wrong secret token in URL | Check token matches `WOL_SECRET` in systemd service |
| curl returns OK but PC doesn't wake | WOL not configured on PC | Check BIOS, NIC Power Management, disable Fast Startup |
| Alexa says "not supported on this device" | Skill not enabled or locale mismatch | Re-enable skill in Alexa app Dev tab |
| Alexa says "there was a problem" | Lambda timeout | Increase Lambda timeout to 10 seconds |
| Tailscale Funnel not active after reboot | Funnel config not persisted | Re-run `sudo tailscale funnel --https=8443 --bg 9999` |

---

## Notes

- **fauxmo** is installed at `/opt/fauxmo/` but disabled — UPnP/SSDP multicast does not work between wired (amelai) and WiFi (Alexa) on the Linksys Velop WHW03v2 mesh router
- The Tailscale Funnel exposes **only port 8443** publicly — all other services remain tailnet-only
- The secret token in the webhook URL prevents unauthorised WOL triggers
