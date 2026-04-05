# Frequently Asked Questions: Database Access via Tailscale

**For**: All staff with remote database access
**Last Updated**: 2026-02-17

---

## General Questions

### What is Tailscale and why are we using it?

**Tailscale** is a modern, secure networking tool that provides Zero Trust access to company resources. Think of it as a "private internet" for our company.

**Why we chose Tailscale over traditional VPN:**
- ✅ **More secure** - Zero Trust architecture (verify every access, not just login)
- ✅ **Easier to use** - Connects automatically, no VPN buttons or manual tunnels
- ✅ **Faster** - Peer-to-peer connections (direct laptop → server, not through cloud)
- ✅ **Better integration** - Uses your existing Microsoft 365 login with MFA
- ✅ **Lower cost** - 60% cheaper than traditional VPN solutions

---

### Is Tailscale safe? How secure is it?

**Very secure.** Tailscale uses:
- **WireGuard encryption** - Military-grade, audited by security experts
- **Azure AD authentication** - Your Microsoft 365 login + MFA required
- **Device verification** - Only company-managed, Intune-enrolled laptops allowed
- **End-to-end encryption** - All traffic encrypted from your laptop to server
- **No public exposure** - Database never exposed to internet
- **SOC 2 Type II certified** - Independently audited for security compliance
- **GDPR compliant** - Meets EU data protection requirements

**Security audits:**
- Tailscale regularly audited by independent security firms
- WireGuard protocol audited by cryptography experts
- Used by thousands of companies worldwide (including Fortune 500)

**In plain English:** Tailscale is more secure than traditional VPNs and significantly more secure than opening the database to the internet.

---

### Can I use Tailscale on my personal laptop or phone?

**No.** Tailscale access is only allowed on:
- Company-owned laptops
- Windows 11 devices
- Managed by IT (enrolled in Intune)

**Why this restriction?**
- **Security**: Personal devices may not have required security controls (antivirus, encryption, patches)
- **Compliance**: GDPR requires we control access to sensitive data
- **Support**: IT can't troubleshoot or remote-wipe personal devices

**If you need access from a personal device:**
- Contact IT to discuss business justification
- IT may issue you a company laptop
- Cloud-based alternatives may be available for limited use cases

---

### What's the difference between Tailscale and our old VPN?

| Feature | Traditional VPN | Tailscale (Zero Trust) |
|---------|----------------|------------------------|
| **Connection** | Manual (click "Connect") | Automatic (always on) |
| **Speed** | Slower (routed through cloud) | Faster (direct peer-to-peer) |
| **Access** | Entire network | Specific resources only |
| **Security** | Login once = trusted | Verify every access |
| **Disconnects** | Frequent | Rare |
| **User experience** | Clunky | Seamless |

**Bottom line:** Tailscale is faster, easier, and more secure than traditional VPN.

---

### Does Tailscale slow down my laptop or drain battery?

**No.** Tailscale is extremely lightweight:
- **CPU usage**: < 0.1% (unnoticeable)
- **RAM usage**: ~20-40 MB (negligible)
- **Battery impact**: Minimal (adds ~5-10 minutes to battery drain per day)
- **Internet bandwidth**: Only when actively using database (not idle)

**You can leave Tailscale running 24/7** without any performance impact.

---

### Can I access the database without Tailscale?

**No.** The database is only accessible via Tailscale for security reasons.

**Why?**
- Database is NOT exposed to the public internet (no inbound firewall ports)
- Only Tailscale-connected devices can reach the database
- This prevents hackers from attempting to access the database

**If you can't connect:**
1. Verify Tailscale is running (green icon)
2. Contact IT if Tailscale won't install or connect

---

## Technical Questions

### What is the database IP address with Tailscale?

**Database address**: `100.64.0.1`
**Port**: `5432`
**SSL**: Required

**Why `100.64.0.1` instead of a normal IP?**
- This is a special Tailscale "shared IP" for the database server
- It only works when Tailscale is connected
- It's the same for all users (simplifies configuration)
- It's NOT accessible from the public internet

**Alternative (CGNAT IP):**
- Your laptop will also see the database as `100.x.x.x` (specific IP varies)
- Both addresses work, but `100.64.0.1` is recommended for simplicity

---

### Which database tools are supported?

**Officially supported:**
- ✅ **DBeaver** (recommended for most users)
- ✅ **pgAdmin** (recommended for advanced users)
- ✅ **psql** (command-line tool)
- ✅ **Any PostgreSQL-compatible client** (DataGrip, Navicat, etc.)
- ✅ **Custom applications** (Python, Java, .NET, Node.js, etc.)

