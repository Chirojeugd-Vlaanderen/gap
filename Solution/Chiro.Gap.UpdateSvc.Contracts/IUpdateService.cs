// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
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
		/// <param name="persoonID">
		/// ID van de persoon
		/// </param>
		/// <param name="adNummer">
		/// Nieuw AD-nummer
		/// </param>
		[OperationContract(IsOneWay = true)]
		void AdNummerToekennen(int persoonID, int adNummer);

		/// <summary>
		/// Vervangt het AD-nummer van de persoon met AD-nummer <paramref name="oudAd"/>
		/// door <paramref name="nieuwAd"/>.  Als er al een persoon bestond met AD-nummer
		/// <paramref name="nieuwAd"/>, dan worden de personen gemerged.
		/// </summary>
		/// <param name="oudAd">AD-nummer van persoon met te vervangen AD-nummer</param>
		/// <param name="nieuwAd">Nieuw AD-nummer</param>
		[OperationContract(IsOneWay = true)]
		void AdNummerVervangen(int oudAd, int nieuwAd);

        /// <summary>
        /// Markeert een groep in GAP als gestopt.
        /// </summary>
        /// <param name="stamNr">Stamnummer te stoppen groep</param>
        /// <param name="stopDatum">Datum vanaf wanneer gestopt</param>
	    [OperationContract(IsOneWay = true)]
	    void GroepDesactiveren(string stamNr, DateTime stopDatum);

        /// <summary>
        /// Synct alle leden van het recentste werkJaar van een groep opnieuw naar Kipadmin
        /// </summary>
        /// <param name="stamNummer">Stamnummer van groep met te syncen leden</param>
	    [OperationContract(IsOneWay = true)]
	    void OpnieuwSyncen(string stamNummer);

	    /// <summary>
	    /// Synct alle abonnementen van het recentste werkJaar van een groep opnieuw naar Kipadmin
	    /// </summary>
	    /// <param name="stamNummer">Stamnummer van groep met te syncen abonnementen</param>
	    /// <remarks>Dit is eigenlijk geen sync van Kipadmin naar GAP, maar een vraag van Kipadmin
	    /// aan GAP om bepaalde zaken opnieuw te syncen.  Eigenlijk staat dit dus niet op zijn
	    /// plaats in deze service.  Maar voorlopig staat het hier, omdat UpdateService de
	    /// enige manier is om communicatie van KIP naar GAP te arrangeren.</remarks>
        [OperationContract(IsOneWay = true)]
	    void AbonnementenOpnieuwSyncen(string stamNummer);
	}
}
