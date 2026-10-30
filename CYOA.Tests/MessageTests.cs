
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CYOA;

namespace CYOA.Tests;

[TestClass]
public class MessagesTests
{
    // Locate JSON files in the test output directory.
    private static string GetLanguagePath(string language)
    {
        return Path.Combine(
            AppContext.BaseDirectory,
            "MessageFiles",
            $"{language}.json");
    }

    // Verify a message can be retrieved by its ID.
    [TestMethod]
    public void GetMessage_ReturnsCorrectMessage()
    {
        // Arrange
        Messages messages = new Messages();
        messages.LanguageDictionary["1"] = "Welcome to the game!";

        // Act
        string result = messages.GetMessage(1);

        // Assert
        Assert.AreEqual("Welcome to the game!", result);
    }

    // Verify all three language files load correctly.
    [TestMethod]
    public void ReadDictionary_LoadsAllThreeLanguages()
    {
        Messages messages = new Messages();

        // English
        messages.ReadDictionary(GetLanguagePath("English"));
        Assert.AreEqual("Welcome to the game!", messages.GetMessage(1));
        Assert.AreEqual("Choose your character.", messages.GetMessage(2));

        // French
        messages.ReadDictionary(GetLanguagePath("French"));
        Assert.AreEqual("Bienvenue dans le jeu !", messages.GetMessage(1));

        // Spanish
        messages.ReadDictionary(GetLanguagePath("Spanish"));
        Assert.AreEqual("¡Bienvenido al juego!", messages.GetMessage(1));
    }

    // Verify JSON section headings are excluded from the dictionary.
    [TestMethod]
    public void ReadDictionary_IgnoresSectionHeadings()
    {
        Messages messages = new Messages();

        messages.ReadDictionary(GetLanguagePath("English"));

        Assert.HasCount(58, messages.LanguageDictionary);
        Assert.IsFalse(
            messages.LanguageDictionary.ContainsKey("_SECTION_1"));
        Assert.IsFalse(
            messages.LanguageDictionary.ContainsKey("_SECTION_10"));

        Assert.AreEqual(
            "Welcome to the game!",
            messages.GetMessage(1));
    }

    // Verify a single placeholder is replaced.
    [TestMethod]
    public void FormatMessage_ReplacesDragonName()
    {
        Messages messages = new Messages();

        messages.ReadDictionary(GetLanguagePath("English"));

        string result = messages.FormatMessage(
            21, "dragonName", "Smaug");

        Assert.AreEqual("You hit Smaug.", result);
    }

    // Verify multiple placeholders are replaced.
    [TestMethod]
    public void FormatMessage_ReplacesMultiplePlaceholders()
    {
        Messages messages = new Messages();

        messages.ReadDictionary(GetLanguagePath("English"));

        string result = messages.FormatMessage(
            23,
            new Dictionary<string, string>
            {
                { "damage", "8" },
                { "healthPoints", "12" }
            });

        Assert.Contains("8 damage points", result);
        Assert.Contains("health points is now 12", result);
        Assert.DoesNotContain("{damage}", result);
        Assert.DoesNotContain("{healthPoints}", result);
    }

    // Verify Spanish contains all 58 messages.
    [TestMethod]
    public void ReadDictionary_SpanishHasAllMessages()
    {
        Messages messages = new Messages();
        messages.ReadDictionary(GetLanguagePath("Spanish"));

        Assert.HasCount(58, messages.LanguageDictionary);

        for (int i = 1; i <= 58; i++)
        {
            Assert.IsTrue(
                messages.LanguageDictionary.ContainsKey(i.ToString()),
                $"Missing Spanish message {i}");
        }
    }

    // Verify French contains all 58 messages.
    [TestMethod]
    public void ReadDictionary_FrenchHasAllMessages()
    {
        Messages messages = new Messages();
        messages.ReadDictionary(GetLanguagePath("French"));

        Assert.HasCount(58, messages.LanguageDictionary);

        for (int i = 1; i <= 58; i++)
        {
            Assert.IsTrue(
                messages.LanguageDictionary.ContainsKey(i.ToString()),
                $"Missing French message {i}");
        }
    }

