// CgEntities.cs - deze file zorgt ervoor dat de entity's erven van BasisEntity.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CgDal
{
    public partial class Persoon : BasisEntity { };
    public partial class PersoonsAdres : BasisEntity { };
    public partial class AdresType : BasisEntity { };
    public partial class CommunicatieVorm : BasisEntity { };
}