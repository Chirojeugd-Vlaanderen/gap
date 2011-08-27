using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Chiro.Poc.Ioc;

namespace Chiro.Poc.ServiceGedoe
{
    public static class ServiceHelper
    {
        public static TResult CallService<TContract, TResult>(Func<TContract, TResult> operatie) where TContract: class
        {
            TResult result;

            var client = Factory.Maak<IServiceClient<TContract>>();

            using (client)
            {
                result = client.Call(operatie);
            }

            return result;
        }
    }
}
