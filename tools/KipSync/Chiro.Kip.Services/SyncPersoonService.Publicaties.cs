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
﻿using System;
using System.Diagnostics;
using System.ServiceModel;
using Chiro.Kip.Data;
using System.Linq;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Kip.Services
{
	public partial class SyncPersoonService
	{
		/// <summary>
		/// Bestelt dubbelpunt voor de gegeven persoon in het gegeven groepswerkjaar, gegeven dat de persoon
		/// een AD-nummer heeft
		/// </summary>
		/// <param name="adNummer">AD-nummer van persoon die Dubbelpunt wil</param>
		/// <param name="stamNummer">Groep die Dubbelpunt betaalt</param>
		/// <param name="werkJaar">Werkjaar waarvoor Dubbelpuntabonnement</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DubbelpuntBestellen(int adNummer, string stamNummer, int werkJaar)
		{
			ChiroGroep groep = null;
			string feedback = String.Empty;
			using (var db = new kipadminEntities())
			{
				// Heeft de persoon toevallig al een abonnement voor het gegeven werkjaar?

				var abonnement = (from ab in db.Abonnement
						  where ab.kipPersoon.AdNummer == adNummer && ab.werkjaar == werkJaar
						  select ab).FirstOrDefault();

				if (abonnement == null)
				{
					// We doen enkel verder als er nog geen abonnement is.

					// Haal groep en persoon op.

					groep = (from g in db.Groep.OfType<ChiroGroep>()
						 where g.STAMNR == stamNummer
						 select g).FirstOrDefault();

					var persoon = (from p in db.PersoonSet
						       where p.AdNummer == adNummer
						       select p).FirstOrDefault();

					if (persoon == null)
					{
						_log.FoutLoggen(groep.GroepID, String.Format(
						    "Dubbelpunt voor onbestaand ad-nummer {0}", adNummer));
						return;
					}

					// Bestaat er al een niet-doorgeboekte rekening voor Dubbelpunt voor het gegeven
					// groepswerkjaar?

					var rekening = (from f in db.RekeningSet
							where f.WERKJAAR == werkJaar && f.REK_BRON == "DP" && f.DOORGEBOE == "N"
							      && f.STAMNR == stamNummer
							select f).FirstOrDefault();

					if (rekening == null)
					{
						// Nog geen rekening; maak er een nieuwe
						rekening = new Rekening
						{
							WERKJAAR = (short)werkJaar,
							TYPE = "F",
							REK_BRON = "DP",
							STAMNR = stamNummer,
							VERWIJSNR = 0,
							FACTUUR = "N",
							FACTUUR2 = "N",
							DOORGEBOE = "N",
							DAT_REK = DateTime.Now,
							STEMPEL = DateTime.Now
						};
						db.AddToRekeningSet(rekening);
					}

					abonnement = new Abonnement
					{
						werkjaar = werkJaar,
						UITG_CODE = "DP",
						Groep = groep,
						GRATIS = "N",
						REKENING = rekening,
						AANVR_DAT = DateTime.Now,
						STEMPEL = DateTime.Now,
						kipPersoon = persoon,
						EXEMPLAAR = 1,
						AANT_EXEM = 1,
						BESTELD1 = "J",
						BESTELD2 = "J",
						BESTELD3 = "J",
						BESTELD4 = "J",
						BESTELD5 = "J",
						BESTELD6 = "J",
						BESTELD7 = "J",
						BESTELD8 = "J",
						BESTELD9 = "J",
						BESTELD10 = "J",
						BESTELD11 = "J",
						BESTELD12 = "J",
						BESTELD13 = "J",
						BESTELD14 = "J",
						BESTELD15 = "J"
					};

					db.AddToAbonnement(abonnement);
					db.SaveChanges();

					feedback = String.Format(
						"Dubbelpuntabonnement voor {0} {1} AD{2}, rekening {3}",
						persoon.VoorNaam, persoon.Naam, persoon.AdNummer, rekening.NR);
				}
			}
			_log.BerichtLoggen((groep == null ? 0 : groep.GroepID), feedback);
		}

		/// <summary>
		/// Bestelt dubbelpunt voor een 'onbekende' persoon in het gegeven groepswerkjaar
		/// </summary>
		/// <param name="details">details voor de persoon die Dubbelpunt wil bestellen</param>
		/// <param name="stamNummer">Groep die Dubbelpunt betaalt</param>
		/// <param name="werkJaar">Werkjaar waarvoor Dubbelpuntabonnement</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DubbelpuntBestellenNieuwePersoon(PersoonDetails details, string stamNummer, int werkJaar)
		{
			// Als het AD-nummer al gekend is, moet (gewoon) 'LidBewaren' gebruikt worden.
			Debug.Assert(details.Persoon.AdNummer == null);

			if (String.IsNullOrEmpty(details.Persoon.VoorNaam))
			{
				_log.FoutLoggen(0, String.Format(
					"Persoon zonder voornaam niet geupdatet: {0}",
					details.Persoon.Naam));
				return;
			}

			int adnr = UpdatenOfMaken(details);
			DubbelpuntBestellen(adnr, stamNummer, werkJaar);
		}

	}
}
