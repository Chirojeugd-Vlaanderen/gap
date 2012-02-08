using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chiro.Poc.OData.WebApp
{
    public static class Business
    {
        public static List<LidDetail> AlleLedenOphalen()
        {
            return new List<LidDetail>
                       {
                           new LidDetail
                               {
                                   ID = 1,
                                   Naam = "Nielske",
                                   Afdeling = "Speelclub",
                                   GeboorteDatum = new DateTime(2003, 07, 22)
                               },
                           new LidDetail
                               {
                                   ID = 2,
                                   Naam = "Warke",
                                   Afdeling = "Sloebers",
                                   GeboorteDatum = new DateTime(2004, 07, 29)
                               },
                           new LidDetail
                               {
                                   ID = 4,
                                   Naam = "Stanneke",
                                   Afdeling = "Rakwi's",
                                   GeboorteDatum = new DateTime(2001, 04, 07)
                               }
                       };
        }
    }
}