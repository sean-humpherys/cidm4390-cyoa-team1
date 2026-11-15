using CYOA;
using cidm4390_cyoa_team1;

namespace CYOA.Tests;

[TestClass]
[DoNotParallelize]
public class MerchantTests
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
        using var output = new StringWriter();
        using var reader = new StringReader(input);
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
    public void Claim_EnchantsCurrentWeaponOnce()
    {
        var player = new Player();
        var weapon = player.weapon;
        var messages = Load();
        Assert.IsFalse(player.merchantGiftClaimed);
        Capture("2\n", () => new Merchant(messages).Encounter(player));
        Assert.AreSame(weapon, player.weapon);
        Assert.AreEqual(16, weapon.maxDamage);
        Assert.IsTrue(player.merchantGiftClaimed);
        string repeat = Capture("", () => new Merchant(messages).Encounter(player));
        StringAssert.Contains(repeat, messages.GetMessage(64));
        Assert.AreEqual(16, weapon.maxDamage);
        Assert.IsFalse(new Player().merchantGiftClaimed);
        Assert.IsFalse(new Player("New", Race.Elf, Occupation.Archer).merchantGiftClaimed);
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void LocalizedEncounter_LeavingValidationClaimAndRepeat(string language)
    {
        var messages = Load(language);
        var player = new Player();
        var merchant = new Merchant(messages);
        string leave = Capture("3\n", () => merchant.Encounter(player));
        StringAssert.Contains(leave, messages.GetMessage(62));
        Assert.IsFalse(player.merchantGiftClaimed);
        Assert.AreEqual(12, player.weapon.maxDamage);
        string claim = Capture("4\n\nx\n2\n", () => merchant.Encounter(player));
        foreach (int id in new[] { 59, 60, 61, 63 })
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(messages.GetMessage(id)));
            StringAssert.Contains(claim, messages.GetMessage(id));
        }
        Assert.AreEqual(3, claim.Split(messages.GetMessage(63)).Length - 1);
        StringAssert.Contains(messages.GetMessage(60), "2 =");
        StringAssert.Contains(messages.GetMessage(60), "3 =");
        StringAssert.Contains(messages.GetMessage(60), "+4");
        StringAssert.Contains(messages.GetMessage(60), "1 =");
        Assert.IsTrue(player.merchantGiftClaimed);
        Assert.AreEqual(16, player.weapon.maxDamage);
        StringAssert.Contains(Capture("", () => merchant.Encounter(player)), messages.GetMessage(64));
        Assert.AreEqual(16, player.weapon.maxDamage);
    }

    [TestMethod]
    public void EndOfInput_LeavesWithoutClaiming()
    {
        var player = new Player();
        var messages = Load();
        string output = Capture("", () => new Merchant(messages).Encounter(player));
        StringAssert.Contains(output, messages.GetMessage(62));
        Assert.IsFalse(player.merchantGiftClaimed);
        Assert.AreEqual(12, player.weapon.maxDamage);
    }

    [TestMethod]
    [DataRow("Fighter", "long sword", 12)]
    [DataRow("Magician", "lightning bolt spell", 12)]
    [DataRow("Thief", "dagger", 6)]
    [DataRow("Archer", "long bow", 8)]
    public void EveryOccupation_EnchantPreservesWeaponAndStats(string occupation, string type, int maximum)
    {
        var messages = Load();
        Player player = null!;
        Capture($"Human\nHero\n{occupation}\nroll\nroll\nroll\n2\n", () =>
            player = new CharacterCreation(messages, _ => 3).CreateCharacter());
        Assert.IsFalse(player.merchantGiftClaimed);
        Assert.AreEqual(0, player.armorProtection);
        var weapon = player.weapon;
        string art = weapon.asciiArt;
        int strength = player.strength, agility = player.agility, health = player.health;
        Capture("2\n", () => new Merchant(messages).Encounter(player));
        Assert.AreSame(weapon, player.weapon);
        Assert.AreEqual(type, weapon.type);
        Assert.AreEqual(art, weapon.asciiArt);
        Assert.AreEqual(maximum + 4, weapon.maxDamage);
        Assert.AreEqual(0, player.armorProtection);
        Assert.AreEqual(strength, player.strength);
        Assert.AreEqual(agility, player.agility);
        Assert.AreEqual(health, player.health);
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void SouthRepeatThenNorth_UsesSameEnchantedPlayerInCombat(string language)
    {
        var messages = Load(language);
        var player = new Player { strength = 0, agility = 20 };
        var weapon = player.weapon;
        string output = Capture("s\n3\ns\n2\ns\nn\na\nr\n", () => new Adventure(messages).StartAdventure(player));
        Assert.AreSame(weapon, player.weapon);
        Assert.AreEqual(16, weapon.maxDamage);
        StringAssert.Contains(output, messages.GetMessage(62));
        string confirmation = messages.GetMessage(61);
        StringAssert.Contains(output, confirmation);
        int afterConfirmation = output.IndexOf(confirmation) + confirmation.Length;
        StringAssert.Contains(output[afterConfirmation..], messages.GetMessage(7));
        StringAssert.Contains(output, messages.GetMessage(64));
        StringAssert.Contains(output, weapon.asciiArt);
        StringAssert.Contains(output, messages.GetMessage(41));
        Assert.AreEqual(4, output.Split(messages.GetMessage(7)).Length - 1);
    }

    [TestMethod]
    public void Combat_UsesEnchantedMaximumAndKeepsMinimumDamage()
    {
        var messages = Load();
        var player = new Player { weapon = new Weapon("dagger", 6, "dagger art") };
        Capture("2\n", () => new Merchant(messages).Encounter(player));
        foreach (int damage in new[] { 10, 1 })
        {
            var dragon = new Dragon("Dragon") { health = 50, agility = 0 };
            int initialHealth = dragon.health;
            var rolls = new Queue<int>(new[] { 1, 20, damage, 1 });
            var maxima = new List<int>();
            var combat = new Combat(player, dragon, messages, sides => { maxima.Add(sides); return rolls.Dequeue(); });
            string output = Capture("", combat.PlayerAttacksDragonSequence);
            CollectionAssert.AreEqual(new[] { 20, 20, 10, 4 }, maxima);
            Assert.AreEqual(initialHealth - damage, dragon.health);
            StringAssert.Contains(output, messages.FormatMessage(23, new Dictionary<string, string>
                { ["damage"] = damage.ToString(), ["healthPoints"] = dragon.health.ToString() }));
        }
    }

    [TestMethod]
    public void ArmorClaim_ProtectsPlayerWithoutChangingWeaponOrStats()
    {
        var player = new Player();
        var weapon = player.weapon;
        var messages = Load();
        string output = Capture("1\n", () => new Merchant(messages).Encounter(player));
        Assert.AreEqual(2, player.armorProtection);
        Assert.IsTrue(player.merchantGiftClaimed);
        Assert.AreSame(weapon, player.weapon);
        Assert.AreEqual(12, weapon.maxDamage);
        Assert.AreEqual("long sword", weapon.type);
        Assert.AreEqual("====>", weapon.asciiArt);
        Assert.AreEqual(20, player.health);
        Assert.AreEqual(10, player.agility);
        Assert.AreEqual(10, player.strength);
        StringAssert.Contains(output, messages.GetMessage(65));
    }

    [TestMethod]
    [DataRow(2, 7, 15, 5)]
    [DataRow(2, 2, 20, 0)]
    [DataRow(2, 1, 20, 0)]
    [DataRow(0, 7, 13, 7)]
    public void EnemyHit_AppliesProtectionAndReportsActualDamage(int protection, int rolledDamage, int expectedHealth, int appliedDamage)
    {
        var messages = Load();
        var player = new Player { agility = 0, armorProtection = protection };
        var dragon = new Dragon("Dragon") { strength = 20 };
        var rolls = new Queue<int>(new[] { 1, 20, rolledDamage, 1 });
        string output = Capture("", () => new Combat(player, dragon, messages, _ => rolls.Dequeue()).DragonAttacksPlayerSequence());
        Assert.AreEqual(expectedHealth, player.health);
        Assert.AreEqual(0, player.agility);
        StringAssert.Contains(output, messages.FormatMessage(39, new Dictionary<string, string>
            { ["damage"] = appliedDamage.ToString(), ["healthPoints"] = expectedHealth.ToString() }));
    }

    [TestMethod]
    public void NewPlayers_StartWithoutProtectionOrClaim()
    {
        foreach (var player in new[] { new Player(), new Player("New", Race.Elf, Occupation.Magician) })
        {
            Assert.AreEqual(0, player.armorProtection);
            Assert.IsFalse(player.merchantGiftClaimed);
        }
    }

    [TestMethod]
    [DataRow("English", "magical armor")]
    [DataRow("Spanish", "armadura mágica")]
    [DataRow("French", "armure magique")]
    public void LocalizedArmor_LeaveThenClaimAndRefuseFurtherGifts(string language, string armorTerm)
    {
        var messages = Load(language);
        var player = new Player();
        var merchant = new Merchant(messages);
        Capture("3\n", () => merchant.Encounter(player));
        Assert.AreEqual(0, player.armorProtection);
        Assert.IsFalse(player.merchantGiftClaimed);
        string output = Capture("\ninvalid\n1\n", () => merchant.Encounter(player));
        StringAssert.Contains(output, messages.GetMessage(63));
        StringAssert.Contains(output, messages.GetMessage(65));
        StringAssert.Contains(messages.GetMessage(60), armorTerm);
        StringAssert.Contains(messages.GetMessage(65), armorTerm);
        foreach (string number in new[] { "1 =", "2 =", "3 =", "+4", "2", "0" })
            StringAssert.Contains(messages.GetMessage(60), number);
        Assert.AreEqual(2, player.armorProtection);
        Assert.IsTrue(player.merchantGiftClaimed);
        foreach (string attemptedChoice in new[] { "1\n", "2\n" })
        {
            string repeat = Capture(attemptedChoice, () => merchant.Encounter(player));
            StringAssert.Contains(repeat, messages.GetMessage(64));
            Assert.DoesNotContain(messages.GetMessage(60), repeat);
            Assert.AreEqual(2, player.armorProtection);
            Assert.AreEqual(12, player.weapon.maxDamage);
        }
    }

    [TestMethod]
    public void WeaponClaim_PreventsArmorOrAdditionalEnchantments()
    {
        var messages = Load();
        var player = new Player();
        var merchant = new Merchant(messages);
        Capture("2\n", () => merchant.Encounter(player));
        foreach (string attemptedChoice in new[] { "1\n", "2\n" })
        {
            StringAssert.Contains(Capture(attemptedChoice, () => merchant.Encounter(player)), messages.GetMessage(64));
            Assert.AreEqual(0, player.armorProtection);
            Assert.AreEqual(16, player.weapon.maxDamage);
        }
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    public void ArmorJourney_ReturnsToDirectionsThenEntersDragonCombat(string language)
    {
        var messages = Load(language);
        var player = new Player { strength = 0, agility = 20 };
        string output = Capture("s\n3\ns\n1\ns\nn\na\nr\n", () => new Adventure(messages).StartAdventure(player));
        Assert.AreEqual(2, player.armorProtection);
        Assert.AreEqual(12, player.weapon.maxDamage);
        Assert.AreEqual(20, player.health);
        StringAssert.Contains(output, messages.GetMessage(62));
        string confirmation = messages.GetMessage(65);
        StringAssert.Contains(output, confirmation);
        int afterConfirmation = output.IndexOf(confirmation) + confirmation.Length;
        StringAssert.Contains(output[afterConfirmation..], messages.GetMessage(7));
        Assert.AreEqual(4, output.Split(messages.GetMessage(7)).Length - 1);
        StringAssert.Contains(output, messages.GetMessage(65));
        StringAssert.Contains(output, messages.GetMessage(64));
        StringAssert.Contains(output, player.weapon.asciiArt);
        StringAssert.Contains(output, messages.GetMessage(41));
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(2, false)]
    [DataRow(0, true)]
    [DataRow(2, true)]
    public void EnemyMissOrDodge_DoesNotDamagePlayer(int protection, bool dodge)
    {
        var messages = Load();
        var player = new Player { armorProtection = protection, agility = 10 };
        var dragon = new Dragon("Dragon") { strength = 10 };
        var rolls = new Queue<int>(dodge ? new[] { 1, 10 } : new[] { 11 });
        string output = Capture("", () => new Combat(player, dragon, messages, _ => rolls.Dequeue()).DragonAttacksPlayerSequence());
        Assert.AreEqual(20, player.health);
        Assert.AreEqual(10, player.agility);
        StringAssert.Contains(output, messages.GetMessage(dodge ? 38 : 37));
    }

    [TestMethod]
    [DataRow(0, 5, -2, 7)]
    [DataRow(2, 5, 0, 5)]
    [DataRow(2, 4, -1, 5)]
    public void LethalEnemyHit_ReportsAppliedDamageAndDefeat(int protection, int health, int expectedHealth, int appliedDamage)
    {
        var messages = Load();
        var player = new Player { health = health, agility = 0, armorProtection = protection };
        var dragon = new Dragon("Dragon") { strength = 20 };
        var rolls = new Queue<int>(new[] { 1, 20, 7 });
        string output = Capture("", () => new Combat(player, dragon, messages, _ => rolls.Dequeue()).DragonAttacksPlayerSequence());
        Assert.AreEqual(expectedHealth, player.health);
        StringAssert.Contains(output, messages.GetMessage(40));
        StringAssert.Contains(output, messages.FormatMessage(39, new Dictionary<string, string>
            { ["damage"] = appliedDamage.ToString(), ["healthPoints"] = expectedHealth.ToString() }));
    }
}
