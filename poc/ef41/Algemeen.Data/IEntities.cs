using System.Collections.Generic;

namespace Algemeen.Data
{
    public interface IEntities
    {
        void WijzigingenBewaren();
        IEnumerable<TEntiteit> Alles<TEntiteit>() where TEntiteit: class, IBasisEntiteit;
        TEntiteit Toevoegen<TEntiteit>(TEntiteit entiteit) where TEntiteit: class, IBasisEntiteit;
        TEntiteit Ophalen<TEntiteit>(int ID) where TEntiteit: class, IBasisEntiteit;
    }
}