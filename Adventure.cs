using System;
using System.IO;
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
                    ShowEnding(_messages.GetMessage(41));
                    finished = true;
                    break;

                case "e":
                    ShowEnding("You leave the adventure. The End.");
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

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(_messages.GetMessage(10));
        Console.ForegroundColor = originalColor;

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
                    ShowEnding(_messages.GetMessage(41));
                    return true;

                case "a":
                    if (Fight(player, dragon))
                    {
                        return true;
                    }

                    break;

                default:
                    Console.WriteLine(
                        "Invalid choice. Enter 'r' or 'a'.");
                    break;
            }
        }
    }
    private bool Fight(Player player, Dragon dragon)
    {
        Console.WriteLine(_messages.GetMessage(20));

        int attackRoll = _die.Roll(20);

        if (attackRoll <= player.strength)
        {
            Console.WriteLine(_messages.GetMessage(21));

            int defenseRoll = _die.Roll(20);

            if (defenseRoll <= dragon.agility)
            {
                Console.WriteLine(_messages.GetMessage(22));
                Console.WriteLine(dragon.MissedMeTaunt());
            }
            else
            {
                int damage = _die.Roll(player.weapon.maxDamage);
                dragon.health -= damage;

                Console.WriteLine(_messages.GetMessage(23));

                if (dragon.health <= 0)
                {
                    ShowEnding(_messages.GetMessage(24));
                    return true;
                }
            }
        }
        else
        {
            Console.WriteLine(_messages.GetMessage(35));
            Console.WriteLine(player.MissedMeTaunt());
        }

        if (dragon.health > 0)
        {
            int dragonAttackRoll = _die.Roll(20);

            if (dragonAttackRoll <= dragon.strength)
            {
                Console.WriteLine(_messages.GetMessage(36));

                int defenseRoll = _die.Roll(20);

                if (defenseRoll <= player.agility)
                {
                    Console.WriteLine(_messages.GetMessage(38));
                    Console.WriteLine(player.MissedMeTaunt());
                }
                else
                {
                    int damage = _die.Roll(dragon.weapon.maxDamage);
                    player.health -= damage;

                    Console.WriteLine(_messages.GetMessage(39));

                    if (player.health <= 0)
                    {
                        ShowEnding(_messages.GetMessage(40));
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

    private static void ShowEnding(string ending)
    {
        Console.WriteLine(ending);
    }
}