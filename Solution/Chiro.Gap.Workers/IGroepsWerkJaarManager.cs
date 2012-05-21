using System;
using System.Collections.Generic;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Workers
{
	public interface IGroepsWerkJaarManager
	{
		/// <summary>
		/// Haalt het groepswerkjaar op bij een gegeven <paramref name="groepsWerkJaarID"/>
		/// </summary>
		/// <param name="groepsWerkJaarID">
		/// ID van het gevraagde GroepsWerkJaar
		/// </param>
		/// <param name="extras">
		/// Bepaalt op te halen gerelateerde entiteiten
		/// </param>
		/// <returns>
		/// Gevraagde groepswerkjaar
		/// </returns>
		GroepsWerkJaar Ophalen(int groepsWerkJaarID, GroepsWerkJaarExtras extras);

		/// <summary>
		/// Haalt de afdelingen van een groep op die niet gebruikt zijn in een gegeven 
		/// groepswerkjaar, op basis van een <paramref name="groepsWerkJaarID"/>
		/// </summary>
		/// <param name="groepsWerkJaarID">
		/// ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
		/// opgezocht moeten worden.
		/// </param>
		/// <returns>
		/// De ongebruikte afdelingen van een groep in het gegeven groepswerkjaar
		/// </returns>
		IList<Afdeling> OngebruikteAfdelingenOphalen(int groepsWerkJaarID);

		/// <summary>
		/// Haalt recentste groepswerkjaar voor een groep op.
		/// </summary>
		/// <param name="groepID">
		/// ID gevraagde groep
		/// </param>
		/// <returns>
		/// Het recentste Groepswerkjaar voor de opgegeven groep
		/// </returns>
		GroepsWerkJaar RecentsteOphalen(int groepID);

		/// <summary>
		/// Haalt recentste groepswerkjaar voor een groep op.
		/// </summary>
		/// <param name="groepID">
		/// ID gevraagde groep
		/// </param>
		/// <param name="extras">
		/// Bepaalt eventuele mee op te halen gekoppelde entiteiten
		/// </param>
		/// <returns>
		/// Het recentste Groepswerkjaar voor de opgegeven groep
		/// </returns>
		GroepsWerkJaar RecentsteOphalen(int groepID, GroepsWerkJaarExtras extras);

		/// <summary>
		/// Haalt recentste groepswerkjaar voor een groep op.
		/// </summary>
		/// <param name="code">
		/// Stamnummer van de gevraagde groep
		/// </param>
		/// <param name="extras">
		/// Bepaalt eventuele mee op te halen gekoppelde entiteiten
		/// </param>
		/// <returns>
		/// Het recentste Groepswerkjaar voor de opgegeven groep
		/// </returns>
		GroepsWerkJaar RecentsteOphalen(string code, GroepsWerkJaarExtras extras);

		/// <summary>
		/// Bepaalt ID van het recentste GroepsWerkJaar gemaakt voor een
		/// gegeven groep.
		/// </summary>
		/// <param name="groepID">
		/// ID van Groep
		/// </param>
		/// <returns>
		/// ID van het recentste GroepsWerkJaar
		/// </returns>
		int RecentsteGroepsWerkJaarIDGet(int groepID);

		/// <summary>
		/// Maakt een nieuw groepswerkjaar in het gevraagde werkjaar.
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
		/// Berekent wat het nieuwe werkjaar zal zijn als op deze moment de jaarovergang zou gebeuren.
		/// </summary>
		/// <returns>
		/// Het jaar waarin dat nieuwe werkjaar begint
		/// </returns>
		int NieuweWerkJaar();

		/// <summary>
		/// Bepaalt de datum vanaf wanneer het volgende werkjaar begonnen kan worden
		/// </summary>
		/// <param name="werkJaar">
		/// Jaartal van het 'huidige' werkjaar (i.e. 2010 voor 2010-2011 enz)
		/// </param>
		/// <returns>
		/// Datum in het gegeven werkjaar vanaf wanneer het nieuwe aangemaakt mag worden
		/// </returns>
		DateTime StartOvergang(int werkJaar);

		/// <summary>
		/// Persisteert een groepswerkjaar in de database
		/// </summary>
		/// <param name="gwj">
		/// Te persisteren groepswerkjaar, gekoppeld aan de groep
		/// </param>
		/// <param name="groepsWerkJaarExtras">
		/// Bepaalt welke gerelateerde entiteiten mee gepersisteerd
		/// moeten worden
		/// </param>
		/// <returns>
		/// Het gepersisteerde groepswerkjaar, met eventuele nieuwe ID's
		/// </returns>
		GroepsWerkJaar Bewaren(GroepsWerkJaar gwj, GroepsWerkJaarExtras groepsWerkJaarExtras);
	}
}