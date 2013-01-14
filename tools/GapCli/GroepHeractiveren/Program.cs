using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Adf.ServiceModel;
using Chiro.Gap.UpdateSvc.Contracts;

namespace GroepHeractiveren
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.Error.WriteLine(Properties.Resources.Gebruik, Environment.CommandLine);
            }
            else
            {
                ServiceHelper.CallService<IUpdateService>(svc => svc.GroepDesactiveren(args[0], null));
            }
        }
    }
}
