// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. personen bevat
    /// </summary>
    public class PersonenManager
    {
        private readonly IPersonenDao _dao;
        private readonly IAutorisatieManager _autorisatieMgr;

        /// <summary>
        /// Creëert een PersonenManager
        /// </summary>
        /// <param name="dao">Repository voor personen</param>
        /// <param name="autorisatieMgr">Worker die autorisatie regelt</param>
        public PersonenManager(IPersonenDao dao, IAutorisatieManager autorisatieMgr)
        {
            _dao = dao;
            _autorisatieMgr = autorisatieMgr;
        }

        /// <summary>
        /// Haalt personen op die een adres gemeen hebben met de 
        /// GelieerdePersoon
        /// met gegeven ID
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van 
        /// GelieerdePersoon waarvan huisgenoten
        /// gevraagd zijn</param>
        /// <returns>Lijstje met personen</returns>
        /// <remarks>Parameter: GelieerdePersoonID, return value: Personen!</remarks>
        public IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
        {
            if (_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                // Haal alle huisgenoten op

                IList<Persoon> allen = _dao.HuisGenotenOphalen(gelieerdePersoonID);

                // Welke mag ik zien?

                IList<int> selectie = _autorisatieMgr.EnkelMijnPersonen(
                    (from p in allen select p.ID).ToList());

                // Enkel de geselecteerde personen afleveren.

                var resultaat = from p in allen
                                where selectie.Contains(p.ID)
                                select p;

                return resultaat.ToList();
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Verhuist een persoon van oudAdres naar nieuwAdres.  Persisteert niet.
        /// </summary>
        /// <param name="verhuizer">Te verhuizen GelieerdePersoon</param>
        /// <param name="oudAdres">Oud adres, met personen gekoppeld</param>
        /// <param name="nieuwAdres">Nieuw adres, met personen gekoppeld</param>
        /// <param name="adresType">Adrestype voor de nieuwe koppeling persoon-adres</param>
        /// <remarks>Als de persoon niet gekoppeld is aan het oude adres,
        /// zal hij of zij ook niet verhuizen</remarks>
        public void Verhuizen(Persoon verhuizer, Adres oudAdres, Adres nieuwAdres, AdresTypeEnum adresType)
        {
            Verhuizen(new[] { verhuizer }, oudAdres, nieuwAdres, adresType);
        }

        /// <summary>
        /// Verhuist een persoon van oudAdres naar nieuwAdres.  Persisteert niet.
        /// </summary>
        /// <param name="verhuizers">Te verhuizen personen</param>
        /// <param name="oudAdres">Oud adres, met personen gekoppeld</param>
        /// <param name="nieuwAdres">Nieuw adres, met personen gekoppeld</param>
        /// <param name="adresType">Adrestype voor de nieuwe koppeling persoon-adres</param>
        /// <remarks>Als de persoon niet gekoppeld is aan het oude adres,
        /// zal hij of zij ook niet verhuizen</remarks>
        public void Verhuizen(IEnumerable<Persoon> verhuizers, Adres oudAdres, Adres nieuwAdres, AdresTypeEnum adresType)
        {
            var persIDs = (from p in verhuizers
                           select p.ID).ToArray();
            var mijnPersIDs = _autorisatieMgr.EnkelMijnPersonen(persIDs);

            if (persIDs.Count() == mijnPersIDs.Count())
            {
                // Vind personen waarvan het adres al gekoppeld is.

                var bestaand = verhuizers.SelectMany(p => p.PersoonsAdres.Where(pa => pa.Adres.ID == nieuwAdres.ID));

                if (bestaand.FirstOrDefault() != null)
                {
                    // Geef een exception met daarin de persoonsadresobjecten die al bestaan

                    throw new BlokkerendeObjectenException<PersoonsAdres>(
                        bestaand,
                        bestaand.Count(),
                        Properties.Resources.WonenDaarAl);
                }

                var oudePersoonsAdressen = verhuizers.SelectMany(p => p.PersoonsAdres.Where(pa => pa.Adres.ID == oudAdres.ID));

                foreach (var pa in oudePersoonsAdressen)
                {
                    // verwijder koppeling oud adres->persoonsadres

                    pa.Adres.PersoonsAdres.Remove(pa);

                    // adrestype

                    pa.AdresType = adresType;

                    // koppel persoonsadres aan nieuw adres

                    pa.Adres = nieuwAdres;

                    nieuwAdres.PersoonsAdres.Add(pa);
                }
            }
            else
            {
                // Minstens een persoon waarvoor de user geen GAV is.  Zo'n gepruts verdient
                // een onverbiddellijke geen-gav-exception.

                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Een collectie personen ophalen van wie de ID's opgegeven zijn
        /// </summary>
        /// <param name="personenIDs">De ID's van de personen die in de collectie moeten zitten</param>
        /// <param name="extras">Geeft aan welke gerelateerde entiteiten mee opgehaald moeten worden</param>
        /// <returns>Een collectie met de gevraagde personen</returns>
        public IList<Persoon> LijstOphalen(IEnumerable<int> personenIDs, PersoonsExtras extras)
        {
            var paths = new List<Expression<Func<Persoon, object>>>();

            if ((extras & (PersoonsExtras.Communicatie | PersoonsExtras.Categorieen | PersoonsExtras.Uitstappen)) != 0)
            {
                // niet ondersteund, want dan moeten we eerst nog gaan uitvlooien
                // welke gelieerde persoon we precies nodig hebben.
                throw new NotSupportedException();
            }

            if ((extras & PersoonsExtras.Adressen) != 0)
            {
                paths.Add(p => p.PersoonsAdres.First().Adres);
            }

            if ((extras & PersoonsExtras.VoorkeurAdres) == PersoonsExtras.VoorkeurAdres)
            {
                // voorkeursadres van een (niet-gelieerde) persoon is niet eenduidig.
                throw new NotSupportedException();
            }

            if ((extras & PersoonsExtras.Groep) != 0)
            {
                paths.Add(p => p.GelieerdePersoon.First().Groep);
            }
            else if ((extras & PersoonsExtras.AlleGelieerdePersonen) != 0)
            {
                paths.Add(p => p.GelieerdePersoon);
            }

            if ((extras & PersoonsExtras.GroepsWerkJaren) != 0)
            {
                throw new NotSupportedException();
            }

            // TODO (#1043): dit is nogal veel dubbel werk.  EnkelMijnPersonen laadt alle gelieerde personen,
            // om te kijken welke personen overeen komen met 'mijn' personen.  Daarna worden, indien
            // 'extras|PersoonsExtras.MijnGelieerdePersonen' gezet is, nog eens dezelfde gelieerde
            // persoonsextra's opgehaald.
            return _dao.Ophalen(
                _autorisatieMgr.EnkelMijnPersonen(personenIDs),
                paths.ToArray());
        }

        /// <summary>
        /// Haalt een lijst op van personen, op basis van een lijst <paramref name="gelieerdePersoonIDs"/>.
        /// </summary>
        /// <param name="gelieerdePersoonIDs">ID's van *GELIEERDE* personen, waarvan de corresponderende persoonsobjecten
        /// opgehaald moeten worden.</param>
        /// <param name="extras">Bepaalt welke gekoppelde entiteiten mee opgehaald moeten worden.</param>
        /// <returns>De gevraagde personen</returns>
        public IEnumerable<Persoon> LijstOphalenViaGelieerdePersoon(IEnumerable<int> gelieerdePersoonIDs, PersoonsExtras extras)
        {
            var mijnGelieerdePersonen = _autorisatieMgr.EnkelMijnGelieerdePersonen(gelieerdePersoonIDs);

            var paths = new List<Expression<Func<Persoon, object>>>();

            if ((extras & (PersoonsExtras.Communicatie | PersoonsExtras.Categorieen | PersoonsExtras.Uitstappen)) != 0)
            {
                // niet ondersteund, want dan moeten we eerst nog gaan uitvlooien
                // welke gelieerde persoon we precies nodig hebben.
                throw new NotSupportedException();
            }

            if ((extras & PersoonsExtras.GroepsWerkJaren) != 0)
            {
                throw new NotSupportedException();
            }

            if ((extras & PersoonsExtras.Adressen) != 0)
            {
                paths.Add(p => p.PersoonsAdres.First().Adres);
            }
            else if ((extras & PersoonsExtras.VoorkeurAdres) == PersoonsExtras.VoorkeurAdres)
            {
                // voorkeursadres van een (niet-gelieerde) persoon is niet eenduidig.
                throw new NotSupportedException();
            }

            if ((extras & PersoonsExtras.Groep) != 0)
            {
                paths.Add(p => p.GelieerdePersoon.First().Groep);
            }
            else if ((extras & PersoonsExtras.AlleGelieerdePersonen) != 0)
            {
                paths.Add(p => p.GelieerdePersoon);
            }

            return _dao.OphalenViaGelieerdePersoon(mijnGelieerdePersonen, paths.ToArray());
        }

        /// <summary>
        /// Haalt persoon met gegeven <paramref name="persoonID"/> op
        /// </summary>
        /// <param name="persoonID">ID van op te halen persoon</param>
        /// <returns>opgehaalde persoon</returns>
        /// <remarks>
        /// Voorlopig is dit enkel voor supergavs; de gewone users halen gelieerde personen op.
        /// </remarks>
        public Persoon Ophalen(int persoonID)
        {
            if (_autorisatieMgr.IsSuperGav())
            {
                return _dao.Ophalen(persoonID);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Bewaart de gegeven persoon (voorlopig geen gerelateerde entiteiten)
        /// </summary>
        /// <param name="persoon">Te bewaren persoon</param>
        /// <returns>De bewaarde persoon</returns>
        /// <remarks>
        /// Voorlopig enkel voor 'supergavs'.  In het algemeen worden persoonsmanipulaties via de gelieerde
        /// persoon gedaan.
        /// </remarks>
        public Persoon Bewaren(Persoon persoon)
        {
            if (_autorisatieMgr.IsSuperGav())
            {
                return _dao.Bewaren(persoon);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Verlegt alle referenties van de persoon met ID <paramref name="dubbelID"/> naar de persoon met ID
        /// <paramref name="origineelID"/>, en verwijdert vervolgens de dubbele persoon.
        /// </summary>
        /// <param name="origineelID">ID van de te behouden persoon</param>
        /// <param name="dubbelID">ID van de te verwijderen persoon, die eigenlijk gewoon dezelfde is de te
        /// behouden.</param>
        /// <remarks>Het is niet proper dit soort van logica in de data access te doen.  Anderzijds zou het een 
        /// heel gedoe zijn om dit in de businesslaag te implementeren, omdat er heel wat relaties verlegd moeten worden.
        /// Dat wil zeggen: relaties verwijderen en vervolgens nieuwe maken.  Dit zou een heel aantal 'TeVerwijderens' met zich
        /// meebrengen, wat het allemaal zeer complex zou maken.  Vandaar dat we gewoon via een stored procedure werken.<para />
        /// </remarks>
        public void DubbelVerwijderen(int origineelID, int dubbelID)
        {
            if (_autorisatieMgr.IsSuperGav())
            {
                // Dit gebeurt in data access, omdat het te moeilijk zou worden om de wijzigingen
                // mooi te propageren naar de data access.

                _dao.DubbelVerwijderen(origineelID, dubbelID);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Zoekt in de database personen met een gedeeld AD-nummer, en merget deze.
        /// </summary>
        public void FixGedeeldeAds()
        {
            if (_autorisatieMgr.IsSuperGav())
            {
                foreach (var koppel in _dao.DubbelsZoekenOpBasisVanAd())
                {
                    // Dit gebeurt in data access, omdat het te moeilijk zou worden om de wijzigingen
                    // mooi te propageren naar de data access.

                    _dao.DubbelVerwijderen(koppel.I1, koppel.I2);
                }
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Kent een AD-nummer toe aan een persoon, en persisteert.  Als er al een persoon bestond met
        /// het gegeven AD-nummer, worden de personen gemerged.
        /// </summary>
        /// <param name="persoon">Persoon met toe te kennen AD-nummer</param>
        /// <param name="adNummer">Toe te kennen AD-nummer</param>
        public void AdNummerToekennen(Persoon persoon, int adNummer)
        {
            if (_autorisatieMgr.IsSuperGav())
            {
                // Wie heeft het gegeven AD-nummer al?
                var gevonden = _dao.ZoekenOpAd(adNummer);

                foreach (var p in gevonden.Where(prs => prs.ID != persoon.ID))
                {
                    // Als er andere personen zijn met hetzelfde AD-nummer, 
                    // merge dan met deze persoon.

                    // Door 'persoon.ID' als origineel te kiezen, vermijden
                    // we dat persoon van ID verandert.

                    _dao.DubbelVerwijderen(persoon.ID, p.ID);
                }

                // Tenslotte het echte werk.

                persoon.AdNummer = adNummer;
                _dao.Bewaren(persoon);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Zoekt personen op basis van <paramref name="adNummer"/>.
        /// </summary>
        /// <param name="adNummer">AD-nummer te zoeken personen</param>
        /// <returns>De gevonden persoon, zonder gekoppelde entiteiten</returns>
        /// <remarks>Normaalgezien is er per AD-nummer maar 1 persoon.  Maar voor de zekerheid leveren we toch
        /// een IEnumerable op, voor uitzonderlijke gevallen.</remarks>
        public IEnumerable<Persoon> ZoekenOpAd(int adNummer)
        {
            if (_autorisatieMgr.IsSuperGav())
            {
                return _dao.ZoekenOpAd(adNummer);
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }
    }
}
