using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CgDal
{
    /// <summary>
    /// Geeft aan of een entity nieuw, gewijzigd of verwijderd is.
    /// </summary>
    public enum EntityStatus
    {
        Geen = 0,
        Nieuw = 1,
        Gewijzigd = 2,
        Verwijderd = 3
    }

    /// <summary>
    /// We zullen alle gegenereerde entity's laten erven van BasisEntity,
    /// zodat alle entity's weten welke status ze hebben.
    /// Via partial classes krijgen we dat wel in orde.
    /// </summary>
    [DataContract]
    public class BasisEntity
    {
        /// <summary>
        /// De status van de entity: nieuw, gewijzigd of verwijderd
        /// </summary>

        [DataMember]
        public EntityStatus Status {get; set;}
    }
}
