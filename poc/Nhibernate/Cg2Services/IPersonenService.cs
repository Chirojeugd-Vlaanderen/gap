using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Core.Domain;

namespace Cg2Services
{
    // NOTE: If you change the interface name "IPersonenService" here, you must also update the reference to "IPersonenService" in Web.config.
    [ServiceContract]
    [ServiceKnownType(typeof(Persoon))]
    [ServiceKnownType(typeof(GeslachtsType))]
    [ServiceKnownType(typeof(CommunicatieVorm))]
    public interface IPersonenService
    {
        /// <summary>
        /// Haalt persoon op uit database
        /// </summary>
        /// <param name="persoonID">ID op te halen persoon</param>
        /// <returns>Het gevraagde persoonsobject, null als niets gevonden
        /// is.</returns>
        [OperationContract]
        Persoon Ophalen(int persoonID);

        /// <summary>
        /// Persisteert een transiente persoon
        /// </summary>
        /// <param name="p">te persisteren persoon</param>
        /// <returns>Toegekend ID</returns>
        [OperationContract]
        int Bewaren(Persoon p);

        /// <summary>
        /// Maakt een persistent persoon opnieuw transient
        /// </summary>
        /// <param name="p">persoon die uit storage verwijderd moet
        /// worden</param>
        [OperationContract]
        void Verwijderen(Persoon p);

        /// <summary>
        /// Updatet een persoon in de database
        /// </summary>
        /// <param name="nieuw">Te updaten persoon</param>
        /// <param name="oorspronkelijk">Indien beschikbaar, is dit de 
        /// oorspronkelijke persoon.  Mag null zijn.</param>
        /// <returns>'Rowversion' (timestamp) van de nieuwe persoon</returns>
        [OperationContract]
        byte[] Updaten(Persoon nieuw, Persoon oorspronkelijk);

        /// <summary>
        /// Bewaart een transiente persoon, of updatet een detached
        /// persoon.
        /// </summary>
        /// <param name="p">Te bewaren/updaten persoon</param>
        /// <returns>Toegekend ID</returns>
        [OperationContract]
        int BewarenOfUpdaten(Persoon p);

        /// <summary>
        /// Haalt persoon op uit database, inclusief communicatiegegevens
        /// </summary>
        /// <param name="persoonID">ID op te halen persoon</param>
        /// <returns>Persoonsobject waarbij member Communicatie de
        /// communicatievormen van de persoon bevat.  Als de persoon niet
        /// werd gevonden, is het resultaat null.</returns>
        [OperationContract]
        Persoon OphalenMetCommunicatie(int persoonID);

        /// <summary>
        /// Enkel om te testen
        /// </summary>
        /// <returns>Een teststring</returns>
        [OperationContract]
        string Hallo();
    }
}
