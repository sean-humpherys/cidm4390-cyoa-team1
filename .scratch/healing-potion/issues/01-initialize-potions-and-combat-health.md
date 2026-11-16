# 01: Initialize healing potions and display combat health

Status: ready-for-agent

**What to build:** A newly created character enters dragon combat with one potion and accurate current health, maximum health, and potion supply displayed.

**Blocked by:** None (can start immediately).

## Acceptance criteria

- [ ] Every player creation path starts with exactly one potion.
- [ ] Maximum health equals final starting health, including race bonuses, and remains fixed after damage.
- [ ] Every combat action menu displays current health, maximum health, and remaining potions.
- [ ] Labels use the actual English, Spanish, and French dictionaries.
- [ ] Existing attack, retreat, and merchant behavior remains functional. Do not advertise the potion command until Ticket 02 makes it usable.

## Test requirements

- [ ] Cover both player constructors and CharacterCreation, including existing races and occupations without a redundant Cartesian product.
- [ ] Verify independent state for newly created players.
- [ ] Through Adventure's console interface, verify initial and repeated status displays in all three languages.
- [ ] Verify reducing current health does not reduce maximum health.

## TDD requirements

Follow one failing test → observe failure → minimal implementation → passing test → refactor, repeating incrementally. Write one failing test at a time at the approved public boundaries. Do not batch all tests before implementing behavior.

## Traceability

PRD AC1, AC2, and status-related AC7/AC10.

## Comments

2026-11-16: Implemented Ticket 01 only. Fresh players have one healing potion; constructor and race-adjusted character-creation health establish fixed maximum health. Every dragon action menu displays localized current/maximum health and potion supply. Attack/retreat prompts remain unchanged; no potion command or drinking behavior was added.

TDD evidence: six successive red/green cycles covered potion defaults, constructor maximum health, race-adjusted creation, and English/Spanish/French status. The first two reds were missing-member compilation failures. The race-adjusted creation cycle used one parameterized test method with four failing cases before its shared fix; localization cases were introduced one language at a time. Repeated-menu regression assertions passed against the already-correct loop without additional production changes. A final refactor routed constructor starting health through the same initialization operation, removing the separate maximum-health constant.

Validation: focused initialization/menu tests 12 passed, 0 failed; full sequential fallback suite 122 passed, 0 failed. Build succeeded with the pre-existing MSTEST0037 warning in LanguageTest. Standard command `dotnet test CYOA.Tests --no-restore --nologo -p:UseSharedCompilation=false` built successfully but aborted before tests with `SocketException (13): Permission denied` when VSTest attempted its local socket. The temporary in-process runner executed compiled MSTest methods and DataRow assertions sequentially; it does not verify standard test discovery, adapter behavior, or normal parallel execution. Standard-runner verification remains pending outside the restricted sandbox.

Evidence is under `/private/tmp/healing01-01-red.txt` through `healing01-06-green.txt`, `healing01-refactor.txt`, `healing01-full.txt`, and `healing01-standard.txt`; temporary runner files are outside the repository. Spec review found no functional violations or scope creep; standards review's duplicate initialization concern was resolved. No changes were made to Tickets 02 or 03, and nothing was committed or pushed.
