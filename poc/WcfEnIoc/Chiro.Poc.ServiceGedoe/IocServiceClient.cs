using System;
using Chiro.Poc.Ioc;

namespace Chiro.Poc.ServiceGedoe
{
    public class IocServiceClient<TContract> : IServiceClient<TContract> where TContract : class
    {
        private readonly TContract _channel;

        public IocServiceClient()
        {
            _channel = Factory.Maak<TContract>();
        }

        public TResult Call<TResult>(Func<TContract, TResult> operatie)
        {
            return operatie.Invoke(_channel);
        }

        public void Dispose()
        {
            if (_channel is IDisposable)
            {
                (_channel as IDisposable).Dispose();
            }
        }
    }
}
