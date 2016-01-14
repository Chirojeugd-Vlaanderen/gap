/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using System;
using System.Linq;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Maintenance
{
    /// <summary>
    /// Deze klasse controleert of er leden zijn zonder AD-nummer. In theorie kan dit
    /// niet voorkomen. In praktijk kan het zijn dat een AD-nummer fout was. In dat
    /// geval wordt het AD-nummer opnieuw verwijderd. En die gevallen zoeken we hier
    /// op om opnieuw te syncen.
    /// 
    /// (Ik doe dat niet meteen bij het resetten van het AD-nummer; op die manier is
    /// er geen koppeling nodig tussen UpdateApi en CiviSync)
    /// </summary>
    public class RelationshipMaintenance: IDisposable
    {
        // Data access
        private readonly IRepositoryProvider _repositoryProvider;
        private readonly IRepository<Lid> _ledenRepo;

        // Businesslogica
        private readonly IGroepsWerkJarenManager _groepsWerkJarenManager;

        // Synchronisatie
        private readonly ILedenSync _ledenSync;

        /// <summary>
        /// Constructor die dependency injection faciliteert.
        /// </summary>
        /// <param name="repositoryProvider">Toegang tot de database</param>
        /// <param name="groepsWerkJarenManager">Geruiken we vooral om het huidige werkjaar te bepalen.</param>
        /// <param name="ledenSync">Toegang tot CiviSync</param>
        public RelationshipMaintenance(IRepositoryProvider repositoryProvider, IGroepsWerkJarenManager groepsWerkJarenManager,
            ILedenSync ledenSync)
        {
            _repositoryProvider = repositoryProvider;
            _ledenRepo = _repositoryProvider.RepositoryGet<Lid>();
            _groepsWerkJarenManager = groepsWerkJarenManager;
            _ledenSync = ledenSync;
        }

        /// <summary>
        /// Zoekt alle leden op zonder AD-nummer, en synchroniseert deze opnieuw, in de hoop dat
        /// ze nu wel een AD-nummer krijgen.
        /// </summary>
        public void LedenZonderAdOpnieuwSyncen()
        {
            int huidigWerkJaar = _groepsWerkJarenManager.HuidigWerkJaarNationaal();

            var teSyncen = (from l in _ledenRepo.Select("GelieerdePersoon.Persoon", "GroepsWerkJaar")
                where l.GelieerdePersoon.Persoon.AdNummer == null && l.GroepsWerkJaar.WerkJaar == huidigWerkJaar
                select l).ToArray();

            Console.WriteLine("Aanvragen van {0} AD-nummers", teSyncen.Count());

            _ledenSync.Bewaren(teSyncen);
        }

        #region Disposable thingy

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed) return;
            if (disposing)
            {
                // Dispose managed resources.
                _repositoryProvider.Dispose();
            }
            _disposed = true;
        }

        ~RelationshipMaintenance()
        {
            Dispose(false);
        }

        #endregion
    }
}
