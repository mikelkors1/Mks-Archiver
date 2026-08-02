using System;
using Figgle;

namespace MksArchiver.Cli;

[EmbedFiggleFont(memberName: "Font", fontName: "standard")]
internal static partial class Art {}

public class Greetings
{
    private static readonly ConsoleColor DefaultColor = Console.ForegroundColor;
    
    private const string BoldOn = "\u001b[1m";
    private const string BoldOff = "\u001b[0m";
    private const string UnderlineOn = "\u001b[4m";
    private const string UnderlineOff = "\u001b[24m";
    
    
    private static readonly ConsoleColor[] RaindbowColors =
    {
        ConsoleColor.Red,
        ConsoleColor.Green,
        ConsoleColor.Yellow,
        ConsoleColor.Blue,
        ConsoleColor.Cyan,
        ConsoleColor.Magenta
    };

    private static void TurnOnBold() => Console.Write(BoldOn);
    private static void TurnOffBold() => Console.Write(BoldOff);
    private static void SetCursorCoords(int x = 0, int y = 0) => Console.SetCursorPosition(Console.CursorLeft + x, Console.CursorTop + y);
    private static void SetColor(ConsoleColor color) => Console.ForegroundColor = color;
    private static void WriteUnderlined(string text) => Console.Write($"{UnderlineOn}{text}{UnderlineOff}");
    
    public static void PrintGreeting()
    {
        string Text = Art.Font.Render("mikelkors");
        string[] SplitedText = Text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        for (int i = 0; i < SplitedText.Length; i++)
        {
            Console.ForegroundColor = RaindbowColors[i % RaindbowColors.Length];
            Console.Write(SplitedText[i]);
            if (i != SplitedText.Length - 1) Console.WriteLine();
        }
        
        TurnOnBold();
        SetCursorCoords(-17);
        SetColor(ConsoleColor.Magenta);
        Console.Write("𝓪𝓻𝓬𝓱𝓲𝓿𝓮𝓻");
        SetColor(DefaultColor);
        TurnOffBold();
        
        Console.WriteLine();
        Console.WriteLine();
        
        TurnOffBold();
        SetColor(ConsoleColor.Blue);
        Console.Write("Actual archiver version: v.1.0.0");
        SetColor(DefaultColor);
        TurnOffBold();
        
        Console.WriteLine();
        Console.WriteLine();

        Console.Write("Github link: ");
        SetColor(ConsoleColor.DarkCyan);
        WriteUnderlined("https://github.com/mikelkors/MksArchiver");
        SetColor(DefaultColor);
        
        Console.WriteLine();

        Console.Write("Argument to get Help: ");
        SetColor(ConsoleColor.DarkCyan);
        WriteUnderlined("--help");
        SetColor(DefaultColor);
        
        Console.WriteLine('\n');
    }
}
