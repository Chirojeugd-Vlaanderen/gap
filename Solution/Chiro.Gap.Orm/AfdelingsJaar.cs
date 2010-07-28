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
	/// Een afdelingsjaar bepaalt welke geboortejaren, geslacht en officiële afdelingen
	/// in een bepaald werkjaar overeenkomen met een gegeven afdeling.
	/// </summary>
	public partial class AfdelingsJaar : IEfBasisEntiteit
	{
		/// <summary>
		/// Instantieert een lege gemengde afdeling
		/// </summary>
		public AfdelingsJaar()
		{
			Geslacht = GeslachtsType.Gemengd;
		}

		public GeslachtsType Geslacht
		{
			get { return (GeslachtsType)GeslachtsInt; }
			set { GeslachtsInt = (int)value; }
		}

		private bool _teVerwijderen = false;

		public bool TeVerwijderen
		{
			get { return _teVerwijderen; }
			set { _teVerwijderen = value; }
		}

		public string VersieString
		{
			get { return this.VersieStringGet(); }
			set { this.VersieStringSet(value); }
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
	}
}
