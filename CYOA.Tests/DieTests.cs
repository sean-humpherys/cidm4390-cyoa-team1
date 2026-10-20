
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cidm4390_cyoa_team1;

namespace CYOA.Tests
{
    [TestClass]
    public class DieTests
    {
        [TestMethod]
        public void Roll_D20_ReturnsNumberBetween1And20()
        {
            Die die = new Die();

            for (int i = 0; i < 100; i++)
            {
                int result = die.Roll(20);

                Assert.IsTrue(result >= 1 && result <= 20);
            }
        }

        [TestMethod]
        public void Roll_D6_ReturnsNumberBetween1And6()
        {
            Die die = new Die();

            for (int i = 0; i < 100; i++)
            {
                int result = die.Roll(6);

                Assert.IsTrue(result >= 1 && result <= 6);
            }
        }

        [TestMethod]
        public void Roll_OneSidedDie_AlwaysReturns1()
        {
            Die die = new Die();

            Assert.AreEqual(1, die.Roll(1));
        }

        [TestMethod]
        public void Roll_ZeroSides_ThrowsException()
        {
            Die die = new Die();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => die.Roll(0));
        }

        [TestMethod]
        public void Roll_NegativeSides_ThrowsException()
        {
            Die die = new Die();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => die.Roll(-6));
        }
    }
}
