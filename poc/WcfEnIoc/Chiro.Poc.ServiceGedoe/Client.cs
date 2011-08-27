using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Poc.ServiceGedoe
{
    public class Client<TContract> : System.ServiceModel.ClientBase<TContract>, IClient<TContract> where TContract : class
    {
        public Client()
        {
        }

        public TResult Call<TResult>(Func<TContract,TResult> operatie)
        {
            return operatie.Invoke(Channel);
        }
    }
}
