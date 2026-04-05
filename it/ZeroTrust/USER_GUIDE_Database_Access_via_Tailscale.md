# User Guide: Secure Remote Database Access

**Version**: 1.0
**Last Updated**: 2026-02-17
**Applies To**: All staff requiring remote access to company database

---

## What is Tailscale?

**Tailscale** is a secure network that allows you to access company resources (database, CCTV) from anywhere in the world, just as if you were in the office. Think of it as a "private internet" for our company.

### Why are we using Tailscale?

✅ **More secure** - No passwords to remember, uses your existing Microsoft 365 login
✅ **Easier to use** - Connects automatically, no VPN buttons to click
✅ **Faster** - Direct connections mean better performance
✅ **Always on** - Works from home, hotels, coffee shops, anywhere with internet

### Is it safe?

**Yes, very safe:**
- Uses military-grade encryption (same as banking websites)
- Requires your Microsoft 365 login with multi-factor authentication
- Only works on company-approved laptops (managed by IT)
- Regularly audited by independent security experts
- Does NOT expose the database to the internet

---

## Getting Started

### Step 1: Initial Setup (One-Time, 5 Minutes)

Tailscale will be automatically installed on your laptop by IT. You'll see a **Tailscale icon** in your system tray (bottom-right corner of your screen, near the clock).

**What the icon looks like:**
- **Grey circle with dots** = Not connected
- **Green circle with checkmark** = Connected and ready

#### First-Time Authentication

1. **Click the Tailscale icon** in your system tray (bottom-right corner)

2. **Click "Log in"**

3. You'll see a browser window open asking you to sign in

4. **Select "Sign in with Microsoft"**

5. **Enter your work email address** (your normal Microsoft 365 email)

6. **Complete multi-factor authentication** (enter your MFA code from Microsoft Authenticator app)

7. You'll see a success message: **"You're connected!"**

8. Close the browser window - you're done!

**That's it!** Tailscale will now connect automatically whenever you start your laptop.

---

### Step 2: Connecting to the Database

Once Tailscale is running (green icon), you can connect to the database using your usual database tools.

#### Option A: Using DBeaver

DBeaver is a free, user-friendly database tool that works with PostgreSQL.

**Connection Settings:**

1. Open **DBeaver**

2. Click **Database** → **New Database Connection**

3. Select **PostgreSQL** → Click **Next**

4. Enter the following details:

   | Setting | Value |
   |---------|-------|
   | **Host** | `100.64.0.1` (this is the Tailscale address) |
   | **Port** | `5432` |
   | **Database** | `your_database_name` *(IT will provide this)* |
   | **Username** | `your_username` *(IT will provide this)* |
   | **Password** | `your_password` *(IT will provide this)* |
   | **SSL** | Require |

5. Click **Test Connection** (should show "Connected")

6. Click **Finish**

**✅ You're connected!** You can now run queries and view data.

---

#### Option B: Using pgAdmin

pgAdmin is a popular PostgreSQL management tool.

**Connection Settings:**

1. Open **pgAdmin**

2. Right-click **Servers** → **Create** → **Server**

3. On the **General** tab:
   - **Name**: `Company Database (Remote)`

4. On the **Connection** tab:

   | Setting | Value |
   |---------|-------|
   | **Host name/address** | `100.64.0.1` |
   | **Port** | `5432` |
   | **Maintenance database** | `postgres` |
   | **Username** | `your_username` *(IT will provide this)* |
   | **Password** | `your_password` *(IT will provide this)* |
   | **Save password** | ✅ (optional, for convenience) |

5. On the **SSL** tab:
   - **SSL mode**: `Require`

6. Click **Save**

**✅ Connected!** The database will appear in your server list.

---

#### Option C: Custom Applications

If you use a custom application that connects to the database:

**Update your connection string to:**
```
Host: 100.64.0.1
Port: 5432
Database: your_database_name
Username: your_username
Password: your_password
SSL Mode: Require
```

Ask your application developer or IT support if you need help updating the connection settings.

---

### Step 3: Accessing the CCTV System

Tailscale also provides secure access to the office CCTV system.

**To access CCTV:**

1. Ensure **Tailscale is connected** (green icon in system tray)

2. Open your **web browser** (Chrome, Edge, Firefox)

3. Go to the CCTV web interface:
   ```
   https://192.168.10.100
   ```
   *(IT will provide the exact address)*

4. **Log in** with your CCTV credentials (separate from database login)

5. View cameras as normal

**Note**: CCTV streaming may be slower than in-office access depending on your internet connection. This is normal.

---

## Daily Usage

### How to Know if Tailscale is Working

**Check the Tailscale icon in your system tray:**

| Icon | Status | What to Do |
|------|--------|-----------|
| 🟢 **Green checkmark** | ✅ Connected | Ready to use - no action needed |
| 🟡 **Yellow/orange** | ⚠️ Connecting | Wait 10-20 seconds |
| ⚪ **Grey/disconnected** | ❌ Not connected | Click icon → "Connect" |
| 🔴 **Red X** | ❌ Error | See troubleshooting below |

