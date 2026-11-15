# 01: Meet the merchant and enchant the current weapon

Status: ready-for-agent

**What to build:** A player can travel south, accept one free magical weapon upgrade or leave, return to the direction menu, and take the enchanted weapon north to fight the dragon. This is the first complete playable slice; the armor choice arrives in ticket 02 rather than appearing as a nonworking menu option.

**Blocked by:** None (can start immediately).

## Acceptance criteria

- [ ] South travel opens a Merchant encounter and returns to the direction menu using the same player rather than ending the adventure.
- [ ] Offer 2 = magical weapon and 3 = leave, explaining the free gift and the +4 maximum damage effect in the selected language. Reserve 1 for the armor slice; do not advertise an unavailable upgrade.
- [ ] Choosing the weapon increases the current weapon's maximum damage by exactly 4, preserves its type and appearance, and confirms the choice.
- [ ] Support every occupation, including the magician's spell, without changing strength, hit probability, or minimum damage.
- [ ] Player owns the gift-claimed state, initialized as unclaimed on every existing creation path. A new game starts with a new eligible player.
- [ ] A claim consumes the one gift per player per game. Revisits explain that the gift was received and return without changing the weapon again.
- [ ] Leaving preserves eligibility and equipment; the player can revisit and claim later.
- [ ] Invalid and blank input retry with a localized message without changing state. End-of-input at the merchant acts as leaving.
- [ ] All dialogue introduced in this slice uses the existing English, Spanish, and French message dictionaries.
- [ ] Combat accepts controlled dice rolls following the existing CharacterCreation pattern while existing callers retain ordinary Die behavior. Introduce this narrow seam within this slice, not as a separate refactoring project.

## Testing requirements

- [ ] Add automated encounter tests for claiming, leaving then returning, repeat claims, invalid/blank input followed by a valid choice, and merchant end-of-input.
- [ ] Cover weapon upgrades for every occupation, asserting +4 maximum damage and preserved identity and appearance.
- [ ] Verify a south-to-north journey retains the enchanted weapon and enters dragon combat; supply complete console input.
- [ ] Through public combat behavior, use controlled rolls to verify the increased roll maximum and resulting dragon health loss. Verify existing default construction and existing combat tests still work.
- [ ] Check the introduced messages against actual dictionaries in all three languages.
- [ ] Update old south-path ending expectations and run the full automated suite.
- [ ] Manually demonstrate south, weapon selection, return, repeat visit, and north travel.

## Scope and traceability

Covers PRD AC1, weapon-related AC2, AC4, and the corresponding AC5 behavior. Armor is intentionally completed in ticket 02. Test observable output and player/dragon state; restore console streams and use nonparallel console tests as in the existing suite. Do not add currency, inventory, new weapon types, or unrelated refactoring.
