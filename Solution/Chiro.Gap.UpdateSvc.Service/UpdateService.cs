using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using Chiro.Gap.Orm;
using Chiro.Gap.UpdateSvc.Contracts;
using Chiro.Gap.Workers;

namespace Chiro.Gap.UpdateSvc.Service
{
	public class UpdateService : IUpdateService
	{
		private readonly PersonenManager _personenMgr;

		/// <summary>
		/// Dependency Injection via de constructor
		/// </summary>
		/// <param name="personenManager">te gebruiken personenmanagerobject</param>
		public UpdateService(PersonenManager personenManager)
		{
			_personenMgr = personenManager;
		}

		/// <summary>
		/// Kent het AD-nummer <paramref name="adNummer"/> toe aan de persoon met ID
		/// <paramref name="persoonID"/>.
		/// </summary>
		/// <param name="persoonID">ID van persoon met toe te kennen AD-nummer</param>
		/// <param name="adNummer">toe te kennen AD-nummer</param>
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
		/// <param name="nieuwAd">nieuw AD-nummer</param>
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
	}
}
