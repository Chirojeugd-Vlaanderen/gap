// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor uitgebreide info over personen
	/// </summary>
	[DataContract]
	public class PersoonDetail : PersoonInfo
	{
		/// <summary>
		/// De ID van de persoon
		/// </summary>
		[DataMember]
		public int PersoonID { get; set; }

		// TODO: Fix #546

		/// <summary>
		/// Geeft aan of de persoon als kind ingeschreven is in een groep
		/// </summary>
		[DataMember]
		public Boolean IsLid { get; set; }

		/// <summary>
		/// Geeft aan of de persoon als leiding ingeschreven is in een groep
		/// </summary>
		[DataMember]
		public Boolean IsLeiding { get; set; }

		// TODO: Fix #546

		/// <summary>
		/// Geeft aan of de persoon op basis van zijn/haar leeftijd in één van de afdelingen past
		/// </summary>
		[DataMember]
		public Boolean KanLidWorden { get; set; }

		/// <summary>
		/// Geeft aan of de persoon de juiste leeftijd heeft om leiding te kunnen worden
		/// </summary>
		[DataMember]
		public Boolean KanLeidingWorden { get; set; }

		/// <summary>
		/// De lijst van categorieën die aan de persoon toegekend zijn
		/// </summary>
		[DataMember]
		public IList<CategorieInfo> CategorieLijst { get; set; }

		/// <summary>
		/// De ID van het adres dat als voorkeursadres gemarkeerd is
		/// voor deze persoon
		/// </summary>
		[DataMember]
		public int? VoorkeursAdresID { get; set; }

		/// <summary>
		/// Concatenatie van voornaam en naam
		/// </summary>
		public string VolledigeNaam
		{
			get
			{
				return VoorNaam + " " + Naam;
			}
		}

		/// <summary>
		/// Geeft weer of de persoon geabonneerd is op Dubbelpunt
		/// </summary>
		[DataMember]
		public bool DubbelPuntAbonnement { get; set; }
	}
}
