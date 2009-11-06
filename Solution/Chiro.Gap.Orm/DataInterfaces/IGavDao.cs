using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
    /// <summary>
    /// Data access voor GAV's
    /// </summary>
    public interface IGavDao: IDao<Gav>
    {
        /// <summary>
        /// Haalt GAV-object op op basis van login
        /// </summary>
        /// <param name="login">deuheu</param>
        /// <returns>GAV horende bij gegeven login</returns>
        Gav Ophalen(string login);
    }
}
