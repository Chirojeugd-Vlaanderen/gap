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
using System.Linq;
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
        private readonly IMailer _mailer;

        public GebruikersRechtenManager(
            IMailer mailer)
        {
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
				   && (gr.VervalDatum == null || gr.VervalDatum > DateTime.Now)
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
