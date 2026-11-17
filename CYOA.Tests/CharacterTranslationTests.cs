using CYOA;
using cidm4390_cyoa_team1;

namespace CYOA.Tests;

[TestClass]
[DoNotParallelize]
public class CharacterTranslationTests
{
    [TestMethod]
    [DataRow(Language.Spanish, Race.Human, "Humano")]
    [DataRow(Language.Spanish, Race.Elf, "Elfo")]
    [DataRow(Language.Spanish, Race.Dwarf, "Enano")]
    [DataRow(Language.Spanish, Race.Halfling, "Mediano")]
    [DataRow(Language.French, Race.Human, "Humain")]
    [DataRow(Language.French, Race.Elf, "Elfe")]
    [DataRow(Language.French, Race.Dwarf, "Nain")]
    [DataRow(Language.French, Race.Halfling, "Halfelin")]
    [DataRow(Language.English, Race.Human, "Human")]
    [DataRow(Language.English, Race.Elf, "Elf")]
    [DataRow(Language.English, Race.Dwarf, "Dwarf")]
    [DataRow(Language.English, Race.Halfling, "Halfling")]
    public void RaceTranslation_PreservesExactNamesAndCompatibility(Language language, Race race, string expected)
    {
        var messages = new Messages { CurrentLanguage = language };
        var creation = new CharacterCreation(messages, _ => throw new AssertFailedException("Translation must not roll dice."));
        Assert.AreEqual(expected, messages.TranslateRace(race));
        Assert.AreEqual(expected, creation.TranslateRace(race));
        Assert.IsEmpty(messages.LanguageDictionary);
        Assert.AreEqual(language, messages.CurrentLanguage);
    }
    private static Messages Load(Language language)
    {
        var messages = new Messages();
        messages.ReadDictionary(Path.Combine(AppContext.BaseDirectory, "MessageFiles", language + ".json"));
        return messages;
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
    [DataRow(Language.Spanish, Race.Human, "Humano")]
    [DataRow(Language.Spanish, Race.Elf, "Elfo")]
    [DataRow(Language.Spanish, Race.Dwarf, "Enano")]
    [DataRow(Language.Spanish, Race.Halfling, "Mediano")]
    [DataRow(Language.French, Race.Human, "Humain")]
    [DataRow(Language.French, Race.Elf, "Elfe")]
    [DataRow(Language.French, Race.Dwarf, "Nain")]
    [DataRow(Language.French, Race.Halfling, "Halfelin")]
    [DataRow(Language.English, Race.Human, "Human")]
    [DataRow(Language.English, Race.Elf, "Elf")]
    [DataRow(Language.English, Race.Dwarf, "Dwarf")]
    [DataRow(Language.English, Race.Halfling, "Halfling")]
    public void CharacterStatistics_PreserveLocalizedRace(Language language, Race race, string expected)
    {
        var messages = Load(language);
        var player = new Player("Reader", race, Occupation.Fighter);
        string before = System.Text.Json.JsonSerializer.Serialize(player);
        var creation = new CharacterCreation(messages, _ => throw new AssertFailedException("Statistics must not roll dice."));
        string output = Capture(() => creation.DisplayCharacterStats(player));
        StringAssert.Contains(output, $"{messages.GetMessage(28)}: {expected}");
        Assert.AreEqual(before, System.Text.Json.JsonSerializer.Serialize(player));
    }
    [TestMethod]
    [DataRow(Language.Spanish, Race.Human, "Humano")]
    [DataRow(Language.Spanish, Race.Elf, "Elfo")]
    [DataRow(Language.Spanish, Race.Dwarf, "Enano")]
    [DataRow(Language.Spanish, Race.Halfling, "Mediano")]
    [DataRow(Language.French, Race.Human, "Humain")]
    [DataRow(Language.French, Race.Elf, "Elfe")]
    [DataRow(Language.French, Race.Dwarf, "Nain")]
    [DataRow(Language.French, Race.Halfling, "Halfelin")]
    [DataRow(Language.English, Race.Human, "Human")]
    [DataRow(Language.English, Race.Elf, "Elf")]
    [DataRow(Language.English, Race.Dwarf, "Dwarf")]
    [DataRow(Language.English, Race.Halfling, "Halfling")]
    public void DragonDefenseTaunt_PreservesLocalizedRaceAndCombatState(Language language, Race race, string expected)
    {
        var messages = Load(language);
        var player = new Player("Reader", race, Occupation.Fighter) { strength = 20 };
        var dragon = new Dragon("Dragon") { health = 30, agility = 20 };
        var rolls = new Queue<int>(new[] { 1, 1, 3 }); // Hit, defended, race-based taunt.
        string before = System.Text.Json.JsonSerializer.Serialize(player);
        string output = Capture(() => new Combat(player, dragon, messages, _ => rolls.Dequeue()).PlayerAttacksDragonSequence());
        StringAssert.Contains(output, messages.FormatMessage(50, "race", expected));
        StringAssert.Contains(output, messages.FormatMessage(22, "dragonName", "Dragon"));
        Assert.AreEqual(before, System.Text.Json.JsonSerializer.Serialize(player));
        Assert.AreEqual(30, dragon.health);
        Assert.IsEmpty(rolls);
        Assert.DoesNotContain("{race}", output);
    }
    [TestMethod]
    [DataRow(Language.Spanish, Occupation.Fighter, "Guerrero")]
    [DataRow(Language.Spanish, Occupation.Magician, "Mago")]
    [DataRow(Language.Spanish, Occupation.Thief, "Ladrón")]
    [DataRow(Language.Spanish, Occupation.Archer, "Arquero")]
    [DataRow(Language.French, Occupation.Fighter, "Guerrier")]
    [DataRow(Language.French, Occupation.Magician, "Magicien")]
    [DataRow(Language.French, Occupation.Thief, "Voleur")]
    [DataRow(Language.French, Occupation.Archer, "Archer")]
    [DataRow(Language.English, Occupation.Fighter, "Fighter")]
    [DataRow(Language.English, Occupation.Magician, "Magician")]
    [DataRow(Language.English, Occupation.Thief, "Thief")]
    [DataRow(Language.English, Occupation.Archer, "Archer")]
    public void OccupationTranslation_PreservesExactNamesAndCompatibility(Language language, Occupation occupation, string expected)
    {
        var messages = new Messages { CurrentLanguage = language };
        var creation = new CharacterCreation(messages, _ => throw new AssertFailedException("Translation must not roll dice."));
        Assert.AreEqual(expected, messages.TranslateOccupation(occupation));
        Assert.AreEqual(expected, creation.TranslateOccupation(occupation));
        Assert.IsEmpty(messages.LanguageDictionary);
        Assert.AreEqual(language, messages.CurrentLanguage);
    }
    [TestMethod]
    [DataRow(Language.Spanish, Occupation.Fighter, "Guerrero")]
    [DataRow(Language.Spanish, Occupation.Magician, "Mago")]
    [DataRow(Language.Spanish, Occupation.Thief, "Ladrón")]
    [DataRow(Language.Spanish, Occupation.Archer, "Arquero")]
    [DataRow(Language.French, Occupation.Fighter, "Guerrier")]
    [DataRow(Language.French, Occupation.Magician, "Magicien")]
    [DataRow(Language.French, Occupation.Thief, "Voleur")]
    [DataRow(Language.French, Occupation.Archer, "Archer")]
    [DataRow(Language.English, Occupation.Fighter, "Fighter")]
    [DataRow(Language.English, Occupation.Magician, "Magician")]
    [DataRow(Language.English, Occupation.Thief, "Thief")]
    [DataRow(Language.English, Occupation.Archer, "Archer")]
    public void CharacterStatistics_PreserveLocalizedOccupation(Language language, Occupation occupation, string expected)
    {
        var messages = Load(language);
        var player = new Player("Reader", Race.Human, occupation);
        string before = System.Text.Json.JsonSerializer.Serialize(player);
        var creation = new CharacterCreation(messages, _ => throw new AssertFailedException("Statistics must not roll dice."));
        string output = Capture(() => creation.DisplayCharacterStats(player));
        StringAssert.Contains(output, $"{messages.GetMessage(29)}: {expected}");
        Assert.AreEqual(before, System.Text.Json.JsonSerializer.Serialize(player));
    }
    [TestMethod]
    [DataRow(Language.Spanish, Occupation.Fighter, "Guerrero")]
    [DataRow(Language.Spanish, Occupation.Magician, "Mago")]
    [DataRow(Language.Spanish, Occupation.Thief, "Ladrón")]
    [DataRow(Language.Spanish, Occupation.Archer, "Arquero")]
    [DataRow(Language.French, Occupation.Fighter, "Guerrier")]
    [DataRow(Language.French, Occupation.Magician, "Magicien")]
    [DataRow(Language.French, Occupation.Thief, "Voleur")]
    [DataRow(Language.French, Occupation.Archer, "Archer")]
    [DataRow(Language.English, Occupation.Fighter, "Fighter")]
    [DataRow(Language.English, Occupation.Magician, "Magician")]
    [DataRow(Language.English, Occupation.Thief, "Thief")]
    [DataRow(Language.English, Occupation.Archer, "Archer")]
    public void CombatIntroduction_PreservesLocalizedOccupationAndWeapon(Language language, Occupation occupation, string expected)
    {
        var messages = Load(language);
        var player = new Player("Reader", Race.Human, occupation) { strength = 0 };
        var dragon = new Dragon("Dragon") { health = 30 };
        var rolls = new Queue<int>(new[] { 20 }); // Guaranteed player miss.
        string before = System.Text.Json.JsonSerializer.Serialize(player);
        string output = Capture(() => new Combat(player, dragon, messages, _ => rolls.Dequeue()).PlayerAttacksDragonSequence());
        string weapon = language switch
        {
            Language.Spanish => "espada larga",
            Language.French => "épée longue",
            _ => "long sword"
        };
        StringAssert.Contains(output, messages.FormatMessage(20, new Dictionary<string, string>
        {
            ["occupation"] = expected, ["weapon"] = weapon
        }));
        StringAssert.Contains(output, messages.FormatMessage(35, "dragonName", "Dragon"));
        Assert.AreEqual(before, System.Text.Json.JsonSerializer.Serialize(player));
        Assert.AreEqual(30, dragon.health);
        Assert.IsEmpty(rolls);
    }
    [TestMethod]
    [DataRow(Language.English)]
    [DataRow(Language.Spanish)]
    [DataRow(Language.French)]
    [DataRow((Language)999)]
    public void UndefinedEnumValues_PreserveNumericStringFallbacks(Language language)
    {
        var messages = new Messages { CurrentLanguage = language };
        var creation = new CharacterCreation(messages, _ => throw new AssertFailedException("Translation must not roll dice."));
        Assert.AreEqual("999", messages.TranslateRace((Race)999));
        Assert.AreEqual("-1", messages.TranslateRace((Race)(-1)));
        Assert.AreEqual("999", messages.TranslateOccupation((Occupation)999));
        Assert.AreEqual("-1", messages.TranslateOccupation((Occupation)(-1)));
        Assert.AreEqual("999", creation.TranslateRace((Race)999));
        Assert.AreEqual("-1", creation.TranslateRace((Race)(-1)));
        Assert.AreEqual("999", creation.TranslateOccupation((Occupation)999));
        Assert.AreEqual("-1", creation.TranslateOccupation((Occupation)(-1)));
        Assert.IsEmpty(messages.LanguageDictionary);
        Assert.AreEqual(language, messages.CurrentLanguage);
    }
    [TestMethod]
    [DataRow(Race.Human, Occupation.Fighter, "Human", "Fighter")]
    [DataRow(Race.Elf, Occupation.Magician, "Elf", "Magician")]
    [DataRow(Race.Dwarf, Occupation.Thief, "Dwarf", "Thief")]
    [DataRow(Race.Halfling, Occupation.Archer, "Halfling", "Archer")]
    public void UnsupportedLanguage_PreservesEnglishEnumNames(Race race, Occupation occupation, string expectedRace, string expectedOccupation)
    {
        var messages = new Messages { CurrentLanguage = (Language)999 };
        var creation = new CharacterCreation(messages, _ => throw new AssertFailedException("Translation must not roll dice."));
        Assert.AreEqual(expectedRace, messages.TranslateRace(race));
        Assert.AreEqual(expectedOccupation, messages.TranslateOccupation(occupation));
        Assert.AreEqual(expectedRace, creation.TranslateRace(race));
        Assert.AreEqual(expectedOccupation, creation.TranslateOccupation(occupation));
        Assert.IsEmpty(messages.LanguageDictionary);
        Assert.AreEqual((Language)999, messages.CurrentLanguage);
    }
    [TestMethod]
    public void LanguageChanges_AreObservedWithoutMutatingMessagesOrWritingOutput()
    {
        var messages = new Messages();
        var dictionary = messages.LanguageDictionary;
        dictionary["sentinel"] = "unchanged";
        var creation = new CharacterCreation(messages, _ => throw new AssertFailedException("Translation must not roll dice."));
        string output = Capture(() =>
        {
            foreach (var (language, expectedRace, expectedOccupation) in new[]
            {
                (Language.Spanish, "Elfo", "Ladrón"),
                (Language.French, "Elfe", "Voleur"),
                (Language.English, "Elf", "Thief"),
                ((Language)999, "Elf", "Thief")
            })
            {
                messages.CurrentLanguage = language;
                Assert.AreEqual(expectedRace, messages.TranslateRace(Race.Elf));
                Assert.AreEqual(language, messages.CurrentLanguage);
                Assert.AreEqual(expectedOccupation, messages.TranslateOccupation(Occupation.Thief));
                Assert.AreEqual(language, messages.CurrentLanguage);
                Assert.AreEqual(expectedRace, creation.TranslateRace(Race.Elf));
                Assert.AreEqual(language, messages.CurrentLanguage);
                Assert.AreEqual(expectedOccupation, creation.TranslateOccupation(Occupation.Thief));
                Assert.AreEqual(language, messages.CurrentLanguage);
            }
        });
        Assert.AreSame(dictionary, messages.LanguageDictionary);
        Assert.HasCount(1, dictionary);
        Assert.AreEqual("unchanged", dictionary["sentinel"]);
        Assert.AreEqual("", output);
    }
}
