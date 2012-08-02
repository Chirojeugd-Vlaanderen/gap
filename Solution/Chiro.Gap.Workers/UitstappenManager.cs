// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Businesslogica wat betreft uitstappen en bivakken
    /// </summary>
    public class UitstappenManager
    {
        private readonly IGroepsWerkJaarDao _groepsWerkJaarDao;
        private readonly IAutorisatieManager _autorisatieManager;
        private readonly IUitstappenDao _uitstappenDao;
        private readonly IBivakSync _sync;

        /// <summary>
        /// Constructor.  De parameters moeten 'ingevuld' worden via dependency injection.
        /// </summary>
        /// <param name="uitstappenDao">
        /// Data access voor uitstappen
        /// </param>
        /// <param name="groepsWerkJaarDao">
        /// Data access voor groepswerkjaren
        /// </param>
        /// <param name="autorisatieManager">
        /// Businesslogica voor autorisatie
        /// </param>
        /// <param name="sync">
        /// Proxy naar service om bivakaangiftes te syncen met Kipadmin
        /// </param>
        public UitstappenManager(
            IUitstappenDao uitstappenDao,
            IGroepsWerkJaarDao groepsWerkJaarDao,
            IAutorisatieManager autorisatieManager,
            IBivakSync sync)
        {
            _uitstappenDao = uitstappenDao;
            _groepsWerkJaarDao = groepsWerkJaarDao;
            _autorisatieManager = autorisatieManager;
            _sync = sync;
        }

        /// <summary>
        /// Koppelt een uitstap aan een groepswerkjaar.  Dit moet typisch
        /// enkel gebeuren bij een nieuwe uitstap.
        /// </summary>
        /// <param name="uitstap">
        /// Te koppelen uitstap
        /// </param>
        /// <param name="gwj">
        /// Te koppelen groepswerkjaar
        /// </param>
        /// <returns>
        /// <paramref name="uitstap"/>, gekoppeld aan <paramref name="gwj"/>.
        /// </returns>
        public Uitstap Koppelen(Uitstap uitstap, GroepsWerkJaar gwj)
        {
            if (!_autorisatieManager.IsGavGroepsWerkJaar(gwj.ID) || !_autorisatieManager.IsGavUitstap(uitstap.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            if (uitstap.GroepsWerkJaar != null && uitstap.GroepsWerkJaar != gwj)
            {
                throw new BlokkerendeObjectenException<GroepsWerkJaar>(uitstap.GroepsWerkJaar,
                                                                       Resources.UitstapAlGekoppeld);
            }

            if (!_groepsWerkJaarDao.IsRecentste(gwj.ID))
            {
                throw new FoutNummerException(FoutNummer.GroepsWerkJaarNietBeschikbaar, Resources.GroepsWerkJaarVoorbij);
            }

            uitstap.GroepsWerkJaar = gwj;
            gwj.Uitstap.Add(uitstap);
            return uitstap;
        }

        /// <summary>
        /// Haalt de uitstap met gegeven <paramref name="uitstapID"/> op, inclusief
        /// het gekoppelde groepswerkjaar.
        /// </summary>
        /// <param name="uitstapID">
        /// ID op te halen uitstap
        /// </param>
        /// <param name="extras">
        /// TODO (#190): documenteren
        /// </param>
        /// <returns>
        /// De uitstap, met daaraan gekoppeld het groepswerkjaar.
        /// </returns>
        public Uitstap Ophalen(int uitstapID, UitstapExtras extras)
        {
            var paths = new List<Expression<Func<Uitstap, object>>>();

            if (!_autorisatieManager.IsGavUitstap(uitstapID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            if ((extras & UitstapExtras.Groep) == UitstapExtras.Groep)
            {
                paths.Add(u => u.GroepsWerkJaar.Groep);
            }
            else if ((extras & UitstapExtras.GroepsWerkJaar) == UitstapExtras.GroepsWerkJaar)
            {
                paths.Add(u => u.GroepsWerkJaar);
            }

            if ((extras & UitstapExtras.Deelnemers) == UitstapExtras.Deelnemers)
            {
                paths.Add(u => u.Deelnemer.First().GelieerdePersoon.Persoon.WithoutUpdate());
            }

            if ((extras & UitstapExtras.Contact) == UitstapExtras.Contact)
            {
                paths.Add(u => u.ContactDeelnemer.WithoutUpdate());
            }

            if ((extras & UitstapExtras.Plaats) != 0)
            {
                paths.Add(u => u.Plaats.Adres);
            }

            return _uitstappenDao.Ophalen(uitstapID, paths.ToArray());
        }

        /// <summary>
        /// Bewaart de uitstap met het gekoppelde groepswerkjaar
        /// </summary>
        /// <param name="uitstap">
        /// Te bewaren uitstap
        /// </param>
        /// <param name="extras">
        /// Bepaalt de mee te bewaren koppelingen
        /// </param>
        /// <param name="sync">
        /// Als <c>true</c>, wordt de uitstap gesynct naar Kipadmin.
        /// </param>
        /// <returns>
        /// De bewaarde uitstap
        /// </returns>
        /// <remarks>
        /// Groepswerkjaar wordt altijd mee bewaard.
        /// De parameter <paramref name="sync"/> staat erbij om te vermijden dat voor het
        /// bewaren van een koppeling een (ongewijzigd) bivak mee over de lijn moet.
        /// </remarks>
        public Uitstap Bewaren(Uitstap uitstap, UitstapExtras extras, bool sync)
        {
            // kopieer eerst een aantal gekoppelde entiteiten (voor sync straks), want na het bewaren van het bivak zijn we die kwijt...
            var groep = uitstap.GroepsWerkJaar == null ? null : uitstap.GroepsWerkJaar.Groep;
            var plaats = uitstap.Plaats;

            Debug.Assert(uitstap.GroepsWerkJaar != null);

            if (!_autorisatieManager.IsGavUitstap(uitstap.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            if (!_groepsWerkJaarDao.IsRecentste(uitstap.GroepsWerkJaar.ID))
            {
                throw new FoutNummerException(FoutNummer.GroepsWerkJaarNietBeschikbaar, Resources.GroepsWerkJaarVoorbij);
            }

            // Sowieso groepswerkjaar koppelen.
            Debug.Assert(uitstap.GroepsWerkJaar != null);
            var koppelingen = new List<Expression<Func<Uitstap, object>>> { u => u.GroepsWerkJaar.WithoutUpdate() };

            if ((extras & UitstapExtras.Plaats) != 0)
            {
                koppelingen.Add(u => u.Plaats);
            }

            if ((extras & UitstapExtras.Deelnemers) == UitstapExtras.Deelnemers)
            {
                koppelingen.Add(u => u.Deelnemer.First().GelieerdePersoon.WithoutUpdate());
            }

            if ((extras & UitstapExtras.Contact) == UitstapExtras.Contact)
            {
                koppelingen.Add(u => u.ContactDeelnemer.WithoutUpdate());
            }

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
            uitstap = _uitstappenDao.Bewaren(uitstap, koppelingen.ToArray());
            if (uitstap.IsBivak && sync)
            {
                // De 'Bewaren' hierboven heeft tot gevolg dat de groep niet
                // meer gekoppeld is (wegens onderliggende beperkingen van
                // AttachObjectGraph).  Vandaar dat we voor het gemak
                // die groep hier opnieuw koppelen.
                // Idem voor plaats

                // Opgelet.  Dit is inconsistent gedrag van de software :-(
                uitstap.GroepsWerkJaar.Groep = groep;
                uitstap.Plaats = plaats;
                _sync.Bewaren(uitstap);
            }
            else if (sync)
            {
                // Dit om op te vangen dat een bivak afgevinkt wordt als bivak.
                // TODO (#1044): betere manier bedenken
                _sync.Verwijderen(uitstap.ID);
            }

#if KIPDORP
                tx.Complete();
            }
#endif
            return uitstap;
        }

        /// <summary>
        /// Verwijderd de uitstap
        /// </summary>
        /// <param name="uitstapID">
        /// ID van uitstap die te verwijderen is
        /// </param>
        /// <returns>
        /// Verwijderd de uitstap en toont het overzichtsscherm
        /// </returns>
        public bool UitstapVerwijderen(int uitstapID)
        {
            if (!_autorisatieManager.IsGavUitstap(uitstapID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            Uitstap uitstap = _uitstappenDao.Ophalen(uitstapID);
            uitstap.TeVerwijderen = true;
            _uitstappenDao.Bewaren(uitstap);

            return true;
        }

        /// <summary>
        /// Haalt alle uitstappen van een gegeven groep op.
        /// </summary>
        /// <param name="groepID">
        /// ID van de groep
        /// </param>
        /// <param name="inschrijvenMogelijk">
        /// Als dit <c>true</c> is, worden enkel de gegevens opgehaald
        /// van uitstappen waarvoor nog ingeschreven kan worden.
        /// </param>
        /// <returns>
        /// Details van uitstappen
        /// </returns>
        /// <remarks>
        /// Om maar iets te doen, ordenen we voorlopig op einddatum
        /// </remarks>
        public IEnumerable<Uitstap> OphalenVanGroep(int groepID, bool inschrijvenMogelijk)
        {
            if (!_autorisatieManager.IsGavGroep(groepID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            return _uitstappenDao.OphalenVanGroep(groepID, inschrijvenMogelijk).OrderByDescending(u => u.DatumTot);
        }

        /// <summary>
        /// Koppelt een plaats aan een uitstap
        /// </summary>
        /// <param name="uitstap">
        /// Te koppelen uitstap
        /// </param>
        /// <param name="plaats">
        /// Te koppelen plaats
        /// </param>
        /// <returns>
        /// Uitstap, met plaats gekoppeld.  Persisteert niet
        /// </returns>
        public Uitstap Koppelen(Uitstap uitstap, Plaats plaats)
        {
            if (!_autorisatieManager.IsGavUitstap(uitstap.ID) || !_autorisatieManager.IsGavPlaats(plaats.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            plaats.Uitstap.Add(uitstap);
            uitstap.Plaats = plaats;

            return uitstap;
        }

        /// <summary>
        /// Schrijft de gegeven <paramref name="gelieerdePersonen"/> in voor de gegeven
        /// <paramref name="uitstap"/>, al dan niet als <paramref name="logistiekDeelnemer"/>.
        /// </summary>
        /// <param name="uitstap">
        /// Uitstap waarvoor in te schrijven, gekoppeld aan groep
        /// </param>
        /// <param name="gelieerdePersonen">
        /// In te schrijven gelieerde personen, gekoppeld aan groep
        /// </param>
        /// <param name="logistiekDeelnemer">
        /// Als <c>true</c>, dan worden de 
        /// <paramref name="gelieerdePersonen"/> ingeschreven als logistiek deelnemer.
        /// </param>
        public void Inschrijven(Uitstap uitstap,
                                IEnumerable<GelieerdePersoon> gelieerdePersonen,
                                bool logistiekDeelnemer)
        {
            var alleGpIDs = (from gp in gelieerdePersonen select gp.ID).Distinct();
            var mijnGpIDs = _autorisatieManager.EnkelMijnGelieerdePersonen(alleGpIDs);

            if (alleGpIDs.Count() != mijnGpIDs.Count() || !_autorisatieManager.IsGavUitstap(uitstap.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

            Debug.Assert(groepen.Count() > 0); // De gelieerde personen moeten aan een groep gekoppeld zijn.
            Debug.Assert(uitstap.GroepsWerkJaar != null);
            Debug.Assert(uitstap.GroepsWerkJaar.Groep != null);

            // Als er meer dan 1 groep is, dan is er minstens een groep verschillend van de groep
            // van de uitstap (duivenkotenprincipe));););
            if (groepen.Count() > 1 || groepen.First().ID != uitstap.GroepsWerkJaar.Groep.ID)
            {
                throw new FoutNummerException(
                    FoutNummer.UitstapNietVanGroep,
                    Resources.FoutieveGroepUitstap);
            }

            if (!_groepsWerkJaarDao.IsRecentste(uitstap.GroepsWerkJaar.ID))
            {
                throw new FoutNummerException(
                    FoutNummer.GroepsWerkJaarNietBeschikbaar,
                    Resources.GroepsWerkJaarVoorbij);
            }

            // Als er nu nog geen exception gethrowd is, dan worden eindelijk de deelnemers gemaakt.
            // (koppel enkel de gelieerde personen die nog niet aan de uitstap gekoppeld zijn)
            foreach (var gp in gelieerdePersonen.Where(gp => !gp.Deelnemer.Any(d => d.Uitstap.ID == uitstap.ID)))
            {
                var deelnemer = new Deelnemer
                                    {
                                        GelieerdePersoon = gp,
                                        Uitstap = uitstap,
                                        HeeftBetaald = false,
                                        IsLogistieker = logistiekDeelnemer,
                                        MedischeFicheOk = false
                                    };
                gp.Deelnemer.Add(deelnemer);
                uitstap.Deelnemer.Add(deelnemer);
            }
        }

        /// <summary>
        /// Haalt de deelnemers (incl. lidgegevens van het betreffende groepswerkjaar)
        /// van de gegeven uitstap op.
        /// </summary>
        /// <param name="uitstapID">
        /// ID van uitstap waarvan deelnemers op te halen zijn
        /// </param>
        /// <returns>
        /// De deelnemers van de gevraagde uitstap.
        /// </returns>
        public IEnumerable<Deelnemer> DeelnemersOphalen(int uitstapID)
        {
            if (!_autorisatieManager.IsGavUitstap(uitstapID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            return _uitstappenDao.DeelnemersOphalen(uitstapID);
        }

        /// <summary>
        /// Stuurt alle bivakken van werkJaar <paramref name="werkJaar"/> opnieuw naar
        /// kipadmin.
        /// </summary>
        /// <param name="werkJaar">
        /// Het werkJaar waarvan de gegevens opnieuw gesynct moeten worden
        /// </param>
        public void OpnieuwSyncen(int werkJaar)
        {
            if (!_autorisatieManager.IsSuperGav())
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            var alles = _uitstappenDao.AlleBivakkenOphalen(werkJaar);

            foreach (var bivak in alles)
            {
                _sync.Bewaren(bivak);
            }
        }

        /// <summary>
        /// Gaat na welke gegevens er nog ontbreken in de geregistreerde bivakken om van een
        /// geldige bivakaangifte te kunnen spreken.
        /// </summary>
        /// <param name="groepID">
        /// De ID van de groep waar het over gaat.
        /// </param>
        /// <param name="groepsWerkJaar">
        /// Het werkJaar waarvoor de gegevens opgehaald moeten worden.
        /// </param>
        /// <returns>
        /// Een lijstje met opmerkingen/feedback voor de gebruiker, zodat die weet 
        /// of er nog iets extra's ingevuld moet worden.
        /// </returns>
        /// <exception cref="GeenGavException">
        /// Komt voor als de gebruiker op dit moment geen GAV is voor de groep met de opgegeven <paramref name="groepID"/>,
        /// maar ook als de gebruiker geen GAV was/is in het opgegeven <paramref name="groepsWerkJaar"/>.
        /// </exception>
        public BivakAangifteLijstInfo BivakStatusOphalen(int groepID, GroepsWerkJaar groepsWerkJaar)
        {
            if (!_autorisatieManager.IsGavGroepsWerkJaar(groepsWerkJaar.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            if (!_autorisatieManager.IsGavGroep(groepID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            var statuslijst = new BivakAangifteLijstInfo();

            var aangiftesetting = Settings.Default.BivakAangifteStart;
            var aangiftestart = new DateTime(groepsWerkJaar.WerkJaar + 1, aangiftesetting.Month, aangiftesetting.Day);
            if (0 <= aangiftestart.CompareTo(DateTime.Today))
            {
                statuslijst.AlgemeneStatus = BivakAangifteStatus.NogNietVanBelang;
            }
            else
            {
                var bivaklijst = OphalenVanGroep(groepID, false).Where(e => e.IsBivak).ToList();
                bool allesingevuld = true;
                foreach (var uitstap in bivaklijst)
                {
                    var aangifteInfo = new BivakAangifteInfo
                                           {
                                               ID = uitstap.ID,
                                               Omschrijving = uitstap.Naam
                                           };

                    var uitstapmetdetails = Ophalen(uitstap.ID,
                                                    UitstapExtras.Contact | UitstapExtras.Plaats |
                                                    UitstapExtras.GroepsWerkJaar);
                    if (uitstapmetdetails.GroepsWerkJaar.ID != groepsWerkJaar.ID)
                    {
                        continue;
                    }

                    if (uitstapmetdetails.Plaats == null && uitstapmetdetails.ContactDeelnemer == null)
                    {
                        aangifteInfo.Status = BivakAangifteStatus.PlaatsEnPersoonInTeVullen;
                        allesingevuld = false;
                    }
                    else if (uitstapmetdetails.Plaats == null)
                    {
                        aangifteInfo.Status = BivakAangifteStatus.PlaatsInTeVullen;
                        allesingevuld = false;
                    }
                    else if (uitstapmetdetails.ContactDeelnemer == null)
                    {
                        aangifteInfo.Status = BivakAangifteStatus.PersoonInTeVullen;
                        allesingevuld = false;
                    }
                    else
                    {
                        aangifteInfo.Status = BivakAangifteStatus.Ingevuld;
                    }

                    statuslijst.Bivakinfos.Add(aangifteInfo);
                }

                if (statuslijst.Bivakinfos.Count == 0)
                {
                    statuslijst.AlgemeneStatus = BivakAangifteStatus.DringendInTeVullen;
                }
                else
                {
                    statuslijst.AlgemeneStatus = allesingevuld
                                                     ? BivakAangifteStatus.Ingevuld
                                                     : BivakAangifteStatus.DringendInTeVullen;
                }
            }

            return statuslijst;
        }
    }
}