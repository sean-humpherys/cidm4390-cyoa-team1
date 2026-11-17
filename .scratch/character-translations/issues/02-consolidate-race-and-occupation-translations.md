# 02: Consolidate race and occupation translations and finish dependency cleanup

**What to build:** Messages owns all character-term mappings. CharacterCreation and Combat share its race/occupation translations, and Combat no longer constructs CharacterCreation or a translation-only Die. All existing localized presentation and gameplay remain unchanged.

**Blocked by:** 01: Consolidate weapon translations across character creation, Inventory, and combat. Ticket 01 establishes shared ownership in Messages and migrates weapon callers before final dependency cleanup.

**Status:** ready-for-agent

## Acceptance criteria

- [x] Preserve every race and occupation translation exactly in English, Spanish, and French, including accents and capitalization, as recorded in the approved specification.
- [x] Preserve enum-string fallbacks for undefined race/occupation values and existing default behavior for unsupported language values. Do not introduce new validation or normalization.
- [x] Translate using the current language of the same Messages instance, including after language changes, without requiring numbered-message dictionary entries or file loading.
- [x] Retain CharacterCreation's existing public race and occupation translators as forwarding methods with no independent mappings.
- [x] Migrate CharacterCreation's race/occupation display and Combat's introductions and race-specific dragon taunts to the shared implementation. Preserve their existing localized wording and gameplay in all three languages.
- [x] Remove Adventure's unused private race and occupation translators. Confirm Messages contains the sole implementation of each character-term mapping and Ticket 01's shared weapon translation remains intact.
- [x] Remove Combat's translation-only construction of CharacterCreation and its associated Die. Preserve normal gameplay randomness, controlled-roll support, attack/defense/damage rules, taunt selection, and defeat behavior.
- [x] Translation emits no output, consumes no randomness, and changes neither language/dictionary nor player/equipment state. Preserve dictionary contents, message IDs, formatting, character creation rules, Inventory behavior, merchant gifts, healing potions, and movement.
- [x] Run focused shared-translation, compatibility, and caller tests and full regression coverage, including Messages, CharacterCreation, Adventure, Combat, merchant, healing potion, and both Inventory suites. Record actual commands, results, and runner blockers without equating fallback execution with standard VSTest execution.

## Red-green-refactor TDD requirements

- Begin only after Ticket 01 is complete. Recheck its passing tests and current working tree; preserve all existing implementation and test changes.
- Use the approved Messages public translation interface for race/occupation tables, enum fallbacks, unsupported-language defaults, current-language changes, dictionary independence, and absence of side effects. Retain the approved CharacterCreation compatibility and existing gameplay test interfaces; introduce no private-helper tests or additional production testing interface.
- Add one failing shared-behavior test, observe its failure or missing-interface build failure, implement the minimum to pass, then review/refactor with passing checks before the next behavior. Migrate callers in small vertical slices, keeping the suite green before removing old mappings and translation-only dependencies.
- Use literal expected translations from the approved specification, not values obtained from another translator. Existing characterization tests may pass immediately; report that accurately without manufacturing a red phase.
- Preserve output checks for combat occupation/weapon introductions and race-based taunts with deterministic existing controlled rolls. Keep console tests nonparallel, finite, and responsible for restoring streams in a finally block.
- Review architectural ownership and dependency removal directly rather than asserting implementation text or private calls. Do not widen the refactor into Dragon statistics/claw-name translation, console abstractions, or randomness redesign.
- Run focused tests after each slice and builds regularly. Attempt the standard runner, use the available fallback if needed, and report the actual full-suite result and environment limitations separately.

## Scope and approval

The user approved the specification, compatibility delegates, testing interfaces, both ticket descriptions, and dependency order Ticket 01 → Ticket 02. This publication supersedes the specification's earlier pending-approval wording. Only ticket creation is currently authorized; do not implement code, write tests, commit, or push until separately instructed.

No gameplay changes, new translations, localization framework, dictionary changes, Player initialization cleanup, healing potion extraction, or unrelated architecture work are included.

## Comments

### Ticket 02 implementation and verification

The user's later instruction authorized this ticket's implementation and tests, superseding the earlier publication-only restriction. Ticket 01 and both Inventory tickets were preserved. No commits or pushes were made.

Messages now owns the race and occupation mappings as well as the already completed weapon mapping. CharacterCreation's public race/occupation methods remain thin compatibility delegates, and its statistics display uses Messages directly. Combat uses Messages for occupation introductions and race-based dragon taunts; its translation-only CharacterCreation and Die construction were removed. Adventure's unused race/occupation translators were removed. Gameplay randomness and controlled-roll support remain unchanged.

Start-of-task source comparison confirmed that the only existing source files changed by this ticket were Messages, CharacterCreation, Adventure, and Combat. All dictionaries and pre-existing test files, including WeaponTranslationTests and both Inventory test classes, were unchanged. No player, equipment, merchant, healing potion, movement, or combat rules were modified.

### Red-green-refactor evidence

Each new failing mapping case was added individually, executed, minimally implemented, and rerun green before the next mapping case. The sequence contained 15 observed red-green cycles:

