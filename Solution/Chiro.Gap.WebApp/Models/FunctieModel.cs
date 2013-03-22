using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    public class FunctieModel: MasterViewModel
    {
        public FunctieDetail HuidigeFunctie { get; set; }
    }
}