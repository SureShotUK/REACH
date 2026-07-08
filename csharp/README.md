<img src="../Portland Long.png" alt="Portland Long" style="width:40%; height:auto;" align="right">

# C# Repos Scaffolding

Claude Code workflow files for the Windows `repos` folder (~100 C# applications and
NuGet package libraries). Drafted here in terminai so they sync to every machine via
GitHub; they take effect once copied into the repos folder on the Windows PC.

## Installation (on the Windows PC)

Copy into the **repos root** (the parent folder containing all the application folders):

| File here | Destination in repos folder |
|---|---|
| `repos-CLAUDE.md` | `CLAUDE.md` |
| `commands/assess.md` | `.claude/commands/assess.md` |

Then complete the two `TODO` sections in the new `CLAUDE.md` (package names, conventions)
— or let the first Claude Code session fill them in.

## Usage

1. Launch Claude Code **at the repos root** (so multi-package sessions can see all
   named folders), signed in to the **Pro account** for assessment work.
2. Run `/assess <FolderName>` for a single application, or
   `/assess <PackageA> <PackageB> <PackageC>` for a related package group.

The command works through: git fetch/pull → build + vulnerability baseline →
security/reliability/efficiency review (`REVIEW.md`) → xUnit exemplar tests →
`CLAUDE.md`/`README.md`/`ARCHITECTURE.md` with Mermaid flow + sequence diagrams →
approved fixes → commit and push.

## First engagement

The authentication/authorization NuGet package group, assessed together as one system:

1. Key Vault credential retrieval package
2. User authentication package (direct sign-in or via web browser)
3. Database login / DbContext provider package

Expected extra outputs for this group: an end-to-end Mermaid sequence diagram of the
credential → sign-in → DbContext flow, and cross-package findings (API seams, version
alignment, credential handling between packages).

## Notes

- Mermaid diagrams render on GitHub and in VS Code natively; Visual Studio needs a
  Mermaid extension to preview them.
- `/assess` never modifies product code without approval — Phase 2 ends with a ranked
  findings list and a decision point. Tests and docs are additive and proceed
  automatically.
