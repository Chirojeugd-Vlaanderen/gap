using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.ServiceContracts;
using Cg2.Workers;
using Cg2.Orm;
using Cg2.Fouten.Exceptions;
using System.Security.Permissions;
using Cg2.Fouten.FaultContracts;

namespace Cg2.Services
{
    // NOTE: If you change the class name "GelieerdePersonenService" here, you must also update the reference to "GelieerdePersonenService" in Web.config.
    public class GelieerdePersonenService : IGelieerdePersonenService
    {
        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<GelieerdePersoon> AllenOphalen(int groepID)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();

            var result = pm.AllenOphalen(groepID);
            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<GelieerdePersoon>  PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();

            var result = pm.PaginaOphalen(groepID, pagina, paginaGrootte, out aantalOpgehaald);

            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();
            AuthorisatieManager am = new AuthorisatieManager();

            if (am.IsGavGroep(ServiceSecurityContext.Current.WindowsIdentity.Name, groepID))
            {
                var result = pm.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalOpgehaald);

                return result;
            }
            else
            {
                throw new GeenGavException("Je bent geen GAV van deze groep.");
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void PersoonBewaren(GelieerdePersoon persoon)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();
            pm.Bewaren(persoon);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<GelieerdePersoon> zoekPersonen(string naamgedeelte, int pagina, int paginagrootte)
        {
            throw new NotImplementedException();
        }
        //... andere zoekmogelijkheden

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();
            AuthorisatieManager am = new AuthorisatieManager();

            if (am.IsGavGelieerdePersoon(ServiceSecurityContext.Current.WindowsIdentity.Name, gelieerdePersoonID))
            {
                var result = pm.DetailsOphalen(gelieerdePersoonID);

                return result;
            }
            else
            {
                throw new GeenGavException("Deze persoon is niet gelieerd aan je groep(en).");
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID, PersoonsInfo gevraagd)
        {
            throw new NotImplementedException();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public Adres AdresMetBewonersOphalen(int adresID)
        {
            AdressenManager adm = new AdressenManager();
            AuthorisatieManager aum = new AuthorisatieManager();


            // TODO: combineer onderstaande calls

            IList<int> groepenLijst = aum.GekoppeldeGroepenGet(ServiceSecurityContext.Current.WindowsIdentity.Name);

            return adm.AdresMetBewonersOphalen(adresID, groepenLijst);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void Verhuizen(IList<int> gelieerdePersonen, Adres nieuwAdres, int oudAdresID)
        {
            GelieerdePersonenManager pm = new GelieerdePersonenManager();
            AuthorisatieManager aum = new AuthorisatieManager();
            AdressenManager adm = new AdressenManager();

            // Zoek adres op in database, of maak een nieuw.
            // (als straat en gemeente gekend)

            try
            {
                nieuwAdres = adm.ZoekenOfMaken(nieuwAdres);
            }
            catch (VerhuisException ex)
            {
                throw new FaultException<VerhuisFault>(ex.Fault);
            }
            catch (Exception)
            {
                // onverwachte exception gewoon opnieuw throwen.
                throw;
            }

            // Om foefelen te vermijden: we werken enkel op de gelieerde
            // personen waar de gebruiker GAV voor is.

            IList<int> mijnGelieerdePersonen = aum.EnkelMijnGelieerdePersonen(gelieerdePersonen, ServiceSecurityContext.Current.WindowsIdentity.Name);

            // Haal bronadres en alle bewoners op

            Adres oudAdres = adm.AdresMetBewonersOphalen(oudAdresID, null);

            // Selecteer enkel bewoners uit mijnGelieerdePersonen

            IList<GelieerdePersoon> teVerhuizen =
                (from PersoonsAdres pa
                in oudAdres.PersoonsAdres
                where mijnGelieerdePersonen.Contains(pa.GelieerdePersoon.ID)
                select pa.GelieerdePersoon).ToList();

            // Bovenstaande query meteen evalueren en resultaten in een lijst.
            // Als ik dat niet doe, dan verandert het 'in' gedeelte van
            // de foreach tijdens de loop, en daar kan .net niet mee
            // lachen.

            foreach (GelieerdePersoon verhuizer in teVerhuizen)
            {
                pm.Verhuizen(verhuizer, oudAdres, nieuwAdres);
            }

            // Persisteren

            adm.Bewaren(nieuwAdres);

            // Bij een verhuis, blijven de PersoonsAdresobjecten dezelfde,
            // maar worden ze aan een ander adres gekoppeld.  Een post
            // van het nieuwe adres (met persoonsadressen) koppelt bijgevolg
            // de persoonsobjecten los van het oude adres.
            // Bijgevolg moet het oudeAdres niet gepersisteerd worden.
        }
    }
}
