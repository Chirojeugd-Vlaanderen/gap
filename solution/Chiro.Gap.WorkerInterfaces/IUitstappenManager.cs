using System.Collections.Generic;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IUitstappenManager
    {
        /// <summary>
        /// Koppelt een plaats aan een uitstap
        /// </summary>
        /// <param name="uitstap">
        /// Te koppelen uitstap
        /// </param>
        /// <param name="plaats">
        /// Te koppelen plaats
        /// </param>
        /// <returns>
        /// Uitstap, met plaats gekoppeld.  Persisteert niet
        /// </returns>
        Uitstap Koppelen(Uitstap uitstap, Plaats plaats);

        /// <summary>
        /// Bepaalt of het de tijd van het jaar is voor de bivakaangifte
        /// </summary>
        /// <param name="groepsWerkJaar">Huidige groepswerkjaar</param>
        /// <returns><c>True</c> als de bivakaangifte voor <paramref name="groepsWerkJaar"/> moet worden doorgegeven, 
        /// anders <c>false</c></returns>
        bool BivakAangifteVanBelang(GroepsWerkJaar groepsWerkJaar);

        /// <summary>
        /// Bepaalt de status van de gegeven <paramref name="uitstap"/>
        /// </summary>
        /// <param name="uitstap">Uitstap, waarvan status bepaald moet worden</param>
        /// <returns>De status van de gegeven <paramref name="uitstap"/></returns>
        BivakAangifteStatus StatusBepalen(Uitstap uitstap);

        /// <summary>
        /// Nagaan of alle vereisten voldaan zijn om de opgegeven gelieerde personen allemaal in te schrijven
        /// voor de opgegeven uitstap.
        /// </summary>
        /// <param name="uitstap">De uitstap waar we mensen voor willen inschrijven</param>
        /// <param name="gelieerdePersonen">De mensen die we willen inschrijven</param>
        /// <returns><c>True</c> als alle voorwaarden voldaan zijn, anders <c>false</c></returns>
        bool InschrijvingenValideren(Uitstap uitstap, List<GelieerdePersoon> gelieerdePersonen);
    }
}