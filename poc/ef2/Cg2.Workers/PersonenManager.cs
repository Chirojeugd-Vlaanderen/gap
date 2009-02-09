using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;

namespace Cg2.Workers
{
    public class PersonenManager
    {
        public Persoon Updaten(Persoon p, Persoon origineel)
        {
            PersonenDao dao = new PersonenDao();
            return dao.Updaten(p, origineel);
        }

        public Persoon Ophalen(int persoonID)
        {
            PersonenDao dao = new PersonenDao();
            return dao.Ophalen(persoonID);
        }

        public Persoon OphalenMetCommunicatie(int persoonID)
        {
            PersonenDao dao = new PersonenDao();

            return dao.OphalenMetCommunicatie(persoonID);
        }
    }
}
