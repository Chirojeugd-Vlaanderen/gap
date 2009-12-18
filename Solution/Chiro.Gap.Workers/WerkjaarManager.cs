using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Workers
{
    public class WerkJaarManager
    {

        private IGroepenDao _dao;
        private IAutorisatieManager _autorisatieMgr;

        public IGroepenDao Dao
        {
            get { return _dao; }
        }

        public WerkJaarManager(IGroepenDao groepenDao, IAutorisatieManager autorisatieMgr)
        {
            _dao = groepenDao;
            _autorisatieMgr = autorisatieMgr;
        }

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
            if (!_autorisatieMgr.IsGavGroepsWerkJaar(gwj.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroepsWerkJaar);
            }
            if (!_autorisatieMgr.IsGavAfdeling(afd.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGavAfdeling);
            }

            Debug.Assert(gwj.Groep != null);
            Debug.Assert(afd.Groep != null);

            // FIXME: Eigenlijk zou onderstaande if ook moeten
            // werken zonder de .ID's, want ik heb equals geoverload.
            // Maar dat is blijkbaar niet zo evident.

            if (!gwj.Groep.Equals(afd.Groep))
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
            if (_autorisatieMgr.IsGavGroep(groepID))
            {
                return _dao.RecentsteGroepsWerkJaarGet(groepID).ID;
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
            }
        }

        /// <summary>
        /// Haalt het huidige werkjaar op (beginjaar) voor een bepaalde groep
        /// </summary>
        /// <param name="groepID">ID van de groep</param>
        /// <returns>beginjaar van het huidige werkjaar voor die bepaalde groep</returns>
        public int HuidigWerkJaarGet(int groepID)
        {
            // TODO: Beter documenteren!

            if (_autorisatieMgr.IsGavGroep(groepID))
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
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGavGroep);
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
