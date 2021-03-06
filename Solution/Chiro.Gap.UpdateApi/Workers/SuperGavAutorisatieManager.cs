/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Verfijning gebruikersrechten Copyright 2015 Chirojeugd-Vlaanderen vzw
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
using System.ServiceModel;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.UpdateApi.Workers
{
    /// <summary>
    /// Autorisatiemanager die slechts 2 dingen doet:
    /// 1. Zeggen 'Ik ben super-GAV'.
    /// 2. De gebruikersnaam opleveren
    /// Alle andere zaken zijn niet-geïmplementeerd, en voor de veiligheid
    /// blijven ze dat best ook.
    /// </summary>
    /// <remarks>Deze klasse is een kopie van die in Chiro.Gap.UpdataSvc.Service.  Het lijkt
    /// me zo dom om hiervoor een apart project te maken.  Maar ik weet ook niet
    /// goed waar ik dit anders kwijt moet.  Ik wil dit uit Chiro.Gap.Workers houden,
    /// om te vermijden dat je super-gav kunt worden door gewoon de unity-configuration
    /// aan te passen.  Nu kun je dat enkel als Chiro.Gap.Diagnostics.Service.dll
    /// beschikbaar is.</remarks>
    public class SuperGavAutorisatieManager: IAutorisatieManager
    {

        /// <summary>
        /// Verwijdert uit een lijst van GelieerdePersoonID's de ID's
        /// van GelieerdePersonen voor wie de aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="gelieerdePersonenIDs">Lijst met ID's van gelieerde personen</param>
        /// <returns>Enkel de ID's van die personen voor wie de gebruiker GAV is</returns>
        /// <remarks></remarks>
        public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verwijdert uit een lijst van PersoonID's de ID's
        /// van personen voor wie de aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="personenIDs">Lijst met ID's van personen</param>
        /// <returns>Enkel de ID's van die personen voor wie de gebruiker GAV is</returns>
        /// <remarks></remarks>
        public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Geeft true als de aangelogde user
        /// 'superrechten' heeft
        /// (zoals het verwijderen van leden uit vorig werkJaar, het
        /// verwijderen van leden waarvan de probeerperiode voorbij is,...)
        /// </summary>
        /// <returns><c>True</c> (enkel) als user supergav is</returns>
        /// <remarks></remarks>
        public bool IsSuperGav()
        {
            return true;
        }


        /// <summary>
        /// Bepaalt de gebruikersnaam van de huidig aangemelde gebruiker.
        /// (lege string indien niet van toepassing)
        /// </summary>
        /// <returns>Username aangemelde gebruiker</returns>
        /// <remarks></remarks>
        public string GebruikersNaamGet()
        {
            return ServiceSecurityContext.Current == null ? String.Empty
                : ServiceSecurityContext.Current.WindowsIdentity.Name;
        }
        
        public bool IsGav(Groep groep)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(CommunicatieVorm communicatieVorm)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(GroepsWerkJaar groepsWerkJaar)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(GelieerdePersoon gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Deelnemer gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Plaats gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Uitstap gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(GebruikersRechtV2 gelieerdePersoon)
        {
            throw new NotImplementedException();
        }
        
        public bool IsGav(Lid gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Afdeling gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Categorie gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(IList<GelieerdePersoon> gelieerdePersonen)
        {
            throw new NotImplementedException();
        }

        public List<GelieerdePersoon> MijnGelieerdePersonen(IList<Persoon> personen)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(IList<PersoonsAdres> persoonsAdressen)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(IList<Persoon> personen)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(IList<Groep> groepen)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(IList<Lid> leden)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Abonnement abonnement)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gebruikerses the recht gelieerde persoon.
        /// </summary>
        /// <param name="gelieerdePersoonID">The gelieerde persoon ID.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public GebruikersRechtV2 GebruikersRechtGelieerdePersoon(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Determines whether the specified gebruikersrecht is verlengbaar.
        /// </summary>
        /// <param name="gebruikersrecht">The gebruikersrecht.</param>
        /// <returns><c>true</c> if the specified gebruikersrecht is verlengbaar; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        public bool IsVerlengbaar(GebruikersRechtV2 gebruikersrecht)
        {
            throw new NotImplementedException();
        }

        public bool IsGav(Persoon p)
        {
            throw new NotImplementedException();
        }

        public bool HeeftPermissies(Groep groep, Domain.Permissies permissies)
        {
            throw new NotImplementedException();
        }

        public Permissies PermissiesOphalen(Lid lid)
        {
            throw new NotImplementedException();
        }

        public Permissies PermissiesOphalen(Functie functie)
        {
            throw new NotImplementedException();
        }

        public bool MagLezen(Persoon ik, Persoon persoon2)
        {
            throw new NotImplementedException();
        }

        public Permissies PermissiesOphalen(Groep groep, SecurityAspect aspecten)
        {
            throw new NotImplementedException();         
        }

        public bool MagZichzelfLezen(Persoon persoon)
        {
            throw new NotImplementedException();
        }

        public Permissies PermissiesOphalen(GelieerdePersoon gelieerdePersoon)
        {
            throw new NotImplementedException();
        }

        public Permissies EigenPermissies(Persoon persoon)
        {
            throw new NotImplementedException();
        }

        public GebruikersRechtV2 GebruikersRechtOpEigenGroep(GelieerdePersoon gp)
        {
            throw new NotImplementedException();
        }
    }
}
