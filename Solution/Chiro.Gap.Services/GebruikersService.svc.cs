/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * Bijgewerkt gebruikersbeheer Copyright 2014 Johan Vervloet
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
﻿using System;
using System.Collections.Generic;
﻿using System.Diagnostics;
﻿using System.Linq;
using System.ServiceModel;
using System.Web;
﻿using Chiro.Ad.ServiceContracts;
﻿using Chiro.Cdf.Poco;
﻿using Chiro.Cdf.ServiceHelper;
﻿using Chiro.Cdf.Sso;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WorkerInterfaces;
#if KIPDORP
using System.Transactions;
#endif

using GebruikersRecht = Chiro.Gap.ServiceContracts.DataContracts.GebruikersRecht;
using Chiro.Gap.ServiceContracts.DataContracts;
using AutoMapper;

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
        private readonly IRepository<Groep> _groepenRepo;
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
        /// <param name="repositoryProvider">De repository provider levert alle nodige repository's op.</param>
        /// <param name="serviceHelper">Service helper die gebruikt zal worden om de active-directory-service aan te spreken.</param>
        public GebruikersService(IAutorisatieManager autorisatieMgr,
                                 IGebruikersRechtenManager gebruikersRechtenMgr,
                                 IAuthenticatieManager authenticatieManager,
                                 ILedenManager ledenManager,
                                 IGroepsWerkJarenManager groepsWerkJarenManager,
                                 IGelieerdePersonenManager gelieerdePersonenManager,
                                 IRepositoryProvider repositoryProvider,
                                 ServiceHelper serviceHelper): base(ledenManager, groepsWerkJarenManager, authenticatieManager, autorisatieMgr)
        {
            _repositoryProvider = repositoryProvider;

            _rechtenRepo = repositoryProvider.RepositoryGet<GebruikersRechtV2>();
            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
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
                    Properties.Resources.AdNummerVerplicht);
            }
            if (string.IsNullOrEmpty(_gelieerdePersonenMgr.ContactEmail(gelieerdePersoon)))
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.EMailVerplicht, Properties.Resources.EmailOntbreekt);
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
        /// <paramref name="gelieerdePersoonId"/> af voor de groepen met gegeven <paramref name="groepIds"/>
        /// </summary>
        /// <param name="gelieerdePersoonId">Id van gelieerde persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIds">Id's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        public void RechtenAfnemen(int gelieerdePersoonId, int[] groepIds)
        {
            var gelieerdePersoon = _gelieerdePersonenRepo.ByID(gelieerdePersoonId);
            Gav.Check(gelieerdePersoon);

            var persoon = gelieerdePersoon.Persoon;
            RechtenAfnemen(persoon, groepIds);
        }

        /// <summary>
        /// Neemt de alle gebruikersrechten van de gelieerde persoon met gegeven
        /// <paramref name="gelieerdePeroonID"/> af voor de groepen met gegeven <paramref name="groepIds"/>
        /// </summary>
        /// <param name="gelieerdePersoonID">GelieerdePersoonID van gelieerde persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIds">Id's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        public void RechtenAfnemenGelieerdePersoon(int gelieerdePersoonID, int[] groepIds)
        {
            var persoon = _gelieerdePersonenRepo.ByID(gelieerdePersoonID).Persoon;
            // RechtenAfnemen controleert de rechten.
            RechtenAfnemen(persoon, groepIds);
        }

        private void RechtenAfnemen(Persoon persoon, IEnumerable<int> groepIds)
        {
            Gav.Check(persoon);
            if (persoon == null)
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var vervallenrechten =
                (from g in persoon.GebruikersRechtV2 where groepIds.Contains(g.Groep.ID) select g).ToList();

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
                    Bericht = Properties.Resources.KoppelingLoginPersoonOntbreekt,
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
                                                                  Bericht = Properties.Resources.EmailOntbreekt,
                                                                  FoutNummer = FoutNummer.EMailVerplicht
                                                              });
            }

            email = email.Replace(';', ',');

            var cp = new CredentialsProvider(Properties.Settings.Default.EncryptieSleutel,
                                             Properties.Settings.Default.HashSleutel);

            var credentials = cp.Genereren(String.Format("{0};{1};{2};{3:dd/MM/yyyy H:mm:ss zzz}", naam, stamnr, email, DateTime.Now));

            return String.Format(Properties.Settings.Default.UrlVerzekeraar,
                                 HttpUtility.UrlEncode(credentials.GeencrypteerdeUserInfo),
                                 HttpUtility.UrlEncode(credentials.Hash));
        }

        /// <summary>
        /// Levert de details van de gebruiker met gegeven <paramref name="login"/>.
        /// </summary>
        /// <param name="login">Login van een gebruiker.</param>
        /// <returns>Details van de gebruiker met gegeven <paramref name="login"/>.</returns>
        public GebruikersDetail GebruikerOphalen(string login)
        {
            int? adNummer = _authenticatieMgr.AdNummerGet();
            if (adNummer == null)
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.KoppelingLoginPersoonOntbreekt, String.Format(
                    Properties.Resources.KoppelingLoginPersoonOntbreekt,
                    login,
                    adNummer));
            }
            var persoon = (from p in _personenRepo.Select()
                           where p.AdNummer == adNummer
                           select p).FirstOrDefault();

            if (persoon == null)
            {
                throw FaultExceptionHelper.FoutNummer(FoutNummer.KoppelingLoginPersoonOntbreekt, String.Format(
                    Properties.Resources.KoppelingLoginPersoonOntbreekt,
                    login,
                    adNummer));
            }

            // Als je hieronder een exception krijgt dat je gebruiker niet gevonden is, dan kun je die
            // aanmaken, en meteen rechten geven op 1 of meerdere willekeurige groepen. Je hebt hiervoor het
            // AD-nummer nodig uit de exception. (Als je aan het ontwikkelen bent, is dat een dummy-adnr.)
            //
            // Stel dat dat AD-nummer 1445 is, dan gaat het bijvoorbeeld als volgt:
            //   exec auth.spWillekeurigeGroepToekennenAd 1455, 'Vervloet', 'Johan', '1977-03-08', 1
            // De parameters zijn AD-nummer, naam, voornaam, geboortedatum en geslacht.

            // Mag ik mijn eigen gegevens lezen?
            if (_autorisatieMgr.MagLezen(persoon, persoon))
            {
                throw FaultExceptionHelper.GeenGav();
            }

            return Mapper.Map<Persoon, GebruikersDetail>(persoon);
        }
    }
}