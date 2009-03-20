using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Orm;

namespace Cg2.ServiceContracts
{
    // NOTE: If you change the interface name "IGroepenService" here, you must also update the reference to "IGroepenService" in Web.config.
    [ServiceContract]
    public interface IGroepenService
    {
        /// <summary>
        /// Persisteert een groep in de database
        /// </summary>
        /// <param name="g">Te persisteren groep</param>
        /// <returns>De persoon met eventueel gewijzigde informatie</returns>
        /// <remarks>FIXME: gedetailleerde exception</remarks>
        [OperationContract]
        Groep Updaten(Groep g);

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


        /*
            acties op GROEP g
            groepophalen(searcharg)
            groepupdate(g)
            =>nieuwe groep
            groepverwijderen(g)? (JV: enkel nuttig voor eventueel admingedeelte)
            groepophalenmetadressen(g)
            groepophalenmetpersonen(g) => alle standaardpersonen dus (JV: wat zijn standaardpersonen? Alles lijkt me sowieso te veel)
            groepophalenmetfuncties(g)
            groepophalenmetvrijevelden(g)
            bivakorganiseren(g, b)
            stelGAVin
            verwijderGAV (JV: ik zou hier de 'VervalDatum' op nu instellen, zodat geregistreerd blijft dat iemand ooit gav was)
            maaknieuwesatelliet(g, s)
            afdelingsjaartoevoegen(g)
            afdelingenreorganiseren(g) (JV: bedoel je het bewaren van de afdelingen gekoppeld aan de groep?)
            afdelingsjaarverwijderen(g) (JV: kan enkel als er geen leden in dat afdelingsjaar zijn/waren)
            afdelingtoevoegen
         */
    }
}
