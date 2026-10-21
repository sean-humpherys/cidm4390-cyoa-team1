using Microsoft.VisualStudio.TestTools.UnitTesting;
using CYOA;
using System.Reflection;
using System.Reflection.Metadata;
using System.Diagnostics.SymbolStore;

namespace CYOA.Tests;

[TestClass]
public class MessagesTests
{
    [TestMethod] //Basic Message Test
    public void GetMessage_ReturnsCorrectMessage()
    {
        // Arrange
        Messages messages = new Messages ();
        messages.LanguageDictionary["1"] = "Welcome to the game!";

        // Act
        string result = messages.GetMessage(1);

        // Assert
        Assert.AreEqual("Welcome to the game!", result);
    }

    [TestMethod] //Language Load Test
    public void ReadDictionary_LoadsAllThreeLanguages()
    {
        Messages messages = new Messages();

        // English
        string englishPath = Path.Combine(
            AppContext.BaseDirectory, "MessageFiles", "English.txt");

        messages.ReadDictionary(englishPath);
        Assert.AreEqual("Welcome to the game!", messages.GetMessage(1));
        Assert.AreEqual("Choose your character.", messages.GetMessage(2));

        // French
        string frenchPath = Path.Combine(
            AppContext.BaseDirectory, "MessageFiles", "French.txt");

        messages.ReadDictionary(frenchPath);
        Assert.AreEqual("Bienvenue dans le jeu !", messages.GetMessage(1));

        // Spanish
        string spanishPath = Path.Combine(
            AppContext.BaseDirectory, "MessageFiles", "Spanish.txt");

        messages.ReadDictionary(spanishPath);
        Assert.AreEqual("¡Bienvenido al juego!", messages.GetMessage(1));
    }

    [TestMethod] //Ignore .txt Comments
    public void ReadDictionary_IgnoresComments()
    {
        Messages messages = new Messages();

        messages.ReadDictionary("MessageFiles/English.txt");

        Assert.AreEqual(
            "Welcome to the game!",
            messages.GetMessage(1));
    }

    [TestMethod] //Format Replace Test
    public void FormatMessage_ReplacesDragonName()
    {
        Messages messages = new Messages();

        messages.ReadDictionary("MessageFiles/English.txt");

        string result = messages.FormatMessage(
            21, "dragonName", "Smaug");
        
        Assert.AreEqual("You hit Smaug.", result);
    }

    [TestMethod] //Replace Multiples
    public void FormatMessage_ReplacesMultiplePlaceholders()
    {
        Messages messages = new Messages();
        messages.ReadDictionary("MessageFiles/English.txt");

        string result = messages.FormatMessage(23,
            new Dictionary<string, string>
            {
                {"damage", "8"},
                {"healthPoints", "12"}
            });

        Assert.IsTrue(result.Contains("8 damage points"));
        Assert.IsTrue(result.Contains("health points is now 12"));
        Assert.IsFalse(result.Contains("{damage}"));
        Assert.IsFalse(result.Contains("{healthPoints}"));
    }

    [TestMethod] //Spanish Test
    public void ReadDictionary_SpanishHasAllMessages()
    {
        Messages messages = new Messages();
        messages.ReadDictionary("MessageFiles/Spanish.txt");

         Assert.AreEqual(51, messages.LanguageDictionary.Count);

        for (int i = 1; i <= 51; i++)
        {
            Assert.IsTrue(
                messages.LanguageDictionary.ContainsKey(i.ToString()),
                $"Missing Spanish message {i}");
        }
    }

    [TestMethod] //French Test
    public void ReadDictionary_FrenchHasAllMessages()
    {
        Messages messages = new Messages();
        messages.ReadDictionary("MessageFiles/French.txt");

         Assert.AreEqual(51, messages.LanguageDictionary.Count);

        for (int i = 1; i <= 51; i++)
        {
            Assert.IsTrue(
                messages.LanguageDictionary.ContainsKey(i.ToString()),
                $"Missing French message {i}");
        }
    }

    [TestMethod] //Current Language Test
    public void ReadDictionary_UpdatesCurrentLanguage()
    {
        Messages messages = new Messages();

        messages.ReadDictionary("MessageFiles/Spanish.txt");
        Assert.AreEqual(Language.Spanish, messages.CurrentLanguage);

        messages.ReadDictionary("MessageFiles/French.txt");
        Assert.AreEqual(Language.French, messages.CurrentLanguage);

        messages.ReadDictionary("MessageFiles/English.txt");
        Assert.AreEqual(Language.English, messages.CurrentLanguage);
    }
}