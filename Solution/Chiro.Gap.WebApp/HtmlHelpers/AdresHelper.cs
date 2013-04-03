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

using System;
using System.Web.Mvc;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.HtmlHelpers
{
	public static class AdresHelper
	{
		public static string Adres(this HtmlHelper htmlHelper, PersoonOverzicht p)
		{
			if (String.IsNullOrEmpty(p.StraatNaam) || p.PostNummer == null || String.IsNullOrEmpty(p.WoonPlaats))
			{
				return "<span class=\"error\">onvolledig</span>";
			}
			else
			{
				// TODO (#238): Buitenlandse adressen
				return String.Format(
					"{0} {1} {2}<br />{3} {4}",
					p.StraatNaam,
					p.HuisNummer,
					p.Bus,
					p.PostNummer,
					p.WoonPlaats);
			}
		}
	}
}
