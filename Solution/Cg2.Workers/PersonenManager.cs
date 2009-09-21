using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;
using Cg2.Fouten.Exceptions;
using Cg2.Ioc;

namespace Cg2.Workers
{
    public class PersonenManager
    {
        private IPersonenDao _dao;

        public PersonenManager(IPersonenDao dao)
        {
            _dao = dao;
        }

        /// <summary>
        /// Verhuist een persoon van oudAdres naar nieuwAdres
        /// </summary>
        /// <param name="verhuizer">te verhuizen GelieerdePersoon</param>
        /// <param name="oudAdres">oud adres</param>
        /// <param name="nieuwAdres">nieuw adres</param>
        /// <remarks>Als de persoon niet gekoppeld is aan het oude adres,
        /// zal hij ook niet verhiuzen</remarks>
        public void Verhuizen(Persoon verhuizer, Adres oudAdres, Adres nieuwAdres)
        {
            PersoonsAdres persoonsadres
                = (from PersoonsAdres pa in verhuizer.PersoonsAdres
                  where pa.Adres.ID == oudAdres.ID
                  select pa).FirstOrDefault();

            if (oudAdres.PersoonsAdres != null)
            {
                oudAdres.PersoonsAdres.Remove(persoonsadres);
            }
            //TODO probleem dat nieuwadres attached is en persoon detached, waardoor hij de twee
            //contexten niet kan vergelijken.
            persoonsadres.Adres = nieuwAdres;

            if (nieuwAdres.PersoonsAdres != null)
            {
                nieuwAdres.PersoonsAdres.Add(persoonsadres);
            }
        }

        /// <summary>
        /// Koppelt het gegeven Adres via een nieuw PersoonsAdresObject
        /// aan de gegeven GelieerdePersoon
        /// </summary>
        /// <param name="p">GelieerdePersoon die er een adres bij krijgt</param>
        /// <param name="adres">Toe te voegen adres</param>
        public void AdresToevoegen(Persoon p, Adres adres)
        {
            PersoonsAdres pa = new PersoonsAdres { Adres = adres, Persoon = p, AdresTypeID = 1 };
            p.PersoonsAdres.Add(pa);
            adres.PersoonsAdres.Add(pa);
        }

        public IList<Persoon> LijstOphalen(List<int> personenIDs)
        {
            AutorisatieManager authMgr = Factory.Maak<AutorisatieManager>();

            return _dao.LijstOphalen(authMgr.EnkelMijnPersonen(personenIDs));
        }


    }
}
