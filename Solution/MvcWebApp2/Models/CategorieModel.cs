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
        public int AanvragerID { get; set; }

        /// <summary>
        /// Nieuwe categorie voor de gegeven gelieerde personen
        /// </summary>
        public IList<Categorie> categorieIDs { get; set; }

        /// <summary>
        /// Standaardconstructor - creeert lege
        /// </summary>
        public CategorieModel()
        {
            AanvragerID = 0;
            categorieIDs = new List<Categorie>();
        }

        public CategorieModel(int aanvragerID, int groepID) : this()
        {
            AanvragerID = aanvragerID;
            categorieIDs = ServiceHelper.CallService<IGroepenService, IList<Categorie>>(l => l.OphalenMetCategorieen(groepID).Categorie.ToList<Categorie>());
        }
    }
}
