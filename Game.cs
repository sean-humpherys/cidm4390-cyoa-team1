using System;
using System.IO;
using CYOA;
using cidm4390_cyoa_team1;

namespace CYOA;

public class Game
{
    private readonly Messages _messages = new Messages();

    public void StartGame()
    {
        LoadDefaultMessages();

        Console.WriteLine(_messages.GetMessage(5));

        SelectLanguage();

        bool gameRunning = true;

        while (gameRunning)
        {
            Console.WriteLine(_messages.GetMessage(6));

            string? choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                {
                    
                    Die die = new Die();
                    CharacterCreation characterCreation =
                        new CharacterCreation(_messages, die);

                    Player player = characterCreation.CreateCharacter();

                    
                    Adventure adventure = new Adventure(_messages);
                    adventure.StartAdventure(player);

                    gameRunning = false;
                    break;
                }

                case "2":
                    EndGame();
                    gameRunning = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please enter 1 or 2.");
                    break;
            }
        }
    }

    private void SelectLanguage()
    {
        while (true)
        {
            Console.WriteLine(
                "Which language do you prefer? " +
                "English (E), Spanish (S), or French (F)?");

            string? choice = Console.ReadLine()?.Trim()
                .ToLowerInvariant();

            string? fileName = choice switch
            {
                "e" => "English.json",
                "s" => "Spanish.json",
                "f" => "French.json",
                _ => null
            };

            if (fileName == null)
            {
                Console.WriteLine(
                    "Invalid choice. Please enter E, S, or F.");
                continue;
            }

            string? filePath = FindMessageFile(fileName);

            if (filePath == null)
            {
                throw new FileNotFoundException(
                    $"Could not find MessageFiles/{fileName}.");
            }

            _messages.ReadDictionary(filePath);

            return;
        }
    }
    public void EndGame()
    {
        Console.WriteLine(_messages.GetMessage(4));
    }

    private void LoadDefaultMessages()
    {
        string? filePath = FindMessageFile("English.json");

        if (filePath == null)
        {
            throw new FileNotFoundException(
                "Could not find MessageFiles/English.json. " +
                "Check that the MessageFiles folder is available.");
        }

        _messages.ReadDictionary(filePath);
    }

    private static string? FindMessageFile(string fileName)
    {
        string[] startingDirectories =
        {
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory()
        };

        foreach (string startingDirectory in startingDirectories)
        {
            DirectoryInfo? directory =
                new DirectoryInfo(startingDirectory);

            while (directory != null)
            {
                string path = Path.Combine(
                    directory.FullName,
                    "MessageFiles",
                    fileName);

                if (File.Exists(path))
                {
                    return path;
                }

                directory = directory.Parent;
            }
        }

        return null;
    }

    private void CreatePlayer()
    {
        Console.WriteLine(_messages.GetMessage(13));
        string? raceInput = Console.ReadLine()?.Trim();

        Console.WriteLine(_messages.GetMessage(14));
        string? name = Console.ReadLine()?.Trim();

        Console.WriteLine(_messages.GetMessage(15));
        string? occupationInput = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(raceInput) ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(occupationInput))
            {
                Console.WriteLine(
                    "Character information cannot be empty.");
                return;
            }
        Race race;
        Occupation occupation;

        switch (raceInput.ToLowerInvariant())
        {
            case "human":
            case "humano":
            case "humain":
                race = Race.Human;
                break;
            
            case "elf":
            case "elfo":
            case "elfe":
                race = Race.Elf;
                break;
            
            case "dwarf":
            case "enano":
            case "nain":
                race = Race.Dwarf;
                break;
            
            case "halfling":
            case "mediano":
            case "halfelin":
                race = Race.Halfling;
                break;
            
            default:
                Console.WriteLine("Invalid Race. Please try again.");
                return;

        }

        switch (occupationInput.ToLowerInvariant())
        {
            case "fighter":
            case "guerrero":
            case "Guerrier":
                occupation = Occupation.Fighter;
                break;

            case "wizard":
            case "mago":
            case "magicien":
                occupation = Occupation.Magician;
                break;

            case "thief":
            case "ladrón":
            case "voleur":
                occupation = Occupation.Thief;
                break;

            case "archer":
            case "arquero":
                occupation = Occupation.Archer;
                break;

            default:
                Console.WriteLine("Invalid occupation. Please try again.");
            return;
        }

        Player player = new Player(name, race, occupation);

        Console.WriteLine(player.DisplayStats());
    }
}