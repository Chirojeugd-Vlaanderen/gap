// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Linq;
using System.ServiceModel;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.UpdateSvc.Contracts;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers;

namespace Chiro.Gap.UpdateSvc.Service
{
    /// <summary>
    /// Service die updates doorgeeft van GAP naar KipAdmin of omgekeerd
    /// </summary>
    public class UpdateService : IUpdateService
	{
		private readonly PersonenManager _personenMgr;
	    private readonly IGroepenManager _groepenMgr;
	    private readonly ILedenManager _ledenMgr;
	    private readonly AbonnementenManager _abonnementenManager;
	    private readonly ILedenSync _ledenSync;
	    private readonly IDubbelpuntSync _dubbelpuntSync;

	    /// <summary>
	    /// Dependency Injection via de constructor
	    /// </summary>
	    /// <param name="personenManager">
	    /// Te gebruiken worker voor Personen
	    /// </param>
	    /// <param name="groepenManager">
	    /// Te gebruiken worker voor Groepen
	    /// </param>
	    /// <param name="ledenManager">
	    /// Te gebruiken worker voor Leden
	    /// </param>
	    /// <param name="abonnementenManager">
        /// Te gebruiken worker voor abonnementen
	    /// </param>
	    /// <param name="ledenSync">
	    /// Te gebruiken LedenSync terug naar Kipadmin
	    /// </param>
	    /// <param name="dubbelpuntSync">
        /// Te gebruiken DubbelpuntSync terug naar Kipadmin
	    /// </param>
	    public UpdateService(PersonenManager personenManager, IGroepenManager groepenManager, ILedenManager ledenManager, AbonnementenManager abonnementenManager, ILedenSync ledenSync, IDubbelpuntSync dubbelpuntSync)
		{
			_personenMgr = personenManager;
		    _groepenMgr = groepenManager;
	        _ledenMgr = ledenManager;
	        _abonnementenManager = abonnementenManager;
	        _ledenSync = ledenSync;
	        _dubbelpuntSync = dubbelpuntSync;
		}

		/// <summary>
		/// Kent het AD-nummer <paramref name="adNummer"/> toe aan de persoon met ID
		/// <paramref name="persoonID"/>.
		/// </summary>
		/// <param name="persoonID">ID van persoon met toe te kennen AD-nummer</param>
		/// <param name="adNummer">Toe te kennen AD-nummer</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AdNummerToekennen(int persoonID, int adNummer)
		{
			Persoon p = _personenMgr.Ophalen(persoonID);

			_personenMgr.AdNummerToekennen(p, adNummer);

			Console.WriteLine("Ad-nummer {0} toegekend aan {1}. (ID {2})", adNummer, p.VolledigeNaam, p.ID);
		}

		/// <summary>
		/// Vervangt het AD-nummer van de persoon met AD-nummer <paramref name="oudAd"/>
		/// door <paramref name="nieuwAd"/>.  Als er al een persoon bestond met AD-nummer
		/// <paramref name="nieuwAd"/>, dan worden de personen gemerged.
		/// </summary>
		/// <param name="oudAd">AD-nummer van persoon met te vervangen AD-nummer</param>
		/// <param name="nieuwAd">Nieuw AD-nummer</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AdNummerVervangen(int oudAd, int nieuwAd)
		{
			var gevonden = _personenMgr.ZoekenOpAd(oudAd);

			foreach (var p in gevonden)
			{
				_personenMgr.AdNummerToekennen(p, nieuwAd);
				Console.WriteLine("Ad-nummer {0} vervangen door {1}. ({2}, ID {3})", oudAd, nieuwAd, p.VolledigeNaam, p.ID);
			}
		}

        /// <summary>
        /// Synct alle leden van het recentste werkJaar van een groep opnieuw naar Kipadmin
        /// </summary>
        /// <param name="stamNummer">Stamnummer van groep met te syncen leden</param>
        /// <remarks>Dit is eigenlijk geen sync van Kipadmin naar GAP, maar een vraag van Kipadmin
        /// aan GAP om bepaalde zaken opnieuw te syncen.  Eigenlijk staat dit dus niet op zijn
        /// plaats in deze service.  Maar voorlopig staat het hier, omdat UpdateService de
        /// enige manier is om communicatie van KIP naar GAP te arrangeren.</remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
	    public void OpnieuwSyncen(string stamNummer)
        {
            // ophalen met stamnummer levert (toevallig ;-/) het recentste groepswerkjaar mee.

            var gwj = _groepenMgr.Ophalen(stamNummer).GroepsWerkJaar.FirstOrDefault();

            if (gwj == null)
            {
                Console.WriteLine("Geen groepswerkjaar gevonden voor {0}", stamNummer);
            }
            else
            {
                var leden = _ledenMgr.OphalenUitGroepsWerkJaar(gwj.ID, true);

                foreach (var lid in leden)
                {
                    _ledenSync.Bewaren(lid);
                }

                Console.WriteLine("Leden van {0} voor werkJaar {1} opnieuw gesynct naar Kipadmin", stamNummer, gwj.WerkJaar);                
            }
        }

        /// <summary>
        /// Synct alle abonnementen van het recentste werkJaar van een groep opnieuw naar Kipadmin
        /// </summary>
        /// <param name="stamNummer">Stamnummer van groep met te syncen abonnementen</param>
        /// <remarks>Dit is eigenlijk geen sync van Kipadmin naar GAP, maar een vraag van Kipadmin
        /// aan GAP om bepaalde zaken opnieuw te syncen.  Eigenlijk staat dit dus niet op zijn
        /// plaats in deze service.  Maar voorlopig staat het hier, omdat UpdateService de
        /// enige manier is om communicatie van KIP naar GAP te arrangeren.</remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AbonnementenOpnieuwSyncen(string stamNummer)
        {
            // ophalen met stamnummer levert (toevallig ;-/) het recentste groepswerkjaar mee.

            var gwj = _groepenMgr.Ophalen(stamNummer).GroepsWerkJaar.FirstOrDefault();

            if (gwj == null)
            {
                Console.WriteLine("Geen groepswerkjaar gevonden voor {0}", stamNummer);
            }
            else
            {
                var abonnementen = _abonnementenManager.OphalenUitGroepsWerkJaar(gwj.ID);

                foreach (var ab in abonnementen)
                {
                    _dubbelpuntSync.Abonneren(ab);
                }

                Console.WriteLine("Abonnementen van {0} voor werkJaar {1} opnieuw gesynct naar Kipadmin", stamNummer, gwj.WerkJaar);                
            }
        }
	}
}