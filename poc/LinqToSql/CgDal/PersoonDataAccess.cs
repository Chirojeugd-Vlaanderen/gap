using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.Linq;

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
        /// Haalt persoonsinfo inclusief adressen op
        /// </summary>
        /// <param name="persoonID">PersoonID van op te vragen persoon</param>
        /// <returns>gegevens van de gevraagde persoon</returns>
        public Persoon PersoonMetAdressenGet(int persoonID)
        {
            using (ChiroGroepClassesDataContext context = new ChiroGroepClassesDataContext())
            {
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<Persoon>(p => p.PersoonsAdres);
                options.LoadWith<PersoonsAdres>(a => a.Adres);

                context.LoadOptions = options;
                context.ObjectTrackingEnabled = false;

                return context.Persoons.SingleOrDefault<Persoon>(p => p.PersoonID == persoonID);
            }
        }

        /// <summary>
        /// Ophalen persoonsinfo van alle personen uit een groep
        /// </summary>
        /// <param name="GroepID">ID van de gevraagde groep</param>
        /// <returns>Een lijst met objecten van het type 'vPersoonsInfo'</returns>
        public IList<vPersoonsInfo> GelieerdePersonenInfoGet(int GroepID)
        {
            using (ChiroGroepClassesDataContext context = new ChiroGroepClassesDataContext())
            {
                context.ObjectTrackingEnabled = false;

                var lijst = from pi in context.vPersoonsInfos
                            from gp in context.GelieerdePersoons
                            where pi.PersoonID == gp.PersoonID && gp.GroepID == GroepID
                            select pi;

                return lijst.ToList<vPersoonsInfo>();
            }
        }


        /// <summary>
        /// Persisteert eventuele wijzigingen in het persoonsobject in de database.
        /// Ook wijzigingen in eventuele 'persoonsadressen' worden meegenomen.
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
                        if (persoon.PersoonsAdres != null)
                        {
                            context.PersoonsAdres.InsertAllOnSubmit<PersoonsAdres>(persoon.PersoonsAdres);
                        }
                        break;
                    case EntityStatus.Gewijzigd:
                        context.Persoons.Attach(persoon, true);
                        if (persoon.PersoonsAdres != null)
                        {
                            // attach gewijzigde en verwijderde persoonsadressen
                            context.PersoonsAdres.AttachAll<PersoonsAdres>(
                                persoon.PersoonsAdres.Where<PersoonsAdres>(pa => pa.Status == EntityStatus.Gewijzigd || pa.Status == EntityStatus.Verwijderd));

                            // markeer nieuwe persoonsadressen als 'toe te voegen'
                            context.PersoonsAdres.InsertAllOnSubmit<PersoonsAdres>(
                                persoon.PersoonsAdres.Where<PersoonsAdres>(pa => pa.Status == EntityStatus.Nieuw));

                            // markeer (nu geattachte) te verwijderen adressen
                            context.PersoonsAdres.DeleteAllOnSubmit<PersoonsAdres>(
                                persoon.PersoonsAdres.Where<PersoonsAdres>(pa => pa.Status == EntityStatus.Verwijderd));
                        }
                        break;
                    case EntityStatus.Verwijderd:
                        context.Persoons.Attach(persoon, true);

                        // Ik ga hier geen children (PersoonsAdres,...) verwijderen.
                        // Ik verwacht hier een exceptie als er afhankelijkheden
                        // zijn naar de te verwijderen persoon.

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
