# Healing Potion PRD — CIDM 4390 Part 4, Category A

Status: ready-for-agent

## Problem Statement

Players have no way to recover health during dragon combat. They need a limited, understandable healing choice that works for every occupation, creates a trade-off against attacking, and remains separate from the merchant's magical armor or magical weapon gift. Players also need to see their health and remaining potion supply before choosing an action.

## Solution

Every new player starts with one healing potion. During dragon combat, the player may drink it instead of attacking, including before the first attack. A successful drink restores up to 10 health points without exceeding the player's fixed maximum health, consumes the potion, and then permits the dragon's normal attack. Unsuccessful attempts preserve state and do not give the dragon a turn. All instructions and feedback use the selected English, Spanish, or French language.

## User Stories

1. As a player of any occupation, I want to start with one healing potion, so that healing is available regardless of my character choice.
2. As a player, I want my potion to be separate from the merchant gift, so that choosing magical armor or a magical weapon does not remove access to healing.
3. As a player, I want a new game to restore my starting potion supply, so that a previous game's consumption does not affect a new character.
4. As a player, I want to drink from the dragon's action menu, so that I can choose healing when deciding my next combat action.
5. As a player, I want to drink before my first attack, so that I do not have to attack to unlock the healing choice.
6. As a player, I want drinking to replace attacking for that turn, so that using my limited healing resource is a meaningful decision.
7. As a player, I want healing to happen before the dragon attacks, so that the recovered health can help me survive that attack.
8. As a player wearing magical armor, I want its protection during the dragon's response, so that my merchant upgrade continues to work normally.
9. As a player, I want the potion to restore 10 health points, so that its effect is predictable.
10. As a player, I want healing capped at maximum health, so that a potion cannot increase my health capacity.
11. As a player, I want maximum health to include my starting race bonuses, so that my chosen character's starting health defines the cap.
12. As a player, I want maximum health to remain fixed, so that damage and healing do not move the cap.
13. As a player at full health, I want an attempted drink to preserve my potion, so that I do not waste it.
14. As a player at full health, I want the menu to return without a dragon attack, so that an unsuccessful attempt does not cost a turn.
15. As a player, I want a successful drink to consume my only potion, so that it cannot be reused.
16. As a player with no potion, I want a clear explanation and another choice, so that I know why drinking is unavailable.
17. As a player, I want consistent attack, potion, and retreat commands across languages, so that changing language does not change the controls.
18. As a player, I want uppercase commands and surrounding spaces accepted, so that minor input variations do not reject my choice.
19. As a player, I want the potion option to remain visible after use, so that attempting it can explain my empty supply.
20. As a player, I want current health, maximum health, and remaining potions at every combat menu, so that I can make an informed choice.
21. As a player with no potion and full health, I want the no-potions message to take precedence, so that availability is explained consistently.
22. As a player, I want a successful drink to report the actual amount restored, so that partial healing near maximum health is understandable.
23. As a player, I want the healing confirmation before the dragon's response, so that I can follow the order of events.
24. As a player, I want defeat to remain immediate at zero health or below, so that potions do not silently become resurrection items.
25. As a player, I want merchant visits to leave potion availability unchanged, so that the one-potion limit remains clear.
26. As an English-, Spanish-, or French-speaking player, I want all new instructions, status labels, and outcomes localized, so that I can understand healing in my selected language.
27. As a player, I want invalid and blank combat input to retry without penalty, so that input mistakes do not consume a potion or give the dragon a turn.
28. As a student maintainer, I want observable acceptance examples and deterministic tests, so that I can develop the feature through red-green-refactor and demonstrate it reliably.

## Requirements and Acceptance Criteria

### AC1: Starting supply and independence

- Given any newly created player, regardless of occupation or race, the player starts with exactly one healing potion.
- Existing player constructors and the character-creation flow must initialize a consistent fresh-game potion supply.
- The potion belongs to that player for that game. A different new player starts with one potion even if a previous player used theirs.
- Starting potion supply is independent of the merchant gift and its claim state.
- Leaving the merchant, claiming either upgrade, and revisiting after a claim do not grant, consume, or replace a potion.
- No in-game action replenishes the starting potion.

### AC2: Fixed maximum health

- After character creation, maximum health equals the player's final starting health, including all existing race bonuses.
- A directly constructed player has maximum health equal to that constructor's starting health.
- Maximum health remains fixed for that game. Damage, potion use, magical armor, magical weapon, merchant visits, and menu retries do not change it.
- Preserve the existing character-creation health calculation and bonuses; do not change rolls or rebalance races.
- Example: a player whose final starting health is 18 begins at 18/18 and retains a maximum of 18 after taking damage.

### AC3: Combat availability and commands

