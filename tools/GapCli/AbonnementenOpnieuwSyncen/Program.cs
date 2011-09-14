using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbonnementenOpnieuwSyncen.Properties;

namespace AbonnementenOpnieuwSyncen
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() != 1)
            {
                Console.WriteLine(Resources.Gebruik, Environment.CommandLine);
                Console.ReadLine();
            }
            else
            {
                using (var svc = new UpdateServiceReference.UpdateServiceClient())
                {
                    svc.AbonnementenOpnieuwSyncen(args[0]);
                }
            }
        }
    }
}
