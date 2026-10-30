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
                    Console.WriteLine(_messages.GetMessage(56));
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

}