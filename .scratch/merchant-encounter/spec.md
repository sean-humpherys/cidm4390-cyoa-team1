# Merchant Encounter PRD

Status: ready-for-agent

## Problem Statement

Traveling south currently ends the adventure without a meaningful encounter. Players need a small, understandable choice that improves their chances against the dragon and makes the south path useful. The feature must remain manageable for a CIDM 4390 assignment.

## Solution

Traveling south introduces a merchant who offers one free upgrade: magical armor or a magical weapon. Players can also leave without accepting either. After every visit, players return to the direction menu and can travel north to fight the dragon with their upgrade.

## User Stories

1. As a player, I want to meet a merchant when traveling south, so that this path offers a meaningful encounter.
2. As a player, I want three clearly numbered choices, so that I can select armor, a weapon enchantment, or leave.
3. As a player, I want to understand each upgrade's effect before choosing, so that I can make an informed decision.
4. As a player, I want a free upgrade, so that I can benefit without managing currency.
5. As a player, I want magical armor, so that successful enemy hits cause less damage.
6. As a player, I want my existing weapon enchanted, so that it can deal more damage while retaining its identity.
7. As a player of any occupation, I want a useful weapon upgrade, so that the merchant works for my character, including a magician using a spell.
8. As a player, I want to leave without choosing, so that I can postpone my decision.
9. As a player, I want to return after leaving and still claim an upgrade, so that declining temporarily does not forfeit the offer.
10. As a player, I want confirmation of my upgrade, so that I know my choice took effect.
11. As a player, I want to return to the direction menu after visiting, so that I can continue adventuring.
12. As a player, I want my upgrade to remain active against the dragon, so that my choice affects combat.
13. As a player, I want repeat visits to explain when my gift has already been claimed, so that the one-upgrade limit is clear.
14. As a player, I want invalid or blank choices to prompt me again, so that input mistakes do not choose an upgrade for me.
15. As an English-, Spanish-, or French-speaking player, I want merchant dialogue in my selected language, so that I can understand the encounter.
16. As a student maintainer, I want predictable automated tests and a short manual checklist, so that I can demonstrate the feature reliably.

## Requirements and Acceptance Criteria

### AC1: Encounter and navigation

- Given an active adventure, selecting south opens the merchant encounter instead of the previous south-path ending.
- An eligible player sees 1 = magical armor, 2 = magical weapon, and 3 = leave, with the effects and free one-upgrade limit explained.
- Accepting either upgrade, leaving, or visiting after a prior claim returns to the direction menu.
- The same player's upgrade remains active when they subsequently travel north and enter dragon combat.

### AC2: One free upgrade per player per game

- A new player starts with no merchant gift claimed and no armor protection.
- Selecting either upgrade applies it immediately and marks the gift as claimed.
- A player cannot receive both upgrades, swap upgrades, or stack bonuses through repeat visits.
- A repeat visit after claiming displays an already-received message and returns without changing equipment or protection.
- Leaving before claiming changes neither equipment nor eligibility; a later visit still offers both upgrades.
- Eligibility and bonuses belong to the current player and do not persist into a new game.

### AC3: Magical armor

- Selecting armor grants 2 points of damage reduction and leaves the weapon unchanged.
- Each successful, undefended enemy hit applies the rolled damage minus 2, with a minimum of 0.
- For example, rolled damage of 7, 2, and 1 causes health losses of 5, 0, and 0 respectively.
- Armor never heals the player or changes agility, dodge probability, or maximum health.
- Misses and successful dodges continue to cause no damage. Players without armor retain existing damage behavior.
- Combat reports the damage actually applied after armor reduction.

### AC4: Magical weapon

- Selecting the weapon upgrade increases the current weapon's maximum damage by exactly 4 and leaves armor protection unchanged.
- The weapon's type and appearance are preserved, including a magician's spell.
- The existing damage roll uses the increased maximum: a dagger's range changes from 1–6 to 1–10.
- This increases potential damage, not the minimum damage or every individual roll. Strength and hit probability remain unchanged.

### AC5: Input and localization

- Menu options use the same numbers in English, Spanish, and French.
- Invalid or blank input displays a localized validation message and repeats the merchant menu without changing player state.
- End-of-input at the merchant is treated as leaving and returns control to the direction menu without claiming a gift.
- Introductions, choices, upgrade effects, confirmations, leaving text, validation, and already-received messages use the selected language through the existing message system.

