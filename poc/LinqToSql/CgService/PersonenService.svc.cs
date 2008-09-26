using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CgDal;

namespace PersonenService
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

        public void PersoonUpdaten(Persoon persoon)
        {
            DataAccess.PersoonUpdaten(persoon);
        }
    }
}
