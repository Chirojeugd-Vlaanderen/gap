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
		/// <param name="bewoners">AD-nummers en adrestypes voor personen de dat adres moeten krijgen</param>
		[OperationContract(IsOneWay = true)]
		void VoorkeurAdresUpdated(Adres adres, IEnumerable<Bewoner> bewoners);

		/// <summary>
		/// Aan te roepen met 'contactinfo'.  De nieuwe contactinfo wordt vooraan toegevoegd aan de lijst
		/// met bestaande contactinfo van hetzelfde type
		/// </summary>
		/// <param name="adNr">AD-nummer van persoon waarvoor contactinfo toe te voegen</param>
		/// <param name="communicatieMiddelen">toe te voegen contactinfo</param>
		[OperationContract(IsOneWay = true)]
		void CommunicatieToevoegen(int adNr, IEnumerable<CommunicatieMiddel> communicatieMiddelen);

		/// <summary>
		/// Verwijdert een communicatiemiddel uit Kipadmin.
		/// </summary>
		/// <param name="adNr">AD-nummer van persoon die communicatiemiddel moet verliezen</param>
		/// <param name="communicatie">Gegevens over het te verwijderen communicatiemiddel</param>
		[OperationContract(IsOneWay = true)]
		void CommunicatieVerwijderen(int adNr, CommunicatieMiddel communicatie);
	}
}
