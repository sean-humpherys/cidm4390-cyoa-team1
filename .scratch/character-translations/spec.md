# Consolidate Character Translations

Status: ready-for-agent
Approval: specification, compatibility delegates, testing interfaces, and both tickets approved by the user; implementation and verification are complete.

## Problem Statement

Players expect consistent English, Spanish, and French names throughout character creation, Inventory, and combat. Maintainers currently have to keep duplicate translation mappings synchronized. Combat constructs CharacterCreation and a Die solely to access translations, coupling combat presentation to character creation.

## Solution

Concentrate race, occupation, and weapon-name translations in the existing Messages module. CharacterCreation, Adventure's Inventory display, and Combat share that implementation. Preserve every existing translated string, capitalization, fallback, and gameplay behavior.

## User Stories

1. As an English-speaking player, I want existing character names and equipment terms preserved, so that the refactor does not change what I read.
2. As a Spanish-speaking player, I want the same accented translations throughout the game, so that presentation remains consistent.
3. As a French-speaking player, I want the same translations throughout the game, so that presentation remains consistent.
4. As a player, I want character statistics to retain their existing localized output, so that character creation behaves as before.
5. As a player, I want Inventory to retain its four labeled values, so that a translation refactor does not add or remove information.
6. As a player, I want combat introductions and race-based taunts to retain their existing wording, so that combat presentation remains unchanged.
7. As a player, I want a magical weapon upgrade to retain the same weapon name, so that increased maximum damage does not introduce a new label.
8. As a maintainer, I want one authoritative implementation for character-term translations, so that corrections do not require duplicated edits.
9. As a maintainer, I want Combat to obtain translations without constructing CharacterCreation, so that unrelated modules are not coupled.
10. As an existing caller, I want CharacterCreation's public translation methods to remain usable, so that internal reorganization does not break their interface.
11. As a maintainer, I want unknown values to retain their existing fallbacks, so that the refactor does not silently introduce normalization or validation.
12. As a maintainer, I want translation to read the currently selected language without mutating state or consuming randomness, so that localization remains a presentation concern.
13. As a maintainer, I want existing merchant, healing potion, combat, and Inventory regression tests to remain green, so that architecture work does not alter gameplay.

## Implementation Decisions

- Deepen the existing Messages module with the shared race, occupation, and weapon translation behavior. No separate localization framework, adapter hierarchy, or inventory framework is needed.
- Messages owns one implementation of each mapping and reads its current language when translating. Do not cache a language in callers or couple character-term translation to numbered-message dictionary contents.
- Move mappings without changing their values. Preserve English enum-name rendering for race/occupation and the original weapon string in English. Preserve exact Spanish/French strings in the contract below.
- CharacterCreation's display behavior and Adventure's Inventory rendering use the shared implementation. Retain CharacterCreation's existing public translation methods as thin delegates to Messages for compatibility, with no independent mappings.
- Combat uses Messages for occupation, weapon, and race translation. Remove its construction of CharacterCreation and the associated Die that exist only for translation; retain its existing gameplay randomness and controlled-roll support.
- Remove Adventure's duplicate weapon translation implementation and its unused private race/occupation translators after migration. Inventory remains Adventure behavior, not a new module.
- Preserve unknown weapon strings exactly, including casing and surrounding whitespace. Do not normalize, trim, translate partially matching names, or add an enchanted label. Preserve null passthrough if a caller supplies null to the existing weapon translator despite its nonnullable declaration; do not add new validation exceptions.
- Undefined race/occupation enum values retain their existing enum ToString result. An unsupported language enum value retains the existing default behavior: enum-name rendering for race/occupation and unchanged weapon strings.
- Translation itself emits no console output, consumes no rolls, loads no files, and changes neither Messages' language/dictionary nor player/equipment state. Existing dictionary loading and message formatting remain intact.
- Preserve the current uncommitted Inventory implementation and both completed ticket test suites. This is a behavior-preserving refactor, not a new gameplay feature.

### Exact translation contract

| Race | English | Spanish | French |
| --- | --- | --- | --- |
| Human | Human | Humano | Humain |
| Elf | Elf | Elfo | Elfe |
| Dwarf | Dwarf | Enano | Nain |
| Halfling | Halfling | Mediano | Halfelin |

| Occupation | English | Spanish | French |
| --- | --- | --- | --- |
| Fighter | Fighter | Guerrero | Guerrier |
| Magician | Magician | Mago | Magicien |
| Thief | Thief | Ladrón | Voleur |
| Archer | Archer | Arquero | Archer |

| Weapon input / English | Spanish | French |
| --- | --- | --- |
| long sword | espada larga | épée longue |
| lightning bolt spell | hechizo de rayo | sort de foudre |
| dagger | daga | dague |
| long bow | arco largo | arc long |

### Acceptance criteria

