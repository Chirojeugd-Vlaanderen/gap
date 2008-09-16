using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CgDal;
using System.Data;
using System.Diagnostics;

namespace CgBll
{
    /// <summary>
    /// business layer class voor PersoonsAdres (test)
    /// </summary>
    public class PersoonsAdresBll
    {
        private ChiroGroepEntities context;

        /// <summary>
        /// Standaardconstructor; creert context
        /// </summary>
        public PersoonsAdresBll()
        {
            context = new ChiroGroepEntities();
        }

        public void PersoonsAdresUpdaten(PersoonsAdres bijgewerktAdres
            , PersoonsAdres oorspronkelijkAdres)
        {
            try
            {
                Debug.WriteLine("AdresTypeID van bijgewerktAdres: " + bijgewerktAdres.AdresType.AdresTypeID);

                // De oorspronkelijke gevens opnieuw attachen aan
                // de ObjectContext.

                context.Attach(oorspronkelijkAdres);
                
                // Nieuwe gegevens overzetten naar oorspronkelijkAdres
                // zodat de objectcontext ze opmerkt.

                context.ApplyReferencePropertyChanges(bijgewerktAdres
                    , oorspronkelijkAdres);
                context.ApplyPropertyChanges(bijgewerktAdres.EntityKey.EntitySetName
                    , bijgewerktAdres);
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                throw;
            }
        }
    }
}
