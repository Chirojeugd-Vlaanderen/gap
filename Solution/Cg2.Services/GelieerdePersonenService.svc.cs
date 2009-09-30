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
        public IList<GelieerdePersoon>  PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            GelieerdePersonenManager pm = Factory.Maak<GelieerdePersonenManager>();

            var result = pm.PaginaOphalen(groepID, pagina, paginaGrootte, out aantalTotaal);

            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<PersoonInfo> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            GelieerdePersonenManager pm = Factory.Maak<GelieerdePersonenManager>();

            var gelieerdePersonen = pm.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalTotaal);
            return PersoonInfoMapper.mapPersoon(gelieerdePersonen);
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
        public Adres AdresMetBewonersOphalen(int adresID)
        {
            AdressenManager adm = Factory.Maak<AdressenManager>();
            return adm.AdresMetBewonersOphalen(adresID);
        }

        // Verhuizen van een lijst personen
        // FIXME: Deze functie werkt op PersoonID's en niet op
        // GelieerdePersoonID's, en bijgevolg hoort dit eerder thuis
        // in een PersonenService ipv een GelieerdePersonenService.
        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void PersonenVerhuizen(IList<int> personenIDs, Adres nieuwAdres, int oudAdresID)
        {
            PersonenManager pm = Factory.Maak<PersonenManager>();
            AutorisatieManager aum = Factory.Maak<AutorisatieManager>();
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

            IList<int> mijnPersonen = aum.EnkelMijnPersonen(personenIDs);

            // Haal bronadres en alle bewoners op

            Adres oudAdres = adm.AdresMetBewonersOphalen(oudAdresID);

            // Selecteer enkel bewoners uit mijnGelieerdePersonen

            IList<Persoon> teVerhuizen =
                (from PersoonsAdres pa
                in oudAdres.PersoonsAdres
                where mijnPersonen.Contains(pa.Persoon.ID)
                select pa.Persoon).ToList();

            // Bovenstaande query meteen evalueren en resultaten in een lijst.
            // Als ik dat niet doe, dan verandert het 'in' gedeelte van
            // de foreach tijdens de loop, en daar kan .net niet mee
            // lachen.

            foreach (Persoon verhuizer in teVerhuizen)
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
        public void AdresToevoegenAanPersonen(List<int> personenIDs, Adres adres)
        {
            // Dit gaat sterk lijken op op verhuizen.

            PersonenManager persMgr = Factory.Maak<PersonenManager>();
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
                // Als er een FaultException optreedt, is het normaal
                // dat die hier opnieuw gethrowd wordt.  Bij het 
                // debuggen zal de applicatie hier stilvallen.  Druk
                // gewoon F5 om verder te gaan.

                throw;
            }

            // Personen ophalen
            IList<Persoon> personenLijst = persMgr.LijstOphalen(personenIDs);

            // Adres koppelen
            foreach (Persoon p in personenLijst)
            {
                persMgr.AdresToevoegen(p, adres);
            }

            // persisteren
            adrMgr.Bewaren(adres);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void AdresVerwijderenVanPersonen(List<int> personenIDs, int adresID)
        {
            AdressenManager adrMgr = Factory.Maak<AdressenManager>();

            // Adres ophalen, met bewoners voor GAV

            Adres adr = adrMgr.AdresMetBewonersOphalen(adresID);

            IList<PersoonsAdres> teVerwijderen = (from pa in adr.PersoonsAdres
                                                  where personenIDs.Contains(pa.Persoon.ID)
                                                  select pa).ToList();

            foreach (PersoonsAdres pa in teVerwijderen)
            {
                pa.TeVerwijderen = true;
            }

            adrMgr.Bewaren(adr);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
        {
            PersonenManager pm = Factory.Maak<PersonenManager>();

            return pm.HuisGenotenOphalen(gelieerdePersoonID);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void CommVormToevoegenAanPersoon(int gelieerdepersonenID, CommunicatieVorm commvorm)
        {
            GelieerdePersonenManager mgr = Factory.Maak<GelieerdePersonenManager>();

            mgr.CommVormToevoegen(commvorm, gelieerdepersonenID);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void CommVormVerwijderenVanPersoon(int gelieerdepersonenID, int commvormID)
        {
            GelieerdePersonenManager mgr = Factory.Maak<GelieerdePersonenManager>();

            mgr.CommVormVerwijderen(commvormID, gelieerdepersonenID);
        }

        ///TODO dit moet gecontroleerd worden!
        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void AanpassenCommVorm(CommunicatieVorm v)
        {
            GelieerdePersonenManager mgr = Factory.Maak<GelieerdePersonenManager>();

            mgr.BewarenMetCommVormen(v.GelieerdePersoon);
        }

        #endregion
    }
}
