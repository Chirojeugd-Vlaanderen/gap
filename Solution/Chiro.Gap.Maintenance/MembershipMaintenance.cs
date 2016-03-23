/*
 * Copyright 2015, 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
        private readonly ILedenManager _ledenManager;

        // Synchronisatie
        private readonly IPersonenSync _personenSync;

        public MembershipMaintenance(
            IRepositoryProvider repositoryProvider, 
            IGroepsWerkJarenManager groepsWerkJarenManager,
            ILedenManager ledenManager,
            IPersonenSync personenSync)
        {
            _repositoryProvider = repositoryProvider;
            _ledenRepo = _repositoryProvider.RepositoryGet<Lid>();
            _groepsWerkJarenManager = groepsWerkJarenManager;
            _ledenManager = ledenManager;
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
            int overgezetTeller = 0;
            int totaalTeller = 0;

            // TODO: Het bepalen van de aan te vragen memberships, is eigenlijk business logic,
            // en moet in de workers. Maar soit.

            // Zoek eerst de leden waarvan IsAangesloten nog niet gezet is.
            var nietAangeslotenLeden = (from l in _ledenRepo.Select("GelieerdePersoon.Persoon.PersoonsVerzekering", "GroepsWerkJaar")
                where
                    // maak memberships voor niet-aangesloten leden
                    !l.IsAangesloten && 
                    // maak enkel memberships voor huidig werkjaar
                    l.GroepsWerkJaar.WerkJaar == huidigWerkJaar &&
                    // actieve leden waarvan de instapperiode voorbij is
                    l.EindeInstapPeriode < vandaag && !l.NonActief &&
                    // enkel als de groep nog actief was wanneer instapperiode verviel (#4528)
                    (l.GroepsWerkJaar.Groep.StopDatum == null || l.GroepsWerkJaar.Groep.StopDatum > l.EindeInstapPeriode)
                select l).ToArray();

            // Overloop de gevonden leden, en kijk na in hoeverre ze naar de Civi moeten.
            // TODO: Die loop is misschien overkill. We zouden ook de leden die niet iedere keer opnieuw
            // bekeken moeten worden kunnen markeren met IsAangesloten = true.
            // TODO: Fix issue #4966
            foreach (var lid in nietAangeslotenLeden)
            {
                // TODO: We kunnen deze loop ook verwijderen door in UpdateAPI voor alle relevante leden
                // 'is_aangesloten = 1' te zetten. Dat wil zeggen: als er iemand betalend aangesloten wordt,
                // dan zijn alle leden van de persoon voor dat werkjaar aangesloten. Wordt iemand
                // niet-betalend aangesloten, dan worden alle andere niet-betalende leden van die persoon
                // mee aangesloten. Dan blijft uiteraard #4966 nog te fixen.

                ++totaalTeller;
                if (_ledenManager.IsBetalendAangesloten(lid))
                {
                    // Als er al betaald is voor het membership, dan gaat het membership
                    // niet opnieuw naar Civi, zodat in het membership de aanvrager dezelfde
                    // blijft als de betaler.
                    continue;
                }
                if (_ledenManager.GratisAansluiting(lid) && _ledenManager.IsAangesloten(lid))
                {
                    // Als deze aansluiting gratis is, en het lid is al ergens anders aangesloten,
                    // dan moeten we niets meer doen.
                    continue;
                }
                _personenSync.MembershipRegistreren(lid);
                Console.Write("{0} ", ++overgezetTeller);
            }

            Console.WriteLine("{1} leden nagekeken, {0} memberships gesynct.", overgezetTeller, totaalTeller);
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
