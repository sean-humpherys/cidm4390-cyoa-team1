# 02: Drink a healing potion as a complete combat turn

Status: ready-for-agent

**What to build:** Players can choose `p` instead of attacking, with capped healing, consumption, safe rejection, and the correct dragon response.

**Blocked by:** 01 — Initialize healing potions and display combat health.

## Acceptance criteria

- [ ] Offer `a`, `p`, and `r` in every combat menu, including before the first attack. Accept uppercase and surrounding spaces.
- [ ] Keep `p` visible after consumption; potion use remains limited to combat.
- [ ] A successful drink restores up to 10 health points, consumes the potion, and preserves maximum health and equipment.
- [ ] Display actual healing, updated current/maximum health, and remaining potions before exactly one normal dragon attack. Drinking does not attack the dragon.
- [ ] Apply existing armor protection, misses, defenses, and defeat behavior normally.
- [ ] Check availability first. No-potion and full-health rejections return to the menu without state changes or a dragon turn.
- [ ] Blank and invalid input display `a`/`p`/`r` guidance and retry without penalty.
- [ ] Defeat at zero or below is immediate; potions never activate automatically or revive the player.
- [ ] All introduced prompts, confirmations, and rejection messages work in English, Spanish, and French.

## Test requirements

- [ ] Cover the PRD's healing examples: 5/18 → 15/18, 8/18 → 18/18, 13/18 → 18/18, and 17/18 → 18/18.
- [ ] Verify consumption, confirmation ordering, one dragon response, and updated next-menu status through Adventure.
- [ ] Test full-health preservation, repeated empty-supply attempts, and no-potion precedence at full health.
- [ ] Verify command normalization and invalid/blank retries followed by valid input.
- [ ] Use controlled Combat rolls for exact damage, armor, misses, defenses, and lethal outcomes; verify Adventure's orchestration separately.
- [ ] Verify an unused potion cannot prevent defeat and queued commands are not processed after defeat.
- [ ] Exercise actual dictionaries in all three languages.

## Boundary constraint

Use the approved interfaces. If deterministic Adventure tests require an additional randomness interface, propose that narrow change before introducing it.

## TDD requirements

Follow one failing test → observe failure → minimal implementation → passing test → refactor, repeating incrementally. Write one failing test at a time at the approved public boundaries. Do not batch all tests before implementing behavior.

## Traceability

PRD AC3–AC10.

## Comments

2026-11-16: Implemented potion handling in the dragon action loop and English/Spanish/French prompts, confirmations, rejection messages, and input guidance. The response reuses existing Combat behavior; a defeated player is rejected before another combat menu is displayed. Maximum health, creation, and merchant logic from Ticket 01 are unchanged. Ticket 01's temporary assertion that no potion command is advertised was removed because this ticket makes that command functional. No Ticket 03 work, commits, or pushes were performed.

TDD evidence: eleven red/green cycles were run with one failing executable test case introduced at a time (no failing parameterized batches). Cycles covered the three localized menu prompts, successful 10-point healing/consumption/response, capped healing, full-health preservation, empty-supply precedence, invalid-input guidance, Spanish/French outcomes, and defeated-player rejection. Additional literal healing examples, normalization, empty injured supply, controlled damage/armor/miss/defense/lethal cases, and out-of-combat rejection were added and run one at a time as regression checks against the already-implemented behavior. Refactoring renamed the player fixture and shared transcript-order assertions across languages.

Focused command: temporary helper `/private/tmp/healing02-run.sh HealingPotionTurnTests` builds the test project, builds the previously established temporary in-process runner, copies actual dictionaries, and invokes the compiled test class. Result: 22 passed, 0 failed. Full sequential fallback runner result: 144 passed, 0 failed, including Ticket 01 and existing merchant tests. Build succeeded; the pre-existing LanguageTest analyzer warning remains. `git diff --check` passed.

Standard command `dotnet test CYOA.Tests --no-restore --nologo -p:UseSharedCompilation=false` built successfully but aborted before execution with `SocketException (13): Permission denied` while binding VSTest's communication socket. The fallback executes compiled MSTest assertions sequentially and does not verify standard discovery, adapters, or normal parallel execution. Logs are `/private/tmp/healing02-01-red.txt` through `healing02-11-green.txt`, `/private/tmp/healing02-focused.txt`, `/private/tmp/healing02-full.txt`, `/private/tmp/healing02-standard.txt`, and the per-regression logs. Temporary runners are outside the repository.

Verification limitation: Adventure output verifies healing confirmation precedes exactly one normal enemy response and that subsequent status is correct in all three languages. Exact enemy damage, magical armor, misses, defenses, and lethal outcomes are checked separately through existing controlled-roll Combat calls on a healed player. The lethal test then re-enters Adventure with the defeated player; it does not deterministically test a lethal potion response within the same Adventure invocation. Review found the gameplay termination logic correct, but that integration case remains a coverage gap. An optional Adventure controlled-roll function was proposed to the user; no additional interface has been approved or added. Keep this limitation visible until the seam decision is resolved; do not describe the ticket as fully verified.

Standards review: no documented violations; fixture naming concern resolved. Spec review: no identified gameplay defect; localized ordering/count assertions and controlled healed-player miss/defense checks were strengthened after review. The same-invocation lethal-response coverage limitation above remains outstanding.

2026-11-16 follow-up: The user approved the narrow optional Adventure controlled-roll function. Added one failing test, `LethalPotionResponse_EndsSameAdventureWithoutReadingAnotherAction`, and observed its missing-overload compilation failure before implementing the constructor overload and passing its roll function to Combat. The original constructor delegates to ordinary Die rolls. No change to defeat handling was needed.

The deterministic test now demonstrates, in one Adventure invocation, health 1/18 → 11/18 from healing → -1/18 from a controlled 12-damage enemy response; zero remaining potions; defeat output; no subsequent action or direction prompt; and the queued second `p` left unread. This resolves the previously documented same-invocation lethal-response coverage gap.

Final follow-up validation: the new test passed; focused potion-turn suite 23 passed, 0 failed; full sequential fallback suite 145 passed, 0 failed. Standard `dotnet test CYOA.Tests --no-restore --nologo -p:UseSharedCompilation=false` still builds then aborts on the sandbox's denied VSTest socket, so ordinary adapter/parallel execution remains unverified here. Evidence: `/private/tmp/healing02-seam-red.txt`, `healing02-seam-green.txt`, `healing02-seam-focused.txt`, `healing02-seam-full.txt`, and `healing02-seam-standard.txt`. No Ticket 03 work, commits, or pushes.
