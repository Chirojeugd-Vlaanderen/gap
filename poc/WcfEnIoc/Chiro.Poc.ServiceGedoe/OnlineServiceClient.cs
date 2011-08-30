using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Poc.ServiceGedoe
{
    /// <summary>
    /// Implementatie van IServiceClient die service methods van een WCF-service aanspreekt.  
    /// De nodige informatie over de WCF-service wordt opgehaald uit de App.Conf van de toepassing.
    /// </summary>
    /// <typeparam name="TContract">Het servicecontract van de aan te spreken service</typeparam>
    public class OnlineServiceClient<TContract> : System.ServiceModel.ClientBase<TContract>, IServiceClient<TContract> where TContract : class
    {
        /// <summary>
        /// Roep een service method aan met een <typeparamref name="TResult"/> als resultaat
        /// </summary>
        /// <typeparam name="TResult">type van het resultaat van de service call</typeparam>
        /// <param name="operatie">lambda-expressie die omschrijft welke service call aangeroepen moet worden.</param>
        /// <returns>Het resultaat van de service call</returns>
        public TResult Call<TResult>(Func<TContract,TResult> operatie)
        {
            return operatie.Invoke(Channel);
        }
    }
}
