// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
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

		// TODO (#704): StandaardGeboorteJaarVan en StandaardGeboorteJaarTot mogen wat mij betreft gerust
		// in het datacontract zitten, maar dan gewoon als 'data member'.  Het berekenen van deze
		// jaren hoort thuis in de business, en mag niet door het datacontract zelf gebeuren

		/// <summary>
		/// Standaard 'geboortejaar van' voor gegeven werkJaar
		/// </summary>
		/// <param name="werkJaar">Het werkJaar waar het over gaat</param>
		/// <returns>Een jaartal</returns>
		public int StandaardGeboorteJaarVan(int werkJaar)
		{
			return werkJaar - LeefTijdTot;
		}

		/// <summary>
		/// Standaard 'geboortejaar van' voor dit werkJaar
		/// </summary>
		/// <param name="werkJaar">Het werkJaar waar het over gaat</param>
		/// <returns>Een jaartal</returns>
		public int StandaardGeboorteJaarTot(int werkJaar)
		{
			return werkJaar - LeefTijdVan;
		}
	}
}
