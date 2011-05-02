using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    public class UitstapModel : MasterViewModel
    {
        public UitstapDetail Uitstap { get; set; }
    }
}