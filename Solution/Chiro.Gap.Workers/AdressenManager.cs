// <copyright company="Chirojeugd-Vlaanderen vzw" file="AdressenManager.cs">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.Properties;

using Adres = Chiro.Gap.Orm.Adres;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. adressen bevat
    /// </summary>
    public class AdressenManager
    {
        private readonly IAdressenDao _adressenDao;
        private readonly IStratenDao _stratenDao;
        private readonly ISubgemeenteDao _subgemeenteDao;
        private readonly ILandenDao _landenDao;
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly IAdressenSync _sync;

        /// <summary>
        /// Creëert nieuwe adressenmanager
        /// </summary>
        /// <param name="adressenDao">
        /// Repository voor adressen
        /// </param>
        /// <param name="stratenDao">
        /// Repository voor straten
        /// </param>
        /// <param name="subgemeenteDao">
        /// Repository voor 'subgemeentes'
        /// </param>
        /// <param name="landenDao">
        /// Repository voor landen
        /// </param>
        /// <param name="autorisatieMgr">
        /// Worker die autorisatie regelt
        /// </param>
        /// <param name="sync">
        /// Zorgt voor synchronisate van adressen naar KipAdmin
        /// </param>
        public AdressenManager(
            IAdressenDao adressenDao,
            IStratenDao stratenDao,
            ISubgemeenteDao subgemeenteDao,
            ILandenDao landenDao,
            IAutorisatieManager autorisatieMgr,
            IAdressenSync sync)
        {
            _adressenDao = adressenDao;
            _stratenDao = stratenDao;
            _subgemeenteDao = subgemeenteDao;
            _landenDao = landenDao;
            _autorisatieMgr = autorisatieMgr;
            _sync = sync;
        }

        #region proxy naar data acces

        /// <summary>
        /// Haalt een adres op, samen met alle personen die hun voorkeursadres
        /// daar hebben.
        /// </summary>
        /// <param name="adresID">
        /// ID van het gevraagde adres
        /// </param>
        /// <returns>
        /// Het adres met de bewoners die daar hun voorkeursadres hebben
        /// </returns>
        /// <remarks>
        /// Voorlopig enkel voor supergav
        /// </remarks>
        public Adres OphalenMetVoorkeurBewoners(int adresID)
        {
            if (_autorisatieMgr.IsSuperGav())
            {
                return _adressenDao.Ophalen(
                    adresID, adr => adr.PersoonsAdres.First().GelieerdePersoon.First().Persoon);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Haalt het adres met ID <paramref name="adresID"/> op, inclusief de bewoners (gelieerde personen) uit de groep met ID
        /// <paramref name="groepID"/>
        /// </summary>
        /// <param name="adresID">
        /// ID van het op te halen adres
        /// </param>
        /// <param name="groepID">
        /// ID van de groep waaruit bewoners moeten worden gehaald
        /// </param>
        /// <returns>
        /// Het gevraagde adres met de relevante bewoners.
        /// </returns>
        public Adres AdresMetBewonersOphalen(int adresID, int groepID)
        {
            return AdresMetBewonersOphalen(adresID, groepID, false);
        }

        /// <summary>
        /// Haalt het adres met ID <paramref name="adresID"/> op, inclusief de bewoners (gelieerde personen) uit de groep met ID
        /// <paramref name="groepID"/>
        /// </summary>
        /// <param name="adresID">
        /// ID van het op te halen adres
        /// </param>
        /// <param name="groepID">
        /// ID van de groep waaruit bewoners moeten worden gehaald
        /// </param>
        /// <param name="alleGelieerdePersonen">
        /// Indien true, worden alle gelieerde personen van de bewoners mee opgehaald,
        /// ook diegene waar je geen GAV voor bent.
        /// </param>
        /// <returns>
        /// Het gevraagde adres met de relevante bewoners.
        /// </returns>
        /// <remarks>
        /// ALLE ANDERE ADRESSEN VAN DE GEKOPPELDE BEWONERS WORDEN OOK MEE OPGEHAALD
        /// </remarks>
        public Adres AdresMetBewonersOphalen(int adresID, int groepID, bool alleGelieerdePersonen)
        {
            if (_autorisatieMgr.IsGavGroep(groepID))
            {
                return _adressenDao.BewonersOphalen(adresID, new[] { groepID }, alleGelieerdePersonen);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Haalt het adres met ID <paramref name="adresID"/> op, inclusief de bewoners (gelieerde personen) uit de groepen met ID
        /// in <paramref name="groepIDs"/>
        /// </summary>
        /// <param name="adresID">
        /// ID van het op te halen adres
        /// </param>
        /// <param name="groepIDs">
        /// ID van de groepen waaruit bewoners moeten worden gehaald
        /// </param>
        /// <param name="alleGelieerdePersonen">
        /// Indien true, worden alle gelieerde personen van de bewoners 
        /// mee opgehaald, ook diegene waar je geen GAV voor bent.
        /// </param>
        /// <returns>
        /// Het gevraagde adres met de relevante bewoners.
        /// </returns>
        /// <remarks>
        /// ALLE ANDERE ADRESSEN VAN DE GEKOPPELDE BEWONERS WORDEN OOK MEE OPGEHAALD
        /// </remarks>
        public Adres AdresMetBewonersOphalen(int adresID, IEnumerable<int> groepIDs, bool alleGelieerdePersonen)
        {
            if (_autorisatieMgr.IsGavGroepen(groepIDs))
            {
                return _adressenDao.BewonersOphalen(adresID, groepIDs, alleGelieerdePersonen);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Persisteert adres in de database, samen met alle gekoppelde personen en gelieerde personen.
        /// </summary>
        /// <param name="adr">
        /// Te persisteren adres
        /// </param>
        /// <returns>
        /// Het adres met eventueel nieuw ID
        /// </returns>
        public Adres Bewaren(Adres adr)
        {
            Adres resultaat;
#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif

            // enkel voorkeursadressen van personen met ad-nummer naar Kipadmin
            // Dit kan voor heen-en-weer-effecten in Kipadmin zorgen, als 2 groepen
            // beurtelings hun voorkeuradres aanpassen.
            // (Gelukkig wordt er momenteel niet teruggesynct van kipadmin naar gap)

            // check op voorkeursadres is een beetje tricky:
            // als een persoonsadres een gelieerde persoon heeft, dan wil dat zeggen dat
            // het persoonsadres het voorkeursadres is van die gelieerde persoon.
            // De persoon van het persoonsadres moet altijd dezelfde zijn als de persoon van
            // de gelieerde persoon.
            // Bijwerking: als je een adres wijzigt, dat voor jouw groep niet standaard is,
            // maar voor een andere groep wel, dan gaat dat adres *toch* als standaardadres
            // naar Kipadmin.  Maar dat is op zich geen probleem.
            var teSyncen = (from pa in adr.PersoonsAdres
                            where pa.GelieerdePersoon.Count > 0 // voorkeursadres
                                  && (pa.Persoon.AdNummer != null || pa.Persoon.AdInAanvraag)
                            // met ad-nummer
                            select pa).ToArray();

            _sync.StandaardAdressenBewaren(teSyncen);

            resultaat = _adressenDao.Bewaren(adr);

#if KIPDORP
                tx.Complete();
            }
#endif
            return resultaat;
        }

        #endregion

        /// <summary>
        /// Maakt een nieuw adres op basis van de info in <paramref name="adresInfo"/>, en persisteert
        /// </summary>
        /// <param name="adresInfo">
        /// Gegevens voor het nieuwe adres
        /// </param>
        /// <returns>
        /// Het nieuw gemaakte adres
        /// </returns>
        private Adres Maken(AdresInfo adresInfo)
        {
            var problemen = new Dictionary<string, FoutBericht>();
            Adres adr;

            if (string.IsNullOrEmpty(adresInfo.LandNaam) ||
                string.Compare(adresInfo.LandNaam, Resources.Belgie, true) == 0)
            {
                // Belgisch adres.  Zoek en koppel straat en gemeente
                adr = new BelgischAdres();

                var s = _stratenDao.Ophalen(adresInfo.StraatNaamNaam, adresInfo.PostNr);
                if (s != null)
                {
                    // Straat gevonden: aan adres koppelen
                    ((BelgischAdres)adr).StraatNaam = s;
                    s.BelgischAdres.Add((BelgischAdres)adr);
                }
                else
                {
                    // Straat niet gevonden: foutbericht toevoegen
                    problemen.Add("StraatNaamNaam",
                                  new FoutBericht
                                      {
                                          FoutNummer = FoutNummer.StraatNietGevonden,
                                          Bericht = string.Format(
                                              Resources.StraatNietGevonden,
                                              adresInfo.StraatNaamNaam,
                                              adresInfo.PostNr)
                                      });
                }

                var sg = _subgemeenteDao.Ophalen(adresInfo.WoonPlaatsNaam, adresInfo.PostNr);
                if (sg != null)
                {
                    // Gemeente gevonden: aan adres koppelen
                    ((BelgischAdres)adr).WoonPlaats = sg;
                    sg.BelgischAdres.Add((BelgischAdres)adr);
                }
                else
                {
                    // Gemeente niet gevonden: foutbericht toevoegen
                    problemen.Add("WoonPlaatsNaam",
                                  new FoutBericht
                                      {
                                          FoutNummer = FoutNummer.WoonPlaatsNietGevonden,
                                          Bericht = Resources.GemeenteNietGevonden
                                      });
                }
            }
            else
            {
                // Buitenlands adres.  Straat en gemeente zijn gewone strings.
                // Zoek en koppel land.
                adr = new BuitenLandsAdres();

                ((BuitenLandsAdres)adr).Straat = adresInfo.StraatNaamNaam;
                ((BuitenLandsAdres)adr).WoonPlaats = adresInfo.WoonPlaatsNaam;
                ((BuitenLandsAdres)adr).PostCode = adresInfo.PostCode;
                ((BuitenLandsAdres)adr).PostNummer = adresInfo.PostNr;

                Land l = _landenDao.Ophalen(adresInfo.LandNaam);

                if (l != null)
                {
                    // Gemeente gevonden: aan adres koppelen
                    ((BuitenLandsAdres)adr).Land = l;
                    l.BuitenLandsAdres.Add((BuitenLandsAdres)adr);
                }
                else
                {
                    // Gemeente niet gevonden: foutbericht toevoegen
                    problemen.Add("LandNaam",
                                  new FoutBericht
                                      {
                                          FoutNummer = FoutNummer.LandNietGevonden,
                                          Bericht = Resources.LandNietGevonden
                                      });
                }
            }

            if (problemen.Count != 0)
            {
                throw new OngeldigObjectException(problemen);
            }

            adr.HuisNr = adresInfo.HuisNr;
            adr.Bus = adresInfo.Bus;

            adr = _adressenDao.Bewaren(adr);

            // bewaren brengt Versie en ID automatisch in orde.
            return adr;
        }

        /// <summary>
        /// Zoekt adres op, op basis van de parameters.
        /// Als er zo geen adres bestaat, wordt het aangemaakt, op
        /// voorwaarde dat de straat en subgemeente geidentificeerd
        /// kunnen worden.  Als ook dat laatste niet het geval is,
        /// wordt een exception gethrowd.
        /// </summary>
        /// <param name="adresInfo">
        /// Bevat de gegevens van het te zoeken/maken adres
        /// </param>
        /// <returns>
        /// Gevonden adres
        /// </returns>
        /// <remarks>
        /// Ieder heeft het recht adressen op te zoeken
        /// </remarks>
        public Adres ZoekenOfMaken(AdresInfo adresInfo)
        {
            var problemen = new Dictionary<string, FoutBericht>();

            // Al maar preventief een collectie fouten verzamelen.  Als daar uiteindelijk
            // geen foutberichten inzitten, dan is er geen probleem.  Anders
            // creëer ik een exception.
            if (adresInfo.StraatNaamNaam == string.Empty)
            {
                problemen.Add("StraatNaamNaam",
                              new FoutBericht
                                  {
                                      FoutNummer = FoutNummer.StraatOntbreekt,
                                      Bericht = string.Format(
                                          Resources.StraatOntbreekt,
                                          adresInfo.StraatNaamNaam,
                                          adresInfo.PostNr)
                                  });
            }

            // Controle formaat postnummer enkel voor belgische adressen.
            if ((string.IsNullOrEmpty(adresInfo.LandNaam) ||
                 string.Compare(adresInfo.LandNaam, Resources.Belgie, true) == 0) &&
                (adresInfo.PostNr < 1000 || adresInfo.PostNr > 9999))
            {
                problemen.Add("PostNr",
                              new FoutBericht
                                  {
                                      FoutNummer = FoutNummer.OngeldigPostNummer,
                                      Bericht = string.Format(
                                          Resources.OngeldigPostNummer,
                                          adresInfo.StraatNaamNaam,
                                          adresInfo.PostNr)
                                  });
            }

            if (adresInfo.WoonPlaatsNaam == string.Empty)
            {
                problemen.Add("WoonPlaatsNaam",
                              new FoutBericht
                                  {
                                      FoutNummer = FoutNummer.WoonPlaatsOntbreekt,
                                      Bericht = string.Format(
                                          Resources.WoonPlaatsOntbreekt,
                                          adresInfo.StraatNaamNaam,
                                          adresInfo.PostNr)
                                  });
            }

            // Als er hier al fouten zijn: gewoon throwen.  Me hiel 't stad, mor ni me maa!
            if (problemen.Count != 0)
            {
                throw new OngeldigObjectException(problemen);
            }

            var adresInDb = _adressenDao.Ophalen(adresInfo, false);

            if (adresInDb == null)
            {
                return Maken(adresInfo);
            }
            else
            {
                if (adresInDb is BelgischAdres)
                {
                    Debug.Assert(((BelgischAdres)adresInDb).StraatNaam != null);
                    Debug.Assert(((BelgischAdres)adresInDb).WoonPlaats != null);
                }

                return adresInDb;
            }
        }

        #region adressen ophalen

        /// <summary>
        /// Een lijst van subgemeenten ophalen
        /// </summary>
        /// <returns>
        /// Een lijst van subgemeenten
        /// </returns>
        public IList<WoonPlaats> GemeentesOphalen()
        {
            return _subgemeenteDao.AllesOphalen();
        }

        /// <summary>
        /// De lijst van beschikbare landen ophalen
        /// </summary>
        /// <returns>
        /// De lijst van beschikbare landen
        /// </returns>
        public IEnumerable<Land> LandenOphalen()
        {
            return _landenDao.AllesOphalen();
        }

        /// <summary>
        /// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
        /// met het gegeven <paramref name="straatBegin"/>.
        /// </summary>
        /// <param name="straatBegin">
        /// Eerste letters van de te zoeken straatnamen
        /// </param>
        /// <param name="postNr">
        /// Postnummer waarin te zoeken
        /// </param>
        /// <returns>
        /// Gegevens van de gevonden straten
        /// </returns>
        public IList<StraatNaam> StratenOphalen(string straatBegin, int postNr)
        {
            return StratenOphalen(straatBegin, new[] { postNr });
        }

        /// <summary>
        /// Haalt alle straten op uit een gegeven rij <paramref name="postNrs"/>, waarvan de naam begint
        /// met het gegeven <paramref name="straatBegin"/>.
        /// </summary>
        /// <param name="straatBegin">
        /// Eerste letters van de te zoeken straatnamen
        /// </param>
        /// <param name="postNrs">
        /// Postnummers waarin te zoeken
        /// </param>
        /// <returns>
        /// Gegevens van de gevonden straten
        /// </returns>
        public IList<StraatNaam> StratenOphalen(string straatBegin, IEnumerable<int> postNrs)
        {
            return _stratenDao.MogelijkhedenOphalen(straatBegin, postNrs);
        }

        #endregion
    }
}