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
                    ["occupation"] = TranslateOccupation(player.occupation),
                    ["weapon"] = TranslateWeapon(player.weapon.type)
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
            Console.WriteLine(_messages.GetMessage(48));
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
                Console.WriteLine(_messages.GetMessage(51));
            }
        }

        return false;
    }

    private string TranslateOccupation(Occupation occupation)
    {
        return _messages.CurrentLanguage switch
        {
            Language.Spanish => occupation switch
            {
                Occupation.Fighter => "Guerrero",
                Occupation.Magician => "Mago",
                Occupation.Thief => "Ladrón",
                Occupation.Archer => "Arquero",
                _ => occupation.ToString()
            },
            Language.French => occupation switch
            {
                Occupation.Fighter => "Guerrier",
                Occupation.Magician => "Magicien",
                Occupation.Thief => "Voleur",
                Occupation.Archer => "Archer",
                _ => occupation.ToString()
            },
            _ => occupation.ToString()
        };
    }

    private string TranslateWeapon(string weapon)
    {
        return _messages.CurrentLanguage switch
        {
            Language.Spanish => weapon switch
            {
                "long sword" => "espada larga",
                "lightning bolt spell" => "hechizo de rayo",
                "dagger" => "daga",
                "long bow" => "arco largo",
                _ => weapon
            },
            Language.French => weapon switch
            {
                "long sword" => "épée longue",
                "lightning bolt spell" => "sort de foudre",
                "dagger" => "dague",
                "long bow" => "arc long",
                _ => weapon
            },
            _ => weapon
        };
    }
}