| ID | Required observable or structural outcome |
| --- | --- |
| AC1 | Shared Messages translation returns the exact 36 language/value combinations in the tables: 12 race, 12 occupation, and 12 weapon results. |
| AC2 | Custom, empty, differently cased, whitespace-bearing, and null weapon inputs retain their existing passthrough behavior. Undefined race/occupation values retain their enum-string fallback. Unsupported language values retain default behavior. |
| AC3 | Changing the language on the same Messages instance changes subsequent translations; callers do not retain stale translations. Translation works with an empty numbered-message dictionary as the current mappings do. |
| AC4 | CharacterCreation's existing public translation methods return the same results through delegation, including fallback behavior. Character statistics retain existing labels, values, and translations in all three languages. |
| AC5 | Inventory retains normalized main-menu-only access, exactly four localized values, the current weapon's unchanged name, automatic menu return, and read-only behavior in all languages. Merchant weapon upgrades alter damage but not naming. |
| AC6 | Combat attack introductions preserve localized occupations and weapons; race-specific dragon taunts preserve localized race names. Existing attack, defense, damage, taunt selection, and defeat behavior remains unchanged. |
| AC7 | Translation has no console, randomness, file-loading, language/dictionary mutation, or player/equipment mutation side effects. |
| AC8 | Code review confirms Messages contains the sole implementation for each character-term mapping. CharacterCreation delegates contain no copies; Adventure's duplicate/private unused translators are removed; Combat no longer constructs CharacterCreation or a translation-only Die. |
| AC9 | Dictionary contents, numbered message IDs, formatting, character creation rules, player defaults, merchant gifts, magical armor, healing potion behavior, and movement remain unchanged. |
| AC10 | Focused translation/caller tests and the full regression suite are run. Actual runner results and environment blockers are recorded without equating fallback execution with standard VSTest execution. |

## Testing Decisions

### Approved test seams

- Primary shared seam: Messages' public character-translation behavior. This extends an existing module's public interface and provides a direct way to test all exact strings, fallback cases, language changes, and absence of dictionary requirements. It does not prove gameplay callers migrated correctly; integration checks and review cover that.
- Retain the existing public CharacterCreation translation interface for compatibility checks and the existing CharacterCreation character/statistics, Adventure console, and controlled-roll Combat interfaces for regression coverage. These existing seams verify actual displayed terms and unchanged gameplay. No private-helper tests, mock translation collaborators, or additional production testing interface is proposed.
- The user explicitly approved these testing interfaces before implementation, including Messages character translation and the existing gameplay interfaces.

### Red-green-refactor requirements

- Before editing production code, recheck the working tree and establish the available runner baseline. The most recent reported fallback baseline is 199 passing cases; remeasure rather than assume it.
- For each newly exposed shared translation behavior, write one failing test with a literal expected result, observe the failure or missing-interface build failure, implement the minimum needed to pass, then review/refactor with passing checks before the next behavior. Do not bulk-write all tests or migrate every caller before the first green slice.
- Work vertically: translate a term through Messages, migrate its affected callers, verify their observable output, then continue. Keep mappings that still serve unmigrated callers until their slice is complete.
- Existing behavior characterization tests can pass immediately. Record that accurately; do not fabricate a failing phase or change existing translations to manufacture red. After a green characterization check, perform the behavior-preserving move and rerun it. Separate new-interface red-green evidence from regression evidence.
- Use literal expected translations from the approved tables, not expected values obtained by calling another translator or reading the implementation's mapping.
- Verify language changes on a single Messages instance, unknown enum values, all passthrough weapon cases, and dictionary independence through the public translation behavior. Keep compatibility delegation coverage focused on behavior, not method-call counts.
- For console tests, follow existing nonparallel test conventions, restore streams in a finally block, supply finite input, and use existing controlled rolls for deterministic combat. Preserve exact wording/capitalization while avoiding unrelated whitespace-sensitive full-screen snapshots.
- Verify the architectural requirements in AC8 by code review, not brittle source-text assertions. No tests against private methods or speculative adapter interfaces.

### Regression plan

- Prior art: existing Messages tests for language state/formatting, CharacterCreation console/statistics tests, multilingual Combat defense-taunt regression coverage, and both Inventory display/journey test classes.
- Run focused tests after every slice and builds regularly. At completion, run Messages, CharacterCreation, Adventure, Combat, merchant, healing potion, and both Inventory suites, then the full available suite.
- Attempt the standard runner and record any actual failure. Recent runs built successfully but VSTest aborted before execution because its communication socket could not bind (permission denied). The existing temporary sequential runner is an available fallback; report its commands, test counts, failures, and discovery/execution limitations separately. Do not claim either runner outcome until rerun during implementation.

## Out of Scope

- Production code or test implementation, implementation ticket publication, commits, or pushes during this approval phase.
- New translations, wording corrections, new languages, input aliases, case normalization, changed fallbacks, or dictionary/message-ID changes.
- Dragon statistics/claw-name translation reorganization, which is separate from these character-term mappings.
- Player constructor cleanup, healing potion extraction, combat redesign, console abstractions, randomness redesign, inheritance changes, new localization frameworks, and unrelated architecture work.
- New Inventory functionality, equipment management, gameplay changes, or modifications to the completed Inventory requirements.

## Further Notes

- The user approved consolidation into Messages after the architecture review and requested specification plus proposed tickets before implementation. The local to-spec convention assigns ready-for-agent to the spec; that label does not substitute for the requested approval or authorize implementation.
- The user approved compatibility delegation; the implemented delegates preserve CharacterCreation's public interface while deleting duplicate mapping knowledge.
- Approved and completed ticket order: 01 weapon translation through all existing callers; 02 race/occupation translation through their callers and final removal of the translation-only Combat dependency. Ticket 02 depends on the shared ownership and weapon migration established by 01. Each slice includes its own behavior tests and regression checks.
- No essential gameplay question or approval check remains. Both approved tickets are implemented, with acceptance criteria and verification evidence recorded in their completion comments. The user subsequently reported all 324 tests passing with standard dotnet test in the regular VS Code terminal.
