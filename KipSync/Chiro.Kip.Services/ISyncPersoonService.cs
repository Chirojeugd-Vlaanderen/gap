using System.Collections.Generic;
using System.ServiceModel;
using Chiro.Kip.Services.DataContracts;

namespace Chiro.Kip.Services
{
	[ServiceContract]
	public interface ISyncPersoonService
	{
		[OperationContract(IsOneWay = true)]
		void PersoonUpdated(Persoon persoon);

		/// <summary>
		/// Aan te roepen als een voorkeursadres gewijzigd moet worden.
		/// </summary>
		/// <param name="adres">Nieuw voorkeursadres</param>
		/// <param name="adNummers">AD-nummers van personen de dat adres moeten krijgen</param>
		[OperationContract(IsOneWay = true)]
		void VoorkeurAdresUpdated(Adres adres, IEnumerable<int> adNummers);

		[OperationContract(IsOneWay = true)]
		void CommunicatieUpdated(Persoon persoon, IEnumerable<Communicatiemiddel> communicatiemiddelen);
	}
}
