# 01: Consolidate weapon translations across character creation, Inventory, and combat

**What to build:** Messages owns the shared weapon-name translation implementation. Character creation, Adventure's Inventory, and Combat use it while preserving every existing displayed name and gameplay behavior.

**Blocked by:** None (can start immediately).

**Status:** ready-for-agent

## Acceptance criteria

- [x] Preserve all four supported weapon names exactly in English, Spanish, and French, including accents and capitalization, as recorded in the approved specification.
- [x] Preserve passthrough behavior for custom, empty, differently cased, whitespace-bearing, and null weapon inputs. Do not trim, normalize, validate, or add an enchanted label.
- [x] Use the current language on the supplied Messages instance. Changing that language affects subsequent translations; no numbered-message dictionary entries or file loading are required. Unsupported language values preserve the existing unchanged-string fallback.
- [x] Retain CharacterCreation's existing public weapon translator as a forwarding method with no independent mapping.
- [x] Migrate weapon-name rendering in CharacterCreation, Adventure's Inventory, and Combat to the shared Messages implementation. Remove Adventure's duplicate weapon mapping.
- [x] Verify unchanged character statistics, Inventory output, and combat weapon wording in all three languages. Inventory still displays exactly four localized values, returns automatically, and remains read-only and limited to the main direction menu.
- [x] Translation emits no output, consumes no randomness, and changes neither language/dictionary nor player/equipment state. Numbered messages, dictionaries, formatting, merchant upgrades, healing potions, movement, and combat behavior remain unchanged.
- [x] Review confirms Messages contains the sole weapon mapping. Preserve race/occupation behavior and the CharacterCreation dependency still needed by Combat for those terms until Ticket 02.
- [x] Run focused translation, compatibility, and caller tests plus the full available regression suite. Record actual commands, results, and any standard-runner or fallback limitations.

## Red-green-refactor TDD requirements

- Recheck the working tree and establish a fresh baseline before production edits; preserve the completed Inventory implementation and tests.
- Use the approved Messages public translation interface for exact strings, fallbacks, current-language changes, dictionary independence, and side-effect checks. Use CharacterCreation's existing public translator for compatibility and the existing CharacterCreation, Adventure console, and controlled-roll Combat interfaces for caller regression tests.
- Add one failing test for newly exposed shared translation behavior, observe its failure or missing-interface build failure, make the smallest change to pass, then review/refactor with passing checks before the next behavior. Work in vertical slices through the affected callers; do not write all tests or migrate everything first.
- Use literal expected translations from the approved specification. Do not compute expected output by calling another translator or copying a result from the implementation at runtime.
- Existing characterization/regression checks may pass immediately; record that honestly rather than introducing artificial failures. Preserve a green suite during behavior-preserving migration.
- Console tests run nonparallel, use finite scripted input, and restore console streams in a finally block. Use existing controlled rolls where needed. Verify mapping ownership by review, not source-text tests or assertions against private helpers.
- Recheck standard VSTest execution. If it remains unavailable, run the existing available fallback and distinguish its passing cases from standard discovery/execution guarantees. Do not assume the previous 199-case baseline or socket blocker still applies without checking.

## Scope and approval

The user approved the specification, compatibility delegates, testing interfaces, both ticket descriptions, and dependency order. This publication supersedes the specification's earlier pending-approval wording. Only ticket creation is currently authorized; do not implement code, write tests, commit, or push until separately instructed.

Race/occupation migration and final translation-only dependency cleanup belong to Ticket 02. New localization frameworks, gameplay changes, dictionary edits, and unrelated architecture refactoring are out of scope.

## Comments

### Ticket 01 implementation and verification

The user's later implementation instruction authorizes this ticket only, superseding the earlier publication-only restriction. Ticket 02 was not started. No commits or pushes were made.

Messages now owns the single weapon translation implementation. CharacterCreation statistics, Adventure Inventory, and Combat weapon introductions use it directly. CharacterCreation's public TranslateWeapon method remains a thin delegate. Adventure's duplicate weapon translator was removed. Race/occupation mappings, their compatibility behavior, and Combat's CharacterCreation/Die construction remain unchanged for Ticket 02.

All dictionaries, numbered messages, player state rules, merchant gifts, healing potions, and existing gameplay/test files were preserved. A start-of-task snapshot confirmed that the only existing production files changed by this ticket were Messages, CharacterCreation, Adventure, and Combat; the additional test class is WeaponTranslationTests. Earlier uncommitted Inventory changes were preserved.

