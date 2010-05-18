// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model gebruikt om iemand een nieuwe communicatievorm te geven:
	/// telefoonnummer, mailadres, enz.
	/// </summary>
	public class NieuweCommVormModel : MasterViewModel
	{
		/// <summary>
		/// ID van GelieerdePersoon waarvoor aangeklikt dat
		/// hij/zij een extra adres nodig heeft
		/// </summary>
		public PersoonDetail Aanvrager { get; set; }

		/// <summary>
		/// Nieuwe communicatievorm (telefoonnummer, mailadres, ...)
		/// voor de gegeven gelieerde personen
		/// </summary>
		public CommunicatieInfo NieuweCommVorm { get; set; }

		public IEnumerable<CommunicatieTypeInfo> Types { get; set; }

		/// <summary>
		/// De standaardconstructor - creëert leeg NieuweCommVorm
		/// </summary>
		public NieuweCommVormModel()
		{
			Aanvrager = new PersoonDetail();
			NieuweCommVorm = new CommunicatieInfo();
		}

		public NieuweCommVormModel(PersoonDetail aanvrager, IEnumerable<CommunicatieTypeInfo> types)
			: this()
		{
			Aanvrager = aanvrager;
			Types = types;
			NieuweCommVorm = new CommunicatieInfo();
		}
	}
}
