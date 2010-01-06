using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// In het algemeen bevat het GelieerdePersonenModel informatie over slechts 1 persoon.
	/// Deze informatie zit dan in <c>HuidigePersoon</c>.
	/// 
	/// Wanneer dit model gebruikt wordt voor het toevoegen van een nieuwe persoon, dan
	/// bevat het ook mogelijke gelijkaardige personen (<c>GelijkaardigePersonen</c>) en
	/// een boolean <c>Forceer</c> die aangeeft of een nieuwe persoon geforceerd moet worden
	/// ondanks gevonden gelijkaardige personen.
	/// </summary>
	public class GelieerdePersonenModel : MasterViewModel
	{
		/// <summary>
		/// Informatie over een te tonen of te wijzigen persoon
		/// </summary>
		public GelieerdePersoon HuidigePersoon { get; set; }

		/// <summary>
		/// Lijst met info over eventueel gelijkaardige personen
		/// </summary>
		public IEnumerable<PersoonInfo> GelijkaardigePersonen { get; set; }

		/// <summary>
		/// Geeft aan of een persoon ook toegevoegd moet worden als er al gelijkaardige
		/// personen bestaan.
		/// </summary>
		public bool Forceer { get; set; }

		/// <summary>
		/// Standaardconstructor.
		/// </summary>
		public GelieerdePersonenModel() : base() 
		{
			Forceer = false;
		}
	}
}