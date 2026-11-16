# 03: Verify complete potion journeys and record acceptance results

Status: ready-for-agent

**What to build:** Demonstrated potion behavior across character creation, merchant visits, both magical upgrades, and dragon combat.

**Blocked by:** 02 — Drink a healing potion as a complete combat turn, which already depends on 01.

## Acceptance criteria

- [x] Leaving, claiming either merchant gift, and repeat visits preserve potion supply and maximum health.
- [x] Northward combat retains the same player's potion state and merchant upgrade.
- [x] No merchant interaction replenishes a consumed potion; a new player starts fresh.
- [x] Existing merchant, attack, retreat, armor, weapon, and ending behavior remains correct.
- [x] Record actual manual results and any pending checks without claiming unperformed validation.

## Test requirements

- [x] Reuse existing tests; add only missing observable journey assertions.
- [x] Cover both merchant upgrades and leaving/revisiting, with complete console input.
- [x] Run the full automated suite and record commands, counts, and failures.
- [x] Clearly distinguish sandbox runner-start failures from test failures or passes; identify any fallback's limitations.
- [x] Demonstrate healing, capped healing, full-health preservation, empty supply, invalid input, dragon responses, and defeat.
- [x] Inspect English, Spanish, and French dialogue and record the results.

## TDD requirements

For any missing integration coverage or discovered defect, follow one failing test → observe failure → minimal implementation → passing test → refactor, repeating incrementally. Write one failing test at a time at the approved public boundaries. Do not duplicate existing tests merely to create work for this ticket.

## Traceability

Integration verification across PRD AC1–AC10.


## Verification results — 2026-11-16

### Outcome and scope

All gameplay acceptance criteria AC1–AC10 in the approved Category A healing potion PRD are satisfied by the reviewed implementation and available evidence. No production defects were found. Standard VSTest execution remains pending on a host that allows its local communication socket; the successful fallback run below is not a claim that standard discovery, adapter behavior, or parallel execution passed. The instructor's external assignment document was inaccessible during requirements gathering, so compliance with any additional, unprovided rubric requirements cannot be independently certified.

Only existing potion-turn tests and this Ticket 03 record were changed during this ticket. Production Adventure, Player, CharacterCreation, Combat, and Merchant files were compared with their start-of-ticket snapshots and are unchanged. No unrelated feature, refactor, commit, or push was introduced.

### Existing coverage reused and missing integration assertions added

Reused CharacterCreation/Player initialization tests, potion choice/rejection/turn-order tests, actual-language dictionary tests, controlled Combat damage/armor/miss/defense/defeat tests, the same-invocation lethal potion-response test, and existing merchant tests.

Added only two observable integration test scenarios to the existing potion-turn test class:

1. Merchant journey: create a character, leave the merchant, revisit and claim, revisit after claiming, travel north with the same player, heal, attempt an empty potion, attack, and retreat. Six cases cover both gifts in English, Spanish, and French. Assertions cover original weapon identity/appearance, enchanted maximum or armor protection, fixed maximum health, consumed potion, localized merchant/empty messages, confirmation before enemy response, prompt counts, and exact health under controlled rolls. Armor finishes at 10/18; weapon finishes at 7/18 after the additional normal attack turn.
2. Consumed supply and fresh character: after consuming the potion through Adventure, use the public Merchant encounter boundary to check leaving, claiming, and an attempted alternative repeat claim without replenishment. Both gift cases verify protection/weapon state and a separately created fresh character with one potion, full starting/maximum health, and no prior gift. These direct encounters verify a state invariant; they do not introduce a return-to-merchant route after retreat, which still ends an adventure.

Each case was added and run individually before adding the next. All eight passed immediately against the completed Tickets 01/02 implementation. These were missing regression assertions, not newly introduced gameplay behavior; no artificial failure or unnecessary production change was made to manufacture a red phase. Had a defect been exposed, it would have required a failing test, minimum correction, green verification, and only then any necessary refactor. No such defect was found.

### PRD acceptance review

