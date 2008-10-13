using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace CgDal
{
    /// <summary>
    /// Toegang tot groepsdata in database
    /// </summary>
    public class GroepsDataAccess
    {
        /// <summary>
        /// Gegevens van een ChiroGroep ophalen (incl. groepsgegevens)
        /// </summary>
        /// <param name="GroepID">ID van gevraagde groep</param>
        /// <returns>Een ChiroGroep object met de gevraagde gegevens</returns>
        /// <remarks>Dit werkt niet.  De groepsgegevens komen niet mee.
        /// Gebruik beter 'ChiroGroepGroepGet'.</remarks>
        public ChiroGroep ChiroGroepGet(int groepID)
        {
            using (ChiroGroepClassesDataContext context = new ChiroGroepClassesDataContext())
            {
                DataLoadOptions options = new DataLoadOptions();

                options.LoadWith<ChiroGroep>(g => g.Groep);

                context.ObjectTrackingEnabled = false;
                context.DeferredLoadingEnabled = false;
                context.LoadOptions = options;

                return (from g in context.ChiroGroeps
                              where g.chiroGroepID == groepID
                              select g).Single<ChiroGroep>();

            }
        }

        /// <summary>
        /// Haalt groepsobject op voor een gegeven Chirogroep
        /// </summary>
        /// <param name="groepID">GroepID</param>
        /// <returns>Een groepsobject met de gegevens van zowel
        /// groep als Chirogroep</returns>
        /// <remarks>Deze functie is een workaround, omdat
        /// ChiroGroepGet niet werkt</remarks>
        public Groep ChiroGroepGroepGet(int groepID)
        {
            using (ChiroGroepClassesDataContext context = new ChiroGroepClassesDataContext())
            {
                DataLoadOptions options = new DataLoadOptions();

                options.LoadWith<Groep>(g => g.ChiroGroep);

                context.ObjectTrackingEnabled = false;
                context.DeferredLoadingEnabled = false;
                context.LoadOptions = options;

                return (from g in context.Groeps
                        from cg in context.ChiroGroeps
                        where (g.GroepID == groepID) && (g.GroepID == cg.chiroGroepID)
                        select g).Single<Groep>();

            }
        }

    }
}
