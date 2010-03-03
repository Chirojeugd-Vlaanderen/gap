// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
    public class LedenModel : MasterViewModel 
    {
        public Dictionary<int, AfdelingInfo> AfdelingsInfoDictionary { get; set; }
        
        /// <summary>
		/// Bevat de huidige afdelingen van een lid, of de geselecteerde na de ui, voor leiding
        /// </summary>
		public List<int> AfdelingIDs { get; set; }
        
        /// <summary>
		/// Bevat de huidige of de nieuwe gewenste afdeling voor een kind
        /// </summary>
		public int AfdelingID { get; set; }

        public LidInfo HuidigLid { get; set; }

        public LedenModel() : base() { }
    }
}
