using System;
using System.IO;
using System.Reflection;

namespace PFire
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var server = new PFireServer(baseDirectory);
            server.Start();
            Console.WriteLine("Emulated XFire Server \"PFire\", started.");
            Console.Read();
        }
    }
}
