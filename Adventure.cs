using System;
using CYOA;
using cidm4390_cyoa_team1;

namespace CYOA;

public class Adventure
{
    private readonly Messages _messages;

    public Adventure(Messages messages)
    {
        _messages = messages;
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
                    Console.WriteLine(_messages.GetMessage(57));
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
        Console.WriteLine(dragon.DisplayStats(_messages.CurrentLanguage));

        while (true)
        {
            Console.WriteLine(_messages.GetMessage(12));

            string? choice =
                Console.ReadLine()?.Trim().ToLowerInvariant();

            switch (choice)
            {
                case "r":
                    Console.WriteLine(_messages.GetMessage(41));
                    return true;

                case "a":
                    Combat combat = new Combat(player, dragon, _messages);
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

    //Translate Races
    private string TranslateRace(Race race)
    {
        return _messages.CurrentLanguage switch
        {
            Language.Spanish => race switch
            {
                Race.Human => "Humano",
                Race.Elf => "Elfo",
                Race.Dwarf => "Enano",
                Race.Halfling => "Mediano",
                _ => race.ToString()
            },
            Language.French => race switch
            {
                Race.Human => "Humain",
                Race.Elf => "Elfe",
                Race.Dwarf => "Nain",
                Race.Halfling => "Halfelin",
                _ => race.ToString()
            },
            _ => race.ToString()
        };
    }
}