// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor info over officiele afdeling
	/// </summary>
	[DataContract]
	public class OfficieleAfdelingDetail
	{
		/// <summary>
		/// Naam van de officiele afdeling
		/// </summary>
		[DataMember]
		public string Naam { get; set; }

		/// <summary>
		/// De OfficieleAfdelingID
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// Standaard oudste leeftijd voor deze afdeling
		/// </summary>
		[DataMember]
		public int LeefTijdVan { get; set; }

		/// <summary>
		/// Standaard jongste leeftijd voor deze afdeling
		/// </summary>
		[DataMember]
		public int LeefTijdTot { get; set; }

		/// <summary>
		/// Standaard 'geboortejaar van' voor gegeven werkjaar
		/// </summary>
		/// <param name="werkjaar"></param>
		/// <returns></returns>
		public int StandaardGeboorteJaarVan(int werkjaar)
		{
			return werkjaar - LeefTijdTot;
		}

		/// <summary>
		/// Standaard 'geboortejaar van' voor dit werkjaar
		/// </summary>
		/// <param name="werkjaar"></param>
		/// <returns></returns>
		public int StandaardGeboorteJaarTot(int werkjaar)
		{
			return werkjaar - LeefTijdVan;
		}
	}
}
