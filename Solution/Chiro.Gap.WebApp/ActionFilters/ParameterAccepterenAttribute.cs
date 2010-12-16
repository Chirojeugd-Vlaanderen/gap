// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
		/// 'Waarde' van de knop (gegeven in value-attribuut van knop input)
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
		/// <returns><c>true</c> indien deze controlleractie uitgevoerd mag worden, anders <c>false</c>.</returns>
		public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
		{
			var req = controllerContext.RequestContext.HttpContext.Request;
			return req.Form[Naam] == Waarde;
		}
	}
}