### Connecting Each Day

**Good news:** Tailscale connects **automatically** when you start your laptop!

You don't need to do anything - just turn on your laptop, sign in to Windows, and Tailscale will connect in the background.

**If Tailscale doesn't connect automatically:**
1. Click the Tailscale icon in system tray
2. Click **"Connect"**
3. Wait 10-20 seconds for connection

---

## Working From Different Locations

### Home Office

✅ **Works perfectly** - Connect to your home WiFi as normal, Tailscale will work automatically.

### Coffee Shops / Hotels / Public WiFi

✅ **Works on any WiFi** - Tailscale encrypts all your traffic, so it's safe even on public networks.

**⚠️ Security tip:** Even though Tailscale is secure, avoid accessing sensitive data on completely untrusted networks (e.g., airport WiFi). Use your mobile hotspot instead if possible.

### Mobile Hotspot (Phone as WiFi)

✅ **Recommended for maximum security** - Using your phone's 4G/5G connection is very secure and works great with Tailscale.

### International Travel (Europe, Canada, etc.)

✅ **Works worldwide** - Tailscale works from any country with internet access.

**Performance note:** Database queries may be slightly slower from Canada or distant locations, but should still be usable.

---

## Troubleshooting Common Issues

### Issue 1: Tailscale Icon is Grey (Disconnected)

**Solution:**
1. Click the Tailscale icon
2. Click **"Connect"**
3. Wait 10-20 seconds

If it stays grey:
1. Right-click Tailscale icon
2. Select **"Quit Tailscale"**
3. Open Start menu → Search for **"Tailscale"** → Open it
4. Wait for connection

Still not working? Contact IT Support.

---

### Issue 2: "Can't Connect to Database" Error

**Check these things in order:**

**1. Is Tailscale connected?**
   - Look for green checkmark in system tray
   - If grey, click icon → "Connect"

**2. Is the Tailscale address correct?**
   - Database host should be `100.64.0.1` (not your old VPN address)
   - Port should be `5432`

**3. Are your credentials correct?**
   - Try re-entering your username and password
   - Check for typos (especially in password)

**4. Is your laptop awake?**
   - If laptop was asleep, close database client and reopen it
   - Tailscale may need to re-establish connection after sleep

**5. Still not working?**
   - Contact IT Support with:
     - Your username
     - What you're trying to do
     - Exact error message (take a screenshot)

---

### Issue 3: Tailscale Keeps Asking Me to Log In

**This can happen if:**
- Your Microsoft 365 password was recently changed
- Your laptop was reimaged or reset
- Your account authentication expired (every 90 days)

**Solution:**
1. Click the Tailscale icon
2. Click **"Log in"** (or "Re-authenticate")
3. Complete Microsoft 365 login with MFA
4. You're reconnected!

**Security note:** This is intentional - regular re-authentication keeps your access secure.

---

### Issue 4: Database is Very Slow

**Normal performance:**
- Simple queries: < 1 second
- Complex reports: 5-30 seconds (same as in office)

**If everything is slow:**

**Check your internet connection:**
1. Open a web browser → Go to <a href="https://fast.com" target="_blank">fast.com</a>
2. Check your download speed:
   - **Good**: 10+ Mbps (fast enough for database work)
   - **Marginal**: 2-10 Mbps (will work but may be slower)
   - **Too slow**: < 2 Mbps (database will be very slow)

**If internet is slow:**
- Move closer to WiFi router
- Restart your WiFi router
- Switch to mobile hotspot (often faster than public WiFi)
- Contact your internet provider

**If internet is fast but database is still slow:**
- Close and reopen your database client
- Contact IT Support (may be a database server issue)

---

### Issue 5: "Access Denied" or "Permission Denied"

**This means:**
- Your account may not have access to the database you're trying to open
- Your database password may have changed
- Your account may have been disabled

**Solution:**
- Contact IT Support to verify your database permissions
- Do NOT try multiple passwords (this may lock your account)

---

### Issue 6: Tailscale Won't Install or Update

**This is managed by IT:**
- Tailscale is installed and updated automatically via company management software
- You cannot install it yourself

**If you need Tailscale:**
- Contact IT Support
- They will push the installation to your laptop remotely

---

## Security & Privacy

### What Can Tailscale See?

**Tailscale the company can see:**
- When you connect/disconnect
- Your laptop's Tailscale IP address (e.g., `100.64.0.1`)
- Which company resources you access (database, CCTV)

**Tailscale CANNOT see:**
- Your database queries
- The data you're viewing
- Your passwords
- Any other activity on your laptop

### What Can IT See?

**IT can see:**
- When you connect to the database
- Your username and IP address
- Which databases you access
- Approximate duration of connection

**IT can see (if query logging is enabled):**
- Specific database queries you run (for audit/compliance purposes)

**IT CANNOT see:**
- Your personal files on your laptop
- Your web browsing (unless on company network)
- Your emails (unless you use company email server)

### Is My Data Encrypted?

