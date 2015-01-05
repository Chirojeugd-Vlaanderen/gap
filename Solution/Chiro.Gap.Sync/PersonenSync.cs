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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Adres = Chiro.Gap.Poco.Model.Adres;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Klasse die de synchronisatie van persoonsgegevens naar Kipadmin regelt
	/// </summary>
	public class PersonenSync : BaseSync, IPersonenSync
	{
        /// <summary>
        /// Constructor.
        /// 
        /// De ServiceHelper wordt geïnjecteerd door de dependency injection container. Wat de
        /// ServiceHelper precies zal opleveren, hangt af van welke IChannelProvider geregistreerd
        /// is bij de container.
        /// </summary>
        /// <param name="serviceHelper">ServiceHelper, nodig voor service calls.</param>
        public PersonenSync(ServiceHelper serviceHelper) : base(serviceHelper) { }

		/// <summary>
		/// Stuurt de persoonsgegevens, samen met eventueel adressen en/of communicatie, naar Kipadmin
		/// </summary>
		/// <param name="gp">Gelieerde persoon, persoonsinfo</param>
		/// <param name="metStandaardAdres">Stuurt ook het standaardadres mee (moet dan wel gekoppeld zijn)</param>
		/// <param name="metCommunicatie">Stuurt ook communicatie mee.  Hiervoor wordt expliciet alle
		/// communicatie-info opgehaald, omdat de workers typisch niet toestaan dat de gebruiker alle
		/// communicatie ophaalt.</param>
		public void Bewaren(GelieerdePersoon gp, bool metStandaardAdres, bool metCommunicatie)
		{
			// Wijzigingen van personen met ad-nummer worden doorgesluisd
			// naar Kipadmin.

			Debug.Assert(gp.Persoon.InSync);

            var syncPersoon = Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(gp.Persoon);
			ServiceHelper.CallService<ISyncPersoonService>(svc => svc.PersoonUpdaten(syncPersoon));

			if (metStandaardAdres && gp.PersoonsAdres != null)
			{
				// Adressen worden mee bewaard.  Update standaardadres, als dat er is
				// (standaardadres is gelieerdePersoon.PersoonsAdres;
				// alle adressen in gelieerdePersoon.Persoon.PersoonsAdres).

				// TODO (#238): Buitenlandse adressen!
                var syncAdres = Mapper.Map<Adres, Kip.ServiceContracts.DataContracts.Adres>(gp.PersoonsAdres.Adres);

				var syncBewoner = new Bewoner
				{
                    Persoon = Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(gp.Persoon),
					AdresType = (AdresTypeEnum)gp.PersoonsAdres.AdresType
				};

				ServiceHelper.CallService<ISyncPersoonService>(svc => svc.StandaardAdresBewaren(syncAdres, new List<Bewoner> { syncBewoner }));
			}
			if (metCommunicatie)
			{
			    var syncCommunicatie =
			        Mapper.Map<IEnumerable<CommunicatieVorm>, List<CommunicatieMiddel>>(
			            gp.Persoon.GelieerdePersoon.SelectMany(gp2 => gp2.Communicatie));
			    ServiceHelper.CallService<ISyncPersoonService>(svc => svc.AlleCommunicatieBewaren(syncPersoon, syncCommunicatie));
			}
		}
	}
}
