using System;
using System.IO;
using System.Reflection;
using PFire.Core;
using Topshelf;

namespace PFire.WindowsService
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            HostFactory.Run(x =>
            {
                x.Service<PFireServer>(s =>
                {
                    s.ConstructUsing(name => new PFireServer(baseDirectory));
                    s.WhenStarted(pf => pf.Start());
                    s.WhenStopped(pf => pf.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Emulated XFire Server");
                x.SetDisplayName("PFire Server");
                x.SetServiceName("PFireServer");
            });
        }
    }
}