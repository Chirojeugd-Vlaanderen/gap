using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm.DataInterfaces
{
    /// <summary>
    /// Data Access Object voor straten
    /// </summary>
    public interface IStratenDao: IDao<Straat>
    {
        /// <summary>
        /// Haalt straat op op basis van naam en postnummer
        /// </summary>
        /// <param name="naam">straatnaam</param>
        /// <param name="postNr">postnummer</param>
        /// <returns>relevante straat</returns>
        Straat Ophalen(string naam, int postNr);
    }
}
