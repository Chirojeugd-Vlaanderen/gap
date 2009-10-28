using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Chiro.Gap.Workers;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Cdf.Ioc;
using System.Diagnostics;
using Chiro.Chiro.Gap.ServiceContracts.Mappers;

namespace Chiro.Gap.Services
{
    // NOTE: If you change the class name "GroepenService" here, you must also update the reference to "GroepenService" in Web.config.
    public class GroepenService : IGroepenService
    {
        #region Manager Injection

        private readonly GroepenManager gm;
        private readonly WerkJaarManager wm;
        private readonly AutorisatieManager am;
        private readonly GelieerdePersonenManager gpm = Factory.Maak<GelieerdePersonenManager>();

        public GroepenService(GroepenManager gm, WerkJaarManager wm, AutorisatieManager am, GelieerdePersonenManager gpm)
        {
            this.gm = gm;
            this.wm = wm;
            this.am = am;
            this.gpm = gpm;
        }

        #endregion

        #region IGroepenService Members

        public GroepInfo OphalenInfo(int GroepId)
        {
            var gr = Ophalen(GroepId);
            return GroepInfoMapper.mapGroep(gr);
        }

        public Groep Bewaren(Groep g)
        {
            try
            {
                return gm.Bewaren(g);
            }
            catch (Exception e)
            {
                // TODO: fatsoenlijke exception handling
                throw new FaultException(e.Message, new FaultCode("Optimistic Concurrency Exception"));
            }
        }

        public Groep Ophalen(int groepID)
        {
            var result = gm.Ophalen(groepID);
            return result;
        }

        public Groep OphalenMetAdressen(int groepID)
        {
            var result = gm.OphalenMetAdressen(groepID);
            return result;
        }

        public Groep OphalenMetFuncties(int groepID)
        {
            var result = gm.OphalenMetFuncties(groepID);
            return result;
        }

        public Groep OphalenMetAfdelingen(int groepID)
        {
            var result = gm.OphalenMetAfdelingen(groepID);
            return result;
        }

        public Groep OphalenMetVrijeVelden(int groepID)
        {
            var result = gm.OphalenMetVrijeVelden(groepID);
            return result;
        }

        public int RecentsteGroepsWerkJaarIDGet(int groepID)
        {
            return wm.RecentsteGroepsWerkJaarIDGet(groepID);
        }

        /// <summary>
        /// TODO: Documentatie bijwerken, en naam veranderen in HuidgWerkJaarOphalen
        /// (of iets gelijkaardig; zie coding standaard). 
        /// Deze documentatie is alleszins onvolledig, want ze gaat ervan uit dat groepen
        /// nooit ophouden te bestaan.  Wat moet deze functie teruggeven als de groep
        /// geen werking meer heeft?
        /// 
        /// Geeft het huidige werkjaar van de gegeven groep terug. Dit is gegarandeerd het huidige jaartal wanneer de
        /// huidige dag tussen de deadline voor het nieuwe werkjaar en de begindatum van het volgende werkjaar ligt.
        /// In de tussenperiode hangt het ervan af of de groep de overgang al heeft gemaakt, en dit is te zien aan 
        /// het laatst gemaakte groepswerkjaar
        /// </summary>
        /// <param name="groepID"></param>
        /// <returns></returns>
        public int HuidigWerkJaarGet(int groepID)
        {
            return wm.HuidigWerkJaarGet(groepID);
        }

        public void AanmakenAfdeling(int groepID, string naam, string afkorting)
        {
            gm.AfdelingToevoegen(groepID, naam, afkorting);
        }

        public void AanmakenAfdelingsJaar(Groep g, Afdeling aj, OfficieleAfdeling oa, int geboortejaarbegin, int geboortejaareind)
        {
            gm.AfdelingsJaarToevoegen(g, aj, oa, geboortejaarbegin, geboortejaareind);
        }


        public IList<OfficieleAfdeling> OphalenOfficieleAfdelingen()
        {
            return gm.OfficieleAfdelingenOphalen();
        }

        public IEnumerable<GroepInfo> OphalenMijnGroepen()
        {
            var result = am.GekoppeldeGroepenGet();
            return GroepInfoMapper.mapGroepen(result);
        }

        #endregion

        #region Categorieen

        public IList<GelieerdePersoon> PersonenOphalenUitCategorie(int categorieID)
        {
			return gpm.OphalenCategorie(categorieID).GelieerdePersoon.ToList();
        }

        //TODO een efficiente manier vinden om een bepaalde eigenschap toe te voegen aan een al geladen element.
        //of anders in de workers methoden aanbieden om lambda expressies mee te geven: dan eerst bepalen wat allemaal nodig is, dan 1 keer laden
        //en dan zijn we terug bij het idee om in het object bij te houden wat hij allemaal heeft geladen
        public Groep OphalenMetCategorieen(int groepID)
        {
            var result = gm.OphalenMetCategorieen(groepID);
            return result;
        }

        public void CategorieToevoegen(Categorie c, int groepID)
        {
			gm.CategorieToevoegen(c, groepID);
        }

        public void CategorieVerwijderen(int categorieID)
        {
			gm.CategorieVerwijderen(categorieID);
        }

        public void CategorieAanpassen(Categorie c)
        {
            // Nog niet klaar
			throw new NotImplementedException();
        }

        //TODO moet hier eigenlijk een groep worden meegegeven, of kan die worden afgeleid uit de aanroeper?
        public void PersonenToevoegenAanCategorie(IList<int> gelieerdePersonenIDs, int categorieID)
        {
			gpm.CategorieKoppelen(gelieerdePersonenIDs, categorieID, true);
        }

        public void PersonenVerwijderenUitCategorie(IList<int> gelieerdePersonenIDs, int categorieID)
        {
			gpm.CategorieKoppelen(gelieerdePersonenIDs, categorieID, false);
        }

        #endregion categorieen
    }
}
