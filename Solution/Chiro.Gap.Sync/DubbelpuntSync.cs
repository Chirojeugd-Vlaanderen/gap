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
using System.Diagnostics;
using AutoMapper;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Klasse voor de synchronisatie van Dubbelpuntabonnementen
	/// </summary>
	public class DubbelpuntSync : IDubbelpuntSync
	{
	    /// <summary>
	    /// Synct een Dubbelpuntabonnement naar Kipadmin
	    /// </summary>
        /// <param name="abonnement">Te syncen abonnement</param>
	    public void Abonneren(Abonnement abonnement)
		{
			Debug.Assert(abonnement.GelieerdePersoon != null);
			Debug.Assert(abonnement.GelieerdePersoon.Persoon != null);
            Debug.Assert(abonnement.GroepsWerkJaar != null);
            Debug.Assert(abonnement.GroepsWerkJaar.Groep != null);

	        var gp = abonnement.GelieerdePersoon;
	        var huidigWj = abonnement.GroepsWerkJaar;

			if (gp.Persoon.AdNummer != null)
			{
				// Jeeej! We hebben een AD-nummer! Dubbelpuntabonnement is een fluitje van een cent.

				ServiceHelper.CallService<ISyncPersoonService>(svc => svc.DubbelpuntBestellen(
					gp.Persoon.AdNummer ?? 0,
					huidigWj.Groep.Code,
					huidigWj.WerkJaar));
			}
			else
			{
                throw new NotImplementedException();
			}
		}
	}
}
