// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Chiro.Gap.Orm;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class PersoonInfo
	{
		[DataMember]
		public int? AdNummer
		{
			get;
			set;
		}

		[DataMember]
		public int GelieerdePersoonID
		{
			get;
			set;
		}

		[DataMember]
		public int ChiroLeeftijd
		{
			get;
			set;
		}

		[DataMember]
		public int PersoonID
		{
			get;
			set;
		}

		[DataMember]
		public string VoorNaam
		{
			get;
			set;
		}

		[DataMember]
		public string Naam
		{
			get;
			set;
		}

		[DataMember]
		public DateTime? GeboorteDatum
		{
			get;
			set;
		}

		[DataMember]
		public GeslachtsType Geslacht
		{
			get;
			set;
		}

		[DataMember]
		public Boolean IsLid
		{
			get;
			set;
		}

		[DataMember]
		public IList<Categorie> CategorieLijst
		{
			get;
			set;
		}

		public string VolledigeNaam
		{
			get
			{
				return VoorNaam + " " + Naam;
			}
		}
	}
}
