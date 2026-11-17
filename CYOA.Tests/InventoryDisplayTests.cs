using CYOA;
using cidm4390_cyoa_team1;

namespace CYOA.Tests;

[TestClass]
[DoNotParallelize]
public class InventoryDisplayTests
{
    private static Messages Load(string language = "English")
    {
        var messages = new Messages();
        messages.ReadDictionary(Path.Combine(AppContext.BaseDirectory, "MessageFiles", language + ".json"));
        return messages;
    }

    private static string Inspect(Messages messages, Player player, string input = "i\ne\n")
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        using var reader = new StringReader(input);
        using var output = new StringWriter();
        try
        {
            Console.SetIn(reader);
            Console.SetOut(output);
            new Adventure(messages, _ => throw new AssertFailedException("Inspection must not roll dice.")).StartAdventure(player);
            return output.ToString();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    private static string[] Summary(string output, Messages messages)
    {
        string menu = messages.GetMessage(7);
        string afterMenu = output[(output.IndexOf(menu, StringComparison.Ordinal) + menu.Length)..];
        return afterMenu[..afterMenu.IndexOf(menu, StringComparison.Ordinal)]
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }

    [TestMethod]
    public void EnglishInspection_DisplaysExactlyFourValuesAndReturnsToMenu()
    {
        var messages = Load();
        string output = Inspect(messages, new Player());
        CollectionAssert.AreEqual(new[] { "Weapon: long sword", "Weapon maximum damage: 12",
            "Armor protection: 0", "Healing potions remaining: 1" }, Summary(output, messages));
        StringAssert.Contains(output, "Adventure ends. Bye.");
    }
    [TestMethod]
    public void EnglishDirectionGuidance_OffersInventoryAndExplainsInvalidInput()
    {
        string output = Inspect(Load(), new Player(), "x\n\ni\ne\n");
        StringAssert.Contains(output, "Enter 'i' to view inventory.");
        StringAssert.Contains(output, "Invalid choice. Enter 'n', 's', 'i', or 'e'.");
    }
    [TestMethod]
    public void SpanishInspection_LocalizesValuesAndDirectionGuidance()
    {
        var messages = Load("Spanish");
        string output = Inspect(messages, new Player(), "x\ni\ne\n");
        StringAssert.Contains(output, "Escribe 'i' para ver el inventario.");
        StringAssert.Contains(output, "Opción no válida. Escribe 'n', 's', 'i' o 'e'.");
        output = Inspect(messages, new Player());
        CollectionAssert.AreEqual(new[] { "Arma: espada larga", "Daño máximo del arma: 12",
            "Protección de la armadura: 0", "Pociones curativas restantes: 1" }, Summary(output, messages));
    }
    [TestMethod]
    public void FrenchInspection_LocalizesValuesAndDirectionGuidance()
    {
        var messages = Load("French");
        string output = Inspect(messages, new Player(), "x\ni\ne\n");
        StringAssert.Contains(output, "Tapez 'i' pour afficher l’inventaire.");
        StringAssert.Contains(output, "Choix invalide. Tapez 'n', 's', 'i' ou 'e'.");
        output = Inspect(messages, new Player());
        CollectionAssert.AreEqual(new[] { "Arme : épée longue", "Dégâts maximums de l’arme : 12",
            "Protection de l’armure : 0", "Potions de soins restantes : 1" }, Summary(output, messages));
    }
    [TestMethod]
    [DataRow("English", "long sword", "long sword")]
    [DataRow("English", "lightning bolt spell", "lightning bolt spell")]
    [DataRow("English", "dagger", "dagger")]
    [DataRow("English", "long bow", "long bow")]
    [DataRow("Spanish", "long sword", "espada larga")]
    [DataRow("Spanish", "lightning bolt spell", "hechizo de rayo")]
    [DataRow("Spanish", "dagger", "daga")]
    [DataRow("Spanish", "long bow", "arco largo")]
    [DataRow("French", "long sword", "épée longue")]
    [DataRow("French", "lightning bolt spell", "sort de foudre")]
    [DataRow("French", "dagger", "dague")]
    [DataRow("French", "long bow", "arc long")]
    public void RepeatedNormalizedInspection_ShowsCurrentValuesAndPreservesAllState(
        string language, string weaponType, string translatedWeapon)
    {
        var messages = Load(language);
        var player = new Player("Reader", Race.Elf, Occupation.Archer)
        {
            health = 7, strength = 8, agility = 9, armorProtection = 2,
            healingPotions = 3, merchantGiftClaimed = true,
            weapon = new Weapon(weaponType, 17, "NO ART")
        };
        var weapon = player.weapon;
        string output = Inspect(messages, player, " I \ni\ne\n");
        string[] expected = language switch
        {
            "Spanish" => new[] { "Arma: " + translatedWeapon, "Daño máximo del arma: 17",
                "Protección de la armadura: 2", "Pociones curativas restantes: 3" },
            "French" => new[] { "Arme : " + translatedWeapon, "Dégâts maximums de l’arme : 17",
                "Protection de l’armure : 2", "Potions de soins restantes : 3" },
            _ => new[] { "Weapon: " + translatedWeapon, "Weapon maximum damage: 17",
                "Armor protection: 2", "Healing potions remaining: 3" }
        };
        CollectionAssert.AreEqual(expected, Summary(output, messages));
        // After removing the first menu, the next summary must be identical.
        string menu = messages.GetMessage(7);
        string remainder = output[(output.IndexOf(menu, StringComparison.Ordinal) + menu.Length)..];
        CollectionAssert.AreEqual(expected, Summary(remainder, messages));
        Assert.HasCount(4, output.Split(menu)); // Three prompts: initial and after each inspection.
        Assert.AreSame(weapon, player.weapon);
        Assert.AreEqual(("Reader", Race.Elf, Occupation.Archer, 7, 20, 8, 9, 2, 3, true),
            (player.name, player.race, player.occupation, player.health, player.maxHealth,
                player.strength, player.agility, player.armorProtection, player.healingPotions, player.merchantGiftClaimed));
        Assert.AreEqual((weaponType, 17, "NO ART"), (weapon.type, weapon.maxDamage, weapon.asciiArt));
        Assert.DoesNotContain("NO ART", output);
        Assert.DoesNotContain("{", output);
    }
    [TestMethod]
    [DataRow("English", "Healing potions remaining: 0")]
    [DataRow("Spanish", "Pociones curativas restantes: 0")]
    [DataRow("French", "Potions de soins restantes : 0")]
    public void EmptyPotions_AreDisplayedAsZeroWithoutHealing(string language, string expected)
    {
        var player = new Player { healingPotions = 0, health = 5 };
        var messages = Load(language);
        string output = Inspect(messages, player);
        Assert.AreEqual(expected, Summary(output, messages)[3]);
        Assert.AreEqual(0, player.healingPotions);
        Assert.AreEqual(5, player.health);
    }
    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void InvalidAndBlankInput_RetryWithoutChangingPlayerState(string language)
    {
        var messages = Load(language);
        var player = new Player { health = 5, healingPotions = 0, armorProtection = 2 };
        var weapon = player.weapon;
        string before = System.Text.Json.JsonSerializer.Serialize(player);
        string output = Inspect(messages, player, "x\n\n I \ne\n");
        Assert.HasCount(3, output.Split(messages.GetMessage(57)));
        Assert.HasCount(5, output.Split(messages.GetMessage(7)));
        Assert.AreEqual(before, System.Text.Json.JsonSerializer.Serialize(player));
        Assert.AreSame(weapon, player.weapon);
    }
}
