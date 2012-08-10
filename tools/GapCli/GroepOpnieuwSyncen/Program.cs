using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroepOpnieuwSyncen.Properties;

namespace GroepOpnieuwSyncen
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
                throw new NotImplementedException();

                // TODO: Als ik dit nog ooit zou gebruiken, moet dit opnieuw geimplementeerd worden met servicehelper.

                //using (var svc = new GapUpdateService.UpdateServiceClient())
                //{
                //    svc.OpnieuwSyncen(args[0]);
                //}
            }
        }
    }
}
