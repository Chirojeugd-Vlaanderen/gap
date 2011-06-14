// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
