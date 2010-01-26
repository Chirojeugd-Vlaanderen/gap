using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using System.Diagnostics;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace Chiro.Gap.Orm
{
	// Als er een persoon met adressen over de service gestuurd wordt,
	// en een PersoonsAdres is uit de lijst met PersoonsAdressen 
	// verdwenen, dan is het de bedoeling dat
	// het PersoonsAdresobject mee verdwijnt uit de database.  Om daarvoor
	// te zorgen, is onderstaand attribuut AssociationEndBehavior
	// nodig.  (Als dat attribuut er niet zou staan, dan zou enkel
	// de koppeling tussen Persoon en Persoonsadres verdwijnen, en
	// dat heeft dan weer een key violation tot gevolg.)

	[AssociationEndBehavior("PersoonsAdres", Owned = true)]
	[AssociationEndBehavior("Persoon", Owned = true)]
	[MetadataType(typeof(GelieerdePersoon_Validatie))]
	public partial class GelieerdePersoon : IEfBasisEntiteit
	{
		/// <summary>
		/// Nested class die toelaat om validatie properties op te zetten, en die gereferenced wordt door het MetadataType attribute
		/// Dit kan niet op de echte class, want die wordt gegenereerd door de EF Designer
		/// </summary>
		public class GelieerdePersoon_Validatie
		{
			public Groep Groep { get; set; }

			[Verplicht]
			[DisplayName("Chiroleeftijd")]
			[Range(-8, +3, ErrorMessage = "{0} is beperkt van {1} tot {2}.")]
			[DisplayFormat(DataFormatString = "{0:+#0;-#0}", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
			public int ChiroLeefTijd { get; set; }

			public Persoon Persoon { get; set; }

		}


		// We gaan de lijst met PersoonsInfo niet opnemen in de
		// klasse.  De programmeur moet te allen tijde maar weten
		// welke informatie hij wel/niet opgevraagd heeft.

		// private IList<PersoonsInfo> _meeGeleverd;

		#region Standaarddingen IBasisEntiteit
		private bool _teVerwijderen = false;

		public bool TeVerwijderen
		{
			get { return _teVerwijderen; }
			set { _teVerwijderen = value; }
		}

		public GelieerdePersoon()
			: base()
		{
			// _meeGeleverd = null;
		}

		public string VersieString
		{
			get { return this.VersieStringGet(); }
			set { this.VersieStringSet(value); }
		}
		#endregion


		#region Identity en equality

		/// <summary>
		/// Wordt geoverride om in overeenstemming te zijn met de equals override:
		/// 2 objecten die equal zijn moeten dezelfde hashcode hebben.
		/// Omdat dit niet te garanderen was op basis van de entiteitseigenschappen tijdens deserializen (worden niet altijd gezet
		/// voor het wordt opgeroepen), wordt er niet geimplementeerd dat objecten met hetzelfde ID dezelfde hashcode hebben, maar
		/// dat objecten van dezelfde entiteitsklasse dezelfde ID hebben (een superset van objecten met dezelfde ID)
		/// 
		/// Het is mogelijk dat dit performantieproblemen geeft, maar vermoed wordt van niet, omdat uit ID weinig andere eigenschappen
		/// worden afgeleid.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return 9;
		}

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

		public IList<Categorie> CategorieLijstGet()
		{
			IList<Categorie> result = new List<Categorie>();
			foreach (Categorie c in this.Categorie)
			{
				result.Add(c);
			}
			return result;
		}
	}
}
