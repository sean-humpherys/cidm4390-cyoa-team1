
using System;
using System.IO;
using System.Collections.Generic;
using CYOA;

namespace cidm4390_cyoa_team1
{
    public class Combat
    {
        public Player player { get; set; }
        public Dragon dragon { get; set; }

        private readonly Messages _messages;
        private readonly Func<int, int> _roll;

        // Constructor required by the UML diagram.
        public Combat(Player player, Dragon dragon)
            : this(player, dragon, CreateDefaultMessages())
        {
        }

        // Constructor for integration with the Game class.
        public Combat(Player player, Dragon dragon, Messages messages)
            : this(player, dragon, messages, new Die().Roll)
        {
        }

        public Combat(Player player, Dragon dragon, Messages messages, Func<int, int> roll)
        {
            _roll = roll ?? throw new ArgumentNullException(nameof(roll));
            this.player = player ??
                throw new ArgumentNullException(nameof(player));

            this.dragon = dragon ??
                throw new ArgumentNullException(nameof(dragon));

            _messages = messages ??
                throw new ArgumentNullException(nameof(messages));
        }

        // Load default English messages for standalone combat.
        private static Messages CreateDefaultMessages()
        {
            string filePath = Path.Combine(
                AppContext.BaseDirectory,
                "MessageFiles",
                "English.json"
            );

            Messages messages = new Messages();
            messages.ReadDictionary(filePath);

            return messages;
        }

        public void PlayerAttacksDragonSequence()
        {
            // Display the attack introduction in the selected language.
            Console.WriteLine(_messages.FormatMessage(
                20,
                new Dictionary<string, string>
                {
                    { "occupation", _messages.TranslateOccupation(player.occupation)},
                    { "weapon", _messages.TranslateWeapon(player.weapon.type)}
                }));

            Console.WriteLine(player.weapon.asciiArt);

            // Roll a 20-sided die to determine whether the player hits.
            int attackRoll = _roll(20);

            if (attackRoll > player.strength)
            {
                Console.WriteLine(_messages.FormatMessage(
                    35, "dragonName", dragon.name));
                return;
            }

            Console.WriteLine(_messages.FormatMessage(
                21, "dragonName", dragon.name));

            // The dragon rolls a 20-sided die to defend.
            int defenseRoll = _roll(20);

            if (defenseRoll <= dragon.agility)
            {
                Console.WriteLine(_messages.FormatMessage(
                    22, "dragonName", dragon.name));

                // Random dragon defense taunt (48-51).
                int tauntId = _roll(4) + 47;

                Console.WriteLine(_messages.FormatMessage(
                    tauntId, "race", _messages.TranslateRace(player.race)));
                return;
            }

            // The dragon failed to defend.
            int damage = _roll(player.weapon.maxDamage);
            dragon.health -= damage;

            Console.WriteLine(_messages.FormatMessage(
                23,
                new Dictionary<string, string>
                {
                    { "damage", damage.ToString() },
                    { "healthPoints", dragon.health.ToString() }
                }));

            if (dragon.health <= 0)
            {
                Console.WriteLine(_messages.GetMessage(24));
                return;
            }

            // Random dragon reply after surviving damage (44-47).
            int replyId = _roll(4) + 43;
            Console.WriteLine(_messages.GetMessage(replyId));
        }

        public void DragonAttacksPlayerSequence()
        {

            // Roll a 20-sided die for the dragon's attack.
            int attackRoll = _roll(20);

            if (attackRoll > dragon.strength)
            {
                Console.WriteLine(_messages.GetMessage(37));
                return;
            }

            Console.WriteLine(_messages.FormatMessage(
                36, "dragonName", dragon.name));

            // Roll a 20-sided die for the player's defense.
            int defenseRoll = _roll(20);

            if (defenseRoll <= player.agility)
            {
                Console.WriteLine(_messages.GetMessage(38));

                // Display the player's defense taunt
                // in the selected language (Message #58).
                Console.WriteLine(_messages.FormatMessage(
                    58, "playerName", player.name));
                return;
            }

            // The player failed to defend.
            int damage = Math.Max(0, _roll(dragon.weapon.maxDamage) - player.armorProtection);
            player.health -= damage;

            Console.WriteLine(_messages.FormatMessage(
                39,
                new Dictionary<string, string>
                {
                    { "damage", damage.ToString() },
                    { "healthPoints", player.health.ToString() }
                }));

            if (player.health <= 0)
            {
                Console.WriteLine(_messages.GetMessage(40));
                return;
            }

            // Random sarcastic dragon reply (44-47).
            int replyId = _roll(4) + 43;
            Console.WriteLine(_messages.GetMessage(replyId));
        }
    }
}