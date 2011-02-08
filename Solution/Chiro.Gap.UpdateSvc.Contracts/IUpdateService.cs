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
	}
}
