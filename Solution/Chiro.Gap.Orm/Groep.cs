// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Instantieert een Groep-object dat zorgt voor samenwerking met Entity Framework
	/// </summary>
	[AssociationEndBehavior("Afdeling", Owned = true)]
	[AssociationEndBehavior("Categorie", Owned = true)]
	public partial class Groep : IEfBasisEntiteit
	{
		private bool _teVerwijderen;

		public abstract Niveau Niveau { get; }

		/// <summary>
		/// Wordt gebruikt om te verwijderen entiteiten mee te markeren
		/// </summary>
		public bool TeVerwijderen
		{
			get
			{
				return _teVerwijderen;
			}
			set
			{
				_teVerwijderen = value;
			}
		}

		/// <summary>
		/// Geeft stringrepresentatie van Versie weer (hex).
		/// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
		/// </summary>
		public string VersieString
		{
			get
			{
				return this.VersieStringGet();
			}
			set
			{
				this.VersieStringSet(value);
			}
		}

		#region Identity en equality

		/// <summary>
		/// Een arbitraire waarde waarmee we het object kunnen identificeren
		/// </summary>
		/// <returns>Een int waarmee we het object kunnen herkennen</returns>
		public override int GetHashCode()
		{
			return 7;
		}

		/// <summary>
		/// Vergelijkt het huidige object met een ander om te zien of het over
		/// twee instanties van hetzelfde object gaat
		/// </summary>
		/// <param name="obj">Het object waarmee we het huidige willen vergelijken</param>
		/// <returns><c>True</c> als het schijnbaar om twee instanties van hetzelfde object gaat</returns>
		public override bool Equals(object obj)
		{
			var andere = obj as Groep;
			// Als obj geen Groep is, wordt andere null.

			if (andere == null)
			{
				return false;
			}
			else if (ID == 0 || andere.ID == 0)
			{
				return base.Equals(andere);
			}
			else
			{
				return ID.Equals(andere.ID);
			}
		}

		#endregion
	}
}
