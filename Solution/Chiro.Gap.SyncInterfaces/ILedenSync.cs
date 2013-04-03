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
	/// Interface voor synchronisatie van lidinfo naar Kipadmin
	/// </summary>
	public interface ILedenSync
	{
		/// <summary>
		/// Stuurt een lid naar Kipadmin
		/// </summary>
		/// <param name="l">Te bewaren lid</param>
		void Bewaren(Lid l);

		/// <summary>
		/// Updatet de functies van het lid in Kipadmin
		/// </summary>
		/// <param name="lid">Lid met functies</param>
		/// <remarks>Als er geen persoonsgegevens meegeleverd zijn, halen we die wel even op :)</remarks>
		void FunctiesUpdaten(Lid lid);

		/// <summary>
		/// Updatet de afdelingen van <paramref name="lid"/> in Kipadmin
		/// </summary>
		/// <param name="lid">Het lidobject dat we bewerken</param>
		/// <remarks>Alle (!) relevante gegevens van het lidobject worden hier sowieso opnieuw opgehaald, anders was het
		/// te veel een gedoe.</remarks>
		void AfdelingenUpdaten(Lid lid);

		/// <summary>
		/// Updatet het lidtype van <paramref name="lid"/> in Kipadmin
		/// </summary>
		/// <param name="lid">Lid waarvan het lidtype geupdatet moet worden</param>
		void TypeUpdaten(Lid lid);

        /// <summary>
        /// Verwijdert een lid uit Kipadmin
        /// </summary>
        /// <param name="lid">Te verwijderen lid</param>
	    void Verwijderen(Lid lid);

        /// <summary>
        /// Stuurt een aantal leden naar Kipadmin
        /// </summary>
        /// <param name="leden">Te bewaren leden</param>
	    void Bewaren(IList<Lid> leden);
	}
}
