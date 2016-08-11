/*
 * Copyright 2008-2013,2015,2016 the GAP developers. See the NOTICE file at the 
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
using System.Collections.Generic;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;

namespace Chiro.Gap.Dummies
{
    /// <summary>
    /// Gauw een klasse die gebruikt kan worden om eender welke Sync te mocken.
    /// </summary>
    public class DummySync : IAdressenSync, ICommunicatieSync, IPersonenSync, ILedenSync, IVerzekeringenSync, IBivakSync, IGroepenSync, IAbonnementenSync
    {
        public void StandaardAdressenBewaren(IList<PersoonsAdres> persoonsAdressen)
        {
        }

        public void Verwijderen(CommunicatieVorm communicatieVorm)
        {
        }

        public void Toevoegen(CommunicatieVorm commvorm)
        {
        }

        public void Bijwerken(CommunicatieVorm communicatieVorm, string origineelNummer)
        {
        }

        public void Updaten(GelieerdePersoon gp)
        {
            if (gp == null)
            {
                throw new ArgumentNullException("gp");
            }
        }

        public void MembershipRegistreren(Lid lid)
        {
        }

        /// <summary>
        /// Probeert de gegeven gelieerde persoon in de Chirocivi te vinden, en updatet
        /// hem als dat lukt. Wordt de persoon niet gevonden, dan wordt er een
        /// nieuwe aangemaakt.
        /// </summary>
        /// <param name="gp">Te bewaren gelieerde persoon</param>
        public void UpdatenOfMaken(GelieerdePersoon gp)
        {
        }

        public void CommunicatieUpdaten(GelieerdePersoon gp)
        {
        }

        public void Bewaren(Lid l)
        {
        }

        public void FunctiesUpdaten(Lid lid)
        {
        }

        public void AfdelingenUpdaten(Lid lid)
        {
        }

        public void TypeUpdaten(Lid lid)
        {
        }

        public void Verwijderen(Lid lid)
        {
        }

        public void Bewaren(IList<Lid> leden)
        {
        }

        public void Abonneren(GelieerdePersoon gp)
        {
        }

        public void Bewaren(PersoonsVerzekering persoonsVerzekering, GroepsWerkJaar gwj)
        {
        }

    	public void Bewaren(Uitstap uitstap)
    	{
    	}

    	public void Verwijderen(int uitstapID)
    	{
    	}

        public void Bewaren(Groep g)
        {
        }

        /// <summary>
        /// Sluit het huidige werkjaar van de gegeven <param name="groep"> af in Civi. Dat komt erop neer dat
        /// alle lidrelaties worden beeindigd.</param>
        /// </summary>
        /// <param name="groep">Groep waarvan het werkjaar afgesloten moet worden.</param>
        public void WerkjaarAfsluiten(Groep groep)
        {
        }

        public void AbonnementBewaren(Abonnement teSyncenAbonnement)
        {
        }

        public void AlleAbonnementenVerwijderen(GelieerdePersoon gelieerdePersoon)
        {
        }

        public void Uitschrijven(UitschrijfInfo info)
        {
        }
    }
}
