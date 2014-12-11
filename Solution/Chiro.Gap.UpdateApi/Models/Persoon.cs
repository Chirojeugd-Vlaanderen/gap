using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chiro.Gap.UpdateApi.Models
{
    /// <summary>
    /// Dit is een erg beperkt model voor een persoon.
    /// 
    /// Op termijn kan de echte api gebruikt worden om updates van Chirocivi
    /// naar GAP te sturen.
    /// </summary>
    public class Persoon
    {
        public int PersoonId { get; set; }
        public int AdNummer { get; set; }
    }
}