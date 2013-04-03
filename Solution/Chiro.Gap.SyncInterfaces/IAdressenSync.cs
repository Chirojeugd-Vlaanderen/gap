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
using System.Collections.Generic;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.SyncInterfaces
{
    /// <summary>
    /// Regelt de synchronisatie van adresgegevens naar Kipadmin
    /// </summary>
    public interface IAdressenSync
	{
		/// <summary>
		/// Stelt de gegeven persoonsadressen in als standaardadressen in Kipadmin
		/// </summary>
		/// <param name="persoonsAdressen">Persoonsadressen die als standaardadressen (adres 1) naar
		/// Kipadmin moeten.  Personen moeten gekoppeld zijn, net zoals adressen met straatnaam en gemeente</param>
		void StandaardAdressenBewaren(IEnumerable<PersoonsAdres> persoonsAdressen);
	}
}