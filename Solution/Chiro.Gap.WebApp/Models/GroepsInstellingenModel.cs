// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor overzicht van algemene groepsinfo
	/// </summary>
	/// <remarks>
	/// Aangezien ik de info van een ChiroGroep nodig heb, en de members van IMasterViewModel
	/// hiervan een subset zijn, map ik deze via een impliciete implementatie van IMasterViewModel.
	/// </remarks>
	public class GroepsInstellingenModel : IMasterViewModel
	{
		public GroepDetail Detail { get; set; }
		public GroepInfo Info { get { return Detail; } }
		public CategorieInfo NieuweCategorie { get; set; }

		#region IMasterViewModel Members

		int IMasterViewModel.GroepID
		{
			get { return Info.ID; }
		}

		string IMasterViewModel.GroepsNaam
		{
			get { return Info.Naam; }
		}

		string IMasterViewModel.Plaats
		{
			get { return Info.Plaats; }
		}

		string IMasterViewModel.StamNummer
		{
			get { return Info.StamNummer; }
		}

		public string Titel { get; set; }

		public IList<Mededeling> Mededelingen { get; set; }

		#endregion
	}
}
