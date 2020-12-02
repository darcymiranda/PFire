using System;

namespace PFire.Core.Util
{
    public static class ConsoleLogger
    {
        public static void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
        }
    }
}
