/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkt gebruikersbeheer Copyright 2014, 2015, 2017 Chirojeugd-Vlaanderen vzw
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.Web;
using Chiro.Ad.ServiceContracts;
using Chiro.Cdf.Poco;
using Chiro.Cdf.ServiceHelper;
using Chiro.Cdf.Sso;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Services.Properties;
using Chiro.Gap.WorkerInterfaces;
using GebruikersRecht = Chiro.Gap.ServiceContracts.DataContracts.GebruikersRecht;
using AutoMapper;
#if KIPDORP
using System.Transactions;
#endif

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Interface voor de service voor gebruikersrechtenbeheer.
    /// </summary>
    public class GebruikersService : BaseService, IGebruikersService, IDisposable
    {
        #region Disposable etc

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _repositoryProvider.Dispose();
                }
                disposed = true;
            }
        }

        ~GebruikersService()
        {
            Dispose(false);
        }

        #endregion

        private readonly ServiceHelper _serviceHelper;
        public ServiceHelper ServiceHelper { get { return _serviceHelper; } }

        // Repositories, verantwoordelijk voor data access.
        private readonly IRepositoryProvider _repositoryProvider;
        private readonly IRepository<GebruikersRechtV2> _rechtenRepo;
        private readonly IRepository<GelieerdePersoon> _gelieerdePersonenRepo;
        private readonly IRepository<Persoon> _personenRepo; 

        // Managers voor niet-triviale businesslogica

        private readonly IGebruikersRechtenManager _gebruikersRechtenMgr;
        private readonly IGelieerdePersonenManager _gelieerdePersonenMgr;

        private readonly GavChecker _gav;

        /// <summary>
        /// Nieuwe groepenservice
        /// </summary>
        /// <param name="autorisatieMgr">Verantwoordelijke voor autorisatie</param>
        /// <param name="gebruikersRechtenMgr">Businesslogica aangaande gebruikersrechten</param>
        /// <param name="authenticatieManager">Levert de gebruikersnaam op</param>
        /// <param name="ledenManager">Businesslogica m.b.t. de leden</param>
        /// <param name="groepsWerkJarenManager">Businesslogica m.b.t. de groepswerkjaren.</param>
        /// <param name="gelieerdePersonenManager">Businesslogica i.f.v. gelieerde personen</param>
        /// <param name="abonnementenManager">Businesslogica i.f.v. abonnementen.</param>
        /// <param name="repositoryProvider">De repository provider levert alle nodige repository's op.</param>
        /// <param name="serviceHelper">Service helper die gebruikt zal worden om de active-directory-service aan te spreken.</param>
        public GebruikersService(IAutorisatieManager autorisatieMgr,
                                 IGebruikersRechtenManager gebruikersRechtenMgr,
                                 IAuthenticatieManager authenticatieManager,
                                 ILedenManager ledenManager,
                                 IGroepsWerkJarenManager groepsWerkJarenManager,
                                 IGelieerdePersonenManager gelieerdePersonenManager,
                                 IAbonnementenManager abonnementenManager,
                                 IRepositoryProvider repositoryProvider,
                                 ServiceHelper serviceHelper)
            : base(ledenManager, groepsWerkJarenManager, authenticatieManager, autorisatieMgr, abonnementenManager)
        {
            _repositoryProvider = repositoryProvider;

            _rechtenRepo = repositoryProvider.RepositoryGet<GebruikersRechtV2>();
            _gelieerdePersonenRepo = repositoryProvider.RepositoryGet<GelieerdePersoon>();
            _personenRepo = repositoryProvider.RepositoryGet<Persoon>();

            _gebruikersRechtenMgr = gebruikersRechtenMgr;
            _gelieerdePersonenMgr = gelieerdePersonenManager;

            _gav = new GavChecker(_autorisatieMgr);

            _serviceHelper = serviceHelper;
        }

        public GavChecker Gav
        {
            get { return _gav; }
        }

        /// <summary>
        /// Als de persoon met gegeven <paramref name="gelieerdePersoonId"/> nog geen account heeft, krijgt
        /// hij een account voor zijn eigen groep. Aan die account worden dan de meegegeven 
        /// <paramref name="gebruikersRecht"/> gekoppeld.
        /// </summary>
        /// <param name="gelieerdePersoonId">Id van gelieerde persoon die rechten moet krijgen</param>
        /// <param name="gebruikersRecht">Rechten die de account moet krijgen. Mag leeg zijn. Bestaande 
        /// gebruikersrechten worden zo mogelijk verlengd als ze in <paramref name="gebruikersRecht"/> 
        /// voorkomen, eventuele bestaande rechten niet in <paramref name="gebruikersRecht"/> blijven
        /// onaangeroerd.
        /// </param>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public void RechtenToekennen(int gelieerdePersoonId, GebruikersRecht gebruikersRecht)
        {
            var gelieerdePersoon = _gelieerdePersonenRepo.ByID(gelieerdePersoonId);
            Gav.Check(gelieerdePersoon);

            if (gebruikersRecht == null)
            {
                // Als er geen gebruikersrechten meegegeven zijn, dan geven we de gelieerde persoon
                // rechten 'geen' op zijn eigen groep.
                gebruikersRecht = new GebruikersRecht();
            }

            var p = gelieerdePersoon.Persoon;

            if (p.AdNummer == null)
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.AdNummerVerplicht,
                    Resources.AdNummerVerplicht);
            }
            if (string.IsNullOrEmpty(_gelieerdePersonenMgr.ContactEmail(gelieerdePersoon)))
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.EMailVerplicht, Resources.EmailOntbreekt);
            }

            _gebruikersRechtenMgr.ToekennenOfWijzigen(gelieerdePersoon.Persoon, gelieerdePersoon.Groep,
                gebruikersRecht.PersoonsPermissies, gebruikersRecht.GroepsPermissies, gebruikersRecht.AfdelingsPermissies,
                gebruikersRecht.IedereenPermissies);


