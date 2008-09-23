using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CgDal
{
    /// <summary>
    /// Data-access voor de personen.
    /// </summary>
    public class PersoonDataAccess
    {
        /// <summary>
        /// Haalt een persoon op op basis van ID
        /// </summary>
        /// <param name="persoonID">PersoonID van op te halen persoon</param>
        /// <returns></returns>
        public Persoon PersoonGet(int persoonID)
        {
            using (ChiroGroepClassesDataContext context = new ChiroGroepClassesDataContext())
            {
                context.ObjectTrackingEnabled = false;
                // Het lagenmodel maakt objecttracking onmogelijk, dus zetten
                // we dat hier alvast uit.

                return context.Persoons.SingleOrDefault<Persoon>(p => p.PersoonID == persoonID);
            }
        }

        /// <summary>
        /// Persisteert eventuele wijzigingen in het persoonsobject in de database.
        /// </summary>
        /// <param name="persoon">de persoon in kwestie</param>
        public void PersoonUpdaten(Persoon persoon)
        {
            Debug.Assert(persoon != null);

            using (ChiroGroepClassesDataContext context = new ChiroGroepClassesDataContext())
            {
                switch (persoon.Status)
                {
                    case EntityStatus.Geen:
                        break;
                    case EntityStatus.Nieuw:
                        context.Persoons.InsertOnSubmit(persoon);
                        break;
                    case EntityStatus.Gewijzigd:
                        context.Persoons.Attach(persoon, true);
                        break;
                    case EntityStatus.Verwijderd:
                        context.Persoons.Attach(persoon, true);
                        context.Persoons.DeleteOnSubmit(persoon);
                        break;
                    default:
                        break;
                }
                context.SubmitChanges();
            }
        }
    }
}
