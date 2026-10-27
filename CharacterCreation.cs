
using System;
using CYOA;

namespace cidm4390_cyoa_team1;

public class CharacterCreation
{
    private readonly Messages _messages;
    private readonly Func<int, int> _roll;

    
    // Uses the existing Die class for normal gameplay.
    public CharacterCreation(Messages messages, Die die)
        : this(messages, die.Roll)
    {
    }

    // Allows predetermined dice rolls for testing.
    public CharacterCreation(Messages messages, Func<int, int> roll)
    {
        _messages = messages;
        _roll = roll;
    }

    public Player CreateCharacter()
    {
        Race race = SelectRace();
        string name = EnterName();
        Occupation occupation = SelectOccupation();

        Player player = new Player(name, race, occupation);

        player.weapon = CreateWeapon(occupation);

        int strengthModifier = RollStrength(player);
        RollAgility(player);
        RollHealth(player, strengthModifier);

        ShowCharacterMenu(player);

        return player;
    }

    private Race SelectRace()
    {
        while (true)
        {
            Console.WriteLine(_messages.GetMessage(13));
            string? input = Console.ReadLine()?.Trim();

            if (Enum.TryParse(input, true, out Race race) &&
                Enum.IsDefined(race))
            {
                return race;
            }

            Console.WriteLine("Invalid race. Please try again.");
        }
    }

    private string EnterName()
    {
        while (true)
        {
            Console.WriteLine(_messages.GetMessage(14));
            string? name = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            Console.WriteLine("Character name cannot be empty.");
        }
    }

    private Occupation SelectOccupation()
    {
        while (true)
        {
            Console.WriteLine(_messages.GetMessage(15));
            string? input = Console.ReadLine()?.Trim();

            if (Enum.TryParse(input, true, out Occupation occupation) &&
                Enum.IsDefined(occupation))
            {
                return occupation;
            }

            Console.WriteLine("Invalid occupation. Please try again.");
        }
    }

    private static Weapon CreateWeapon(Occupation occupation)
    {
        return occupation switch
        {
            Occupation.Fighter =>
                new Weapon("long sword", 12, "-)=====>"),

            Occupation.Magician =>
                new Weapon("lightning bolt spell", 12, "zap~~~~~~"),

            Occupation.Thief =>
                new Weapon("dagger", 6, "-)==>"),

            Occupation.Archer =>
                new Weapon("long bow", 8, "} -->"),

            _ => throw new ArgumentOutOfRangeException(
                nameof(occupation))
        };
    }

    private int WaitForRoll(int messageId, int sides)
    {
        while (true)
        {
            Console.WriteLine(_messages.GetMessage(messageId));

            string? input = Console.ReadLine()?.Trim();

            if (string.Equals(
                input, "roll", StringComparison.OrdinalIgnoreCase))
            {
                return _roll(sides);
            }

            Console.WriteLine("Please type 'roll' to continue.");
        }
    }

    private int RollStrength(Player player)
    {
        int baseStrength = WaitForRoll(16, 20);
        int modifier = _roll(4);

        if (player.race == Race.Halfling)
        {
            modifier = -modifier;
        }

        player.strength = baseStrength + modifier;

        return modifier;
    }

    private void RollAgility(Player player)
    {
        int baseAgility = WaitForRoll(17, 20);

        if (player.race == Race.Elf ||
            player.race == Race.Halfling)
        {
            baseAgility += _roll(4);
        }

        player.agility = baseAgility;
    }

    private void RollHealth(Player player, int strengthModifier)
    {
        int baseHealth = WaitForRoll(18, 20);

        if (player.race != Race.Halfling)
        {
            baseHealth += strengthModifier;
        }

        player.health = baseHealth;
    }

    private void ShowCharacterMenu(Player player)
    {
        while (true)
        {
            Console.WriteLine(_messages.GetMessage(19));
            string? choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    DisplayCharacterStats(player);
                    break;

                case "2":
                    return;

                default:
                    Console.WriteLine(
                        "Invalid choice. Please enter 1 or 2.");
                    break;
            }
        }
    }

    public void DisplayCharacterStats(Player player)
    {
        Console.WriteLine(
            $"{_messages.GetMessage(27)}: {player.name}");

        Console.WriteLine(
            $"{_messages.GetMessage(28)}: {player.race}");

        Console.WriteLine(
            $"{_messages.GetMessage(29)}: {player.occupation}");

        Console.WriteLine(
            $"{_messages.GetMessage(30)}: {player.strength}");

        Console.WriteLine(
            $"{_messages.GetMessage(31)}: {player.agility}");

        Console.WriteLine(
            $"{_messages.GetMessage(32)}: {player.health}");

        Console.WriteLine(
            $"{_messages.GetMessage(33)}: {player.weapon.type}");

        Console.WriteLine(
            $"{_messages.GetMessage(34)}: {player.weapon.maxDamage}");
    }
}