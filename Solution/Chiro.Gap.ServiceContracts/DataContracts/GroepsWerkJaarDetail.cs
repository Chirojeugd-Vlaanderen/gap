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
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Gedetailleerde informatie over een groepswerkjaar
	/// </summary>
	[DataContract]
	public class GroepsWerkJaarDetail
	{
	    /// <summary>
	    /// Het beginjaartal, bv. 2011 voor 2011-2012
	    /// </summary>
	    [DataMember]
		public int WerkJaar { get; set; }

	    /// <summary>
	    /// De ID van het werkJaar
	    /// </summary>
	    [DataMember]
		public int WerkJaarID { get; set; }

	    /// <summary>
	    /// TODO (#190): documenteren
	    /// </summary>
	    [DataMember]
		public WerkJaarStatus Status { get; set; }

	    /// <summary>
	    /// De ID van de groep die in het opgegeven werkJaar actief is
	    /// </summary>
	    [DataMember]
		public int GroepID { get; set; }

	    /// <summary>
	    /// De naam van de groep
	    /// </summary>
	    [DataMember]
		public string GroepNaam { get; set; }

	    /// <summary>
	    /// De gemeente waar de groep zich bevindt
	    /// </summary>
	    [DataMember]
		public string GroepPlaats { get; set; }

	    /// <summary>
	    /// Het stamnummer van de groep
	    /// </summary>
	    [DataMember]
		public string GroepCode { get; set; }

	    /// <summary>
	    /// Geeft aan of het om een lokale groep, een gewest of een verbond gaat
	    /// </summary>
	    [DataMember]
		public Niveau GroepNiveau { get; set; }
	}
}
