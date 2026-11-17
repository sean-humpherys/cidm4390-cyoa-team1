using System.Text.Json;
using cidm4390_cyoa_team1;

namespace CYOA;

public class Messages
{
    public Language CurrentLanguage {get; set; }
    public Dictionary<string, string> LanguageDictionary {get; set; }
        =new Dictionary<string, string>();

    public string TranslateRace(Race race)
    {
        return CurrentLanguage switch
        {
            Language.Spanish => race switch
            {
                Race.Halfling => "Mediano",
                Race.Dwarf => "Enano",
                Race.Elf => "Elfo",
                Race.Human => "Humano",
                _ => race.ToString()
            },
            Language.French => race switch
            {
                Race.Halfling => "Halfelin",
                Race.Dwarf => "Nain",
                Race.Elf => "Elfe",
                Race.Human => "Humain",
                _ => race.ToString()
            },
            _ => race.ToString()
        };
    }

    public string TranslateOccupation(Occupation occupation)
    {
        return CurrentLanguage switch
        {
            Language.Spanish => occupation switch
            {
                Occupation.Archer => "Arquero",
                Occupation.Thief => "Ladrón",
                Occupation.Magician => "Mago",
                Occupation.Fighter => "Guerrero",
                _ => occupation.ToString()
            },
            Language.French => occupation switch
            {
                Occupation.Thief => "Voleur",
                Occupation.Magician => "Magicien",
                Occupation.Fighter => "Guerrier",
                _ => occupation.ToString()
            },
            _ => occupation.ToString()
        };
    }

    public string TranslateWeapon(string weapon)
    {
        return CurrentLanguage switch
        {
            Language.Spanish => weapon switch
            {
                "long bow" => "arco largo",
                "dagger" => "daga",
                "lightning bolt spell" => "hechizo de rayo",
                "long sword" => "espada larga",
                _ => weapon
            },
            Language.French => weapon switch
            {
                "long bow" => "arc long",
                "dagger" => "dague",
                "lightning bolt spell" => "sort de foudre",
                "long sword" => "épée longue",
                _ => weapon
            },
            _ => weapon
        };
    }

public string GetMessage(int whichMessage)
    {
        string messageKey = whichMessage.ToString();
        return LanguageDictionary[messageKey];
    }

public void ReadDictionary(string languageFilePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(languageFilePath);

        if (!Enum.TryParse<Language>(fileName, true, out Language language)
            || !Enum.IsDefined(typeof(Language), language))
        {
            throw new ArgumentException($"Unsupported language: {fileName}");
        }

        string json = File.ReadAllText(languageFilePath);

        Dictionary<string, string> messages =
            JsonSerializer.Deserialize<Dictionary<string, string>>(json)
            ?? throw new FormatException("Language file is empty or invalid.");

        // Remove section headings so only numbered messages remain.
        foreach (string key in messages.Keys.ToList())
        {
            if (key.StartsWith("_SECTION_"))
            {
                messages.Remove(key);
            }
        }

        LanguageDictionary = messages;
        CurrentLanguage = language;
    }

public string FormatMessage(int whichMessage, string placeholder, string value)
    {
        string message = GetMessage(whichMessage);
        return message.Replace("{" + placeholder + "}", value);
    }

public string FormatMessage(int whichMessage, Dictionary<string, string> replacements)
    {
        string message = GetMessage(whichMessage);

        foreach (var replacement in replacements)
        {
            message = message.Replace("{" + replacement.Key + "}", replacement.Value);
        }

        return message;
    }

// Randomizer for dragon taunts and sarcastic replies
private static readonly Random _random = new Random();

public string GetRandomMessage(int firstId, int lastId)
    {
        int messageId = _random.Next(firstId, lastId + 1);
        return GetMessage(messageId);
    }
}