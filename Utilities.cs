using System;

namespace Utilities
{
    namespace Exceptions
    {
        public class NoNoisyThingException: Exception 
        {
            public NoNoisyThingException(string e): base(e) {}
        }
    }

    public static class TermColors
    {
        public static ConsoleColor DefaultBgColor = ConsoleColor.Black;
        public static ConsoleColor DefaultFgColor = ConsoleColor.White;

        public static ConsoleColor LightColor = ConsoleColor.Yellow;
        public static ConsoleColor WaterColor = ConsoleColor.Blue;
        public static ConsoleColor DangerColor = ConsoleColor.Red;
        public static ConsoleColor VictoryColor = ConsoleColor.Green;
        public static ConsoleColor BumpColor = ConsoleColor.DarkYellow;
    }

    public static class Utilities
    {
        public static Random rand = new Random();

        public static void WriteColored(ConsoleColor color, string words)
        {
            Console.ForegroundColor = color;
            Console.Write(words);
            ResetTermColors();
        }

        public static void WriteColoredLine(ConsoleColor color, string line)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            ResetTermColors();
        }

        public static void WritePromptedColoredLine(ConsoleColor color, string line)
        {
            WriteColoredLine(color, line);
            Console.Write("Press enter to continue.");
            Console.Read();
        }

        public static void ResetTermColors()
        {
            Console.ForegroundColor = TermColors.DefaultFgColor;
        }
    }
}