using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
{
    [DataContract]
    public class StraatInfo
    {
        [DataMember]
        public int? PostNr { get; set; }

		[DataMember]
		public String Naam { get; set; }
    }
}
