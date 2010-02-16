using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using System.Web.Mvc;

namespace Chiro.Gap.WebApp.Models
{
	public class PersoonInfoModel : MasterViewModel
	{
		public int HuidigePagina { get; set; }
		public int AantalPaginas { get; set; }
		public int Totaal { get; set; }

		public IList<PersoonInfo> PersoonInfos { get; set; }

		public List<int> GekozenGelieerdePersoonIDs { get; set; }
		public int GekozenActie { get; set; }

		public SelectList GroepsCategorieen { get; set; }

		public PersoonInfoModel() : base() { }
	}
}