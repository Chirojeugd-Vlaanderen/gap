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
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Klasse die de synchronisatie van persoonsgegevens naar Kipadmin regelt
	/// </summary>
	public class PersonenSync : BaseSync, IPersonenSync
	{
	    private readonly IGroepsWerkJarenManager _groepsWerkJarenManager;
	    private readonly ILedenManager _ledenManager;

	    /// <summary>
	    /// Constructor.
	    /// 
	    /// De ServiceHelper wordt geïnjecteerd door de dependency injection container. Wat de
	    /// ServiceHelper precies zal opleveren, hangt af van welke IChannelProvider geregistreerd
	    /// is bij de container.
	    /// </summary>
	    /// <param name="serviceHelper">ServiceHelper, nodig voor service calls.</param>
	    /// <param name="groepsWerkJarenManager">GroepsWerkJarenManager; wordt gebruikt om te kijken of iemand
	    /// verzekerd is tegen loonverlies.</param>
	    /// <param name="ledenManager">Ledenlogica.</param>
	    public PersonenSync(ServiceHelper serviceHelper, IGroepsWerkJarenManager groepsWerkJarenManager, ILedenManager ledenManager)
	        : base(serviceHelper)
	    {
	        _groepsWerkJarenManager = groepsWerkJarenManager;
	        _ledenManager = ledenManager;
	    }

        /// <summary>
        /// Probeert de gegeven gelieerde persoon in de Chirocivi te vinden, en updatet
        /// hem als dat lukt. Wordt de persoon niet gevonden, dan wordt er een
        /// nieuwe aangemaakt.
        /// </summary>
        /// <param name="gp">Te bewaren gelieerde persoon</param>
	    public void UpdatenOfMaken(GelieerdePersoon gp)
        {
            Debug.Assert(gp.Persoon.InSync);

            var details = MappingHelper.Map<GelieerdePersoon, PersoonDetails>(gp);
            ServiceHelper.CallService<ISyncPersoonService>(svc => svc.PersoonUpdatenOfMaken(details));
        }

	    /// <summary>
	    /// Updatet een bestaand persoon in Kipadmin: persoonsgegevens, en eventueel ook adressen en/of communicatie.
	    /// 
	    /// </summary>
	    /// <param name="gp">Gelieerde persoon, persoonsinfo</param>
	    /// <remarks>Enkel voor het wijzigen van bestaande personen!</remarks>
	    public void Updaten(GelieerdePersoon gp)
		{
			// Wijzigingen van personen met ad-nummer worden doorgesluisd
			// naar Kipadmin.

			Debug.Assert(gp.Persoon.InSync);

            var syncPersoon = MappingHelper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(gp.Persoon);
			ServiceHelper.CallService<ISyncPersoonService>(svc => svc.PersoonUpdaten(syncPersoon));
		}

        /// <summary>
        /// Registreert een membership in de Chirocivi voor het gegeven <paramref name="lid"/>
        /// </summary>
        /// <param name="lid">Lid waarvoor membership geregistreerd moet worden</param>
	    public void MembershipRegistreren(Lid lid)
        {
            var persoon = lid.GelieerdePersoon.Persoon;
            int werkJaar = lid.GroepsWerkJaar.WerkJaar;
            if (persoon.AdNummer != null)
            {
                var gedoe = new MembershipGedoe
                {
                    Gratis = _ledenManager.GratisAansluiting(lid),
                    MetLoonVerlies = _groepsWerkJarenManager.IsVerzekerd(lid, Verzekering.LoonVerlies),
                    StamNummer = _ledenManager.StamNummer(lid)
                };
                ServiceHelper.CallService<ISyncPersoonService>(
                    svc => svc.MembershipBewaren(persoon.AdNummer.Value, werkJaar, gedoe));
            }
            else
            {
                var gelieerdePersoon = lid.GelieerdePersoon;

                var details = MappingHelper.Map<GelieerdePersoon, PersoonDetails>(gelieerdePersoon);
                bool isVerzekerd = _groepsWerkJarenManager.IsVerzekerd(lid, Verzekering.LoonVerlies);

                var gedoe = new MembershipGedoe
                {
                    Gratis = _ledenManager.GratisAansluiting(lid),
                    MetLoonVerlies = isVerzekerd,
                    StamNummer = _ledenManager.StamNummer(lid)
                };
                ServiceHelper.CallService<ISyncPersoonService>(
                    svc => svc.MembershipNieuwePersoonBewaren(details, werkJaar, gedoe));
            }
	    }
	}
}
