// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
    public class CommunicatieTypeDao : Dao<CommunicatieType, ChiroGroepEntities>, ICommunicatieTypeDao 
    {
        public override CommunicatieType Ophalen(int communicatieTypeID)
        {
            using (ChiroGroepEntities db = new ChiroGroepEntities())
            {
                return (
                    from ct in db.CommunicatieType
                    where ct.ID == communicatieTypeID
                    select ct).FirstOrDefault();
            }
        }
    }
}