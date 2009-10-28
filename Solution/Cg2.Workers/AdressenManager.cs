using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using System.Diagnostics;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Fouten.FaultContracts;
using System.ServiceModel;

namespace Cg2.Workers
{
    public class AdressenManager
    {
        private IAdressenDao _dao;
        private IStratenDao _stratenDao;
        private ISubgemeenteDao _subgemeenteDao;
        private IAutorisatieManager _autorisatieMgr;

        /// <summary>
        /// Creeert nieuwe adressenmanager
        /// </summary>
        /// <param name="dao">DAO voor adressen</param>
        /// <param name="stratenDao">DAO voor straten</param>
        /// <param name="subgemeenteDao">DAO voor 'subgemeentes'</param>
        /// <param name="autorisatieMgr">Autorisatiemanager</param>
        public AdressenManager(IAdressenDao dao, IStratenDao stratenDao, ISubgemeenteDao subgemeenteDao, IAutorisatieManager autorisatieMgr)
        {
            _dao = dao;
            _stratenDao = stratenDao;
            _subgemeenteDao = subgemeenteDao;
            _autorisatieMgr = autorisatieMgr;
        }

        #region proxy naar data acces

        /// <summary>
        /// Haalt een adres op, inclusief bewoners waar de ingelogde
        /// user gebruikersrechten op heeft.
        /// </summary>
        /// <param name="adresID">ID van het gevraagde adres</param>
        /// <returns>adres met daaraan gekoppeld de bewoners</returns>
        public Adres AdresMetBewonersOphalen(int adresID)
        {
            // Adressen zitten vast in de database, en daar is niets
            // interessants over te zeggen.  Voorlopig mag iedereen elk
            // adres opzoeken.  Voor de bewonersgegevens worden de
            // rechten uiteraard wel gecontroleerd.

            return _dao.BewonersOphalen(adresID, _autorisatieMgr.GebruikersNaamGet());
        }

        /// <summary>
        /// Persisteert adres in de database
        /// </summary>
        /// <param name="adr">Te persisteren adres</param>
        /// <returns>Het adres met eventueel nieuw ID</returns>
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
        /// <remarks>Ieder heeft het recht adressen op te zoeken</remarks>
        public Adres ZoekenOfMaken(Adres adr)
        {
            AdresFault vf = new AdresFault();

            // Al maar preventief een VerhuisFault aanmaken.  Als daar uiteindelijk
            // geen foutberichten inzitten, dan is er geen probleem.  Anders
            // creer ik een exception met de verhuisfault daarin.

            Adres adresInDb;

            Debug.Assert(adr.Straat.Naam != String.Empty);
            Debug.Assert(adr.Straat.PostNr > 0);
            Debug.Assert(adr.Subgemeente.Naam != String.Empty);

            adr.ID = 0;

            adresInDb = _dao.Ophalen(adr.Straat.Naam, adr.HuisNr, adr.Bus, adr.Straat.PostNr, adr.PostCode, adr.Subgemeente.Naam, false);

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

                    vf.BerichtToevoegen(AdresFaultCode.OnbekendeStraat, "Straat.Naam"
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

                    vf.BerichtToevoegen(AdresFaultCode.OnbekendeGemeente, "SubGemeente.Naam"
                        , String.Format("Deelgemeente {0} met postnummer {1} niet gevonden."
                        , adr.Subgemeente.Naam, adr.Straat.PostNr));
                }

                if (vf.Berichten.Count != 0)
                {
                    throw new AdresException(vf);
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
