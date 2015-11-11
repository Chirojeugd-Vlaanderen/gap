/*
 * Copyright 2008-2013, 2015 the GAP developers. See the NOTICE file at the 
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
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.SyncInterfaces
{
	/// <summary>
	/// Interface voor synchronisatie van persoonsinfo naar Kipadmin
	/// </summary>
	public interface IPersonenSync
	{
	    /// <summary>
	    /// Stuurt de persoonsgegevens, samen met eventueel adressen en/of communicatie, naar Kipadmin
	    /// </summary>
	    /// <param name="gp">Gelieerde persoon, persoonsinfo</param>
	    void Updaten(GelieerdePersoon gp);

        /// <summary>
        /// Registreert een membership in de Chirocivi voor het gegeven <paramref name="lid"/>
        /// </summary>
        /// <param name="lid">Lid waarvoor membership geregistreerd moet worden</param>
	    void MembershipRegistreren(Lid lid);

	    /// <summary>
	    /// Probeert de gegeven gelieerde persoon in de Chirocivi te vinden, en updatet
	    /// hem als dat lukt. Wordt de persoon niet gevonden, dan wordt er een
	    /// nieuwe aangemaakt.
	    /// </summary>
	    /// <param name="gp">Te bewaren gelieerde persoon</param>
	    void UpdatenOfMaken(GelieerdePersoon gp);
	}
}