- Every dragon action menu offers `a` = attack, `p` = drink a healing potion, and `r` = retreat.
- The same command letters apply in English, Spanish, and French.
- Normalize command case and trim surrounding whitespace: `P` and ` p ` select the same action as `p`; preserve equivalent handling of `a` and `r`.
- The potion choice is available before the first attack and between subsequent combat actions while the player is alive.
- Keep the potion option visible when no potion remains and when health is full.
- Potion use is limited to the dragon combat action menu. Do not add potion use to direction, character-creation, or merchant menus.

### AC4: Successful healing

- Given a living player with one potion and current health below maximum health, selecting `p` succeeds.
- Restore 10 health points or the amount needed to reach maximum health, whichever is smaller.
- Consume exactly one potion, leaving zero. Maximum health, strength, agility, weapon, and merchant gift state remain unchanged.
- Examples before the dragon's response:

| Current health | Maximum health | Potions before | Actual restored | Health after healing | Potions after |
| --- | --- | --- | --- | --- | --- |
| 5 | 18 | 1 | 10 | 15 | 0 |
| 8 | 18 | 1 | 10 | 18 | 0 |
| 13 | 18 | 1 | 5 | 18 | 0 |
| 17 | 18 | 1 | 1 | 18 | 0 |

- A partial restoration still consumes the whole potion and uses the turn.

### AC5: Turn order and dragon response

- A successful drink replaces the player's attack for that turn; it deals no damage to the dragon.
- Apply healing and consume the potion first.
- Next, display the actual restored health, updated current/maximum health, and remaining potion count.
- Only after that confirmation, the dragon gets exactly one normal attack sequence.
- Preserve existing hit, defense, damage, and defeat behavior, including magical armor's reduction of successful, undefended incoming hits by 2, clamped to zero.
- If the player survives, return to the action menu with post-attack health and zero potions displayed.
- Example without armor: 5/18 health, successful drink to 15/18, then a successful undefended dragon hit rolling 7 leaves 8/18.
- Example with magical armor: the same situation leaves 10/18 because the rolled 7 applies 5 damage.
- A miss or successful defense after drinking leaves the healed health unchanged.

### AC6: Rejected potion attempts and precedence

Evaluate requests in this order for living players:

| Condition | Feedback meaning | State change | Dragon response |
| --- | --- | --- | --- |
| No potion remains, regardless of whether health is full | You have no healing potions left | None | None |
| A potion remains and health is at maximum | You are already at full health | None; preserve the potion | None |
| A potion remains and health is below maximum | Successful healing confirmation | AC4 | AC5 |

- Both rejected outcomes return to the combat action menu and redisplay current status.
- Repeated rejected attempts do not change health, maximum health, potion count, equipment, or gift state.
- Specifically, at full health with zero potions, display the no-potions message rather than the full-health message.

### AC7: Combat status and successful-use feedback

- At every dragon action menu, display current health, maximum health, and remaining healing potions using localized labels.
- This includes the first menu and menus after an attack, successful nonlethal potion turn, rejected potion attempt, or invalid input.
- Status represents the state at the time the menu is displayed, including any preceding dragon damage.
- Successful healing confirmation reports actual restored points rather than always claiming 10. It includes current/maximum health and potions remaining before the dragon's attack.
- Example meaning: "You recover 5 health points. Health: 18/18. Healing potions: 0."
- Exact punctuation and line layout may follow existing conventions; all values and meanings must remain observable.

### AC8: Defeat, attack, and retreat

- Reaching current health of zero or below ends the adventure immediately under the existing defeat behavior.
- A remaining potion cannot activate automatically, postpone defeat, or revive the player.
- After a lethal dragon response to drinking, display defeat and stop offering actions; do not process a subsequent queued potion command.
- Concrete lethal example: 1/20 health heals to 11/20; an unarmored undefended hit of 12 leaves -1 and ends the adventure with zero potions.
- Attack and retreat retain their existing meanings. Adding the potion option must preserve magical weapon damage, magical armor protection, dragon victory/defeat endings, and merchant navigation.

### AC9: Invalid and blank input

- Blank, whitespace-only, or unrecognized combat input displays a localized reminder containing `a`, `p`, and `r` and repeats the action menu.
- Invalid input does not change health, maximum health, potion supply, player equipment, or dragon health and does not give the dragon a turn.
- Subsequent valid input is handled normally. Complete test input must include a later valid action or retreat.
- General end-of-input behavior outside the existing merchant encounter is not changed by this feature.

### AC10: Localization

Use the existing message dictionaries and selected-language mechanism for all new or modified player-facing dialogue:

| Message purpose | Required meaning/data in each language |
| --- | --- |
| Combat action prompt | Attack `a`, drink healing potion `p`, retreat `r`; option remains visible after consumption |
| Combat status labels | Current health, maximum health, and healing potions remaining |
| Successful drink | Actual restored points, updated current and maximum health, remaining potions |
| No potion | No healing potions left; higher priority than full-health rejection |
| Full health | Already at full health; potion is preserved |
| Invalid input | Enter `a`, `p`, or `r` |

