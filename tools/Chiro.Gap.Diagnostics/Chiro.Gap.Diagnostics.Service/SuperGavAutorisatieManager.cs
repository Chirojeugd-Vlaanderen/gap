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
using System.ServiceModel;

using Chiro.Gap.Orm;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers;

namespace Chiro.Gap.Diagnostics.Service
{
    /// <summary>
    /// Autorisatiemanager die slechts 2 dingen doet:
    /// 1. Zeggen 'Ik ben super-GAV'.
    /// 2. De gebruikersnaam opleveren
    /// Alle andere zaken zijn niet-geïmplementeerd, en voor de veiligheid
    /// blijven ze dat best ook.
    /// </summary>
    /// <remarks>
    /// Deze klasse is een kopie van die in Chiro.Gap.UpdataSvc.Service.  Het lijkt
    /// me zo dom om hiervoor een apart project te maken.  Maar ik weet ook niet
    /// goed waar ik dit anders kwijt moet.  Ik wil dit uit Chiro.Gap.Workers houden,
    /// om te vermijden dat je super-gav kunt worden door gewoon de unity-configuration
    /// aan te passen.  Nu kun je dat enkel als Chiro.Gap.Diagnostics.Service.dll
    /// beschikbaar is.
    /// </remarks>
    public class SuperGavAutorisatieManager : IAutorisatieManager
    {
        public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
        {
            throw new NotImplementedException();
        }

        public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> EnkelMijnLeden(IEnumerable<int> lidIDs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verwijdert uit een lijst <paramref name="afdelingIDs"/> de ID's van afdelingen voor wie de
        /// aangemelde gebruiker geen GAV is.
        /// </summary>
        /// <param name="afdelingIDs">ID's van afdelingen</param>
        /// <returns>Enkel de <paramref name="afdelingIDs"/> van afdelingen waarvoor de gebruiker GAV is.</returns>
        public IEnumerable<int> EnkelMijnAfdelingen(IEnumerable<int> afdelingIDs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Groep> MijnGroepenOphalen()
        {
            throw new NotImplementedException();
        }

        public bool IsGavGelieerdePersoon(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavGroep(int groepID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavGroepen(IEnumerable<int> groepIDs)
        {
            throw new NotImplementedException();
        }

        public bool IsGavGroepsWerkJaar(int groepsWerkJaarID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavPersoon(int persoonID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavAfdeling(int afdelingsID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavAfdelingsJaar(int afdelingsJaarID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavLid(int lidID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavCategorie(int categorieID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavCommVorm(int commvormID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavFunctie(int functieID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavPersoonsAdres(int persoonsAdresID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavPersoonsAdressen(IEnumerable<int> persoonsAdresIDs)
        {
            throw new NotImplementedException();
        }

        public bool IsGavUitstap(int uitstapID)
        {
            throw new NotImplementedException();
        }

        public bool IsSuperGav()
        {
            return true;
        }

        public string GebruikersNaamGet()
        {
            return ServiceSecurityContext.Current == null ? String.Empty
                : ServiceSecurityContext.Current.WindowsIdentity.Name;
        }

        public IEnumerable<int> MijnGroepIDsOphalen()
        {
            throw new NotImplementedException();
        }

        public bool IsGavPlaats(int plaatsID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavDeelnemer(int deelnemerID)
        {
            throw new NotImplementedException();
        }

        public bool IsGavGebruikersRecht(int gebruikersRechtID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Geeft de gegeven <paramref name="gav"/> gebruikersrecht voor de gegeven <paramref name="groep"/>,
        /// met een zekere <paramref name="vervalDatum"/>.  Persisteert niet.
        /// </summary>
        /// <param name="gav">
        /// GAV die gebruikersrecht moet krijgen
        /// </param>
        /// <param name="groep">
        /// Groep waarvoor gebruikersrecht verleend moet worden
        /// </param>
        /// <param name="vervalDatum">
        /// Vervaldatum van het gebruikersrecht
        /// </param>
        /// <returns>
        /// Het gegeven GebruikersRecht
        /// </returns>
        /// <remarks>
        /// Aan de GAV moeten al zijn gebruikersrechten op voorhand gekoppeld zijn.
        /// Als er al een gebruikersrecht bestaat, wordt gewoon de vervaldatum aangepast.
        /// </remarks>
        public GebruikersRecht GebruikersRechtToekennen(string gav, int groep, DateTime vervalDatum)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Kijkt na of de aangelogde gebruiker GAV is van de account met gegeven <paramref name="accountID"/>.
        /// </summary>
        /// <param name="accountID">ID van de te controleren account</param>
        /// <returns><c>true</c> als de aangelogde gebruiker GAV is van de account met gegeven <paramref name="accountID"/></returns>
        /// <remarks>met account bedoel ik hetgeen nu de Gav-klasse is. Maar op termijn moet die klasse hernoemd worden naar
        /// account (zie #1357)</remarks>
        public bool IsGavAccount(int accountID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Controleert of de aangelogde gebruiker GAV-rechten heeft op de gebruikersrechten met gegeven
        /// <paramref name="gebruikersRechtIDs"/>
        /// </summary>
        /// <param name="gebruikersRechtIDs">ID's van gebruikersrechten die gecontroleerd moeten worden</param>
        /// <returns><c>true</c> als de aangelogde gebruiker GAV-rechten heeft op de gebruikersrechten met gegeven
        /// <paramref name="gebruikersRechtIDs"/></returns>
        public bool IsGavGebruikersRechten(int[] gebruikersRechtIDs)
        {
            throw new NotImplementedException();
        }

        public GebruikersRecht GebruikersRechtGelieerdePersoon(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        public bool IsVerlengbaar(GebruikersRecht gebruikersrecht)
        {
            throw new NotImplementedException();
        }
    }
}
