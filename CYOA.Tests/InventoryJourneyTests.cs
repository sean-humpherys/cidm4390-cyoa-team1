using CYOA;
using cidm4390_cyoa_team1;

namespace CYOA.Tests;

[TestClass]
[DoNotParallelize]
public class InventoryJourneyTests
{
    private static Messages Load(string language)
    {
        var messages = new Messages();
        messages.ReadDictionary(Path.Combine(AppContext.BaseDirectory, "MessageFiles", language + ".json"));
        return messages;
    }

    private static Player CreatePlayer() => new Player("Keeper", Race.Elf, Occupation.Thief)
    {
        health = 5, strength = 0, agility = 0,
        weapon = new Weapon("dagger", 6, "dagger-art")
    };

    private static string Run(Messages messages, Player player, string input, Func<int, int>? roll = null)
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        using var reader = new StringReader(input);
        using var output = new StringWriter();
        try
        {
            Console.SetIn(reader);
            Console.SetOut(output);
            new Adventure(messages, roll ?? (_ => throw new AssertFailedException("This journey must not roll dice.")))
                .StartAdventure(player);
            return output.ToString();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    // Literal labels and translations come from the approved specification.
    private static string[] ExpectedInventory(string language, int damage, int protection, int potions) => language switch
    {
        "Spanish" => new[] { "Arma: daga", $"Daño máximo del arma: {damage}",
            $"Protección de la armadura: {protection}", $"Pociones curativas restantes: {potions}" },
        "French" => new[] { "Arme : dague", $"Dégâts maximums de l’arme : {damage}",
            $"Protection de l’armure : {protection}", $"Potions de soins restantes : {potions}" },
        _ => new[] { "Weapon: dagger", $"Weapon maximum damage: {damage}",
            $"Armor protection: {protection}", $"Healing potions remaining: {potions}" }
    };

    private static string[][] Inventories(string output, Messages messages) => output
        .Split(messages.GetMessage(7))
        .Select(block => block.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        .Where(lines => lines.Length > 0 && (lines[0].StartsWith("Weapon: ", StringComparison.Ordinal)
            || lines[0].StartsWith("Arma: ", StringComparison.Ordinal)
            || lines[0].StartsWith("Arme : ", StringComparison.Ordinal)))
        .ToArray();

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void MagicalWeaponJourney_InventoryShowsUpgradeOnceAcrossRepeatedInspectionsAndVisits(string language)
    {
        var messages = Load(language);
        var player = CreatePlayer();
        var weapon = player.weapon;
        string output = Run(messages, player, "i\ns\n2\ni\ni\ns\ni\ne\n");
        var inventories = Inventories(output, messages);
        Assert.HasCount(4, inventories);
        CollectionAssert.AreEqual(ExpectedInventory(language, 6, 0, 1), inventories[0]);
        foreach (var inventory in inventories.Skip(1))
            CollectionAssert.AreEqual(ExpectedInventory(language, 10, 0, 1), inventory);
        Assert.HasCount(2, output.Split(messages.GetMessage(61)));
        StringAssert.Contains(output, messages.GetMessage(64));
        StringAssert.Contains(output, messages.GetMessage(4));
        Assert.AreSame(weapon, player.weapon);
        Assert.AreEqual(("dagger", 10, "dagger-art"), (weapon.type, weapon.maxDamage, weapon.asciiArt));
        Assert.AreEqual(("Keeper", Race.Elf, Occupation.Thief, 5, 20, 0, 0, 0, 1, true),
            (player.name, player.race, player.occupation, player.health, player.maxHealth,
                player.strength, player.agility, player.armorProtection, player.healingPotions, player.merchantGiftClaimed));
        Assert.DoesNotContain("{", output);
    }
    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void MagicalArmorJourney_LeavingKeepsGiftAvailableAndInventoryShowsProtectionOnce(string language)
    {
        var messages = Load(language);
        var player = CreatePlayer();
        var weapon = player.weapon;
        string output = Run(messages, player, "i\ns\n3\ni\ns\n1\ni\ns\ni\ne\n");
        var inventories = Inventories(output, messages);
        Assert.HasCount(4, inventories);
        CollectionAssert.AreEqual(ExpectedInventory(language, 6, 0, 1), inventories[0]);
        CollectionAssert.AreEqual(ExpectedInventory(language, 6, 0, 1), inventories[1]);
        CollectionAssert.AreEqual(ExpectedInventory(language, 6, 2, 1), inventories[2]);
        CollectionAssert.AreEqual(ExpectedInventory(language, 6, 2, 1), inventories[3]);
        StringAssert.Contains(output, messages.GetMessage(62));
        Assert.HasCount(2, output.Split(messages.GetMessage(65)));
        StringAssert.Contains(output, messages.GetMessage(64));
        StringAssert.Contains(output, messages.GetMessage(4));
        Assert.AreSame(weapon, player.weapon);
        Assert.AreEqual(("dagger", 6, "dagger-art"), (weapon.type, weapon.maxDamage, weapon.asciiArt));
        Assert.AreEqual((5, 20, 0, 0, 2, 1, true), (player.health, player.maxHealth,
            player.strength, player.agility, player.armorProtection, player.healingPotions, player.merchantGiftClaimed));
        Assert.DoesNotContain("{", output);
    }
    [TestMethod]
    [DataRow("English", "1", 6, 2, 15)]
    [DataRow("English", "2", 10, 0, 13)]
    [DataRow("Spanish", "1", 6, 2, 15)]
    [DataRow("Spanish", "2", 10, 0, 13)]
    [DataRow("French", "1", 6, 2, 15)]
    [DataRow("French", "2", 10, 0, 13)]
    public void ConsumedPotionJourney_NewAdventureSessionRetainsInventoryAndMerchantGift(
        string language, string gift, int maximumDamage, int protection, int remainingHealth)
    {
        var messages = Load(language);
        var player = CreatePlayer();
        player.health = 15;
        var weapon = player.weapon;
        // Restore only 5 to reach 20, then receive a normal hit for 7 before armor.
        var rolls = new Queue<int>(new[] { 1, 20, 7, 1 });
        string first = Run(messages, player, $"i\ns\n{gift}\ni\nn\np\np\nr\n", _ => rolls.Dequeue());
        var initialInventories = Inventories(first, messages);
        Assert.HasCount(2, initialInventories);
        CollectionAssert.AreEqual(ExpectedInventory(language, 6, 0, 1), initialInventories[0]);
        CollectionAssert.AreEqual(ExpectedInventory(language, maximumDamage, protection, 1), initialInventories[1]);
        string healing = messages.FormatMessage(67, new Dictionary<string, string>
        {
            ["restored"] = "5", ["health"] = "20", ["maxHealth"] = "20", ["potions"] = "0"
        });
        string damage = messages.FormatMessage(39, new Dictionary<string, string>
        {
            ["damage"] = protection == 2 ? "5" : "7", ["healthPoints"] = remainingHealth.ToString()
        });
        StringAssert.Contains(first, healing);
        StringAssert.Contains(first[(first.IndexOf(healing, StringComparison.Ordinal) + healing.Length)..], damage);
        Assert.HasCount(2, first.Split(messages.FormatMessage(36, "dragonName", "Dragon")));
        StringAssert.Contains(first, messages.GetMessage(69));
        StringAssert.Contains(first, messages.GetMessage(41));
        Assert.IsEmpty(rolls);
        Assert.AreEqual((remainingHealth, 20, 0, protection, true),
            (player.health, player.maxHealth, player.healingPotions, player.armorProtection, player.merchantGiftClaimed));

        // Retreat ends an adventure; inspect the same player in a separate invocation.
        string beforeSecondSession = System.Text.Json.JsonSerializer.Serialize(player);
        string second = Run(messages, player, "i\ns\ni\ni\ne\n");
        var retainedInventories = Inventories(second, messages);
        Assert.HasCount(3, retainedInventories);
        foreach (var inventory in retainedInventories)
            CollectionAssert.AreEqual(ExpectedInventory(language, maximumDamage, protection, 0), inventory);
        StringAssert.Contains(second, messages.GetMessage(64));
        StringAssert.Contains(second, messages.GetMessage(4));
        Assert.AreEqual(beforeSecondSession, System.Text.Json.JsonSerializer.Serialize(player));
        Assert.AreSame(weapon, player.weapon);
        Assert.AreEqual(("dagger", maximumDamage, "dagger-art"), (weapon.type, weapon.maxDamage, weapon.asciiArt));
        Assert.DoesNotContain("{", first + second);
    }
    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void MerchantInteraction_RejectsInventoryWithoutChangingStateAndReturnsNormally(string language)
    {
        var messages = Load(language);
        var player = CreatePlayer();
        var weapon = player.weapon;
        string before = System.Text.Json.JsonSerializer.Serialize(player);
        string output = Run(messages, player, "i\ns\ni\n3\ni\ne\n");
        var inventories = Inventories(output, messages);
        Assert.HasCount(2, inventories);
        foreach (var inventory in inventories)
            CollectionAssert.AreEqual(ExpectedInventory(language, 6, 0, 1), inventory);
        string merchantInteraction = output[(output.IndexOf(messages.GetMessage(59), StringComparison.Ordinal))..];
        merchantInteraction = merchantInteraction[..merchantInteraction.IndexOf(messages.GetMessage(7), StringComparison.Ordinal)];
        StringAssert.Contains(merchantInteraction, messages.GetMessage(63));
        StringAssert.Contains(merchantInteraction, messages.GetMessage(62));
        Assert.HasCount(3, merchantInteraction.Split(messages.GetMessage(60)));
        foreach (string inventoryLine in ExpectedInventory(language, 6, 0, 1))
            Assert.DoesNotContain(inventoryLine, merchantInteraction);
        StringAssert.Contains(output, messages.GetMessage(4));
        Assert.AreEqual(before, System.Text.Json.JsonSerializer.Serialize(player));
        Assert.AreSame(weapon, player.weapon);
    }
    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void DragonCombat_RejectsInventoryWithoutStateChangesOrEnemyTurns(string language)
    {
        var messages = Load(language);
        var player = CreatePlayer();
        var weapon = player.weapon;
        string before = System.Text.Json.JsonSerializer.Serialize(player);
        string output = Run(messages, player, "i\nn\ni\ni\nr\n");
        var inventories = Inventories(output, messages);
        Assert.HasCount(1, inventories);
        CollectionAssert.AreEqual(ExpectedInventory(language, 6, 0, 1), inventories[0]);
        string combatInteraction = output[output.IndexOf(messages.GetMessage(12), StringComparison.Ordinal)..];
        Assert.HasCount(3, combatInteraction.Split(messages.GetMessage(25)));
        Assert.HasCount(4, combatInteraction.Split(messages.GetMessage(12)));
        foreach (string inventoryLine in ExpectedInventory(language, 6, 0, 1))
            Assert.DoesNotContain(inventoryLine, combatInteraction);
        StringAssert.Contains(output, messages.GetMessage(9));
        StringAssert.Contains(output, messages.GetMessage(41));
        Assert.DoesNotContain(messages.GetMessage(4), output);
        Assert.AreEqual(before, System.Text.Json.JsonSerializer.Serialize(player));
        Assert.AreSame(weapon, player.weapon);
    }
    [TestMethod]
    [DataRow("English", "1", 6, 2, 15)]
    [DataRow("English", "2", 10, 0, 13)]
    [DataRow("Spanish", "1", 6, 2, 15)]
    [DataRow("Spanish", "2", 10, 0, 13)]
    [DataRow("French", "1", 6, 2, 15)]
    [DataRow("French", "2", 10, 0, 13)]
    public void InspectionThenNorth_PreservesNormalAttackAndArmorDamage(
        string language, string gift, int maximumDamage, int protection, int remainingHealth)
    {
        var messages = Load(language);
        var player = CreatePlayer();
        player.health = 20;
        var weapon = player.weapon;
        // Player misses; dragon hits for 7 before armor. Inspections consume no rolls.
        var rolls = new Queue<int>(new[] { 20, 1, 20, 7, 1 });
        string output = Run(messages, player, $"i\ns\n{gift}\ni\nn\na\nr\n", _ => rolls.Dequeue());
        var inventories = Inventories(output, messages);
        Assert.HasCount(2, inventories);
        CollectionAssert.AreEqual(ExpectedInventory(language, 6, 0, 1), inventories[0]);
        CollectionAssert.AreEqual(ExpectedInventory(language, maximumDamage, protection, 1), inventories[1]);
        StringAssert.Contains(output, messages.GetMessage(9));
        StringAssert.Contains(output, messages.FormatMessage(35, "dragonName", "Dragon"));
        StringAssert.Contains(output, "dagger-art");
        StringAssert.Contains(output, messages.FormatMessage(39, new Dictionary<string, string>
        {
            ["damage"] = protection == 2 ? "5" : "7", ["healthPoints"] = remainingHealth.ToString()
        }));
        Assert.HasCount(2, output.Split(messages.FormatMessage(36, "dragonName", "Dragon")));
        StringAssert.Contains(output, messages.GetMessage(41));
        Assert.IsEmpty(rolls);
        Assert.AreSame(weapon, player.weapon);
        Assert.AreEqual((maximumDamage, protection, 1, remainingHealth, 20, true),
            (weapon.maxDamage, player.armorProtection, player.healingPotions,
                player.health, player.maxHealth, player.merchantGiftClaimed));
        Assert.DoesNotContain("{", output);
    }
}
