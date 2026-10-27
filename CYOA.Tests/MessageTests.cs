
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

    [TestMethod] // Basic Message Test
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

    [TestMethod] // Language Load Test
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

    [TestMethod] // Verify JSON section headings are excluded
    public void ReadDictionary_IgnoresSectionHeadings()
    {
        Messages messages = new Messages();

        messages.ReadDictionary(GetLanguagePath("English"));

        Assert.AreEqual(51, messages.LanguageDictionary.Count);
        Assert.IsFalse(
            messages.LanguageDictionary.ContainsKey("_SECTION_1"));

        Assert.AreEqual(
            "Welcome to the game!",
            messages.GetMessage(1));
    }

    [TestMethod] // Format Replace Test
    public void FormatMessage_ReplacesDragonName()
    {
        Messages messages = new Messages();

        messages.ReadDictionary(GetLanguagePath("English"));

        string result = messages.FormatMessage(
            21, "dragonName", "Smaug");

        Assert.AreEqual("You hit Smaug.", result);
    }

    [TestMethod] // Replace Multiples
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

        Assert.IsTrue(result.Contains("8 damage points"));
        Assert.IsTrue(result.Contains("health points is now 12"));
        Assert.IsFalse(result.Contains("{damage}"));
        Assert.IsFalse(result.Contains("{healthPoints}"));
    }

    [TestMethod] // Spanish Test
    public void ReadDictionary_SpanishHasAllMessages()
    {
        Messages messages = new Messages();
        messages.ReadDictionary(GetLanguagePath("Spanish"));

        Assert.AreEqual(51, messages.LanguageDictionary.Count);

        for (int i = 1; i <= 51; i++)
        {
            Assert.IsTrue(
                messages.LanguageDictionary.ContainsKey(i.ToString()),
                $"Missing Spanish message {i}");
        }
    }

    [TestMethod] // French Test
    public void ReadDictionary_FrenchHasAllMessages()
    {
        Messages messages = new Messages();
        messages.ReadDictionary(GetLanguagePath("French"));

        Assert.AreEqual(51, messages.LanguageDictionary.Count);

        for (int i = 1; i <= 51; i++)
        {
            Assert.IsTrue(
                messages.LanguageDictionary.ContainsKey(i.ToString()),
                $"Missing French message {i}");
        }
    }

    [TestMethod] // Current Language Test
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

    
    [TestMethod] // Missing Message Test
    public void GetMessage_MissingKey_ThrowsException()
    {
        Messages messages = new Messages();

        messages.ReadDictionary(GetLanguagePath("English"));

        Assert.ThrowsExactly<KeyNotFoundException>(
            () => messages.GetMessage(999));
    }
}