// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model gebruikt om een communicatievorm weer te geven.
	/// </summary>
	public class CommVormModel : MasterViewModel
	{
		/// <summary>
		/// ID van GelieerdePersoon wiens/wier communicatievorm 
		/// we bekijken 
		/// </summary>
		public GelieerdePersoon Aanvrager { get; set; }

		/// <summary>
		/// Nieuwe input voor de communicatievorm voor de gegeven gelieerde personen
		/// </summary>
		public CommunicatieVorm NieuweCommVorm { get; set; }

		/// <summary>
		/// Standaardconstructor - creëert leeg CommVormModel
		/// </summary>
		public CommVormModel()
		{
			Aanvrager = new GelieerdePersoon();
			NieuweCommVorm = new CommunicatieVorm();
		}

		public CommVormModel(GelieerdePersoon aanvrager, CommunicatieVorm v)
			: this()
		{
			Aanvrager = aanvrager;
			NieuweCommVorm = v;
		}
	}
}
