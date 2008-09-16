using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CgDal;
using System.Data;

namespace CgBll
{
    /// <summary>
    /// (test)business Class voor Persoon.
    /// </summary>
    public class PersoonBll
    {
        private ChiroGroepEntities context;

        public PersoonBll()
        {
            context = new ChiroGroepEntities();
        }

        /// <summary>
        /// Haal persoonsgegevens op op basis van ID
        /// </summary>
        /// <param name="persoonID">ID van op te halen persoon</param>
        /// <returns>Gedetacht entityobject voor gevraagde persoon</returns>
        public Persoon PersoonGet(int persoonID)
        {
            var q = from p in context.Persoon where p.PersoonID == persoonID 
                    select p;
            var result = q.First();
            context.Detach(result);
            return result;
        }

        /// <summary>
        /// Haal lijst met persoonsadressen van gegeven persoon op, inclusief 
        /// adrestype
        /// </summary>
        /// <param name="persoonID">ID van persoon waarvan adressen opgevraagd 
        /// moeten worden</param>
        /// <returns>een List met adressen</returns>
        public List<PersoonsAdres> PersoonsAdressenGet(int persoonID)
        {
            var q = from a in context.PersoonsAdres 
                    where a.PersoonID == persoonID select a;

            foreach (PersoonsAdres a in q)
            {
                a.AdresTypeReference.Load();
                context.Detach(a);
            }
            return q.ToList();
        }

        /// <summary>
        /// Updatet een persoonsentityobject.  Hiervoor is een oorspronkelijke
        /// niet-gewijzigde kloon van het object nodig.
        /// 
        /// Het gaat als volgt:
        ///  * de oorspronkelijke versie wordt opnieuw aan de datacontext
        ///    geattacht.
        ///  * de gegevens worden overgezet van de gewijzigde persoon naar
        ///    de oorspronkelijke persoon, zodat de datacontext de wijzigingen
        ///    merkt
        ///  * Uiteindelijk bewaart de data context de wijzigingen
        /// </summary>
        /// <param name="bijgewerktePersoon">Persoon met toe te passen 
        /// wijzigingen</param>
        /// <param name="oorspronkelijkePersoon">Oorspronkelijk persoon</param>
        public void PersoonUpdaten(Persoon bijgewerktePersoon
            , Persoon oorspronkelijkePersoon)
        {
            try
            {
                // De oorspronkelijke persoonsgegevens worden geattacht
                // aan de datacontext.

                context.Attach(oorspronkelijkePersoon);

                // De gegevens van de nieuwe persoon worden overgenomen
                // in het oude object, zodat de datacontext deze
                // gegevens bewaart.

                // Eerst de wijzigingen in referenties.
                // (via extension method)
                context.ApplyReferencePropertyChanges(bijgewerktePersoon
                                                      , oorspronkelijkePersoon);

                // Dan de wijzigingen in de 'gewone' properties
                // Dit is een bestaande method van ObjectContext.  De eerste
                // parameter is de naam van de entity set, in dit geval 'Persoon'.
                //
                // Ik ben er niet uit hoe deze method nu weet dat hij de
                // gegevens van oorspronkelijkePersoon moet overschrijven.

                context.ApplyPropertyChanges(
                    bijgewerktePersoon.EntityKey.EntitySetName
                    , bijgewerktePersoon);

                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                // Als het veld 'versie' in de database niet overeenkomt met
                // het veld 'versie' van de oorspronkelijke persoon, dan
                // heeft er ondertussen iemand anders de persoon gewijzigd.
                // Throw de exceptie.
                throw;
            }
        }
    }
}