## Implementation Decisions

- Add one Merchant class responsible for the encounter, numbered menu, validation, eligibility check, upgrade application, and localized encounter output.
- Player owns armor protection and whether the merchant gift has been claimed. Initialize these consistently for all existing player creation paths.
- Adventure invokes the merchant for south travel and resumes its direction loop using the same player afterward.
- Retain the existing Weapon model; enchant the current weapon by increasing its maximum damage without replacing its type or appearance.
- Combat reduces incoming damage after a successful enemy hit and failed player defense. Clamp applied damage to zero and report the resulting damage and health.
- Allow Combat to accept a predetermined dice-roll function for tests, following the established CharacterCreation approach. Existing callers continue to use normal Die rolls without changing their construction behavior.
- Reuse the current message dictionaries for English, Spanish, and French. Do not introduce a separate translation system.
- Keep the design limited to these responsibilities; no generalized equipment framework is needed.

## Testing Decisions

### Test boundaries and prior art

- Prefer observable behavior: selected menu output, player state, direction navigation, and health changes. Do not assert private methods, internal call order, or a particular class layout.
- Use the existing Adventure console-input/output boundary for journey tests and the Merchant encounter boundary for choice and validation tests. Follow existing Adventure tests' console restoration and nonparallel execution conventions.
- Test combat through its public attack sequences. The only new testing seam is the approved roll-function input to Combat, modeled on CharacterCreation's existing deterministic-roll support.
- Test actual localized message dictionaries as well as focused test dictionaries where appropriate. Assert meaningful merchant text and effects rather than brittle full transcripts.
- These boundaries and the deterministic-roll approach were approved during the requirements review.

### Automated checklist

- [ ] Armor choice grants exactly 2 protection, records the claim, and leaves the weapon unchanged.
- [ ] Weapon choice adds exactly 4 maximum damage, records the claim, preserves type and appearance, and leaves armor unchanged; cover every occupation, including the magician.
- [ ] Leaving preserves player state and allows a subsequent claim.
- [ ] Repeat visits after either claim cannot stack, swap, or grant a second gift and display the already-received message.
- [ ] Invalid and blank input retry without granting an upgrade; a subsequent valid choice works.
- [ ] Merchant end-of-input exits safely without claiming an upgrade. Test at the encounter boundary; broader direction-menu end-of-input handling is outside this change.
- [ ] Each merchant outcome returns to the direction menu; journey tests supply a later exit or north choice so input is complete.
- [ ] A south-to-north journey uses the same upgraded player when entering dragon combat, covering both upgrades.
- [ ] Controlled enemy rolls verify armor reduction for damage below, equal to, and above 2, including zero health loss and accurate displayed damage.
- [ ] Controlled player rolls verify combat uses the enchanted weapon's increased maximum and applies the resulting damage to the dragon.
- [ ] Unarmored damage, misses, successful dodges, and existing combat defeat behavior retain their expected behavior.
- [ ] Each supported language contains and displays the required merchant messages without missing-message output.
- [ ] Update existing south-path tests for the merchant behavior and run the complete existing test suite.

### Manual checklist

- [ ] Visit the merchant in English, Spanish, and French; verify readable dialogue, option numbers, validation, confirmations, and direction-menu return.
- [ ] Leave, revisit, and claim an upgrade; revisit again to verify the one-gift limit.
- [ ] In separate games, take armor and the weapon upgrade north, enter dragon combat, and check their effects and combat messages.

## Out of Scope

- Currency, prices, buying, selling, and trading.
- Inventory management, additional equipment classes, or an equipment hierarchy.
- Multiple gifts, upgrade stacking, swapping, unequipping, or additional merchant items.
- Saving upgrades or claim eligibility between games.
- Changes to character occupations, base weapon types, enemy statistics, hit probability, or dodge probability.
- New enemies, additional paths, general combat redesign, or unrelated cleanup.
- A new localization framework or additional languages.
- General end-of-input handling outside the merchant encounter.

## Further Notes

- The user approved the consolidated requirements and automated/manual testing checklist before requesting this PRD.
- This document records an agreed, bounded implementation target for the CIDM 4390 assignment. The ready-for-agent status indicates specification readiness, not authorization to implement during this documentation task.
- No game code is implemented by this PRD, and no test execution or manual playthrough is claimed here.
- Merchant, magical armor, and magical weapon use the project's agreed glossary vocabulary.
