using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Poc.ServiceGedoe
{
    public class Client<TContract> : System.ServiceModel.ClientBase<TContract> where TContract : class
    {
        public Client()
        {
        }

        public Client(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public Client(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public TResult Call<TResult>(Func<TContract,TResult> operatie)
        {
            return operatie.Invoke(Channel);
        }
    }
}
