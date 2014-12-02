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

using Chiro.Cdf.Ioc;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers;

namespace Chiro.WatiN.Test
{
	/// <summary>
	/// Een aantal helperfuncties voor de WatiN-tests
	/// </summary>
	public static class TestHelper
	{
		private static GroepenManager _gMgr;
		private static IAutorisatieManager _auMgr;

		static TestHelper()
		{
			Factory.ContainerInit(); // Zeker zijn dat IOC-container geinitialiseerd is
			_gMgr = Factory.Maak<GroepenManager>();
			_auMgr = Factory.Maak<IAutorisatieManager>();
		}

		/// <summary>
		/// Roept de businesslaag rechtstreeks op (zonder user interface) om de alle (gelieerde) personen
		/// van een testgroep te verwijderen
		/// </summary>
		/// <param name="groepID">ID van de op te kuisen groep</param>
		public static void KuisOp(int groepID)
		{
			_gMgr.GelieerdePersonenVerwijderen(groepID, true);
		}

		/// <summary>
		/// Geeft de gebruiker die de test runt GAV rechten op de groep met gegeven <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">groepID van de groep waarop GAV-rechten gegeven moeten worden</param>
		public static void GeefGavRecht(int groepID)
		{
			// geef gebruiker gebruikersrecht voor een dag
			_auMgr.GebruikersRechtToekennen(
				String.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName),
				groepID,
				DateTime.Now.AddDays(1));
		}

		/// <summary>
		/// Ontneemt de GAV-rechten op de groep met gegeven <paramref name="groepID"/> van de gebruiker
		/// die de test runt.
		/// </summary>
		/// <param name="groepID">groepID van de groep waarop GAV-rechten gegeven moeten worden</param>
		public static void OntneemGavRecht(int groepID)
		{
			// ontneem gebruikersrecht door vervaldatum op Nu te zetten.
			_auMgr.GebruikersRechtToekennen(
				String.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName),
				groepID,
				DateTime.Now);
		}

	}
}
