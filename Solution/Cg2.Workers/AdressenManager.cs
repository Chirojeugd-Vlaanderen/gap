using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;
using System.Diagnostics;
using Cg2.Fouten.Exceptions;
using Cg2.Fouten.FaultContracts;

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

        public Adres Bewaren(Adres adr)
        {
            return _dao.Bewaren(adr);
        }

        #endregion

        /// <summary>
        /// Zoekt adres (incl. straat en subgemeente), op basis
        /// van
        ///  - straat.naam
        ///  - huisnummer
        ///  - straat.postnr
        ///  - subgemeente.naam
        /// 
        /// Als er zo geen adres bestaat, wordt het aangemaakt, op
        /// voorwaarde dat de straat en subgemeente geidentificeerd
        /// kunnen worden.  Als ook dat laatste niet het geval is,
        /// wordt een exception gethrowd.
        /// </summary>
        /// <param name="adr">Adresobject met zoekinfo</param>
        /// <returns>Gevonden adres</returns>
        public Adres ZoekenOfMaken(Adres adr)
        {
            VerhuisFault vf = new VerhuisFault();

            // Al maar preventief een VerhuisFault aanmaken.  Als daar uiteindelijk
            // geen foutberichten inzitten, dan is er geen probleem.  Anders
            // creer ik een exception met de verhuisfault daarin.

            Adres adresInDb;

            Debug.Assert(adr.Straat.Naam != String.Empty);
            Debug.Assert(adr.Straat.PostNr > 0);
            Debug.Assert(adr.Subgemeente.Naam != String.Empty);

            adr.ID = 0;

            adresInDb = _dao.Ophalen(adr.Straat.Naam, adr.HuisNr, adr.Bus, adr.Straat.PostNr, adr.PostCode, adr.Subgemeente.Naam, true);

            if (adresInDb == null)
            {
                // Adres niet gevonden.  Probeer straat en gemeente te vinden

                Straat s;
                Subgemeente sg;

                s = _stratenDao.Ophalen(adr.Straat.Naam, adr.Straat.PostNr);
                if (s != null)
                {
                    // Straat gevonden: aan adres koppelen

                    adr.Straat = s;
                    s.Adres.Add(adr);
                }
                else
                {
                    // Straat niet gevonden: foutbericht toevoegen

                    vf.BerichtToevoegen(VerhuisFoutCode.OnbekendeStraat, "Straat.Naam"
                        , String.Format("Straat {0} met postnummer {1} niet gevonden."
                        , adr.Straat.Naam, adr.Straat.PostNr));
                }

                sg = _subgemeenteDao.Ophalen(adr.Subgemeente.Naam, adr.Straat.PostNr);
                if (sg != null)
                {
                    // Gemeente gevonden: aan adres koppelen

                    adr.Subgemeente = sg;
                    sg.Adres.Add(adr);
                }
                else
                {
                    // Gemeente niet gevonden: foutbericht toevoegen

                    vf.BerichtToevoegen(VerhuisFoutCode.OnbekendeGemeente, "SubGemeente.Naam"
                        , String.Format("Deelgemeente {0} met postnummer {1} niet gevonden."
                        , adr.Subgemeente.Naam, adr.Straat.PostNr));
                }

                if (vf.Berichten.Count != 0)
                {
                    throw new VerhuisException(vf);
                }

                adr = _dao.Bewaren(adr);
                // bewaren brengt Versie en ID automatisch in orde.

                return adr;
            }
            else
            {
                Debug.Assert(adresInDb.Straat != null);
                Debug.Assert(adresInDb.Subgemeente != null);

                return adresInDb;
            }
        }
    }
}
