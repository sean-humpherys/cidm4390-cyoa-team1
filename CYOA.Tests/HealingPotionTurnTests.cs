using CYOA;
using cidm4390_cyoa_team1;

namespace CYOA.Tests;

[TestClass]
[DoNotParallelize]
public class HealingPotionTurnTests
{
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

    private static Player PlayerWithHealth(int health)
    {
        Player player = null!;
        Capture("Human\nHero\nThief\nroll\nroll\nroll\n2\n", () =>
            player = new CharacterCreation(Load(), sides => sides == 20 ? 15 : 3).CreateCharacter());
        player.health = health;
        return player;
    }


    private static void AssertPotionTurnOrder(string output, Messages messages, string confirmation, string nextStatus)
    {
        StringAssert.Contains(output, confirmation);
        string afterHealing = output[(output.IndexOf(confirmation) + confirmation.Length)..];
        string hit = messages.FormatMessage(36, "dragonName", "Dragon");
        string miss = messages.GetMessage(37);
        int responses = afterHealing.Split(miss).Length - 1 + afterHealing.Split(hit).Length - 1;
        Assert.AreEqual(1, responses);
        int responsePosition = afterHealing.Contains(hit) ? afterHealing.IndexOf(hit) : afterHealing.IndexOf(miss);
        StringAssert.Contains(afterHealing[(responsePosition + 1)..], nextStatus);
        Assert.DoesNotContain("{", afterHealing);
    }

    [TestMethod]
    public void EnglishMenu_OffersPotionBeforeFirstAttack()
    {
        string output = Capture("n\nr\n", () => new Adventure(Load()).StartAdventure(new Player()));
        StringAssert.Contains(output, "Enter 'p' to drink a healing potion");
    }

    [TestMethod]
    public void SpanishMenu_OffersPotionBeforeFirstAttack()
    {
        string output = Capture("n\nr\n", () => new Adventure(Load("Spanish")).StartAdventure(new Player()));
        StringAssert.Contains(output, "Escribe 'p' para beber una poción curativa");
    }

    [TestMethod]
    public void FrenchMenu_OffersPotionBeforeFirstAttack()
    {
        string output = Capture("n\nr\n", () => new Adventure(Load("French")).StartAdventure(new Player()));
        StringAssert.Contains(output, "Tapez 'p' pour boire une potion de soins");
    }

    [TestMethod]
    public void Drink_RestoresTenAndConsumesPotionBeforeEnemyResponse()
    {
        var messages = Load();
        var player = PlayerWithHealth(5);
        player.agility = 20;
        var weapon = player.weapon;
        string output = Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        Assert.AreEqual(15, player.health);
        Assert.AreEqual(18, player.maxHealth);
        Assert.AreEqual(0, player.healingPotions);
        Assert.AreSame(weapon, player.weapon);
        Assert.DoesNotContain(player.weapon.asciiArt, output);
        AssertPotionTurnOrder(output, messages,
            "You recover 10 health points. Health: 15/18. Healing potions: 0.",
            "Health: 15/18 | Healing potions: 0");
    }

    [TestMethod]
    public void Drink_NearMaximumRestoresOnlyMissingHealth()
    {
        var player = PlayerWithHealth(13);
        player.agility = 20;
        string output = Capture("n\np\nr\n", () => new Adventure(Load()).StartAdventure(player));
        Assert.AreEqual(18, player.health);
        Assert.AreEqual(18, player.maxHealth);
        Assert.AreEqual(0, player.healingPotions);
        StringAssert.Contains(output, "You recover 5 health points. Health: 18/18. Healing potions: 0.");
    }

    [TestMethod]
    public void FullHealth_PreservesPotionAndReturnsWithoutEnemyTurn()
    {
        var messages = Load();
        var player = PlayerWithHealth(18);
        string output = Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        Assert.AreEqual(18, player.health);
        Assert.AreEqual(1, player.healingPotions);
        StringAssert.Contains(output, "You are already at full health.");
        Assert.DoesNotContain(messages.GetMessage(37), output);
        Assert.DoesNotContain(messages.FormatMessage(36, "dragonName", "Dragon"), output);
        Assert.AreEqual(2, output.Split(messages.GetMessage(12)).Length - 1);
    }

