// Met dank aan http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cg2.Adf.ServiceModel;

using Cg2.Orm;
using Cg2.ServiceContracts;

namespace MvcWebApp2.Models
{
    /// <summary>
    /// Model voor zaken die op de masterpage getoond moeten worden,
    /// </summary>
    public class MasterViewModel
    {
        public string Groepsnaam { get; set; }
        public string Gemeente { get; set; }
        public string StamNummer { get; set; }
    }
}
