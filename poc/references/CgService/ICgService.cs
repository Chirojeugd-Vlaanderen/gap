using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CgDal;

namespace CgService
{
    // NOTE: If you change the interface name "ICgService" here, you must also update the reference to "ICgService" in Web.config.
    [ServiceContract]
    public interface ICgService
    {
        #region Persoonsdingen
        [OperationContract]
        Persoon PersoonGet(int persoonID);
        [OperationContract]
        List<PersoonsAdres> PersoonsAdressenGet(int persoonID);
        [OperationContract]
        void PersoonUpdaten(Persoon bijgewerktePersoon, Persoon oorspronkelijkePersoon);
        #endregion

        #region Adressen
        [OperationContract]
        void PersoonsAdresUpdaten(PersoonsAdres bijgewerktAdres, PersoonsAdres oorspronkelijkAdres);
        #endregion

        #region Adrestypes
        [OperationContract]
        AdresType WerkAdresType();
        [OperationContract]
        AdresType KotAdresType();
        [OperationContract]
        AdresType ThuisAdresType();
        [OperationContract]
        AdresType OnbekendAdresType();
        [OperationContract]
        #endregion

        String Hello();
    }

}
