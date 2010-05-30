// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model gebruikt om een communicatievorm weer te geven.
	/// </summary>
	public class CommVormModel : MasterViewModel
	{
        /// <summary>
        /// De standaardconstructor - creëert leeg CommVormModel
        /// </summary>
        public CommVormModel()
        {
            Aanvrager = new PersoonDetail();
            NieuweCommVorm = new CommunicatieInfo();
        }

        public CommVormModel(PersoonDetail aanvrager, CommunicatieInfo v)
            : this()
        {
            Aanvrager = aanvrager;
            NieuweCommVorm = v;
        }

		/// <summary>
		/// ID van GelieerdePersoon wiens/wier communicatievorm 
		/// we bekijken 
		/// </summary>
		public PersoonDetail Aanvrager { get; set; }

		/// <summary>
		/// Nieuwe input voor de communicatievorm voor de gegeven gelieerde personen
		/// </summary>
		public CommunicatieInfo NieuweCommVorm { get; set; }
	}
}
