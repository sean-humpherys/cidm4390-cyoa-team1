
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
            string? input = Console.ReadLine()?.Trim().ToLowerInvariant();

            Race? race = input switch
            {
                "human" or "humano" or "humain" => Race.Human,
                "elf" or "elfo" or "elfe" => Race.Elf,
                "dwarf" or "enano" or "nain" => Race.Dwarf,
                "halfling" or "halfing" or "mediano" or "halfelin"
                => Race.Halfling,
                _ => null
            };
        if (race.HasValue)
            {
                return race.Value;
            }

            Console.WriteLine(_messages.GetMessage(52));

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
            string? input = Console.ReadLine()?.Trim().ToLowerInvariant();

            Occupation? occupation = input switch
            {
                "fighter" or "guerrero" or "guerrier"
                    => Occupation.Fighter,

                "magician" or "wizard" or "mago" or "magicien"
                    => Occupation.Magician,

                "thief" or "ladrón" or "ladron" or "voleur"
                    => Occupation.Thief,

                "archer" or "arquero"
                    => Occupation.Archer,

                _ => null
            };

            if (occupation.HasValue)
            {
                return occupation.Value;
            }

            Console.WriteLine(_messages.GetMessage(54));
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

    public void DisplayCharacterStats(Player player)
    {
        Console.WriteLine(
            $"{_messages.GetMessage(27)}: {player.name}");

        Console.WriteLine(
            $"{_messages.GetMessage(28)}: {TranslateRace(player.race)}");

        Console.WriteLine(
            $"{_messages.GetMessage(29)}: {TranslateOccupation(player.occupation)}");

        Console.WriteLine(
            $"{_messages.GetMessage(30)}: {player.strength}");

        Console.WriteLine(
            $"{_messages.GetMessage(31)}: {player.agility}");

        Console.WriteLine(
            $"{_messages.GetMessage(32)}: {player.health}");

        Console.WriteLine(
            $"{_messages.GetMessage(33)}: {TranslateWeapon(player.weapon.type)}");

        Console.WriteLine(
            $"{_messages.GetMessage(34)}: {player.weapon.maxDamage}");
    }
}