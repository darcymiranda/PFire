using System.IO;
using System.Reflection;
using PFire.Core;

namespace PFire.Console
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var server = new PFireServer(baseDirectory);
            server.Start();
            System.Console.WriteLine("Emulated XFire Server \"PFire\", started.");
            System.Console.Read();
        }
    }
}
