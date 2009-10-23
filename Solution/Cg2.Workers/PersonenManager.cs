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
        private IAutorisatieManager _autorisatieMgr;

        public PersonenManager(IPersonenDao dao, IAutorisatieManager autorisatieMgr)
        {
            _dao = dao;
            _autorisatieMgr = autorisatieMgr;
        }

        /// <summary>
        /// Haalt personen op die een adres gemeen hebben met de 
        /// GelieerdePersoon
        /// met gegeven ID
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van 
        /// GelieerdePersoon waarvan huisgenoten
        /// gevraagd zijn</param>
        /// <returns>Lijstje met personen</returns>
        /// <remarks>Parameter: GelieerdePersoonID, return value: Personen!</remarks>
        public IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
        {
            if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                // Haal alle huisgenoten op

                IList<Persoon> allen = _dao.HuisGenotenOphalen(gelieerdePersoonID);

                // Welke mag ik zien?

                IList<int> selectie = _autorisatieMgr.EnkelMijnPersonen(
                    (from p in allen select p.ID).ToList());

                // Enkel de geselecteerde personen afleveren.

                var resultaat = from p in allen
                                where selectie.Contains(p.ID)
                                select p;

                return resultaat.ToList();
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
            }
        }


        /// <summary>
        /// Verhuist een persoon van oudAdres naar nieuwAdres.  Persisteert niet.
        /// </summary>
        /// <param name="verhuizer">te verhuizen GelieerdePersoon</param>
        /// <param name="oudAdres">oud adres</param>
        /// <param name="nieuwAdres">nieuw adres</param>
        /// <remarks>Als de persoon niet gekoppeld is aan het oude adres,
        /// zal hij ook niet verhiuzen</remarks>
        public void Verhuizen(Persoon verhuizer, Adres oudAdres, Adres nieuwAdres)
        {
            if (_autorisatieMgr.IsGavPersoon(verhuizer.ID))
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
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
            }
        }

        /// <summary>
        /// Koppelt het gegeven Adres via een nieuw PersoonsAdresObject
        /// aan de gegeven GelieerdePersoon, als thuisadres.  Persisteert niet.
        /// </summary>
        /// <param name="p">GelieerdePersoon die er een adres bij krijgt</param>
        /// <param name="adres">Toe te voegen adres</param>
        /// <remarks>Overbodig geworden door overload waarbij adrestype ook meegegeven wordt</remarks>
        //public void AdresToevoegen(Persoon p, Adres adres)
        //{
        //    if (_autorisatieMgr.IsGavPersoon(p.ID))
        //    {
        //        PersoonsAdres pa = new PersoonsAdres { Adres = adres, Persoon = p, AdresType =  AdresTypeEnum.Thuis  };
        //        p.PersoonsAdres.Add(pa);
        //        adres.PersoonsAdres.Add(pa);
        //    }
        //    else
        //    {
        //        throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
        //    }
        //}


        /// <summary>
        /// Koppelt het gegeven Adres via een nieuw PersoonsAdresObject
        /// aan de gegeven GelieerdePersoon.  Persisteert niet.
        /// </summary>
        /// <param name="p">GelieerdePersoon die er een adres bij krijgt</param>
        /// <param name="adres">Toe te voegen adres</param>
        public void AdresToevoegen(Persoon p, Adres adres, AdresTypeEnum adrestype)
        {
            if (_autorisatieMgr.IsGavPersoon(p.ID))
            {
                PersoonsAdres pa = new PersoonsAdres { Adres = adres, Persoon = p, AdresType = adrestype };
                p.PersoonsAdres.Add(pa);
                adres.PersoonsAdres.Add(pa);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGelieerdePersoon);
            }
        }

        public IList<Persoon> LijstOphalen(List<int> personenIDs)
        {
            AutorisatieManager authMgr = Factory.Maak<AutorisatieManager>();

            return _dao.LijstOphalen(authMgr.EnkelMijnPersonen(personenIDs));
        }


    }
}
