# Category B: Inventory Display

Status: ready-for-agent

Final approved specification synthesized from the completed grill-with-docs interview and the existing Inventory glossary entry. The user approved the Adventure console testing interface and both implementation tickets; implementation and verification are complete. The user subsequently reported all 324 tests passing with standard dotnet test in the regular VS Code terminal.

## Problem Statement

Players need to inspect their current equipment and remaining healing potions before choosing a direction, including after visiting the merchant. They should not have to infer equipment values from earlier messages.

## Solution

Offer Inventory in the main direction menu as a read-only summary of the current weapon and its maximum damage, magical armor protection, and healing potions remaining. Read the existing player and equipment state each time the display is requested. Do not introduce equipment management or an inventory framework.

## User Stories

1. As a player, I want to discover Inventory in the main direction menu, so that I can inspect my equipment before traveling.
2. As a player, I want to see my current weapon, so that I know what I carry.
3. As a player, I want to see its current maximum damage, so that I can see the effect of a magical weapon upgrade.
4. As a player, I want to see my magical armor protection, so that I know my damage reduction.
5. As a player without magical armor, I want armor protection displayed as 0, so that I know I have no protection.
6. As a player, I want to see healing potions remaining, so that I can plan their use.
7. As a player with zero healing potions, I want that count displayed explicitly, so that I know my supply is empty.
8. As a player, I want inspection to preserve my health and possessions, so that viewing information has no gameplay cost.
9. As a player, I want to continue choosing a direction after inspection, so that I can resume the adventure.
10. As a player, I want repeated inspections to show current values, so that merchant upgrades are visible immediately.
11. As an English-speaking player, I want the display and command guidance in English, so that I understand them.
12. As a Spanish-speaking player, I want the display and command guidance in Spanish, so that I understand them.
13. As a French-speaking player, I want the display and command guidance in French, so that I understand them.
14. As a player, I want existing travel, merchant, and combat choices to retain their behavior, so that inspection does not disrupt the adventure.

## Implementation Decisions

### Approved requirements

- Inventory has the glossary meaning: a read-only summary of equipment and healing potions carried by the player.
- Display exactly four labeled values: weapon name, current maximum damage, armor protection, and healing potions remaining. Include no heading, artwork, character stats, or additional information in the summary. The direction menu is displayed separately after the summary.
- Availability is limited to the main direction menu, as required by the current request. Do not offer it in character creation, the merchant encounter, or dragon combat.
- Reuse Adventure's menu flow, the current Player and Weapon objects, and the existing Messages/language dictionaries. No new inventory collection, item hierarchy, persistence, or parallel equipment state.
- Read weapon type and maximum damage from the current weapon; armor protection and remaining potions come from the player. Do not infer upgrades from merchant gift eligibility: that flag cannot distinguish which gift was selected.
- Preserve the player and weapon object identities and every property during inspection. Do not heal, consume potions, change damage or protection, claim a gift, roll dice, travel, or trigger combat.
- Preserve existing merchant and healing rules. Display the actual current maximum damage after a magical weapon upgrade; do not apply the upgrade again when rendering.

### Interaction and rendering

- Use `i` in all three languages, with the menu's existing trimming and case-insensitive input handling. Accept `i`, `I`, and ` I `.
- Print only the four labeled values below, then immediately redisplay the main direction menu. Do not add a pause, confirmation, or Inventory submenu.
- Always show numeric armor protection and potion counts, including 0. Use the same labels for zero and positive values; do not substitute absence text.
- Keep the weapon's existing localized type name after the merchant's magical weapon upgrade and show the updated maximum damage. Do not add an enchanted label or invent an enchanted status flag.
- Update the main-menu invalid-choice guidance to include `i`; blank and unsupported input retain the retry behavior and leave state unchanged.
- Do not add character stats, artwork, status annotations, item management, or other information.

### Localization contract

All Inventory labels and menu guidance use the selected language through the existing localization mechanism. Numbers reflect current state. Placeholders below describe substitutions, not literal output. Existing direction instructions remain present.

