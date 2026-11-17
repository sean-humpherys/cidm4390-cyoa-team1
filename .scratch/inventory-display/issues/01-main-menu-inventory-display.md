# 01: Main-menu Inventory display in English, Spanish, and French

**What to build:** Players can inspect their current inventory and immediately continue choosing a direction.

**Blocked by:** None (can start immediately).

**Status:** ready-for-agent

## Acceptance criteria

- [x] Main-menu guidance offers `i`; accept `i`, `I`, and surrounding spaces.
- [x] Display exactly four labeled values: weapon name, current maximum damage, numeric armor protection, and healing potions remaining.
- [x] Display zero armor protection and zero potions explicitly.
- [x] Localize labels, command guidance, invalid-choice guidance, and all four supported weapon names in English, Spanish, and French using the existing dictionaries.
- [x] Return automatically to the direction menu without confirmation or an extra input.
- [x] Repeated inspection preserves all player values and player/weapon identities, consumes no randomness, and causes no movement or combat.
- [x] Include no heading, artwork, character stats, enchanted label, or item management.
- [x] Reuse existing player and equipment state; introduce no inventory framework.

## TDD requirements

Through the existing Adventure console interface, complete one failing behavior test → minimal implementation → passing checks → review/refactor at a time. Cover default values, normalization, repeated inspection, zero counts, nondefault values, localization, and invalid input. Covers spec AC1–AC3, AC6–AC10, and AC13.

Console tests run nonparallel, restore console streams, and assert observable output and public state.

## Authorization

The user approved this ticket's acceptance criteria, dependencies, localization, and TDD requirements exactly as proposed. Ticket publication is authorized; implementation code and tests are not authorized in this phase.

## Comments

### Ticket 01 implementation and verification

The user's later instruction authorized implementation of Ticket 01 only, superseding the earlier publication-only authorization above. Ticket 02 was not started. No commit or push was made. Existing glossary and specification edits were preserved.

Implemented a main-direction-menu `i` branch and four localized value lines using the existing Adventure, Player, Weapon, Messages, and weapon translation behavior. Existing input normalization handles uppercase and surrounding spaces. The menu loop returns automatically; no new state, framework, or test interface was introduced. Merchant, healing potion, combat, and movement handlers were not changed.

TDD evidence: observed the English four-value test fail against the old invalid-command output, then pass after adding the display. Observed the English guidance test fail, then pass after updating menu/error wording. Added Spanish, observed a missing-message failure, added its labels/guidance, and observed green. Repeated that red-green sequence for French. Added checks for inherited normalization, all supported weapon names, repeated read-only inspection, current nondefault values, zero potions, and invalid/blank input; these passed immediately because existing behavior and the preceding implementation already satisfied them. No artificial failures were introduced, and strict red-before-green for every individual regression assertion is not claimed. Review found no necessary production refactor.

Focused fallback command: `zsh /private/tmp/healing02-run.sh InventoryDisplayTests` — 22 passed, 0 failed. Message regression command: `zsh /private/tmp/healing02-run.sh MessagesTests` — 13 passed, 0 failed. Full fallback command: `zsh /private/tmp/healing02-run.sh` — 175 passed, 0 failed. The existing temporary runner builds the projects, loads real dictionaries, invokes MSTest methods/data rows sequentially, and applies per-test initialization/cleanup. It is a fallback, not a substitute for standard test discovery/execution guarantees. Full output was saved temporarily to `/private/tmp/inventory01-full.txt`.

The first full fallback run exposed three existing message-count expectations (69); updated counts and completeness ranges to 73 for the four new labels. All cases then passed. Builds report no errors; the existing LanguageTest MSTEST0037 warning remains on recompilation. `git diff --check` passed.

Both focused and full standard `dotnet test CYOA.Tests --no-restore --nologo -p:UseSharedCompilation=false` attempts (focused with `--filter FullyQualifiedName~AdventureTests`) built successfully but aborted before execution: VSTest's communication socket could not bind, `System.Net.Sockets.SocketException (13): Permission denied`. Normal VSTest execution remains an environment verification blocker. No permission escalation, commit, push, or Ticket 02 work was attempted.

Spec review: no missing, incorrect, or extra Ticket 01 behavior found. Its initial AC9 coverage suggestion was addressed with all-language assertions for two retries, preserved player state, and weapon identity; follow-up review confirmed no remaining findings. No production refactor was needed.

Standards review: no actionable findings in Adventure, the three dictionaries, message completeness tests, or the new console tests. Existing dispatch/localization conventions and weapon translation were reused; no refactor was warranted. Final review totals: Standards 0 findings; Spec 0 remaining findings.
