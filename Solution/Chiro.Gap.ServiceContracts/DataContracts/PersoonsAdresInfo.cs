// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// AdresInfo, met als extra informatie het AdresType (thuisadres, werkadres,...)
	/// </summary>
	[DataContract]
	public class PersoonsAdresInfo : AdresInfo
	{
		/// <summary>
		/// Enumwaarde die de relatie van de persoon tot het adres beschrijft (bv. 'kotadres', 'thuis')
		/// </summary>
		[DataMember]
		public AdresTypeEnum AdresType { get; set; }

		/// <summary>
		/// Het PersoonsAdresID
		/// </summary>
		[DataMember]
		public int PersoonsAdresID { get; set; }
	}

	/// <summary>
	/// Flauw datacontractje dat enkel een PersoonID en een AdresID bevat.  Wordt gebruikt om informatie
	/// mee te geven over al bestaande adressen
	/// (Helaas had ik een datacontract geschreven met dezelfde naam als een ander datacontract van Broes.)
	/// </summary>
	[DataContract]
	public class PersoonsAdresInfo2
	{
		/// <summary>
		/// De ID van de persoon
		/// </summary>
		[DataMember]
		public int PersoonID { get; set; }

		/// <summary>
		/// De ID van het adres
		/// </summary>
		[DataMember]
		public int AdresID { get; set; }
	}
}
