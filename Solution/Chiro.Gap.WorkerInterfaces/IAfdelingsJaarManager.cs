using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IAfdelingsJaarManager
    {
        /// <summary>
        /// Maakt een afdelingsjaar voor een groep en een afdeling, persisteert niet.
        /// </summary>
        /// <param name="a">
        /// Afdeling voor nieuw afdelingsjaar
        /// </param>
        /// <param name="oa">
        /// Te koppelen officiële afdeling
        /// </param>
        /// <param name="gwj">
        /// Groepswerkjaar (koppelt de afdeling aan een groep en een werkJaar)
        /// </param>
        /// <param name="geboorteJaarBegin">
        /// Geboortejaar van
        /// </param>
        /// <param name="geboorteJaarEind">
        /// Geboortejaar tot
        /// </param>
        /// <param name="geslacht">
        /// Bepaalt of de afdeling een jongensafdeling, meisjesafdeling of
        /// gemengde afdeling is.
        /// </param>
        /// <returns>
        /// Het aangemaakte afdelingsjaar
        /// </returns>
        AfdelingsJaar Aanmaken(
            Afdeling a,
            OfficieleAfdeling oa,
            GroepsWerkJaar gwj,
            int geboorteJaarBegin,
            int geboorteJaarEind,
            GeslachtsType geslacht);

        /// <summary>
        /// De afdelingen van het gegeven lid worden aangepast van whatever momenteel zijn afdelingen zijn naar
        /// de gegeven lijst nieuwe afdelingen.
        /// Een kind mag maar 1 afdeling hebben, voor een leider staan daar geen constraints op.
        /// Persisteert, want ingeval van leiding kan het zijn dat er links lid-&gt;afdelingsjaar moeten 
        /// verdwijnen.
        /// </summary>
        /// <param name="l">
        /// Lid, geladen met groepswerkjaar met afdelingsjaren
        /// </param>
        /// <param name="afdelingsJaren">
        /// De afdelingsjaren waarvan het kind lid is
        /// </param>
        /// <returns>
        /// Lidobject met gekoppeld(e) afdelingsja(a)r(en)
        /// </returns>
        Lid Vervangen(Lid l, IEnumerable<AfdelingsJaar> afdelingsJaren);

        /// <summary>
        /// De gegeven <paramref name="leden"/> worden toegevoegd naar
        /// de gegeven lijst nieuwe afdelingen.  Eventuele koppelingen met bestaande afdelingen worden
        /// verwijderd.
        /// <para>
        /// </para>
        /// Een kind mag maar 1 afdeling hebben, voor een leider staan daar geen constraints op.
        /// Persisteert, want ingeval van leiding kan het zijn dat er links lid-&gt;afdelingsjaar moeten 
        /// verdwijnen.
        /// </summary>
        /// <param name="leden">
        /// Leden, geladen met groepswerkjaar met afdelingsjaren
        /// </param>
        /// <param name="afdelingsJaren">
        /// De afdelingsjaren waaraan de leden gekoppeld moeten worden
        /// </param>
        /// <returns>
        /// Lidobjecten met gekoppeld(e) afdelingsja(a)r(en)
        /// </returns>
        IEnumerable<Lid> Vervangen(IEnumerable<Lid> leden, IEnumerable<AfdelingsJaar> afdelingsJaren);
    }
}
