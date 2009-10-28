// Met dank aan http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Adf.ServiceModel;

using Chiro.Gap.Orm;
using Cg2.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model voor zaken die op de masterpage getoond moeten worden,
    /// </summary>
    public class MasterViewModel
    {
        /// <summary>
        /// ID van de Chirogroep
        /// </summary>
        public int GroepID { get; set; }

        /// <summary>
        /// Naam van de Chirogroep
        /// </summary>
        public string Groepsnaam { get; set; }

        /// <summary>
        /// Plaats van de Chirogroep
        /// </summary>
        public string Plaats { get; set; }

        /// <summary>
        /// Nationaal Stamnummer
        /// </summary>
        public string StamNummer { get; set; }

        /// <summary>
        /// Titel van de pagina
        /// </summary>
        public string Title { get; set; }

    }
}
