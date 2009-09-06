using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Orm;
using System.Diagnostics;
using Cg2.Fouten.Exceptions;

namespace Cg2.Workers
{
    public class WerkJaarManager
    {

        private IGroepenDao _dao;

        public IGroepenDao Dao
        {
            get { return _dao; }
        }

        #region Constructors

        public WerkJaarManager(IGroepenDao groepenDao)
        {
            _dao = groepenDao;
        }

        #endregion

        /// <summary>
        /// Maakt een nieuw afdelingsjaar op basis van groepswerkjaar,
        /// afdeling en officiele afdeling.
        /// </summary>
        /// <param name="gwj">Groepswerkjaar voor afdelingsjaar</param>
        /// <param name="afd">Afdeling voor afdelingswerkjaar</param>
        /// <param name="oa">Corresponderende officiele afdeling voor afd</param>
        /// <param name="jaarVan">startpunt interval geboortejaren</param>
        /// <param name="jaarTot">eindpunt interval geboortejaren</param>
        /// <returns>Afdelingsjaar met daaraan gekoppeld groepswerkjaar
        /// , afdeling en officiele afdeling.</returns>
        /// <remarks>gwj.Groep en afd.Groep mogen niet null zijn</remarks>
        public AfdelingsJaar AfdelingsJaarMaken(GroepsWerkJaar gwj, Afdeling afd, OfficieleAfdeling oa, int jaarVan, int jaarTot)
        {
            Debug.Assert(gwj.Groep != null);
            Debug.Assert(afd.Groep != null);

            // FIXME: Eigenlijk zou onderstaande if ook moeten
            // werken zonder de .ID's, want ik heb equals geoverload.
            // Maar dat is blijkbaar niet zo evident.

            if (gwj.Groep.ID != afd.Groep.ID)
            {
                throw new FoutieveGroepException("De afdeling is niet gekoppeld aan de groep van het groepswerkjaar.");
            }

            AfdelingsJaar resultaat = new AfdelingsJaar();

            resultaat.GeboorteJaarVan = jaarVan;
            resultaat.GeboorteJaarTot = jaarTot;

            resultaat.GroepsWerkJaar = gwj;
            resultaat.Afdeling = afd;
            resultaat.OfficieleAfdeling = oa;

            gwj.AfdelingsJaar.Add(resultaat);
            afd.AfdelingsJaar.Add(resultaat);
            oa.AfdelingsJaar.Add(resultaat);

            return resultaat;
        }

        /// <summary>
        /// Bepaalt ID van het recentste GroepsWerkJaar gemaakt voor een
        /// gegeven groep.
        /// </summary>
        /// <param name="groepID">ID van Groep</param>
        /// <returns>ID van het recentste GroepsWerkJaar</returns>
        public int RecentsteGroepsWerkJaarIDGet(int groepID)
        {
            return _dao.RecentsteGroepsWerkJaarGet(groepID).ID;
        }

        public int OphalenHuidigGroepsWerkjaar(int groepID)
        {

            var begindatumnieuwwerkjaar = Properties.Settings.Default.WerkjaarStartNationaal;
            var deadlinenieuwwerkjaar = Properties.Settings.Default.WerkjaarVerplichteOvergang;
            var huidigedatum = System.DateTime.Today;

            if (compare(huidigedatum.Day, huidigedatum.Month, begindatumnieuwwerkjaar.Day, begindatumnieuwwerkjaar.Month) < 0)
            {
                return huidigedatum.Year;
            }
            else
            {
                if (compare(deadlinenieuwwerkjaar.Day, deadlinenieuwwerkjaar.Month, huidigedatum.Day, huidigedatum.Month) < 0)
                {
                    return huidigedatum.Year;
                }
                else
                {
                    int werkjaar = _dao.RecentsteGroepsWerkJaarGet(groepID).WerkJaar;
                    Debug.Assert(huidigedatum.Year == werkjaar || werkjaar + 1 == huidigedatum.Year);
                    return werkjaar;
                }
            }
        }


        // WTF???
        private int compare(int dag1, int maand1, int dag2, int maand2)
        {
            if (maand1 < maand2 || (maand1 == maand2 && dag1 < dag2))
            {
                return -1;
            }
            else
            {
                if (maand1 > maand2 || (maand1 == maand2 && dag1 > dag2))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}
