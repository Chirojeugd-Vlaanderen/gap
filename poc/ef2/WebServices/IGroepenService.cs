using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Orm;

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
        /// <remarks>FIXME: gedetailleerde exception</remarks>
        [OperationContract]
        Groep Updaten(Groep g, Groep origineel);

        /// <summary>
        /// Haalt groep op uit database
        /// </summary>
        /// <param name="groepID">ID van op te halen groep</param>
        /// <returns>het gevraagde groepsobject, of null indien niet gevonden.
        /// </returns>
        [OperationContract]
        [ServiceKnownType(typeof(Groep))]
        Groep Ophalen(int groepID);

        /// <summary>
        /// Functie om de service te testen
        /// </summary>
        /// <returns>Een teststring</returns>
        [OperationContract]
        string Hallo();
    }
}
