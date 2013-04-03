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

using System.Web.Mvc;

namespace Chiro.Gap.WebApp.ActionFilters
{
	/// <summary>
	/// Eigen attribuut dat gebruikt kan worden in forms met meer dan 1 knop, om te detecteren op 
	/// welke knop geklikt is
	/// </summary>
	public class ParameterAccepterenAttribute : ActionMethodSelectorAttribute
	{
		/// <summary>
		/// Naam van de knop
		/// </summary>
		public string Naam
		{
			get;
			set;
		}

		/// <summary>
		/// De 'waarde' van de knop (gegeven in value-attribuut van knop input)
		/// </summary>
		public string Waarde
		{
			get;
			set;
		}

		/// <summary>
		/// Decoreer een controlleractie met dit attribuut als ze enkel uitgevoerd mag worden indien
		/// het form-element met id <c>Naam</c> de waarde <c>Waarde</c> heeft.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="methodInfo"></param>
		/// <returns><c>True</c> indien deze controlleractie uitgevoerd mag worden, anders <c>false</c>.</returns>
		public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
		{
			var req = controllerContext.RequestContext.HttpContext.Request;
			return req.Form[Naam] == Waarde;
		}
	}
}
