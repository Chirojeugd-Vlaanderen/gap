using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;

namespace BackendTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Factory.ContainerInit();
            var sh = new ServiceHelper(new ChannelFactoryChannelProvider());
            bool isLive = sh.CallService<IGroepenService, bool>(svc => svc.IsLive());
            Console.WriteLine("Done.");
        }
    }
}
