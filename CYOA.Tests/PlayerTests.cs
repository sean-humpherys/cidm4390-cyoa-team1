using Microsoft.VisualStudio.TestTools.UnitTesting;
using cidm4390_cyoa_team1;

namespace CYOA.Tests
{
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void Constructor_SetsNameRaceAndOccupation()
        {
            Player player = new Player("Aragorn", Race.Human, Occupation.Fighter);

            Assert.AreEqual("Aragorn", player.name);
            Assert.AreEqual(Race.Human, player.race);
            Assert.AreEqual(Occupation.Fighter, player.occupation);
        }

        [TestMethod]
        public void Constructor_SetsStatsAndWeapon()
        {
            Player player = new Player ();

            Assert.AreEqual(10, player.strength);
            Assert.AreEqual(10, player.agility);
            Assert.AreEqual(20, player.health);
            Assert.IsNotNull(player.weapon);
            Assert.AreEqual("long sword", player.weapon.type);
        }

        [TestMethod]
        public void MissedMeTaunt_ReturnsMessage()
        {
            Player player = new Player();
            Assert.IsFalse(string.IsNullOrWhiteSpace(player.MissedMeTaunt()));
        }
        
        [TestMethod]
        public void WasHitTaunt_ReturnsMessage()
        {
            Player player = new Player();
            Assert.IsFalse(string.IsNullOrWhiteSpace(player.WasHitTaunt()));
        }

        [TestMethod]
        public void DisplayStats_IncludesPlayerInformation()
        {
            Player player = new Player ("Aragorn", Race.Human, Occupation.Fighter);

            string stats = player.DisplayStats();

            StringAssert.Contains(stats, "Aragorn");
            StringAssert.Contains(stats, "Human");
            StringAssert.Contains(stats, "Fighter");
            StringAssert.Contains(stats, "Strength");
            StringAssert.Contains(stats, "Health");
            StringAssert.Contains(stats, "long sword");

        }
    }
}