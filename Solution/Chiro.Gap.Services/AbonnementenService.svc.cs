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
﻿using System;
using System.Diagnostics;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AbonnementenService" in code, svc and config file together.
    public class AbonnementenService : IAbonnementenService
    {
        private readonly IGelieerdePersonenManager _gpMgr;
        private readonly IAbonnementenManager _abMgr;
        private readonly ILedenManager _ledenMgr;
        private readonly IVeelGebruikt _veelGebruikt;



        /// <summary>
        /// Constructor, zorgt voor dependency injection
        /// </summary>
        /// <param name="gelieerdePersonenManager">Businesslogica voor gelieerde personen</param>
        /// <param name="abonnementenManager">Businesslogica voor abonnementen</param>
        /// <param name="ledenManager">Businesslogica voor leden</param>
        /// <param name="veelGebruikt">cache</param>
        public AbonnementenService(IGelieerdePersonenManager gelieerdePersonenManager,
                                   IAbonnementenManager abonnementenManager, ILedenManager ledenManager,
                                   IVeelGebruikt veelGebruikt)
        {
            _gpMgr = gelieerdePersonenManager;
            _abMgr = abonnementenManager;
            _ledenMgr = ledenManager;
            _veelGebruikt = veelGebruikt;
        }

        /// <summary>
        /// Bestelt Dubbelpunt voor de persoon met GelieerdePersoonID <paramref name="gelieerdePersoonID"/>.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon van persoon die Dubbelpunt wil</param>
        public void DubbelPuntBestellen(int gelieerdePersoonID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
            // TODO (#1048): exceptions op databaseniveau catchen

            //GelieerdePersoon gp = null;
            //Abonnement abonnement = null;

            //try
            //{
            //    // Het ophalen van alle groepswerkjaren is overkill.  Maar ik doe het toch.
            //    gp = _gpMgr.Ophalen(gelieerdePersoonID, PersoonsExtras.Adressen | PersoonsExtras.AbonnementenDitWerkjaar | PersoonsExtras.GroepsWerkJaren);
            //}
            //catch (GeenGavException ex)
            //{
            //    FoutAfhandelaar.FoutAfhandelen(ex);
            //}
            //catch (FoutNummerException ex)
            //{
            //    FoutAfhandelaar.FoutAfhandelen(ex);
            //}

            //Debug.Assert(gp != null);

            //var groepsWerkJaar = gp.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).FirstOrDefault();   // recentste groepswerkjaar
            //var dubbelpunt = _abMgr.PublicatieOphalen(PublicatieTypeEnum.Dubbelpunt);

            //try
            //{
            //    abonnement = _abMgr.Abonneren(dubbelpunt, gp, groepsWerkJaar);
            //}
            //catch (BlokkerendeObjectenException<Abonnement> ex)
            //{
            //    // heeft al een abonnement
            //    FoutAfhandelaar.FoutAfhandelen(ex);
            //}
            //catch (FoutNummerException ex)
            //{
            //    if (ex.FoutNummer == FoutNummer.AdresOntbreekt || ex.FoutNummer == FoutNummer.BestelPeriodeDubbelpuntVoorbij)
            //    {
            //        // Verwachte exception afhandelen
            //        FoutAfhandelaar.FoutAfhandelen(ex);
            //    }
            //    else
            //    {
            //        // Onverwachte exception opnieuw throwen
            //        throw;
            //    }
            //}

            //_abMgr.Bewaren(abonnement);
        }

        /// <summary>
        /// Controleert of de gelieerde persoon met gegeven <paramref name="id"/> recht heeft op gratis Dubbelpunt.
        /// </summary>
        /// <param name="id">GelieerdePersoonID</param>
        /// <returns><c>true</c> als de persoon zeker een gratis dubbelpuntabonnement krijgt voor jouw groep. <c>false</c>
        /// als de persoon zeker geen gratis dubbelpuntabonnement krijgt voor jouw groep. En <c>null</c> als het niet
        /// duidelijk is. (In praktijk: als de persoon contactpersoon is, maar als er meerdere contactpersonen zijn.)
        /// </returns>
        public bool? HeeftGratisDubbelpunt(int id)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
            //var groep = _gpMgr.Ophalen(id, PersoonsExtras.Groep).Groep;
            //var groepsWerkJaar = _veelGebruikt.GroepsWerkJaarIDOphalen(groep.ID);
            //var contactPersonen = _ledenMgr.Ophalen(groepsWerkJaar.ID, NationaleFunctie.ContactPersoon);

            //if (contactPersonen.Any(ld => ld.GelieerdePersoon.ID == id))
            //{
            //    if (contactPersonen.First() == contactPersonen.Last())
            //    {
            //        // Er is maar 1 contactpersoon, en dat is de gelieerde persoon die we zoeken
            //        return true;    // gratis dubbelpunt
            //    }
            //    // Meerdere contactpersonen. Mogelijk gratis dubbelpunt.
            //    return null;
            //}

            //// Persoon is geen contactpersoon van zijn groep. Geen gratis dubbelpunt.
            //return false;
        }
    }
}
