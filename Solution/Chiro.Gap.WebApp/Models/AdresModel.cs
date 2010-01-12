using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Adf.ServiceModel;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using System.ComponentModel;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model voor verhuis:
    ///   . op een adres A wonen personen
    ///   . een aantal van die personen verhuizen naar een nieuw adres B
	/// Model voor een nieuw adres
    /// </summary>
    public class AdresModel : MasterViewModel 
    {
        /// <summary>
        /// ID van GelieerdePersoon die graag zou verhuizen.
        /// Wordt bewaard om achteraf terug naar de details van de
        /// aanvrager te kunnen redirecten.
        /// </summary>
        public int AanvragerID { get; set; }

		/// <summary>
		/// Adrestype voor het nieuwe adres
		/// </summary>
		[DisplayName("Adrestype")]
		public AdresTypeEnum AdresType { get; set; }

		/// <summary>
		/// Lijst met bewoners van het huidige adres
		/// </summary>
		public IList<GewonePersoonInfo> Bewoners { get; set; }

        /// <summary>
        /// Het adres (wordt geladen met het oude adres, komt terug met het nieuwe
        /// </summary>
        public AdresInfo Adres { get; set; }

		/// <summary>
		/// Het ID van het oude adres
		/// </summary>
		public int OudAdresID { get; set; }

        /// <summary>
        /// De IDs van de gekozen gelieerdepersonen die mee verhuizen (subset van de bewoners).
        /// </summary>
        public List<int> PersoonIDs { get; set; }

        /// <summary>
        /// Standaardconstructor
        /// </summary>
        public AdresModel()
        {
			Bewoners = new List<GewonePersoonInfo>();
			PersoonIDs = new List<int>();
			Adres = new AdresInfo();
        }
    }
}