| PRD criterion | Evidence | Result |
| --- | --- | --- |
| AC1 — one starting potion, independent player and merchant state, no replenishment | Existing constructor/creation tests and both new consumed-supply/fresh-character cases | Pass |
| AC2 — final starting health including race bonuses establishes fixed maximum | Existing all-race/occupation initialization cases; new merchant/response health assertions; manual creation/status inspection | Pass |
| AC3 — combat-only potion command, before first attack, normalization, persistent option | Existing command/out-of-combat tests; initial full-health attempt in every normal console run | Pass |
| AC4 — 10-point healing, cap, actual amount, consumption | Existing literal 5/18, 8/18, 13/18, and 17/18 examples; controlled 10-point and capped demonstrations | Pass |
| AC5 — heal/confirm before exactly one dragon response, armor, misses, defenses | Existing output-order/count and controlled Combat tests; new same-player northward journeys; controlled demos | Pass |
| AC6 — availability first, full/empty rejection costs no turn | Existing rejection/precedence tests; normal console checks and controlled empty-at-full/full-health runs | Pass |
| AC7 — accurate initial/repeated menu status and pre-response confirmation | Initialization/repeated-menu and potion ordering tests; localized console transcripts | Pass |
| AC8 — immediate defeat, no revival/automatic use, unchanged attack and retreat | Existing same-invocation lethal-response and unused-potion defeat tests; normal victory/defeat/retreat output; controlled lethal demos leave next queued p unread | Pass |
| AC9 — invalid/blank retry with no state change or enemy turn | Existing input tests; blank/x checks in normal and controlled console runs | Pass |
| AC10 — English, Spanish, French dictionaries, numeric substitutions, enemy messages | Existing actual dictionary tests, all six new language/upgrade cases, and transcript inspection below | Pass |

### Automated commands and actual results

The temporary helper builds the current test project with `dotnet build CYOA.Tests --no-restore -p:UseSharedCompilation=false --nologo -v:quiet`, builds the previously established in-process runner, and copies the actual message dictionaries. Focused execution:

```sh
zsh /private/tmp/healing02-run.sh HealingPotionTurnTests
dotnet /private/tmp/merchant-test-runner/bin/Debug/net10.0/runner.dll HealingPotionInitializationTests
dotnet /private/tmp/merchant-test-runner/bin/Debug/net10.0/runner.dll MerchantTests
```

Results: potion-turn tests **31 passed, 0 failed**; initialization/status tests **12 passed, 0 failed**; merchant tests **33 passed, 0 failed**. Thus focused potion coverage has 43 passing cases, plus 33 merchant regressions.

Full standard command:

```sh
dotnet test CYOA.Tests --no-restore --nologo -p:UseSharedCompilation=false
```

The build succeeds, but VSTest aborts before executing tests with `System.Net.Sockets.SocketException (13): Permission denied` while binding its communication socket. This is a runner-start failure, not an assertion failure or successful test run.

Full fallback command:

```sh
dotnet /private/tmp/merchant-test-runner/bin/Debug/net10.0/runner.dll
```

Result: **153 passed, 0 failed**, including all existing tests and DataRow cases. The runner invokes the compiled MSTest methods and assertions sequentially; it does not replace production logic. It handles the current suite's method/DataRow cases, console restoration in the tests, and method lifecycle hooks. It does not establish VSTest discovery, adapter integration, or normal parallel execution. The runner and generated artifacts remain outside the repository. Build output retains only the pre-existing MSTEST0037 warning in LanguageTest. `git diff --check` passed.

### Manual gameplay checklist and observed results

These were scripted console acceptance exercises with inspected output, not physical keyboard sessions. Normal games used `dotnet run --no-build` and ordinary random dice, with an adaptive stdin driver. For each language/upgrade pair:

- [x] Start a fresh Human Thief through the actual Game and CharacterCreation prompts; inspect full starting health/maximum health and one potion.
- [x] Travel south, leave, revisit, select the designated gift, revisit again, and observe refusal and return to directions.
- [x] Travel north; attempt a potion before attacking while full, preserving it without a dragon response.
- [x] Enter x and a blank line; inspect localized a/p/r guidance and unchanged menu status.
- [x] Attack until injured, then drink; inspect actual capped healing, zero potions, and the following normal dragon response.
- [x] Attempt the potion again, observe the localized no-potions message, then retreat if alive.
- [x] Inspect original dagger art, complete menu letters, localized labels/outcomes, and numeric values without unresolved placeholders.

