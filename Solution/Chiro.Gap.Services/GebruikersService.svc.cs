/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Chiro.Ad.ServiceContracts;
using Chiro.Cdf.Poco;
using Chiro.Cdf.ServiceHelper;
using Chiro.Cdf.Sso;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Services.Properties;
using Chiro.Gap.WorkerInterfaces;
using GebruikersRecht = Chiro.Gap.ServiceContracts.DataContracts.GebruikersRecht;
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
        private readonly IRepository<Gav> _rechtenRepo;
        private readonly IRepository<Groep> _groepenRepo;
        private readonly IRepository<GelieerdePersoon> _gelieerdePersonenRepo;
        private readonly IRepository<Gav> _gavRepo; 

        // Managers voor niet-triviale businesslogica

        private readonly IGebruikersRechtenManager _gebruikersRechtenMgr;
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly IAuthenticatieManager _authenticatieMgr;
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
            : base(ledenManager, groepsWerkJarenManager, abonnementenManager)
        {
            _repositoryProvider = repositoryProvider;

            _rechtenRepo = repositoryProvider.RepositoryGet<Gav>();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
            _gelieerdePersonenRepo = repositoryProvider.RepositoryGet<GelieerdePersoon>();
            _gavRepo = repositoryProvider.RepositoryGet<Gav>();

            _gebruikersRechtenMgr = gebruikersRechtenMgr;
            _autorisatieMgr = autorisatieMgr;
            _authenticatieMgr = authenticatieManager;
            _gelieerdePersonenMgr = gelieerdePersonenManager;

            _gav = new GavChecker(_autorisatieMgr);

            _serviceHelper = serviceHelper;
        }

        public GavChecker Gav
        {
            get { return _gav; }
        }

        /// <summary>
        /// Als de persoon met gegeven <paramref name="gelieerdePersoonId"/> nog geen account heeft, wordt er een
        /// account voor gemaakt. Aan die account worden dan de meegegeven <paramref name="gebruikersRechten"/>
        /// gekoppeld.  Gebruikersrechten zijn standaard 14 maanden geldig.
        /// </summary>
        /// <param name="gelieerdePersoonId">Id van gelieerde persoon die rechten moet krijgen</param>
        /// <param name="gebruikersRechten">Rechten die de account moet krijgen. Mag leeg zijn. Bestaande 
        /// gebruikersrechten worden zo mogelijk verlengd als ze in <paramref name="gebruikersRechten"/> 
        /// voorkomen, eventuele bestaande rechten niet in <paramref name="gebruikersRechten"/> blijven
        /// onaangeroerd.
        /// </param>
        public void RechtenToekennen(int gelieerdePersoonId, GebruikersRecht[] gebruikersRechten)
        {
            var gelieerdePersoon = _gelieerdePersonenRepo.ByID(gelieerdePersoonId);
            Gav.Check(gelieerdePersoon);

            var p = gelieerdePersoon.Persoon;
            var account = p.Gav.FirstOrDefault();

            if (account == null)
            {
                if (p.AdNummer == null)
                {
                    throw FaultExceptionHelper.FoutNummer(FoutNummer.AdNummerVerplicht,
                                                          Resources.AdNummerVerplicht);
                }
                if (string.IsNullOrEmpty(_gelieerdePersonenMgr.ContactEmail(gelieerdePersoon)))
                {
                    throw FaultExceptionHelper.FoutNummer(FoutNummer.EMailVerplicht, Resources.EmailOntbreekt);
                }

                account = new Gav();
                account.Persoon.Add(p);
                p.Gav.Add(account);
            }           
            RechtenToekennen(account, gebruikersRechten);

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif
                if (account.ID == 0)
                {
                    Debug.Assert(p.AdNummer != null);

                    // nieuw account
                    string username =
                        ServiceHelper.CallService<IAdService, string>(
                            svc =>
                            svc.GapLoginAanvragen(p.AdNummer.Value, p.VoorNaam, p.Naam,
                                                  _gelieerdePersonenMgr.ContactEmail(gelieerdePersoon)));
                    account.Login = string.Format(@"CHIROPUBLIC\{0}", username);
                }
                _gavRepo.SaveChanges();
#if KIPDORP
                tx.Complete();
            }
