using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Orm.DataInterfaces
{
    /// <summary>
    /// Met een GelieerdePersoon moet altijd het geassocieerde
    /// persoonsobject meekomen, anders heeft het weinig zin.
    /// </summary>
    public interface IGelieerdePersonenDao: IDao<GelieerdePersoon>
    {
        IList<GelieerdePersoon> AllenOphalen(int GroepID);
        IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald);
    }
}
