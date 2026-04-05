# Quick Start Guide: Database Access

**One-Page Reference for Experienced Users**

---

## ⚡ TL;DR (Too Long; Didn't Read)

Tailscale provides secure, zero-trust access to the company database and CCTV. It uses your Microsoft 365 login (Azure AD + MFA) and connects automatically. Database is accessible at `100.64.0.1:5432` when Tailscale is connected.

---

## 🚀 First-Time Setup (5 Minutes)

1. **Tailscale auto-installed** by IT (appears in system tray)
2. **Click Tailscale icon** → "Log in"
3. **Sign in with Microsoft** (your work email + MFA)
4. **Done** - Tailscale connects automatically from now on

---

## 🔌 Database Connection (DBeaver / pgAdmin)

**Connection Settings:**

```
Host:     100.64.0.1
Port:     5432
Database: your_database_name
Username: your_username
Password: your_password
SSL:      Require
```

**Connection String (for apps):**
```
postgresql://username:password@100.64.0.1:5432/database_name?sslmode=require
```

---

## 🎥 CCTV Access

**URL**: `https://192.168.10.100` *(check with IT for exact address)*

1. Ensure Tailscale is connected (green icon)
2. Open browser → Go to CCTV URL
3. Log in with CCTV credentials (separate from database)

---

## ✅ Status Check

| Icon | Status | Action |
|------|--------|--------|
| 🟢 **Green** | Connected | ✅ Ready to use |
| 🟡 **Yellow** | Connecting | Wait 10-20 sec |
| ⚪ **Grey** | Disconnected | Click → "Connect" |
| 🔴 **Red** | Error | Contact IT |

---

## 🔧 Troubleshooting (90% of Issues)

### Can't Connect to Database?

1. **Check Tailscale** - Green icon? → If not, click icon → "Connect"
2. **Check address** - Host must be `100.64.0.1` (not old VPN address)
3. **Restart database client** - Close and reopen DBeaver/pgAdmin
4. Still broken? → **Contact IT Support**

### Tailscale Won't Connect?

1. Right-click icon → "Quit Tailscale"
2. Start menu → Search "Tailscale" → Open
3. Click "Log in" if prompted
4. Still broken? → **Contact IT Support**

### Slow Performance?

1. Check internet speed: <a href="https://fast.com" target="_blank">fast.com</a> → Need 10+ Mbps
2. Close and reopen database client
3. Switch to mobile hotspot if public WiFi is slow

---

## 🛡️ Security Reminders

✅ **Keep Tailscale running** (auto-connects)
✅ **Log out of DB client** when done
✅ **Use unique passwords** (never reuse Microsoft 365 password)
❌ **Don't share credentials** (each person gets their own account)
❌ **Don't install on personal devices** (company laptops only)

**Lost laptop?** → **Immediately contact IT** (they'll disable access remotely)

---

## 🌍 Works From Anywhere

✅ Home WiFi
✅ Coffee shops / hotels
✅ Mobile hotspot (recommended for public places)
✅ International travel (Europe, Canada, worldwide)

**Note**: Database may be slightly slower from distant locations (normal).

---

## 📞 IT Support

**Email**: itsupport@yourcompany.com
**Phone**: +44 (0) 1234 567890
**Teams**: #it-support channel

**When reporting issues, include:**
- Your name and laptop hostname
- What you were trying to do
- Exact error message (screenshot if possible)
- Tailscale icon color (green/grey/red)

---

## 📚 Full Documentation

For detailed troubleshooting, security information, and FAQs:
- **Full User Guide**: `USER_GUIDE_Database_Access_via_Tailscale.md`
- **IT Knowledge Base**: <a href="https://helpdesk.yourcompany.com" target="_blank">helpdesk.yourcompany.com</a>

---

**Print this page and keep it at your desk for quick reference!**

**Version**: 1.0 | **Last Updated**: 2026-02-17