**Configuration:** Same connection settings regardless of tool:
- Host: `100.64.0.1`
- Port: `5432`
- SSL: Required

**Need help configuring a specific tool?** Contact IT Support.

---

### Can I use command-line psql?

**Yes!** If you have `psql` installed:

```bash
psql -h 100.64.0.1 -p 5432 -U your_username -d your_database_name
```

**SSL connection:**
```bash
psql "postgresql://your_username@100.64.0.1:5432/your_database_name?sslmode=require"
```

---

### What if I need to connect from a script or application?

**Update your connection string to:**

**PostgreSQL connection string format:**
```
postgresql://username:password@100.64.0.1:5432/database_name?sslmode=require
```

**JDBC (Java):**
```
jdbc:postgresql://100.64.0.1:5432/database_name?ssl=true&sslmode=require
```

**Python (psycopg2):**
```python
import psycopg2

conn = psycopg2.connect(
    host="100.64.0.1",
    port=5432,
    database="database_name",
    user="your_username",
    password="your_password",
    sslmode="require"
)
```

**Node.js (pg):**
```javascript
const { Client } = require('pg');

const client = new Client({
  host: '100.64.0.1',
  port: 5432,
  database: 'database_name',
  user: 'your_username',
  password: 'your_password',
  ssl: { rejectUnauthorized: false }
});
```

**Need help with your specific language/framework?** Contact IT Support.

---

### How fast is the connection? Will queries be slower?

**Performance depends on your internet connection:**

| Internet Speed | Database Performance |
|----------------|---------------------|
| **10+ Mbps** | ✅ Excellent (same as office) |
| **5-10 Mbps** | ✅ Good (slight delay on large reports) |
| **2-5 Mbps** | ⚠️ Acceptable (noticeable delay) |
| **< 2 Mbps** | ❌ Slow (frustrating experience) |

**Typical latency (delay):**
- **UK home/office**: 5-15 ms (excellent)
- **UK public WiFi**: 10-30 ms (good)
- **Europe**: 15-40 ms (good)
- **Canada**: 30-80 ms (acceptable)

**For comparison:**
- **In-office network**: 1-2 ms
- **Traditional VPN**: 20-60 ms (Tailscale is faster!)

**Tips for best performance:**
- Use wired Ethernet if available (faster than WiFi)
- Sit close to WiFi router
- Use 5 GHz WiFi instead of 2.4 GHz (if available)
- Avoid public WiFi during peak hours
- Use mobile hotspot for consistent performance

---

## Privacy & Security

### Can Tailscale see my database queries?

**No.** Tailscale provides the encrypted connection, but **cannot see** the content of your database traffic.

**What Tailscale can see:**
- ✅ When you connect/disconnect
- ✅ Your laptop's Tailscale IP address
- ✅ That you connected to the database server

**What Tailscale CANNOT see:**
- ❌ Your database queries (SQL statements)
- ❌ The data you're viewing
- ❌ Your database password
- ❌ Any other activity on your laptop

**Analogy:** Tailscale is like a postal service that delivers encrypted letters. They know you sent a letter to the database, but they can't read what's inside.

---

### Can IT see what I'm doing in the database?

**It depends on what's being logged:**

**IT can see:**
- ✅ When you connect to the database
- ✅ Your username and IP address
- ✅ Which database you accessed
- ✅ Connection duration

**IT can see (if query logging is enabled):**
- ✅ Specific SQL queries you run (for audit/compliance purposes)
- ✅ Tables and data you accessed