Thirteen normal-game executions were recorded. Six completed the whole checklist. Six other executions ended in victory and one in defeat before the whole checklist was completed; those runs were retained as ending evidence, not counted as successful full healing demonstrations. Random outcomes were not treated as proof of exact rolled damage.

| Language | Gift | Successful attempt | Observed actual healing | Outcome |
| --- | --- | --- | --- | --- |
| English | Magical armor | 2 | 4; health 15/19 → 19/19 before response | Checklist complete; retreat |
| English | Magical weapon | 3 | 5; health 13/18 → 18/18 before response | Checklist complete; retreat |
| Spanish | Magical armor | 2 | 7; health 10/17 → 17/17 before response | Checklist complete; retreat |
| Spanish | Magical weapon | 2 | 5; health 11/16 → 16/16 before response | Checklist complete; retreat |
| French | Magical armor | 3 | 7; health 15/22 → 22/22 before response | Checklist complete; retreat |
| French | Magical weapon | 1 | 6; health 6/12 → 12/12 before response | Checklist complete; retreat |

A temporary console demonstration then exercised the approved CharacterCreation/Adventure APIs with controlled rolls, in each of the three languages. It used the approved optional Adventure roll function and changed no production code. All 15 scenarios completed:

| Scenario, repeated in English/Spanish/French | Observed result |
| --- | --- |
| Start at 5/18, leave/claim armor/revisit, drink, enemy roll 7 | Restores 10 to 15/18; applies 5 damage with armor; finishes 10/18, zero potions; empty rejection and retreat work |
| Start at 13/18, drink, guaranteed defense, attempt again | Restores exactly 5 to 18/18; zero potions; no-potions message; retreat |
| Full 18/18 with one potion, then p/x/blank/r | Full-health and validation messages; one potion preserved; no enemy rolls requested |
| Full 18/18 with zero potions, then p/p/r | No-potions message twice, taking precedence over full health; no enemy rolls requested |
| Start at 1/18, drink, enemy roll 12, queue another p | Restores 10 to 11/18; damage leaves -1/18; defeat ends the same invocation; queued p remains unread |

Actual English, Spanish, and French action/status/success/full/empty/validation dictionaries were inspected. Commands remain a/p/r in all three. Spanish uses `poción curativa`/`Pociones curativas`, French uses `potion de soins`/`Potions de soins`, and merchant vocabulary remains consistent with the glossary. Output has the required values and readable messages. No new localization system was added.

### Evidence locations and reproducibility limits

Temporary session artifacts (not intended for commit):

- `/private/tmp/healing03-journey-1.txt` through `healing03-journey-6.txt`, and `healing03-consumed-1.txt`/`healing03-consumed-2.txt`: incremental new integration-case runs.
- `/private/tmp/healing03-focused-turns.txt`, `healing03-focused-init.txt`, `healing03-focused-merchant.txt`, `healing03-full.txt`, and `healing03-standard.txt`: final automated output.
- `/private/tmp/healing03-manual/`: all 13 ordinary game transcripts and exact stdin sequences; `*-accepted.txt` identifies the six complete scenarios. The adaptive driver was `/private/tmp/healing03-manual.py`.
- `/private/tmp/healing03-controlled-demo.txt`: all 15 controlled console demonstrations. Command: `dotnet run --project /private/tmp/healing03-demo/demo.csproj -p:UseSharedCompilation=false -p:NuGetAudit=false`.

The temporary files may be removed by the system. This ticket preserves the actual results, inputs' intent, and limitations; it does not make the temporary tools permanent project dependencies. Standard test execution and any rubric requirements not present in the approved PRD remain the only external verification limits. No known gameplay issue remains.

### Final review

Standards review found no documented violations or actionable maintainability concerns. Spec review found no gameplay or scope-blocking defect and confirmed the automated counts and controlled demonstrations. It noted that the eight new regression cases passed immediately, so literal red-phase compliance for those additions is not claimed. This is a process qualification, not evidence of a missing gameplay behavior; no defect or new behavior required implementation during Ticket 03. Both reviews confirmed that production files are unchanged and that standard-runner and external-rubric limits are clearly disclosed.
