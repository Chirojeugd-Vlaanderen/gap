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

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Sommige functies zijn gepredefinieerd, en hun codes moeten constant zijn.
	/// (Dit is enkel van toepassing op nationaal gedefinieerde functies)
	/// </summary>
	public enum GepredefinieerdeFunctieType
	{
		Geen = 0,
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
		private bool _teVerwijderen = false;

		public bool TeVerwijderen
		{
			get { return _teVerwijderen; }
			set { _teVerwijderen = value; }
		}

		public override int GetHashCode()
		{
			// Al stiekem hopend op een bugfix ivm het desynchronisatieprobleem met entity's,
			// gebruik ik toch al ID.GetHashCode() als hash code.

			return ID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is Functie)
			{
				Functie f = obj as Functie;

				if (f.ID != 0 && f.ID == this.ID)
				{
					return true;
				}
				else if (f.ID == 0 && this.ID == 0)
				{
					return base.Equals(f);
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public string VersieString
		{
			get { return this.VersieStringGet(); }
			set { this.VersieStringSet(value); }
		}

		/// <summary>
		/// Als de functie gepredefinieerd is, kom je via deze property te weten over welke
		/// gepredefinieerde functie het gaat.
		/// </summary>
		public GepredefinieerdeFunctieType GepredefinieerdeFunctie
		{
			get
			{
				// Constructie met linq, en niet gewoon conversie, om default (0, 'Geen') te krijgen
				// als de functiecode niet overeenkomt met een gepredefinieerde functie.
				//
				return (from GepredefinieerdeFunctieType f in Enum.GetValues(typeof(GepredefinieerdeFunctieType))
						where (int)f == ID
						select f).FirstOrDefault();
			}
		}
	}
}
