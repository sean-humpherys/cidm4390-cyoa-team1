# Merchant encounter acceptance results

Verified on 2026-11-15 against the working tree, including Tickets 01 and 02. No production defects were found. Ticket 03 changes extend the existing two journey tests to English, Spanish, and French, include leaving before the armor claim, and assert direction-menu return after each claim. No new production feature or testing seam was introduced.

## PRD review

| Requirement | Evidence | Result |
| --- | --- | --- |
| AC1: south encounter, complete menu, return, same upgraded player north | Both existing journey tests now run in all three languages; full console demonstrations below | Pass |
| AC2: new-player defaults, one gift, leaving, no swapping or stacking | Constructor/CharacterCreation tests, encounter tests for both cross-upgrade attempts, leaving and repeat visits | Pass |
| AC3: armor grants 2; rolled 7/2/1 applies 5/0/0; no healing; normal misses, dodges, defeat | Public Combat tests with predetermined rolls and exact health/message assertions; controlled demonstration below | Pass |
| AC4: current weapon maximum +4, type/art retained, every occupation including spell, minimum 1 unchanged | EveryOccupation and controlled weapon combat tests; dagger demonstration below | Pass |
| AC5: localized introduction, choices, effects, confirmation, leave, validation, already claimed; merchant EOF leaves | Actual dictionaries and encounter tests, six console demonstrations; EOF remains tested only at Merchant boundary | Pass |
| Scope | Reviewed Player, Merchant, Adventure, Combat and dictionaries | No currency, inventory framework, persistence, new enemies, or unrelated feature changes |

## Automated verification

Commands run from the repository root:

```sh
dotnet build CYOA.Tests --no-restore -p:UseSharedCompilation=false --nologo
dotnet test CYOA.Tests --no-restore --nologo -p:UseSharedCompilation=false
```

Build succeeds. One pre-existing MSTEST0037 warning remains in `CYOA.Tests/LanguageTest.cs:15`.

The standard test command builds successfully but aborts before running tests: `System.Net.Sockets.SocketException (13): Permission denied` when VSTest binds its local communication socket. A successful standard MSTest run is **pending outside this sandbox**; this report does not claim one.

As a fallback, a temporary .NET console runner directly invokes the compiled MSTest methods and DataRow cases sequentially, executes assertions, reports exceptions, and restores console streams through the tests' existing `finally` blocks. It supports TestInitialize/TestCleanup and awaits Task results. The current suite has no ignored, dynamic-data, expected-exception, or class/assembly lifecycle cases requiring additional runner semantics. It does not verify VSTest discovery, adapter behavior, or the suite's normal parallel execution. The runner and copied message dictionaries are outside the repository under `/private/tmp/merchant-test-runner/`.

```sh
dotnet build /private/tmp/merchant-test-runner/runner.csproj --no-restore -p:UseSharedCompilation=false --nologo
cp -R MessageFiles /private/tmp/merchant-test-runner/bin/Debug/net10.0/
dotnet /private/tmp/merchant-test-runner/bin/Debug/net10.0/runner.dll MerchantTests
dotnet /private/tmp/merchant-test-runner/bin/Debug/net10.0/runner.dll
```

Focused merchant cases: **33 passed, 0 failed**. Full suite: **110 passed, 0 failed**. These counts include DataRow cases. The temporary runner references the current compiled test/application assemblies, not a reimplementation. Its temporary files are session evidence, not a permanent test command for other machines. Use the standard command above on an unrestricted host.

## Manual acceptance checklist and actual results

Completed six fresh game executions with `dotnet run --no-build`, one for each language/upgrade pair. Input was supplied through stdin; the resulting transcripts were inspected. These are scripted console demonstrations, not physical keyboard sessions. English character names/occupation input are accepted in each selected language by the existing CharacterCreation parser.

For each execution:

- [x] Choose E, S, or F, create a Human Thief, and finish character creation.
- [x] Travel `s`, choose `3`, and verify leaving returns to directions.
- [x] Travel `s` again, enter `x` and a blank line, and verify two localized validation messages and a repeated menu.
- [x] Choose `1` for armor or `2` for weapon. Verify the localized confirmation and return to directions.
- [x] Travel `s` again. Verify the already-received message and immediate return without another upgrade menu.
- [x] Travel `n`, choose `a`, and verify localized dragon combat and the original dagger art. Supply `r` to retreat if combat continues.
- [x] Verify four direction prompts, readable merchant text, fixed option numbers, both effect explanations, and no unresolved message placeholders.

