// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

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
		public int GekozenCategorieID { get; set; }

		public IList<CategorieInfo> GroepsCategorieen { get; set; }

		public PersoonInfoModel() : base() { }
	}
}