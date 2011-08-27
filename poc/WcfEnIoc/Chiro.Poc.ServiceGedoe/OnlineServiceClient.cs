using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Poc.ServiceGedoe
{
    public class OnlineServiceClient<TContract> : System.ServiceModel.ClientBase<TContract>, IServiceClient<TContract> where TContract : class
    {
        public OnlineServiceClient()
        {
        }

        public TResult Call<TResult>(Func<TContract,TResult> operatie)
        {
            return operatie.Invoke(Channel);
        }
    }
}