| Group | Cases added in order | Red observation | Green progression |
| --- | --- | --- | --- |
| Race 01–04 | Spanish Human → Humano; Elf → Elfo; Dwarf → Enano; Halfling → Mediano | Missing TranslateRace method first (CS1061), then exactly one failing new translation row per cycle | 1, 2, 3, 4 focused cases passed |
| Race 05–08 | French Human → Humain; Elf → Elfe; Dwarf → Nain; Halfling → Halfelin | Exactly one failing new translation row per cycle | 5, 6, 7, 8 focused cases passed |
| Occupation 01–04 | Spanish Fighter → Guerrero; Magician → Mago; Thief → Ladrón; Archer → Arquero | Missing TranslateOccupation method first (CS1061), then exactly one failing new translation row per cycle | 37, 38, 39, 40 focused cases passed |
| Occupation 05–07 | French Fighter → Guerrier; Magician → Magicien; Thief → Voleur | Exactly one failing new translation row per cycle | 41, 42, 43 focused cases passed |

English names and French Archer already satisfy the enum-name fallback and passed when added as characterization cases. Undefined enums, unsupported languages, live-language changes, dictionary independence, side-effect checks, and existing caller output also passed immediately. No artificial failures were introduced or claimed for these regression checks.

Race statistics and controlled race-based dragon-defense taunts were characterized before caller migration: 36 focused cases passed before and after the race move. Occupation statistics and combat introductions were characterized before the final move: 72 focused cases passed before and after. Removing duplicate mappings and unnecessary translation-only construction was performed with these checks green; no additional architecture refactor was needed.

### Commands and actual results

- Baseline: `zsh /private/tmp/healing02-run.sh` — 243 passed, 0 failed.
- Each mapping cycle: `zsh /private/tmp/healing02-run.sh CharacterTranslationTests` — observed the expected missing-method build failure or single newly failing row, then a passing run before adding the next case.
- Final focused suite: `zsh /private/tmp/healing02-run.sh CharacterTranslationTests` — 81 passed, 0 failed. This includes 12 exact race/compatibility cases, 12 exact occupation/compatibility cases, 12 race-statistics cases, 12 occupation-statistics cases, 12 controlled race-taunt cases, 12 controlled combat-introduction cases, 4 undefined-enum cases across language choices, 4 unsupported-language cases, and 1 live-language/dictionary/no-output case.
- Full regression: `zsh /private/tmp/healing02-run.sh` — 324 passed, 0 failed, including every pre-existing weapon, Messages, character creation, Adventure, Combat, merchant, healing potion, and Inventory test case.
- Standard runner: `dotnet test CYOA.Tests --no-restore --nologo -p:UseSharedCompilation=false` — projects built successfully, then VSTest aborted before executing tests with `System.Net.Sockets.SocketException (13): Permission denied` while binding its communication socket. Standard discovery/execution remains an environment verification blocker.
- Builds succeeded; recompilation reported only the existing LanguageTest MSTEST0037 warning. `git diff --check` passed.

The existing temporary fallback runner builds both projects, copies the real dictionaries, and invokes MSTest methods/data rows sequentially with per-test initialization/cleanup. Its passing cases do not establish successful standard VSTest discovery or execution. No new production testing interface was added. Console tests remain nonparallel and restore redirected output in a finally block.

Temporary evidence is under `/private/tmp`: `character02-baseline.txt`; `character02-race-01-red.txt` through `character02-race-08-red.txt` and corresponding `-green.txt` files; `character02-occupation-01-red.txt` through `character02-occupation-07-red.txt` and corresponding `-green.txt` files; `character02-race-english.txt`, `character02-race-statistics-before.txt`, `character02-race-taunt-before.txt`, `character02-race-after.txt`, `character02-occupation-defaults.txt`, `character02-occupation-statistics-before.txt`, `character02-occupation-combat-before.txt`, `character02-callers-after.txt`, `character02-enum-fallbacks.txt`, `character02-language-fallbacks.txt`, `character02-focused.txt`, `character02-standard.txt`, and `character02-full.txt`. `character02-before.json` and `character02-task.diff` capture the start-of-task source and task-only changes. Temporary artifacts may disappear; these results and limitations are preserved here.

### Code review findings

Standards: 0 actionable findings. Centralized mappings, compatibility delegation, deterministic console tests, and scope follow repository conventions; no additional abstraction was warranted.

Spec: 0 actionable findings. All exact translations, fallbacks, language state behavior, compatibility methods, callers, and dependency removal match the approved scope. French Archer correctly uses its unchanged enum-name fallback. The existing weapon implementation, dictionaries, Inventory behavior, and gameplay rules were preserved.

### Final standard-runner verification reported by the user

Before requesting the combined commit, the user confirmed that all 324 tests passed using standard `dotnet test` in the regular VS Code terminal. This supplies standard-runner verification outside the agent sandbox. The earlier socket-permission errors remain historical records of the sandbox runs, not an outstanding test failure in the user's regular terminal. No production or test code changed after the reported successful run; commit preparation only refreshed relevant documentation.
