using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class GroepInfo
	{
		/// <summary>
		/// GroepID van de groep
		/// </summary>
		[DataMember]
		public int ID;

		/// <summary>
		/// Groepsnaam
		/// </summary>
		[DataMember]
		public string Naam;

		/// <summary>
		/// Plaats van de groep, indien van toepassing
		/// </summary>
		/// <remarks>Enkel Chirogroepen hebben een plaats</remarks>
		[DataMember]
		public string Plaats;

		/// <summary>
		/// Stamnummer, heeft enkel nog nut als zoeksleutel.
		/// </summary>
		[DataMember]
		public string StamNummer;

		/// <summary>
		/// Lijst met info over afdelingen van huidig werkjaar.
		/// </summary>
		/// <remarks>
		/// Kan <c>null</c> blijven indien niet relevant.
		/// </remarks>
		[DataMember]
		public IList<AfdelingInfo> AfdelingenDitWerkJaar;

		/// <summary>
		/// Lijst met info over afdelingen van categorieen.
		/// </summary>
		/// <remarks>
		/// Kan <c>null</c> blijven indien niet relevant.
		/// </remarks>
		[DataMember]
		public IList<CategorieInfo> Categorie;
	}
}