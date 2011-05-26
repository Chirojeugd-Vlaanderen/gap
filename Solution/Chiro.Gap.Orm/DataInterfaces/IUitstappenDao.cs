using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
    /// <summary>
    /// Interface voor data access voor uitstappen
    /// </summary>
    public interface IUitstappenDao : IDao<Uitstap>
    {
        /// <summary>
        /// Haalt alle uitstappen van een gegeven groep op.
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <param name="inschrijvenMogelijk">Als dit <c>true</c> is, worden enkel de uitstappen van het
        /// huidige werkjaar van de groep opgehaald.</param>
        /// <returns>Details van uitstappen</returns>
        IEnumerable<Uitstap> OphalenVanGroep(int groepID, bool inschrijvenMogelijk);

        /// <summary>
        /// Haalt de deelnemers (incl. lidgegevens van het betreffende groepswerkjaar)
        /// van de gegeven uitstap op.
        /// </summary>
        /// <param name="uitstapID">ID van uitstap waarvan deelnemers op te halen zijn</param>
        /// <returns>De deelnemers van de gevraagde uitstap.</returns>
    	IEnumerable<Deelnemer> DeelnemersOphalen(int uitstapID);

        /// <summary>
        /// Haalt alle bivakken op van alle groepen, uit gegeven <paramref name="werkjaar"/>,
        /// inclusief bivakplaats (met adres), contactpersoon, groepswerkjaar (met groep)
        /// </summary>
        /// <param name="werkjaar">Werkjaar waarvan de bivakken opgehaald moeten worden.</param>
        /// <returns>Alle bivakken uit het <paramref name="werkjaar"/></returns>
    	IEnumerable<Uitstap> AlleBivakkenOphalen(int werkjaar);
    }
}
