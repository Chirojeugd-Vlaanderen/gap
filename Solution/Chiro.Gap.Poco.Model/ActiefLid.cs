/*
 * Copyright 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
	/// <summary>
	/// Actief lid.
	/// 
	/// Dit is een view die voornamelijk wat ID's bevat van de leden die
	/// momenteel actief zijn.
	/// </summary>
	public class ActiefLid: BasisEntiteit
	{
		public ActiefLid ()
		{
		}
		public int LidID { get; set; }
		public int PersoonID { get; set; }
		public int GroepID { get; set; }
		public int? AdNummer { get; set; }
		public string Code { get; set; }
		public int WerkJaar { get; set; }

	    /// <summary>
	    /// De bedoeling is dat Versie een timestamp (row version) is, voor concurrency control
	    /// </summary>
	    public override byte[] Versie { get; set; }

	    /// <summary>
	    /// ID is de primary key
	    /// </summary>
	    public override int ID { get; set; }
	}
}

