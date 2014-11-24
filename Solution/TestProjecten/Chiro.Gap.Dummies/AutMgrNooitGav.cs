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
﻿using System.Collections.Generic;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Dummies
{
	/// <summary>
	/// Autorisatiemanager die er steeds van uitgaat dat
	/// de gebruiker geen rechten heeft.
	/// (nuttig voor authorisatietests..)
	/// </summary>
	public class AutMgrNooitGav : IAutorisatieManager
	{
		public IList<int> EnkelMijnGelieerdePersonen(IEnumerable<int> gelieerdePersonenIDs)
		{
			return new List<int>();
		}

		public IList<int> EnkelMijnPersonen(IEnumerable<int> personenIDs)
		{
			return new List<int>();
		}

		public string GebruikersNaamGet()
		{
			return "Paria";
		}

		public bool IsSuperGav()
		{
			return false;
		}

		public IEnumerable<Groep> MijnGroepenOphalen()
		{
			return new List<Groep>();
		}

	    public bool IsGav(Groep groep)
	    {
	        return false;
	    }

	    public bool IsGav(CommunicatieVorm communicatieVorm)
	    {
	        return false;
	    }

	    public bool IsGav(GroepsWerkJaar groepsWerkJaar)
	    {
	        return false;
	    }

	    public bool IsGav(GelieerdePersoon gelieerdePersoon)
	    {
	        return false;
	    }

	    public bool IsGav(Deelnemer gelieerdePersoon)
	    {
            return false;
	    }

	    public bool IsGav(Plaats gelieerdePersoon)
	    {
            return false;
	    }

	    public bool IsGav(Uitstap gelieerdePersoon)
	    {
            return false;
	    }

	    public bool IsGav(GebruikersRechtV2 gelieerdePersoon)
	    {
            return false;
	    }

        public bool IsGav(Lid gelieerdePersoon)
        {
            return false;
        }

        public bool IsGav(Afdeling gelieerdePersoon)
        {
            return false;
        }

        public bool IsGav(Categorie gelieerdePersoon)
        {
            return false;
        }

	    public bool IsGav(IList<GelieerdePersoon> gelieerdePersonen)
	    {
	        return false;
	    }

	    public List<GelieerdePersoon> MijnGelieerdePersonen(IList<Persoon> personen)
	    {
	        return new List<GelieerdePersoon>();
	    }

	    public bool IsGav(IList<PersoonsAdres> persoonsAdressen)
	    {
	        return false;
	    }

	    public bool IsGav(IList<Persoon> personen)
	    {
	        return false;
	    }

	    public bool IsGav(IList<Lid> leden)
	    {
	        return false;
	    }

	    public bool IsGav(IList<Groep> groepen)
	    {
	        return false;
	    }

        public bool IsGav(Persoon p)
        {
            return false;
        }
    }
}