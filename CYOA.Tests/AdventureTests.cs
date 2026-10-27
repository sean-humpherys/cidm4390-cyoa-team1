using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CYOA;
using cidm4390_cyoa_team1;


namespace CYOA.Tests;

[DoNotParallelize]
[TestClass]

public class AdventureTests
{
    private static Messages CreateMessages()
    {
        return new Messages
        {
            LanguageDictionary = new Dictionary<string, string>
            {
                ["4"] = "Adventure ends. Bye.",
                ["7"] = "You are in the mountains. Enter n, s, or e.",
                ["8"] = "You take the South path. You return home bored.",
                ["9"] = "You take the North path. You encounter a dragon.",
                ["10"] = "The dragon says: I am {dragonName}. Leave or perish!",
                ["11"] = "Your magic ring gives you the stats on the dragon.",
                ["12"] = "Enter r to retreat or a to attack.",
                ["25"] = "Enter r for retreat and a for attack.",
                ["26"] = "You hastily retreat licking your wounds.",
                ["41"] = "You quickly retreat. You come home poor but alive. The End."
            }
        };
    }

    private static Player CreatePlayer()
    {
        return new Player("TestHero", Race.Human, Occupation.Fighter);
    }

    [TestMethod]
    public void StartAdventure_SouthPath_DisplaysSouthScenario()
    {
        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;

        using StringWriter output = new StringWriter();

        try
        {
            Console.SetIn(new StringReader("s\n"));
            Console.SetOut(output);

            Adventure adventure = new Adventure(CreateMessages());
            adventure.StartAdventure(CreatePlayer());

            StringAssert.Contains(
                output.ToString(),
                "You take the South path.");

            StringAssert.Contains(
                output.ToString(),
                "You return home bored.");
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }

    [TestMethod]
    public void StartAdventure_ExitChoice_DisplaysExitMessage()
    {
        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;

        using StringWriter output = new StringWriter();

        try
        {
            Console.SetIn(new StringReader("e\n"));
            Console.SetOut(output);

            Adventure adventure = new Adventure(CreateMessages());
            adventure.StartAdventure(CreatePlayer());

            StringAssert.Contains(
                output.ToString(),
                "Adventure ends. Bye.");
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }

    [TestMethod]
    public void StartAdventure_NorthThenRetreat_DisplaysDragonAndEnding()
    {
        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;

        using StringWriter output = new StringWriter();

        try
        {
            Console.SetIn(new StringReader("n\nr\n"));
            Console.SetOut(output);

            Adventure adventure = new Adventure(CreateMessages());
            adventure.StartAdventure(CreatePlayer());

            string result = output.ToString();

            StringAssert.Contains(
                result,
                "You take the North path.");

            StringAssert.Contains(
                result,
                "I am Dragon.");

            StringAssert.Contains(
                result,
                "You hastily retreat");

            StringAssert.Contains(
                result,
                "The End.");
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }

    [TestMethod]
    public void StartAdventure_InvalidChoice_AsksForAnotherChoice()
    {
        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;

        using StringWriter output = new StringWriter();

        try
        {
            Console.SetIn(new StringReader("x\ns\n"));
            Console.SetOut(output);

            Adventure adventure = new Adventure(CreateMessages());
            adventure.StartAdventure(CreatePlayer());

            StringAssert.Contains(
                output.ToString(),
                "Invalid choice");

            StringAssert.Contains(
                output.ToString(),
                "You take the South path.");
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }
}