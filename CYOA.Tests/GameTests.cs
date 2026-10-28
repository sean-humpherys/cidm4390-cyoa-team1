
using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CYOA;

namespace CYOA.Tests;

[DoNotParallelize]
[TestClass]
public class GameTests
{
    [TestMethod]
    public void StartGame_Option2_DisplaysExitMessage()
    {
        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;

        using StringWriter output = new StringWriter();

        try
        {
            Console.SetIn(new StringReader("e\n2\n"));
            Console.SetOut(output);

            Game game = new Game();
            game.StartGame();

            string result = output.ToString();

            StringAssert.Contains(
                result,
                "Welcome to Choose Your Adventure Game");

            StringAssert.Contains(
                result,
                "Adventure ends. Bye.");
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }

    [TestMethod]
    public void StartGame_InvalidChoice_ShowsError()
    {
        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;

        using StringWriter output = new StringWriter();

        try
        {
            Console.SetIn(new StringReader("e\nx\n2\n"));
            Console.SetOut(output);

            Game game = new Game();
            game.StartGame();

            StringAssert.Contains(
                output.ToString(),
                "Invalid choice");
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }
    }
}