using System;
using CYOA;
using cidm4390_cyoa_team1;

namespace CYOA;

public class Adventure
{
    private readonly Messages _messages;
    private readonly Die _die;

    public Adventure(Messages messages)
    {
        _messages = messages;
        _die = new Die();
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
                    Console.WriteLine(_messages.GetMessage(8));
                    finished = true;
                    break;

                case "e":
                    Console.WriteLine(_messages.GetMessage(4));
                    finished = true;
                    break;

                default:
                    Console.WriteLine(
                        "Invalid choice. Enter 'n', 's', or 'e'.");
                    break;
            }
        }
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
        Console.WriteLine(dragon.DisplayStats());

        while (true)
        {
            Console.WriteLine(_messages.GetMessage(12));

            string? choice =
                Console.ReadLine()?.Trim().ToLowerInvariant();

            switch (choice)
            {
                case "r":
                    Console.WriteLine(_messages.GetMessage(26));
                    Console.WriteLine(_messages.GetMessage(41));
                    return true;

                case "a":
                    if (Fight(player, dragon))
                        return true;
                    break;

                default:
                    Console.WriteLine(
                        _messages.GetMessage(25));
                    break;
            }
        }
    }

    private bool Fight(Player player, Dragon dragon)
    {
        Console.WriteLine(
            _messages.FormatMessage(
                20,
                new System.Collections.Generic.Dictionary<string, string>
                {
                    ["occupation"] = player.occupation.ToString(),
                    ["weapon"] = player.weapon.type
                }));

        int attackRoll = _die.Roll(20);

        if (attackRoll <= player.strength)
        {
            Console.WriteLine(
                _messages.FormatMessage(
                    21, "dragonName", dragon.name));

            int defenseRoll = _die.Roll(20);

            if (defenseRoll <= dragon.agility)
            {
                Console.WriteLine(
                    _messages.FormatMessage(
                        22, "dragonName", dragon.name));
                Console.WriteLine(dragon.MissedMeTaunt());
            }
            else
            {
                int damage = _die.Roll(player.weapon.maxDamage);
                dragon.health -= damage;

                Console.WriteLine(
                    _messages.FormatMessage(
                        23,
                        new System.Collections.Generic.Dictionary<string, string>
                        {
                            ["damage"] = damage.ToString(),
                            ["healthPoints"] =
                                Math.Max(0, dragon.health).ToString()
                        }));

                if (dragon.health <= 0)
                {
                    Console.WriteLine(_messages.GetMessage(24));
                    Console.WriteLine(_messages.GetMessage(42));
                    return true;
                }
            }
        }
        else
        {
            Console.WriteLine(
                _messages.FormatMessage(
                    35, "dragonName", dragon.name));
            Console.WriteLine(player.MissedMeTaunt());
        }

        if (dragon.health > 0)
        {
            int dragonAttackRoll = _die.Roll(20);

            if (dragonAttackRoll <= dragon.strength)
            {
                Console.WriteLine(
                    _messages.FormatMessage(
                        36, "dragonName", dragon.name));

                int defenseRoll = _die.Roll(20);

                if (defenseRoll <= player.agility)
                {
                    Console.WriteLine(_messages.GetMessage(38));
                    Console.WriteLine(player.WasHitTaunt());
                }
                else
                {
                    int damage = _die.Roll(dragon.weapon.maxDamage);
                    player.health -= damage;

                    Console.WriteLine(
                        _messages.FormatMessage(
                            39,
                            new System.Collections.Generic.Dictionary<string, string>
                            {
                                ["damage"] = damage.ToString(),
                                ["healthPoints"] =
                                    Math.Max(0, player.health).ToString()
                            }));

                    if (player.health <= 0)
                    {
                        Console.WriteLine(_messages.GetMessage(40));
                        Console.WriteLine(_messages.GetMessage(43));
                        return true;
                    }
                }
            }
            else
            {
                Console.WriteLine(_messages.GetMessage(37));
                Console.WriteLine(dragon.MissedMeTaunt());
            }
        }

        return false;
    }
}