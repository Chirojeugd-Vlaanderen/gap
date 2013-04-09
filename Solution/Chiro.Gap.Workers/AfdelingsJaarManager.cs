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
using System.Linq.Expressions;
using System.Transactions;

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
	/// <summary>
    /// Worker die alle businesslogica i.v.m. afdelingsjaren bevat
    /// </summary>
    public class AfdelingsJaarManager : IAfdelingsJaarManager
	{
        private readonly ILedenSync _ledenSync;

        private readonly IVeelGebruikt _veelGebruikt;
        private readonly IAutorisatieManager _autorisatieMgr;

        public AfdelingsJaarManager(
            IVeelGebruikt veelGebruikt,
            IAutorisatieManager autorisatieMgr,
            ILedenSync ledenSync)
        {
            _veelGebruikt = veelGebruikt;
            _autorisatieMgr = autorisatieMgr;
            _ledenSync = ledenSync;
        }

        /// <summary>
        /// Maakt een afdelingsjaar voor een groep en een afdeling, persisteert niet.
        /// </summary>
        /// <param name="a">
        /// Afdeling voor nieuw afdelingsjaar
        /// </param>
        /// <param name="oa">
        /// Te koppelen officiële afdeling
        /// </param>
        /// <param name="gwj">
        /// Groepswerkjaar (koppelt de afdeling aan een groep en een werkJaar)
        /// </param>
        /// <param name="geboorteJaarBegin">
        /// Geboortejaar van
        /// </param>
        /// <param name="geboorteJaarEind">
        /// Geboortejaar tot
        /// </param>
        /// <param name="geslacht">
        /// Bepaalt of de afdeling een jongensafdeling, meisjesafdeling of
        /// gemengde afdeling is.
        /// </param>
        /// <returns>
        /// Het aangemaakte afdelingsjaar
        /// </returns>
        public AfdelingsJaar Aanmaken(
            Afdeling a,
            OfficieleAfdeling oa,
            GroepsWerkJaar gwj,
            int geboorteJaarBegin,
            int geboorteJaarEind,
            GeslachtsType geslacht)
        {
            if (!_autorisatieMgr.IsGav(a) || !_autorisatieMgr.IsGav(gwj))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            if (gwj.Groep.ID != a.ChiroGroep.ID)
            {
                throw new FoutNummerException(
                    FoutNummer.AfdelingNietVanGroep,
                    Resources.AfdelingNietVanGroep);
            }

            // Leden moeten minstens in het 1ste leerjaar zitten voor we ze inschrijven.
            // De maximumleeftijd is arbitrair nattevingerwerk. :-)
            if (!(gwj.WerkJaar - geboorteJaarEind >= Settings.Default.MinLidLeefTijd)
                || !(gwj.WerkJaar - geboorteJaarBegin <= Settings.Default.MaxLidLeefTijd)
                || !(geboorteJaarBegin <= geboorteJaarEind))
            {
                throw new ValidatieException(Resources.OngeldigeGeboortejarenVoorAfdeling,
                                             FoutNummer.FouteGeboortejarenVoorAfdeling);
            }

            var afdelingsJaar = new AfdelingsJaar
                                    {
                                        OfficieleAfdeling = oa,
                                        Afdeling = a,
                                        GroepsWerkJaar = gwj,
                                        GeboorteJaarVan = geboorteJaarBegin,
                                        GeboorteJaarTot = geboorteJaarEind,
                                        Geslacht = geslacht
                                    };

            // TODO check if no conflicts with existing afdelingsjaar
            a.AfdelingsJaar.Add(afdelingsJaar);
            oa.AfdelingsJaar.Add(afdelingsJaar);
            gwj.AfdelingsJaar.Add(afdelingsJaar);

            return afdelingsJaar;
        }
    }
}