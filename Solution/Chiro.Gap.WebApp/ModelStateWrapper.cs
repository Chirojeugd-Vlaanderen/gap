// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Web.Mvc;

using Chiro.Gap.Validatie;

namespace Chiro.Gap.WebApp
{
	/// <summary>
	/// Een ModelStateWrapper pakt een ModelStateDictionary in
	/// in een IValidatieDictionary, zodat die door
	/// Chiro.Gap.Validatie gebruikt kan worden.
	/// </summary>
	public class ModelStateWrapper : IValidatieDictionary
	{
		private readonly ModelStateDictionary _modelState;

		public ModelStateWrapper(ModelStateDictionary modelState)
		{
			_modelState = modelState;
		}

		public void BerichtToevoegen(string key, string bericht)
		{
			_modelState.AddModelError(key, bericht);
		}

		public bool IsGeldig
		{
			get { return _modelState.IsValid; }
		}
	}
}
