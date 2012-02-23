// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
    /// <summary>
    /// Gegevenstoegangsobject voor afdelingen
    /// </summary>
    public class AfdelingenDao : Dao<Afdeling, ChiroGroepEntities>, IAfdelingenDao
    {
        /// <summary>
        /// Afdeling ophalen op basis van ID.
        /// </summary>
        /// <param name="afdelingID">ID van gewenste afdeling</param>
        /// <returns>Afdeling en gekoppelde Chirogroep</returns>
        public override Afdeling Ophalen(int afdelingID)
        {
            using (var db = new ChiroGroepEntities())
            {
                db.Afdeling.MergeOption = MergeOption.NoTracking;

                var resultaat = (
                                         from Afdeling afd
                                             in db.Afdeling.Include(afd => afd.ChiroGroep)
                                         where afd.ID == afdelingID
                                         select afd).FirstOrDefault();

                // Die hack om ook de Chirogroep op te halen, is niet meer nodig sinds
                // .NET 4

                return resultaat;
            }
        }

        /// <summary>
        /// Haalt de afdelingen van een groep op die niet gebruikt zijn in een gegeven 
        /// groepswerkjaar, op basis van een <paramref name="groepsWerkJaarID"/>
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
        /// opgezocht moeten worden.</param>
        /// <returns>De ongebruikte afdelingen van een groep in het gegeven groepswerkjaar</returns>
        public IList<Afdeling> OngebruikteOphalen(int groepsWerkJaarID)
        {
            using (var db = new ChiroGroepEntities())
            {
                db.Afdeling.MergeOption = MergeOption.NoTracking;

                return (from afdeling in db.Afdeling
                        where afdeling.ChiroGroep.GroepsWerkJaar.Any(gwj => gwj.ID == groepsWerkJaarID)
                        && !afdeling.AfdelingsJaar.Any(afdj => afdj.GroepsWerkJaar.ID == groepsWerkJaarID)
                        select afdeling).ToList();
            }
        }

        /// <summary>
        /// Haalt alle officiele afdelingen op
        /// </summary>
        /// <returns>Lijst officiele afdelingen</returns>
        public IList<OfficieleAfdeling> OfficieleAfdelingenOphalen()
        {
            IList<OfficieleAfdeling> result;
            using (var db = new ChiroGroepEntities())
            {
                result = (from d in db.OfficieleAfdeling
                          select d).ToList();
            }
            return Utility.DetachObjectGraph(result);
        }

        /// <summary>
        /// Haalt de officiele afdeling met ID <paramref name="officieleAfdelingID"/> op.
        /// </summary>
        /// <param name="officieleAfdelingID">ID van de op te halen officiele afdeling.</param>
        /// <returns>Officiele afdeling met ID <paramref name="officieleAfdelingID"/></returns>
        public OfficieleAfdeling OfficieleAfdelingOphalen(int officieleAfdelingID)
        {
            OfficieleAfdeling resultaat;
            using (var db = new ChiroGroepEntities())
            {
                resultaat = (from d in db.OfficieleAfdeling
                             where d.ID == officieleAfdelingID
                             select d).FirstOrDefault();
            }
            return Utility.DetachObjectGraph(resultaat);
        }
    }
}
