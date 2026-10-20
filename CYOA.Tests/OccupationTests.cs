
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cidm4390_cyoa_team1;

[TestClass]
public class OccupationTests
{
    [TestMethod]
    public void Occupation_ContainsAllFourRequiredValues()
    {
        string[] expected =
        {
            "Fighter",
            "Magician",
            "Thief",
            "Archer"
        };

        string[] actual = Enum.GetNames<Occupation>();

        CollectionAssert.AreEquivalent(expected, actual);
    }

    [TestMethod]
    public void Occupation_HasExactlyFourOptions()
    {
        Assert.HasCount(4, Enum.GetValues<Occupation>());
    }
}
