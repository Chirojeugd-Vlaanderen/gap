// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Instantieert een GebruikersRecht-object dat zorgt voor samenwerking met Entity Framework
	/// </summary>
	public partial class GebruikersRecht : IEfBasisEntiteit
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

		/// <summary>
		/// Een arbitraire waarde waarmee we het object kunnen identificeren
		/// </summary>
		/// <returns>Een int waarmee we het object kunnen identificeren</returns>
		public override int GetHashCode()
		{
			return 10;
		}

        /// <summary>
        /// Geeft weer of de vervaldatum verlengbaar is. Dit is eigenlijk business, dus dat
        /// staat hier verkeerd.
        /// </summary>
        /// <returns></returns>
        public bool IsVerlengbaar
        {
            get
            {
                return VervalDatum != null && ((DateTime) VervalDatum) < DateTime.Now.AddMonths(
                    Properties.Settings.Default.MaandenGebruikersRechtVerlengbaar);
            }
        }

        /// <summary>
        /// Rol van de gebruiker. Op dit moment ondersteunen we enkel GAV (zie #844)
        /// </summary>
	    public Rol Rol
	    {
	        get { return VervalDatum == null || VervalDatum < DateTime.Now ? Rol.Geen : Rol.Gav; }
	    }
    }
}
