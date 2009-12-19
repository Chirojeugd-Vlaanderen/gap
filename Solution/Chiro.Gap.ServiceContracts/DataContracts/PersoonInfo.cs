using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
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
