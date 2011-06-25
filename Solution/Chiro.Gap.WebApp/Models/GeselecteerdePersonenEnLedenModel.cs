using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	public class GeselecteerdePersonenEnLedenModel: MasterViewModel
	{
		public GeselecteerdePersonenEnLedenModel()
		{
			PersoonEnLidInfos = new List<InTeSchrijvenLid>();
			BeschikbareAfdelingen = new List<ActieveAfdelingInfo>();
			GelieerdePersoonIDs = new List<int>();
			InTeSchrijvenGelieerdePersoonIDs = new List<int>();
			LeidingMakenGelieerdePersoonIDs = new List<int>();
			ToegekendeAfdelingsJaarIDs = new List<int>();
		}

		/// <summary>
		/// Een lijst met ID's van gelieerde personen, gebruikt voor selecties
		/// </summary>
		public IList<int> GelieerdePersoonIDs { get; set; }
		public IList<int> InTeSchrijvenGelieerdePersoonIDs { get; set; }
		public IList<int> LeidingMakenGelieerdePersoonIDs { get; set; }
		public IList<int> ToegekendeAfdelingsJaarIDs { get; set; }

		/// <summary>
		/// Een lijst met alle nodige persoons en leden informatie.
		/// </summary>
		public IList<InTeSchrijvenLid> PersoonEnLidInfos { get; set; }

		/// <summary>
		/// Lijst met de actieve afdelingen dit werkjaar
		/// </summary>
		public IEnumerable<ActieveAfdelingInfo> BeschikbareAfdelingen { get; set; }
	}
}