using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.Domain;
using Cg2.Core.DataInterfaces;
using Cg2.Data.Ef;

namespace Cg2.Workers
{
    public class PersonenManager: IPersonenManager
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

        public Persoon Bewaren(Persoon p)
        {
            PersonenDao dao = new PersonenDao();
            return dao.Bewaren(p);
        }

        public void Verwijderen(Persoon p)
        {
            PersonenDao dao = new PersonenDao();
            dao.Verwijderen(p);
        }

        public Persoon OphalenMetCommunicatie(int persoonID)
        {
            PersonenDao dao = new PersonenDao();

            return dao.OphalenMetCommunicatie(persoonID);
        }


    }
}
