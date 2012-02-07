using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chiro.Gap.Diagnostics.WebApp.Models
{
    /// <summary>
    /// Eenvoudig model voor samenvatting functieproblemen
    /// </summary>
    public class FunctiesIndexModel
    {
        public int AantalProblemen { get; set; }
        public string ProblemenRapportUrl { get; set; }
    }
}