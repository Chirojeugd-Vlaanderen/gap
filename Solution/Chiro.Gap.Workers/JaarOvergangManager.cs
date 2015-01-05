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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

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
        /// Maakt voor de geven <paramref name="groep"/> een nieuw werkJaar aan
        /// en maakt daarin de opgegeven afdelingen aan, met hun respectieve leeftijdsgrenzen (geboortejaren).
        /// </summary>
        /// <param name="teActiveren">
        /// De afdelingen die geactiveerd moeten worden, met ingestelde geboortejaren 
        /// </param>
        /// <param name="groep">
        /// Groep die de jaarovergang uitvoert
        /// </param>
        /// <param name="officieleAfdelingenRepo">Repository om de officiele afdelingen op te halen.</param>
        /// <exception cref="GapException">
        /// Komt voor wanneer de jaarvergang te vroeg gebeurt.
        /// </exception>
        /// <exception cref="FoutNummerException">
        /// Komt voor als er een afdeling bij zit die niet gekend is in de groep, of als er een afdeling gekoppeld is
        /// aan een onbestaande nationale afdeling. Ook validatiefouten worden op deze manier doorgegeven.
        /// </exception>
        /// <remarks>We persisteren hier niets</remarks>
        public void JaarOvergangUitvoeren(IList<AfdelingsJaarDetail> teActiveren, Groep groep, IRepository<OfficieleAfdeling> officieleAfdelingenRepo)
        {
            // TODO: Het gebruik van AfdelingsJaarDetail (een datacontract) is hier vrij raar.
            // En ook die toegang tot de officiele afdelingen via een repository is lelijk.
            // Ik denk dat er hier te veel in de workers staat. Te herbekijken allemaal.

            var voriggwj = groep.GroepsWerkJaar.OrderByDescending(grwj => grwj.WerkJaar).First();

            if (!_groepsWerkJaarManager.OvergangMogelijk(DateTime.Today, voriggwj.WerkJaar))
            {
                throw new GapException(Resources.JaarovergangTeVroeg);
            }

            var gwj = _groepsWerkJaarManager.VolgendGroepsWerkJaarMaken(groep);

            // Alle gevraagde afdelingen aanmaken en opslaan
            foreach (var afdelingsJaarDetail in teActiveren)
            {
                // Als er afdelingsgegevens meegeleverd zijn, dan moet groep een chirogroep zijn.
                Debug.Assert(groep is ChiroGroep);

                // Zoek de afdeling van de groep met het gevraagde ID
                var afd = (from a in ((ChiroGroep)groep).Afdeling
                           where afdelingsJaarDetail.AfdelingID == a.ID
                           select a).FirstOrDefault();

                if (afd == null)
                {
                    throw new FoutNummerException(FoutNummer.ValidatieFout,
                                                  Resources.OngeldigeAfdelingBinnenGroep);
                }

                // Zoek de officiële afdeling dat overeenkomt met de geselecteerde ID
                var offafd = officieleAfdelingenRepo.ByID(afdelingsJaarDetail.OfficieleAfdelingID);


                if (offafd == null)
                {
                    throw new FoutNummerException(FoutNummer.ValidatieFout,
                                                  Resources.OngeldigeAfdelingNationaal);
                }

                // Maak het afdelingsjaar aan en voegt het toe aan het nieuwe groepswerkjaar 
                // Door dat te bewaren, bewaren we ook de afdelingsjaren, dus hoeft dat hier niet
                try
                {
                    _afdelingsJaarMgr.Aanmaken(afd,
                                               offafd,
                                               gwj,
                                               afdelingsJaarDetail.GeboorteJaarVan,
                                               afdelingsJaarDetail.GeboorteJaarTot,
                                               afdelingsJaarDetail.Geslacht);
                }
                catch (FoutNummerException ex)
                {
                    throw new FoutNummerException(FoutNummer.ValidatieFout,
                                                  string.Format("Fout voor {0}: {1}", afd.Naam, ex.Message));
                }
            }
            // gwj is nu meteen gekoppeld aan de afdelingsjaren, en vice versa.
        }
    }
}