**Yes, always:**
- **Laptop → Database**: End-to-end encrypted (military-grade WireGuard protocol)
- **Laptop → CCTV**: Encrypted HTTPS connection
- **Even if you're on public WiFi**: Fully encrypted, completely safe

### Multi-Factor Authentication (MFA)

**Required:** You must complete MFA when logging in to Tailscale.

**This protects:**
- Your account (even if password is stolen, hackers can't log in)
- Company data (unauthorized devices can't access database)

**You'll be asked for MFA:**
- First time you set up Tailscale
- Every 90 days (re-authentication)
- If you log in from a new device

---

## Best Practices

### ✅ DO:

- **Keep Tailscale running** - It uses minimal battery and connects automatically
- **Log out of database client** when you're done (closes connection cleanly)
- **Use strong, unique database passwords** (never reuse your Microsoft 365 password)
- **Report suspicious activity** to IT (unexpected login prompts, strange errors)
- **Keep your laptop updated** (Windows Updates include security patches)

### ❌ DON'T:

- **Don't share your database credentials** - Each person should have their own account
- **Don't write down database passwords** - Use a password manager or let browser save it
- **Don't disable Tailscale** - You won't be able to access the database if it's off
- **Don't try to access database without Tailscale** - It won't work (database is not on public internet)
- **Don't install Tailscale on personal devices** - Only company-managed laptops are allowed

---

## Frequently Asked Questions

### Can I use Tailscale on my personal laptop?

**No.** Tailscale access is only allowed on company-managed laptops for security reasons.

If you need access from a personal device, contact IT to discuss options.

---

### Do I need to disconnect Tailscale when I'm done?

**No.** Tailscale can run 24/7 - it uses very little battery and has no impact on your laptop performance.

Just close your database client when you're done working.

---

### What if I forget my database password?

**Contact IT Support** to reset your password. Do NOT try multiple guesses - this may lock your account.

---

### Can I access the database from my phone or tablet?

**No, not currently.** Database access is only available from company laptops.

If you have a business need for mobile access, contact IT to discuss options.

---

### What happens if I lose my laptop?

**Immediately contact IT Support.** They will:
1. Disable your Tailscale access remotely (database becomes inaccessible from lost laptop)
2. Disable your database account (as a precaution)
3. Remotely wipe your laptop if needed (if not encrypted)

**This is why you must report lost laptops ASAP** - your data and the company database are protected if we can act quickly.

---

### Why is the database address `100.64.0.1`?

This is a special Tailscale IP address that only works when Tailscale is connected. It's not accessible from the public internet.

**Think of it like a private phone number** - only people in the company can dial it.

---

### Does Tailscale work if the office internet is down?

**Yes!** Tailscale connects you directly to the database server. As long as the server has internet access (which it does via backup connection), you can still connect remotely.

---

### Can I use Tailscale to access other office resources?

**Not yet.** Currently, Tailscale provides access to:
- PostgreSQL database
- CCTV system

In the future, IT may add access to other resources (file servers, printers, etc.).

---

### How much data does Tailscale use?

**Very little.** Typical usage:
- **Database queries**: 1-100 KB per query (almost nothing)
- **CCTV streaming**: 1-5 GB per hour of viewing (similar to Netflix)

If you're on limited mobile data, avoid watching CCTV for extended periods.

---

## Getting Help

### IT Support Contact Information

**For Tailscale or database access issues:**

📧 **Email**: itsupport@yourcompany.com
☎️ **Phone**: +44 (0) 1234 567890
💬 **Teams**: Message the **#it-support** channel

**When contacting support, please include:**
1. Your name and laptop hostname
2. What you were trying to do
3. Exact error message (take a screenshot if possible)
4. Whether Tailscale icon is green, grey, or red

### Self-Service Resources

- **Tailscale Status**: Click icon → "Admin Console" (view your connection status)
- **Company IT Knowledge Base**: <a href="https://helpdesk.yourcompany.com" target="_blank">helpdesk.yourcompany.com</a>
- **Database Documentation**: <a href="https://docs.yourcompany.com/database" target="_blank">docs.yourcompany.com/database</a>

---

## Quick Reference Card

**Print this section and keep it at your desk:**

### Tailscale Status Icons
- 🟢 Green = Connected and ready
- 🟡 Yellow = Connecting (wait 10-20 seconds)
- ⚪ Grey = Disconnected (click icon → "Connect")
- 🔴 Red = Error (contact IT)

### Database Connection Details
- **Host**: `100.64.0.1`
- **Port**: `5432`
- **SSL**: Required
- **Username**: *(provided by IT)*
- **Password**: *(provided by IT)*

### CCTV Access
- **URL**: `https://192.168.10.100` *(check with IT for exact address)*
- **Username**: *(provided by IT)*
- **Password**: *(provided by IT)*

### IT Support
- **Email**: itsupport@yourcompany.com
- **Phone**: +44 (0) 1234 567890

---

**Document Version**: 1.0
**Last Reviewed**: 2026-02-17
**Next Review**: 2026-08-17 (6 months)

---

*This guide was created to help you securely access company resources from anywhere. If you have suggestions for improving this document, please contact IT Support.*
