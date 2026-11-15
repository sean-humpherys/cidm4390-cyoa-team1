# 03: Verify complete merchant journeys and assignment demonstration

Status: ready-for-agent

**What to build:** Deliver a verified, demonstrable merchant encounter across both upgrades and all three languages. This is a small integration and release-check ticket, not a replacement for the automated tests in tickets 01 and 02.

**Blocked by:** 01 — Meet the merchant and enchant the current weapon; 02 — Choose magical armor and reduce incoming combat damage.

## Acceptance criteria

- [ ] Review the completed feature against every PRD acceptance criterion; add only missing integration coverage and fix any feature-specific defects discovered.
- [ ] Both upgrade journeys allow south, a claim, return to directions, and north with the same upgraded player.
- [ ] Leaving then returning retains eligibility; revisiting after either claim cannot grant, swap, or stack upgrades.
- [ ] All merchant introduction, numbered choices, effect explanations, confirmations, leaving, invalid-input, and already-received messages display correctly in English, Spanish, and French.
- [ ] The full existing automated test suite passes, with south-path expectations updated and unrelated behavior preserved.
- [ ] Record a short manual demonstration checklist and actual results, distinguishing completed checks from any checks still pending.
- [ ] No currency, inventory framework, persistence, new enemies, or unrelated behavior has entered the scope.

## Testing requirements

- [ ] Reuse existing encounter, journey, and deterministic combat tests. Add only missing observable integration assertions; do not duplicate tests merely to create work for this ticket.
- [ ] Exercise each merchant outcome returning to the direction menu with complete console input. Keep merchant end-of-input tests at the encounter boundary; general direction-menu end-of-input changes remain out of scope.
- [ ] Run the full automated suite and record its command and result without claiming unperformed checks.
- [ ] Manually visit the merchant in English, Spanish, and French, checking wording, option numbers, invalid input, confirmations, and return navigation.
- [ ] Manually leave, revisit, claim, and revisit again to confirm the gift limit.
- [ ] In separate games, take armor and the weapon upgrade north and verify combat effects and messages.

## Scope and traceability

Final evidence for PRD AC1–AC5 and its manual testing checklist. Both feature slices must already include their own tests. No new production feature or broad refactoring is planned here.

## Comments

2026-11-15: Completed the integration review, expanded the existing journeys to all three languages, and performed six scripted console acceptance runs plus controlled combat demonstrations. See [acceptance results](../acceptance-results.md) for the AC1–AC5 review, manual checklist, exact commands, documentation/Git readiness, and remaining limitations. The sequential fallback executed all 110 test cases successfully; the standard VSTest run remains pending outside the sandbox because local socket binding is denied. No production defects were found, and nothing was committed or pushed.
