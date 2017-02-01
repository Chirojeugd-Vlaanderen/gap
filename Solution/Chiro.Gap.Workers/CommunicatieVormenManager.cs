/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
using AutoMapper;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.Validatie;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. communicatievormen bevat (telefoonnummer, mailadres, enz.)
    /// </summary>
    public class CommunicatieVormenManager : ICommunicatieVormenManager
    {
        private readonly IAutorisatieManager _autorisatieMgr;

        public CommunicatieVormenManager(IAutorisatieManager autorisatieMgr)
        {
            _autorisatieMgr = autorisatieMgr;
        }

        /// <summary>
        /// Koppelt een communicatievorm aan een gelieerde persoon.
        /// </summary>
        /// <param name="gelieerdePersoon">
        /// De gelieerde persoon voor wie de communicatievorm toegevoegd of aangepast wordt
        /// </param>
        /// <param name="nieuweCommunicatieVorm">
        /// De nieuwe gegevens voor de communicatievorm
        /// </param>
        /// <returns>
        /// De lijst van effectief gekoppelde communicatievormen. Als <paramref name="nieuweCommunicatieVorm"/>
        /// gezinsgebonden is, kunnen dat er meer zijn.
        /// </returns>
        /// <remarks>
        /// Als de communicatievorm de eerste van een bepaald type is, dan wordt dat ook de voorkeur.
        /// </remarks>
        public List<CommunicatieVorm> Koppelen(GelieerdePersoon gelieerdePersoon, CommunicatieVorm nieuweCommunicatieVorm)
        {
            // Opmerking: gebruik dit niet als referentie-implementatie:
            // Te veel rommel!

            var gekoppeld = new List<CommunicatieVorm>();

            var cvValid = new CommunicatieVormValidator();

            if (!cvValid.Valideer(nieuweCommunicatieVorm))
            {
                throw new FoutNummerException(FoutNummer.ValidatieFout, string.Format(Resources.CommunicatieVormValidatieFeedback, 
                                                           nieuweCommunicatieVorm.Nummer, 
                                                           nieuweCommunicatieVorm.CommunicatieType.Omschrijving));
            }

            // nieuweCommunicatieVorm kan hier nog geen ID hebben
            Debug.Assert(nieuweCommunicatieVorm.ID == 0);

            nieuweCommunicatieVorm.GelieerdePersoon = gelieerdePersoon;
            gelieerdePersoon.Communicatie.Add(nieuweCommunicatieVorm);

            gekoppeld.Add(nieuweCommunicatieVorm);

            if (nieuweCommunicatieVorm.IsGezinsgebonden)
            {
                // Beetje gepruts voor gezinsgebonden communicatie. Dit zit niet juist op database-niveau. (#1070)
                // Als er gezinsgebonden communicatie wordt toegevoegd, dan voegen we die hier toe aan alle andere
                // gezinsleden, of juister: adresgenoten.

                var alleAdresgenoten =
                    gelieerdePersoon.Persoon.PersoonsAdres.Select(pa => pa.Adres)
                      .SelectMany(adr => adr.PersoonsAdres)
                      .Select(pa => pa.Persoon).ToList();
                var mijnAdresGenoten = _autorisatieMgr.MijnGelieerdePersonen(alleAdresgenoten);

                // adresgenoten die ik niet zelf ben, en die het e-mailadres nog niet hebben.
                var relevanteAdresGenoten = (from gp in mijnAdresGenoten
                                             where gp.ID != gelieerdePersoon.ID
                                                   &&
                                                   !gp.Communicatie.Any(
                                                       cv =>
                                                       cv.CommunicatieType.ID ==
                                                       nieuweCommunicatieVorm.CommunicatieType.ID &&
                                                       String.Compare(cv.Nummer, nieuweCommunicatieVorm.Nummer,
                                                                             StringComparison.OrdinalIgnoreCase) == 0)
                                             select gp).ToList();

                // Automapper gauw gebruiken om te klonen. (mogelijk niet zo mooi ;))
                Mapper.CreateMap<CommunicatieVorm, CommunicatieVorm>();

                foreach (var adresgenoot in relevanteAdresGenoten)
                {
                    var cvKloon = new CommunicatieVorm();
                    Mapper.Map(nieuweCommunicatieVorm, cvKloon);

                    cvKloon.GelieerdePersoon = adresgenoot;
                    adresgenoot.Communicatie.Add(cvKloon);
                    gekoppeld.Add(cvKloon);
                }
            }

            // Voorkeurscommunicatie zetten voor toegevoegde communicatievorm.
            // Opgelet: als de communicatievorm gezinsgebonden was, en voorkeur, dan zal die
            // bij de gezinsgenoten die hem nog niet hebben als voorkeurscommunicatie van dat
            // type toegevoegd worden. Dit is misschien gewenst, misschien ook niet. Maar
            // alleszins erg verwarrend. Zie ook #1070.

            foreach (var cv in gekoppeld)
            {
                bool eersteVanType = (from c in cv.GelieerdePersoon.Communicatie
                                      where c.CommunicatieType.ID == nieuweCommunicatieVorm.CommunicatieType.ID
                                            && !Equals(c, cv)
                                      select c).FirstOrDefault() == null;

                if (eersteVanType || cv.Voorkeur)
                {
                    VoorkeurZetten(nieuweCommunicatieVorm);
                }
            }

            return gekoppeld;
        }

        /// <summary>
        /// Stelt de gegeven communicatievorm in als voorkeurscommunicatievorm voor zijn
        /// type en gelieerde persoon
        /// </summary>
        /// <param name="cv">
        /// Communicatievorm die voorkeurscommunicatievorm moet worden,
        /// gegeven zijn type en gelieerde persoon
        /// </param>
        public void VoorkeurZetten(CommunicatieVorm cv)
        {
            foreach (
                var communicatieVorm in
                    cv.GelieerdePersoon.Communicatie.Where(c => c.CommunicatieType.ID == cv.CommunicatieType.ID).ToArray
                        ())
            {
                communicatieVorm.Voorkeur = Equals(communicatieVorm, cv);
            }
        }

        /// <summary>
        /// Werkt de gegeven <paramref name="communicatieVorm"/> bij op basis van de informatie
        /// in <paramref name="communicatieInfo"/>.
        /// </summary>
        /// <param name="communicatieVorm">Bij te werken communicatievorm</param>
        /// <param name="communicatieInfo">Nieuwe informatie communicatievorm</param>
        public void Bijwerken(CommunicatieVorm communicatieVorm, CommunicatieInfo communicatieInfo)
        {
            // Entity framework zal zelf nakijken of er een ID wordt overschreven, en of er geen concurrency-
            // problemen optraden. Daar trekken we ons niets van aan.

            if (communicatieVorm.CommunicatieType.ID != communicatieInfo.CommunicatieTypeID)
            {
                // Wisselen van communicatietypes ondersteunen we momenteel niet
                throw new NotSupportedException();
            }

            if (!IsGeldig(communicatieInfo.Nummer, communicatieVorm.CommunicatieType))
            {
                throw new FoutNummerException(FoutNummer.ValidatieFout, string.Format(Resources.CommunicatieVormValidatieFeedback,
                                                           communicatieInfo.Nummer,
                                                           communicatieVorm.CommunicatieType.Omschrijving));
            }

            // Voor de rest kopieer ik gewoon velden van communicatieInfo naar communicatieVorm. Ik gebruik hier
            // geen automapper, om te vermijden dat er onverwachte zaken worden overschreven. Bovendien is
            // het stuk 'voorkeur' speciaal.

            communicatieVorm.IsGezinsgebonden = communicatieInfo.IsGezinsGebonden;
            communicatieVorm.Nota = communicatieInfo.Nota;
            communicatieVorm.Nummer = communicatieInfo.Nummer;
            communicatieVorm.VersieString = communicatieInfo.VersieString;
            
            if (communicatieInfo.Voorkeur)
            {
                VoorkeurZetten(communicatieVorm);
            }
        }

        /// <summary>
        /// Controleert of <paramref name="p"/> een geldige communicatievorm zou zijn
        /// voor het gegeven <paramref name="communicatieType"/>
        /// </summary>
        /// <param name="p">telefoonnummer, e-mailadres,...</param>
        /// <param name="communicatieType">een communicatietype</param>
        /// <returns><c>true</c> als <paramref name="p"/> een geldige communicatievorm zou zijn
        /// voor het gegeven <paramref name="communicatieType"/></returns>
        public bool IsGeldig(string p, CommunicatieType communicatieType)
        {
            var validator = new CommunicatieVormValidator();

            return
                validator.Valideer(new CommunicatieValidatieInfo
                                       {
                                           CommunicatieTypeValidatie = communicatieType.Validatie,
                                           Nummer = p
                                       });
        }

        private class CommunicatieValidatieInfo: ICommunicatie
        {
            public string Nummer { get; set; }
            public string CommunicatieTypeValidatie { get; set; }
        }
    }
}