// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class GroepInfo
	{
		/// <summary>
		/// GroepID van de groep
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// Naam van de groep
		/// </summary>
		[DataMember]
		public string Naam { get; set; }

		/// <summary>
		/// Plaats van de groep, indien van toepassing
		/// </summary>
		/// <remarks>Enkel Chirogroepen hebben een plaats</remarks>
		[DataMember]
		public string Plaats { get; set; }

		/// <summary>
		/// Stamnummer, heeft enkel nog nut als zoeksleutel.
		/// </summary>
		[DataMember]
		[DisplayName("Stamnummer")]
		public string StamNummer { get; set; }

		/// <summary>
		/// Lijst met info over afdelingen van huidig werkjaar.
		/// </summary>
		/// <remarks>
		/// Kan <c>null</c> blijven indien niet relevant.
		/// </remarks>
		[DataMember]
		public IList<AfdelingInfo> AfdelingenDitWerkJaar { get; set; }

		/// <summary>
		/// Lijst met info over afdelingen van categorieën.
		/// </summary>
		/// <remarks>
		/// Kan <c>null</c> blijven indien niet relevant.
		/// </remarks>
		[DataMember]
		public IList<CategorieInfo> Categorie { get; set; }
	}
}