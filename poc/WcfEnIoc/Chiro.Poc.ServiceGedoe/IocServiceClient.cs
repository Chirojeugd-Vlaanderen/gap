using System;
using Chiro.Poc.Ioc;

namespace Chiro.Poc.ServiceGedoe
{
    /// <summary>
    /// Implementatie van IServiceClient die aanroepen van service methods niet naar de WCF-service doorspeelt,
    /// maar de IOC-container gebruikt om implementaties van het service contract <typeparamref name="TContract"/> 
    /// te instantiëren.
    /// </summary>
    /// <typeparam name="TContract">Het servicecontract van de aan te spreken service</typeparam>
    public class IocServiceClient<TContract> : IServiceClient<TContract> where TContract : class
    {
        private readonly TContract _contractImplementatie;

        /// <summary>
        /// De constructor roept de IOC-container aan om een implementatie van het servicecontract
        /// te instantieren.
        /// </summary>
        public IocServiceClient()
        {
            _contractImplementatie = Factory.Maak<TContract>();
        }

        /// <summary>
        /// Roep een service method aan met een <typeparamref name="TResult"/> als resultaat
        /// </summary>
        /// <typeparam name="TResult">type van het resultaat van de service call</typeparam>
        /// <param name="operatie">lambda-expressie die omschrijft welke service call aangeroepen moet worden.</param>
        /// <returns>Het resultaat van de service call</returns>
        public TResult Call<TResult>(Func<TContract, TResult> operatie)
        {
            return operatie.Invoke(_contractImplementatie);
        }

        /// <summary>
        /// Ik voorzie een Dispose-functie, omdat de OnlineServiceClient (die erft van ClientBase) disposable is.
        /// Deze klasse moet de OnlineServiceClient kunnen vervangen, en moet dus ook disposable zijn.
        /// </summary>
        public void Dispose()
        {
            // Maak van de gelegenheid gebruik om zo nodig de implementerende klasse te disposen.
            if (_contractImplementatie is IDisposable)
            {
                (_contractImplementatie as IDisposable).Dispose();
            }
        }
    }
}
