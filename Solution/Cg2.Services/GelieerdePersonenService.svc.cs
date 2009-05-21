using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Security.Permissions;

using Cg2.ServiceContracts;
using Cg2.Workers;
using Cg2.Orm;
using Cg2.Fouten.Exceptions;
using Cg2.Fouten.FaultContracts;
using Cg2.Ioc;

namespace Cg2.Services
{
    // NOTE: If you change the class name "GelieerdePersonenService" here, you must also update the reference to "GelieerdePersonenService" in Web.config.
    public class GelieerdePersonenService : IGelieerdePersonenService
    {
        #region IGelieerdePersonenService Members

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<GelieerdePersoon> AllenOphalen(int groepID)
        {
            GelieerdePersonenManager pm = Factory.Maak<GelieerdePersonenManager>();

            var result = pm.AllenOphalen(groepID);
            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<GelieerdePersoon>  PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            GelieerdePersonenManager pm = Factory.Maak<GelieerdePersonenManager>();

            var result = pm.PaginaOphalen(groepID, pagina, paginaGrootte, out aantalOpgehaald);

            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald)
        {
            GelieerdePersonenManager pm = Factory.Maak<GelieerdePersonenManager>();

            return pm.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalOpgehaald);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void PersoonBewaren(GelieerdePersoon persoon)
        {
            GelieerdePersonenManager pm = Factory.Maak<GelieerdePersonenManager>();
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
            GelieerdePersonenManager pm = Factory.Maak<GelieerdePersonenManager>();
            return pm.DetailsOphalen(gelieerdePersoonID);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID, PersoonsInfo gevraagd)
        {
            throw new NotImplementedException();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public Adres AdresMetBewonersOphalen(int adresID)
        {
            AdressenManager adm = Factory.Maak<AdressenManager>();
            return adm.AdresMetBewonersOphalen(adresID);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void Verhuizen(IList<int> gelieerdePersonenIDs, Adres nieuwAdres, int oudAdresID)
        {
            GelieerdePersonenManager pm = Factory.Maak<GelieerdePersonenManager>();
            AuthorisatieManager aum = Factory.Maak<AuthorisatieManager>();
            AdressenManager adm = Factory.Maak<AdressenManager>();

            // Zoek adres op in database, of maak een nieuw.
            // (als straat en gemeente gekend)

            try
            {
                nieuwAdres = adm.ZoekenOfMaken(nieuwAdres);
            }
            catch (AdresException ex)
            {
                throw new FaultException<AdresFault>(ex.Fault);
            }
            catch (Exception)
            {
                // OPMERKING: Deze exceptie is verwacht als je een foute
                // straat of gemeente hebt ingetikt.  Ze wordt gecatcht
                // aan de kant van de UI.  Je kan de applicatie dus gewoon
                // verder laten runnen in Visual Studio; druk hiervoor
                // op F5 :-)

                throw;
            }

            // Om foefelen te vermijden: we werken enkel op de gelieerde
            // personen waar de gebruiker GAV voor is.

            IList<int> mijnGelieerdePersonen = aum.EnkelMijnGelieerdePersonen(gelieerdePersonenIDs);

            // Haal bronadres en alle bewoners op

            Adres oudAdres = adm.AdresMetBewonersOphalen(oudAdresID);

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

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void AdresToevoegen(List<int> gelieerdePersonenIDs, Adres adres)
        {
            // Dit gaat sterk lijken op op verhuizen.

            GelieerdePersonenManager persMgr = Factory.Maak<GelieerdePersonenManager>();
            AdressenManager adrMgr = Factory.Maak<AdressenManager>();

            // Adres opzoeken in database
            try
            {
                adres = adrMgr.ZoekenOfMaken(adres);
            }
            catch (AdresException ex)
            {
                throw new FaultException<AdresFault>(ex.Fault);
            }
            catch (Exception)
            {
                throw;
            }

            // Personen ophalen
            IList<GelieerdePersoon> personenLijst = persMgr.LijstOphalen(gelieerdePersonenIDs);

            // Adres koppelen
            foreach (GelieerdePersoon p in personenLijst)
            {
                persMgr.AdresToevoegen(p, adres);
            }

            // persisteren
            adrMgr.Bewaren(adres);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<GelieerdePersoon> HuisGenotenOphalen(int gelieerdePersoonID)
        {
            // FIXME: enkel huisgenoten ophalen die ook gelieerd zijn aan groepen
            // van aanvrager!

            GelieerdePersonenManager gm = Factory.Maak<GelieerdePersonenManager>();
            return gm.HuisGenotenOphalen(gelieerdePersoonID);
        }

        #endregion
    }
}
