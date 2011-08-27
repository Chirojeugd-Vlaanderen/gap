using System;

namespace Chiro.Poc.ServiceGedoe
{
    public interface IClient<out TContract> : IDisposable where TContract : class
    {
        TResult Call<TResult>(Func<TContract, TResult> operatie);
    }
}
