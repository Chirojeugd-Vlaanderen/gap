using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CgDal;

namespace CgService
{
    public class PersonenService : IPersonenService
    {
        PersoonDataAccess DataAccess { get; set; }

        public PersonenService()
        {
            DataAccess = new PersoonDataAccess();
        }

        public string Hallo()
        {
            return "Hallo service!";
        }

        public Persoon PersoonGet(int persoonID)
        {
            return DataAccess.PersoonGet(persoonID);
        }

        public Persoon PersoonMetAdressenGet(int persoonID)
        {
            return DataAccess.PersoonMetAdressenGet(persoonID);
        }

        public IList<vPersoonsInfo> GelieerdePersonenInfoGet(int GroepID)
        {
            return DataAccess.GelieerdePersonenInfoGet(GroepID);
        }

        public int PersoonUpdaten(Persoon persoon)
        {
            return DataAccess.PersoonUpdaten(persoon);
        }
    }
}
