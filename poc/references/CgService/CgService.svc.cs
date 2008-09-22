using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CgBll;
using CgDal;
using System.Diagnostics;

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

        #region Personen
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
        /// Haalt persoonsadressen op van de persoon met het gegeven ID
        /// </summary>
        /// <param name="persoonID">ID van de persoon waarvan de adressen opgevraagd moeten worden</param>
        /// <returns>Een lijst met objecten van het type PersoonsAdres</returns>
        public IList<PersoonsAdres> PersoonsAdressenGet(int persoonID)
        {
            return new PersoonBll().PersoonsAdressenGet(persoonID);
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
        #endregion

        #region Adressen
        /// <summary>
        /// Updatet een object van het type PersoonsAdres
        /// </summary>
        /// <param name="bijgewerktAdres">bijgewerkt object</param>
        /// <param name="oorspronkelijkAdres">oorspronkelijk object</param>
        public void PersoonsAdresUpdaten(PersoonsAdres bijgewerktAdres, PersoonsAdres oorspronkelijkAdres)
        {
            Debug.WriteLine("AdresTypeID van bijgewerktAdres: " + bijgewerktAdres.AdresType.AdresTypeID);
            new PersoonsAdresBll().PersoonsAdresUpdaten(bijgewerktAdres, oorspronkelijkAdres);
        }
        #endregion

        #region AdresTypes
        /// <summary>
        /// Levert het adrestype 'thuis' af
        /// </summary>
        /// <returns>adrestype 'thuis'</returns>
        public AdresType ThuisAdresType()
        {
            return new AdresTypeBll().Thuis;
        }

        /// <summary>
        /// Levert het adrestype 'werk' af
        /// </summary>
        /// <returns>adrestype 'werk'</returns>
        public AdresType WerkAdresType()
        {
            return new AdresTypeBll().Werk;
        }

        /// <summary>
        /// Levert het adrestype 'kot' af
        /// </summary>
        /// <returns>adrestype 'kot'</returns>
        public AdresType KotAdresType()
        {
            return new AdresTypeBll().Kot;
        }

        /// <summary>
        /// Levert het adrestype 'onbekend' af
        /// </summary>
        /// <returns>adrestype 'onbekend'</returns>
        public AdresType OnbekendAdresType()
        {
            return new AdresTypeBll().Overig;
        }
        #endregion
    }
}
