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
        private IPersonenDao _dao = new PersonenDao();

        /// <summary>
        /// Data Access Object dat aangesproken kan worden
        /// voor CRUD-operaties
        /// </summary>
        public IPersonenDao Dao
        {
            get { return _dao; }
        }

        /// <summary>
        /// Maak persoon lid
        /// </summary>
        /// <param name="p">Lid te maken persoon</param>
        /// <param name="g">Groep waarvan persoon lid moet worden</param>
        /// <returns>Persoon met bijhorend lidobject</returns>
        public Persoon LidMaken(Persoon p, Groep g)
        {
            throw new NotImplementedException();
        }
    }
}
