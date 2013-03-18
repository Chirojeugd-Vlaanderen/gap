// <copyright company="Chirojeugd-Vlaanderen vzw" file="AdressenManager.cs">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

using Adres = Chiro.Gap.Poco.Model.Adres;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. adressen bevat
    /// </summary>
    public class AdressenManager : IAdressenManager
    {
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly IAdressenSync _sync;

        public AdressenManager(
            IAutorisatieManager autorisatieMgr,
            IAdressenSync sync)
        {
            _autorisatieMgr = autorisatieMgr;
            _sync = sync;
        }

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
            throw new NotImplementedException(NIEUWEBACKEND.Info);
            //var problemen = new Dictionary<string, FoutBericht>();
            //Adres adr;

            //if (string.IsNullOrEmpty(adresInfo.LandNaam) ||
            //    string.Compare(adresInfo.LandNaam, Resources.Belgie, true) == 0)
            //{
            //    // Belgisch adres.  Zoek en koppel straat en gemeente
            //    adr = new BelgischAdres();

            //    var s = _stratenDao.Ophalen(adresInfo.StraatNaamNaam, adresInfo.PostNr);
            //    if (s != null)
            //    {
            //        // Straat gevonden: aan adres koppelen
            //        ((BelgischAdres)adr).StraatNaam = s;
            //        s.BelgischAdres.Add((BelgischAdres)adr);
            //    }
            //    else
            //    {
            //        // Straat niet gevonden: foutbericht toevoegen
            //        problemen.Add("StraatNaamNaam",
            //                      new FoutBericht
            //                          {
            //                              FoutNummer = FoutNummer.StraatNietGevonden,
            //                              Bericht = string.Format(
            //                                  Resources.StraatNietGevonden,
            //                                  adresInfo.StraatNaamNaam,
            //                                  adresInfo.PostNr)
            //                          });
            //    }

            //    var sg = _subgemeenteDao.Ophalen(adresInfo.WoonPlaatsNaam, adresInfo.PostNr);
            //    if (sg != null)
            //    {
            //        // Gemeente gevonden: aan adres koppelen
            //        ((BelgischAdres)adr).WoonPlaats = sg;
            //        sg.BelgischAdres.Add((BelgischAdres)adr);
            //    }
            //    else
            //    {
            //        // Gemeente niet gevonden: foutbericht toevoegen
            //        problemen.Add("WoonPlaatsNaam",
            //                      new FoutBericht
            //                          {
            //                              FoutNummer = FoutNummer.WoonPlaatsNietGevonden,
            //                              Bericht = Resources.GemeenteNietGevonden
            //                          });
            //    }
            //}
            //else
            //{
            //    // Buitenlands adres.  Straat en gemeente zijn gewone strings.
            //    // Zoek en koppel land.
            //    adr = new BuitenLandsAdres();

            //    ((BuitenLandsAdres)adr).Straat = adresInfo.StraatNaamNaam;
            //    ((BuitenLandsAdres)adr).WoonPlaats = adresInfo.WoonPlaatsNaam;
            //    ((BuitenLandsAdres)adr).PostCode = adresInfo.PostCode;
            //    ((BuitenLandsAdres)adr).PostNummer = adresInfo.PostNr;

            //    Land l = _landenDao.Ophalen(adresInfo.LandNaam);

            //    if (l != null)
            //    {
            //        // Gemeente gevonden: aan adres koppelen
            //        ((BuitenLandsAdres)adr).Land = l;
            //        l.BuitenLandsAdres.Add((BuitenLandsAdres)adr);
            //    }
            //    else
            //    {
            //        // Gemeente niet gevonden: foutbericht toevoegen
            //        problemen.Add("LandNaam",
            //                      new FoutBericht
            //                          {
            //                              FoutNummer = FoutNummer.LandNietGevonden,
            //                              Bericht = Resources.LandNietGevonden
            //                          });
            //    }
            //}

            //if (problemen.Count != 0)
            //{
            //    throw new OngeldigObjectException(problemen);
            //}

            //adr.HuisNr = adresInfo.HuisNr;
            //adr.Bus = adresInfo.Bus;

            //adr = _adressenDao.Bewaren(adr);

            //// bewaren brengt Versie en ID automatisch in orde.
            //return adr;
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
        /// <param name="adressen">
        /// Lijst met bestaande adressen om na te kijken of het nieuwe adres al bestaat
        /// </param>
        /// <returns>
        /// Gevonden adres
        /// </returns>
        /// <remarks>
        /// Ieder heeft het recht adressen op te zoeken
        /// </remarks>
        public Adres ZoekenOfMaken(AdresInfo adresInfo, IQueryable<Adres> adressen)
        {
            var problemen = new Dictionary<string, FoutBericht>();

            // Al maar preventief een collectie fouten verzamelen.  Als daar uiteindelijk
            // geen foutberichten in zitten, dan is er geen probleem.  Anders
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

            // Controle formaat postnummer enkel voor Belgische adressen.
            if ((string.IsNullOrEmpty(adresInfo.LandNaam) ||
                 String.Compare(adresInfo.LandNaam, Resources.Belgie, StringComparison.OrdinalIgnoreCase) == 0) &&
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

            // Nagaan of het adres al bestaat
            var resultaat = (from adr in adressen.OfType<BelgischAdres>()
                               where adr.StraatNaam.Naam == adresInfo.StraatNaamNaam
                                     && adr.StraatNaam.PostNummer == adresInfo.PostNr
                                     && adr.HuisNr == adresInfo.HuisNr
                               select adr).FirstOrDefault() ?? ((from adr in adressen.OfType<BuitenLandsAdres>()
                                                                 where adr.Straat == adresInfo.StraatNaamNaam
                                                                       && adr.PostNummer == adresInfo.PostNr
                                                                       && adr.HuisNr == adresInfo.HuisNr
                                                                       && adr.Land.Naam == adresInfo.LandNaam
                                                                       && adr.PostCode == adresInfo.PostCode
                                                                 select adr).FirstOrDefault() ?? Maken(adresInfo));

            return resultaat;
        }
    }
}