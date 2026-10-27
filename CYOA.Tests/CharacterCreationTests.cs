
using System;
using System.Collections.Generic;
using System.IO;
using CYOA;
using cidm4390_cyoa_team1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CYOA.Tests;

[TestClass]
[DoNotParallelize]
public class CharacterCreationTests
{
    // Creates sample messages so tests do not depend on TXT or JSON files.
    private static Messages CreateMessages()
    {
        var messages = new Messages();

        for (int id = 13; id <= 19; id++)
        {
            messages.LanguageDictionary[id.ToString()] = $"Prompt {id}";
        }

        for (int id = 27; id <= 34; id++)
        {
            messages.LanguageDictionary[id.ToString()] = $"Label {id}";
        }

        return messages;
    }

    // Checks that a Fighter has the correct name, race, occupation, and weapon.
    [TestMethod]
    public void CreateCharacter_Fighter_AssignsCorrectWeapon()
    {
        var messages = CreateMessages();
        var creation = new CharacterCreation(messages, new Die());

        string input = "Human\nAlex\nFighter\nroll\nroll\nroll\n2\n";

        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;

        try
        {
            Console.SetIn(new StringReader(input));
            Console.SetOut(new StringWriter());

            Player player = creation.CreateCharacter();

            Assert.AreEqual("Alex", player.name);
            Assert.AreEqual(Race.Human, player.race);
            Assert.AreEqual(Occupation.Fighter, player.occupation);
            Assert.AreEqual("long sword", player.weapon.type);
            Assert.AreEqual(12, player.weapon.maxDamage);
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }

    // Checks that an Elf's strength, agility, and health are within valid ranges.
    [TestMethod]
    public void CreateCharacter_Elf_AppliesRacialModifiers()
    {
        var creation = new CharacterCreation(CreateMessages(), new Die());

        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;

        try
        {
            Console.SetIn(new StringReader(
                "Elf\nLuna\nArcher\nroll\nroll\nroll\n2\n"));
            Console.SetOut(new StringWriter());

            Player player = creation.CreateCharacter();

            Assert.IsTrue(player.strength >= 2 && player.strength <= 24);
            Assert.IsTrue(player.agility >= 2 && player.agility <= 24);
            Assert.IsTrue(player.health >= 2 && player.health <= 24);
            Assert.AreEqual("long bow", player.weapon.type);
            Assert.AreEqual(8, player.weapon.maxDamage);
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }

    // Checks that all eight character statistics display correctly.
    [TestMethod]
    public void DisplayCharacterStats_ShowsAllEightFields()
    {
        var creation = new CharacterCreation(CreateMessages(), new Die());

        var player = new Player("Robin", Race.Halfling, Occupation.Thief)
        {
            strength = 9,
            agility = 15,
            health = 12,
            weapon = new Weapon("dagger", 6, "-)==>")
        };

        TextWriter originalOutput = Console.Out;

        try
        {
            using var writer = new StringWriter();
            Console.SetOut(writer);

            creation.DisplayCharacterStats(player);

            string output = writer.ToString();

            Assert.IsTrue(output.Contains("Label 27: Robin"));
            Assert.IsTrue(output.Contains("Label 28: Halfling"));
            Assert.IsTrue(output.Contains("Label 29: Thief"));
            Assert.IsTrue(output.Contains("Label 30: 9"));
            Assert.IsTrue(output.Contains("Label 31: 15"));
            Assert.IsTrue(output.Contains("Label 32: 12"));
            Assert.IsTrue(output.Contains("Label 33: dagger"));
            Assert.IsTrue(output.Contains("Label 34: 6"));
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }

    // Checks that viewing stats returns the player to the character menu.
    [TestMethod]
    public void CreateCharacter_ViewStats_ReturnsToMenu()
    {
        var creation = new CharacterCreation(CreateMessages(), new Die());

        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;

        try
        {
            Console.SetIn(new StringReader(
                "Dwarf\nGimli\nFighter\nroll\nroll\nroll\n1\n2\n"));

            using var writer = new StringWriter();
            Console.SetOut(writer);

            Player player = creation.CreateCharacter();

            string output = writer.ToString();

            Assert.AreEqual("Gimli", player.name);
            Assert.IsTrue(output.Contains("Label 27: Gimli"));

            int menuCount = output.Split("Prompt 19").Length - 1;
            Assert.AreEqual(2, menuCount);
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }
}