**IT CANNOT see:**
- ❌ Your personal files on your laptop
- ❌ Your web browsing (unless you're on company network)
- ❌ Your emails (unless using company email server)

**Why does IT log database queries?**
- **Compliance**: GDPR and data protection regulations require audit trails
- **Security**: Detect unauthorized access or data exfiltration
- **Troubleshooting**: Diagnose slow queries or database issues
- **Accountability**: Understand who accessed what data and when

**This is normal and required** for any company handling sensitive data.

---

### What data does Tailscale collect about me?

**Tailscale collects minimal data:**

**Account information:**
- Your Microsoft 365 email address (used for login)
- Your laptop's Tailscale IP address (`100.x.x.x`)
- Device information (hostname, OS version)

**Connection logs:**
- When you connect/disconnect
- Which company resources you access
- Connection duration

**Tailscale does NOT collect:**
- ❌ Database queries or data
- ❌ Files on your laptop
- ❌ Web browsing history
- ❌ Emails or documents
- ❌ Passwords

**Data retention:**
- Connection logs: 90 days
- Account information: Duration of employment

**Data location:**
- Stored in EU and US data centers (GDPR-compliant via Standard Contractual Clauses)

**For full details:** <a href="https://tailscale.com/privacy-policy" target="_blank">Tailscale Privacy Policy</a>

---

### Is my connection encrypted on public WiFi?

**Yes, always.** Tailscale uses end-to-end encryption:

**Encryption:**
- **WireGuard protocol** - Military-grade encryption (same strength as banking apps)
- **End-to-end** - Encrypted from your laptop to the database server
- **Safe on ANY WiFi** - Coffee shops, hotels, airports, etc.

**Even if someone intercepts your WiFi traffic:**
- They see only encrypted gibberish
- They cannot see your database queries
- They cannot steal your credentials
- They cannot access the database

**That said, best practices:**
- ✅ Use Tailscale on public WiFi (fully protected)
- ✅ Prefer mobile hotspot for sensitive work (extra security layer)
- ❌ Don't access highly confidential data on completely untrusted networks (e.g., random airport WiFi)

---

## Troubleshooting

### Tailscale won't connect (grey icon)

**Try these steps in order:**

**1. Basic restart:**
- Right-click Tailscale icon → "Quit Tailscale"
- Start menu → Search "Tailscale" → Open
- Wait 30 seconds for connection

**2. Check internet connection:**
- Open browser → Go to google.com (does it load?)
- If no internet, Tailscale can't connect

**3. Re-authenticate:**
- Click Tailscale icon → "Log in"
- Sign in with Microsoft (email + MFA)

**4. Restart Windows:**
- Restart your laptop
- Tailscale should auto-connect after reboot

**Still not working?**
- Contact IT Support (may be a server-side issue)
- Provide: laptop hostname, Tailscale error message (if any)

---

### "Access denied" or "Authentication failed" (database)

**This means your database credentials are incorrect or your account is disabled.**

**Common causes:**

**1. Wrong password:**
- Re-enter password carefully (check caps lock)
- Try copying from password manager (avoid typos)
- Do NOT try multiple guesses (may lock your account)

**2. Account disabled:**
- Contact IT to verify your account is active
- May have been disabled due to inactivity or policy change

**3. Wrong username:**
- Check username with IT (may be different from email)

**4. SSL not enabled:**
- Verify SSL mode is set to "Require" in connection settings

**Solution:**
- Contact IT Support for password reset (do not guess!)
- Provide: your username, exact error message

---

### Connection drops frequently

**If Tailscale disconnects often:**

**Causes:**
- **Laptop sleep** - Windows puts Tailscale to sleep, reconnects when waking
- **WiFi switching** - Moving between WiFi networks causes brief disconnect
- **Poor internet** - Unreliable connection drops Tailscale

**Solutions:**

**1. Keep laptop awake during work:**
- Settings → System → Power → "When plugged in, PC goes to sleep after" → **Never**

**2. Disable WiFi power saving:**
- Device Manager → Network adapters → Right-click WiFi → Properties → Power Management → **Uncheck "Allow computer to turn off this device"**

**3. Use Ethernet** (wired connection is more stable than WiFi)

**4. Upgrade internet** (if home WiFi frequently drops)

**Note:** Brief 5-10 second disconnects when waking from sleep are normal and expected.

---

### Database connection was working, now it's not

**If it worked yesterday but doesn't work today:**

**Check:**
1. **Is Tailscale connected?** (Green icon?)
2. **Did your password change?** (Contact IT if unsure)
3. **Is database server running?** (IT will notify if planned maintenance)

**Try:**
1. Close database client completely (DBeaver, pgAdmin)
2. Disconnect Tailscale (right-click icon → "Disconnect")
3. Reconnect Tailscale (click icon → "Connect")
4. Reopen database client
5. Try connecting again

**Still broken?**
- Contact IT Support
- Provide: when it last worked, exact error message, screenshot

---

### Error: "SSL connection required"

**This means you're not connecting with SSL encryption.**

**Solution (DBeaver):**
1. Right-click connection → "Edit Connection"
2. Go to "Driver properties" tab
3. Find **"ssl"** property → Set to **"true"**
4. Find **"sslmode"** property → Set to **"require"**
5. Click "Test Connection" → "Finish"

**Solution (pgAdmin):**
1. Right-click server → "Properties"
2. Go to "SSL" tab
3. **SSL mode** → Select **"Require"**
4. Click "Save"

---

## Usage Questions

### Can I save my database password in DBeaver/pgAdmin?

**Yes, but consider security:**

**Saving password is:**
- ✅ **Convenient** - Don't need to type password every time
- ⚠️ **Less secure** - Anyone with physical access to your laptop can open database

**Best practice:**
- **If laptop is always with you** - OK to save password
- **If laptop is left unattended** - Do NOT save password
- **Always lock laptop** when stepping away (Windows Key + L)
- **Use strong laptop password** (protects saved credentials)

**Most secure option:**
- Use password manager (e.g., 1Password, LastPass)
- Copy password from manager when needed
- Never write passwords on paper or sticky notes

---

### How do I access different databases?

**If you have access to multiple databases:**

**Option 1: Multiple connections (recommended)**
- Create separate connection for each database in DBeaver/pgAdmin
- Name them clearly (e.g., "Production DB", "Reporting DB", "Test DB")
- All use same host (`100.64.0.1`) but different database names

**Option 2: Switch databases**
- Connect to default database (usually `postgres`)
- Use SQL: `\c other_database_name` (psql) or switch via GUI

**Don't have access to a database you need?**
- Contact IT to request access
- Provide: database name, business justification
- IT will grant minimum necessary permissions (least-privilege principle)

---

### Can I connect to the database from multiple programs at once?

**Yes!** You can have:
- DBeaver open
- pgAdmin open
- Custom application running
- psql command-line session

**All at the same time.** PostgreSQL supports multiple concurrent connections.

**Note:** Each connection counts towards your session limit (usually no limit for regular users, but admins may have restrictions).

---

### Does Tailscale work from international locations?

**Yes, Tailscale works worldwide.**

**Performance by region:**

| Region | Expected Performance |
|--------|---------------------|
| **UK** | ✅ Excellent (5-15 ms latency) |
| **Europe** | ✅ Good (15-40 ms latency) |
| **Canada** | ⚠️ Acceptable (30-80 ms latency) |
| **Asia** | ⚠️ Slower (100-200 ms latency) |
| **Australia** | ⚠️ Slower (150-250 ms latency) |

**In practice:**
- Simple queries work fine from anywhere
- Large reports may take longer from distant locations
- Interactive work is still usable (just slightly slower)

**No configuration changes needed** - Tailscale automatically finds the fastest route.

---

### What happens if I put my laptop to sleep?

**Normal behavior:**
1. You close your laptop lid (laptop sleeps)
2. Tailscale connection pauses
3. You open laptop (laptop wakes)
4. Tailscale reconnects automatically (5-15 seconds)

**Impact on database connection:**
- **Short sleep** (< 5 minutes) - Database client may stay connected
- **Long sleep** (> 30 minutes) - Database connection will drop, need to reconnect

**Best practice:**
- Close database client before putting laptop to sleep
- Reopen database client after waking laptop
- This ensures clean connection (avoids half-open connections)

---

## Account & Access

### How do I get database access?

**To request database access:**

1. **Submit IT ticket** (email: itsupport@yourcompany.com)
2. **Provide:**
   - Your name and department
   - Which database(s) you need access to
   - Business justification (why you need access)
   - Required permissions (read-only or read-write)
   - Approval from your manager (may be required)

3. **IT will:**
   - Verify your request with your manager
   - Create your database account with minimum necessary permissions
   - Provide you with username and temporary password
   - Ensure Tailscale is installed on your laptop

4. **You'll receive:**
   - Database username
   - Temporary password (change on first login)
   - Connection instructions

**Typical approval time**: 1-2 business days

---

### My database password expired, how do I reset it?

**To reset your database password:**

1. **Contact IT Support** (email or phone)
2. **Provide:** Your database username
3. **IT will:** Send you a temporary password
4. **You'll:** Connect and change password on first login

**Password requirements:**
- Minimum 12 characters
- Must include: uppercase, lowercase, number, special character
- Cannot reuse last 5 passwords
- Expires every 90 days (you'll receive reminder email)

**Do NOT:**
- Share your database password with anyone
- Reuse your Microsoft 365 password
- Write password on paper or sticky notes

---

### I'm leaving the company, what happens to my access?

**Your access will be automatically removed when you leave:**

**On your last day:**
- Microsoft 365 account disabled (can't log in to Tailscale)
- Database account disabled (can't query database)
- Tailscale connection blocked (laptop can't reach database)

**If you're offboarding someone:**
- HR notifies IT of last working day
- IT disables all access at end of day
- No action needed from you

**If you still have access after leaving:**
- This is a security issue - contact IT immediately
- Do NOT use company resources after employment ends (illegal)

---

## Platform & Compatibility

### Which operating systems does Tailscale support?

**Officially supported at our company:**
- ✅ **Windows 11** (your company laptops)

**Tailscale also supports (not used at our company):**
- macOS (10.13+)
- Linux (Ubuntu, Debian, CentOS, etc.)
- iOS (iPhone/iPad)
- Android
- ChromeOS

**If you have a different OS:**
- Contact IT to discuss compatibility
- May need different solution or additional approval

---

### Do I need admin rights to install Tailscale?

**No.** Tailscale is installed and managed by IT remotely (via Intune).

**You cannot:**
- Install Tailscale yourself (requires IT push)
- Uninstall Tailscale (managed by IT policy)
- Update Tailscale manually (auto-updates via Intune)

**If you need Tailscale:**
- Contact IT (they'll push installation to your laptop)
- Usually installed within 24 hours

---

### Can Tailscale work without internet?

**No.** Tailscale requires internet connection to:
- Authenticate with Microsoft 365
- Establish encrypted tunnel to database server
- Coordinate peer-to-peer connections

**If you're offline:**
- Tailscale shows grey icon (disconnected)
- Database is inaccessible
- Reconnects automatically when internet returns

**Minimum internet requirement:**
- **Download speed**: 2+ Mbps (10+ Mbps recommended)
- **Upload speed**: 1+ Mbps
- **Latency**: < 200 ms (lower is better)

---

## Getting Help

### I have a question not answered here

**Contact IT Support:**

**📧 Email**: itsupport@yourcompany.com
- Best for non-urgent questions
- Include screenshots if possible
- Response within 4-24 hours

**☎️ Phone**: +44 (0) 1234 567890
- For urgent issues (database down, can't access critical data)
- Available: Mon-Fri 8am-6pm GMT

**💬 Microsoft Teams**: #it-support channel
- Quick questions and real-time help
- Other users may answer before IT responds
- Good for "Has anyone else seen this?" questions

---

### How do I report a Tailscale or database problem?

**When contacting IT, please provide:**

**1. Your information:**
- Name
- Laptop hostname (Start → Settings → System → About → "Device name")
- Email address

**2. Problem description:**
- What you were trying to do
- What happened instead
- When did it start (was it ever working?)

**3. Technical details:**
- Tailscale icon color (green/grey/red/yellow)
- Exact error message (copy text or screenshot)
- Database client you're using (DBeaver, pgAdmin, etc.)

**4. What you've tried:**
- Restarted Tailscale?
- Restarted laptop?
- Tested on different WiFi?

**Example good support request:**

> Subject: Can't connect to database - "Authentication failed" error
>
> Hi IT,
>
> I'm trying to connect to the database using DBeaver but getting an "Authentication failed" error.
>
> Details:
> - Name: John Smith
> - Laptop: LAPTOP-12345
> - Email: john.smith@company.com
> - Database username: jsmith
> - Tailscale status: Green (connected)
> - Error: "FATAL: password authentication failed for user 'jsmith'"
>
> This was working yesterday. I haven't changed my password. I've tried restarting DBeaver but same error.
>
> Screenshot attached.
>
> Thanks,
> John

---

### Where can I find more technical documentation?

**Resources:**

**Company-specific:**
- **IT Knowledge Base**: <a href="https://helpdesk.yourcompany.com" target="_blank">helpdesk.yourcompany.com</a>
- **Database Documentation**: <a href="https://docs.yourcompany.com/database" target="_blank">docs.yourcompany.com/database</a>

**Tailscale official:**
- **Tailscale Docs**: <a href="https://tailscale.com/kb/" target="_blank">tailscale.com/kb/</a>
- **Tailscale FAQ**: <a href="https://tailscale.com/kb/1018/faq/" target="_blank">tailscale.com/kb/1018/faq/</a>

**PostgreSQL:**
- **PostgreSQL Docs**: <a href="https://www.postgresql.org/docs/" target="_blank">postgresql.org/docs/</a>
- **DBeaver Guide**: <a href="https://dbeaver.com/docs/wiki/" target="_blank">dbeaver.com/docs/wiki/</a>
- **pgAdmin Docs**: <a href="https://www.pgadmin.org/docs/" target="_blank">pgadmin.org/docs/</a>

---

**Still have questions?** Contact IT Support - we're here to help!

**Version**: 1.0 | **Last Updated**: 2026-02-17
