// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Instantieert een Persoon-object dat zorgt voor samenwerking met Entity Framework
	/// </summary>
	/// <remarks>
	/// Als er een persoon met adressen over de service gestuurd wordt,
	/// en een PersoonsAdres uit de lijst met PersoonsAdressen 
	/// is TeVerwijderen, dan is het de bedoeling dat
	/// het PersoonsAdresobject mee verdwijnt uit de database.  Om daarvoor
	/// te zorgen, is attribuut AssociationEndBehavior
	/// nodig.  (Als dat attribuut er niet stond, zou enkel
	/// de koppeling tussen Persoon en Persoonsadres verdwijnen, en
	/// dat heeft dan weer een key violation tot gevolg.)
	/// </remarks>
	[AssociationEndBehavior("PersoonsAdres", Owned = true)]
	public partial class Persoon : IEfBasisEntiteit
	{
		private bool _teVerwijderen;

		/// <summary>
		/// Wordt gebruikt om te verwijderen entiteiten mee te markeren
		/// </summary>
		public bool TeVerwijderen
		{
			get { return _teVerwijderen; }
			set { _teVerwijderen = value; }
		}

		/// <summary>
		/// Geeft stringrepresentatie van Versie weer (hex).
		/// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
		/// </summary>
		public string VersieString
		{
			get { return this.VersieStringGet(); }
			set { this.VersieStringSet(value); }
		}

		#region Identity en equality

		/// <summary>
		/// Een arbitraire waarde waarmee we het object kunnen identificeren
		/// </summary>
		/// <returns>Een int waarmee we het object kunnen identificeren</returns>
		public override int GetHashCode()
		{
			return 3;
		}

		/// <summary>
		/// Vergelijkt het huidige object met een ander om te zien of het over
		/// twee instanties van hetzelfde object gaat
		/// </summary>
		/// <param name="obj">Het object waarmee we het huidige willen vergelijken</param>
		/// <returns><c>True</c> als het schijnbaar om twee instanties van hetzelfde object gaat</returns>
		public override bool Equals(object obj)
		{
			IEfBasisEntiteit andere = obj as Persoon;
			// Als obj geen GelieerdePersoon is, wordt andere null.

			if (andere == null)
			{
				return false;
			}
			else
			{
				return (ID != 0) && (ID == andere.ID)
					|| (ID == 0 || andere.ID == 0) && base.Equals(andere);
			}

			// Is obj geen GelieerdePersoon, dan is de vergelijking altijd vals.
			// Hebben beide objecten een ID verschillend van 0, en zijn deze
			// ID's gelijk, dan zijn de objecten ook gelijk.  Zo niet gebruiken we
			// base.Equals, wat eigenlijk niet helemaal correct is.
		}

		#endregion

		/// <summary>
		/// Een enumwaarde voor het geslacht van de persoon
		/// </summary>
		public GeslachtsType Geslacht
		{
			get
			{
				return (GeslachtsType)GeslachtsInt;
			}
			set
			{
				GeslachtsInt = (int)value;
			}
		}

		/// <summary>
		/// Concatenatie van voornaam en naam
		/// </summary>
		public string VolledigeNaam
		{
			get
			{
				return String.Format("{0} {1}", VoorNaam, Naam);
			}
		}
	}
}
