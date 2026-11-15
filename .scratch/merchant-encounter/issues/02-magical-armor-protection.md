# 02: Choose magical armor and reduce incoming combat damage

Status: ready-for-agent

**What to build:** A player can choose magical armor instead of a weapon enchantment, then travel north and receive its protection against the dragon. This completes the merchant's mutually exclusive upgrade choice.

**Blocked by:** 01 — Meet the merchant and enchant the current weapon.

## Acceptance criteria

- [ ] The complete localized menu offers 1 = magical armor, 2 = magical weapon, and 3 = leave and explains both effects and the one-free-upgrade limit.
- [ ] Player owns armor protection initialized to zero on every existing creation path. Choosing armor sets protection to 2, confirms the gift, records the claim, and leaves the weapon unchanged.
- [ ] Armor and weapon claims share the same gift limit. Neither repeat visits nor attempted alternative choices grant a second upgrade, swap upgrades, or stack bonuses.
- [ ] Leaving preserves access to either option on a later visit; armor selection returns to the direction menu with the same player.
- [ ] On a successful, undefended enemy hit, Combat applies rolled damage minus protection, clamped to zero, and displays the actual applied damage and remaining health.
- [ ] Rolled damage of 7, 2, and 1 causes health losses of 5, 0, and 0 with armor. Armor never heals or changes health capacity, agility, or dodge probability.
- [ ] Unarmored damage, misses, successful dodges, and existing defeat behavior remain correct.
- [ ] Armor instructions and confirmations appear in English, Spanish, and French through the existing message system.

## Testing requirements

- [ ] Verify zero protection and unclaimed eligibility for newly created players, including all existing constructors.
- [ ] Test armor selection, unchanged weapon, claim state, return navigation, and both directions of attempted cross-upgrade claims using observable encounter behavior.
- [ ] Verify a south-to-north journey retains armor when entering dragon combat.
- [ ] Use the controlled-roll seam from ticket 01 through public Combat sequences to test damage below, equal to, and above 2; assert exact health loss and displayed applied damage.
- [ ] Verify no healing, unarmored damage, misses, successful dodges, and lethal damage behavior with deterministic rolls where needed.
- [ ] Verify the final menu and armor messages in all three actual language dictionaries and run the full automated suite.
- [ ] Manually demonstrate armor selection, repeat-visit refusal, and dragon combat.

## Scope and traceability

Completes PRD AC1–AC5 alongside ticket 01, particularly AC3 and mutual exclusion in AC2. Reuse the Merchant, Player, and Combat responsibilities already agreed. No armor hierarchy, inventory, unequipping, or additional testing seams.
