using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.Linq;
using System.IO;

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
                context.ObjectTrackingEnabled = true;

                return (from p in context.Persoons
                        where p.PersoonID == persoonID
                        select p).SingleOrDefault<Persoon>();
            }
        }

        /// <summary>
        /// Haalt persoonsinfo inclusief adressen op
        /// </summary>
        /// <param name="persoonID">PersoonID van op te vragen persoon</param>
        /// <returns>gegevens van de gevraagde persoon</returns>
        public Persoon PersoonMetDetailsGet(int persoonID)
        {
            using (ChiroGroepClassesDataContext context = new ChiroGroepClassesDataContext())
            {
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<Persoon>(p => p.PersoonsAdres);
                options.LoadWith<PersoonsAdres>(a => a.Adres);
                options.LoadWith<Persoon>(p => p.CommunicatieVorms);

                context.DeferredLoadingEnabled = false;
                context.LoadOptions = options;
                context.ObjectTrackingEnabled = true;

                return (from p in context.Persoons
                        where p.PersoonID == persoonID
                        select p).SingleOrDefault<Persoon>();
            }
        }

        /// <summary>
        /// Ophalen persoonsinfo van alle personen uit een groep
        /// </summary>
        /// <param name="GroepID">ID van de gevraagde groep</param>
        /// <param name="Page">Paginanummer, te beginnen van 0</param>
        /// <param name="PageSize">Aantal records per pagina</param>
        /// <returns>Een lijst met objecten van het type 'vPersoonsInfo'</returns>
        public IList<vPersoonsInfo> GelieerdePersonenInfoGet(int GroepID)
        {
            using (ChiroGroepClassesDataContext context = new ChiroGroepClassesDataContext())
            {
                context.ObjectTrackingEnabled = false;

                var lijst = from pi in context.vPersoonsInfos
                            where pi.GroepID == GroepID
                            select pi;

                return lijst.ToList<vPersoonsInfo>();
            }
        }


        /// <summary>
        /// Persisteert eventuele wijzigingen in het persoonsobject in de database.
        /// Ook wijzigingen in eventuele 'persoonsadressen' worden meegenomen.
        /// </summary>
        /// <param name="persoon">de persoon in kwestie</param>
        /// <returns>ID van de geupdatete persoon (vooral interessant bij insert)</returns>
        public int PersoonUpdaten(Persoon persoon)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            Debug.Assert(persoon != null);

            using (ChiroGroepClassesDataContext context = new ChiroGroepClassesDataContext())
            {
                context.Log = sw;

                if (persoon.PersoonID <= 0)
                {
                    context.Persoons.InsertOnSubmit(persoon);
                }
                else
                {
                    context.Persoons.Attach(persoon, true);
                }

                // Bestaande telefoonnrs opnieuw attachen
                context.CommunicatieVorms.AttachAll<CommunicatieVorm>
                    (from cv in persoon.CommunicatieVorms
                     where cv.CommunicatieVormID > 0
                     select cv, true);

                // Nieuwe telefoonnrs moeten geïnsert worden
                context.CommunicatieVorms.InsertAllOnSubmit<CommunicatieVorm>
                    (from cv in persoon.CommunicatieVorms
                     where cv.CommunicatieVormID <= 0
                     select cv);

                // Verwijderde telefoonnrs
                context.CommunicatieVorms.DeleteAllOnSubmit<CommunicatieVorm>
                    (from cv in persoon.CommunicatieVorms
                     where cv.CommunicatieVormID > 0 && cv.TeVerwijderen == true
                     select cv);
                
                context.SubmitChanges();
            }

            Debug.WriteLine(sb.ToString());
            return persoon.PersoonID;
        }
    }
}
