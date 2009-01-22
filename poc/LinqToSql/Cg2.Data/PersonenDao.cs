using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.Domain;
using System.Data.Linq;

namespace Cg2.Data.LTS
{
    public class PersonenDao: Dao<Persoon>, IPersonenDao
    {
        public Persoon OphalenMetCommunicatie(int id)
        {
            Persoon result;

            using (Cg2DataContext db = new Cg2DataContext())
            {
                DataLoadOptions dlo = new DataLoadOptions();
                dlo.LoadWith<Persoon>(p => p.Communicatie);
                db.LoadOptions = dlo;

                Table<Persoon> tabel = db.GetTable(typeof(Persoon))
                    as Table<Persoon>;


                result = (
                    from t in tabel
                    where t.ID == id
                    select t).FirstOrDefault<Persoon>();
            }
            return result;
        }
    }
}
