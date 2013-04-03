using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Dummy repository. Bevat een lijst entiteiten van type <typeparamref name="T"/>, en voert
    /// zoekopdrachten uit op die lijst.
    /// </summary>
    /// <typeparam name="T">Basisentiteit waarvoor dit een repository is</typeparam>
    /// <remarks>Dit wordt gewoon gebruikt voor unit tests</remarks>
    public class DummyRepo<T>: IRepository<T> where T:BasisEntiteit
    {
        private IList<T> _entiteiten;

        public DummyRepo(IList<T> entiteiten)
        {
            _entiteiten = entiteiten;
        }

        public void Dispose()
        {
        }

        public IQueryable<T> Select()
        {
            return _entiteiten.AsQueryable();
        }

        public IEnumerable<T> GetAll()
        {
            return _entiteiten;
        }

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            return _entiteiten.Where(predicate);
        }

        public T GetSingle(Func<T, bool> predicate)
        {
            return _entiteiten.Single(predicate);
        }

        public T GetFirst(Func<T, bool> predicate)
        {
            return _entiteiten.First(predicate);
        }

        public T ByID(int id)
        {
            return _entiteiten.FirstOrDefault(en => en.ID == id);
        }

        public List<T> ByIDs(IEnumerable<int> IDs)
        {
            return _entiteiten.Where(src => IDs.Contains(src.ID)).ToList();
        }

        public void Add(T entity)
        {
            _entiteiten.Add(entity);
        }

        public void Delete(T entity)
        {
            _entiteiten.Remove(entity);
        }

        public void Delete(IList<T> entities)
        {
            foreach (var entity in entities)
            {
                _entiteiten.Remove(entity);
            }
        }

        public void Attach(T entity)
        {
            throw new NotImplementedException();
        }

        public int ContextHash { get; private set; }
        public void SaveChanges()
        {
        }
    }
}
