using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
{
    [DataContract]
    public class LidInfo
    {
        [DataMember]
        public int LidID { get; set; }

        [DataMember]
        public PersoonInfo PersoonInfo { get; set; }

		/// <summary>
		/// Kind of leiding
		/// </summary>
		[DataMember]
		public LidType Type { get; set; }

        [DataMember]
        public bool LidgeldBetaald { get; set; }

		/// <summary>
		/// De datum van het einde van de instapperiode
		/// enkel voor kinderen en niet aanpasbaar
		/// </summary>
		[DataMember]
		public DateTime? EindeInstapperiode { get; set; }

		/// <summary>
		/// Geeft aan of het lid inactief is of niet
		/// </summary>
		[DataMember]
		public bool NonActief { get; set; }

		/// <summary>
		/// Geeft aan of de leid(st)er een abonnement heeft op dubbelpunt
		/// </summary>
		[DataMember]
		public bool? DubbelPunt { get; set; }

		/// <summary>
		/// De lijst van afdelingIDs waarin het lid zit (1 voor een kind)
		/// </summary>
        [DataMember]
        public IList<int> AfdelingIdLijst { get; set; }
    }
}
