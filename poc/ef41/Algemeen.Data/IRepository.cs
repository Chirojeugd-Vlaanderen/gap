using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Algemeen.Data
{
    public interface IRepository
    {
        void WijzigingenBewaren();
        IEnumerable<TEntiteit> Alles<TEntiteit>() where TEntiteit: class, IBasisEntiteit;
        TEntiteit Toevoegen<TEntiteit>(TEntiteit entiteit) where TEntiteit: class, IBasisEntiteit;
        TEntiteit Ophalen<TEntiteit>(int ID) where TEntiteit: class, IBasisEntiteit;
        TEntiteit Ophalen<TEntiteit>(int ID, params Expression<Func<TEntiteit, object>>[] paths) where TEntiteit : class, IBasisEntiteit;
        void Verwijderen<TEntiteit>(TEntiteit entiteit) where TEntiteit : class, IBasisEntiteit;
    }
}