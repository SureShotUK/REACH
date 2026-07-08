---
description: Deep assessment of one C# application, or a group of related NuGet package repos, to make it as secure, reliable and efficient as possible
argument-hint: <app-or-package-folder> [related-package-folder ...]
---

# /assess — Deep Application / Package-Group Assessment

Target folder(s): $ARGUMENTS

Work on **only** the folder(s) named above. Do not read or modify any other repo in this
directory, even if it references the target. When multiple folders are given they are a
group of related NuGet packages and must be assessed **together as one system**, not
three separate reviews.

Work through the phases in order. Do not skip a phase. Announce each phase as you start it.

## Phase 0 — Git sync (before anything else)

For **each** target folder:
1. `git remote -v` — report the origin URL, stating whether the repo is on the personal or company GitHub account
2. `git fetch`
3. `git status` — if there are uncommitted changes, **stop**, report them, and wait for instructions
4. `git pull` on the current branch — if the pull conflicts, **stop** and report

Only proceed when every target repo is clean and up to date.

## Phase 1 — Baseline

For each target folder:
1. `dotnet build` the solution. Record (do not yet fix) all errors and warnings.
2. `dotnet list package --vulnerable --include-transitive` — record any vulnerable dependencies.
3. `dotnet list package --outdated` — record outdated dependencies.
4. Note the target framework(s), and for NuGet packages the current package version.
5. If the build fails, fixing the build is the first task — report the cause and proposed fix before changing anything.

For package groups additionally:
- Map which package depends on which (project/package references).
- Check version alignment of shared dependencies across the group (e.g. Azure SDK, EF Core versions).

## Phase 2 — Review (report only — change no code in this phase)

Write findings to `REVIEW.md` in each target repo (for a package group, one combined
`REVIEW.md` in the first-named repo, with per-package sections). Every finding needs a
severity (Critical / High / Medium / Low), a `file:line` reference, and a concrete
recommendation.

Assess, in priority order:
1. **Security** — secrets or connection strings in code/config, credential lifetime and caching, token handling, SQL injection, injection via config, overly broad Key Vault / DB permissions implied by the code, missing certificate validation.
2. **Reliability** — missing `using`/disposal, async/await misuse (sync-over-async, `async void`, unawaited tasks), missing error handling around network/Key Vault/DB calls, absence of retry/timeout policies, thread-safety of shared state.
3. **Correctness** — logic errors, edge cases (null/empty, cancelled tokens), incorrect exception swallowing.
4. **Efficiency** — repeated Key Vault round-trips that should be cached, N+1 database queries, unnecessary allocations in hot paths, connection/context lifetime issues.
5. **Package-group seams** (multi-package assessments only) — awkward or inconsistent public APIs between the packages, duplicated logic across packages, data passed between packages in fragile shapes, versioning strategy.

End the phase by presenting a summary of findings ranked by severity and **ask which
fixes to apply**. Do not fix product code without approval. (Phases 3–4 are additive
and proceed without asking.)

## Phase 3 — Tests

For each target:
1. If no test project exists, create `<Name>.Tests` (xUnit) alongside the solution and add it to the `.sln`.
2. Write exemplar tests for the riskiest core logic identified in Phase 2 — not blanket coverage. Aim for tests that would catch a regression in the behaviour that matters.
3. Mock external dependencies (Key Vault, database) at the seam; do not require live credentials to run the tests.
4. `dotnet test` must be green before the phase ends.

## Phase 4 — Documentation and diagrams

1. Create or update `CLAUDE.md` in each target repo: what it is, how it's structured, how to build/test/pack, gotchas. Write it for a future AI session with no prior knowledge.
2. Create or update `README.md`: purpose and usage (for packages: how a consuming app uses it, with a code sample).
3. For package groups, write `ARCHITECTURE.md` in the first-named repo containing:
   - A **Mermaid flowchart** of the packages and their dependencies (including the consuming application's position).
   - A **Mermaid sequence diagram** of the runtime flow across the group — e.g. app start → credential retrieval from Key Vault → user authentication (direct or browser) → database login → DbContext returned — showing which package owns each step.

## Phase 5 — Apply approved fixes

Apply only the fixes approved at the end of Phase 2, smallest/safest first.
After each fix: `dotnet build` and `dotnet test` must pass before moving to the next.

## Phase 6 — Git finish (always, before ending)

For **each** target repo with changes:
1. `dotnet build` and `dotnet test` one final time — both green.
2. Stage and commit with a descriptive message summarising the assessment outcome.
3. `git push` the current branch.
4. Report per repo: what was committed, what remains open in `REVIEW.md`.
