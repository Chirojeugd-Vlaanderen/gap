using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;

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
        /// Groepswerkjaar (koppelt de afdeling aan een groep en een werkjaar)
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
        /// Op basis van een ID een afdelingsjaar ophalen
        /// </summary>
        /// <param name="afdelingsJaarID">
        /// De ID van het afdelingsjaar
        /// </param>
        /// <param name="extras">
        /// Bepaalt welke gerelateerde entiteiten mee opgehaald moeten worden.
        /// </param>
        /// <returns>
        /// Het afdelingsjaar met de opgegeven ID
        /// </returns>
        AfdelingsJaar Ophalen(int afdelingsJaarID, AfdelingsJaarExtras extras);

        /// <summary>
        /// Verwijdert AfdelingsJaar uit database
        /// </summary>
        /// <param name="afdelingsJaarID">
        /// ID van het AfdelingsJaar
        /// </param>
        /// <returns>
        /// <c>True</c> on successful
        /// </returns>
        bool Verwijderen(int afdelingsJaarID);

        /// <summary>
        /// Het opgegeven afdelingsjaar opslaan
        /// </summary>
        /// <param name="aj">
        /// Het afdelingsjaar dat opgeslagen moet worden
        /// </param>
        void Bewaren(Afdeling aj);

        /// <summary>
        /// Het opgegeven afdelingsjaar opslaan
        /// </summary>
        /// <param name="aj">
        /// Het afdelingsjaar dat opgeslagen moet worden
        /// </param>
        void Bewaren(AfdelingsJaar aj);

        /// <summary>
        /// Wijzigt de property's van <paramref name="afdelingsJaar"/>
        /// </summary>
        /// <param name="afdelingsJaar">
        /// Te wijzigen afdelingsjaar
        /// </param>
        /// <param name="officieleAfdeling">
        /// Officiele afdeling
        /// </param>
        /// <param name="geboorteJaarVan">
        /// Ondergrens geboortejaar
        /// </param>
        /// <param name="geboorteJaarTot">
        /// Bovengrens geboortejaar
        /// </param>
        /// <param name="geslachtsType">
        /// Jongensafdeling, meisjesafdeling of gemengde afdeling
        /// </param>
        /// <param name="versieString">
        /// Versiestring uit database voor concurrency controle
        /// </param>
        /// <remarks>
        /// Groepswerkjaar en afdeling kunnen niet gewijzigd worden.  Verwijder hiervoor het
        /// afdelingsjaar, en maak een nieuw.
        /// </remarks>
        void Wijzigen(
            AfdelingsJaar afdelingsJaar,
            OfficieleAfdeling officieleAfdeling,
            int geboorteJaarVan,
            int geboorteJaarTot,
            GeslachtsType geslachtsType,
            string versieString);

        /// <summary>
        /// Haalt een afdeling op, op basis van <paramref name="afdelingID"/>
        /// </summary>
        /// <param name="afdelingID">
        /// ID van op te halen afdeling
        /// </param>
        /// <returns>
        /// De gevraagde afdeling
        /// </returns>
        Afdeling AfdelingOphalen(int afdelingID);

        /// <summary>
        /// Haalt lijst officiële afdelingen op.
        /// </summary>
        /// <returns>
        /// Lijst officiële afdelingen
        /// </returns>
        IList<OfficieleAfdeling> OfficieleAfdelingenOphalen();

        /// <summary>
        /// Haalt een officiele afdeling op, op basis van zijn ID
        /// </summary>
        /// <param name="officieleAfdelingID">
        /// ID van de op te halen officiele afdeling
        /// </param>
        /// <returns>
        /// Opgehaalde officiele afdeling
        /// </returns>
        OfficieleAfdeling OfficieleAfdelingOphalen(int officieleAfdelingID);

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
