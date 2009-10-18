using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Cg2.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;


namespace MvcWebApp2.Models
{
    /// <summary>
    /// Model gebruikt om iemand een nieuw adres te geven.
    /// </summary>
    public class CategorieModel : MasterViewModel 
    {
        // ID van GelieerdePersoon waarvoor aangeklikt dat
        // hij/zij een extra categorie nodig heeft
        public GelieerdePersoon Aanvrager { get; set; }

        /// <summary>
        /// Nieuwe categorie voor de gegeven gelieerde personen
        /// </summary>
        public IEnumerable<Categorie> Categorieen { get; set; }

        public int selectie { get; set; }

        /// <summary>
        /// Standaardconstructor - creeert lege
        /// </summary>
        public CategorieModel()
        {
            Aanvrager = null;
            Categorieen = new List<Categorie>();
        }

        public CategorieModel(IEnumerable<Categorie> c, GelieerdePersoon gp) : this()
        {
            Categorieen = c;
            Aanvrager = gp;
        }
    }
}
