// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

#if KIPDORP
using System.Transactions;
#endif
using System;
using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. de jaarovergang bevat
    /// </summary>
    public class JaarOvergangManager : IJaarOvergangManager
    {
        private readonly IGroepenManager _groepenMgr;
		private readonly IChiroGroepenManager _chiroGroepenMgr;
        private readonly IAfdelingsJaarManager _afdelingsJaarMgr;
		private readonly IGroepsWerkJarenManager _groepsWerkJaarManager;

    	/// <summary>
    	/// Maakt een nieuwe jaarovergangsmanager aan
    	/// </summary>
    	/// <param name="gm">
    	/// 	De worker voor Groepen
    	/// </param>
    	/// <param name="cgm">
    	/// 	De worker voor Chirogroepen
    	/// </param>
    	/// <param name="ajm">
    	/// 	De worker voor AfdelingsJaren
    	/// </param>
    	/// <param name="wm">
    	/// 	De worker voor GroepsWerkJaren
    	/// </param>
		public JaarOvergangManager(IGroepenManager gm, IChiroGroepenManager cgm, IAfdelingsJaarManager ajm, IGroepsWerkJarenManager wm)
        {
            // TODO (#1095): visie ontwikkelen over wanneer we IoC toepassen
            _groepenMgr = gm;
            _chiroGroepenMgr = cgm;
            _afdelingsJaarMgr = ajm;
            _groepsWerkJaarManager = wm;
        }

        /// <summary>
        /// Maakt voor de groep met de opgegeven <paramref name="groepID"/> een nieuw werkJaar aan
        /// en maakt daarin de opgegeven afdelingen aan, met hun respectieve leeftijdsgrenzen (geboortejaren).
        /// </summary>
        /// <param name="teActiveren">
        /// De afdelingen die geactiveerd moeten worden, met ingestelde geboortejaren 
        /// </param>
        /// <param name="groepID">
        /// De ID van de groep die de jaarovergang uitvoert
        /// </param>
        /// <exception cref="GapException">
        /// Komt voor wanneer de jaarvergang te vroeg gebeurt.
        /// </exception>
        /// <exception cref="FoutNummerException">
        /// Komt voor als er een afdeling bij zit die niet gekend is in de groep, of als er een afdeling gekoppeld is
        /// aan een onbestaande nationale afdeling. Ook validatiefouten worden op deze manier doorgegeven.
        /// </exception>
        /// <remarks>Er worden geen leden gemaakt door deze method.</remarks>
        public void JaarOvergangUitvoeren(IEnumerable<AfdelingsJaarDetail> teActiveren, int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
//            // TODO (#1135): internal datacontract maken

//            var voriggwj = _groepsWerkJaarManager.RecentsteOphalen(groepID);

//            if (!_groepsWerkJaarManager.OvergangMogelijk(DateTime.Today, voriggwj.WerkJaar))
//            {
//                throw new GapException(Resources.JaarovergangTeVroeg);
//            }

//            // TODO check dat roll-back gebeurt
//            // TODO check of er meer voorwaarden gecontroleerd moeten worden
//#if KIPDORP
//    // We proberen eens met een hogere timeout. (5 minuten ipv standaard 1)
//    // (refs #866)

//                using (var scope = new TransactionScope(
//                    TransactionScopeOption.Required, 
//                    new TimeSpan(0, 0, 5, 0)))
//                {
//#endif
//            Groep g;

//            // Groep ophalen.  Als er afdelingen meegegeven zijn, dan gaat het zeker
//            // om een ChiroGroep.  Zonder afdelingen is een object van het type
//            // (abstract) Groep voldoende.

//            // ReSharper disable ConvertIfStatementToConditionalTernaryExpression
//            if (teActiveren.FirstOrDefault() == null)
//            {
//                g = _groepenMgr.Ophalen(groepID, GroepsExtras.GroepsWerkJaren);
//            }
//            else
//            {
//                g = _chiroGroepenMgr.Ophalen(groepID,
//                                             ChiroGroepsExtras.AlleAfdelingen | ChiroGroepsExtras.GroepsWerkJaren);
//            }

//            // ReSharper restore ConvertIfStatementToConditionalTernaryExpression
//            var gwj = _groepsWerkJaarManager.VolgendGroepsWerkJaarMaken(g);

//            // Dit nieuwe groepswerkjaar gaan we nu nog niet bewaren, maar zodadelijk meteen 
//            // met de afdelingsjaren bij.  Op die manier hebben we meteen de juiste koppelingen
//            var offafdelingen = _afdelingsJaarMgr.OfficieleAfdelingenOphalen();

//            // Alle gevraagde afdelingen aanmaken en opslaan
//            foreach (var afdinfo in teActiveren)
//            {
//                // Als er afdelingsgegevens meegeleverd zijn, dan moet g een chirogroep zijn.
//                Debug.Assert(g is ChiroGroep);

//                var afdinfo1 = afdinfo;

//                // Lokale variabele om "Access to modified closure" te vermijden [wiki:VeelVoorkomendeWaarschuwingen#Accesstomodifiedclosure]

//                // Zoek de afdeling van de groep met het gevraagde ID
//                var afd = (from a in ((ChiroGroep) g).Afdeling
//                           where afdinfo1.AfdelingID == a.ID
//                           select a).FirstOrDefault();

//                if (afd == null)
//                {
//                    throw new FoutNummerException(FoutNummer.ValidatieFout, Resources.OngeldigeAfdelingBinnenGroep);
//                }

//                // Zoek de officiële afdeling dat overeenkomt met de geselecteerde ID
//                var offafd = (from a in offafdelingen
//                              where afdinfo1.OfficieleAfdelingID == a.ID
//                              select a).FirstOrDefault();

//                if (offafd == null)
//                {
//                    throw new FoutNummerException(FoutNummer.ValidatieFout, Resources.OngeldigeAfdelingNationaal);
//                }

//                // Maak het afdelingsjaar aan en voegt het toe aan het nieuwe groepswerkjaar 
//                // Door dat te bewaren, bewaren we ook de afdelingsjaren, dus hoeft dat hier niet
//                try
//                {
//                    _afdelingsJaarMgr.Aanmaken(afd,
//                                               offafd,
//                                               gwj,
//                                               afdinfo.GeboorteJaarVan,
//                                               afdinfo.GeboorteJaarTot,
//                                               afdinfo.Geslacht);
//                }
//                catch (ValidatieException ex)
//                {
//                    throw new FoutNummerException(FoutNummer.ValidatieFout,
//                                                  string.Format("Fout voor {0}: {1}", afd.Naam, ex.Message));
//                }
//            }

//            // Bewaar nu 'in 1 trek'  meteen groepswerkjaar *en* afdelingsjaren.
//            _groepsWerkJaarManager.Bewaren(gwj, GroepsWerkJaarExtras.Groep | GroepsWerkJaarExtras.Afdelingen);

//            // gwj is nu meteen gekoppeld aan de afdelingsjaren, en vice versa.
//#if KIPDORP
//                    scope.Complete();
//                }
//#endif
        }
    }
}