#endif
        }

        /// <summary>
        /// Geeft de account met gegeven <paramref name="gebruikersNaam"/> de gegeven
        /// <paramref name="gebruikersRechten"/>.  Gebruikersrechten zijn standaard 14 maanden geldig.
        /// De gegeven accout moet bestaan; we moeten vermijden dat eender welke user zomaar accounts
        /// kan maken voor chiro.wereld.
        /// </summary>
        /// <param name="gebruikersNaam">gebruikersnaam van de account die rechten moet krijgen</param>
        /// <param name="gebruikersRechten">Rechten die de account moet krijgen.
        /// Bestaande gebruikersrechten worden zo mogelijk verlengd als ze in 
        /// <paramref name="gebruikersRechten"/> voorkomen, eventuele bestaande rechten niet in 
        /// <paramref name="gebruikersRechten"/> blijven onaangeroerd.
        /// </param>
        public void RechtenToekennenGebruiker(string gebruikersNaam, GebruikersRecht[] gebruikersRechten)
        {
            var account =
                (from g in _rechtenRepo.Select() where Equals(g.Login, gebruikersNaam) select g).FirstOrDefault();

            RechtenToekennen(account, gebruikersRechten);
        }

        private void RechtenToekennen(Gav account, GebruikersRecht[] gebruikersRechten)
        {
            // TODO: Deze method staat hier zo wat verloren.
            // In principe zou ze beter naar de workers gaan. Maar daar hebben we geen toegang
            // tot _groepenRepo.
            // Beter zou zijn dat de groepen al opgehaald zijn, en er een structuurtje is
            // {Groep, Recht}, maar dat is dan weer nog niet helemaal afgestemd op de database.
            // (zie #1357)

            if (gebruikersRechten == null)
            {
                return;
            }

            // Momenteel ondersteunen we enkel GAV-rollen
            var nietOndersteund = (from gr in gebruikersRechten
                                   where gr.Rol != Rol.Gav
                                   select gr).FirstOrDefault();
            if (nietOndersteund != null)
            {
                throw new NotSupportedException(String.Format(Resources.RolNietOndersteund,
                                                              nietOndersteund.Rol));
            }

            foreach (var groep in gebruikersRechten.Select(recht => _groepenRepo.ByID(recht.GroepID)))
            {
                Gav.Check(groep);
                _gebruikersRechtenMgr.ToekennenOfVerlengen(account, groep);
            }

            _rechtenRepo.SaveChanges();
        }

        /// <summary>
        /// Neemt de alle gebruikersrechten van de gelieerde persoon met gegeven
        /// <paramref name="gelieerdePersoonId"/> af voor de groepen met gegeven <paramref name="groepIds"/>
        /// </summary>
        /// <param name="gelieerdePersoonId">Id van gelieerde persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIds">Id's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        public void RechtenAfnemen(int gelieerdePersoonId, int[] groepIds)
        {
            var gelieerdePersoon = _gelieerdePersonenRepo.ByID(gelieerdePersoonId);
            Gav.Check(gelieerdePersoon);

            var account = gelieerdePersoon.Persoon.Gav.Single();
            RechtenAfnemen(account, groepIds);
        }

        /// <summary>
        /// Neemt de alle gebruikersrechten van de gelieerde persoon met gegeven
        /// <paramref name="gebruikersNaam"/> af voor de groepen met gegeven <paramref name="groepIds"/>
        /// </summary>
        /// <param name="gebruikersNaam">gebruikersnaam van gelieerde persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIds">Id's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        public void RechtenAfnemenGebruiker(string gebruikersNaam, int[] groepIds)
        {
            var account =
                (from g in _rechtenRepo.Select() where Equals(g.Login, gebruikersNaam) select g).FirstOrDefault();
            RechtenAfnemen(account, groepIds);
        }

        private void RechtenAfnemen(Gav account, IEnumerable<int> groepIds)
        {
            Gav.Check(account);
            if (account == null)
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var vervallenrechten =
                (from g in account.GebruikersRecht where groepIds.Contains(g.Groep.ID) select g).ToList();

            foreach (var vervallenrecht in vervallenrechten)
            {
                vervallenrecht.VervalDatum = DateTime.Today.AddDays(-1);
            }

            _rechtenRepo.SaveChanges();
        }

        /// <summary>
        /// Levert een redirection-url op naar de site van de verzekeraar
        /// </summary>
        /// <returns>Redirection-url naar de site van de verzekeraar</returns>
        public string VerzekeringsUrlGet(int groepID)
        {
            string userName = _authenticatieMgr.GebruikersNaamGet();

            var mijnGav = (from g in _gavRepo.Select()
                           where
                               String.Compare(g.Login, userName,
                                              StringComparison.InvariantCultureIgnoreCase) == 0
                           select g).First();

            if (mijnGav.Persoon.Count == 0 || mijnGav.Persoon.First().GelieerdePersoon == null)
            {
                throw new FaultException<FoutNummerFault>(new FoutNummerFault
                {
                    Bericht = Resources.KoppelingGavPersoonOntbreekt,
                    FoutNummer = FoutNummer.KoppelingGavPersoonOntbreekt
                });
            }

            var mijnGp = (from gp in mijnGav.Persoon.First().GelieerdePersoon
                          where gp.Groep.ID == groepID
                          select gp).First();

            // haal puntkomma's uit onderdelen, want die zijn straks veldseparatiedingen
            var naam = String.Format("{0} {1}", mijnGp.Persoon.VoorNaam, mijnGp.Persoon.Naam).Replace(';', ',');
            var stamnr = (from gr in mijnGav.GebruikersRecht
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
        public int? AangelogdeGebruikerLidIdGet(int groepID)
        {
            var userName = _authenticatieMgr.GebruikersNaamGet();
            var gav = (from g in _gavRepo.Select()
                where
                    string.Compare(g.Login, userName,
                        StringComparison.InvariantCultureIgnoreCase) == 0
                select g).First();
            if (!gav.Persoon.Any())
            {
                return null; // geen persoon gevonden
            }
            var lps = (from gp in gav.Persoon.First().GelieerdePersoon
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
    }
}