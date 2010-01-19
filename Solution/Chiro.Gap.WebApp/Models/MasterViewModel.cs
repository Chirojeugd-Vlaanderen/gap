// Met dank aan http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chiro.Adf.ServiceModel;

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
	/// ID van de Chirogroep
	/// </summary>
        public string GroepsNaam { get; set; }

        /// <summary>
        /// Plaats van de Chirogroep
        /// </summary>
        public string Plaats { get; set; }

        /// <summary>
        /// Het stamnummer wordt niet meer gebruikt als primary key, maar zal nog wel
	/// lang gebruikt worden als handige manier om een groep op te zoeken.
        /// </summary>
        public string StamNummer { get; set; }

        /// <summary>
        /// Titel van de pagina
        /// </summary>
        public string Titel { get; set; }

    }
}
