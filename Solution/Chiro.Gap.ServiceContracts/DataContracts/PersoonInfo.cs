// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
{
    [DataContract]
    public class PersoonInfo
    {
        [DataMember]
        public int? AdNummer { get; set; }

        [DataMember]
        public int GelieerdePersoonID { get; set; }

		[DataMember]
		public int PersoonID { get; set; }

        [DataMember]
        public string VolledigeNaam { get; set; }

        [DataMember]
        public DateTime? GeboorteDatum { get; set; }

        [DataMember]
        public GeslachtsType Geslacht { get; set; }

        [DataMember]
        public Boolean IsLid { get; set; }

        [DataMember]
        public IList<Categorie> CategorieLijst { get; set; }
    }
}
