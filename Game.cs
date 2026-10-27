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

        bool gameRunning = true;

        while (gameRunning)
        {
            Console.WriteLine(_messages.GetMessage(6));

            string? choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    CreatePlayer();
                    gameRunning = false;
                    break;

                case "2":
                    EndGame();
                    gameRunning = false;
                    break;

                default:
                    Console.WriteLine(
                        "Invalid choice. Please enter 1 or 2.");
                    break;
            }
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

        if (!Enum.TryParse<Race>(
                raceInput, true, out Race race) ||
            !Enum.TryParse<Occupation>(
                occupationInput, true, out Occupation occupation))
            {
                Console.WriteLine(
                    "Invalid race or occupation. Please try again.");
                return;
            }

        Player player = new Player(name, race, occupation);

        Console.WriteLine(player.DisplayStats());
    }
}