// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor zaken die op de masterpage getoond moeten worden,
	/// </summary>
	/// <remarks>
	/// Met dank aan http://stackoverflow.com/questions/768236/how-to-create-a-strongly-typed-master-page-using-a-base-controller-in-asp-net-mvc
	/// </remarks>
	public class MasterViewModel : IMasterViewModel
	{
		/// <summary>
		/// De standaardconstructor maakt gewoon een lege lijst met mededelingen
		/// </summary>
		public MasterViewModel()
		{
			Mededelingen = new List<Mededeling>();
		}

		/// <summary>
		/// ID van de Chirogroep
		/// </summary>
		public int GroepID { get; set; }

		/// <summary>
		/// Naam van de Chirogroep
		/// </summary>
		public string GroepsNaam { get; set; }

		/// <summary>
		/// Plaats van de Chirogroep
		/// </summary>
		public string Plaats { get; set; }

		/// <summary>
		/// Het stamnummer wordt niet meer gebruikt als primary key, maar zal nog wel
		/// </summary>
		public string StamNummer { get; set; }

		/// <summary>
		/// Titel van de pagina
		/// </summary>
		public string Titel { get; set; }

		/// <summary>
		/// Kan de GAV meerdere groepen beheren?
		/// Deze waarde bepaalt of we de link tonen waar je een andere groep kunt kiezen.
		/// </summary>
		public bool? MeerdereGroepen { get; set; }

		/// <summary>
		/// Mededelingen die ergens getoond moeten worden
		/// </summary>
		public IList<Mededeling> Mededelingen { get; set; }
	}
}
