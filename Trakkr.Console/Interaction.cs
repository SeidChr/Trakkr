using System;

namespace Trakkr.Console
{
    public  class Interaction
    {
        private const string yesNo = " (y/n) ";
        private const string somethingFollows = ": ";

        public static void Acknowledge(string message)
        {
            System.Console.Write(message);
            System.Console.ReadKey(true);
            System.Console.WriteLine();
        }

        public static void Notice(string message)
        {
            System.Console.WriteLine(message);
        }

        public static bool Confirm(string message)
        {
            System.Console.Write(message + yesNo);
            var key = System.Console.ReadKey(false);
            var @continue = key.Key == ConsoleKey.Y;
            System.Console.WriteLine();
            return @continue;
        }

        public static string GetText(string message)
        {
            System.Console.Write(message + somethingFollows);
            var text = System.Console.ReadLine();
            return text;
        }
    }
}