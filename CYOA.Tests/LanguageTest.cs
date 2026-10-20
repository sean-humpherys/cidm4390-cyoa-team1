using CYOA;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CYOA.Tests;
[TestClass]
public class LanguageTests
{
    [TestMethod]
    public void Language_ContainsThreeSupportedLanguages()
    {
        Assert.IsTrue(Enum.IsDefined(typeof(Language), Language.English));
        Assert.IsTrue(Enum.IsDefined(typeof(Language), Language.French));
        Assert.IsTrue(Enum.IsDefined(typeof(Language), Language.Spanish));

        Assert.AreEqual(3, Enum.GetValues<Language>().Length);
    }
}