// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Businesslogica wat betreft uitstappen en bivakken
    /// </summary>
    public class UitstappenManager : IUitstappenManager
    {
        private readonly IAutorisatieManager _autorisatieManager;
        private readonly IBivakSync _sync;

        public UitstappenManager(
            IAutorisatieManager autorisatieManager,
            IBivakSync sync)
        {
            _autorisatieManager = autorisatieManager;
            _sync = sync;
        }

        /// <summary>
        /// Koppelt een uitstap aan een groepswerkjaar.  Dit moet typisch
        /// enkel gebeuren bij een nieuwe uitstap.
        /// </summary>
        /// <param name="uitstap">
        /// Te koppelen uitstap
        /// </param>
        /// <param name="gwj">
        /// Te koppelen groepswerkjaar
        /// </param>
        /// <returns>
        /// <paramref name="uitstap"/>, gekoppeld aan <paramref name="gwj"/>.
        /// </returns>
        public Uitstap Koppelen(Uitstap uitstap, GroepsWerkJaar gwj)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Koppelt een plaats aan een uitstap
        /// </summary>
        /// <param name="uitstap">
        /// Te koppelen uitstap
        /// </param>
        /// <param name="plaats">
        /// Te koppelen plaats
        /// </param>
        /// <returns>
        /// Uitstap, met plaats gekoppeld.  Persisteert niet
        /// </returns>
        public Uitstap Koppelen(Uitstap uitstap, Plaats plaats)
        {
            if (!_autorisatieManager.IsGavUitstap(uitstap.ID) || !_autorisatieManager.IsGavPlaats(plaats.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            plaats.Uitstap.Add(uitstap);
            uitstap.Plaats = plaats;

            return uitstap;
        }

        /// <summary>
        /// Stuurt alle bivakken van werkJaar <paramref name="werkJaar"/> opnieuw naar
        /// kipadmin.
        /// </summary>
        /// <param name="werkJaar">
        /// Het werkJaar waarvan de gegevens opnieuw gesynct moeten worden
        /// </param>
        public void OpnieuwSyncen(int werkJaar)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Gaat na welke gegevens er nog ontbreken in de geregistreerde bivakken om van een
        /// geldige bivakaangifte te kunnen spreken.
        /// </summary>
        /// <param name="groepID">
        /// De ID van de groep waar het over gaat.
        /// </param>
        /// <param name="groepsWerkJaar">
        /// Het werkJaar waarvoor de gegevens opgehaald moeten worden.
        /// </param>
        /// <returns>
        /// Een lijstje met opmerkingen/feedback voor de gebruiker, zodat die weet 
        /// of er nog iets extra's ingevuld moet worden.
        /// </returns>
        /// <exception cref="GeenGavException">
        /// Komt voor als de gebruiker op dit moment geen GAV is voor de groep met de opgegeven <paramref name="groepID"/>,
        /// maar ook als de gebruiker geen GAV was/is in het opgegeven <paramref name="groepsWerkJaar"/>.
        /// </exception>
        public BivakAangifteLijstInfo BivakStatusOphalen(int groepID, GroepsWerkJaar groepsWerkJaar)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
            //if (!_autorisatieManager.IsGavGroepsWerkJaar(groepsWerkJaar.ID))
            //{
            //    throw new GeenGavException(Resources.GeenGav);
            //}

            //if (!_autorisatieManager.IsGavGroep(groepID))
            //{
            //    throw new GeenGavException(Resources.GeenGav);
            //}

            //var statuslijst = new BivakAangifteLijstInfo {Bivakinfos = new List<BivakAangifteInfo>()};

            //var aangiftesetting = Settings.Default.BivakAangifteStart;
            //var aangiftestart = new DateTime(groepsWerkJaar.WerkJaar + 1, aangiftesetting.Month, aangiftesetting.Day);
            //if (0 <= aangiftestart.CompareTo(DateTime.Today))
            //{
            //    statuslijst.AlgemeneStatus = BivakAangifteStatus.NogNietVanBelang;
            //}
            //else
            //{
            //    var bivaklijst = OphalenVanGroep(groepID, false).Where(e => e.IsBivak).ToList();
            //    bool allesingevuld = true;
            //    foreach (var uitstap in bivaklijst)
            //    {
            //        var aangifteInfo = new BivakAangifteInfo
            //                               {
            //                                   ID = uitstap.ID,
            //                                   Omschrijving = uitstap.Naam
            //                               };

            //        var uitstapmetdetails = Ophalen(uitstap.ID,
            //                                        UitstapExtras.Contact | UitstapExtras.Plaats |
            //                                        UitstapExtras.GroepsWerkJaar);
            //        if (uitstapmetdetails.GroepsWerkJaar.ID != groepsWerkJaar.ID)
            //        {
            //            continue;
            //        }

            //        if (uitstapmetdetails.Plaats == null && uitstapmetdetails.ContactDeelnemer == null)
            //        {
            //            aangifteInfo.Status = BivakAangifteStatus.PlaatsEnPersoonInTeVullen;
            //            allesingevuld = false;
            //        }
            //        else if (uitstapmetdetails.Plaats == null)
            //        {
            //            aangifteInfo.Status = BivakAangifteStatus.PlaatsInTeVullen;
            //            allesingevuld = false;
            //        }
            //        else if (uitstapmetdetails.ContactDeelnemer == null)
            //        {
            //            aangifteInfo.Status = BivakAangifteStatus.PersoonInTeVullen;
            //            allesingevuld = false;
            //        }
            //        else
            //        {
            //            aangifteInfo.Status = BivakAangifteStatus.Ingevuld;
            //        }

            //        statuslijst.Bivakinfos.Add(aangifteInfo);
            //    }

            //    if (statuslijst.Bivakinfos.Count == 0)
            //    {
            //        statuslijst.AlgemeneStatus = BivakAangifteStatus.DringendInTeVullen;
            //    }
            //    else
            //    {
            //        statuslijst.AlgemeneStatus = allesingevuld
            //                                         ? BivakAangifteStatus.Ingevuld
            //                                         : BivakAangifteStatus.DringendInTeVullen;
            //    }
            //}

            //return statuslijst;
        }

        /// <summary>
        /// Bepaalt of het de tijd van het jaar is voor de bivakaangifte
        /// </summary>
        /// <param name="groepsWerkJaar">huidige groepswerkjaar</param>
        /// <returns><c>true</c> als de bivakaangifte voor <paramref name="groepsWerkJaar"/> moet worden doorgegeven, 
        /// anders <c>false</c></returns>
        public bool BivakAangifteVanBelang(GroepsWerkJaar groepsWerkJaar)
        {
            DateTime startAangifte = Settings.Default.BivakAangifteStart;
            DateTime startAangifteDitWerkjaar = new DateTime(groepsWerkJaar.WerkJaar + 1, startAangifte.Month, startAangifte.Day);

            return (DateTime.Today >= startAangifteDitWerkjaar);
        }

        /// <summary>
        /// Bepaalt de status van de gegeven <paramref name="uitstap"/>
        /// </summary>
        /// <param name="uitstap">Uitstap, waarvan status bepaald moet worden</param>
        /// <returns>De status van de gegeven <paramref name="uitstap"/></returns>
        public BivakAangifteStatus StatusBepalen(Uitstap uitstap)
        {
            var resultaat = BivakAangifteStatus.Ok;

            if (uitstap.Plaats == null)
            {
                resultaat |= BivakAangifteStatus.PlaatsOntbreekt;
            }
            if (uitstap.ContactDeelnemer == null)
            {
                resultaat |= BivakAangifteStatus.ContactOntbreekt;
            }

            return resultaat;
        }

        /// <summary>
        /// Nagaan of alle vereisten voldaan zijn om de opgegeven gelieerde personen allemaal in te schrijven
        /// voor de opgegeven uitstap.
        /// </summary>
        /// <param name="uitstap">De uitstap waar we mensen voor willen inschrijven</param>
        /// <param name="gelieerdePersonen">De mensen die we willen inschrijven</param>
        /// <exception cref="FoutNummerException"></exception>
        /// <returns><c>True</c> als alle voorwaarden voldaan zijn, anders <c>false</c></returns>
        public bool InschrijvingenValideren(Uitstap uitstap, List<GelieerdePersoon> gelieerdePersonen)
        {
            // De gelieerde personen moeten aan een groep gekoppeld zijn.
            var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct().ToList();

            if (!groepen.Any())
            {
                return false;
            }

            if (uitstap.GroepsWerkJaar == null)
            {
                return false;
            }

            if (uitstap.GroepsWerkJaar.Groep == null)
            {
                return false;
            }

            // Als er meer dan 1 groep is, dan is er minstens een groep verschillend van de groep
            // van de uitstap (duivenkotenprincipe));););
            if (groepen.Count() > 1 || groepen.First().ID != uitstap.GroepsWerkJaar.Groep.ID)
            {
                throw new FoutNummerException(
                    FoutNummer.UitstapNietVanGroep,
                    Resources.FoutieveGroepUitstap);
            }

            return true;
        }
    }
}