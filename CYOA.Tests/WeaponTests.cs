
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cidm4390_cyoa_team1;

namespace CYOA.Tests
{
    [TestClass]
    public class WeaponTests
    {
        [TestMethod]
        public void Weapon_ConstructorSetsAllProperties()
        {
            Weapon sword = new Weapon("long sword", 12, "====>");

            Assert.AreEqual("long sword", sword.type);
            Assert.AreEqual(12, sword.maxDamage);
            Assert.AreEqual("====>", sword.asciiArt);
        }

        [TestMethod]
        public void Weapon_SupportsAllFourCharacterWeapons()
        {
            Weapon[] weapons =
            {
                new Weapon("long sword", 12, "====>"),
                new Weapon("lightning bolt spell", 12, "~~~>"),
                new Weapon("dagger", 6, "==>"),
                new Weapon("long bow", 8, ")---->")
            };

            Assert.HasCount(4, weapons);
            Assert.AreEqual(12, weapons[0].maxDamage);
            Assert.AreEqual(12, weapons[1].maxDamage);
            Assert.AreEqual(6, weapons[2].maxDamage);
            Assert.AreEqual(8, weapons[3].maxDamage);
        }

        [TestMethod]
        public void Weapon_PropertiesCanBeUpdated()
        {
            Weapon weapon = new Weapon("dagger", 6, "==>");

            weapon.type = "long sword";
            weapon.maxDamage = 12;
            weapon.asciiArt = "====>";

            Assert.AreEqual("long sword", weapon.type);
            Assert.AreEqual(12, weapon.maxDamage);
            Assert.AreEqual("====>", weapon.asciiArt);
        }
    }
}
