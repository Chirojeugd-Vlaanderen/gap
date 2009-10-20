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
using Chiro.CG2.ServiceContracts.Mappers;

namespace Cg2.Services
{
    // NOTE: If you change the class name "GelieerdePersonenService" here, you must also update the reference to "GelieerdePersonenService" in Web.config.
    public class GelieerdePersonenService : IGelieerdePersonenService
    {
        #region Manager Injection

        private readonly GelieerdePersonenManager gpm;
        private readonly PersonenManager pm;
        private readonly AutorisatieManager aum;
        private readonly AdressenManager adm;
        private readonly GroepenManager gm;
        private readonly CommVormManager cvm;

        public GelieerdePersonenService(GelieerdePersonenManager gpm, PersonenManager pm, AutorisatieManager aum
            , AdressenManager adm, GroepenManager gm, CommVormManager cvm)
        {
            this.gpm = gpm;
            this.pm = pm;
            this.aum = aum;
            this.adm = adm;
            this.gm = gm;
            this.cvm = cvm;
        }

        #endregion

        #region IGelieerdePersonenService Members

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<GelieerdePersoon> AllenOphalen(int groepID)
        {
            var result = gpm.AllenOphalen(groepID);
            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<GelieerdePersoon>  PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            var result = gpm.PaginaOphalen(groepID, pagina, paginaGrootte, out aantalTotaal);
            return result;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<PersoonInfo> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            var gelieerdePersonen = gpm.PaginaOphalenMetLidInfo(groepID, pagina, paginaGrootte, out aantalTotaal);
            return PersoonInfoMapper.mapPersoon(gelieerdePersonen);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public int PersoonBewaren(GelieerdePersoon persoon)
        {
            gpm.Bewaren(persoon);
            return persoon.ID;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public int PersoonAanmaken(GelieerdePersoon persoon, int groepID)
        {
            Groep g = gm.Ophalen(groepID);
            g.GelieerdePersoon.Add(persoon);
            persoon.Groep = g;
            GelieerdePersoon gp = gpm.Bewaren(persoon);
            return gp.ID;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<GelieerdePersoon> zoekPersonen(string naamgedeelte, int pagina, int paginagrootte)
        {
            throw new NotImplementedException();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public GelieerdePersoon PersoonOphalenMetDetails(int gelieerdePersoonID)
        {
            return gpm.DetailsOphalen(gelieerdePersoonID);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public Adres AdresMetBewonersOphalen(int adresID)
        {
            return adm.AdresMetBewonersOphalen(adresID);
        }

        // Verhuizen van een lijst personen
        // FIXME: Deze functie werkt op PersoonID's en niet op
        // GelieerdePersoonID's, en bijgevolg hoort dit eerder thuis
        // in een PersonenService ipv een GelieerdePersonenService.
        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void PersonenVerhuizen(IList<int> personenIDs, Adres nieuwAdres, int oudAdresID)
        {
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
            // Dit gaat sterk lijken op verhuizen.

            // Adres opzoeken in database
            try
            {
                adres = adm.ZoekenOfMaken(adres);
            }
            catch (AdresException ex)
            {
                throw new FaultException<AdresFault>(ex.Fault);
            }

            // Personen ophalen
            IList<Persoon> personenLijst = pm.LijstOphalen(personenIDs);

            // Adres koppelen
            foreach (Persoon p in personenLijst)
            {
                pm.AdresToevoegen(p, adres);
            }

            // persisteren
            adm.Bewaren(adres);

        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void AdresVerwijderenVanPersonen(List<int> personenIDs, int adresID)
        {
            // Adres ophalen, met bewoners voor GAV
            Adres adr = adm.AdresMetBewonersOphalen(adresID);

            IList<PersoonsAdres> teVerwijderen = (from pa in adr.PersoonsAdres
                                                  where personenIDs.Contains(pa.Persoon.ID)
                                                  select pa).ToList();

            foreach (PersoonsAdres pa in teVerwijderen)
            {
                pa.TeVerwijderen = true;
            }

            adm.Bewaren(adr);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
        {
            return pm.HuisGenotenOphalen(gelieerdePersoonID);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void CommVormToevoegenAanPersoon(int gelieerdepersonenID, CommunicatieVorm commvorm, int typeID)
        {
            cvm.CommVormToevoegen(commvorm, gelieerdepersonenID, typeID);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void CommVormVerwijderenVanPersoon(int gelieerdepersonenID, int commvormID)
        {
            cvm.CommVormVerwijderen(commvormID, gelieerdepersonenID);
        }

        ///TODO dit moet gecontroleerd worden!
        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public void AanpassenCommVorm(CommunicatieVorm v)
        {
            cvm.Bewaren(v);
        }

        public CommunicatieVorm ophalenCommVorm(int commvormID)
        {
            return cvm.Ophalen(commvormID);
        }

        public IEnumerable<CommunicatieType> ophalenCommunicatieTypes()
        {
            return cvm.ophalenCommunicatieTypes();
        }

        #endregion

        #region categorieen
        public void CategorieToevoegenAanPersoon(IList<int> gelieerdepersonenIDs, int categorieID)
        {
            gpm.CategorieKoppelen(gelieerdepersonenIDs, categorieID, true);
        }

        public void CategorieVerwijderenVanPersoon(IList<int> gelieerdepersonenIDs, int categorieID)
        {
            gpm.CategorieKoppelen(gelieerdepersonenIDs, categorieID, true);
        }

        public IEnumerable<Categorie> ophalenCategorieen(int groepID)
        {
            return gpm.ophalenCategorieen(groepID);
        }

        #endregion categorieen
    }
}