### Red-green-refactor evidence

The baseline fallback run passed 199 cases. Each newly failing mapping case was added separately and executed before its implementation; the full focused class was rerun after every minimum change:

| Cycle | New behavior | Red observation | Green result |
| --- | --- | --- | --- |
| 01 | Spanish long sword → espada larga | CS1061: Messages has no TranslateWeapon method | 1 passed |
| 02 | Spanish lightning bolt spell → hechizo de rayo | One assertion failure; original English string returned | 2 passed |
| 03 | Spanish dagger → daga | One assertion failure; original English string returned | 3 passed |
| 04 | Spanish long bow → arco largo | One assertion failure; original English string returned | 4 passed |
| 05 | French long sword → épée longue | One assertion failure; original English string returned | 5 passed |
| 06 | French lightning bolt spell → sort de foudre | One assertion failure; original English string returned | 6 passed |
| 07 | French dagger → dague | One assertion failure; original English string returned | 7 passed |
| 08 | French long bow → arc long | One assertion failure; original English string returned | 8 passed |

Subsequent English, passthrough/null, unsupported-language, live-language, dictionary-independence, no-output, and caller characterization cases passed immediately. They are regression evidence, not additional claimed red phases. Expected values are literal approved translations, not values derived from another translator.

After the shared mappings were green, CharacterCreation's mapping was replaced by delegation and its statistics caller migrated; 32 focused cases passed before and after. Adventure was migrated and its duplicate removed; all 22 existing Inventory display cases passed. Combat's weapon caller was migrated after adding its output characterization; all 44 focused cases passed before and after. No unrelated refactor was needed. Compatibility and gameplay tests use the approved public interfaces with controlled rolls; console output is restored and tests run nonparallel.

### Commands and results

- Baseline: `zsh /private/tmp/healing02-run.sh` — 199 passed, 0 failed.
- Each mapping cycle: `zsh /private/tmp/healing02-run.sh WeaponTranslationTests` — observed the expected single new failure (or initial missing-method compile failure), then a passing focused run after implementation.
- Final focused translation/compatibility/caller suite: `zsh /private/tmp/healing02-run.sh WeaponTranslationTests` — 44 passed, 0 failed. This includes 12 exact known-name cases, 7 passthrough cases across all three languages and an unsupported language, 1 live-language/dictionary/no-output case, 12 statistics cases, and 12 combat-introduction cases.
- Inventory regression: `zsh /private/tmp/healing02-run.sh InventoryDisplayTests` — 22 passed, 0 failed after migration.
- Full regression: `zsh /private/tmp/healing02-run.sh` — 243 passed, 0 failed, including all pre-existing Messages, character creation, Adventure, Combat, merchant, healing potion, and Inventory cases.
- Standard runner: `dotnet test CYOA.Tests --no-restore --nologo -p:UseSharedCompilation=false` — builds succeeded; VSTest aborted before executing tests with `System.Net.Sockets.SocketException (13): Permission denied` while binding its communication socket. Standard discovery/execution remains an environment verification blocker.
- `git diff --check` passed. Incremental test builds showed only the existing LanguageTest MSTEST0037 warning, with no new compiler/analyzer warning from this ticket.

The existing temporary fallback runner builds the projects, copies real dictionaries, and invokes MSTest methods/data rows sequentially with per-test initialization/cleanup. Its results do not establish successful standard VSTest discovery or execution.

Temporary evidence is under `/private/tmp`: `weapon01-baseline.txt`, `weapon01-01-red.txt` through `weapon01-08-red.txt`, their corresponding `-green.txt` files, `weapon01-english.txt`, `weapon01-fallbacks.txt`, `weapon01-live-language.txt`, `weapon01-statistics-before.txt`, `weapon01-statistics-after.txt`, `weapon01-inventory-after.txt`, `weapon01-combat-before.txt`, `weapon01-combat-after.txt`, `weapon01-standard.txt`, and `weapon01-full.txt`. The task-only diff and original source snapshot are `weapon01-task.diff` and `weapon01-before.json`. These temporary artifacts may be removed by the system; this record preserves the results and limitations.

### Review

Standards: 0 actionable findings. Shared ownership, compatibility delegation, and test conventions match the repository; no additional abstraction was warranted.

Spec: 0 actionable findings. Exact translations, fallbacks, current-language behavior, compatibility, and all weapon callers are covered. Race/occupation migration and final dependency cleanup remain deferred to Ticket 02.
