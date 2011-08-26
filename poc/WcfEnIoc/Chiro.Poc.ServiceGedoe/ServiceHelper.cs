using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Chiro.Poc.ServiceGedoe
{
    public static class ServiceHelper
    {
        public static TResult CallService<TContract, TResult>(Func<TContract, TResult> operatie) where TContract: class
        {
            TResult result;

            using (var client = new Client<TContract>())
            {
                result = client.Call(operatie);
            }

            return result;
        }
    }
}
