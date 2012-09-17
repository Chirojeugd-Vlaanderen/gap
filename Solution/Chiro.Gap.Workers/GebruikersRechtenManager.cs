// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;  // NIET VERWIJDEREN! Nodig voor live (als 'KIPDORP' gedefined is)

using Chiro.Ad.ServiceContracts;
using Chiro.Adf.ServiceModel;
using Chiro.Cdf.Data;
using Chiro.Cdf.Mailer;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Manager die informatie ophaalt over gebruikersrechten van personen waar jij 
    /// gebruikersrecht op hebt.
    /// </summary>
    public class GebruikersRechtenManager : IGebruikersRechtenManager
    {
        private readonly IAutorisatieDao _autorisatieDao;
        private readonly IGavDao _gavDao;
        private readonly IGelieerdePersonenDao _gelieerdePersonenDao;
        private readonly IAutorisatieManager _autorisatieManager;
        private readonly IMailer _mailer;

        /// <summary>
        /// Constructor.  De parameters moeten 'ingevuld' worden via dependency injection.
        /// </summary>
        /// <param name="autorisatieDao">
        /// DAO die gebruikt zal worden om gegevens ivm gebruikersrechten op te zoeken
        /// </param>
        /// <param name="gelieerdePersonenDao">
        /// Data access voor gelieerdepersonen
        /// </param>
        /// <param name="gavDao">
        /// Voorziet data access voor GAV's
        /// </param>
        /// <param name="autorisatieManager">
        /// Data access ivm gebruikersrechten
        /// </param>
        /// <param name="mailer">
        /// Klasse die mails zal sturen
        /// </param>
        public GebruikersRechtenManager(
            IAutorisatieDao autorisatieDao,
            IGelieerdePersonenDao gelieerdePersonenDao,
            IGavDao gavDao,
            IAutorisatieManager autorisatieManager,
            IMailer mailer)
        {
            _autorisatieDao = autorisatieDao;
            _gelieerdePersonenDao = gelieerdePersonenDao;
            _gavDao = gavDao;
            _autorisatieManager = autorisatieManager;
            _mailer = mailer;
        }

        /// <summary>
        /// Als een gelieerde persoon een gebruikersrecht heeft/had voor zijn eigen groep, dan
        /// levert deze call dat gebruikersrecht op, inclusief GAV-object.
        /// </summary>
        /// <param name="gelieerdePersoonID">
        /// ID van een gelieerde persoon
        /// </param>
        /// <returns>
        /// Gebruikersrecht van de gelieerde persoon met ID <paramref name="gelieerdePersoonID"/>
        /// op zijn eigen groep (if any, anders null)
        /// </returns>
        public GebruikersRecht GebruikersRechtGelieerdePersoon(int gelieerdePersoonID)
        {
            if (!_autorisatieManager.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            return _autorisatieDao.GebruikersRechtGelieerdePersoon(gelieerdePersoonID);
        }

        /// <summary>
        /// Verlengt het gegeven <paramref name="gebruikersRecht"/> (indien mogelijk) tot het standaard aantal maanden
        /// na vandaag.
        /// </summary>
        /// <param name="gebruikersRecht">
        /// Te verlengen gebruikersrecht
        /// </param>
        public void Verlengen(GebruikersRecht gebruikersRecht)
        {
            // TODO Nakijken of deze method niet in onderstaande gebruikt kan worden

            if (!_autorisatieManager.IsGavGebruikersRecht(gebruikersRecht.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }
            
            if (!gebruikersRecht.IsVerlengbaar)
            {
                // Als er gebruikersrecht is, maar dat is niet verlengbaar, dan gooien
                // we er een exception tegenaan.
                throw new FoutNummerException(FoutNummer.GebruikersRechtNietVerlengbaar,
                                              Resources.GebruikersRechtNietVerlengbaar);
            }

            // Vervaldatum aanpassen
            gebruikersRecht.VervalDatum = DateTime.Now.AddMonths(Settings.Default.MaandenGebruikersRechtStandaard);
        }

        /// <summary>
        /// Maakt een account voor de gegeven <paramref name="gelieerdePersoon"/>. In principe is het niet nodig
        /// dat de persoon gelieerd is aan een groep, maar we hebben wel een e-mailadres nodig, en laat die nu
        /// net gekoppeld zijn aan gelieerde personen.
        /// Deze method persisteert, want het aanmaken en bewaren van de account in Gap en het aanmaken van de 
        /// account in AD moeten in 1 transactie gebeuren.
        /// </summary>
        /// <param name="gelieerdePersoon">Gelieerde persoon waarvoor een account gezocht of gemaakt moet 
        /// worden. Er wordt verondersteld dat die al gekoppeld is aan zijn accounts. Wat mogelijk een
        /// beetje stom is. Soit.</param>
        /// <returns>GAV-object voor de gelieerde persoon</returns>
        /// <remarks>Gav is eigenlijk een verkeerde naam; het was beter Login geweest.</remarks>
        public Gav AccountZoekenOfMaken(GelieerdePersoon gelieerdePersoon)
        {
            return AccountZoekenOfMaken(gelieerdePersoon, true);
        }

        /// <summary>
        /// Zoekt een account voor de gegeven <paramref name="gelieerdePersoon"/>. Als geen account wordt
        /// gevonden, en <paramref name="makenAlsNietGevonden"/> <c>true</c> is, wordt een nieuwe account 
        /// aangemaakt, en krijgt de gelieerde persoon een e-mail via zijn voorkeursmailadres.
        /// </summary>
        /// <param name="gelieerdePersoon">Gelieerde persoon waarvoor een account gezocht of gemaakt moet 
        /// worden. Er wordt verondersteld dat die al gekoppeld is aan zijn accounts. Wat mogelijk een
        /// beetje stom is. Soit.</param>
        /// <param name="makenAlsNietGevonden"> Indien <c>true</c> wordt een nieuwe account gemaakt als
        /// er geen is gevonden.</param>
        /// <returns>Account voor de gelieerde persoon (klasse Gav zou beter hernoemd worden als account, 
        /// zie #1357)</returns>
        /// <remarks>De gemaakte account heeft geen rechten.</remarks>
        public Gav AccountZoekenOfMaken(GelieerdePersoon gelieerdePersoon, bool makenAlsNietGevonden)
        {
            var p = gelieerdePersoon.Persoon;

            if (!_autorisatieManager.IsGavGelieerdePersoon(gelieerdePersoon.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            var account = p.Gav.FirstOrDefault();

            if (account != null || !makenAlsNietGevonden)
            {
                // Als we al een account gevonden hebben, of als we er geen nieuwe moeten maken, dan zijn we
                // hier al klaar.
                return account;
            }


            // Check op ad-nummer en e-mailadres

            if (p.AdNummer == null)
            {
                throw new FoutNummerException(FoutNummer.AdNummerVerplicht, Properties.Resources.AdNummerVerplicht);
            }

            if (string.IsNullOrEmpty(gelieerdePersoon.ContactEmail))
            {
                throw new FoutNummerException(FoutNummer.EMailVerplicht, Properties.Resources.EMailVerplicht);
            }

            account = new Gav();
            account.Persoon.Add(p);
            p.Gav.Add(account);

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                string username =
                    ServiceHelper.CallService<IAdService, string>(
                        svc => svc.GapLoginAanvragen(p.AdNummer.Value, p.VoorNaam, p.Naam, gelieerdePersoon.ContactEmail));

                account.Login = string.Format(@"CHIROPUBLIC\{0}", username);

                // Het zou kunnen dat de nieuwe login toch al ergens gekend was. Om key violations te vermijden, halen
                // we lelijke-hackgewijze het account-ID op.

                account.ID = _gavDao.IdOphalen(account.Login);

                account = _gavDao.Bewaren(account, act => act.Persoon);

#if KIPDORP
                tx.Complete();
            }
#endif
            return account;

        }

        /// <summary>
        /// Haalt de account op met gegeven <paramref name="gebruikersNaam"/>, met daaraan gekoppeld alle
        /// groepen waar de account gebruikersrechten op heeft.
        /// </summary>
        /// <param name="gebruikersNaam">Gebruikersnaam op te halen account</param>
        /// <returns>Account voor de gelieerde persoon (klasse Gav zou beter hernoemd worden als account, 
        /// zie #1357)</returns>
        public Gav AccountOphalen(string gebruikersNaam)
        {
            // Omdat een account niet echt aan een specifieke groep gebonden is, is het niet te definieren
            // of een gebruiker rechten heeft op die account. Iedereen mag dus elke account ophalen. Met
            // gekoppelde groepen. We gaan er dus maar vanuit dat dat 'publieke' informatie is. Maar koppel
            // dus zeker niet te veel extra informatie aan wat je hier teruggeeft.
            // Het lijkt erop dat hier iets niet klopt. Maar ik zou niet goed weten hoe het te fixen.

            return (_gavDao.Ophalen(gebruikersNaam));
        }

        /// <summary>
        /// Pas de vervaldatum van het gegeven <paramref name="gebruikersRecht"/> aan, zodanig dat
        /// het niet meer geldig is.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRecht">
        /// Te vervallen gebruikersrecht
        /// </param>
        public void Intrekken(GebruikersRecht gebruikersRecht)
        {
            Intrekken(new[] {gebruikersRecht});
        }

        /// <summary>
        /// Pas de vervaldatum van het de <paramref name="gebruikersRechten"/> aan, zodanig dat
        /// ze niet meer geldig zijn.  ZONDER TE PERSISTEREN.
        /// </summary>
        /// <param name="gebruikersRechten">
        /// Te vervallen gebruikersrechten
        /// </param>
        /// <remarks>Gebruikersrechten die al vervallen zijn, blijven onaangeroerd</remarks>
        public void Intrekken(GebruikersRecht[] gebruikersRechten)
        {
            if (!_autorisatieManager.IsGavGebruikersRechten(gebruikersRechten.Select(gr => gr.ID).ToArray()))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            foreach (var gr in gebruikersRechten.Where(r => r.VervalDatum > DateTime.Now))
            {
                gr.VervalDatum = DateTime.Now.AddDays(-1);
            }
        }

        /// <summary>
        /// Persisteert het gegeven <paramref name="gebruikersRecht"/>, zonder enige koppelingen
        /// </summary>
        /// <param name="gebruikersRecht">
        /// Te bewaren gebruikersrecht
        /// </param>
        public void Bewaren(GebruikersRecht gebruikersRecht)
        {
            Bewaren(new [] {gebruikersRecht});
        }

        /// <summary>
        /// Persisteert de gegeven <paramref name="gebruikersRechten"/>, met koppelingen naar account en groep.
        /// </summary>
        /// <param name="gebruikersRechten">Te persisteren gebruikersrechten</param>
        public void Bewaren(GebruikersRecht[] gebruikersRechten)
        {
            var groepIDs = gebruikersRechten.Select(gr => gr.Groep.ID);
            if (!_autorisatieManager.IsGavGroepen(groepIDs))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            _autorisatieDao.Bewaren(gebruikersRechten, gr => gr.Gav.WithoutUpdate(), gr => gr.Groep.WithoutUpdate());
        }

        /// <summary>
        /// Haalt alle gebruikersrechten op uit de groep met ID <paramref name="groepID"/>, inclusief
        /// persoon.
        /// </summary>
        /// <param name="groepID">
        /// ID van de groep waarvan we de gebruikersrechten willen ophalen
        /// </param>
        /// <returns>
        /// Alle gebruikersrechten uit de groep met ID <paramref name="groepID"/>, inclusief
        /// persoon.
        /// </returns>
        public IEnumerable<GebruikersRecht> AllesOphalen(int groepID)
        {
            if (!_autorisatieManager.IsGavGroep(groepID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            var resultaat = _autorisatieDao.AllesOphalen(groepID);

            return resultaat;
        }

        /// <summary>
        /// Haalt een gebruikersrecht op, gegeven zijn <paramref name="gebruikersRechtID"/>,
        /// zonder koppelingen.
        /// </summary>
        /// <param name="gebruikersRechtID">
        /// ID op te halen gebruikersrecht
        /// </param>
        /// <returns>
        /// Het gevraagde gebruikersrecht, zonder koppelingen
        /// </returns>
        public GebruikersRecht Ophalen(int gebruikersRechtID)
        {
            if (!_autorisatieManager.IsGavGebruikersRecht(gebruikersRechtID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            return _autorisatieDao.Ophalen(gebruikersRechtID);
        }

        /// <summary>
        /// Gegeven een <paramref name="groepID"/>, haal van de GAV's waarvan 
        /// de personen gekend zijn, de gelieerde personen op (die dus de
        /// koppeling bepalen tussen de persoon en de groep met gegeven
        /// <paramref name="groepID"/>).
        /// Voorlopig worden ook de communicatiemiddelen mee opgeleverd.
        /// </summary>
        /// <param name="groepID">
        /// ID van een groep
        /// </param>
        /// <returns>
        /// Beschikbare gelieerde personen waarvan we weten dat ze GAV zijn
        /// voor die groep
        /// </returns>
        /// <remarks>
        /// Vervallen GAV's worden niet opgeleverd
        /// </remarks>
        public IEnumerable<GelieerdePersoon> GavGelieerdePersonenOphalen(int groepID)
        {
            if (!(_autorisatieManager.IsSuperGav() || _autorisatieManager.IsGavGroep(groepID)))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            var gavGps = _gelieerdePersonenDao.OphalenOpBasisVanGavs(groepID);

            return (from gp in gavGps
                    where
                        gp.Persoon.Gav.Any(
                            gav =>
                            gav.GebruikersRecht.Any(
                                gr =>
                                gr.Groep.ID == groepID && (gr.VervalDatum == null || gr.VervalDatum > DateTime.Now)))
                    select gp).ToArray();
        }

        /// <summary>
        /// Geeft de gegeven <paramref name="gav"/> gebruikersrechten op de groep van <paramref name="notificatieOntvanger"/>,
        /// en stuurt <paramref name="notificatieOntvanger"/> een mailtje dat jij dat gebruikersrecht hebt gekregen.
        /// (Dit is uiteraard enkel zinvol wanneer je supergav-rechten hebt, en ook gewone gebruikersrechten wil krijgen).
        /// PERSISTEERT, want GAV-maken en mailtje sturen moet in 1 transactie
        /// </summary>
        /// <param name="gav">
        /// GAV die gebruikersrecht moet krijgen, met daaraan gekoppeld de groepen waarop hij/zij
        /// al gebruikersrecht heeft.
        /// </param>
        /// <param name="notificatieOntvanger">
        /// De gelieerde persoon die de groep bepaalt de <paramref name="gav"/> 
        /// rechten voor moet krijgen, en die een mailtje zal krijgen waarin staat dat hij/zij rechten hebt gekregen.  
        /// </param>
        /// <param name="vervalDatum">
        /// Vervaldatum van het nieuwe gebruikersrecht.
        /// </param>
        /// <param name="reden">
        /// Reden waarom het gebruikersrecht aangevraagd werd.  Wordt mee gemaild naar de
        /// <paramref name="notificatieOntvanger"/>
        /// </param>
        /// <returns>
        /// Het gecreëerde gebruikersrecht
        /// </returns>
        /// <remarks>
        /// Als de gebruiker al een gebruikersrecht heeft, wordt enkel de vervaldatum aangepast
        /// </remarks>
        public GebruikersRecht ToekennenOfVerlengen(Gav gav,
                                                    GelieerdePersoon notificatieOntvanger,
                                                    DateTime vervalDatum,
                                                    string reden)
        {
            if (!_autorisatieManager.IsSuperGav())
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            string mailAdres = notificatieOntvanger.ContactEmail;

            if (string.IsNullOrEmpty(mailAdres))
            {
                throw new FoutNummerException(FoutNummer.EMailVerplicht, Resources.EMailVerplicht);
            }

            var gebruikersRecht = ToekennenOfVerlengen(gav, notificatieOntvanger.Groep, vervalDatum);

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
            gebruikersRecht = _autorisatieDao.Bewaren(gebruikersRecht,
                                                      gr => gr.Gav.Persoon.First().WithoutUpdate(),
                                                      gr => gr.Groep.WithoutUpdate());
            _mailer.Verzenden(mailAdres,
                              Resources.TijdelijkeRechtenMailOnderwerp,
                              string.Format(
                                  Resources.TijdelijkeRechtenMailBody,
                                  notificatieOntvanger.Persoon.VolledigeNaam,
                                  gav.Login,
                                  vervalDatum,
                                  reden,
                                  notificatieOntvanger.Groep.ID));

#if KIPDORP
                tx.Complete();
            }
#endif
            return gebruikersRecht;
        }

        /// <summary>
        /// Koppelt de <paramref name="gav"/> aan de <paramref name="groep"/> met gegeven 
        /// <paramref name="vervalDatum"/>.  PERSISTEERT NIET.
        /// </summary>
        /// <param name="gav">
        /// Te koppelen GAV
        /// </param>
        /// <param name="groep">
        /// Groep waaraan te koppelen
        /// </param>
        /// <param name="vervalDatum">
        /// Vervaldatum gebruikersrecht
        /// </param>
        /// <returns>
        /// Deze method is PRIVATE en moet dat ook blijven, want er wordt niet gecheckt
        /// op fouten, en er worden geen notificatiemails gestuurd.  Deze method mag enkel
        /// onrechtstreeks gebruikt worden, via de publieke methods <see name="ToekennenOfVerlengen"/>
        /// </returns>
        private GebruikersRecht ToekennenOfVerlengen(Gav gav, Groep groep, DateTime vervalDatum)
        {
            // Eerst controleren of de groep nog niet aan de gebruiker is gekoppeld
            var gebruikersrecht = (from gr in gav.GebruikersRecht
                                   where gr.Groep.ID == groep.ID
                                   select gr).FirstOrDefault();

            if (gebruikersrecht == null)
            {
                // Nog geen gebruikersrecht.  Maak aan.
                gebruikersrecht = new GebruikersRecht { ID = 0, Gav = gav, Groep = groep };
                gav.GebruikersRecht.Add(gebruikersrecht);
                groep.GebruikersRecht.Add(gebruikersrecht);
            }
            else if (!gebruikersrecht.IsVerlengbaar)
            {
                throw new FoutNummerException(FoutNummer.GebruikersRechtNietVerlengbaar,
                                              Resources.GebruikersRechtNietVerlengbaar);
            }

            gebruikersrecht.VervalDatum = vervalDatum;

            return gebruikersrecht;
        }

        /// <summary>
        /// Haalt de GAV op voor de gebruiker die momenteel is aangemeld.  Als er zo nog geen bestaat, wordt
        /// die aangemaakt.
        /// </summary>
        /// <returns>
        /// GAV, met eventueel gekoppelde groepen
        /// </returns>
        public Gav MijnGavOphalen()
        {
            string login = _autorisatieManager.GebruikersNaamGet();

            var gav = _gavDao.Ophalen(login) ?? new Gav { ID = 0, Login = login };

            return gav;
        }

        /// <summary>
        /// Kent gebruikersrechten toe voor gegeven <paramref name="groep"/> aan gegeven <paramref name="account"/>.
        /// Standaard zijn deze rechten 14 maand geldig. Als de gebruikersrechten al bestonden, worden ze indien
        /// mogelijk verlengd.
        /// </summary>
        /// <param name="account">account die gebruikersrecht moet krijgen op <paramref name="groep"/></param>
        /// <param name="groep">groep waarvoor <paramref name="account"/> gebruikersrecht moet krijgen</param>
        /// <returns>Het gebruikersrecht</returns>
        /// <remarks>Persisteert niet.</remarks>
        public GebruikersRecht ToekennenOfVerlengen(Gav account, Groep groep)
        {
            // Omdat een account niet per se aan een gelieerde persoon gekoppeld is, controleren we enkel of
            // we gebruikersrechten hebben op de groep. Een koppeling account-groep hebben we vooralsnog niet.

            if (!_autorisatieManager.IsGavGroep(groep.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
            return ToekennenOfVerlengen(account, groep,
                                        DateTime.Now.AddMonths(Settings.Default.MaandenGebruikersRechtStandaard));
        }
    }
}