- Provide readable English, Spanish, and French text through the actual dictionaries, not hard-coded English or a new translation framework.
- Format numeric values correctly with no missing entries or unresolved placeholders.
- Existing localized enemy attack, damage, defense, and defeat messages continue to apply after successful healing.

## Implementation Decisions

- Player owns current health, fixed maximum health, and remaining potion supply. Initialize fresh-player defaults consistently and finalize maximum health after character-creation bonuses have been applied.
- Do not interpret later assignments to current health during combat as changes to maximum health. Preserve valid existing constructor usage and update affected test setup explicitly where a damaged player is needed.
- Adventure's existing dragon action loop handles the new choice, status display, rejected attempts, and return navigation. A successful potion action must be distinguishable from rejection so that only success grants the enemy response.
- Reuse Combat's normal dragon attack and defeat behavior for the response, including armor, rather than duplicating combat rules in potion-specific logic.
- Keep potion validation and application coherent: check availability, check full health, then heal and consume before reporting success and invoking the dragon response. Private method layout and exact property names are implementation choices, not acceptance criteria.
- Reuse CharacterCreation's final health calculation and its existing controlled-roll support. Reuse Combat's existing controlled-roll support for predictable combat evidence.
- Preserve ordinary Die behavior for existing gameplay callers. No generalized inventory, item hierarchy, potion framework, or new persistence mechanism is needed.
- Reuse the existing localization system for the action prompt, status, confirmation, and rejection messages. Update focused test dictionaries and dictionary-completeness expectations when introducing messages.

## Testing Decisions

### Approved public test boundaries

The user approved the following test boundaries after reviewing this specification. They reuse the existing public interfaces:

1. **CharacterCreation and Player construction:** verify one potion, final starting/maximum health, race bonuses, all occupations, and independent fresh-player state. This boundary catches initialization errors; it does not verify combat orchestration.
2. **Adventure console interaction:** the primary behavior boundary for menu choices, status, normalization, retry, outcome messages, turn order, defeat, and merchant-to-north journeys. Assert output and public player/dragon state where available, not private helpers or collaborator call order.
3. **Existing public Combat sequences with controlled rolls:** verify exact enemy damage, armor reduction, miss/defense behavior, and lethal outcomes. These complement journey tests; isolated Combat tests alone do not prove that Adventure grants or suppresses a dragon turn correctly.

These boundaries are confirmed for implementation testing. The user subsequently approved a narrow optional controlled-roll function on Adventure, passed to Combat, to verify exact responses within one Adventure invocation. The existing Adventure constructor continues to use normal Die behavior for existing callers. No other injection interface is approved; avoid a new seam for each component or assertions against private methods.

### Test style and prior art

- Follow the existing Adventure and Merchant console test conventions: complete input, restore console streams in `finally`, and run console-interacting tests nonparallel.
- Follow CharacterCreation's predetermined-roll tests for health calculations and the merchant feature's public Combat controlled-roll tests for numerical examples.
- Use literal worked expectations from AC4–AC8, rather than recomputing the expected values using the implementation's algorithm.
- Assert meaningful localized text and exact displayed values rather than brittle full-transcript snapshots. Read the actual dictionaries in all three languages as well as any focused test dictionaries.
- Verify the absence or presence of an enemy response through observable behavior, such as unchanged health under a controlled hit-capable scenario or one reported attack/damage result. Do not use private method access or internal collaborator mocks.
- Existing merchant tests and ordinary Combat construction must continue to work. Broaden testing only when new failures or unresolved risks justify it.

### Automated acceptance checklist

- [ ] Both existing player constructors start with one potion and coherent current/maximum health.
- [ ] Character creation initializes maximum health after race bonuses; cover all existing races and occupations through appropriate parameterized tests without a redundant full Cartesian product.
- [ ] Creating a second player after a prior player's potion use gives a fresh potion and an independent maximum health.
- [ ] Exact, capped, and one-point healing match AC4; successful use leaves zero potions and does not damage the dragon.
- [ ] Drinking before the first player attack is accepted; potion use is unavailable from other menus.
- [ ] Successful drink heals and confirms first, then produces exactly one dragon response; a surviving next menu shows post-response health.
- [ ] Controlled undefended hits after healing demonstrate unarmored and magical-armor examples in AC5; misses and defenses preserve healed health.
- [ ] Full-health rejection preserves the potion and grants no dragon turn.
- [ ] Zero-potion rejection works both when injured and at full health, takes precedence over full health, and grants no dragon turn.
- [ ] An unsuccessful attempt can be followed by an ordinary valid action, with no hidden state changes.
- [ ] `p`, `P`, and surrounding-space variants work; `a` and `r` retain case/whitespace handling.
- [ ] Blank, whitespace-only, and invalid input retry with localized `a`/`p`/`r` guidance and no state change or enemy response.
- [ ] Every initial and repeated combat menu shows accurate current health, maximum health, and potion count; `p` remains visible at zero potions.
- [ ] Actual healing and status values appear before enemy response output in every language; rejection and validation messages are also localized correctly.
- [ ] A lethal response after healing ends play immediately; an unused potion does not prevent ordinary defeat or process a later queued action.
- [ ] Leaving, claiming either merchant gift, and repeat visits preserve potion supply and maximum health; subsequent north travel retains both the potion state and merchant upgrade.
- [ ] Existing attack, retreat, weapon enchantment, armor, character creation, dictionary, and merchant tests pass with any necessary fixture updates.

