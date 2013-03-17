// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	public class LidInfoModel : LedenLijstModel
	{
		public enum SpecialeLedenLijst
		{
			Geen, Alles, Probeerleden, VerjaardagsLijst, OntbrekendAdres, OntbrekendTelefoonNummer, LeidingZonderEmail
		}

		public LidInfoModel()
		{
			WerkJaarInfos = new List<WerkJaarInfo>();
			LidInfoLijst = new List<LidOverzicht>();
			AfdelingsInfoDictionary = new Dictionary<int, AfdelingDetail>();
			SpecialeLijsten = new Dictionary<SpecialeLedenLijst, string>();
		}

		public int PageHuidig { get; set; }
		public int PageTotaal { get; set; }

		// TODO Aangezien in AfdelingDetail en FunctieDetail ook de ID's zitten, is het overbodig om hieronder dictionary's te gebruiken.

		public Dictionary<int, AfdelingDetail> AfdelingsInfoDictionary { get; set; }
		public Dictionary<int, FunctieDetail> FunctieInfoDictionary { get; set; }

		// De dictionary hieronder is dan wel weer nuttig.

		public Dictionary<SpecialeLedenLijst, string> SpecialeLijsten { get; set; }

		public int IDGetoondGroepsWerkJaar { get; set; }
		public int JaartalGetoondGroepsWerkJaar { get; set; }
		public int JaartalHuidigGroepsWerkJaar { get; set; }
		public IEnumerable<WerkJaarInfo> WerkJaarInfos { get; set; }

		public int AfdelingID { get; set; }
		public int FunctieID { get; set; }
		public SpecialeLedenLijst SpecialeLijst { get; set; }

		// Indien van toepassing: actie die uitgevoerd moet worden op alle leden
		// met GelieerdePersoonID in SelectieGelieerdePersoonIDs.

		public int GekozenActie { get; set; }
	}
}
