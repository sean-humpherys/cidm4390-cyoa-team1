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

        if (Enum.TryParse<Language>(fileName, true, out Language language))
        {
            CurrentLanguage = language;
        }
        
        else
        {
            throw new ArgumentException($"Unsupported language: {fileName}");
        }

        LanguageDictionary.Clear();

        foreach (string line in File.ReadAllLines(languageFilePath))
        {
            if (string.IsNullOrWhiteSpace(line) ||
                line.TrimStart().StartsWith("#"))
            {
                continue;
            }

            string[] parts = line.Split('=', 2);

            if (parts.Length != 2 ||
                !int.TryParse(parts[0].Trim(), out int messageNumber))
            {
                throw new FormatException($"Invalid message: {line}");
            }

            LanguageDictionary[messageNumber.ToString()] = parts[1].Trim();
        }
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
}