| Meaning | English | Spanish | French |
| --- | --- | --- | --- |
| Menu addition | Enter 'i' to view inventory. | Escribe 'i' para ver el inventario. | Tapez 'i' pour afficher l’inventaire. |
| Weapon | Weapon: {weapon} | Arma: {weapon} | Arme : {weapon} |
| Maximum damage | Weapon maximum damage: {damage} | Daño máximo del arma: {damage} | Dégâts maximums de l’arme : {damage} |
| Armor protection | Armor protection: {protection} | Protección de la armadura: {protection} | Protection de l’armure : {protection} |
| Potion count | Healing potions remaining: {potions} | Pociones curativas restantes: {potions} | Potions de soins restantes : {potions} |
| Invalid main-menu input | Invalid choice. Enter 'n', 's', 'i', or 'e'. | Opción no válida. Escribe 'n', 's', 'i' o 'e'. | Choix invalide. Tapez 'n', 's', 'i' ou 'e'. |

Reuse the established weapon translations:

| English | Spanish | French |
| --- | --- | --- |
| long sword | espada larga | épée longue |
| lightning bolt spell | hechizo de rayo | sort de foudre |
| dagger | daga | dague |
| long bow | arco largo | arc long |

Every language must provide every message needed by this feature with matching placeholder names. No missing-key exceptions, unresolved placeholders, or English fallback for the four supported weapon types in Spanish/French output. Keep existing message meanings compatible with their current callers.

## Acceptance Criteria

The feature requirements and Adventure console testing interface below are approved. In every language, each inspection prints exactly the four labeled values and then returns automatically to the direction menu.

| ID | Given / When | Then |
| --- | --- | --- |
| AC1 | Main direction menu is displayed in each supported language | Inventory is discoverable alongside the existing north, south, and exit choices. |
| AC2 | A default player inspects Inventory | Show the localized long sword, maximum damage 12, numeric armor protection 0, and 1 healing potion remaining. |
| AC3 | Each supported weapon type is carried with a known maximum damage | Show the correct localized type and the actual numeric maximum damage, without changing the weapon. Cover all four types in all three languages. |
| AC4 | A dagger has maximum damage 6; input is `i`, south, magical weapon gift, `i`, exit | First summary shows 6; second shows 10. Type remains the localized dagger name with no enchanted label, protection remains 0, potion count remains 1. |
| AC5 | Input is `i`, south, magical armor gift, `i`, exit | Numeric armor protection changes from 0 to 2 only because of the gift. Weapon maximum damage and potion count stay unchanged. |
| AC6 | Player has 0 potions before inspection | Show 0 explicitly; inspection does not replenish, heal, or offer potion use. A supplied count greater than 1 may be used as a rendering fixture; this does not introduce a gameplay action that grants extra potions. |
| AC7 | Input is `i`, `i`, exit | Produce exactly two summaries and return to the direction prompt after each, without consuming an extra input or ending the adventure early. |
| AC8 | Input is ` I `, exit | Produce one summary using the same behavior as lowercase `i`. |
| AC9 | Blank or unsupported main-menu input precedes inspection and exit | Show localized guidance containing all four commands, retry, and do not alter player state. |
| AC10 | Player has nondefault health, attributes, equipment values, potion count, and gift status | Inspection preserves all values and player/weapon identities. No roll, travel message, merchant action, attack, or enemy response occurs because of inspection. |
| AC11 | Player enters `i` inside merchant or combat interaction | Existing invalid-input handling applies; no Inventory summary appears and no gameplay state changes or combat turn is caused by that invalid command. |
| AC12 | Player inspects Inventory then selects north, south, or exit | The selected existing path behaves normally. Inspection does not intercept the next input. Existing combat and merchant regression behavior remains intact. |
| AC13 | Any inventory journey runs in English, Spanish, or French with real dictionaries | All feature wording and supported weapon names use that language; fields contain expected values and no unresolved placeholders. |
| AC14 | A player with a previously consumed potion enters a new main-menu inspection session using the same Player | The summary reports 0, preserving equipment upgrades and the consumed state; inspection does not reinitialize the player. |

## Testing Decisions

