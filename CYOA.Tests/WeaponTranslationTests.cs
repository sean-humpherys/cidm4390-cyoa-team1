using CYOA;
using cidm4390_cyoa_team1;

namespace CYOA.Tests;

[TestClass]
[DoNotParallelize]
public class WeaponTranslationTests
{
    [TestMethod]
    [DataRow(Language.Spanish, "long sword", "espada larga")]
    [DataRow(Language.Spanish, "lightning bolt spell", "hechizo de rayo")]
    [DataRow(Language.Spanish, "dagger", "daga")]
    [DataRow(Language.Spanish, "long bow", "arco largo")]
    [DataRow(Language.French, "long sword", "épée longue")]
    [DataRow(Language.French, "lightning bolt spell", "sort de foudre")]
    [DataRow(Language.French, "dagger", "dague")]
    [DataRow(Language.French, "long bow", "arc long")]
    [DataRow(Language.English, "long sword", "long sword")]
    [DataRow(Language.English, "lightning bolt spell", "lightning bolt spell")]
    [DataRow(Language.English, "dagger", "dagger")]
    [DataRow(Language.English, "long bow", "long bow")]
    public void KnownWeapon_PreservesExactTranslationAndCompatibility(Language language, string weapon, string expected)
    {
        var messages = new Messages { CurrentLanguage = language };
        var creation = new CharacterCreation(messages, _ => throw new AssertFailedException("Translation must not roll dice."));
        Assert.AreEqual(expected, messages.TranslateWeapon(weapon));
        Assert.AreEqual(expected, creation.TranslateWeapon(weapon));
        Assert.IsEmpty(messages.LanguageDictionary);
        Assert.AreEqual(language, messages.CurrentLanguage);
    }
    [TestMethod]
    [DataRow("custom weapon")]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("Long Sword")]
    [DataRow(" long sword ")]
    [DataRow("enchanted dagger")]
    [DataRow(null)]
    public void UnrecognizedWeapon_IsPassedThroughUnchanged(string? weapon)
    {
        foreach (Language language in new[] { Language.English, Language.Spanish, Language.French, (Language)999 })
        {
            var messages = new Messages { CurrentLanguage = language };
            var creation = new CharacterCreation(messages, _ => throw new AssertFailedException("Translation must not roll dice."));
            Assert.AreEqual(weapon, messages.TranslateWeapon(weapon!));
            Assert.AreEqual(weapon, creation.TranslateWeapon(weapon!));
            Assert.IsEmpty(messages.LanguageDictionary);
            Assert.AreEqual(language, messages.CurrentLanguage);
        }
    }
    [TestMethod]
    public void Translation_UsesLiveLanguageAndPreservesMessagesWithoutOutput()
    {
        var messages = new Messages();
        var dictionary = messages.LanguageDictionary;
        dictionary["sentinel"] = "unchanged";
        var creation = new CharacterCreation(messages, _ => throw new AssertFailedException("Translation must not roll dice."));
        var originalOutput = Console.Out;
        using var output = new StringWriter();
        try
        {
            Console.SetOut(output);
            messages.CurrentLanguage = Language.English;
            Assert.AreEqual("long sword", messages.TranslateWeapon("long sword"));
            Assert.AreEqual(Language.English, messages.CurrentLanguage);
            messages.CurrentLanguage = Language.Spanish;
            Assert.AreEqual("espada larga", messages.TranslateWeapon("long sword"));
            Assert.AreEqual("espada larga", creation.TranslateWeapon("long sword"));
            Assert.AreEqual(Language.Spanish, messages.CurrentLanguage);
            messages.CurrentLanguage = Language.French;
            Assert.AreEqual("épée longue", messages.TranslateWeapon("long sword"));
            Assert.AreEqual("épée longue", creation.TranslateWeapon("long sword"));
            Assert.AreEqual(Language.French, messages.CurrentLanguage);
            messages.CurrentLanguage = (Language)999;
            Assert.AreEqual("long sword", messages.TranslateWeapon("long sword"));
            Assert.AreEqual("long sword", creation.TranslateWeapon("long sword"));
            Assert.AreEqual((Language)999, messages.CurrentLanguage);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
        Assert.AreSame(dictionary, messages.LanguageDictionary);
        Assert.HasCount(1, dictionary);
        Assert.AreEqual("unchanged", dictionary["sentinel"]);
        Assert.AreEqual("", output.ToString());
    }
    private static string Capture(Action action)
    {
        var originalOutput = Console.Out;
        using var output = new StringWriter();
        try
        {
            Console.SetOut(output);
            action();
            return output.ToString();
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }

    [TestMethod]
    [DataRow(Language.Spanish, "long sword", "espada larga")]
    [DataRow(Language.Spanish, "lightning bolt spell", "hechizo de rayo")]
    [DataRow(Language.Spanish, "dagger", "daga")]
    [DataRow(Language.Spanish, "long bow", "arco largo")]
    [DataRow(Language.French, "long sword", "épée longue")]
    [DataRow(Language.French, "lightning bolt spell", "sort de foudre")]
    [DataRow(Language.French, "dagger", "dague")]
    [DataRow(Language.French, "long bow", "arc long")]
    [DataRow(Language.English, "long sword", "long sword")]
    [DataRow(Language.English, "lightning bolt spell", "lightning bolt spell")]
    [DataRow(Language.English, "dagger", "dagger")]
    [DataRow(Language.English, "long bow", "long bow")]
    public void CharacterStatistics_PreserveLocalizedWeaponName(Language language, string weapon, string expected)
    {
        var messages = new Messages();
        messages.ReadDictionary(Path.Combine(AppContext.BaseDirectory, "MessageFiles", language + ".json"));
        var player = new Player { weapon = new Weapon(weapon, 17, "unchanged-art") };
        string before = System.Text.Json.JsonSerializer.Serialize(player);
        var creation = new CharacterCreation(messages, _ => throw new AssertFailedException("Statistics must not roll dice."));
        string output = Capture(() => creation.DisplayCharacterStats(player));
        StringAssert.Contains(output, $"{messages.GetMessage(33)}: {expected}");
        StringAssert.Contains(output, $"{messages.GetMessage(34)}: 17");
        Assert.AreEqual(before, System.Text.Json.JsonSerializer.Serialize(player));
    }
    [TestMethod]
    [DataRow(Language.Spanish, "long sword", "espada larga")]
    [DataRow(Language.Spanish, "lightning bolt spell", "hechizo de rayo")]
    [DataRow(Language.Spanish, "dagger", "daga")]
    [DataRow(Language.Spanish, "long bow", "arco largo")]
    [DataRow(Language.French, "long sword", "épée longue")]
    [DataRow(Language.French, "lightning bolt spell", "sort de foudre")]
    [DataRow(Language.French, "dagger", "dague")]
    [DataRow(Language.French, "long bow", "arc long")]
    [DataRow(Language.English, "long sword", "long sword")]
    [DataRow(Language.English, "lightning bolt spell", "lightning bolt spell")]
    [DataRow(Language.English, "dagger", "dagger")]
    [DataRow(Language.English, "long bow", "long bow")]
    public void CombatIntroduction_PreservesLocalizedWeaponName(Language language, string weapon, string expected)
    {
        var messages = new Messages();
        messages.ReadDictionary(Path.Combine(AppContext.BaseDirectory, "MessageFiles", language + ".json"));
        var player = new Player { strength = 0, weapon = new Weapon(weapon, 17, "unchanged-art") };
        var dragon = new Dragon("Dragon") { health = 30 };
        var rolls = new Queue<int>(new[] { 20 });
        string before = System.Text.Json.JsonSerializer.Serialize(player);
        string output = Capture(() => new Combat(player, dragon, messages, _ => rolls.Dequeue()).PlayerAttacksDragonSequence());
        string occupation = language switch
        {
            Language.Spanish => "Guerrero",
            Language.French => "Guerrier",
            _ => "Fighter"
        };
        StringAssert.Contains(output, messages.FormatMessage(20, new Dictionary<string, string>
        {
            ["occupation"] = occupation, ["weapon"] = expected
        }));
        StringAssert.Contains(output, messages.FormatMessage(35, "dragonName", "Dragon"));
        Assert.AreEqual(before, System.Text.Json.JsonSerializer.Serialize(player));
        Assert.AreEqual(30, dragon.health);
        Assert.IsEmpty(rolls);
    }
}
