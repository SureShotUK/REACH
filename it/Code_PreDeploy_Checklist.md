<img src="../Portland Long.png" alt="Portland Long" style="width:40%; height:auto;" align="right">

# Code Pre-Deployment Checklist

*Run before any application goes to production or becomes network-reachable (Nginx deploy, Tailscale serve, scheduled task). Written for the Portland Fuel lead-generation app (.NET 10) and future deployments; applies to any project. A "No" on any starred (★) item blocks the deployment.*

---

## 1. Secrets and Configuration

- [ ] ★ No secrets in source or git history: API keys (Companies House etc.), connection strings, passwords — all in environment variables, user-secrets, or a config file excluded by `.gitignore`. Grep the repo AND `git log -p` for known key fragments before deploying
- [ ] ★ Separate configuration per environment (dev/production) — production endpoints, log levels, and credentials never hard-coded
- [ ] Bash special characters in passwords handled (single quotes / env files — `!` and `$` break double-quoted docker/systemd strings; known past issue)
- [ ] Backup of the previous working deployment taken; rollback steps written down before, not after

## 2. Authentication and Authorisation

- [ ] ★ Every network-reachable page/endpoint requires authentication unless deliberately public — test by hitting endpoints logged out, not by inspecting code
- [ ] ★ M365/Entra ID auth (planned for the lead-gen app): redirect URIs restricted to the real hostnames; client secret expiry noted in the diary; token validation includes audience and issuer
- [ ] Authorisation checked server-side per request, not only via hidden UI elements
- [ ] Trust boundary stated: LAN-only, Tailnet, or public — and the exposure mechanism (Nginx vhost, Tailscale serve + ACL) matches it. For Tailscale: serve rule AND ACL entry both present

## 3. Input Handling and Data

- [ ] ★ All external input validated at the boundary (query params, form posts, uploaded files, third-party API responses — Companies House data is external input too)
- [ ] ★ SQL parameterized everywhere — no string-built queries
- [ ] Output encoding for anything rendered to HTML (XSS); file paths never built from user input
- [ ] Personal data handling reviewed: only data needed is stored; retention thought through (UK GDPR)

## 4. Error Handling, Logging, Resilience

- [ ] ★ Production errors show a generic page — no stack traces, connection strings, or internal paths to the client
- [ ] Errors logged server-side with enough context to diagnose; logs don't capture secrets or full personal records
- [ ] External API failures (rate limits, timeouts, schema changes) handled gracefully — the app degrades, it doesn't crash
- [ ] Restart behaviour verified: app survives a reboot (systemd unit/service configured, not a hand-started process)

## 5. Platform and Transport

- [ ] ★ HTTPS end-to-end for anything beyond LAN; certificates valid and auto-renewing (note expiry if manual)
- [ ] Nginx: security headers set (at minimum `X-Content-Type-Options`, `X-Frame-Options`/CSP, HSTS if public); server tokens off; only intended ports exposed
- [ ] Dependencies current: `dotnet list package --vulnerable` (or equivalent) clean; .NET runtime version supported
- [ ] Firewall posture unchanged or consciously changed: no new inbound ports without a written reason

## 6. Verification Before Sign-Off

- [ ] ★ The deployed app exercised end-to-end in its production location (not just on the dev machine): main workflow, auth flow, one failure path
- [ ] Reviewed by the `csharp-reviewer` agent with criticals resolved
- [ ] Smoke-test steps and rollback steps written into the project notes for next time

---

*Related: `it/CLAUDE.md` (development standards), `it/ZeroTrust/` (remote-access architecture), the `it-security-researcher` agent for anything needing current guidance.*
