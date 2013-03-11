// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

#if KIPDORP
using System.Transactions;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

using Adres = Chiro.Gap.Poco.Model.Adres;
using AdresTypeEnum = Chiro.Gap.Domain.AdresTypeEnum;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. gelieerde personen bevat
    /// </summary>
    public class GelieerdePersonenManager : IGelieerdePersonenManager
    {
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly IPersonenSync _personenSync;
        private readonly IAdressenSync _adressenSync;

        public GelieerdePersonenManager(
            IAutorisatieManager autorisatieMgr,
            IPersonenSync personenSync,
            IAdressenSync adressenSync)
        {
            _autorisatieMgr = autorisatieMgr;
            _personenSync = personenSync;
            _adressenSync = adressenSync;
        }
        /// <summary>
        /// Maak een GelieerdePersoon voor gegeven persoon en groep
        /// </summary>
        /// <param name="persoon">
        /// Te liëren persoon
        /// </param>
        /// <param name="groep">
        /// Groep waaraan te liëren
        /// </param>
        /// <param name="chiroLeeftijd">
        /// Chiroleeftijd gelieerde persoon
        /// </param>
        /// <returns>
        /// Een nieuwe GelieerdePersoon
        /// </returns>
        public GelieerdePersoon Koppelen(Persoon persoon, Groep groep, int chiroLeeftijd)
        {
            if (_autorisatieMgr.IsGav(groep))
            {
                var resultaat = new GelieerdePersoon();

                resultaat.Persoon = persoon;
                resultaat.Groep = groep;
                resultaat.ChiroLeefTijd = chiroLeeftijd;

                persoon.GelieerdePersoon.Add(resultaat);
                groep.GelieerdePersoon.Add(resultaat);

                return resultaat;
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }


        /// <summary>
        /// Koppelt een gelieerde persoon aan een categorie, en persisteert dan de aanpassingen
        /// </summary>
        /// <param name="gelieerdePersonen">
        /// Te koppelen gelieerde persoon
        /// </param>
        /// <param name="categorie">
        /// Te koppelen categorie
        /// </param>
        public void CategorieKoppelen(IList<GelieerdePersoon> gelieerdePersonen, Categorie categorie)
        {
            if (gelieerdePersonen == null)
            {
                throw new ArgumentNullException("gelieerdePersonen");
            }
            if (categorie == null)
            {
                throw new ArgumentNullException("categorie");
            }

            // Heeft de gebruiker rechten voor de groep en de categorie?
            if (gelieerdePersonen.Any(x => !_autorisatieMgr.IsGav(x)))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            if (!_autorisatieMgr.IsGav(categorie))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            foreach (GelieerdePersoon x in gelieerdePersonen)
            {
                if (!x.Groep.Equals(categorie.Groep))
                {
                    throw new FoutNummerException(
                        FoutNummer.CategorieNietVanGroep,
                        Resources.FoutieveGroepCategorie);
                }

                x.Categorie.Add(categorie);
                categorie.GelieerdePersoon.Add(x);
            }
        }


        /// <summary>
        /// Zoekt naar gelieerde personen die gelijkaardig zijn aan een gegeven
        /// <paramref name="persoon"/>.
        /// </summary>
        /// <param name="persoon">
        /// Persoon waarmee vergeleken moet worden
        /// </param>
        /// <param name="groepID">
        /// ID van groep waarin te zoeken
        /// </param>
        /// <returns>
        /// Lijstje met gelijkaardige personen
        /// </returns>
        public IList<GelieerdePersoon> ZoekGelijkaardig(Persoon persoon, int groepID)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        /// <summary>
        /// Maakt het persoonsAdres <paramref name="voorkeur"/> het voorkeursadres van de gelieerde persoon
        /// <paramref name="gp"/>
        /// </summary>
        /// <param name="gp">
        /// Gelieerde persoon die een nieuw voorkeursadres moet krijgen
        /// </param>
        /// <param name="voorkeur">
        /// Persoonsadres dat voorkeursadres moet worden van <paramref name="gp"/>.
        /// </param>
        public void VoorkeurInstellen(GelieerdePersoon gp, PersoonsAdres voorkeur)
        {
            VoorkeurInstellen(gp, voorkeur, true);
        }

        /// <summary>
        /// Maakt het persoonsAdres <paramref name="voorkeur"/> het voorkeursadres van de gelieerde persoon
        /// <paramref name="gp"/>
        /// </summary>
        /// <param name="gp">
        /// Gelieerde persoon die een nieuw voorkeursadres moet krijgen
        /// </param>
        /// <param name="voorkeur">
        /// Persoonsadres dat voorkeursadres moet worden van <paramref name="gp"/>.
        /// </param>
        /// <param name="checkGav">
        /// Indien <paramref name="checkGav"/> <c>false</c> is, mag je het voorkeursadres
        /// van een gelieerde persoon ook wijzigen als je geen GAV bent van die gelieerde persoon.  (Je moet altijd
        /// sowieso GAV zijn van het persoonsadres.)  Dat is nodig in uitzonderlijke gevallen,
        /// bijv. als je iemand een eerste adres geeft, moet dat ook het voorkeursadres worden van de gelieerde personen
        /// in andere groepen, ook al ben je geen GAV van deze groepen.
        /// </param>
        /// <remarks>
        /// Deze method is private, en dat moet zo blijven.  Het is niet de bedoeling dat het gemakkelijk is
        /// om zonder GAV-rechten een adresvoorkeur te veranderen.
        /// </remarks>
        private void VoorkeurInstellen(GelieerdePersoon gp, PersoonsAdres voorkeur, bool checkGav)
        {
            Debug.Assert(gp.Persoon != null);
            Debug.Assert(voorkeur.Persoon != null);

            if (checkGav && !_autorisatieMgr.IsGav(gp) || !voorkeur.GelieerdePersoon.Any(e => _autorisatieMgr.IsGav(e.Groep)))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            // Kijk na of gp en voorkeur wel betrekking hebben op dezelfde persoon.
            if (gp.Persoon.ID != voorkeur.Persoon.ID)
            {
                throw new InvalidOperationException(Resources.PersonenKomenNietOvereen);
            }

            if (gp.PersoonsAdres != null)
            {
                // Als het huidige voorkeursadres van de gelieerde persoon gegeven is
                // verwijder dan de gelieerde persoon
                // uit de collectie gelieerde personen die dat voorkeursadres hebben.
                gp.PersoonsAdres.GelieerdePersoon.Remove(gp);

                // noot: Aangezien we identity en equality niet goed geimplementeerd hebben,
                // kunnen we de check of het voorkeursadres mogelijk het bestaande adres is,
                // hier niet betrouwbaar uitvoeren.  Nogal dikwijls werken we met de ID's, 
                // maar dat kan hier niet, omdat in pratkijk een nieuw adres ook het voorkeuradres
                // kan zijn.  (Nieuwe adressen hebben geen geldig ID.)
            }

            gp.PersoonsAdres = voorkeur;
        }

        /// <summary>
        /// Koppelt het gegeven Adres via nieuwe PersoonsAdresObjecten
        /// aan de Personen gekoppeld aan de gelieerde personen <paramref name="gelieerdePersonen"/>.  
        /// Persisteert niet.
        /// </summary>
        /// <param name="gelieerdePersonen">
        /// Gelieerde  die er een adres bij krijgen, met daaraan gekoppeld hun huidige
        /// adressen, en de gelieerde personen waarop de gebruiker GAV-rechten heeft.
        /// </param>
        /// <param name="adres">
        /// Toe te voegen adres
        /// </param>
        /// <param name="adrestype">
        /// Het adrestype (thuis, kot, enz.)
        /// </param>
        /// <param name="voorkeur">
        /// Indien true, wordt het nieuwe adres voorkeursadres van de gegeven gelieerde personen
        /// </param>
        public void AdresToevoegen(IList<GelieerdePersoon> gelieerdePersonen,
                                   Adres adres,
                                   AdresTypeEnum adrestype,
                                   bool voorkeur)
        {
            // TODO (#1042): Dit lijkt nog te hard op AdresToevoegen in PersonenManager
            var gpersIDs = (from p in gelieerdePersonen
                            select p.ID).ToList();
            var mijngPersIDs = _autorisatieMgr.EnkelMijnGelieerdePersonen(gpersIDs);

            if (gpersIDs.Count() != mijngPersIDs.Count())
            {
                // stiekem personen niet gelieerd aan eigen groep bij in lijst opgenomen.  Geen
                // tijd aan verspillen; gewoon een GeenGavException.
                throw new GeenGavException(Resources.GeenGav);
            }

            // Vind personen waaraan het adres al gekoppeld is.
            // (We hebben chance dat we hier in praktijk nooit komen met een nieuw adres, anders
            // zou onderstaande problemen geven.)
            var bestaand =
                gelieerdePersonen.Select(gp => gp.Persoon).SelectMany(
                    p => p.PersoonsAdres.Where(pa => pa.Adres.ID == adres.ID)).ToList();

            if (bestaand.FirstOrDefault() != null)
            {
                // Sommige personen hebben het adres al.  Geef een exception met daarin de
                // betreffende persoonsadres-objecten.
                throw new BlokkerendeObjectenException<PersoonsAdres>(
                    bestaand,
                    bestaand.Count(),
                    Resources.WonenDaarAl);
            }

            // En dan nu het echte werk:
            foreach (var gelieerdePersoon in gelieerdePersonen)
            {
                // Maak PersoonsAdres dat het adres aan de persoon koppelt.
                var pa = new PersoonsAdres { Adres = adres, Persoon = gelieerdePersoon.Persoon, AdresType = adrestype };
                gelieerdePersoon.Persoon.PersoonsAdres.Add(pa);
                adres.PersoonsAdres.Add(pa);

                if (gelieerdePersoon.Persoon.PersoonsAdres.Count() == 1)
                {
                    // Eerste adres van de gelieerde persoon.  Dit moet bij elke gelieerde persoon het voorkeursadres
                    // worden.

                    foreach (var gp2 in gelieerdePersoon.Persoon.GelieerdePersoon)
                    {
                        VoorkeurInstellen(gp2, pa, false);
                        // De extra parameter 'false' laat toe het voorkeursadres te wijzigen van
                        // een gelieerde persoon waarvoor je geen GAV bent.
                    }
                }
                else if (voorkeur)
                {
                    VoorkeurInstellen(gelieerdePersoon, pa);
                }
            }
        }


        /// <summary>
        /// Verwijder persoonsadressen, en persisteer.  Als ergens een voorkeuradres wegvalt, dan wordt een willekeurig
        /// ander adres voorkeuradres van de gelieerde persoon.
        /// </summary>
        /// <param name="persoonsAdressen">
        /// Te verwijderen persoonsadressen
        /// </param>
        /// <remarks>
        /// Deze method staat wat vreemd onder GelieerdePersonenManager, maar past wel voorkeursadressen
        /// van gelieerde personen aan.
        /// </remarks>
        public void AdressenVerwijderen(IEnumerable<PersoonsAdres> persoonsAdressen)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
//            if (!_autorisatieMgr.IsGavPersoonsAdressen(from pa in persoonsAdressen select pa.ID))
//            {
//                throw new GeenGavException();
//            }

//            var personen = from pa in persoonsAdressen select pa.Persoon;
//            var gelieerdePersonen = personen.SelectMany(p => p.GelieerdePersoon);

//            foreach (var pa in gelieerdePersonen.SelectMany(gp => gp.Persoon.PersoonsAdres))
//            {
//                Debug.Assert(!(pa.Adres is BelgischAdres) || ((BelgischAdres)pa.Adres).StraatNaam != null);
//            }

//            // overloop te verwijderen persoonsadressen

//            foreach (PersoonsAdres pa in persoonsAdressen)
//            {
//                if (pa.GelieerdePersoon.FirstOrDefault() != null)
//                {
//                    // persoonsadres is voorkeuradres van sommige gelieerde personen.
//                    // Voor die gelieerde personen moet
//                    // een nieuw voorkeursadres gekozen worden.  Dit gebeurt willekeurig uit de overige
//                    // adressen.  Als er geen andere adressen zijn, is er ook geen voorkeuradres meer.
//                    PersoonsAdres nieuwVoorkeursAdres;

//                    var alleAdressen = pa.GelieerdePersoon.First().Persoon.PersoonsAdres;

//                    // Probeer eerste en laatste adres...

//                    if (alleAdressen.First().ID != pa.ID)
//                    {
//                        nieuwVoorkeursAdres = alleAdressen.First();
//                    }
//                    else if (alleAdressen.Last().ID != pa.ID)
//                    {
//                        nieuwVoorkeursAdres = alleAdressen.Last();
//                    }
//                    else
//                    {
//                        // Als zowel eerste als laatste PersoonsAdres het te verwijderen adres is,
//                        // dan had de persoon maar 1 adres.  Aangezien dat wordt verwijderd, komt er
//                        // geen voorkeursadres.
//                        nieuwVoorkeursAdres = null;
//                    }

//                    foreach (var pineut in pa.GelieerdePersoon.ToArray())
//                    {
//                        if (nieuwVoorkeursAdres == null)
//                        {
//                            pineut.PersoonsAdres = null;

//                            // 'Vergeet' even het voorkeursadres, zodat we geen conflicten krijgen
//                            // bij het bewaren.
//                        }
//                        else
//                        {
//                            VoorkeurInstellen(pineut, nieuwVoorkeursAdres, false);
//                        }
//                    }
//                }

//                pa.TeVerwijderen = true;
//            }

//#if KIPDORP
//            using (var tx = new TransactionScope())
//            {
//#endif

//            foreach (var gp in gelieerdePersonen)
//            {
//                Debug.Assert(gp.PersoonsAdres == null || !(gp.PersoonsAdres.Adres is BelgischAdres) ||
//                             ((BelgischAdres)gp.PersoonsAdres.Adres).StraatNaam != null);
//            }

//            // eerst syncen naar kipadmin (gewoon om debugtechnische redenen; we zitten toch in
//            // een transactie.)
//            // geef nieuwe voorkeursadressen van personen met ad-nummer door aan kipadmin
//            var voorKeursAdressen = from gp in gelieerdePersonen
//                                    where
//                                        (gp.Persoon.AdNummer != null || gp.Persoon.AdInAanvraag) &&
//                                        gp.PersoonsAdres != null
//                                    select gp.PersoonsAdres;

//            _adressenSync.StandaardAdressenBewaren(voorKeursAdressen);

//            // dan bewaren in GAP
//            // bewaar al dan niet aangepaste voorkeursadres
//            _gelieerdePersonenDao.Bewaren(gelieerdePersonen, gp => gp.PersoonsAdres);

//            // verwijder te verwijderen persoonsadres
//            _personenDao.Bewaren(personen, p => p.PersoonsAdres.First().GelieerdePersoon);

//#if KIPDORP
//                tx.Complete();
//            }
//#endif
        }

        /// <summary>
        /// Koppelt de gelieerde personen met gegeven <paramref name="gelieerdePersoonIDs"/> los
        /// van de gegeven <paramref name="categorie"/>
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's van de los te koppelen gelieerde personen</param>
        /// <param name="categorie">Categorie waar de gelieerde personen losgekoppeld van moeten worden</param>
        public void CategorieLoskoppelen(int[] gelieerdePersoonIDs, Categorie categorie)
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }
    }
}