Reproduce each game with the following input (replace `e` with `s` or `f`, and upgrade `1` with `2` for weapon):

```sh
printf 'e\n1\nHuman\nHero\nThief\nroll\nroll\nroll\n2\ns\n3\ns\nx\n\n1\ns\nn\na\nr\n' | dotnet run --no-build
```

| Language | Armor game | Weapon game | Dialogue review |
| --- | --- | --- | --- |
| English | Pass; entered combat; applied incoming damage shown | Pass; entered combat; attack missed | All merchant message IDs 59–65 readable and complete |
| Spanish | Pass; entered combat; defended incoming attack | Pass; entered combat; incoming damage shown | `mercader`, `armadura mágica`, `arma mágica`; complete menu and confirmations |
| French | Pass; entered combat; incoming damage shown | Pass; entered combat; weapon caused 2 damage | `marchand`, `armure magique`, `arme magique`; complete menu and confirmations |

Full-game combat uses ordinary random dice, so those runs alone cannot establish the exact reduction or upgraded maximum. A separate temporary console demonstration used the existing public Merchant and Combat APIs with the approved controlled-roll function. It changed no production code and added no testing seam. All three languages produced these results:

| Upgrade | Controlled demonstration | Observed result |
| --- | --- | --- |
| Armor | Claim 1; strength 20 dragon hits; player fails defense; enemy damage roll 7 | Protection 2; dagger maximum still 6; displayed applied damage 5; player health 20 → 15 |
| Weapon | Claim 2 on dagger; player hits; dragon fails defense; damage roll 10 | Protection 0; dagger maximum 10; damage roll requested range 1–10; displayed damage 10; dragon health 50 → 40 |

Temporary evidence: `/private/tmp/merchant03-manual/{English,Spanish,French}-{1,2}.txt`, `/private/tmp/merchant03-combat-demo.txt`, `/private/tmp/merchant03-focused.txt`, `/private/tmp/merchant03-full.txt`, and `/private/tmp/merchant03-vstest.txt`. The controlled demo command was `dotnet run --project /private/tmp/merchant03-combat-demo/demo.csproj -p:UseSharedCompilation=false -p:NuGetAudit=false`. Those paths are local session artifacts and may be removed by the system.

## Documentation and Git readiness

Verified the following files exist and are visible as untracked, nonignored files, ready to add to a future commit:

- `AGENTS.md` and `GLOSSARY.md`.
- `docs/agents/issue-tracker.md`, `docs/agents/triage-labels.md`, and `docs/agents/domain.md` referenced by AGENTS.md.
- `.scratch/merchant-encounter/spec.md` and all three numbered issue files.
- This acceptance report.
- Installed implement/TDD/code-review skill instructions and supporting files under `.agents/skills/`, plus `skills-lock.json` (already present before this ticket).

`git check-ignore -v` for the required documentation reports no matching ignore rule. `git ls-files --others --ignored --exclude-standard -- '*.md'` reports no ignored Markdown documentation. `git status --short --ignored` shows only generated `bin/` and `obj/` trees ignored. No required documentation needed an ignore exception or force-add. The documentation remains **untracked, not staged or committed**; it must be included when a commit is later authorized. Existing unrelated installed skills were not changed.

## Remaining limitations

- Standard VSTest execution remains pending on a host that permits its communication socket. Sequential fallback results are not a substitute for verifying adapter discovery and normal parallel execution there.
- One existing test analyzer warning remains; no feature defect is known.
- No commits or pushes were performed.

## Final review

Standards review found no documented-rule violations. Its one low-priority test assertion concern was fixed: both journey tests now explicitly assert the confirmation anchor exists before checking the following direction prompt. Focused and full fallback suites were rerun afterward with unchanged passing counts.

Spec review found no production defects or scope creep. The standard-runner acceptance requirement remains partial because of the documented environment restriction; all other reviewed requirements have passing evidence above.
