/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
		[OperationContract]
		void AdNummerToekennen(int persoonID, int adNummer);

        /// <summary>
        /// Kent gegeven <paramref name="civiID"/> toe aan de persoon met gegeven
        /// <paramref name="persoonID"/>.
        /// </summary>
        /// <param name="persoonID">ID van persoon met toe te kennen Civi-ID</param>
        /// <param name="civiID">toe te kennen Civi-ID</param>
        [OperationContract]
        void CiviIdToekennen(int persoonID, int civiID);

		/// <summary>
		/// Vervangt het AD-nummer van de persoon met AD-nummer <paramref name="oudAd"/>
		/// door <paramref name="nieuwAd"/>.  Als er al een persoon bestond met AD-nummer
		/// <paramref name="nieuwAd"/>, dan worden de personen gemerged.
		/// </summary>
		/// <param name="oudAd">AD-nummer van persoon met te vervangen AD-nummer</param>
		/// <param name="nieuwAd">Nieuw AD-nummer</param>
		[OperationContract]
		void AdNummerVervangen(int oudAd, int nieuwAd);

	    /// <summary>
	    /// Markeert een groep in GAP als gestopt. Of als terug actief.
	    /// </summary>
	    /// <param name="stamNr">Stamnummer te stoppen groep</param>
	    /// <param name="stopDatum">Datum vanaf wanneer gestopt, <c>null</c> om de groep opnieuw te activeren.</param>
	    /// <remarks>Als <paramref name="stopDatum"/> <c>null</c> is, wordt de groep opnieuw actief.</remarks>
	    [OperationContract]
	    void GroepDesactiveren(string stamNr, DateTime? stopDatum);      
	}
}
