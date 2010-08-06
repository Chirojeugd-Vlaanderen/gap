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
	/// Instantieert een PersoonsAdres-object dat zorgt voor samenwerking met Entity Framework
	/// </summary>
	/// <remarks>ZEEEER BELANGRIJK! PersoonsAdres koppelt een persoon aan een adres, via PersoonsAdres.Persoon en 
	/// PersoonsAdres.Adres.  Als het adres het standaardadres is voor de persoon in een bepaalde groep
	/// (GelieerdePersoon), dan wijst PersoonsAdres.GelieerdePersoon naar de gelieerde persoon die standaard
	/// op dit adres woont.  KOPPEKE GOED BIJHOUDEN!</remarks>
	public partial class PersoonsAdres : IEfBasisEntiteit
	{
		private bool _teVerwijderen;

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

		/// <summary>
		/// Een enumwaarde voor het type adres (bv. 'kotadres')
		/// </summary>
		public AdresTypeEnum AdresType
		{
			get
			{
				return (AdresTypeEnum)AdresTypeInt;
			}
			set
			{
				AdresTypeInt = (int)value;
			}
		}

		/// <summary>
		/// Een arbitraire waarde waarmee we het object kunnen identificeren
		/// </summary>
		/// <returns>Een int waarmee we het object kunnen identificeren</returns>
		public override int GetHashCode()
		{
			return 2;
		}
	}
}
