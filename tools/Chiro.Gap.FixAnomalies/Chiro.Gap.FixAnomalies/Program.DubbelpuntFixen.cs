/*
   Copyright 2016 Chirojeugd-Vlaanderen vzw

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Chiro.Cdf.Poco;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities.Custom;
using Chiro.CiviCrm.Api.DataContracts.Filters;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Gap.FixAnomalies.Properties;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Sync;

namespace Chiro.Gap.FixAnomalies
{
    partial class Program
    {
        private static void DubbelpuntFixen(ServiceHelper serviceHelper, string apiKey, string siteKey)
        {
            Console.WriteLine(Resources.Program_DubbelpuntFixen_Actieve_Dubbelpuntabonnementen_ophalen_uit_CiviCRM_);

            int werkjaar = HuidigWerkJaar();

            var request = new MembershipRequest
            {
                MembershipTypeId = 2,
                StatusFilter =
                    new Filter<MembershipStatus>(WhereOperator.In,
                        new[] {MembershipStatus.New, MembershipStatus.Current}),
                ApiOptions = new ApiOptions { Limit = 0 },
                ReturnFields = "external_identifier,custom_76"
            };

            var civiResult = serviceHelper.CallService<ICiviCrmApi, ApiResultValues<MembershipMetAd>>(
                    svc => svc.ChiroDiagnosticsMembersMetAd(apiKey, siteKey, request));
            Console.WriteLine(Resources.Program_Main_Dat_zijn_er__0__, civiResult.Count);

            // Bij leden sorteerden we de AD-nummers als string. Om het moeilijk te maken,
            // sorteren we ze bij DP als integer.
            var civiDps =
				(from r in civiResult.Values.OrderBy(v => ExtIdNaarAdNr(v.ExternalIdentifier))
                    select r).ToList();

            Console.WriteLine(Resources.Program_DubbelpuntFixen_Dubbelpuntabonnementen_ophalen_werkjaar__0____, werkjaar);
            var gapDps = AlleRelevanteAbonnementen(werkjaar);

            var overTeZetten = OntbrekendInCiviZoeken(civiDps, gapDps);
            Console.WriteLine(Resources.Program_DubbelpuntFixen__0__abonnementen_niet_gevonden_in_CiviCRM_, overTeZetten.Count);

            // TODO: command line switch om deze vraag te vermijden.
            Console.Write(Resources.Program_Main_Meteen_syncen__);
            string input = Console.ReadLine();

            if (input.ToUpper() == "J" || input.ToUpper() == "Y")
            {
                NaarCivi(overTeZetten, serviceHelper);
            }

        }

		private static int ExtIdNaarAdNr(string externalIdentifier)
		{
			int result = 0;
			int.TryParse(externalIdentifier, out result);
			return result;
		}

        private static void NaarCivi(List<ActiefAbonnement> overTeZetten, ServiceHelper serviceHelper)
        {
            int counter = 0;
            var sync = new AbonnementenSync(serviceHelper);
            using (var context = new ChiroGroepEntities())
            {
                var repositoryProvider = new RepositoryProvider(context);
                var repo = repositoryProvider.RepositoryGet<Abonnement>();
                foreach (var b in overTeZetten)
                {
                    sync.Bewaren(repo.ByID(b.ID));
                    Console.Write("{0} ", ++counter);
                }
            }
        }

        private static List<ActiefAbonnement> OntbrekendInCiviZoeken(List<MembershipMetAd> civiDps, ActiefAbonnement[] gapDps)
        {
            // Het blijft erg hacky. Hier wordt eigenlijk geen rekening gehouden met het abonnementtype.
            int civiCounter = 0;
            int gapCounter = 0;
            var teSyncen = new List<ActiefAbonnement>();
            int aantalciviBivakken = civiDps.Count;

            Console.WriteLine(Resources.Program_OntbrekendInCiviZoeken_Op_zoek_naar_abonnementen_in_GAP_maar_niet_in_CiviCRM___);
            while (gapCounter < gapDps.Length && civiCounter < aantalciviBivakken)
            {
				while (civiCounter < aantalciviBivakken && gapDps[gapCounter].AdNummer > ExtIdNaarAdNr(civiDps[civiCounter].ExternalIdentifier))
                {
                    ++civiCounter;
                }
				if (civiCounter < aantalciviBivakken && gapDps[gapCounter].AdNummer != ExtIdNaarAdNr(civiDps[civiCounter].ExternalIdentifier))
                {
                    teSyncen.Add(gapDps[gapCounter]);
                    Console.WriteLine("[{0} {1}]", gapDps[gapCounter].AdNummer, gapDps[gapCounter].Type);
                }
                ++gapCounter;
            }
            return teSyncen;
        }

        /// <summary>
        /// Levert een lijstje op van actieve DP-abonnementen, voor monitoring (#5463)
        /// </summary>
        /// <param name="werkjaar">Huidige werkjaar mbt Dubbelpunt.</param>
        /// <returns>Lijst met actieve abonnementen.</returns>
        public static ActiefAbonnement[] AlleRelevanteAbonnementen(int werkjaar)
        {
            // Dit zou beter gebeuren met dependency injection. Maar het is en blijft een hack.
            using (var context = new ChiroGroepEntities())
            {
                var repositoryProvider = new RepositoryProvider(context);
                var repo = repositoryProvider.RepositoryGet<ActiefAbonnement>();

                // Abonnementen in GAP met werkjaar dat nationaal gesproken al voorbij is, zijn niet meer geldig.
                return repo.GetAll().Where(a => a.WerkJaar >= werkjaar).OrderBy(a => a.AdNummer).ToArray();
            }
        }
    }
}
