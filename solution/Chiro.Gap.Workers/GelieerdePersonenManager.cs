﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

#if KIPDORP
using System.Transactions;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chiro.Cdf.Poco;
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
        /// Voegt een <paramref name="nieuwePersoon"/> toe aan de gegegeven <paramref name="groep"/>. Als
        /// <paramref name="forceer"/> niet is gezet, wordt een exception opgegooid als er al een gelijkaardige
        /// persoon aan de groep gekoppeld is.
        /// </summary>
        /// <param name="nieuwePersoon">Nieuwe toe te voegen persoon</param>
        /// <param name="groep">Groep waaraan de persoon gelieerd/gekoppeld moet worden</param>
        /// <param name="chiroLeeftijd">Chiroleeftijd van de persoon</param>
        /// <param name="forceer">Als <c>false</c>, dan wordt een exception opgegooid als er al een gelijkaardige
        /// persoon aan de groep gekoppeld is.</param>
        /// <returns>De gelieerde persoon na het koppelen van <paramref name="nieuwePersoon"/> aan <paramref name="groep"/>.</returns>
        public GelieerdePersoon Toevoegen(Persoon nieuwePersoon, Groep groep, int chiroLeeftijd, bool forceer)
        {
            if (!forceer)
            {
                var bestaande = ZoekGelijkaardig(nieuwePersoon, groep);
                if (bestaande != null && bestaande.Any())
                {
                    throw new BlokkerendeObjectenException<GelieerdePersoon>(bestaande, bestaande.Count,
                                                                             Resources.GelijkaardigePersoon);
                }
            }

            var resultaat = new GelieerdePersoon {Persoon = nieuwePersoon, Groep = groep, ChiroLeefTijd = chiroLeeftijd};

            nieuwePersoon.GelieerdePersoon.Add(resultaat);
            groep.GelieerdePersoon.Add(resultaat);

            return resultaat;
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
        /// <param name="groep">
        /// groep waarin te zoeken
        /// </param>
        /// <returns>
        /// Lijstje met gelijkaardige personen
        /// </returns>
        public List<GelieerdePersoon> ZoekGelijkaardig(Persoon persoon, Groep groep)
        {
            var seNaam = ZoekHelper.Soundex(persoon.Naam);
            var seVoornaam = ZoekHelper.Soundex(persoon.VoorNaam);

            var result = from gp in groep.GelieerdePersoon
                         where
                             (seNaam == gp.Persoon.SeNaam && seVoornaam == gp.Persoon.SeVoornaam) ||
                             (seNaam == gp.Persoon.SeVoornaam && seVoornaam == gp.Persoon.SeNaam)
                         select gp;

            return result.ToList();
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

            // Vind personen waaraan het adres al gekoppeld is.

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

                // Maak zo nodig voorkeursadres
                if (gelieerdePersoon.Persoon.PersoonsAdres.Count() == 1)
                {
                    // Eerste adres van de persoon.  Dit moet bij elke gelieerde persoon het voorkeursadres
                    // worden.

                    foreach (var gp2 in gelieerdePersoon.Persoon.GelieerdePersoon)
                    {
                        gp2.PersoonsAdres = pa;
                    }
                }
                else if (voorkeur)
                {
                    gelieerdePersoon.PersoonsAdres = pa;
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