### Red-green-refactor process and validation

- Work in vertical behavior slices at confirmed public boundaries: write one failing test for an acceptance behavior, observe the meaningful failure, implement only enough to pass, then refactor while preserving passing behavior. Do not implement code during this specification task.
- Prefer an initial tracer test covering a wounded player choosing `p`, observing capped healing and consumption, followed by the normal enemy response; then cover rejection, initialization, localization, and integration incrementally as dependencies require.
- Run the relevant focused test file and build/type checks regularly. At completion, run the full automated suite and record the exact command, counts, failures, and environment limitations.
- The standard project test command is `dotnet test CYOA.Tests`. Prior work encountered sandbox socket restrictions in VSTest. A failure to start the runner is not a test pass; report it explicitly. If a fallback is used, identify its execution semantics and do not claim standard adapter or parallel-run verification.
- Manually demonstrate both full and capped healing, full-health preservation, repeated no-potion attempts, invalid input, and a successful potion turn followed by dragon combat. Demonstrate merchant independence with each upgrade and inspect English, Spanish, and French output. Record actual results separately from this planned checklist.

## Approved Decision Traceability

| Interview decision | Approved outcome | Acceptance coverage |
| --- | --- | --- |
| Q1 | Every new player gets one potion, independent of occupation and merchant gift | AC1 |
| Q2 | Use only from dragon combat menu, including before first attack | AC3 |
| Q3 | Healing first, then normal dragon turn; armor still applies | AC5 |
| Q4 | Restore 10, capped at maximum health | AC4 |
| Q5 | Maximum health is final starting health including race bonuses, fixed for game | AC2 |
| Q6 | Full health preserves potion, shows localized message, costs no turn | AC6 |
| Q7 | Success consumes only potion; empty supply rejects without dragon turn | AC4, AC6 |
| Q8 | Shared `a`/`p`/`r`, case/whitespace normalization, persistent potion option | AC3 |
| Q9 | Every combat menu shows current/maximum health and remaining potions | AC7 |
| Q10 | Check availability first, including when full | AC6 |
| Q11 | Actual restored amount and updated status confirmed before dragon attacks in each language | AC5, AC7, AC10 |
| Q12 | Immediate defeat at zero or below; no automatic use or revival | AC8 |
| Q13 | No replenishment; new game alone gives a new starting potion | AC1 |
| Q14 | Blank/invalid combat input retries with localized guidance, no state change or enemy turn | AC9, AC10 |

## Out of Scope

- Code implementation, implementation tickets, commits, or pushes during specification creation.
- Buying, selling, finding, refilling, crafting, or replacing potions; merchant potion gifts.
- Multiple starting potions, potion varieties, random healing, inventory frameworks, or item hierarchies.
- Out-of-combat healing, resurrection, automatic potion activation, or delayed defeat.
- Changes to merchant gift eligibility, armor/weapon upgrade effects, race bonuses, occupations, enemy statistics, hit/defense probabilities, or base damage rules.
- Changes to maximum health after character creation, saving state between games, additional enemies, new languages, or a replacement localization system.
- General console end-of-input handling or unrelated refactoring.

## Further Notes

- The user approved all 14 gameplay decisions in the grill-with-docs interview and requested this specification. The glossary's Healing potion and Maximum health definitions govern vocabulary.
- `ready-for-agent` records specification readiness under the local issue-tracker convention; it does not authorize implementation in this conversation. The user has confirmed the CharacterCreation, Adventure console, and existing controlled-roll Combat test boundaries; no gameplay or test-boundary decisions remain pending.
- Category A is the user's assignment classification. The linked CIDM 4390 Part 4 assignment document was inaccessible during the interview; exact rubric compliance has not been independently verified. This specification synthesizes the approved decisions without inventing additional assignment requirements.
- This document describes planned behavior and validation. No potion code, tests, tickets, or test results were produced by writing it.
