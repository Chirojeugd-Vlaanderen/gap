// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

// Met dank aan http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor zaken die op de masterpage getoond moeten worden,
	/// </summary>
	public class MasterViewModel : IMasterViewModel
	{
		/// <summary>
		/// ID van de Chirogroep
		/// </summary>
		public int GroepID { get; set; }

		/// <summary>
		/// Naam van de Chirogroep
		/// </summary>
		public string GroepsNaam { get; set; }

		/// <summary>
		/// Plaats van de Chirogroep
		/// </summary>
		public string Plaats { get; set; }

		/// <summary>
		/// Het stamnummer wordt niet meer gebruikt als primary key, maar zal nog wel
		/// </summary>
		public string StamNummer { get; set; }

		/// <summary>
		/// Titel van de pagina
		/// </summary>
		public string Titel { get; set; }
	}
}
