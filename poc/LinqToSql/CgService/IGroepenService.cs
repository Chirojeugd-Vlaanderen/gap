using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CgDal;

namespace CgService
{
    // NOTE: If you change the interface name "IGroepenService" here, you must also update the reference to "IGroepenService" in Web.config.
    [ServiceContract]
    public interface IGroepenService
    {
        /// <summary>
        /// Haalt Chirogroep (incl. groepsgegevens) op met gegeven ID
        /// </summary>
        /// <param name="groepID">ID gevraagde Chirogroep</param>
        /// <returns>gevraagde Chirogroep</returns>
        /// <remarks>Functie werkt niet.  Gebruik ChiroGroepGroepGet</remarks>
        [OperationContract]
        ChiroGroep ChiroGroepGet(int groepID);


        /// <summary>
        /// Haalt groepsobject op voor een gegeven Chirogroep
        /// </summary>
        /// <param name="groepID">ID voor de gevraagde Chirogroep</param>
        /// <returns>Een groepsobject, incl. de gegevens van de Chirogroep</returns>
        [OperationContract]
        Groep ChiroGroepGroepGet(int groepID);
    }
}
