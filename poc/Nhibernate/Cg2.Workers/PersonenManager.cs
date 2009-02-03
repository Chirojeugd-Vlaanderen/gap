using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.Domain;
using Cg2.Core.DataInterfaces;
using Cg2.Data.Nh;

namespace Cg2.Workers
{
    public class PersonenManager: IPersonenManager
    {
        public int Updaten(Persoon p, Persoon origineel)
        {
            Dao<Persoon> dao = new Dao<Persoon>();
            return dao.Updaten(p, origineel).ID;
        }

        public Persoon Ophalen(int persoonID)
        {
            Dao<Persoon> dao = new Dao<Persoon>();
            return dao.Ophalen(persoonID);
        }

        public int Bewaren(Persoon p)
        {
            PersonenDao dao = new PersonenDao();
            return dao.Bewaren(p).ID;
        }

        public Persoon OphalenMetCommunicatie(int persoonID)
        {
            PersonenDao dao = new PersonenDao();
            return dao.OphalenMetCommunicatie(persoonID);
        }
    }
}
