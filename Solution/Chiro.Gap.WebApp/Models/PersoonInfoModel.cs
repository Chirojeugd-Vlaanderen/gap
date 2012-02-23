// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	public class PersoonInfoModel : MasterViewModel
	{
        public PersoonInfoModel()
        {
            PersoonInfos = new List<PersoonDetail>();
            GekozenGelieerdePersoonIDs = new List<int>();
            GroepsCategorieen = new List<CategorieInfo>();
        }

		public int HuidigePagina { get; set; }
		public int AantalPaginas { get; set; }
		public int Totaal { get; set; }

		public IList<PersoonDetail> PersoonInfos { get; set; }

		public List<int> GekozenGelieerdePersoonIDs { get; set; }
		public int GekozenActie { get; set; }
		public int GekozenCategorieID { get; set; }
		public PersoonSorteringsEnum Sortering { get; set; }

		public IList<CategorieInfo> GroepsCategorieen { get; set; }
	}
}