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
        private IDao<AfdelingsJaar> _afdao;

        private IGroepenDao Dao
        {
            get { return _dao; }
        }

        /// <summary>
        /// Deze constructor laat toe om een alternatieve repository voor
        /// de groepen te gebruiken.  Nuttig voor mocking en testing.
        /// </summary>
        /// <param name="dao">Alternatieve dao</param>
        public GroepenManager(IGroepenDao dao, IDao<AfdelingsJaar> afdao)
        {
            _dao = dao;
            _afdao = afdao;
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

        public GroepsWerkJaar RecentsteGroepsWerkJaarGet(int groepID)
        {
            return Dao.RecentsteGroepsWerkJaarGet(groepID);
        }
       
        public void ToevoegenAfdeling(int groepID, string naam, string afkorting)
        {
            Afdeling a = new Afdeling();
            Groep g = _dao.Ophalen(groepID);

            a.AfdelingsNaam = naam;
            a.Afkorting = afkorting;
            a.Groep = g;
            g.Afdeling.Add(a);

            _dao.Bewaren(g);
        }

        public void ToevoegenAfdelingsJaar(Groep g, Afdeling a, OfficieleAfdeling oa, int geboortejaarbegin, int geboortejaareind)
        {
            if (geboortejaarbegin < System.DateTime.Today.Year - 20
                || geboortejaarbegin > geboortejaareind
                || geboortejaareind > System.DateTime.Today.Year - 5)
            {
                throw new InvalidOperationException("Ongeldige geboortejaren voor het afdelingsjaar");
            }

            AfdelingsJaar afdelingsJaar = new AfdelingsJaar();
            GroepsWerkJaar huidigWerkJaar = _dao.RecentsteGroepsWerkJaarGet(g.ID);

            if (!a.Groep.Equals(g))
            {
                throw new InvalidOperationException("Afdeling " + a.AfdelingsNaam + " is geen afdeling van Groep " + g.Naam);
            }

            // TODO: test of de officiele afdeling bestaat, heb
            // ik voorlopig even weggelaten.  Als de afdeling niet
            // bestaat, zal er bij het bewaren toch een exception
            // optreden, aangezien het niet de bedoeling is dat
            // een officiele afdeling bijgemaakt wordt.

            //TODO check if no conflicts with existing afdelingsjaar

            afdelingsJaar.OfficieleAfdeling = oa;
            afdelingsJaar.Afdeling = a;
            afdelingsJaar.GroepsWerkJaar = huidigWerkJaar;
            afdelingsJaar.GeboorteJaarVan = geboortejaarbegin;
            afdelingsJaar.GeboorteJaarTot = geboortejaareind;

            a.AfdelingsJaar.Add(afdelingsJaar);
            oa.AfdelingsJaar.Add(afdelingsJaar);
            huidigWerkJaar.AfdelingsJaar.Add(afdelingsJaar);

            //TODO hier zou ook nog withoutupdate achter de lambdas moeten staan, maar dat is niet mogelijk
            //buiten de chirogroepentities extension
            _afdao.Bewaren(afdelingsJaar, aj => aj.OfficieleAfdeling,
                                     aj => aj.Afdeling,
                                     aj => aj.GroepsWerkJaar);
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


        /// <summary>
        /// Haalt recentste groepswerkjaar op voor gegeven groep
        /// </summary>
        /// <param name="p">ID van gegeven groep</param>
        /// <returns>Gevraagde groepswerkjaar</returns>
        public GroepsWerkJaar RecentsteGroepsWerkJaarGet(Groep g)
        {
            return _dao.RecentsteGroepsWerkJaarGet(g.ID);
        }

        /// <summary>
        /// Ophalen van groep met zijn afdelingen
        /// </summary>
        /// <param name="groepid"></param>
        /// <returns></returns>
        public Groep OphalenMetAfdelingen(int groepid)
        {
            return _dao.Ophalen(groepid, e=>e.Afdeling);
        }

        public Groep OphalenMetCategorieen(int groepID)
        {
            //TODO
            throw new NotImplementedException();
        }

        public Groep OphalenMetFuncties(int groepID)
        {
            //TODO
            throw new NotImplementedException();
        }

        public Groep OphalenMetVrijeVelden(int groepID)
        {
            //TODO
            throw new NotImplementedException();
        }

        public Groep OphalenMetAdressen(int groepID)
        {
            //TODO
            throw new NotImplementedException();
        }

        public Groep Bewaren(Groep g)
        {
            return _dao.Bewaren(g);
        }
    }
}
