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

using System.Diagnostics;

using AutoMapper;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

using CommunicatieType = Chiro.Kip.ServiceContracts.DataContracts.CommunicatieType;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Regelt de synchronisatie van communicatiemiddelen naar Kipadmin
	/// </summary>
	public class CommunicatieSync : ICommunicatieSync
	{
		/// <summary>
		/// Verwijdert een communicatievorm uit Kipadmin
		/// </summary>
		/// <param name="communicatieVorm">Te verwijderen communicatievorm, gekoppeld aan een gelieerde persoon 
		/// met ad-nummer.</param>
		public void Verwijderen(CommunicatieVorm communicatieVorm)
		{
			Debug.Assert(communicatieVorm.GelieerdePersoon != null);
			Debug.Assert(communicatieVorm.GelieerdePersoon.Persoon != null);

            ServiceHelper.CallService<ISyncPersoonService>(
		        svc =>
		        svc.CommunicatieVerwijderen(
		            Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(communicatieVorm.GelieerdePersoon.Persoon),
		            new CommunicatieMiddel
		                {
		                    Type = (CommunicatieType)communicatieVorm.CommunicatieType.ID,
		                    Waarde = communicatieVorm.Nummer
		                }));
		}

		/// <summary>
		/// Bewaart een communicatievorm in Kipadmin
		/// </summary>
        /// <param name="communicatieVorm">Te bewaren communicatievorm, gekoppeld aan persoon</param>
		public void Toevoegen(CommunicatieVorm communicatieVorm)
		{
			Debug.Assert(communicatieVorm.GelieerdePersoon != null);
			Debug.Assert(communicatieVorm.GelieerdePersoon.Persoon != null);

		    ServiceHelper.CallService<ISyncPersoonService>(
		        svc =>
		        svc.CommunicatieToevoegen(
		            Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(
		                communicatieVorm.GelieerdePersoon.Persoon),
		            new CommunicatieMiddel
		                {
		                    Type = (CommunicatieType)communicatieVorm.CommunicatieType.ID,
		                    Waarde = communicatieVorm.Nummer,
		                    GeenMailings = !communicatieVorm.IsVoorOptIn
		                }));
		}

        /// <summary>
        /// Stuurt de gegeven <paramref name="communicatieVorm"/> naar Kipadmin. Om te weten welk de
        /// originele communicatievorm is, kijken we naar de gekoppelde persoon, en gebruiken we
        /// het oorspronkelijke nummer (<paramref name="origineelNummer"/>)
        /// </summary>
        /// <param name="communicatieVorm">Te updaten communicatievorm</param>
        /// <param name="origineelNummer">Oorspronkelijk nummer van die communicatievorm</param>
        /// <remarks>Het is best mogelijk dat het 'nummer' niet is veranderd, maar bijv. enkel de vlag 
        /// 'opt-in'</remarks>
	    public void Bijwerken(CommunicatieVorm communicatieVorm, string origineelNummer)
	    {
            Debug.Assert(communicatieVorm.GelieerdePersoon != null);
            Debug.Assert(communicatieVorm.GelieerdePersoon.Persoon != null);

            ServiceHelper.CallService<ISyncPersoonService>(
                svc =>
                svc.CommunicatieBijwerken(
                    Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(
                        communicatieVorm.GelieerdePersoon.Persoon),
                    origineelNummer,
                    new CommunicatieMiddel
                        {
                            Type = (CommunicatieType) communicatieVorm.CommunicatieType.ID,
                            Waarde = communicatieVorm.Nummer,
                            GeenMailings = !communicatieVorm.IsVoorOptIn
                        }));
	    }
	}
}
