using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;
using System.Diagnostics;
using Cg2.Orm.Exceptions;

namespace Cg2.Workers
{
    public class AdressenManager
    {
        private IAdressenDao _dao;
        private IStratenDao _stratenDao;
        private ISubgemeenteDao _subgemeenteDao;

        #region constructors

        public AdressenManager()
        {
            _dao = new AdressenDao();
            _stratenDao = new StratenDao();
            _subgemeenteDao = new SubgemeenteDao();
        }

        public AdressenManager(IAdressenDao dao, IStratenDao stratenDao, ISubgemeenteDao subgemeenteDao)
        {
            _dao = dao;
            _stratenDao = stratenDao;
            _subgemeenteDao = subgemeenteDao;
        }

        #endregion

        #region proxy naar data acces

        /// <summary>
        /// Haalt een adres op, samen met de gekoppelde personen
        /// </summary>
        /// <param name="adresID">ID op te halen adres</param>
        /// <param name="gelieerdAan">Als een lijst met groepID's gegeven,
        /// dan worden enkel personen gelieerd aan groepen met ID's uit
        /// de lijst meegeleverd.  Indien gelieerdAan null is, krijg
        /// je alle bewoners mee</param>
        /// <returns>Adresobject met gekoppelde personen</returns>
        public Adres AdresMetBewonersOphalen(int adresID, IList<int> gelieerdAan)
        {
            return _dao.BewonersOphalen(adresID, gelieerdAan);
        }

        #endregion

        /// <summary>
        /// Haalt ID en Versie van het adres uit de database.
        /// Indien dat nog niet gebeurd is, wordt het adres
        /// eerst aan de database toegevoegd.
        /// 
        /// Verwacht wordt dat het volgende is ingevuld:
        ///  - straat.naam
        ///  - huisnummer
        ///  - straat.postnr
        ///  - subgemeente.naam
        /// </summary>
        /// <param name="adr">Te syncen adres</param>
        /// <returns>Referentie naar het gesyncte adres</returns>
        public Adres Syncen(ref Adres adr)
        {
            Adres adresInDb;

            Debug.Assert(adr.Straat.Naam != String.Empty);
            Debug.Assert(adr.Straat.PostNr > 0);
            Debug.Assert(adr.Subgemeente.Naam != String.Empty);

            adr.ID = 0;

            adresInDb = _dao.Ophalen(adr.Straat.Naam, adr.HuisNr, adr.Bus, adr.Straat.PostNr, adr.PostCode, adr.Subgemeente.Naam);

            if (adresInDb == null)
            {
                // Adres niet gevonden.  Probeer straat en gemeente te vinden

                Straat s;
                Subgemeente sg;

                s = _stratenDao.Ophalen(adr.Straat.Naam, adr.Straat.PostNr);
                if (s == null)
                {
                    throw new StraatNietGevondenException(String.Format("Straat {0} met postcode {1} niet gevonden.", adr.Straat.Naam, adr.Subgemeente.PostNr));
                }

                adr.Straat = s;
                s.Adres.Add(adr);

                sg = _subgemeenteDao.Ophalen(adr.Subgemeente.Naam, adr.Straat.PostNr);
                if (sg == null)
                {
                    throw new GemeenteNietGevondenException(String.Format("Deelgemeente {0} met postcode {1] niet gevonden.", adr.Subgemeente.Naam, adr.Subgemeente.PostNr));
                }

                adr.Subgemeente = sg;
                sg.Adres.Add(adr);

                adr = _dao.Bewaren(adr);
                // bewaren brengt Versie en ID automatisch in orde.
            }
            else
            {
                Debug.Assert(adresInDb.Straat != null);
                Debug.Assert(adresInDb.Subgemeente != null);

                adr = adresInDb;
            }

            return adr;
        }
    }
}
