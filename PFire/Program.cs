using PFire.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace PFire
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<PFireServer>(s =>
                {
                    s.ConstructUsing(name => new PFireServer());
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
