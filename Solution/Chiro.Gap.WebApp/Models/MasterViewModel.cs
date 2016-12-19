/*
 * Copyright 2008-2013, 2016 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkte authenticatie Copyright 2014 Johan Vervloet
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
using System.ComponentModel;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor zaken die op de masterpage getoond moeten worden,
	/// </summary>
	/// <remarks>
	/// Met dank aan http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc
	/// </remarks>
	public class MasterViewModel
	{
		/// <summary>
		/// De standaardconstructor maakt gewoon een lege lijst met mededelingen
		/// </summary>
		public MasterViewModel()
		{
			Mededelingen = new List<Mededeling>();
		}

		/// <summary>
		/// Geeft aan of we op een live- of een testomgeving werken.
		/// </summary>
		public bool IsLive { get; set; }

		/// <summary>
		/// ID van de Chirogroep
		/// </summary>
		public int GroepID { get; set; }

		/// <summary>
		/// Naam van de Chirogroep
		/// </summary>
		public string GroepsNaam { get; set; }

		/// <summary>
		/// Plaats van de Chirogroep
		/// </summary>
		public string Plaats { get; set; }

		/// <summary>
		/// Het stamnummer wordt niet meer gebruikt als primary key, maar zal nog wel
		/// </summary>
		public string StamNummer { get; set; }

		/// <summary>
		/// Titel van de pagina
		/// </summary>
		public string Titel { get; set; }

		/// <summary>
		/// Int die het *jaartal* van het huidige werkJaar voor de groep bepaalt.
		/// (Bijv. 2010 voor 2010-2011)
		/// </summary>
		public int HuidigWerkJaar { get; set; }

	    /// <summary>
	    /// 'Human readable' representatie van het huidige werkjaar, bijv. '2016-2017'.
	    /// </summary>
	    [DisplayName(@"Huidig werkjaar")]
	    public string WerkJaarWeergave
	    {
	        get { return String.Format("{0}-{1}", HuidigWerkJaar, HuidigWerkJaar + 1); }
	    }

	    /// <summary>
		/// Status van het huidige werkjaar van de groep.
		/// </summary>
		public WerkJaarStatus WerkJaarStatus { get; set; }

		/// <summary>
		/// Kan de GAV meerdere groepen beheren?
		/// Deze waarde bepaalt of we de link tonen waar je een andere groep kunt kiezen.
		/// </summary>
		public bool? MeerdereGroepen { get; set; }

		/// <summary>
		/// Mededelingen die ergens getoond moeten worden
		/// </summary>
		public IList<Mededeling> Mededelingen { get; set; }

		/// <summary>
		/// Niveau van de groep (chirogroep of kadergroep)
		/// </summary>
		public Niveau GroepsNiveau { get; set; }

        /// <summary>
        /// Aangelogde gebruiker
        /// </summary>
        public GebruikersDetail Ik { get; set; }
	}
}