    // Verify loading a file updates the current language.
    [TestMethod]
    public void ReadDictionary_UpdatesCurrentLanguage()
    {
        Messages messages = new Messages();

        messages.ReadDictionary(GetLanguagePath("Spanish"));
        Assert.AreEqual(Language.Spanish, messages.CurrentLanguage);

        messages.ReadDictionary(GetLanguagePath("French"));
        Assert.AreEqual(Language.French, messages.CurrentLanguage);

        messages.ReadDictionary(GetLanguagePath("English"));
        Assert.AreEqual(Language.English, messages.CurrentLanguage);
    }

    // Verify requesting an unknown message throws an exception.
    [TestMethod]
    public void GetMessage_MissingKey_ThrowsException()
    {
        Messages messages = new Messages();

        messages.ReadDictionary(GetLanguagePath("English"));

        Assert.ThrowsExactly<KeyNotFoundException>(
            () => messages.GetMessage(999));
    }

    // Verify English character creation validation messages.
    [TestMethod]
    public void ReadDictionary_CharacterCreationValidationMessages_AreAvailable()
    {
        Messages messages = new Messages();
        messages.ReadDictionary(GetLanguagePath("English"));

        Assert.AreEqual("Invalid race. Please try again.", messages.GetMessage(52));
        Assert.AreEqual("Character name cannot be empty.", messages.GetMessage(53));
        Assert.AreEqual("Invalid occupation. Please try again.", messages.GetMessage(54));
        Assert.AreEqual("Please type 'roll' to continue.", messages.GetMessage(55));
        Assert.AreEqual("Invalid choice. Please enter 1 or 2.", messages.GetMessage(56));
    }

    // Verify French character creation prompts, stats, and validation.
    [TestMethod]
    public void ReadDictionary_FrenchCharacterCreationMessages_AreAvailable()
    {
        Messages messages = new Messages();
        messages.ReadDictionary(GetLanguagePath("French"));

        for (int i = 13; i <= 19; i++)
        {
            Assert.IsFalse(
                string.IsNullOrWhiteSpace(messages.GetMessage(i)),
                $"Missing French character creation prompt {i}");
        }

        for (int i = 27; i <= 34; i++)
        {
            Assert.IsFalse(
                string.IsNullOrWhiteSpace(messages.GetMessage(i)),
                $"Missing French character statistic label {i}");
        }

        for (int i = 52; i <= 56; i++)
        {
            Assert.IsFalse(
                string.IsNullOrWhiteSpace(messages.GetMessage(i)),
                $"Missing French validation message {i}");
        }

        Assert.AreEqual("Nom", messages.GetMessage(27));
        Assert.AreEqual("Force", messages.GetMessage(30));
        Assert.AreEqual("Race invalide. Veuillez réessayer.", messages.GetMessage(52));
    }

    // Verify Spanish character creation prompts, stats, and validation.
    [TestMethod]
    public void ReadDictionary_SpanishCharacterCreationMessages_AreAvailable()
    {
        Messages messages = new Messages();
        messages.ReadDictionary(GetLanguagePath("Spanish"));

        for (int i = 13; i <= 19; i++)
        {
            Assert.IsFalse(
                string.IsNullOrWhiteSpace(messages.GetMessage(i)),
                $"Missing Spanish character creation prompt {i}");
        }

        for (int i = 27; i <= 34; i++)
        {
            Assert.IsFalse(
                string.IsNullOrWhiteSpace(messages.GetMessage(i)),
                $"Missing Spanish character statistic label {i}");
        }

        for (int i = 52; i <= 56; i++)
        {
            Assert.IsFalse(
                string.IsNullOrWhiteSpace(messages.GetMessage(i)),
                $"Missing Spanish validation message {i}");
        }

        Assert.AreEqual("Nombre", messages.GetMessage(27));
        Assert.AreEqual("Fuerza", messages.GetMessage(30));
        Assert.AreEqual("Raza no válida. Inténtalo de nuevo.", messages.GetMessage(52));
    }
}