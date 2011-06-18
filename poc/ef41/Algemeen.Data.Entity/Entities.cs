using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Algemeen.Data.Entity
{
    public class Entities : DbContext, IEntities
    {
        public void WijzigingenBewaren()
        {
            SaveChanges();
        }

        public IEnumerable<TEntiteit> Alles<TEntiteit>() where TEntiteit: class, IBasisEntiteit
        {
            return this.Set<TEntiteit>();
        }

        public TEntiteit Toevoegen<TEntiteit>(TEntiteit entiteit) where TEntiteit: class, IBasisEntiteit
        {
            return (TEntiteit) this.Set<TEntiteit>().Add(entiteit);
        }

        public TEntiteit Ophalen<TEntiteit>(int ID) where TEntiteit : class, IBasisEntiteit
        {
            return (TEntiteit) this.Set<TEntiteit>().Where(ent => ent.ID == ID).FirstOrDefault();
        }
    }
}