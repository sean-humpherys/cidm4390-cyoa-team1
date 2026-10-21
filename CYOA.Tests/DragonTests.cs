
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cidm4390_cyoa_team1;

namespace CYOA.Tests
{
    [TestClass]
    public class DragonTests
    {
        [TestMethod]
        public void Constructor_SetsDragonName()
        {
            Dragon dragon = new Dragon("Smaug");

            Assert.AreEqual("Smaug", dragon.name);
        }

        [TestMethod]
        public void Constructor_GeneratesStatsWithinRange()
        {
            Dragon dragon = new Dragon("Smaug");

            Assert.IsTrue(dragon.strength >= 1 && dragon.strength <= 20);
            Assert.IsTrue(dragon.health >= 1 && dragon.health <= 20);
            Assert.IsTrue(dragon.agility >= 1 && dragon.agility <= 20);
        }

        [TestMethod]
        public void Constructor_EquipsClawsWeapon()
        {
            Dragon dragon = new Dragon("Smaug");

            Assert.IsNotNull(dragon.weapon);
            Assert.AreEqual("claws", dragon.weapon.type);
            Assert.AreEqual(12, dragon.weapon.maxDamage);
            Assert.AreEqual("###", dragon.weapon.asciiArt);
        }

        [TestMethod]
        public void MissedMeTaunt_ReturnsMessage()
        {
            Dragon dragon = new Dragon("Smaug");

            string message = dragon.MissedMeTaunt();

            Assert.IsFalse(string.IsNullOrWhiteSpace(message));
        }

        [TestMethod]
        public void WasHitTaunt_ReturnsMessage()
        {
            Dragon dragon = new Dragon("Smaug");

            string message = dragon.WasHitTaunt();

            Assert.IsFalse(string.IsNullOrWhiteSpace(message));
        }

        [TestMethod]
        public void DisplayStats_IncludesDragonInformation()
        {
            Dragon dragon = new Dragon("Smaug");

            string stats = dragon.DisplayStats();

            StringAssert.Contains(stats, "Smaug");
            StringAssert.Contains(stats, "Strength");
            StringAssert.Contains(stats, "Health");
            StringAssert.Contains(stats, "Agility");
            StringAssert.Contains(stats, "claws");
        }
    }
}
