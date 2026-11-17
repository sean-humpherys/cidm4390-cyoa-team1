using System;
using CYOA;
using cidm4390_cyoa_team1;

namespace CYOA;

public class Adventure
{
    private readonly Messages _messages;
    private readonly Func<int, int> _roll;

    public Adventure(Messages messages)
        : this(messages, new Die().Roll)
    {
    }

    // Allows controlled combat rolls while ordinary gameplay keeps using Die.
    public Adventure(Messages messages, Func<int, int> roll)
    {
        _messages = messages;
        _roll = roll ?? throw new ArgumentNullException(nameof(roll));
    }

    public void StartAdventure(Player player)
    {
        bool finished = false;

        while (!finished)
        {
            Console.WriteLine();
            Console.WriteLine(_messages.GetMessage(7));

            string? choice = Console.ReadLine()?.Trim().ToLowerInvariant();

            switch (choice)
            {
                case "n":
                    finished = NorthPath(player);
                    break;

                case "s":
                    new Merchant(_messages).Encounter(player);
                    break;

                case "i":
                    DisplayInventory(player);
                    break;

                case "e":
                    Console.WriteLine(_messages.GetMessage(4));
                    finished = true;
                    break;

                default:
                    Console.WriteLine(_messages.GetMessage(57));
                    break;
            }
        }
    }

    private void DisplayInventory(Player player)
    {
        Console.WriteLine(_messages.FormatMessage(70, "weapon", _messages.TranslateWeapon(player.weapon.type)));
        Console.WriteLine(_messages.FormatMessage(71, "damage", player.weapon.maxDamage.ToString()));
        Console.WriteLine(_messages.FormatMessage(72, "protection", player.armorProtection.ToString()));
        Console.WriteLine(_messages.FormatMessage(73, "potions", player.healingPotions.ToString()));
    }

    private bool NorthPath(Player player)
    {
        Dragon dragon = new Dragon("Dragon");

        Console.WriteLine(_messages.GetMessage(9));

        ConsoleColor originalColor = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                _messages.FormatMessage(
                    10, "dragonName", dragon.name));
        }
        finally
        {
            Console.ForegroundColor = originalColor;
        }

        Console.WriteLine(_messages.GetMessage(11));
        Console.WriteLine(dragon.DisplayStats(_messages.CurrentLanguage));

        while (true)
        {
            if (player.health <= 0)
            {
                Console.WriteLine(_messages.GetMessage(43));
                return true;
            }

            Console.WriteLine(_messages.FormatMessage(
                66,
                new Dictionary<string, string>
                {
                    { "health", player.health.ToString() },
                    { "maxHealth", player.maxHealth.ToString() },
                    { "potions", player.healingPotions.ToString() }
                }));
            Console.WriteLine(_messages.GetMessage(12));

            string? choice =
                Console.ReadLine()?.Trim().ToLowerInvariant();

            switch (choice)
            {
                case "r":
                    Console.WriteLine(_messages.GetMessage(41));
                    return true;

                case "p":
                    if (player.healingPotions <= 0)
                    {
                        Console.WriteLine(_messages.GetMessage(69));
                        break;
                    }
                    if (player.health >= player.maxHealth)
                    {
                        Console.WriteLine(_messages.GetMessage(68));
                        break;
                    }
                    int restored = Math.Min(10, player.maxHealth - player.health);
                    player.health += restored;
                    player.healingPotions--;
                    Console.WriteLine(_messages.FormatMessage(
                        67,
                        new Dictionary<string, string>
                        {
                            { "restored", restored.ToString() },
                            { "health", player.health.ToString() },
                            { "maxHealth", player.maxHealth.ToString() },
                            { "potions", player.healingPotions.ToString() }
                        }));
                    new Combat(player, dragon, _messages, _roll).DragonAttacksPlayerSequence();
                    break;

                case "a":
                    Combat combat = new Combat(player, dragon, _messages, _roll);
                    if (Fight(player, dragon, combat))
                        return true;
                    break;

                default:
                    Console.WriteLine(
                        _messages.GetMessage(25));
                    break;
            }
        }
    }

    private bool Fight(Player player, Dragon dragon, Combat combat)
    {
        combat.PlayerAttacksDragonSequence();

        if (dragon.health <= 0)
        {
            Console.WriteLine(_messages.GetMessage(42));
            return true;
        }

        combat.DragonAttacksPlayerSequence();

        if (player.health <= 0)
        {
            Console.WriteLine(_messages.GetMessage(43));
            return true;
        }
        return false;
       
    }
}
