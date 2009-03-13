using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.ServiceContracts;
using Cg2.Workers;
using Cg2.Orm;

namespace Cg2.Services
{
    // NOTE: If you change the class name "GelieerdePersonenService" here, you must also update the reference to "GelieerdePersonenService" in Web.config.
    public class GelieerdePersonenService : IGelieerdePersonenService
    {
        public IList<GelieerdePersoon> AllenOphalen(int groepID)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();

            var result = pm.Dao.AllenOphalen(groepID);
            return result;
        }

        public IList<GelieerdePersoon>  PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();

            var result = pm.Dao.PaginaOphalen(groepID, pagina, paginaGrootte, out aantalOpgehaald);

            return result;
        }

        public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();

            var result = pm.Dao.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalOpgehaald);
                          
            return result;
        }

        public void PersoonBewaren(GelieerdePersoon persoon)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();
            pm.Dao.Bewaren(persoon);
        }

        public IList<GelieerdePersoon> zoekPersonen(string naamgedeelte, int pagina, int paginagrootte)
        {
            throw new NotImplementedException();
        }
        //... andere zoekmogelijkheden

        public GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();

            var result = pm.Dao.DetailsOphalen(gelieerdePersoonID);

            return result;
        }

        public GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID, PersoonsInfo gevraagd)
        {
            throw new NotImplementedException();
        }

        public void PersoonVerwijderenUitGroep(int gelieerdePersoonID)
        {
            throw new NotImplementedException();
        }

        public void PersoonAansluitenBijGroep(GelieerdePersoon p)
        {
            throw new NotImplementedException();
        }
    }
}