    [TestMethod]
    public void EmptySupply_TakesPrecedenceAtFullHealthAndCostsNoTurn()
    {
        var messages = Load();
        var player = PlayerWithHealth(18);
        player.healingPotions = 0;
        string output = Capture("n\np\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        StringAssert.Contains(output, "You have no healing potions left.");
        Assert.AreEqual(2, output.Split("You have no healing potions left.").Length - 1);
        Assert.DoesNotContain("You are already at full health.", output);
        Assert.DoesNotContain(messages.GetMessage(37), output);
        Assert.DoesNotContain(messages.FormatMessage(36, "dragonName", "Dragon"), output);
        Assert.AreEqual(18, player.health);
        Assert.AreEqual(0, player.healingPotions);
    }

    [TestMethod]
    public void EnglishInvalidInput_ExplainsAllActionsWithoutEnemyTurn()
    {
        var messages = Load();
        var player = PlayerWithHealth(5);
        string output = Capture("n\n\n   \nx\nr\n", () => new Adventure(messages).StartAdventure(player));
        Assert.AreEqual(3, output.Split("Enter 'a' to attack, 'p' to drink a healing potion, or 'r' to retreat.").Length - 1);
        Assert.DoesNotContain(messages.GetMessage(37), output);
        Assert.DoesNotContain(messages.FormatMessage(36, "dragonName", "Dragon"), output);
        Assert.AreEqual(5, player.health);
        Assert.AreEqual(1, player.healingPotions);
    }

    [TestMethod]
    public void SpanishPotionOutcomes_AreLocalizedAndFormatted()
    {
        var messages = Load("Spanish");
        var player = PlayerWithHealth(13);
        player.agility = 20;
        string output = Capture("n\nx\np\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        AssertPotionTurnOrder(output, messages, "Recuperas 5 puntos de salud. Salud: 18/18. Pociones curativas: 0.", "Salud: 18/18 | Pociones curativas: 0");
        StringAssert.Contains(output, "No te quedan pociones curativas.");
        StringAssert.Contains(output, "Escribe 'a' para atacar, 'p' para beber una poción curativa o 'r' para retirarte.");
        var full = PlayerWithHealth(18);
        string fullOutput = Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(full));
        StringAssert.Contains(fullOutput, "Ya tienes la salud al máximo.");
        Assert.AreEqual(1, full.healingPotions);
    }

    [TestMethod]
    public void FrenchPotionOutcomes_AreLocalizedAndFormatted()
    {
        var messages = Load("French");
        var player = PlayerWithHealth(13);
        player.agility = 20;
        string output = Capture("n\nx\np\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        AssertPotionTurnOrder(output, messages, "Tu récupères 5 points de santé. Santé : 18/18. Potions de soins : 0.", "Santé : 18/18 | Potions de soins : 0");
        StringAssert.Contains(output, "Tu n'as plus de potions de soins.");
        StringAssert.Contains(output, "Tapez 'a' pour attaquer, 'p' pour boire une potion de soins ou 'r' pour battre en retraite.");
        var full = PlayerWithHealth(18);
        string fullOutput = Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(full));
        StringAssert.Contains(fullOutput, "Ta santé est déjà au maximum.");
        Assert.AreEqual(1, full.healingPotions);
    }

    [TestMethod]
    public void DefeatedPlayer_CannotDrinkOrRevive()
    {
        var messages = Load();
        var player = PlayerWithHealth(0);
        player.agility = 20;
        string output = Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        Assert.AreEqual(0, player.health);
        Assert.AreEqual(1, player.healingPotions);
        StringAssert.Contains(output, messages.GetMessage(43));
        Assert.DoesNotContain(messages.GetMessage(12), output);
        Assert.DoesNotContain("You recover", output);
    }

    [TestMethod]
    public void Drink_ExactTenReachesMaximum()
    {

        var player = PlayerWithHealth(8);
        player.agility = 20;
        string output = Capture("n\np\nr\n", () => new Adventure(Load()).StartAdventure(player));
        Assert.AreEqual(18, player.health);
        Assert.AreEqual(0, player.healingPotions);
        StringAssert.Contains(output, "You recover 10 health points. Health: 18/18.");
    }

    [TestMethod]
    public void Drink_OnePointBelowMaximumConsumesWholePotion()
    {

        var player = PlayerWithHealth(17);
        player.agility = 20;
        string output = Capture("n\np\nr\n", () => new Adventure(Load()).StartAdventure(player));
        Assert.AreEqual(18, player.health);
        Assert.AreEqual(0, player.healingPotions);
        StringAssert.Contains(output, "You recover 1 health points. Health: 18/18.");
    }

    [TestMethod]
    public void Drink_AcceptsUppercaseAndSurroundingSpaces()
    {

        var player = PlayerWithHealth(5);
        player.agility = 20;
        string output = Capture(" N \n P \n P \n R \n", () => new Adventure(Load()).StartAdventure(player));
        Assert.AreEqual(15, player.health);
        Assert.AreEqual(0, player.healingPotions);
        StringAssert.Contains(output, "You have no healing potions left.");
        Assert.AreEqual(3, output.Split("Enter 'p' to drink a healing potion").Length - 1);
    }

    [TestMethod]
    public void EmptySupply_WhileInjuredDoesNotHealOrAttack()
    {

        var messages = Load();
        var player = PlayerWithHealth(5);
        player.healingPotions = 0;
        string output = Capture("n\np\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        Assert.AreEqual(5, player.health);
        Assert.AreEqual(0, player.healingPotions);
        Assert.AreEqual(2, output.Split("You have no healing potions left.").Length - 1);
        Assert.DoesNotContain(messages.GetMessage(37), output);
        Assert.DoesNotContain(messages.FormatMessage(36, "dragonName", "Dragon"), output);
    }

    [TestMethod]
    public void HealedPlayer_ReceivesNormalUnarmoredDamage()
    {

        var messages = Load();
        var player = PlayerWithHealth(5);
        player.agility = 20;
        Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        player.agility = 0;
        var dragon = new Dragon("Dragon") { strength = 20 };
        var rolls = new Queue<int>(new[] { 1, 20, 7, 1 });
        string output = Capture("", () => new Combat(player, dragon, messages, _ => rolls.Dequeue()).DragonAttacksPlayerSequence());
        Assert.AreEqual(8, player.health);
        Assert.AreEqual(18, player.maxHealth);
        Assert.AreEqual(0, player.healingPotions);
        StringAssert.Contains(output, "delivers 7 damage points. Your health points is now 8.");
    }

    [TestMethod]
    public void HealedPlayer_RetainsMagicalArmorProtection()
    {

        var messages = Load();
        var player = PlayerWithHealth(5);
        Capture("1\n", () => new Merchant(messages).Encounter(player));
        player.agility = 20;
        Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        player.agility = 0;
        var dragon = new Dragon("Dragon") { strength = 20 };
        var rolls = new Queue<int>(new[] { 1, 20, 7, 1 });
        string output = Capture("", () => new Combat(player, dragon, messages, _ => rolls.Dequeue()).DragonAttacksPlayerSequence());
        Assert.AreEqual(10, player.health);
        Assert.AreEqual(2, player.armorProtection);
        Assert.AreEqual(18, player.maxHealth);
        Assert.IsTrue(player.merchantGiftClaimed);
        StringAssert.Contains(output, "delivers 5 damage points. Your health points is now 10.");
    }

    [TestMethod]
    public void HealedPlayer_CanBeDefeatedAndCannotDrinkOnReentry()
    {

        var messages = Load();
        var player = PlayerWithHealth(1);
        player.agility = 20;
        Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        Assert.AreEqual(11, player.health);
        player.agility = 0;
        var dragon = new Dragon("Dragon") { strength = 20 };
        var rolls = new Queue<int>(new[] { 1, 20, 12 });
        string damage = Capture("", () => new Combat(player, dragon, messages, _ => rolls.Dequeue()).DragonAttacksPlayerSequence());
        Assert.AreEqual(-1, player.health);
        StringAssert.Contains(damage, messages.GetMessage(40));
        string ending = Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        StringAssert.Contains(ending, messages.GetMessage(43));
        Assert.DoesNotContain(messages.GetMessage(12), ending);
        Assert.DoesNotContain("You recover", ending);
        Assert.AreEqual(-1, player.health);
        Assert.AreEqual(0, player.healingPotions);
    }

    [TestMethod]
    public void UnusedPotion_DoesNotPreventLethalCombat()
    {

        var messages = Load();
        var player = PlayerWithHealth(1);
        player.agility = 0;
        var dragon = new Dragon("Dragon") { strength = 20 };
        var rolls = new Queue<int>(new[] { 1, 20, 1 });
        string damage = Capture("", () => new Combat(player, dragon, messages, _ => rolls.Dequeue()).DragonAttacksPlayerSequence());
        Assert.AreEqual(0, player.health);
        Assert.AreEqual(1, player.healingPotions);
        StringAssert.Contains(damage, messages.GetMessage(40));
        string ending = Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        Assert.AreEqual(1, player.healingPotions);
        Assert.DoesNotContain(messages.GetMessage(12), ending);
    }

    [TestMethod]
    public void HealedPlayer_DragonMissLeavesHealthUnchanged()
    {

        var messages = Load();
        var player = PlayerWithHealth(5);
        player.agility = 20;
        Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        var dragon = new Dragon("Dragon") { strength = 10 };
        var rolls = new Queue<int>(new[] { 11 });
        string output = Capture("", () => new Combat(player, dragon, messages, _ => rolls.Dequeue()).DragonAttacksPlayerSequence());
        Assert.AreEqual(15, player.health);
        StringAssert.Contains(output, messages.GetMessage(37));
    }

    [TestMethod]
    public void HealedPlayer_SuccessfulDefenseLeavesHealthUnchanged()
    {

        var messages = Load();
        var player = PlayerWithHealth(5);
        player.agility = 20;
        Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        var dragon = new Dragon("Dragon") { strength = 20 };
        var rolls = new Queue<int>(new[] { 1, 20 });
        string output = Capture("", () => new Combat(player, dragon, messages, _ => rolls.Dequeue()).DragonAttacksPlayerSequence());
        Assert.AreEqual(15, player.health);
        StringAssert.Contains(output, messages.GetMessage(38));
    }

    [TestMethod]
    public void PotionCommand_OutsideCombatDoesNotHealOrConsume()
    {

        var messages = Load();
        var player = PlayerWithHealth(5);
        string output = Capture("p\ne\n", () => new Adventure(messages).StartAdventure(player));
        StringAssert.Contains(output, messages.GetMessage(57));
        Assert.AreEqual(5, player.health);
        Assert.AreEqual(1, player.healingPotions);
        string merchant = Capture("p\n3\n", () => new Merchant(messages).Encounter(player));
        StringAssert.Contains(merchant, messages.GetMessage(63));
        Assert.AreEqual(5, player.health);
        Assert.AreEqual(1, player.healingPotions);
    }

    [TestMethod]
    public void LethalPotionResponse_EndsSameAdventureWithoutReadingAnotherAction()
    {
        var messages = Load();
        var player = PlayerWithHealth(1);
        player.agility = 0;
        var rolls = new Queue<int>(new[] { 1, 20, 12 });
        string? nextInput = null;
        string output = Capture("n\np\np\na\nr\ne\n", () =>
        {
            new Adventure(messages, _ => rolls.Dequeue()).StartAdventure(player);
            nextInput = Console.ReadLine();
        });

        Assert.AreEqual(-1, player.health);
        Assert.AreEqual(18, player.maxHealth);
        Assert.AreEqual(0, player.healingPotions);
        Assert.AreEqual("p", nextInput, "Defeat must return without reading the next queued action.");
        string confirmation = "You recover 10 health points. Health: 11/18. Healing potions: 0.";
        StringAssert.Contains(output, confirmation);
        string afterHealing = output[(output.IndexOf(confirmation) + confirmation.Length)..];
        StringAssert.Contains(afterHealing, "delivers 12 damage points. Your health points is now -1.");
        StringAssert.Contains(afterHealing, messages.GetMessage(40));
        StringAssert.Contains(afterHealing, messages.GetMessage(43));
        Assert.DoesNotContain(messages.GetMessage(12), afterHealing);
        Assert.DoesNotContain(messages.GetMessage(7), afterHealing);
        Assert.DoesNotContain(messages.GetMessage(69), output);
        Assert.DoesNotContain(player.weapon.asciiArt, output);
        Assert.AreEqual(1, output.Split(messages.GetMessage(12)).Length - 1);
        Assert.AreEqual(1, output.Split(messages.FormatMessage(36, "dragonName", "Dragon")).Length - 1);
    }

    [TestMethod]
    [DataRow("English", "1", 2, 6, 10)]
    [DataRow("English", "2", 0, 10, 7)]
    [DataRow("Spanish", "1", 2, 6, 10)]
    [DataRow("Spanish", "2", 0, 10, 7)]
    [DataRow("French", "1", 2, 6, 10)]
    [DataRow("French", "2", 0, 10, 7)]
    public void MerchantJourney_RetainsPotionAndUpgradeThroughHealingAndAttack(string language, string upgrade, int protection, int weaponMaximum, int expectedHealth)
    {
        var messages = Load(language);
        var player = PlayerWithHealth(5);
        player.strength = 0;
        player.agility = 0;
        var weapon = player.weapon;
        string appearance = weapon.asciiArt;
        // Potion response: hit for 7. Player attack: guaranteed miss.
        // Next enemy response: hit for 1 (fully absorbed by magical armor).
        var rolls = new Queue<int>(new[] { 1, 20, 7, 1, 20, 1, 20, 1, 1 });
        string output = Capture($"s\n3\ns\n{upgrade}\ns\nn\np\np\na\nr\n", () =>
            new Adventure(messages, _ => rolls.Dequeue()).StartAdventure(player));

        Assert.AreSame(weapon, player.weapon);
        Assert.AreEqual("dagger", weapon.type);
        Assert.AreEqual(appearance, weapon.asciiArt);
        Assert.AreEqual(weaponMaximum, weapon.maxDamage);
        Assert.AreEqual(protection, player.armorProtection);
        Assert.IsTrue(player.merchantGiftClaimed);
        Assert.AreEqual(18, player.maxHealth);
        Assert.AreEqual(0, player.healingPotions);
        Assert.AreEqual(expectedHealth, player.health);
        Assert.AreEqual(0, player.strength);
        Assert.AreEqual(0, player.agility);
        StringAssert.Contains(output, messages.GetMessage(62));
        StringAssert.Contains(output, messages.GetMessage(upgrade == "1" ? 65 : 61));
        StringAssert.Contains(output, messages.GetMessage(64));
        StringAssert.Contains(output, messages.GetMessage(69));
        StringAssert.Contains(output, messages.GetMessage(41));
        StringAssert.Contains(output, appearance);
        string confirmation = messages.FormatMessage(67, new Dictionary<string, string>
            { ["restored"] = "10", ["health"] = "15", ["maxHealth"] = "18", ["potions"] = "0" });
        StringAssert.Contains(output, confirmation);
        string afterHealing = output[(output.IndexOf(confirmation) + confirmation.Length)..];
        StringAssert.Contains(afterHealing, messages.FormatMessage(36, "dragonName", "Dragon"));
        Assert.AreEqual(4, output.Split(messages.GetMessage(7)).Length - 1);
        Assert.AreEqual(4, output.Split(messages.GetMessage(12)).Length - 1);
        Assert.AreEqual(2, output.Split(messages.FormatMessage(36, "dragonName", "Dragon")).Length - 1);
        Assert.DoesNotContain("{", output);
    }

    [TestMethod]
    [DataRow("1", 2, 6)]
    [DataRow("2", 0, 10)]
    public void MerchantVisits_DoNotReplenishConsumedPotionAndNewCharacterStartsFresh(string upgrade, int protection, int weaponMaximum)
    {
        var messages = Load();
        var player = PlayerWithHealth(5);
        player.agility = 20;
        Capture("n\np\nr\n", () => new Adventure(messages).StartAdventure(player));
        Assert.AreEqual(0, player.healingPotions);
        var merchant = new Merchant(messages);
        Capture("3\n", () => merchant.Encounter(player));
        Assert.AreEqual(0, player.healingPotions);
        Assert.AreEqual(18, player.maxHealth);
        Capture(upgrade + "\n", () => merchant.Encounter(player));
        Assert.AreEqual(0, player.healingPotions);
        Assert.AreEqual(18, player.maxHealth);
        string repeat = Capture(upgrade == "1" ? "2\n" : "1\n", () => merchant.Encounter(player));
        StringAssert.Contains(repeat, messages.GetMessage(64));
        Assert.AreEqual(0, player.healingPotions);
        Assert.AreEqual(18, player.maxHealth);
        Assert.AreEqual(protection, player.armorProtection);
        Assert.AreEqual(weaponMaximum, player.weapon.maxDamage);
        var fresh = PlayerWithHealth(18);
        Assert.AreEqual(1, fresh.healingPotions);
        Assert.AreEqual(18, fresh.maxHealth);
        Assert.AreEqual(18, fresh.health);
        Assert.IsFalse(fresh.merchantGiftClaimed);
        Assert.AreEqual(0, fresh.armorProtection);
        Assert.AreEqual(6, fresh.weapon.maxDamage);
        Assert.AreEqual(0, player.healingPotions);
    }
}
