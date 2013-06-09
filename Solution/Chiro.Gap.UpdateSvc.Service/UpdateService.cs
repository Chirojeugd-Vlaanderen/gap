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

using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Services;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.UpdateSvc.Contracts;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.UpdateSvc.Service
{
    /// <summary>
    /// Service die updates doorgeeft van GAP naar KipAdmin of omgekeerd
    /// </summary>
    public class UpdateService : IUpdateService
    {
        /// <summary>
        /// _context is verantwoordelijk voor het tracken van de wijzigingen aan de
        /// entiteiten. Via _context.SaveChanges() kunnen wijzigingen gepersisteerd
        /// worden.
        /// 
        /// Context is IDisposable. De context wordt aangemaakt door de IOC-container,
        /// en gedisposed op het moment dat de service gedisposed wordt. Dit gebeurt
        /// na iedere call.
        /// </summary>
        private readonly IContext _context;

        private readonly ILedenSync _ledenSync;

        private readonly IRepository<Groep> _groepenRepo;
        private readonly IRepository<Persoon> _personenRepo;

        private readonly GavChecker _gav;

        public UpdateService(IAutorisatieManager autorisatieMgr, ILedenSync ledenSync, IRepositoryProvider repositoryProvider)
        {
            _context = repositoryProvider.ContextGet();
            _personenRepo = repositoryProvider.RepositoryGet<Persoon>();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
            _ledenSync = ledenSync;
            _gav = new GavChecker(autorisatieMgr);
        }

        public GavChecker Gav
        {
            get { return _gav; }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Stelt het AD-nummer van de persoon met Id <paramref name="persoonId"/> in.  
        /// </summary>
        /// <param name="persoonId">
        /// Id van de persoon
        /// </param>
        /// <param name="adNummer">
        /// Nieuw AD-nummer
        /// </param>
        public void AdNummerToekennen(int persoonId, int adNummer)
        {
            var persoon = _personenRepo.ByID(persoonId);
            if (persoon == null)
            {
                return;
            }

            AdNummerToekennen(persoon, adNummer);

            Console.WriteLine("Ad-nummer {0} toegekend aan {1}. (ID {2})", adNummer, persoon.VolledigeNaam, persoon.ID);

            _context.SaveChanges();
        }

        /// <summary>
        /// Vervangt het AD-nummer van de persoon met AD-nummer <paramref name="oudAd"/>
        /// door <paramref name="nieuwAd"/>.  Als er al een persoon bestond met AD-nummer
        /// <paramref name="nieuwAd"/>, dan worden de personen gemerged.
        /// </summary>
        /// <param name="oudAd">AD-nummer van persoon met te vervangen AD-nummer</param>
        /// <param name="nieuwAd">Nieuw AD-nummer</param>
        public void AdNummerVervangen(int oudAd, int nieuwAd)
        {
            var personen = (from g in _personenRepo.Select() where g.AdNummer == oudAd select g);
            foreach (var p in personen)
            {
                AdNummerToekennen(p, nieuwAd);
                Console.WriteLine("Ad-nummer {0} vervangen door {1}. ({2}, ID {3})", oudAd, nieuwAd, p.VolledigeNaam, p.ID);
            }

            _context.SaveChanges();
        }

        private void AdNummerToekennen(Persoon persoon, int adNummer)
        {
            Gav.CheckSuperGav();

            // Wie heeft het gegeven AD-nummer al?
            var personenAlBestaand = (from g in _personenRepo.Select() where g.AdNummer == adNummer select g);

            foreach (var p in personenAlBestaand.Where(prs => prs.ID != persoon.ID))
            {
                // Als er andere personen zijn met hetzelfde AD-nummer, merge dan met deze persoon.
                // Door 'persoon.ID' als origineel te kiezen, vermijden we dat persoon van ID verandert.
                throw new NotImplementedException();
                // Was vroeger blijkbaar een stored procedure
                // _dao.DubbelVerwijderen(persoon.ID, p.ID);
            }

            persoon.AdNummer = adNummer;
        }

        /// <summary>
        /// Markeert een groep in GAP als gestopt. Of als terug actief.
        /// </summary>
        /// <param name="stamNr">Stamnummer te stoppen groep</param>
        /// <param name="stopDatum">Datum vanaf wanneer gestopt, <c>null</c> om de groep opnieuw te activeren.</param>
        /// <remarks>Als <paramref name="stopDatum"/> <c>null</c> is, wordt de groep opnieuw actief.</remarks>
        public void GroepDesactiveren(string stamNr, DateTime? stopDatum)
        {
            // De Johan moet dit nog verder uitwerken, wordt opgeroepen voor KipAdmin als een groep gestopt is
            throw new NotImplementedException(Nieuwebackend.Info);
            //var g = _groepenMgr.Ophalen(stamNr);
            //g.StopDatum = stopDatum;
            //_groepenMgr.Bewaren(g);
            //Console.WriteLine(stopDatum == null ? "Groep opnieuw geactiveerd: {0}" : "Groep gedesactiveerd: {0}", stamNr);
        }

    }
}
