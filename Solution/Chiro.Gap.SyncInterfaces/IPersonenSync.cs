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
		/// <param name="metStandaardAdres">Stuurt ook het standaardadres mee (moet dan wel gekoppeld zijn)</param>
		/// <param name="metCommunicatie">Stuurt ook communicatie mee.  Hiervoor wordt expliciet alle
		/// communicatie-info opgehaald, omdat de workers typisch niet toestaan dat de gebruiker alle
		/// communicatie ophaalt.</param>
		void Bewaren(GelieerdePersoon gp, bool metStandaardAdres, bool metCommunicatie);

		/// <summary>
		/// Stuurt enkel de communicatie van een gelieerde persoon naar Kipadmin
		/// </summary>
		/// <param name="gp">Gelieerde persoon, met daaraan gekoppeld zijn communicatie</param>
		void CommunicatieUpdaten(GelieerdePersoon gp);
	}
}