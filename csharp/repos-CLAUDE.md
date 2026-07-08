# CLAUDE.md — C# Repos Root

This folder contains ~100 C# application and library repositories, each in its own
subfolder with its own git repository. They are built in Visual Studio by a self-taught
developer; assume conventions vary between repos and prefer the patterns already present
in the repo you are working in.

## Scope discipline

- Work **only** in the repo(s) the user names. Never read, build, or modify sibling
  folders unless explicitly asked.
- One application at a time is the norm. Groups of related NuGet packages may be worked
  on together when the user names them together.

## Build and test — dotnet CLI only

- All work goes through the `dotnet` CLI: `dotnet build`, `dotnet test`, `dotnet run`,
  `dotnet pack`. Never assume Visual Studio is available to the session.
- Always `dotnet build` before finishing any change; if a test project exists,
  `dotnet test` must also pass.
- Shared libraries are packaged as **NuGet packages** and consumed by the other
  applications. When changing a library, check the public API surface — breaking
  changes ripple into every consuming app. Flag any breaking change explicitly.

## Git discipline (every session)

- **Before starting work** in a repo: `git fetch`, `git status`, `git pull` on the
  current branch. If the tree is dirty or the pull conflicts, stop and report.
- **Before finishing**: commit all changes with a descriptive message and `git push`
  the current branch.
- Repos may have their origin on either the personal or the company GitHub account
  (both are accessible). At the start of work in a repo, run `git remote -v` and
  report which account it belongs to.

## Application types

- Console applications and services (no GUI apps).
- Shared class libraries published as NuGet packages — including a group handling
  authentication/authorization: Azure Key Vault credential retrieval → user sign-in
  (direct or via web browser) → database login returning Entity Framework DbContexts.
  <!-- TODO: replace with the actual package names and one-line descriptions -->

## Conventions

<!-- TODO: fill in during the first scaffolding session — naming, folder layout,
     logging approach, config approach (appsettings? Key Vault?), EF usage patterns -->

- Match the style of the surrounding code in each repo.
- Tests are xUnit, in a `<Name>.Tests` project alongside the solution.
- Findings from assessments live in `REVIEW.md` in each repo; architecture docs in
  `ARCHITECTURE.md`; orientation for AI sessions in each repo's own `CLAUDE.md`.

## Commands

- `/assess <folder> [folder ...]` — the deep assessment workflow (git sync, baseline,
  security/reliability/efficiency review, tests, docs and Mermaid diagrams, git finish).
  Defined in `.claude/commands/assess.md`.
