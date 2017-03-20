/*
 * Copyright 2008-2013, 2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
using System.Linq;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Deze klasse staat in voor het overzetten van verzekeringsgegevens naar Kipadmin.
    /// </summary>
    public class VerzekeringenSync : BaseSync, IVerzekeringenSync
    {
        private readonly ILedenManager _ledenManager;

        /// <summary>
        /// Constructor.
        /// 
        /// De ServiceHelper wordt geïnjecteerd door de dependency injection container. Wat de
        /// ServiceHelper precies zal opleveren, hangt af van welke IChannelProvider geregistreerd
        /// is bij de container.
        /// </summary>
        /// <param name="serviceHelper">ServiceHelper, nodig voor service calls.</param>
        /// <param name="ledenManager">Wat ledenlogica.</param>
        public VerzekeringenSync(ServiceHelper serviceHelper, ILedenManager ledenManager) : base(serviceHelper)
        {
            _ledenManager = ledenManager;
        }

        /// <summary>
        /// Zet de gegeven <paramref name="persoonsVerzekering"/> over naar Kipadmin.
        /// </summary>
        /// <param name="persoonsVerzekering">Over te zetten persoonsverzekering</param>
        /// <param name="gwj">Bepaalt werkJaar en groep die factuur zal krijgen</param>
        public void Bewaren(PersoonsVerzekering persoonsVerzekering, GroepsWerkJaar gwj)
        {
            Debug.Assert(persoonsVerzekering.Persoon.InSync);
            if (persoonsVerzekering.Persoon.AdNummer.HasValue)
            {
                bool gratis = persoonsVerzekering.Persoon.GelieerdePersoon.Any(
                    gp => gp.Lid.Any(l => l.GroepsWerkJaar.ID == gwj.ID && _ledenManager.GratisAansluiting(l)));
                ServiceHelper.CallService<ISyncPersoonService>(svc => svc.LoonVerliesVerzekeren(
                    persoonsVerzekering.Persoon.AdNummer.Value,
                    gwj.Groep.Code,
                    gwj.WerkJaar,
                    gratis));
            }
            else
            {
                // Verzekeren op basis van details.
                // Een verzekering loonverlies (de enige die we voorlopig ondersteunen) is
                // alleen mogelijk voor leden. Sinds gap 1.x ergens in het verleden, krijgen
                // probeerleden ook al een AD-nr. Dus mogen we ervanuit gaan dat het AD-nummer
                // al in aanvraag is.
                // Niet dat dat nog relevant is eigenlijk.

                var gelieerdePersoon = (from gp in persoonsVerzekering.Persoon.GelieerdePersoon
                                        where Equals(gp.Groep, gwj.Groep)
                                        select gp).Single();

                bool gratis =
                    gelieerdePersoon.Lid.Any(l => l.GroepsWerkJaar.ID == gwj.ID && _ledenManager.GratisAansluiting(l));

                ServiceHelper.CallService<ISyncPersoonService>(
                    svc =>
                    svc.LoonVerliesVerzekerenAdOnbekend(MappingHelper.Map<GelieerdePersoon, PersoonDetails>(gelieerdePersoon),
                                                        gwj.Groep.Code, gwj.WerkJaar, gratis));
            }
        }
    }
}
