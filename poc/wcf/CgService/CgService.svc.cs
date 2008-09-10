using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CgBll;
using CgDal;

namespace CgService
{
    // NOTE: If you change the class name "CgService" here, you must also update the reference to "CgService" in Web.config.

    /// <summary>
    /// CgService is de WCF-service die externe applicaties toelaat
    /// gegevens uit te wisselen met de DAL.
    /// </summary>
    public class CgService : ICgService
    {
        /// <summary>
        /// Hello is gewoon een testmethode, om te zien of de service antwoordt.
        /// </summary>
        /// <returns></returns>
        public String Hello()
        {
            return "Hallo, service!";
        }

        /// <summary>
        /// Haalt persoonsgegevens op
        /// </summary>
        /// <param name="persoonID">ID van persoon waarvan de gegevens opgevraagd moeten worden</param>
        /// <returns>De persoonsgegevens in een persoonsobject</returns>
        public Persoon PersoonGet(int persoonID)
        {
            return new PersoonBll().PersoonGet(persoonID);
        }

        /// <summary>
        /// Updatet persoonsgegevens.  Het oorspronkelijk persoonsobject is nodig om na te gaan
        /// welke persoon geupdatet moet worden, en of er ondertussen wijzigingen gebeurden in de
        /// database.
        /// </summary>
        /// <param name="bijgewerktePersoon">Te bewaren persoonsgegevens</param>
        /// <param name="oorspronkelijkePersoon">Oorspronkelijke gegevens persoon</param>
        public void PersoonUpdaten(Persoon bijgewerktePersoon, Persoon oorspronkelijkePersoon)
        {
            new PersoonBll().PersoonUpdaten(bijgewerktePersoon, oorspronkelijkePersoon);
        }
    }
}
