// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm.Properties;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Instantieert een GelieerdePersoon-object dat zorgt voor samenwerking met Entity Framework
	/// </summary>
	[AssociationEndBehavior("Persoon", Owned = true)]
	[AssociationEndBehavior("Lid", Owned = true)]
	[AssociationEndBehavior("PersoonsAdres", Owned = true)]
	[MetadataType(typeof(GelieerdePersoon_Validatie))]
	public partial class GelieerdePersoon : IEfBasisEntiteit
	{
		/// <summary>
		/// Nested class die toelaat om validatieproperties op te zetten, en die gereferencet wordt door het MetadataType attribute
		/// Dit kan niet op de echte class, want die wordt gegenereerd door de EF Designer
		/// </summary>
		public class GelieerdePersoon_Validatie
		{
			/// <summary>
			/// De groep waar de persoon aan gelieerd is
			/// </summary>
			public Groep Groep
			{
				get;
				set;
			}

			/// <summary>
			/// Het aantal jaren dat de persoon afwijkt van de anderen van zijn/haar generatie
			/// </summary>
			[Verplicht]
			[DisplayName(@"Chiroleeftijd")]
			[Range(-8, 3, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RangeError")]
			[DisplayFormat(DataFormatString = "{0:+#0;-#0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
			public int ChiroLeefTijd
			{
				get;
				set;
			}

			/// <summary>
			/// De persoon die gelieerd is aan een groep
			/// </summary>
			public Persoon Persoon
			{
				get;
				set;
			}
		}

		// We gaan de lijst met PersoonsInfo niet opnemen in de
		// klasse.  De programmeur moet te allen tijde maar weten
		// welke informatie hij wel/niet opgevraagd heeft.

		// private IList<PersoonsInfo> _meeGeleverd;

		#region Standaarddingen IBasisEntiteit

		/// <summary>
		/// Wordt gebruikt om te verwijderen entiteiten mee te markeren
		/// </summary>
		public bool TeVerwijderen { get; set; }

		/// <summary>
		/// Geeft stringrepresentatie van Versie weer (hex).
		/// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
		/// </summary>
		public string VersieString
		{
			get { return this.VersieStringGet(); }
			set { this.VersieStringSet(value); }
		}
		#endregion

		#region Identity en equality

		/// <summary>
		/// Een arbitraire waarde waarmee we het object kunnen identificeren
		/// </summary>
		/// <returns>Een int waarmee we het object kunnen herkennen</returns>
		/// <remarks>
		/// Wordt overridden om in overeenstemming te zijn met de equals override:
		/// 2 objecten die equal zijn moeten dezelfde hashcode hebben.
		/// Omdat dit niet te garanderen was op basis van de entiteitseigenschappen tijdens deserializen (worden niet altijd gezet
		/// voor het wordt opgeroepen), wordt er niet geimplementeerd dat objecten met hetzelfde ID dezelfde hashcode hebben, maar
		/// dat objecten van dezelfde entiteitsklasse dezelfde ID hebben (een superset van objecten met dezelfde ID)
		/// <para />
		/// Het is mogelijk dat dit performantieproblemen geeft, maar vermoed wordt van niet, omdat uit ID weinig andere eigenschappen
		/// worden afgeleid.
		/// </remarks>
		public override int GetHashCode()
		{
			return 9;
		}

		/// <summary>
		/// Vergelijkt het huidige object met een ander om te zien of het over
		/// twee instanties van hetzelfde object gaat
		/// </summary>
		/// <param name="obj">Het object waarmee we het huidige willen vergelijken</param>
		/// <returns><c>True</c> als het schijnbaar om twee instanties van hetzelfde object gaat</returns>
		public override bool Equals(object obj)
		{
			bool result;

			var andere = obj as GelieerdePersoon;

			if (andere == null)
			{
				result = false;
			}
			else if (ID == 0 || andere.ID == 0)
			{
				result = base.Equals(andere);
			}
			else
			{
				result = (ID == andere.ID);
			}

			return result;
		}

		#endregion

		/// <summary>
		/// Haalt een lijst op van de categorieën waar de gelieerde persoon aan toegevoegd is
		/// </summary>
		/// <returns>Een lijst van categorieën</returns>
		public IList<Categorie> CategorieLijstGet()
		{
			return Categorie.ToList();
		}

		/// <summary>
		/// Leeftijd van de persoon, rekening houdende met geboortedatum en chiroleeftijd
		/// </summary>
		public DateTime? GebDatumMetChiroLeefTijd
		{
			get
			{
				return Persoon.GeboorteDatum.HasValue ? Persoon.GeboorteDatum.Value.AddYears(-ChiroLeefTijd) : Persoon.GeboorteDatum;
			}
		}
	}
}
