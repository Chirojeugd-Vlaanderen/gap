using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;
using Cg2.Fouten.Exceptions;
using System.Diagnostics;

namespace Cg2.Workers
{
    public class GroepenManager
    {
        private IGroepenDao _dao;

        public IGroepenDao Dao
        {
            get { return _dao; }
        }

        /// <summary>
        /// Deze constructor laat toe om een alternatieve repository voor
        /// de groepen te gebruiken.  Nuttig voor mocking en testing.
        /// </summary>
        /// <param name="dao">Alternatieve dao</param>
        public GroepenManager(IGroepenDao dao)
        {
            _dao = dao;
        }

        /// <summary>
        /// Haalt groep op, op basis van GroepID
        /// </summary>
        /// <param name="groepID">ID op te halen groep</param>
        /// <returns>gevraagde groep</returns>
        public Groep Ophalen(int groepID)
        {
            return _dao.Ophalen(groepID);
        }
       
        public void ToevoegenAfdeling(int groepID, string naam, string afkorting)
        {            
            Dao.AfdelingCreeren(groepID, naam, afkorting);
        }

        public void ToevoegenAfdelingsJaar(Groep g, Afdeling a, OfficieleAfdeling oa, int geboortejaarbegin, int geboortejaareind)
        {
            if (geboortejaarbegin < System.DateTime.Today.Year - 20
                || geboortejaarbegin > geboortejaareind
                || geboortejaareind > System.DateTime.Today.Year - 5)
            {
                throw new InvalidOperationException("Ongeldige geboortejaren voor het afdelingsjaar");
            }
            Dao.AfdelingsJaarCreeren(g, a, oa, geboortejaarbegin, geboortejaareind);
        }

        public IList<OfficieleAfdeling> OphalenOfficieleAfdelingen()
        {
            return Dao.OphalenOfficieleAfdelingen();
        }

        // haalt alle AfdelingsJaren op bij een gegeven Groep en GroepsWerkJaar
        // Groep en GroepsWerkJaar zijn allebei parameters omdat GroepsWerkJaar.Groep soms null is
        // maakt gebruik van OphalenMetAfdelingen, filtert dan de afdelingsjaren
        // die overeenkomen met het gegeven GroepsWerkJaar
        //
        // TODO: Als GroepsWerkJaar.Groep null is, dan moet deze functie maar zorgen dat
        // GroepsWerkJaar.Groep opgevraagd wordt.  We moeten vermijden da een groep een afdeling
        // toevoegt aan een groepswerkjaar van een andere groep!
        // TODO: Authorisatie!
        public IList<AfdelingsJaar> OphalenAfdelingsJaren(Groep groep, GroepsWerkJaar gwj)
        {
            IList<AfdelingsJaar> result = new List<AfdelingsJaar>();
            Groep g = Dao.OphalenMetAfdelingen(groep.ID);

            int afdelingCnt = g.Afdeling.Count;

            foreach (Afdeling a in g.Afdeling) {

                int jaarCnt = a.AfdelingsJaar.Count;

                foreach (AfdelingsJaar j in a.AfdelingsJaar) {
                    if (j.GroepsWerkJaar.ID == gwj.ID) {
                        result.Add(j);
                    }
                }
            }
            return result;
        }

        public IList<Afdeling> OphalenEigenAfdelingen(int groep)
        {
            return Dao.OphalenEigenAfdelingen(groep);
        }


        /// <summary>
        /// Testfunctie die standaardafdelingen en afdelingsjaren
        /// aanmaakt voor groep 310.
        ///
        /// TODO: generiek maken of verwijderen
        /// </summary>
        public void ToevoegenAfdelingenEnAfdelingsJaren()
        {
            /*ToevoegenAfdeling(310, "Ribbels", "RI");
            ToevoegenAfdeling(310, "Speelclub", "SP");
            ToevoegenAfdeling(310, "Rakwi's", "RA");
            ToevoegenAfdeling(310, "Tito's", "TI");
            ToevoegenAfdeling(310, "Keti's", "KE");
            ToevoegenAfdeling(310, "Aspi's", "AS");*/

            Groep g = Dao.Ophalen(310);
            IList<Afdeling> eigen = OphalenEigenAfdelingen(310);
            IList<OfficieleAfdeling> off = OphalenOfficieleAfdelingen();
            foreach (Afdeling a in eigen)
            {
                foreach (OfficieleAfdeling o in off)
                {
                    if (a.AfdelingsNaam.Length > 2 && o.Naam.StartsWith(a.AfdelingsNaam.Substring(0, 2)))
                    {
                        int start = System.DateTime.Today.Year, eind = System.DateTime.Today.Year;
                        if (o.Naam.StartsWith("Ri"))
                        {
                            start -= 7;
                            eind -= 6;
                        }
                        else if (o.Naam.StartsWith("Sp"))
                        {
                            start -= 9;
                            eind -= 8;
                        }
                        else if (o.Naam.StartsWith("Ra"))
                        {
                            start -= 11;
                            eind -= 10;
                        }
                        else if (o.Naam.StartsWith("Ti"))
                        {
                            start -= 13;
                            eind -= 12;
                        }
                        else if (o.Naam.StartsWith("Ke"))
                        {
                            start -= 15;
                            eind -= 14;
                        }
                        else if (o.Naam.StartsWith("As"))
                        {
                            start -= 17;
                            eind -= 16;
                        }
                        ToevoegenAfdelingsJaar(g, a, o, start, eind);
                    }
                }
            }
        }


        /// <summary>
        /// Haalt recentste groepswerkjaar op voor gegeven groep
        /// </summary>
        /// <param name="p">ID van gegeven groep</param>
        /// <returns>Gevraagde groepswerkjaar</returns>
        public GroepsWerkJaar RecentsteGroepsWerkJaarGet(Groep g)
        {
            return _dao.RecentsteGroepsWerkJaarGet(g.ID);
        }
    }
}
