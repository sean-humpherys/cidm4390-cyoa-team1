using CYOA;
using cidm4390_cyoa_team1;

namespace CYOA.Tests;

[TestClass]
[DoNotParallelize]
public class HealingPotionInitializationTests
{
    [TestMethod]
    public void NewPlayers_StartWithOneIndependentHealingPotion()
    {
        var first = new Player();
        Assert.AreEqual(1, first.healingPotions);
        first.healingPotions = 0;
        var second = new Player("New", Race.Elf, Occupation.Magician);
        Assert.AreEqual(1, second.healingPotions);
        Assert.AreEqual(0, first.healingPotions);
    }

    [TestMethod]
    public void ConstructorMaximumHealth_StaysAtStartingHealthAfterDamage()
    {
        foreach (var player in new[] { new Player(), new Player("Hero", Race.Dwarf, Occupation.Fighter) })
        {
            Assert.AreEqual(20, player.maxHealth);
            player.health = 7;
            Assert.AreEqual(20, player.maxHealth);
            Assert.AreEqual(7, player.health);
        }
    }

    private static Messages Load(string language = "English")
    {
        var messages = new Messages();
        messages.ReadDictionary(Path.Combine(AppContext.BaseDirectory, "MessageFiles", language + ".json"));
        return messages;
    }

    private static string Capture(string input, Action action)
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        using var reader = new StringReader(input);
        using var output = new StringWriter();
        try
        {
            Console.SetIn(reader);
            Console.SetOut(output);
            action();
            return output.ToString();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    [DataRow("Human", "Fighter", 6)]
    [DataRow("Elf", "Magician", 6)]
    [DataRow("Dwarf", "Archer", 6)]
    [DataRow("Halfling", "Thief", 3)]
    public void CharacterCreation_FinalizesMaximumHealthAfterRaceBonuses(string race, string occupation, int expectedHealth)
    {
        Player player = null!;
        var creation = new CharacterCreation(Load(), _ => 3);
        Capture($"{race}\nHero\n{occupation}\nroll\nroll\nroll\n2\n", () => player = creation.CreateCharacter());
        Assert.AreEqual(expectedHealth, player.health);
        Assert.AreEqual(expectedHealth, player.maxHealth);
        Assert.AreEqual(1, player.healingPotions);
        player.health = 1;
        Assert.AreEqual(expectedHealth, player.maxHealth);
    }

    [TestMethod]
    [DataRow("English", "Health: 13/20 | Healing potions: 1")]
    [DataRow("Spanish", "Salud: 13/20 | Pociones curativas: 1")]
    [DataRow("French", "Santé : 13/20 | Potions de soins : 1")]
    public void CombatMenu_DisplaysLocalizedStartingStatus(string language, string expectedStatus)
    {
        var messages = Load(language);
        var player = new Player { health = 13 };
        string output = Capture("n\nr\n", () => new Adventure(messages).StartAdventure(player));
        StringAssert.Contains(output, expectedStatus);
        StringAssert.Contains(output, messages.GetMessage(41));
        Assert.AreEqual(20, player.maxHealth);
        Assert.AreEqual(1, player.healingPotions);
    }

    [TestMethod]
    [DataRow("English", "Health: 13/20 | Healing potions: 1")]
    [DataRow("Spanish", "Salud: 13/20 | Pociones curativas: 1")]
    [DataRow("French", "Santé : 13/20 | Potions de soins : 1")]
    public void CombatMenu_RepeatsAccurateStatusAfterInvalidInputAndAttack(string language, string expectedStatus)
    {
        var messages = Load(language);
        // Guarantee a player miss and a successful defense, regardless of dragon stats.
        var player = new Player { health = 13, strength = 0, agility = 20 };
        string output = Capture("n\nx\na\nr\n", () => new Adventure(messages).StartAdventure(player));
        Assert.AreEqual(3, output.Split(expectedStatus).Length - 1);
        Assert.AreEqual(3, output.Split(messages.GetMessage(12)).Length - 1);
        StringAssert.Contains(output, messages.GetMessage(25));
        StringAssert.Contains(output, messages.GetMessage(41));
        StringAssert.Contains(output, player.weapon.asciiArt);
        Assert.AreEqual(13, player.health);
        Assert.AreEqual(20, player.maxHealth);
        Assert.AreEqual(1, player.healingPotions);
    }
}
