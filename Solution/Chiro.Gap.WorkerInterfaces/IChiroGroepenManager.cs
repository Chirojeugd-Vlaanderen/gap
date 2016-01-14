/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;

namespace Chiro.Gap.WorkerInterfaces
{
	public interface IChiroGroepenManager
	{
		/// <summary>
		/// Maakt een nieuwe afdeling voor een Chirogroep, zonder te persisteren
		/// </summary>
		/// <param name="groep">
		/// Chirogroep waarvoor afdeling moet worden gemaakt, met daaraan gekoppeld
		/// de bestaande afdelingen
		/// </param>
		/// <param name="naam">
		/// Naam van de afdeling
		/// </param>
		/// <param name="afkorting">
		/// Handige afkorting voor in schemaatjes
		/// </param>
		/// <returns>
		/// De toegevoegde (maar nog niet gepersisteerde) afdeling
		/// </returns>
		/// <exception cref="GeenGavException">
		/// Komt voor als de gebruiker geen GAV is voor de opgegeven <paramref name="groep"/>
		/// </exception>
		Afdeling AfdelingToevoegen(ChiroGroep groep, string naam, string afkorting);
	}
}