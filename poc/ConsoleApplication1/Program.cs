using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceReference1.IService1 service = new ServiceReference1.Service1Client();

            Console.WriteLine(service.GetData(4));
            Console.WriteLine(service.GetDataUsingDataContract(new ConsoleApplication1.ServiceReference1.CompositeType()));

            Console.ReadLine();
        }
    }
}
