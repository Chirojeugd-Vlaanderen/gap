using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CgDal
{
    /// <summary>
    /// We zullen alle gegenereerde entity's laten erven van BasisEntity,
    /// zodat alle entity's weten welke status ze hebben.
    /// Via partial classes krijgen we dat wel in orde.
    /// </summary>
    [DataContract]
    public class BasisEntity
    {
        /// <summary>
        /// vlag om aan de service duidelijk te maken dat een entity
        /// verwijderd moet worden.
        /// </summary>

        [DataMember]
        public bool TeVerwijderen {get; set;}
    }
}
