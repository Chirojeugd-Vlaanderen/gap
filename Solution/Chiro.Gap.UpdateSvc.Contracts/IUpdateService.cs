using System.ServiceModel;

namespace Chiro.Gap.UpdateSvc.Contracts
{
	/// <summary>
	/// Servicecontract voor de communicatie van kipadmin naar GAP (voor o.a. het updaten van AD-nummers)
	/// </summary>
	[ServiceContract]
	public interface IUpdateService
	{
		/// <summary>
		/// Stelt het AD-nummer van de persoon met ID <paramref name="persoonID"/> in.  
		/// </summary>
		/// <param name="adNummer">Nieuw AD-nummer</param>
		/// <param name="persoonID">ID van de persoon</param>
		[OperationContract(IsOneWay = true)]
		void AdNummerToekennen(int persoonID, int adNummer);

		/// <summary>
		/// Vervangt het AD-nummer van de persoon met AD-nummer <paramref name="oudAd"/>
		/// door <paramref name="nieuwAd"/>.  Als er al een persoon bestond met AD-nummer
		/// <paramref name="nieuwAd"/>, dan worden de personen gemerged.
		/// </summary>
		/// <param name="oudAd">AD-nummer van persoon met te vervangen AD-nummer</param>
		/// <param name="nieuwAd">nieuw AD-nummer</param>
		[OperationContract(IsOneWay = true)]
		void AdNummerVervangen(int oudAd, int nieuwAd);
	}
}