#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
            _rechtenRepo.SaveChanges();

            // Zoekt de gegeven gebruiker in active directory. Maakt die gebruiker aan als die nog
            // niet bestaat. En voegt hem/haar toe aan de groep GapGebruikers.

            ServiceHelper.CallService<IAdService, string>(
                svc =>
                    svc.GapLoginAanvragen(p.AdNummer.Value, p.VoorNaam, p.Naam,
                        _gelieerdePersonenMgr.ContactEmail(gelieerdePersoon)));
#if KIPDORP
                tx.Complete();
            }
#endif
        }

        /// <summary>
        /// Neemt de alle gebruikersrechten van de gelieerde persoon met gegeven
        /// <paramref name="persoonID"/> af voor de groepen met gegeven <paramref name="groepIDs"/>
        /// </summary>
        /// <param name="persoonID">Id van persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIDs">Id's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public void RechtenAfnemen(int persoonID, int[] groepIDs)
        {
            var persoon = _personenRepo.ByID(persoonID);
            Gav.Check(persoon);

            if (persoon == null)
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var teExpirenRechten =
                (from g in persoon.GebruikersRechtV2
                    where (g.VervalDatum == null || g.VervalDatum >= DateTime.Today) && groepIDs.Contains(g.Groep.ID)
                    select g).ToList();

            foreach (var gr in teExpirenRechten)
            {
                gr.VervalDatum = DateTime.Today.AddDays(-1);
            }

            _rechtenRepo.SaveChanges();
        }

        /// <summary>
        /// Levert een redirection-url op naar de site van de verzekeraar
        /// </summary>
        /// <returns>Redirection-url naar de site van de verzekeraar</returns>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public string VerzekeringsUrlGet(int groepID)
        {
            int? adNummer = _authenticatieMgr.AdNummerGet();
            GelieerdePersoon mijnGp = null;

            if (adNummer == null)
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var ik = (from p in _personenRepo.Select()
                           where p.AdNummer == adNummer
                           select p).First();

            if (ik != null)
            {
                mijnGp = (from gp in ik.GelieerdePersoon
                              where gp.Groep.ID == groepID
                              select gp).FirstOrDefault();
            }


            if (mijnGp == null)
            {
                throw new FaultException<FoutNummerFault>(new FoutNummerFault
                {
                    Bericht = Resources.KoppelingLoginPersoonOntbreekt,
                    FoutNummer = FoutNummer.KoppelingLoginPersoonOntbreekt
                });
            }


            // haal puntkomma's uit onderdelen, want die zijn straks veldseparatiedingen
            var naam = String.Format("{0} {1}", mijnGp.Persoon.VoorNaam, mijnGp.Persoon.Naam).Replace(';', ',');
            var stamnr = (from gr in ik.GebruikersRechtV2
                          where gr.Groep.ID == groepID
                          select gr.Groep.Code).First().Replace(';', ',');
            string email =
                mijnGp.Communicatie.Where(comm => comm.CommunicatieType.ID == (int) CommunicatieTypeEnum.Email)
                      .OrderByDescending(comm => comm.Voorkeur)
                      .Select(comm => comm.Nummer).FirstOrDefault();

            if (email == null)
            {
                throw new FaultException<FoutNummerFault>(new FoutNummerFault
                                                              {
                                                                  Bericht = Resources.EmailOntbreekt,
                                                                  FoutNummer = FoutNummer.EMailVerplicht
                                                              });
            }

            email = email.Replace(';', ',');

            var cp = new CredentialsProvider(Settings.Default.EncryptieSleutel,
                                             Settings.Default.HashSleutel);

            var credentials = cp.Genereren(String.Format("{0};{1};{2};{3:dd/MM/yyyy H:mm:ss zzz}", naam, stamnr, email, DateTime.Now));

            return String.Format(Settings.Default.UrlVerzekeraar,
                                 HttpUtility.UrlEncode(credentials.GeencrypteerdeUserInfo),
                                 HttpUtility.UrlEncode(credentials.Hash));
        }

        /// <summary>
        /// Indien de ingelogde gebruiker lid is voor gegeven groep in het recentste werkjaar, dan wordt de id van dat lid terug gegeven
        /// </summary>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public int? AangelogdeGebruikerLidIdGet(int groepID)
        {
            int? adNummer = _authenticatieMgr.AdNummerGet();

            var ik = (from p in _personenRepo.Select()
                      where p.AdNummer == adNummer
                      select p).First();

            // Mag ik mijn eigen gegevens lezen?
            if (_autorisatieMgr.MagLezen(ik, ik))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var lps = (from gp in ik.GelieerdePersoon
                          where gp.Groep.ID == groepID
                          select gp).ToList();
            var leden = lps.SelectMany(gp => gp.Lid).ToList();
            if (!leden.Any())
            {
                return null;
            }
            var maxJaar = leden.Max(l => l.GroepsWerkJaar.WerkJaar);
            return leden.First(l => l.GroepsWerkJaar.WerkJaar == maxJaar).ID;
        }

        /// <summary>
        /// Levert de details van de persoon met gegeven <paramref name="adNummer"/>, inclusief gebruikersnaam.
        /// </summary>
        /// <param name="adNummer">AD-nummer van een persoon.</param>
        /// <param name="aanMaken">Als deze <c>true</c> is, wordt een 'stub' aangemaakt als de persoon niet wordt gevonden.</param>
        /// <returns>Details van de persoon met gegeven <paramref name="adNummer"/>.</returns>
        // applying PrincipalPermission at class level doesn't seem to work for a WCF service.
        [PrincipalPermission(SecurityAction.Demand, Role = @"GapServiceConsumers")]
        public GebruikersDetail DetailsOphalen(int adNummer, bool aanMaken)
        {
            int? mijnAdNummer = _authenticatieMgr.AdNummerGet();
            if (mijnAdNummer == null)
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.KoppelingLoginPersoonOntbreekt, String.Format(
                    Resources.KoppelingLoginPersoonOntbreekt,
                    _authenticatieMgr.GebruikersNaamGet(),
                    mijnAdNummer));
            }
            var persoon = (from p in _personenRepo.Select()
                where p.AdNummer == adNummer
                select p).FirstOrDefault();
            var ik = (from p in _personenRepo.Select()
                where p.AdNummer == mijnAdNummer
                select p).FirstOrDefault();

            if (persoon == null && aanMaken)
            {
                // We gaan de persoon registreren. Als hij nog niet bestond, had hij nog geen rechten.
                // TODO: gegevens ophalen uit Civi, ipv leeg te laten (zie #5612).
                persoon = new Persoon
                {
                    // FIXME: stub-naam is niet zo goed. Misschien login erin verwerken?
                    VoorNaam = adNummer.ToString(),
                    Naam = "AD-nummer",
                    AdNummer = mijnAdNummer,
                    Geslacht = GeslachtsType.Onbekend,
                };
                _personenRepo.Add(persoon);
                _personenRepo.SaveChanges();
            }

            // Controleer gebruikersrechten. Ik test ook op AD-nummer, want als ik mezelf net heb
            // aangemaakt loopt het anders fout.
            if (adNummer != mijnAdNummer && !_autorisatieMgr.MagLezen(ik, persoon))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            return Mapper.Map<Persoon, GebruikersDetail>(persoon);
        }
    }
}