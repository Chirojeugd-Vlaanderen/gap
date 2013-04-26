using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chiro.Sso.DecryptieTool
{
    [Serializable]
    public class UserInfo
    {
        public string Naam { get; set; }
        public string StamNr { get; set; }
        public string Email { get; set; }
        public DateTime Datum { get; set; }
    }
}