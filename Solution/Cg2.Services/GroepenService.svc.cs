using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Workers;
using Cg2.Orm;
using Cg2.ServiceContracts;
using Cg2.Ioc;
using System.Diagnostics;

namespace Cg2.Services
{
    // NOTE: If you change the class name "GroepenService" here, you must also update the reference to "GroepenService" in Web.config.
    public class GroepenService : IGroepenService
    {
        #region IGroepenService Members

        public Groep Bewaren(Groep g)
        {
            GroepenManager gm = Factory.Maak<GroepenManager>();

            try
            {
                return gm.Dao.Bewaren(g);
            }
            catch (Exception e)
            {
                // TODO: fatsoenlijke exception handling

                throw new FaultException(e.Message, new FaultCode("Optimistic Concurrency Exception"));
            }
        }

        public Groep Ophalen(int groepID)
        {
            GroepenManager gm = Factory.Maak<GroepenManager>();

            var result = gm.Dao.Ophalen(groepID);
            return result;
        }

        public Groep OphalenMetAdressen(int groepID)
        {
            GroepenManager gm = Factory.Maak<GroepenManager>();

            var result = gm.Dao.OphalenMetAdressen(groepID);
            return result;
        }

        public Groep OphalenMetCategorieen(int groepID)
        {
            GroepenManager gm = Factory.Maak<GroepenManager>();

            var result = gm.Dao.OphalenMetCategorieen(groepID);
            return result;
        }

        public Groep OphalenMetFuncties(int groepID)
        {
            GroepenManager gm = Factory.Maak<GroepenManager>();

            var result = gm.Dao.OphalenMetFuncties(groepID);
            return result;
        }

        public Groep OphalenMetAfdelingen(int groepID)
        {
            GroepenManager gm = Factory.Maak<GroepenManager>();

            var result = gm.Dao.OphalenMetAfdelingen(groepID);
            return result;
        }

        public Groep OphalenMetVrijeVelden(int groepID)
        {
            GroepenManager gm = Factory.Maak<GroepenManager>();

            var result = gm.Dao.OphalenMetVrijeVelden(groepID);
            return result;
        }

        /// <summary>
        /// Geeft het huidige werkjaar van de gegeven groep terug. Dit is gegarandeerd het huidige jaartal wanneer de
        /// huidige dag tussen de deadline voor het nieuwe werkjaar en de begindatum van het volgende werkjaar ligt.
        /// In de tussenperiode hangt het ervan af of de groep de overgang al heeft gemaakt, en dit is te zien aan 
        /// het laatst gemaakte groepswerkjaar
        /// </summary>
        /// <param name="groepID"></param>
        /// <returns></returns>
        public int OphalenGroepsWerkjaar(int groepID)
        {
            var begindatumnieuwwerkjaar = Properties.Settings.Default.WerkjaarStartNationaal;
            var deadlinenieuwwerkjaar = Properties.Settings.Default.WerkjaarVerplichteOvergang;

            GroepenManager gm = Factory.Maak<GroepenManager>();
            return gm.OphalenGroepsWerkjaar(groepID, begindatumnieuwwerkjaar, deadlinenieuwwerkjaar);
        }

        public void AanmakenAfdeling(int groepID, string naam, string afkorting)
        {
            GroepenManager gm = Factory.Maak<GroepenManager>();

            gm.ToevoegenAfdeling(groepID, naam, afkorting);
        }

        public void AanmakenAfdelingsJaar(Groep g, Afdeling aj, OfficieleAfdeling oa, int geboortejaarbegin, int geboortejaareind)
        {
            GroepenManager gm = Factory.Maak<GroepenManager>();

            gm.ToevoegenAfdelingsJaar(g, aj, oa, geboortejaarbegin, geboortejaareind);
        }


        public IList<OfficieleAfdeling> OphalenOfficieleAfdelingen()
        {
            GroepenManager gm = Factory.Maak<GroepenManager>();
            return gm.OphalenOfficieleAfdelingen();
        }



        public string Hallo()
        {
            return "Hallo";
        }

        #endregion
    }
}
