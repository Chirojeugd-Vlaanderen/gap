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
﻿using Chiro.Kip.Data;
using System.Linq;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Kip.Services
{
	public partial class SyncPersoonService
	{
		/// <summary>
		/// Verzekert de persoon met AD-nummer <paramref name="adNummer"/> tegen loonverlies voor werkjaar
		/// <paramref name="werkJaar"/>.  De groep met stamnummer <paramref name="stamNummer"/> betaalt.
		/// </summary>
		/// <param name="adNummer">AD-nummer van te verzekeren persoon</param>
		/// <param name="stamNummer">Stamnummer van betalende groep</param>
		/// <param name="werkJaar">Werkjaar waarin te verzekeren voor loonverlies</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void LoonVerliesVerzekeren(int adNummer, string stamNummer, int werkJaar)
		{
			ChiroGroep groep = null;
			string feedback = String.Empty;
			using (var db = new kipadminEntities())
			{
				// Haal groep op.

				groep = (from g in db.Groep.OfType<ChiroGroep>()
					 where g.STAMNR == stamNummer
					 select g).FirstOrDefault();

				// Nakijken of de persoon in kwestie nog niet verzekerd is.

				var bestaande = (from v in db.PersoonsVerzekering
						 where v.kipPersoon.AdNummer == adNummer && v.ExtraVerzekering.WerkJaar == werkJaar
						 select v).FirstOrDefault();

				if (bestaande != null)
				{
					_log.BerichtLoggen(groep.GroepID, String.Format(
						"Dubbele verzekering loonverlies van persoon genegeerd.  Ad-nr {0}",
						adNummer));
					return;
				}

				// Opzoeken persoon

				var persoon = (from p in db.PersoonSet
					       where p.AdNummer == adNummer
					       select p).FirstOrDefault();

				if (persoon == null)
				{
					_log.FoutLoggen(
						groep == null ? 0 : groep.GroepID,
						String.Format(
							"genegeerd: loonverlies onbekend AD-nr. {0}",
							adNummer));
					return;
				}

				// Zoek eerst naar een geschikte lijn in ExtraVerzekering die we
				// kunnen recupereren voor deze verzekering.

				var extraVerzDitWerkjaar = (from v in db.ExtraVerzekering.Include("REKENING")
							    where v.WerkJaar == werkJaar
								  && v.Groep.GroepID == groep.GroepID
							    select v).OrderByDescending(vv => vv.VolgNummer);

				// Laatste opzoeken

				var verzekering = extraVerzDitWerkjaar.FirstOrDefault();

				// Als de laatste verzekering een niet-doorgeboekte rekening heeft,
				// komt deze persoon er gewoon bij.  Is er nog geen laatste verzekering,
				// of is de rekening al wel doorgeboekt, dan maken we er een nieuwe.

				if (verzekering == null || (verzekering.REKENING != null && verzekering.REKENING.DOORGEBOE != "N"))
				{
					int volgNummer = (verzekering == null ? 1 : verzekering.VolgNummer + 1);
					// Bedragen rekening mogen leeg zijn; worden aangevuld bij factuur
					// overzetten in Kipadmin.

					// Kaderploegen krijgen geen factuur

					var rekening = (groep.TYPE.ToUpper() != "L")
							? null
							: new Rekening
							{
								WERKJAAR = (short)werkJaar,
								TYPE = "F",
								REK_BRON = "U_VERZEK",
								STAMNR = stamNummer,
								VERWIJSNR = volgNummer,
								FACTUUR = "N",
								FACTUUR2 = "N",
								DOORGEBOE = "N",
								DAT_REK = DateTime.Now
							};

					verzekering = new ExtraVerzekering
					{
						Datum = DateTime.Now,
						DoodInvaliditeit = null,
						ExtraVerzekeringID = 0,
						Groep = groep,
						LoonVerlies = 0,
						Noot = String.Empty,
						REKENING = rekening,
						VolgNummer = volgNummer,
						WerkJaar = werkJaar
					};

					if (rekening != null)
					{
						db.AddToRekeningSet(rekening);
					}
					db.AddToExtraVerzekering(verzekering);
				}

				// verzekering bevat nu het verzekeringsrecord waarbij de nieuwe
				// persoon opgeteld kan worden.

				// In de 'noot' van het verzekeringsrecord bewaren we
				// comma-separated de AD-nummers van verzekerde personen.
				// Dit is 'legacy'; we connecteren het verzekeringsrecord nu met
				// de verzekerde persoon.

				var persoonsVerzekering = new PersoonsVerzekering
				{
					ExtraVerzekering = verzekering,
					kipPersoon = persoon
				};

				db.AddToPersoonsVerzekering(persoonsVerzekering);

				verzekering.Noot = String.Format(
					"{0},{1}",
					verzekering.Noot,
					adNummer);
				verzekering.Stempel = DateTime.Now;
				verzekering.Wijze = "G";
				++verzekering.LoonVerlies;

				db.SaveChanges();

				feedback = String.Format(
					"Persoon met AD-nr. {0} verzekerd tegen loonverlies voor {1} in {2}",
					adNummer,
					stamNummer,
					werkJaar);
			}
			_log.BerichtLoggen(groep == null ? 0 : groep.GroepID, feedback);
		}

		/// <summary>
		/// Verzekert een persoon zonder AD-nummer tegen loonverlies voor werkjaar
		/// <paramref name="werkJaar"/>.  De groep met stamnummer <paramref name="stamNummer"/> betaalt.
		/// </summary>
		/// <param name="details">details van te verzekeren persoon</param>
		/// <param name="stamNummer">Stamnummer van betalende groep</param>
		/// <param name="werkJaar">Werkjaar waarin te verzekeren voor loonverlies</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void LoonVerliesVerzekerenAdOnbekend(PersoonDetails details, string stamNummer, int werkJaar)
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
			LoonVerliesVerzekeren(adnr, stamNummer, werkJaar);
		}
	}
}
