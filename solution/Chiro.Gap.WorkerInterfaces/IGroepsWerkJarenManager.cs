using System;
using System.Collections.Generic;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    /// <summary>
    /// Worker interface voor GroepsWerkJaar operaties
    /// </summary>
	public interface IGroepsWerkJarenManager
	{

		/// <summary>
		/// Maakt een nieuw groepswerkjaar in het gevraagde werkJaar.
		/// Persisteert niet ;-P
		/// </summary>
		/// <param name="g">
		/// De groep waarvoor een groepswerkjaar aangemaakt moet worden
		/// </param>
		/// <returns>
		/// Het nieuwe groepswerkjaar
		/// </returns>
		/// <throws>OngeldigObjectException</throws>
		GroepsWerkJaar VolgendGroepsWerkJaarMaken(Groep g);

        /// <summary>
        /// Bepaalt of in het gegeven <paramref name='werkJaar' /> op
        /// het gegeven <paramref name='tijdstip' /> de jaarovergang al
        /// kan doorgaan.
        /// </summary>
        /// <param name="tijdstip"> </param>
        /// <param name="werkJaar">
        /// Jaartal van het 'huidige' werkjaar (i.e. 2010 voor 2010-2011 enz)
        /// </param>
        /// <returns>
        /// Datum in het gegeven werkjaar vanaf wanneer het nieuwe aangemaakt mag worden
        /// </returns>
        bool OvergangMogelijk(DateTime tijdstip, int werkJaar);

        /// <summary>
        /// Berekent wat het nieuwe werkjaar zal zijn als op deze moment de jaarovergang zou gebeuren.
        /// </summary>
        /// <returns>
        /// Het jaar waarin dat nieuwe werkJaar begint
        /// </returns>
        /// <remarks>De paramter <paramref name="groepID"/> is er enkel voor debugging purposes</remarks>
		int NieuweWerkJaar(int groepID);

		/// <summary>
		/// Bepaalt de datum vanaf wanneer het volgende werkJaar begonnen kan worden
		/// </summary>
		/// <param name="werkJaar">
		/// Jaartal van het 'huidige' werkJaar (i.e. 2010 voor 2010-2011 enz)
		/// </param>
		/// <returns>
		/// Datum in het gegeven werkJaar vanaf wanneer het nieuwe aangemaakt mag worden
		/// </returns>
		DateTime StartOvergang(int werkJaar);

        /// <summary>
        /// Controleert of de datum <paramref name="dateTime"/> zich in het werkJaar <paramref name="p"/> bevindt.
        /// </summary>
        /// <param name="dateTime">
        /// Te controleren datum
        /// </param>
        /// <param name="p">
        /// Werkjaar.  (2010 voor 2010-2011 enz.)
        /// </param>
        /// <returns>
        /// <c>True</c> als <paramref name="dateTime"/> zich in het werkJaar bevindt; anders <c>false</c>.
        /// </returns>
        bool DatumInWerkJaar(DateTime dateTime, int p);


        /// <summary>
        /// Berekent de theoretische einddatum van het gegeven groepswerkjaar.
        /// </summary>
        /// <param name="groepsWerkJaar">
        /// Groepswerkjaar, met daaraan gekoppeld een werkjaarobject
        /// </param>
        /// <returns>
        /// Einddatum van het gekoppelde werkJaar.
        /// </returns>
        DateTime EindDatum(GroepsWerkJaar groepsWerkJaar);

        /// <summary>
        /// Stelt afdelingsjaren voor voor de gegeven <paramref name="groep"/> en <paramref name="afdelingen"/>
        /// in het werkjaar <paramref name="nieuwWerkJaar"/> - <paramref name="nieuwWerkJaar"/>+1.
        /// </summary>
        /// <param name="groep">Groep waarvoor afdelingsjaren moeten worden voorgesteld, met daaraan gekoppeld
        /// het huidige groepswerkjaar, de huidige afdelingsjaren, en alle beschikbare afdelingen.</param>
        /// <param name="afdelingen">Afdelingen waarvoor afdelingsjaren moeten worden voorgesteld</param>
        /// <param name="nieuwWerkJaar">Bepaalt het werkjaar waarvoor de afdelingsjaren voorgesteld moeten worden.</param>
        /// <returns>Lijstje afdelingsjaren</returns>
        IList<AfdelingsJaar> AfdelingsJarenVoorstellen(ChiroGroep groep, IList<Afdeling> afdelingen, int nieuwWerkJaar);

        GroepsWerkJaar RecentsteOphalen(int groepID);
	}
}