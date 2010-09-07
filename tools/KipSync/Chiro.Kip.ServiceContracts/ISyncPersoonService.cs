using System.Collections.Generic;
using System.ServiceModel;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Kip.ServiceContracts
{
	/// <summary>
	/// Servicecontract voor communicatie GAP->KIP
	/// 
	/// BELANGRIJK: Oorspronkelijk werden voor de meeste methods geen personen over de lijn gestuurd, maar enkel
	/// AD-nummers.  Het idee daarachter was dat toch enkel gegevens van personen met AD-nummer naar kipadmin
	/// gesynct moeten worden.
	/// 
	/// Maar met het AD-nummer alleen kom je er niet.  Het kan namelijk goed zijn dat een persoon gewijzigd wordt
	/// tussen het moment dat hij voor het eerst lid wordt, en het moment dat hij zijn AD-nummer krijgt.  Deze
	/// wijzigingen willen we niet verliezen.
	/// 
	/// Het PersoonID van GAP meesturen helpt in de meeste gevallen.  Maar dat kan mis gaan op het moment dat een persoon
	/// uit kipadmin nog dubbel in GAP zit.  Vooraleer deze persoon zijn AD-nummer krijgt, weten we dat immers niet.
	/// 
	/// Vandaar dat nu alle methods volledige persoonsobjecten gebruiken, zodat het opzoeken van een persoon zo optimaal
	/// mogelijk kan gebeuren.  Het persoonsobject een AD-nummer heeft, wordt er niet naar de rest gekeken.
	/// </summary>
	[ServiceContract]
	public interface ISyncPersoonService
	{
		[OperationContract(IsOneWay = true)]
		void PersoonUpdaten(Persoon persoon);

		/// <summary>
		/// Aan te roepen als een voorkeursadres gewijzigd moet worden.
		/// </summary>
		/// <param name="adres">Nieuw voorkeursadres</param>
		/// <param name="bewoners">AD-nummers en adrestypes voor personen de dat adres moeten krijgen</param>
		[OperationContract(IsOneWay = true)]
		void StandaardAdresBewaren(Adres adres, IEnumerable<Bewoner> bewoners);

		/// <summary>
		/// Voegt 1 communicatiemiddel toe aan de communicatiemiddelen van een persoon
		/// </summary>
		/// <param name="persoon">Persoon die het nieuwe communicatiemiddel krijgt</param>
		/// <param name="communicatieMiddel"></param>
		[OperationContract(IsOneWay = true)]
		void CommunicatieToevoegen(Persoon persoon, CommunicatieMiddel communicatieMiddel);

		/// <summary>
		/// Verwijdert alle bestaande contactinfo, en vervangt door de contactinfo meegegeven in 
		/// <paramref name="communicatieMiddelen"/>.
		/// </summary>
		/// <param name="persoon">persoon waarvoor contactinfo te updaten</param>
		/// <param name="communicatieMiddelen">te updaten contactinfo</param>
		[OperationContract(IsOneWay = true)]
		void AlleCommunicatieBewaren(Persoon persoon, IEnumerable<CommunicatieMiddel> communicatieMiddelen);

		/// <summary>
		/// Verwijdert een communicatiemiddel uit Kipadmin.
		/// </summary>
		/// <param name="pers">Persoonsgegevens van de persoon waarvan het communicatiemiddel moet verdwijnen.</param>
		/// <param name="communicatie">Gegevens over het te verwijderen communicatiemiddel</param>
		[OperationContract(IsOneWay = true)]
		void CommunicatieVerwijderen(Persoon pers, CommunicatieMiddel communicatie);

		/// <summary>
		/// Maakt een persoon met gekend ad-nummer lid, of updatet een bestaand lid
		/// </summary>
		/// <param name="gedoe">de nodige info voor het lid.</param>
		[OperationContract(IsOneWay = true)]
		void LidBewaren(
			int adNummer,
			LidGedoe gedoe);

		/// <summary>
		/// Maakt een persoon zonder ad-nummer lid.
		/// </summary>
		/// <param name="details">Details van de persoon die lid moet kunnen worden</param>
		/// <param name="lidGedoe">nodige info om lid te kunnen maken</param>
		[OperationContract(IsOneWay = true)]
		void NieuwLidBewaren(
			PersoonDetails details,
			LidGedoe lidGedoe);

		/// <summary>
		/// Updatet de functies van een lid.
		/// </summary>
		/// <param name="persoon">Persoon waarvan de lidfuncties geupdatet moeten worden</param>
		/// <param name="stamNummer">Stamnummer van de groep waarin de persoon lid is</param>
		/// <param name="werkJaar">Werkjaar waarin de persoon lid is</param>
		/// <param name="functies">Toe te kennen functies.  Eventuele andere reeds toegekende functies worden verwijderd.</param>
		[OperationContract(IsOneWay = true)]
		void FunctiesUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<FunctieEnum> functies);

		/// <summary>
		/// Updatet de afdelingen van een lid.
		/// </summary>
		/// <param name="persoon">Persoon waarvan de afdelingen geupdatet moeten worden</param>
		/// <param name="stamNummer">Stamnummer van de groep waarin de persoon lid is</param>
		/// <param name="werkJaar">Werkjaar waarin de persoon lid is</param>
		/// <param name="afdelingen">Toe te kennen afdelingen.  Eventuele andere reeds toegekende functies worden verwijderd.</param>
		/// <remarks>Er is in Kipadmin maar plaats voor 2 afdelingen/lid</remarks>
		[OperationContract(IsOneWay = true)]
		void AfdelingenUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<AfdelingEnum> afdelingen);

		/// <summary>
		/// Bestelt dubbelpunt voor de gegeven persoon in het gegeven groepswerkjaar, gegeven dat de persoon
		/// een AD-nummer heeft
		/// </summary>
		/// <param name="adNummer">AD-nummer van persoon die Dubbelpunt wil</param>
		/// <param name="stamNummer">Groep die Dubbelpunt betaalt</param>
		/// <param name="werkJaar">Werkjaar waarvoor Dubbelpuntabonnement</param>
		[OperationContract(IsOneWay = true)]
		void DubbelpuntBestellen(int adNummer, string stamNummer, int werkJaar);

		/// <summary>
		/// Bestelt dubbelpunt voor een 'onbekende' persoon in het gegeven groepswerkjaar
		/// </summary>
		/// <param name="details">details voor de persoon die Dubbelpunt wil bestellen</param>
		/// <param name="stamNummer">Groep die Dubbelpunt betaalt</param>
		/// <param name="werkJaar">Werkjaar waarvoor Dubbelpuntabonnement</param>
		[OperationContract(IsOneWay = true)]
		void DubbelpuntBestellenNieuwePersoon(PersoonDetails details, string stamNummer, int werkJaar);

		/// <summary>
		/// Verzekert de gegeven persoon in het gegeven groepswerkjaar tegen loonverlies, gegeven dat de persoon
		/// een AD-nummer heeft
		/// </summary>
		/// <param name="adNummer">AD-nummer van te verzekeren persoon</param>
		/// <param name="stamNummer">Stamnummer van betalende groep</param>
		/// <param name="werkJaar">Werkjaar voor de verzekering</param>
		[OperationContract(IsOneWay = true)]
		void LoonVerliesVerzekeren(int adNummer, string stamNummer, int werkJaar);

		/// <summary>
		/// Verzekert een persoon waarvan we het AD-nummer nog niet kennen tegen loonverlies
		/// </summary>
		/// <param name="details">Details van de te verzekeren persoon</param>
		/// <param name="stamNummer">Stamnummer van betalende groep</param>
		/// <param name="werkJaar">Werkjaar voor de verzekering</param>
		[OperationContract(IsOneWay = true)]
		void LoonVerliesVerzekerenAdOnbekend(PersoonDetails details, string stamNummer, int werkJaar);	

	}
}
