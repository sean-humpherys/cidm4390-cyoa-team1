
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cidm4390_cyoa_team1;

namespace CYOA.Tests
{
    [TestClass]
    public class CombatTests
    {
        // TEST 1: Verify that the Combat constructor
        // correctly assigns the Player and Dragon.
        [TestMethod]
        public void Combat_Constructor_AssignsPlayerAndDragon()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");

            // Act
            Combat combat = new Combat(player, dragon);

            // Assert
            Assert.AreSame(player, combat.player);
            Assert.AreSame(dragon, combat.dragon);
        }

        // TEST 2: Verify that a missed player attack
        // does not reduce the dragon's health.
        [TestMethod]
        public void PlayerAttacksDragon_WhenAttackMisses_DragonHealthUnchanged()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");

            // A 20-sided die always rolls between 1 and 20.
            // Strength = 0 guarantees the player's attack misses.
            player.strength = 0;

            int originalHealth = dragon.health;

            Combat combat = new Combat(player, dragon);

            // Act
            combat.PlayerAttacksDragonSequence();

            // Assert
            Assert.AreEqual(
                originalHealth,
                dragon.health,
                "The dragon should not lose health when the player misses."
            );
        }

        // TEST 3: Verify that a successful dragon defense
        // prevents damage from the player's attack.
        [TestMethod]
        public void PlayerAttacksDragon_WhenDragonDefends_DragonHealthUnchanged()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");

            // Every 20-sided roll is at or below 20.
            // Strength = 20 guarantees a hit.
            // Agility = 20 guarantees the dragon defends.
            player.strength = 20;
            dragon.agility = 20;

            int originalHealth = dragon.health;

            Combat combat = new Combat(player, dragon);

            // Act
            combat.PlayerAttacksDragonSequence();

            // Assert
            Assert.AreEqual(
                originalHealth,
                dragon.health,
                "The dragon should not lose health when it successfully defends."
            );
        }

        // TEST 4: Verify that an undefended player attack
        // reduces dragon health within the weapon's damage range.
        [TestMethod]
        public void PlayerAttacksDragon_WhenDragonFailsDefense_AppliesWeaponDamage()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");

            // Guarantee the player hits and the dragon fails to defend.
            player.strength = 20;
            dragon.agility = 0;

            // Give the dragon enough health to survive the attack.
            dragon.health = 100;

            int originalHealth = dragon.health;
            int maximumDamage = player.weapon.maxDamage;

            Combat combat = new Combat(player, dragon);

            // Act
            combat.PlayerAttacksDragonSequence();

            // Calculate the actual damage inflicted.
            int actualDamage = originalHealth - dragon.health;

            // Assert: Damage must be at least 1.
            Assert.IsGreaterThanOrEqualTo(
                1,
                actualDamage,
                "A successful, undefended attack must cause at least 1 damage."
            );

            // Assert: Damage must not exceed the weapon's maximum.
            Assert.IsLessThanOrEqualTo(
                maximumDamage,
                actualDamage,
                "Damage must not exceed the weapon's maximum damage."
            );
        }

        // TEST 5: Verify that a successful attack defeats
        // the dragon when its health reaches zero or below.
        [TestMethod]
        public void PlayerAttacksDragon_WhenHealthReachesZero_DragonIsDefeated()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");

            // Guarantee the player hits and the dragon cannot defend.
            player.strength = 20;
            dragon.agility = 0;

            // Any successful attack will defeat the dragon.
            dragon.health = 1;

            Combat combat = new Combat(player, dragon);

            // Act
            combat.PlayerAttacksDragonSequence();

            // Assert
            Assert.IsLessThanOrEqualTo(
                0,
                dragon.health,
                "The dragon's health should be zero or below after defeat."
            );
        }

        // TEST 6: Verify that a missed dragon attack
        // does not reduce the player's health.
        [TestMethod]
        public void DragonAttacksPlayer_WhenAttackMisses_PlayerHealthUnchanged()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");

            // Strength = 0 guarantees the dragon's attack misses.
            dragon.strength = 0;

            int originalHealth = player.health;

            Combat combat = new Combat(player, dragon);

            // Act
            combat.DragonAttacksPlayerSequence();

            // Assert
            Assert.AreEqual(
                originalHealth,
                player.health,
                "The player should not lose health when the dragon misses."
            );
        }

        // TEST 7: Verify that a successful player defense
        // prevents damage from the dragon's attack.
        [TestMethod]
        public void DragonAttacksPlayer_WhenPlayerDefends_PlayerHealthUnchanged()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");

            dragon.strength = 20;
            player.agility = 20;

            int originalHealth = player.health;

            Combat combat = new Combat(player, dragon);

            // Act
            combat.DragonAttacksPlayerSequence();

            // Assert
            Assert.AreEqual(
                originalHealth,
                player.health,
                "The player should not lose health when successfully defending."
            );
        }

        // TEST 8: Verify that an undefended dragon attack
        // reduces player health within the dragon's weapon damage range.
        [TestMethod]
        public void DragonAttacksPlayer_WhenPlayerFailsDefense_AppliesWeaponDamage()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");

            dragon.strength = 20;
            player.agility = 0;
            player.health = 100;

            int originalHealth = player.health;
            int maximumDamage = dragon.weapon.maxDamage;

            Combat combat = new Combat(player, dragon);

            // Act
            combat.DragonAttacksPlayerSequence();

            int actualDamage = originalHealth - player.health;

            // Assert
            Assert.IsGreaterThanOrEqualTo(
                1,
                actualDamage,
                "An undefended dragon attack must cause at least 1 damage."
            );

            Assert.IsLessThanOrEqualTo(
                maximumDamage,
                actualDamage,
                "Dragon damage must not exceed its weapon's maximum damage."
            );
        }

        // TEST 9: Verify that the player is defeated
        // when health reaches zero or below.
        [TestMethod]
        public void DragonAttacksPlayer_WhenHealthReachesZero_PlayerIsDefeated()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");

            dragon.strength = 20;
            player.agility = 0;
            player.health = 1;

            Combat combat = new Combat(player, dragon);

            // Act
            combat.DragonAttacksPlayerSequence();

            // Assert
            Assert.IsLessThanOrEqualTo(
                0,
                player.health,
                "The player's health should be zero or below after defeat."
            );
        }

        // TEST 10: Verify that the constructor rejects a null Player.
        [TestMethod]
        public void Combat_Constructor_WhenPlayerIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            Dragon dragon = new Dragon("Smaug");

            // Act and Assert
            Assert.ThrowsExactly<System.ArgumentNullException>(
                () => new Combat(null!, dragon)
            );
        }

        // TEST 11: Verify that the constructor rejects a null Dragon.
        [TestMethod]
        public void Combat_Constructor_WhenDragonIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            Player player = new Player();

            // Act and Assert
            Assert.ThrowsExactly<System.ArgumentNullException>(
                () => new Combat(player, null!)
            );
        }

        // TEST 12: Verify that the constructor rejects null Messages.
        [TestMethod]
        public void Combat_Constructor_WhenMessagesIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");

            // Act and Assert
            Assert.ThrowsExactly<System.ArgumentNullException>(
                () => new Combat(player, dragon, null!)
            );
        }

        // TEST 13: Verify that the constructor accepts
        // a supplied Messages instance.
        [TestMethod]
        public void Combat_Constructor_WithSuppliedMessages_CreatesCombat()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");
            CYOA.Messages messages = new CYOA.Messages();

            // Act
            Combat combat = new Combat(player, dragon, messages);

            // Assert
            Assert.IsNotNull(combat);
            Assert.AreSame(player, combat.player);
            Assert.AreSame(dragon, combat.dragon);
        }

        // TEST 14: Verify that Combat uses the supplied
        // message dictionary rather than always loading English.
        [TestMethod]
        [DoNotParallelize]
        public void PlayerAttacksDragon_WithSuppliedMessages_UsesCustomMessage()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");

            // Guarantee that the player misses.
            player.strength = 0;

            CYOA.Messages messages = new CYOA.Messages();

            // Supply the messages required by this attack path.
            messages.LanguageDictionary["20"] =
                "CUSTOM ATTACK: {occupation} uses {weapon}.";
            messages.LanguageDictionary["35"] =
                "CUSTOM MISS: {dragonName} avoided the attack.";

            Combat combat = new Combat(player, dragon, messages);

            System.IO.TextWriter originalOutput = System.Console.Out;

            using System.IO.StringWriter output =
                new System.IO.StringWriter();

            try
            {
                // Act
                System.Console.SetOut(output);
                combat.PlayerAttacksDragonSequence();
            }
            finally
            {
                System.Console.SetOut(originalOutput);
            }

            // Assert
            string result = output.ToString();

            StringAssert.Contains(
                result,
                "CUSTOM ATTACK:"
            );

            StringAssert.Contains(
                result,
                "CUSTOM MISS: Smaug avoided the attack."
            );
        }

        // TEST 15: Verify that the correct defeat message
        // appears when the dragon is defeated.
        [TestMethod]
        [DoNotParallelize]
        public void PlayerAttacksDragon_WhenDragonDefeated_DisplaysDefeatMessage()
        {
            // Arrange
            Player player = new Player();
            Dragon dragon = new Dragon("Smaug");

            player.strength = 20;
            dragon.agility = 0;
            dragon.health = 1;

            CYOA.Messages messages = new CYOA.Messages();

            // Supply the messages required by this attack path.
            messages.LanguageDictionary["20"] =
                "Player attacks with {weapon} as a {occupation}.";
            messages.LanguageDictionary["21"] =
                "Player hits {dragonName}.";
            messages.LanguageDictionary["23"] =
                "Damage: {damage}. Remaining health: {healthPoints}.";
            messages.LanguageDictionary["24"] =
                "CUSTOM VICTORY: The dragon has been defeated!";

            Combat combat = new Combat(player, dragon, messages);

            System.IO.TextWriter originalOutput = System.Console.Out;

            using System.IO.StringWriter output =
                new System.IO.StringWriter();

            try
            {
                // Act
                System.Console.SetOut(output);
                combat.PlayerAttacksDragonSequence();
            }
            finally
            {
                System.Console.SetOut(originalOutput);
            }

            // Assert
            Assert.IsLessThanOrEqualTo(
                0,
                dragon.health,
                "The dragon should be defeated."
            );

            StringAssert.Contains(
                output.ToString(),
                "CUSTOM VICTORY: The dragon has been defeated!"
            );
        }
    }
}
