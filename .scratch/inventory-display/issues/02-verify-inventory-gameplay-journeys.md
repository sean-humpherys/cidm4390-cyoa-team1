# 02: Verify Inventory across merchant, healing potion, and combat journeys

**What to build:** Inventory accurately reflects existing gameplay changes while preserving merchant, healing potion, and combat behavior.

**Blocked by:** 01: Main-menu Inventory display in English, Spanish, and French. These journeys require the working Inventory command.

**Status:** ready-for-agent

## Acceptance criteria

- [x] In all three languages, inspect before and after receiving the merchant’s magical weapon gift: a dagger’s maximum damage changes from 6 to 10, retaining its localized name without an enchanted label.
- [x] Inspect before and after receiving magical armor: protection changes from 0 to 2; unrelated values remain unchanged.
- [x] Repeated inspection and merchant visits do not reapply gifts or replenish potions.
- [x] A player with a previously consumed potion displays 0 when the same player enters another adventure session; equipment upgrades remain intact.
- [x] `i` inside merchant and dragon combat interactions retains existing invalid-input behavior, displays no Inventory, and causes no state change or enemy turn.
- [x] After inspection, north, south, and exit work normally.
- [x] Existing merchant eligibility, healing limits and consumption, armor effects, and combat behavior remain intact.
- [x] Record focused test and relevant regression results, including any execution blockers.

## TDD requirements

Use the existing Adventure console interface and its existing controlled-roll support where necessary. Add each missing journey as a failing test, make the smallest required change, then review/refactor with passing checks. Reuse existing regression coverage. Covers AC4–AC5, AC11–AC12, AC14, and multilingual integration coverage.

Console tests run nonparallel, restore console streams, and assert observable output and public state.

## Authorization

The user approved this ticket's acceptance criteria, dependencies, localization, and TDD requirements exactly as proposed. Ticket publication is authorized; implementation code and tests are not authorized in this phase.

## Comments

### Ticket 02 implementation and verification

The user's later instruction authorized Ticket 02 implementation and regression tests, superseding the earlier publication-only authorization above. Ticket 01 was preserved. No architecture refactoring, production changes, commits, or pushes were made.

Added InventoryJourneyTests through the approved Adventure console interface, with real English/Spanish/French dictionaries and the existing controlled-roll constructor. Console tests are nonparallel and restore input/output in a finally block. Literal localized inventory expectations come from the approved specification, not from rendering helpers.

Six gameplay journey methods were added and run separately in this order:

| Journey | Language/gift cases | Observed result |
| --- | --- | --- |
| Magical weapon, repeated inspection and merchant revisit | English, Spanish, French | 3 passed: dagger 6 → 10, localized name unchanged, exact four-value summaries, no duplicate gift, no potion refill |
| Magical armor, leave first, claim later, revisit | English, Spanish, French | 3 passed: leaving preserves eligibility, protection 0 → 2, weapon and potions unchanged |
| Consume potion, then inspect the same player in another adventure session | Both gifts in all three languages | 6 passed: health 15/20 → 20/20 before enemy response, one potion consumed, normal hit 7 reduced to 5 with armor, another potion attempt rejected without another enemy turn; later inspection/merchant visit retains zero potions and upgrade |
| Inventory attempted inside merchant interaction | English, Spanish, French | 3 passed: existing invalid-choice guidance, merchant menu repeats, no Inventory or player/weapon mutation, leave and main-menu inspection/exit still work |
| Inventory attempted during dragon combat | English, Spanish, French | 3 passed: existing combat guidance, no Inventory/state change/controlled combat rolls, north and retreat behavior retained |
| Inspect, claim gift, inspect, travel north and attack | Both gifts in all three languages | 6 passed: normal player miss and one enemy hit, armor reduction retained, potion unconsumed, retreat unchanged |

TDD/process qualification: each journey was written and executed before adding the next journey. All six passed on their first run against the preserved implementation; no failing phase occurred. These are regression verification tests, and strict red-before-green is not claimed. No artificial failure or unnecessary production edit was introduced. One passing test's healing-before-damage assertion was clarified using an output substring; its six cases were rerun and passed. No architecture refactor was needed.

### Commands and actual results

- Baseline: `zsh /private/tmp/healing02-run.sh` — 175 passed, 0 failed before Ticket 02 edits.
- Each journey: `zsh /private/tmp/healing02-run.sh InventoryJourneyTests <method-name>` — the corresponding 3 or 6 cases passed before the next method was added.
- Focused Ticket 02: `zsh /private/tmp/healing02-run.sh InventoryJourneyTests` — 24 passed, 0 failed.
- Preserved Ticket 01: `dotnet /private/tmp/merchant-test-runner/bin/Debug/net10.0/runner.dll InventoryDisplayTests` — 22 passed, 0 failed after the focused build.
- Full regression: `zsh /private/tmp/healing02-run.sh` — 199 passed, 0 failed, including existing merchant, healing potion, combat, movement, and localization tests.
- Standard runner recheck: `dotnet test CYOA.Tests --no-restore --nologo -p:UseSharedCompilation=false` — projects built successfully, then VSTest aborted before executing tests with `System.Net.Sockets.SocketException (13): Permission denied` while binding its communication socket. This remains an environment verification blocker; standard discovery/execution has not been established.
- `git diff --check` passed. Hash comparison against a start-of-task snapshot confirmed all root production source files, language dictionaries, Ticket 01 InventoryDisplayTests, and MessageTests were unchanged by Ticket 02.

The existing temporary fallback runner builds both projects, copies actual language dictionaries, and invokes MSTest methods and data rows sequentially with per-test initialization/cleanup. Its successful run is not a claim of successful VSTest discovery or execution. Incremental rebuilds showed only the existing LanguageTest MSTEST0037 warning, not a new warning in these tests.

Temporary evidence: `/private/tmp/inventory02-baseline.txt`, `inventory02-weapon.txt`, `inventory02-armor.txt`, `inventory02-consumed.txt`, `inventory02-consumed-review.txt`, `inventory02-merchant-scope.txt`, `inventory02-combat-scope.txt`, `inventory02-attack.txt`, `inventory02-focused.txt`, `inventory02-ticket01.txt`, `inventory02-full.txt`, and `inventory02-standard.txt` (all under `/private/tmp`). These files and the temporary runner may be removed by the system; the results and limitations are preserved here.

### Review

Standards review: 0 actionable findings. The tests follow repository console, localization, and glossary conventions; no abstraction or architecture change was warranted.

Spec review: 0 actionable findings. Coverage includes merchant gifts, consumed potion state across sessions, Inventory scope restrictions, and normal movement/combat behavior. Existing gameplay expectations and controlled-roll assumptions match the implementation. No scope creep or production defect was found.
