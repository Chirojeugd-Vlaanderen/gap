using System;

namespace Chiro.Poc.ServiceGedoe
{
    /// <summary>
    /// IServiceClient is de interface die we gebruiken om een service method aan te spreken.
    /// </summary>
    /// <typeparam name="TContract">Het service contract van de aan te spreken service</typeparam>
    public interface IServiceClient<out TContract> : IDisposable where TContract : class
    {
        /// <summary>
        /// Roep een service method aan met een <typeparamref name="TResult"/> als resultaat
        /// </summary>
        /// <typeparam name="TResult">type van het resultaat van de service call</typeparam>
        /// <param name="operatie">lambda-expressie die omschrijft welke service call aangeroepen moet worden.</param>
        /// <returns>Het resultaat van de service call</returns>
        TResult Call<TResult>(Func<TContract, TResult> operatie);
    }
}
