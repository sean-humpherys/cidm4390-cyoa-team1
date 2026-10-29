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
}