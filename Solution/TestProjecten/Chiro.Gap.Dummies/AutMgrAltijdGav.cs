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
using System.Linq;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Autorisatiemanager die altijd alle rechten toekent,
	/// BEHALVE supergav.
	/// (nuttig voor testen van niet-autorisatiegebonden 
	/// business logica.)
	/// </summary>
	public class AutMgrAltijdGav : IAutorisatieManager
	{
		public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
		{
			return gelieerdePersonenIDs.ToList();
		}

		public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
		{
			return personenIDs.ToList();
		}

		public string GebruikersNaamGet()
		{
			throw new NotImplementedException();
		}

		public bool IsSuperGav()
		{
			return false;
		}

	    public bool IsGav(Groep groep)
	    {
	        return true;
	    }

	    public bool IsGav(CommunicatieVorm communicatieVorm)
	    {
	        return true;
	    }

	    public bool IsGav(GroepsWerkJaar groepsWerkJaar)
	    {
	        return true;
	    }

	    public bool IsGav(GelieerdePersoon gelieerdePersoon)
	    {
	        return true;
	    }

	    public bool IsGav(Deelnemer gelieerdePersoon)
	    {
	        return true;
	    }

	    public bool IsGav(Plaats gelieerdePersoon)
	    {
            return true;
	    }

	    public bool IsGav(Uitstap gelieerdePersoon)
	    {
            return true;
	    }

	    public bool IsGav(GebruikersRechtV2 gelieerdePersoon)
	    {
            return true;
	    }

        public bool IsGav(Lid gelieerdePersoon)
        {
            return true;
        }

        public bool IsGav(Afdeling gelieerdePersoon)
        {
            return true;
        }

        public bool IsGav(Categorie gelieerdePersoon)
        {
            return true;
        }

	    public bool IsGav(IList<GelieerdePersoon> gelieerdePersonen)
	    {
	        return true;
	    }

	    public List<GelieerdePersoon> MijnGelieerdePersonen(IList<Persoon> personen)
	    {
            return personen.SelectMany(p => p.GelieerdePersoon).ToList();
	    }

	    public bool IsGav(IList<PersoonsAdres> persoonsAdressen)
	    {
	        return true;
	    }

	    public bool IsGav(IList<Persoon> personen)
	    {
	        return true;
	    }

	    public bool IsGav(IList<Lid> leden)
	    {
	        return true;
	    }

	    public bool IsGav(IList<Groep> groepen)
	    {
	        return true;
	    }

        public bool IsGav(Persoon p)
        {
            return true;
        }


        public bool HeeftPermissies(Groep groep, Domain.Permissies permissies)
        {
            return true;
        }


    }
}
