using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Core.Domain;

namespace WebServices
{
    // NOTE: If you change the interface name "IGroepenService" here, you must also update the reference to "IGroepenService" in Web.config.
    [ServiceContract]
    public interface IGroepenService
    {
        /// <summary>
        /// Persisteert een groep in de database
        /// </summary>
        /// <param name="g">Te persisteren groep</param>
        /// <param name="origineel">Het bewaren zal sneller gaan als een
        /// oorspronkelijk (ongewijzigd) object meegegeven wordt.  Zonder
        /// gaat het ook; geef dan null mee als origineel.
        /// </param>
        /// <returns>De persoon met eventueel gewijzigde informatie</returns>
        [OperationContract]
        Groep Updaten(Groep g, Groep origineel);
    }
}
