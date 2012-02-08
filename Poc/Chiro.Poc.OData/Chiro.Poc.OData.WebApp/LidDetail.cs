using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chiro.Poc.OData.WebApp
{
    public class LidDetail
    {
        public int ID { get; set; }
        public string Naam { get; set; }
        public DateTime GeboorteDatum { get; set; }
        public string Afdeling { get; set; }
    }
}