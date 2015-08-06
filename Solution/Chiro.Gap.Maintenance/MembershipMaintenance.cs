/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using System.Linq;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Maintenance
{
    /// <summary>
    /// Deze klasse is voornamelijk verantwoordelijk voor het aanmaken van
    /// memberships in de civi als de probeerperiode voorbij is.
    /// </summary>
    public class MembershipMaintenance: IDisposable
    {
        // Data access
        private readonly IRepositoryProvider _repositoryProvider;
        private readonly IRepository<Lid> _ledenRepo;

        // Businesslogica
        private readonly IGroepsWerkJarenManager _groepsWerkJarenManager;

        // Synchronisatie
        private readonly IPersonenSync _personenSync;

        public MembershipMaintenance(IRepositoryProvider repositoryProvider, IGroepsWerkJarenManager groepsWerkJarenManager,
            IPersonenSync personenSync)
        {
            _repositoryProvider = repositoryProvider;
            _ledenRepo = _repositoryProvider.RepositoryGet<Lid>();
            _groepsWerkJarenManager = groepsWerkJarenManager;
            _personenSync = personenSync;
        }

        /// <summary>
        /// Stuurt een membership naar CiviCRM voor de leden waarvan de probeerperiode
        /// voorbij is, maar die nog geen membership hadden dit jaar
        /// </summary>
        /// <remarks>
        /// We doen dit enkel voor leden uit het huidige werkjaar.
        /// </remarks>
        public void MembershipsMaken()
        {
            int huidigWerkJaar = _groepsWerkJarenManager.HuidigWerkJaarNationaal();
            DateTime vandaag = _groepsWerkJarenManager.Vandaag();

            var teSyncen = (from l in _ledenRepo.Select("GelieerdePersoon.Persoon.PersoonsVerzekering", "GroepsWerkJaar")
                where
                    (l.GelieerdePersoon.Persoon.LaatsteMembership < huidigWerkJaar ||
                     l.GelieerdePersoon.Persoon.LaatsteMembership == null) &&
                    l.GroepsWerkJaar.WerkJaar == huidigWerkJaar &&
                    l.EindeInstapPeriode < vandaag
                select l).Take(Properties.Settings.Default.LimitMembershipQuery).ToArray();

            foreach (var p in teSyncen)
            {
                    _personenSync.MembershipRegistreren(p);
            }
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

        ~MembershipMaintenance()
        {
            Dispose(false);
        }

        #endregion
    }
}
