using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CgDal;

namespace PersonenService
{
    // NOTE: If you change the interface name "IService1" here, you must also update the reference to "IService1" in Web.config.
    [ServiceContract]
    public interface IPersonenService
    {

        /// <summary>
        /// Testmethode om te zien of de service werkt :-)
        /// </summary>
        /// <returns>Versienummer of zo</returns>
        [OperationContract]
        string Hallo();

        /// <summary>
        /// Haalt persoonsgegevens op op basis van ID
        /// </summary>
        /// <param name="persoonID">PersoonID van op te vragen persoon</param>
        /// <returns></returns>
        [OperationContract]
        Persoon PersoonGet(int persoonID);

        /// <summary>
        /// Persisteert de wijzigingen van het persoonsobject in de database.
        /// </summary>
        /// <param name="persoon">te persisteren persoonsobjec</param>
        [OperationContract]
        void PersoonUpdaten(Persoon persoon);

    }

}
