﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
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
using Chiro.Gap.Validatie;
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
            if (!_autorisatieManager.IsGav(uitstap) || !_autorisatieManager.IsGav(plaats))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            plaats.Uitstap.Add(uitstap);
            uitstap.Plaats = plaats;

            return uitstap;
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