using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
    public interface  ICommunicatieTypeDao : IDao<CommunicatieType>
    {
        /// <summary>
        /// Haalt communicatietype op op basis van CommunicatieTypeID
        /// </summary>
        /// <param name="communicatieTypeID">ID van het communicatieType</param>
        /// <returns>CommunicatieType met de opgegeven ID</returns>
        CommunicatieType Ophalen(int communicatieTypeID);
    }
}
