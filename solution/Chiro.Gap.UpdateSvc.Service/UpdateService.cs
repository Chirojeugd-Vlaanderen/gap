// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
        private readonly IDubbelpuntSync _dubbelpuntSync;

        private readonly IRepository<Groep> _groepenRepo;
        private readonly IRepository<Persoon> _personenRepo;
        
        private readonly GavChecker _gav;

        public UpdateService(IAutorisatieManager autorisatieMgr, ILedenSync ledenSync, IDubbelpuntSync dubbelpuntSync, IRepositoryProvider repositoryProvider)
        {
            _context = repositoryProvider.ContextGet();
            _personenRepo = repositoryProvider.RepositoryGet<Persoon>();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
            _ledenSync = ledenSync;
            _dubbelpuntSync = dubbelpuntSync;
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
            throw new NotImplementedException(NIEUWEBACKEND.Info);
            //var g = _groepenMgr.Ophalen(stamNr);
            //g.StopDatum = stopDatum;
            //_groepenMgr.Bewaren(g);
            //Console.WriteLine(stopDatum == null ? "Groep opnieuw geactiveerd: {0}" : "Groep gedesactiveerd: {0}", stamNr);
        }

        /// <summary>
        /// Synct alle leden van het recentste werkJaar van een groep opnieuw naar Kipadmin
        /// </summary>
        /// <param name="stamNummer">Stamnummer van groep met te syncen leden</param>
        public void OpnieuwSyncen(string stamNummer)
        {
            var groep = (from g in _groepenRepo where Equals(g.Code, stamNummer) select g).FirstOrDefault();
            if (groep == null)
            {
                Console.WriteLine("Geen groep gevonden voor {0}", stamNummer);
                return;
            }

            var gwj = groep.GroepsWerkJaar.OrderByDescending(e => e.WerkJaar).FirstOrDefault();
            if (gwj == null)
            {
                Console.WriteLine("Geen groepswerkjaar gevonden voor {0}", stamNummer);
                return;
            }

            foreach (var lid in gwj.Lid)
            {
                _ledenSync.Bewaren(lid);
            }

                Console.WriteLine("Leden van {0} voor werkjaar {1} opnieuw gesynct naar Kipadmin", stamNummer, gwj.WerkJaar);                
        }

        /// <summary>
        /// Synct alle abonnementen van het recentste werkJaar van een groep opnieuw naar Kipadmin
        /// </summary>
        /// <param name="stamNummer">Stamnummer van groep met te syncen abonnementen</param>
        /// <remarks>Dit is eigenlijk geen sync van Kipadmin naar GAP, maar een vraag van Kipadmin
        /// aan GAP om bepaalde zaken opnieuw te syncen.  Eigenlijk staat dit dus niet op zijn
        /// plaats in deze service.  Maar voorlopig staat het hier, omdat UpdateService de
        /// enige manier is om communicatie van KIP naar GAP te arrangeren.</remarks>
        public void AbonnementenOpnieuwSyncen(string stamNummer)
        {
            var groep = (from g in _groepenRepo where Equals(g.Code, stamNummer) select g).FirstOrDefault();
            if (groep == null)
            {
                Console.WriteLine("Geen groep gevonden voor {0}", stamNummer);
                return;
            }

            var gwj = groep.GroepsWerkJaar.OrderByDescending(e => e.WerkJaar).FirstOrDefault();
            if (gwj == null)
            {
                Console.WriteLine("Geen groepswerkjaar gevonden voor {0}", stamNummer);
                return;
            }

            foreach (var ab in gwj.Abonnement)
            {
                _dubbelpuntSync.Abonneren(ab);
            }

                Console.WriteLine("Abonnementen van {0} voor werkjaar {1} opnieuw gesynct naar Kipadmin", stamNummer, gwj.WerkJaar);                
        }
    }
}