using System.Text.Json;

namespace CYOA;

public class Messages
{
    public Language CurrentLanguage {get; set; }
    public Dictionary<string, string> LanguageDictionary {get; set; }
        =new Dictionary<string, string>();

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