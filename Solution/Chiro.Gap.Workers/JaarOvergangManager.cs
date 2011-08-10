// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#if KIPDORP
using System.Transactions;
#endif
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. de jaarovergang bevat
    /// </summary>
    public class JaarOvergangManager
    {
		private readonly GroepenManager _groepenMgr;
		private readonly ChiroGroepenManager _chiroGroepenMgr;
		private readonly AfdelingsJaarManager _afdelingsJaarMgr;
		private readonly GroepsWerkJaarManager _groepsWerkJaarManager;
		private readonly LedenManager _ledenMgr;

        /// <summary>
        /// Maakt een nieuwe jaarovergangsmanager aan
        /// </summary>
		/// <param name="gm">De worker voor Groepen</param>
		/// <param name="cgm">De worker voor Chirogroepen</param>
		/// <param name="ajm">De worker voor AfdelingsJaren</param>
		/// <param name="wm">De worker voor GroepsWerkJaren</param>
		/// <param name="lm">De worker voor Leden</param>
        public JaarOvergangManager(
			GroepenManager gm,
			ChiroGroepenManager cgm,
			AfdelingsJaarManager ajm,
			GroepsWerkJaarManager wm,
			LedenManager lm)
		{
			_groepenMgr = gm;
			_chiroGroepenMgr = cgm;
			_afdelingsJaarMgr = ajm;
			_groepsWerkJaarManager = wm;
			_ledenMgr = lm;
		}

		// TODO internal datacontract maken
		public void JaarOvergangUitvoeren(IEnumerable<TeActiverenAfdelingInfo> teActiveren, int groepID)
		{
			var voriggwj = _groepsWerkJaarManager.RecentsteOphalen(groepID);

			if (DateTime.Today <= _groepsWerkJaarManager.StartOvergang(voriggwj.WerkJaar))
			{
				throw new GapException("De jaarovergang is enkel toegelaten vanaf een vooropgestelde datum.");
			}

			// TODO unit tests
			// TODO check dat roll-back gebeurt
			// TODO check of er meer voorwaarden gecontroleerd moeten worden
#if KIPDORP
                // We proberen eens met een hogere timeout. (5 minuten ipv standaard 1)
                // (refs #866)

                using (var scope = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TimeSpan(0, 0, 5, 0)))
                {
#endif
			Groep g;

			// Groep ophalen.  Als er afdelingen meegegeven zijn, dan gaat het zeker
			// om een ChiroGroep.  Zonder afdelingen is een object van het type
			// (abstract) Groep voldoende.

			// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
			if (teActiveren.FirstOrDefault() == null)
			{
				g = _groepenMgr.Ophalen(groepID, GroepsExtras.GroepsWerkJaren);
			}
			else
			{
				g = _chiroGroepenMgr.Ophalen(groepID, ChiroGroepsExtras.AlleAfdelingen | ChiroGroepsExtras.GroepsWerkJaren);
			}
			// ReSharper restore ConvertIfStatementToConditionalTernaryExpression

			var gwj = _groepsWerkJaarManager.VolgendGroepsWerkJaarMaken(g);
			// Dit nieuwe groepswerkjaar gaan we nu nog niet bewaren, maar zodadelijk meteen 
			// met de afdelingsjaren bij.  Op die manier hebben we meteen de juiste koppelingen

			var offafdelingen = _afdelingsJaarMgr.OfficieleAfdelingenOphalen();

			// Alle gevraagde afdelingen aanmaken en opslaan
			foreach (var afdinfo in teActiveren)
			{
				// Als er afdelingsgegevens meegeleverd zijn, dan moet g een chirogroep zijn.
				Debug.Assert(g is ChiroGroep);

				var afdinfo1 = afdinfo;
				// Lokale variabele om "Access to modified closure" te vermijden [wiki:VeelVoorkomendeWaarschuwingen#Accesstomodifiedclosure]

				// Zoek de afdeling van de groep met het gevraagde ID
				var afd = (from a in (g as ChiroGroep).Afdeling
						   where afdinfo1.AfdelingID == a.ID
						   select a).FirstOrDefault();

				if (afd == null)
				{
					throw new FoutNummerException(FoutNummer.ValidatieFout, "Een van de gevraagde afdelingen is geen afdeling van de gegeven groep.");
				}

				// Zoek de officiële afdeling dat overeenkomt met de geselecteerde ID
				var offafd = (from a in offafdelingen
							  where afdinfo1.OfficieleAfdelingID == a.ID
							  select a).FirstOrDefault();

				if (offafd == null)
				{
					throw new FoutNummerException(FoutNummer.ValidatieFout, "Een van de gevraagde afdelingen is geen bestaande officiële afdeling.");
				}

				// Maak het afdelingsjaar aan en voegt het toe aan het nieuwe groepswerkjaar 
				// Door dat te bewaren, bewaren we ook de afdelingsjaren, dus hoeft dat hier niet
				try
				{
					_afdelingsJaarMgr.Aanmaken(afd,
											  offafd,
											  gwj,
											  afdinfo.GeboorteJaarVan,
											  afdinfo.GeboorteJaarTot,
											  afdinfo.Geslacht);
				}
				catch (ValidatieException ex)
				{
					throw new FoutNummerException(FoutNummer.ValidatieFout, String.Format("Fout voor {0}: {1}", afd.Naam, ex.Message));
				}
			}

			// Bewaar nu 'in 1 trek'  meteen groepswerkjaar *en* afdelingsjaren.
			_groepsWerkJaarManager.Bewaren(gwj, GroepsWerkJaarExtras.Groep | GroepsWerkJaarExtras.Afdelingen);
			// gwj is nu meteen gekoppeld aan de afdelngsjaren, en vice versa.

#if KIPDORP
                    scope.Complete();
                }
#endif
		}
    }
}