- Confirmed single feature seam: Adventure's public StartAdventure entry point with scripted console input, captured output, a supplied Player, and real selected-language Messages. This catches command routing, live state, localization, return flow, and absence of mutation; it does not test translation helpers independently or claim exhaustive combat correctness. This reuses the existing console interface explicitly approved by the user; no additional test interface is necessary.
- Follow the existing Adventure and HealingPotionTurn console-test conventions: nonparallel execution, finite scripted input ending in exit or retreat, and restoration of console streams in a finally block. Load real dictionaries for localization coverage rather than proving only a synthetic test dictionary works.
- Observe output and public player/equipment state. Do not assert private helper names, message IDs, internal call counts, a particular new class, or full whitespace-sensitive screen snapshots. Use independent literal values and localized phrases from this specification for expected results.
- Use the existing controlled-roll Adventure constructor only where needed to prove inspection consumes no randomness or to make a combat regression journey deterministic. No new production testing seam is proposed.
- Exercise merchant upgrades through the same adventure journey for AC4–AC5. For zero-potion display, supply an already-consumed Player; combat retreat currently ends the adventure, so do not invent a return-to-main-menu path after combat.
- Snapshot state immediately around an inspection-only journey; allow mutations caused by explicitly scripted merchant/combat actions in mixed journeys. Distinguish those actions from inspection.
- Plan vertical red-green-refactor work: one failing behavior test, minimum implementation to pass, then review/refactor with passing tests before the next behavior. Do not bulk-write tests before implementation.
- Suggested slice order: default English inspection and return; read-only repeat inspection and normalized input; live merchant upgrades and zero counts; Spanish/French coverage and weapon variants; invalid input and menu-scope regressions. Each slice uses the same approved public seam.
- Future validation should run the focused feature tests and relevant existing adventure, merchant, potion, and message regressions. Record actual commands/results and any runner limitations then. Prior healing-potion acceptance evidence records standard VSTest aborting before test execution because its communication socket could not bind (permission denied); a sequential fallback passed 153 cases, which does not establish normal VSTest discovery or execution. Recheck the environment during implementation rather than assuming either runner outcome. No implementation tests were written or run for this documentation change.

## Out of Scope

- Implementation code, test code, implementation tickets, commits, pushes, or merges in this phase.
- Adding, dropping, swapping, equipping, selling, purchasing, crafting, or using items through Inventory.
- Inventory access outside the main direction menu, combat inspection, or changes to retreat/navigation rules.
- New inventory frameworks, item abstractions, equipment slots, storage, save/load systems, or duplicate player state.
- Changes to starting potions, healing, maximum health, merchant gifts, weapon damage rules, armor effects, occupations, or combat balance.
- New languages, localization replacement, unrelated refactoring, and general end-of-input handling.

## Further Notes

- Followed repository AGENTS guidance, the local to-spec template, issue-tracker conventions, and the existing Inventory glossary entry. No glossary changes were made for this spec.
- The required `/handoff` exercise was completed using a temporary handoff document and a fresh Codex session. The checkpoint was `/var/folders/5f/1h66h1vs7mg_gx9g1rv3pgh00000gn/T/cyoa-category-b-inventory-handoff.md` (the OS temporary directory, not a repository artifact). The fresh session read AGENTS guidance, the checkpoint, the glossary, and referenced project files; confirmed the approved read-only scope; identified availability as the next unresolved decision; and waited for approval. The user subsequently resumed and completed the interview. This specification captures those later decisions; the checkpoint's pause and open questions describe an earlier state. Temporary-file persistence is not required for implementation.
- Decision traceability: the approved scope establishes read-only display and reuse of existing state; Q2 establishes direction-menu-only availability and no movement/state changes; Q3 establishes normalized `i` input and automatic return; Q4 establishes numeric zeros and localized labels; Q5 establishes current weapon name/damage including merchant upgrades, without an enchanted label; Q6 establishes exactly four labeled values, without artwork, character stats, or additional information.
- The user approved the Adventure console testing interface and both tickets, then authorized and completed implementation. No essential gameplay questions or approval checks remain. Status retains the ready-for-agent specification convention; completed acceptance criteria and verification evidence are recorded in the tickets.
- Category B is the user's assignment classification. The exact instructor rubric was not supplied; this document does not claim independently verified rubric compliance.
