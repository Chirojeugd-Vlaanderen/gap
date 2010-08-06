// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Workers;

namespace Chiro.WatiN.Test
{
	/// <summary>
	/// Een aantal helperfuncties voor de WatiN-tests
	/// </summary>
	public static class TestHelper
	{
		private static GroepenManager _gMgr;
		private static AutorisatieManager _auMgr;

		static TestHelper()
		{
			Factory.ContainerInit(); // Zeker zijn dat IOC-container geinitialiseerd is
			_gMgr = Factory.Maak<GroepenManager>();
			_auMgr = Factory.Maak<AutorisatieManager>();
		}

		/// <summary>
		/// Roept de businesslaag rechtstreeks op (zonder user interface) om de alle (gelieerde) personen
		/// van een testgroep te verwijderen
		/// </summary>
		/// <param name="groepID">ID van de op te kuisen groep</param>
		public static void KuisOp(int groepID)
		{
			_gMgr.GelieerdePersonenVerwijderen(groepID, true);
		}

		/// <summary>
		/// Geeft de gebruiker die de test runt GAV rechten op de groep met gegeven <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">groepID van de groep waarop GAV-rechten gegeven moeten worden</param>
		public static void GeefGavRecht(int groepID)
		{
			// geef gebruiker gebruikersrecht voor een dag
			_auMgr.GebruikersRechtToekennen(
				String.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName),
				groepID,
				DateTime.Now.AddDays(1));
		}

		/// <summary>
		/// Ontneemt de GAV-rechten op de groep met gegeven <paramref name="groepID"/> van de gebruiker
		/// die de test runt.
		/// </summary>
		/// <param name="groepID">groepID van de groep waarop GAV-rechten gegeven moeten worden</param>
		public static void OntneemGavRecht(int groepID)
		{
			// ontneem gebruikersrecht door vervaldatum op Nu te zetten.
			_auMgr.GebruikersRechtToekennen(
				String.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName),
				groepID,
				DateTime.Now);
		}

	}
}
