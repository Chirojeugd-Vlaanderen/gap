/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;  // NIET VERWIJDEREN! Nodig voor live (als 'KIPDORP' gedefined is)

using Chiro.Ad.ServiceContracts;
using Chiro.Adf.ServiceModel;
using Chiro.Cdf.Mailer;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Manager die informatie ophaalt over gebruikersrechten van personen waar jij 
    /// gebruikersrecht op hebt.
    /// </summary>
    public class GebruikersRechtenManager : IGebruikersRechtenManager
    {
        private readonly IAutorisatieManager _autorisatieManager;
        private readonly IMailer _mailer;

        public GebruikersRechtenManager(
            IAutorisatieManager autorisatieManager,
            IMailer mailer)
        {
            _autorisatieManager = autorisatieManager;
            _mailer = mailer;
        }

        /// <summary>
        /// Verlengt het gegeven <paramref name="gebruikersRecht"/> (indien mogelijk)
        /// </summary>
        /// <param name="gebruikersRecht">
        /// Te verlengen gebruikersrecht
        /// </param>
        public void Verlengen(GebruikersRecht gebruikersRecht)
        {
            if (!_autorisatieManager.IsGav(gebruikersRecht))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            if (!gebruikersRecht.IsVerlengbaar)
            {
                // Als er gebruikersrecht is, maar dat is niet verlengbaar, dan gooien
                // we er een exception tegenaan.
                throw new FoutNummerException(FoutNummer.GebruikersRechtNietVerlengbaar,
                                              Resources.GebruikersRechtNietVerlengbaar);
            }

            gebruikersRecht.VervalDatum = NieuweVervalDatum();
        }

        /// <summary>
        /// Bepaalt een nieuwe vervaldatum voor nieuwe of te verlengen gebruikersrechten.
        /// </summary>
        /// <returns>De standaardvervaldatum voor gebruikersrechten die vandaag worden gemaakt of verlengd.</returns>
        private DateTime NieuweVervalDatum()
        {
            // Vervaldatum aanpassen. Als de toegang in de zomer verlengd wordt (vanaf overgangsperiode), 
            // gaat het waarschijnlijk al over rechten voor het komend werkjaar.

            DateTime beginOvergang = new DateTime(
                DateTime.Now.Year,
                Settings.Default.BeginOvergangsPeriode.Month,
                Settings.Default.BeginOvergangsPeriode.Day);

            int jaar = DateTime.Now >= beginOvergang ? DateTime.Now.Year + 1 : DateTime.Now.Year;

            return new DateTime(
                jaar,
                Settings.Default.EindeGebruikersRecht.Month,
                Settings.Default.EindeGebruikersRecht.Day);
        }

        /// <summary>
        /// Maakt een account voor de gegeven <paramref name="gelieerdePersoon"/>. In principe is het niet nodig
        /// dat de persoon gelieerd is aan een groep, maar we hebben wel een e-mailadres nodig, en laat die nu
        /// net gekoppeld zijn aan gelieerde personen.
        /// Deze method persisteert, want het aanmaken en bewaren van de account in Gap en het aanmaken van de 
        /// account in AD moeten in 1 transactie gebeuren.
        /// </summary>
        /// <param name="gelieerdePersoon">Gelieerde persoon waarvoor een account gezocht of gemaakt moet 
        /// worden. Er wordt verondersteld dat die al gekoppeld is aan zijn accounts. Wat mogelijk een
        /// beetje stom is. Soit.</param>
        /// <returns>GAV-object voor de gelieerde persoon</returns>
        /// <remarks>Gav is eigenlijk een verkeerde naam; het was beter Login geweest.</remarks>
        public Gav AccountZoekenOfMaken(GelieerdePersoon gelieerdePersoon)
        {
            return AccountZoekenOfMaken(gelieerdePersoon, true);
        }

        /// <summary>
        /// Zoekt een account voor de gegeven <paramref name="gelieerdePersoon"/>. Als geen account wordt
        /// gevonden, en <paramref name="makenAlsNietGevonden"/> <c>true</c> is, wordt een nieuwe account 
        /// aangemaakt, en krijgt de gelieerde persoon een e-mail via zijn voorkeursmailadres.
        /// </summary>
        /// <param name="gelieerdePersoon">Gelieerde persoon waarvoor een account gezocht of gemaakt moet 
        /// worden. Er wordt verondersteld dat die al gekoppeld is aan zijn accounts. Wat mogelijk een
        /// beetje stom is. Soit.</param>
        /// <param name="makenAlsNietGevonden"> Indien <c>true</c> wordt een nieuwe account gemaakt als
        /// er geen is gevonden.</param>
        /// <returns>Account voor de gelieerde persoon (klasse Gav zou beter hernoemd worden als account, 
        /// zie #1357)</returns>
        /// <remarks>De gemaakte account heeft geen rechten.</remarks>
        public Gav AccountZoekenOfMaken(GelieerdePersoon gelieerdePersoon, bool makenAlsNietGevonden)
        {
            throw new NotImplementedException(Nieuwebackend.Info);
//            var p = gelieerdePersoon.Persoon;

//            if (!_autorisatieManager.IsGavGelieerdePersoon(gelieerdePersoon.ID))
//            {
//                throw new GeenGavException(Properties.Resources.GeenGav);
//            }

//            var account = p.Gav.FirstOrDefault();

//            if (account != null || !makenAlsNietGevonden)
//            {
//                // Als we al een account gevonden hebben, of als we er geen nieuwe moeten maken, dan zijn we
//                // hier al klaar.
//                return account;
//            }


//            // Check op ad-nummer en e-mailadres

//            if (p.AdNummer == null)
//            {
//                throw new FoutNummerException(FoutNummer.AdNummerVerplicht, Properties.Resources.AdNummerVerplicht);
//            }

//            if (string.IsNullOrEmpty(gelieerdePersoon.ContactEmail))
//            {
//                throw new FoutNummerException(FoutNummer.EMailVerplicht, Properties.Resources.EMailVerplicht);
//            }

//            account = new Gav();
//            account.Persoon.Add(p);
//            p.Gav.Add(account);

//#if KIPDORP
//            using (var tx = new TransactionScope())
//            {
//#endif
//                string username =
//                    ServiceHelper.CallService<IAdService, string>(
//                        svc => svc.GapLoginAanvragen(p.AdNummer.Value, p.VoorNaam, p.Naam, gelieerdePersoon.ContactEmail));

//                account.Login = string.Format(@"CHIROPUBLIC\{0}", username);

//                // Het zou kunnen dat de nieuwe login toch al ergens gekend was. Om key violations te vermijden, halen
//                // we lelijke-hackgewijze het account-ID op.

//                account.ID = _gavDao.IdOphalen(account.Login);

//                account = _gavDao.Bewaren(account, act => act.Persoon);

//#if KIPDORP
//                tx.Complete();
//            }
//#endif
//            return account;

        }

        /// <summary>
        /// Pas de vervaldatum van het gegeven <paramref name="gebruikersRecht"/> aan, zodanig dat
        /// het niet meer geldig is.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRecht">
        /// Te vervallen gebruikersrecht
        /// </param>
        public void Intrekken(GebruikersRecht gebruikersRecht)
        {
            Intrekken(new[] { gebruikersRecht });
        }

        /// <summary>
        /// Pas de vervaldatum van het de <paramref name="gebruikersRechten"/> aan, zodanig dat
        /// ze niet meer geldig zijn.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRechten">
        /// Te vervallen gebruikersrechten
        /// </param>
        /// <remarks>Gebruikersrechten die al vervallen zijn, blijven onaangeroerd</remarks>
        public void Intrekken(GebruikersRecht[] gebruikersRechten)
        {
            if (gebruikersRechten.Any(e => !_autorisatieManager.IsGav(e)))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            foreach (var gr in gebruikersRechten.Where(r => r.VervalDatum > DateTime.Now))
            {
                gr.VervalDatum = DateTime.Now.AddDays(-1);
            }
        }
        
        /// <summary>
        /// Koppelt de <paramref name="gav"/> aan de <paramref name="groep"/> met gegeven 
        /// <paramref name="vervalDatum"/>.  PERSISTEERT NIET.
        /// </summary>
        /// <param name="gav">
        /// Te koppelen GAV
        /// </param>
        /// <param name="groep">
        /// Groep waaraan te koppelen
        /// </param>
        /// <param name="vervalDatum">
        /// Vervaldatum gebruikersrecht
        /// </param>
        /// <returns>
        /// Deze method is PRIVATE en moet dat ook blijven, want er wordt niet gecheckt
        /// op fouten, en er worden geen notificatiemails gestuurd.  Deze method mag enkel
        /// onrechtstreeks gebruikt worden, via de publieke methods <see name="ToekennenOfVerlengen"/>
        /// </returns>
        private GebruikersRecht ToekennenOfVerlengen(Gav gav, Groep groep, DateTime vervalDatum)
        {
            // Eerst controleren of de groep nog niet aan de gebruiker is gekoppeld
            var gebruikersrecht = (from gr in gav.GebruikersRecht
                                   where gr.Groep.ID == groep.ID
                                   select gr).FirstOrDefault();

            if (gebruikersrecht == null)
            {
                // Nog geen gebruikersrecht.  Maak aan.
                gebruikersrecht = new GebruikersRecht { ID = 0, Gav = gav, Groep = groep };
                gav.GebruikersRecht.Add(gebruikersrecht);
                groep.GebruikersRecht.Add(gebruikersrecht);
            }
            else if (!gebruikersrecht.IsVerlengbaar)
            {
                throw new FoutNummerException(FoutNummer.GebruikersRechtNietVerlengbaar,
                                              Resources.GebruikersRechtNietVerlengbaar);
            }

            gebruikersrecht.VervalDatum = vervalDatum;

            return gebruikersrecht;
        }


        /// <summary>
        /// Kent gebruikersrechten toe voor gegeven <paramref name="groep"/> aan gegeven <paramref name="account"/>.
        /// Als de gebruikersrechten al bestonden, worden ze indien mogelijk verlengd.
        /// </summary>
        /// <param name="account">Account die gebruikersrecht moet krijgen op <paramref name="groep"/></param>
        /// <param name="groep">Groep waarvoor <paramref name="account"/> gebruikersrecht moet krijgen</param>
        /// <returns>Het gebruikersrecht</returns>
        /// <remarks>Persisteert niet.</remarks>
        public GebruikersRecht ToekennenOfVerlengen(Gav account, Groep groep)
        {
            // Omdat een account niet per se aan een gelieerde persoon gekoppeld is, controleren we enkel of
            // we gebruikersrechten hebben op de groep. Een koppeling account-groep hebben we vooralsnog niet.

            if (!_autorisatieManager.IsGav(groep))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            DateTime vervaldatum = NieuweVervalDatum();

            return ToekennenOfVerlengen(account, groep, vervaldatum);
        }

        /// <summary>
        /// Levert het gebruikersrecht op dat een <paramref name="gelieerdePersoon"/> heeft op zijn eigen groep. 
        /// If any.  Als <paramref name="gelieerdePersoon"/> geen gebruikersrechten heeft op zijn groep, wordt 
        /// <c>null</c> opgeleverd.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// Het gebruikersrecht op dat een <paramref name="gelieerdePersoon"/> heeft op zijn eigen groep. If any. 
        /// Als <paramref name="gelieerdePersoon"/> geen gebruikersrechten heeft op zijn groep, wordt <c>null</c> 
        /// opgeleverd.
        /// </returns>
        public GebruikersRecht GebruikersRechtGet(GelieerdePersoon gelieerdePersoon)
        {
            return
                gelieerdePersoon.Groep.GebruikersRecht.FirstOrDefault(
                    gr => gr.Gav.Persoon.Any(p => p.ID == gelieerdePersoon.Persoon.ID));
        }
    }
}