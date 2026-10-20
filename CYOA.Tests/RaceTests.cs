
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cidm4390_cyoa_team1;

[TestClass]
public class RaceTests
{
    [TestMethod]
    public void Race_ContainsAllFourRequiredValues()
    {
        string[] expected =
        {
            "Elf",
            "Human",
            "Dwarf",
            "Halfling"
        };

        string[] actual = Enum.GetNames<Race>();

        CollectionAssert.AreEquivalent(expected, actual);
    }

[TestMethod]
public void Race_HasExactlyFourOptions()
{
    Assert.HasCount(4, Enum.GetValues<Race>());
}
}
