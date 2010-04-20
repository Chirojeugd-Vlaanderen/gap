// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Sommige functies zijn gepredefinieerd, en hun codes moeten constant zijn.
	/// (Dit is enkel van toepassing op nationaal gedefinieerde functies)
	/// </summary>
	public enum NationaleFunctie
	{
		ContactPersoon = 1,
		GroepsLeiding = 2,
		Vb = 3,
		FinancieelVerantwoordelijke = 4,
		JeugdRaad = 5,
		KookPloeg = 6,
		Proost = 7
	};

	/// <summary>
	/// Functie op lidniveau
	/// </summary>
	public partial class Functie : IEfBasisEntiteit
	{
		/// <summary>
		/// Standaardconstructor; maakt een functie die geschikt is voor alle types leden.
		/// </summary>
		public Functie()
		{
			Type = LidType.Alles;
		}

		private bool _teVerwijderen = false;

		public bool TeVerwijderen
		{
			get { return _teVerwijderen; }
			set { _teVerwijderen = value; }
		}

		// Custom Equals en GetHashCode; geeft mogelijk problemen bij deserializatie

		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.ChiroEquals(obj);
		}

		public string VersieString
		{
			get { return this.VersieStringGet(); }
			set { this.VersieStringSet(value); }
		}

		/// <summary>
		/// Controleert of de functie de nationale functie de nationale functie <paramref name="natFun"/> is.
		/// </summary>
		/// <param name="natFun">Een nationale functie</param>
		/// <returns><c>true</c> als de functie de nationale functie is, anders <c>false</c></returns>
		public bool Is(NationaleFunctie natFun)
		{
			return ID == ((int)natFun);
		}

		/// <summary>
		/// Koppeling tussen enum LidType en databaseveld LidTypeInt
		/// </summary>
		public LidType Type
		{
			get { return (LidType)LidTypeInt; }
			set { LidTypeInt = (int)value; }
		}

	}
}
