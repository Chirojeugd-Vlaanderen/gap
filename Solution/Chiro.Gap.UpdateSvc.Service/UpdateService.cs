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
	// Onderstaand attribuut zorgt ervoor dat er steeds maar 1 instantie van de service actief is.
	// Dat lijkt me niet echt nodig, maar ik doe dit als workaround om dependency injection in orde
	// te krijgen.
	//
	// TODO: Misschien kan dit ook wel via de dependency injection service behavior
	// (zie Chiro.Gap.Services)
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
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
			p.AdNummer = adNummer;
			_personenMgr.Bewaren(p);
		}
	}
}
