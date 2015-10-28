/*
 * Copyright 2014,2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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

using AutoMapper;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.ServiceContracts.Mappers.Properties;
using Chiro.Gap.WorkerInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chiro.Gap.ServiceContracts.Mappers
{
    public class MappingHelper
    {
        private readonly ILedenManager _ledenMgr;
        private readonly IGroepsWerkJarenManager _groepsWerkJarenMgr;
        private readonly IAbonnementenManager _abonnementenMgr;

        public MappingHelper(ILedenManager ledenManager, IGroepsWerkJarenManager groepsWerkjarenManager, IAbonnementenManager abonnementenManager)
        {
            _ledenMgr = ledenManager;
            _groepsWerkJarenMgr = groepsWerkjarenManager;
            _abonnementenMgr = abonnementenManager;
        }

        /// <summary>
        /// Definieert meteen alle nodige mappings.
        /// </summary>
        public void MappingsDefinieren()
        {
            Mapper.CreateMap<Persoon, PersoonInfo>()
                // de members die in src en dst hetzelfde heten, laat ik voor het gemak weg.
                // de members die genegeerd moeten worden, vermeld ik wel expliciet, anders
                // crasht de assert helemaal onderaan.
                .ForMember(dst => dst.GelieerdePersoonID, opt => opt.Ignore())
                .ForMember(dst => dst.ChiroLeefTijd, opt => opt.Ignore());

            Mapper.CreateMap<GelieerdePersoon, PersoonInfo>()
                .ForMember(dst => dst.AdNummer, opt => opt.MapFrom(src => src.Persoon.AdNummer))
                .ForMember(dst => dst.GeboorteDatum, opt => opt.MapFrom(src => src.Persoon.GeboorteDatum))
                .ForMember(dst => dst.SterfDatum, opt => opt.MapFrom(src => src.Persoon.SterfDatum))
                .ForMember(dst => dst.GelieerdePersoonID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => src.Persoon.Geslacht))
                .ForMember(dst => dst.Naam, opt => opt.MapFrom(src => src.Persoon.Naam))
                .ForMember(dst => dst.VersieString, opt => opt.MapFrom(src => src.Persoon.VersieString))
                .ForMember(dst => dst.VoorNaam, opt => opt.MapFrom(src => src.Persoon.VoorNaam))
                .ForMember(dst => dst.NieuwsBrief, opt => opt.MapFrom(src => src.Persoon.NieuwsBrief));

            Mapper.CreateMap<GelieerdePersoon, PersoonDetail>()
                  .ForMember(
                      dst => dst.GelieerdePersoonID,
                      opt => opt.MapFrom(src => src.ID))
                  // TODO (#968): opkuis
                  .ForMember(
                      dst => dst.IsLid,
                      opt => opt.MapFrom(src => _ledenMgr.IsActiefKind(src)))
                  .ForMember(
                      dst => dst.IsLeiding,
                      opt => opt.MapFrom(src => _ledenMgr.IsActieveLeiding(src)))
                  .ForMember(
                      dst => dst.LidID,
                      opt => opt.MapFrom(src => _ledenMgr.LidIDGet(src)))
                  .ForMember(
                      dst => dst.KanLidWorden,
                      opt => opt.MapFrom(src => _ledenMgr.KanInschrijvenAlsKind(src)))
                  .ForMember(
                      dst => dst.KanLeidingWorden,
                      opt => opt.MapFrom(src => _ledenMgr.KanInschrijvenAlsLeiding(src)))
                  .ForMember(
                      dst => dst.AdNummer,
                      opt => opt.MapFrom(src => src.Persoon.AdNummer))
                  .ForMember(
                      dst => dst.GeboorteDatum,
                      opt => opt.MapFrom(src => src.Persoon.GeboorteDatum))
                  .ForMember(
                      dst => dst.SterfDatum,
                      opt => opt.MapFrom(src => src.Persoon.SterfDatum))
                  .ForMember(
                      dst => dst.Naam,
                      opt => opt.MapFrom(src => src.Persoon.Naam))
                  .ForMember(
                      dst => dst.VoorNaam,
                      opt => opt.MapFrom(src => src.Persoon.VoorNaam))
                  .ForMember(
                      dst => dst.CategorieLijst,
                      opt => opt.MapFrom(src => src.Categorie.ToList()))
                  .ForMember(
                      dst => dst.Geslacht,
                      opt => opt.MapFrom(src => src.Persoon.Geslacht))
                  .ForMember(
                      dst => dst.ChiroLeefTijd,
                      opt => opt.MapFrom(src => src.ChiroLeefTijd))
                  .ForMember(
                      dst => dst.PersoonID,
                      opt => opt.MapFrom(src => src.Persoon.ID))
                  .ForMember(
                      dst => dst.VoorkeursAdresID,
                      opt => opt.MapFrom(src => src.PersoonsAdres == null ? 0 : src.PersoonsAdres.ID))
                .ForMember(
                    dst => dst.NieuwsBrief,
                    opt => opt.MapFrom(src => src.Persoon.NieuwsBrief))
                  .ForMember(
                      dst => dst.VolledigeNaam,
                      opt => opt.Ignore());

            Mapper.CreateMap<GelieerdePersoon, PersoonOverzicht>()
                .ForMember(dst => dst.AdNummer, opt => opt.MapFrom(src => src.Persoon.AdNummer))
                .ForMember(dst => dst.Bus, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : src.PersoonsAdres.Adres.Bus))
                .ForMember(dst => dst.Email, opt => opt.MapFrom(src => VoorkeurCommunicatie(src, CommunicatieTypeEnum.Email)))
                .ForMember(dst => dst.NieuwsBrief, opt => opt.MapFrom(src => src.Persoon.NieuwsBrief))
                .ForMember(dst => dst.GeboorteDatum, opt => opt.MapFrom(src => src.Persoon.GeboorteDatum))
                .ForMember(dst => dst.SterfDatum, opt => opt.MapFrom(src => src.Persoon.SterfDatum))
                .ForMember(dst => dst.GelieerdePersoonID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => src.Persoon.Geslacht))
                .ForMember(dst => dst.HuisNummer, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : src.PersoonsAdres.Adres.HuisNr))
                .ForMember(dst => dst.Naam, opt => opt.MapFrom(src => src.Persoon.Naam))
                .ForMember(dst => dst.PostNummer, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : src.PersoonsAdres.Adres.PostNummerGet()))
                .ForMember(dst => dst.StraatNaam, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : src.PersoonsAdres.Adres.StraatGet()))
                .ForMember(dst => dst.TelefoonNummer,
                           opt => opt.MapFrom(src => VoorkeurCommunicatie(src, CommunicatieTypeEnum.TelefoonNummer)))
                .ForMember(dst => dst.VoorNaam, opt => opt.MapFrom(src => src.Persoon.VoorNaam))
                .ForMember(dst => dst.WoonPlaats, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : src.PersoonsAdres.Adres.WoonPlaatsGet()))
                .ForMember(dst => dst.PostCode, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : src.PersoonsAdres.Adres.PostCodeGet()))
                .ForMember(dst => dst.Land, opt => opt.MapFrom(src => src.PersoonsAdres == null ? null : src.PersoonsAdres.Adres.LandGet()));

            // Deze mapping moet alle adresvelden negeren. Ze zal gebruikt worden als tussenstap wanneer
            // we een LidOverzicht nodig hebben, maar de adressen irrelevant zijn. Op die manier vermijden
            // we dat de adressen 'gelazyload' worden.
            Mapper.CreateMap<Lid, LidOverzichtZonderAdres>()
                .ForMember(dst => dst.AdNummer, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.AdNummer))
                .ForMember(dst => dst.Email,
                    opt =>
                        opt.MapFrom(src => VoorkeurCommunicatie(src.GelieerdePersoon, CommunicatieTypeEnum.Email)))
                .ForMember(dst => dst.NieuwsBrief, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.NieuwsBrief))
                .ForMember(dst => dst.GeboorteDatum,
                    opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.GeboorteDatum))
                .ForMember(dst => dst.SterfDatum, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.SterfDatum))
                .ForMember(dst => dst.GelieerdePersoonID, opt => opt.MapFrom(src => src.GelieerdePersoon.ID))
                .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.Geslacht))
                .ForMember(dst => dst.Naam, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.Naam))
                .ForMember(dst => dst.TelefoonNummer,
                    opt =>
                        opt.MapFrom(
                            src => VoorkeurCommunicatie(src.GelieerdePersoon, CommunicatieTypeEnum.TelefoonNummer)))
                .ForMember(dst => dst.VoorNaam, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.VoorNaam))
                .ForMember(dst => dst.Functies, opt => opt.MapFrom(src => src.Functie))
                .ForMember(dst => dst.Afdelingen, opt => opt.MapFrom(src => AfdelingsJaren(src)))
                .ForMember(dst => dst.ChiroLeefTijd, opt => opt.MapFrom(src => src.GelieerdePersoon.ChiroLeefTijd))
                .ForMember(dst => dst.LidID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.EindeInstapPeriode,
                    opt =>
                        opt.MapFrom(src => src.EindeInstapPeriode < DateTime.Now ? null : src.EindeInstapPeriode))
                .ForMember(dst => dst.StraatNaam, opt => opt.Ignore())
                .ForMember(dst => dst.HuisNummer, opt => opt.Ignore())
                .ForMember(dst => dst.Bus, opt => opt.Ignore())
                .ForMember(dst => dst.PostNummer, opt => opt.Ignore())
                .ForMember(dst => dst.PostCode, opt => opt.Ignore())
                .ForMember(dst => dst.WoonPlaats, opt => opt.Ignore())
                .ForMember(dst => dst.Land, opt => opt.Ignore());

            Mapper.CreateMap<Lid, LidAfdelingInfo>()
                  .ForMember(dst => dst.VolledigeNaam,
                             opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.VolledigeNaam))
                  .ForMember(dst => dst.Type, opt => opt.MapFrom(src => src.Type))
                  .ForMember(dst => dst.AfdelingsJaarIDs, opt => opt.MapFrom(src => src.AfdelingsJaarIDs));

            Mapper.CreateMap<LidOverzichtZonderAdres, LidOverzicht>();

            Mapper.CreateMap<Lid, LidOverzicht>()
                .ForMember(dst => dst.AdNummer, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.AdNummer))
                .ForMember(dst => dst.Bus,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.GelieerdePersoon.PersoonsAdres == null
                                    ? null
                                    : src.GelieerdePersoon.PersoonsAdres.Adres.Bus))
                .ForMember(dst => dst.Email,
                    opt => opt.MapFrom(src => VoorkeurCommunicatie(src.GelieerdePersoon, CommunicatieTypeEnum.Email)))
                .ForMember(dst => dst.NieuwsBrief, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.NieuwsBrief))
                .ForMember(dst => dst.GeboorteDatum,
                    opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.GeboorteDatum))
                .ForMember(dst => dst.SterfDatum, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.SterfDatum))
                .ForMember(dst => dst.GelieerdePersoonID, opt => opt.MapFrom(src => src.GelieerdePersoon.ID))
                .ForMember(dst => dst.Geslacht, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.Geslacht))
                .ForMember(dst => dst.HuisNummer,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.GelieerdePersoon.PersoonsAdres == null
                                    ? null
                                    : src.GelieerdePersoon.PersoonsAdres.Adres.HuisNr))
                .ForMember(dst => dst.Naam, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.Naam))
                .ForMember(dst => dst.PostNummer,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.GelieerdePersoon.PersoonsAdres == null
                                    ? null
                                    : src.GelieerdePersoon.PersoonsAdres.Adres.PostNummerGet()))
                .ForMember(dst => dst.StraatNaam,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.GelieerdePersoon.PersoonsAdres == null
                                    ? null
                                    : src.GelieerdePersoon.PersoonsAdres.Adres.StraatGet()))
                .ForMember(dst => dst.TelefoonNummer,
                    opt =>
                        opt.MapFrom(
                            src => VoorkeurCommunicatie(src.GelieerdePersoon, CommunicatieTypeEnum.TelefoonNummer)))
                .ForMember(dst => dst.VoorNaam, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.VoorNaam))
                .ForMember(dst => dst.WoonPlaats,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.GelieerdePersoon.PersoonsAdres == null
                                    ? null
                                    : src.GelieerdePersoon.PersoonsAdres.Adres.WoonPlaatsGet()))
                .ForMember(dst => dst.Functies, opt => opt.MapFrom(src => src.Functie))
                .ForMember(dst => dst.Afdelingen, opt => opt.MapFrom(src => AfdelingsJaren(src)))
                .ForMember(dst => dst.ChiroLeefTijd, opt => opt.MapFrom(src => src.GelieerdePersoon.ChiroLeefTijd))
                .ForMember(dst => dst.LidID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.EindeInstapPeriode,
                    opt => opt.MapFrom(src => src.EindeInstapPeriode < DateTime.Now ? null : src.EindeInstapPeriode))
                .ForMember(dst => dst.PostCode,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.GelieerdePersoon.PersoonsAdres == null
                                    ? null
                                    : src.GelieerdePersoon.PersoonsAdres.Adres.PostCodeGet()))
                .ForMember(dst => dst.Land,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.GelieerdePersoon.PersoonsAdres == null
                                    ? null
                                    : src.GelieerdePersoon.PersoonsAdres.Adres.LandGet()));

            Mapper.CreateMap<Groep, GroepDetail>()
                .ForMember(
                    dst => dst.Plaats, opt => opt.MapFrom(
                    src => src is ChiroGroep ? (src as ChiroGroep).Plaats : String.Empty))
                .ForMember(
                    dst => dst.StamNummer, opt => opt.MapFrom(
                    src => src.Code == null ? String.Empty : src.Code.ToUpper()))
                .ForMember(
                    dst => dst.Categorieen, opt => opt.MapFrom(
                    src => src.Categorie))
                .ForMember(
                    dst => dst.Functies, opt => opt.MapFrom(
                    src => src.Functie))
                .ForMember(
                    dst => dst.Afdelingen,
                    opt => opt.Ignore());

            Mapper.CreateMap<Afdeling, AfdelingInfo>()
                .ForMember(
                    dst => dst.Afkorting,
                    opt => opt.MapFrom(src => src.Afkorting))
                .ForMember(
                    dst => dst.Naam,
                    opt => opt.MapFrom(src => src.Naam))
                .ForMember(
                    dst => dst.ID,
                    opt => opt.MapFrom(src => src.ID));

            Mapper.CreateMap<AfdelingsJaar, AfdelingsJaarInfo>()
                  .ForMember(dst => dst.Afkorting, opt => opt.MapFrom(src => src.Afdeling.Afkorting))
                  .ForMember(dst => dst.Naam, opt => opt.MapFrom(src => src.Afdeling.Naam))
                  .ForMember(dst => dst.ID, opt => opt.MapFrom(src => src.Afdeling.ID));

            Mapper.CreateMap<Functie, FunctieInfo>();
            Mapper.CreateMap<Functie, FunctieDetail>();

            Mapper.CreateMap<Lid, LidInfo>()
                .ForMember(
                    dst => dst.LidID,
                    opt => opt.MapFrom(src => src.ID))
                .ForMember(
                    dst => dst.Type,
                    opt => opt.MapFrom(src => src is Kind ? LidType.Kind : LidType.Leiding))
                .ForMember(
                    dst => dst.NonActief,
                    opt => opt.MapFrom(src => src.NonActief))
                .ForMember(
                    dst => dst.LidgeldBetaald,
                    opt => opt.MapFrom(src => src.LidgeldBetaald))
                .ForMember(
                    dst => dst.AfdelingIdLijst,
                    opt => opt.MapFrom(src => src.AfdelingIds))
                .ForMember(
                    dst => dst.Functies,
                    opt => opt.MapFrom(src => src.Functie))
                .ForMember(
                    dst => dst.GroepsWerkJaarID,
                    opt => opt.MapFrom(src => src.GroepsWerkJaar != null ? src.GroepsWerkJaar.ID : 0))
                .ForMember(
                    dst => dst.VerzekeringLoonVerlies,
                    opt => opt.MapFrom(src => _groepsWerkJarenMgr.IsVerzekerd(
                        src,
                        Verzekering.LoonVerlies)));

            // Dit was beperkt. Dit moet uitgebreider voor de Excelexport.
            Mapper.CreateMap<Lid, PersoonLidInfo>()
                .ForMember(
                    dst => dst.PersoonDetail,
                    opt => opt.MapFrom(src => src.GelieerdePersoon))
                .ForMember(
                    dst => dst.LidInfo,
                    opt => opt.MapFrom(src => src))
                .ForMember(
                    dst => dst.PersoonsAdresInfo,
                    opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.PersoonsAdres))
                .ForMember(
                    dst => dst.CommunicatieInfo,
                    opt => opt.MapFrom(src => src.GelieerdePersoon.Communicatie))
                .ForMember(
                    dst => dst.DubbelpuntAbonnement,
                    opt => opt.MapFrom(src => _abonnementenMgr.HuidigAbonnementTypeGet(src.GelieerdePersoon, 1)))
                .ForMember(
                    dst => dst.GebruikersInfo,
                    opt => opt.Ignore()); // niet relevant, denk ik :-P

            // De bedoeling was om zo veel mogelijk automatisch te kunnen mappen.  Vandaar ook properties
            // zoals StraatNaamNaam en WoonPlaatsNaam.  Maar met de invoering van de buitenlandse adressen,
            // lukt dat niet meer, omdat er een verschil is voor de mapping van een Belgisch en een
            // niet-Belgisch adres.  We werken daarrond via de extension methods StraatGet en
            // WoonPlaatsGet.

            Mapper.CreateMap<Adres, AdresInfo>()
                .ForMember(
                    dst => dst.PostNr,
                    opt => opt.MapFrom(src => src.PostNummerGet()))
                .ForMember(dst => dst.StraatNaamNaam, opt => opt.MapFrom(src => src.StraatGet()))
                .ForMember(dst => dst.WoonPlaatsNaam, opt => opt.MapFrom(src => src.WoonPlaatsGet()))
                .ForMember(dst => dst.LandNaam, opt => opt.MapFrom(src => src.LandGet()))
                .ForMember(dst => dst.PostCode, opt => opt.MapFrom(src => src.PostCodeGet()))
                .ForMember(dst => dst.IsBelgisch, opt => opt.MapFrom(src => src is BelgischAdres));

            Mapper.CreateMap<Adres, GezinInfo>()
                .ForMember(
                    dst => dst.PostNr,
                    opt => opt.MapFrom(src => src.PostNummerGet()))
                .ForMember(
                    dst => dst.Bewoners,        // bewoners moeten manueel gemapt worden (via persoonsadres)
                    opt => opt.Ignore())        // om problemen met gebruikersrechten te vermijden
                .ForMember(dst => dst.StraatNaamNaam, opt => opt.MapFrom(src => src.StraatGet()))
                .ForMember(dst => dst.WoonPlaatsNaam, opt => opt.MapFrom(src => src.WoonPlaatsGet()))
                .ForMember(dst => dst.LandNaam, opt => opt.MapFrom(src => src.LandGet()))
                .ForMember(dst => dst.PostCode, opt => opt.MapFrom(src => src.PostCodeGet()))
                .ForMember(dst => dst.IsBelgisch, opt => opt.MapFrom(src => src is BelgischAdres));

            // Domme mapping
            Mapper.CreateMap<PersoonsAdres, PersoonsAdresInfo>()
                .ForMember(
                    dst => dst.Bus,
                    opt => opt.MapFrom(src => src.Adres.Bus))
                .ForMember(
                    dst => dst.HuisNr,
                    opt => opt.MapFrom(src => src.Adres.HuisNr))
                .ForMember(
                    dst => dst.ID,
                    opt => opt.MapFrom(src => src.Adres.ID))
                .ForMember(
                    dst => dst.PersoonsAdresID,
                    opt => opt.MapFrom(src => src.ID))
                .ForMember(
                    dst => dst.PostNr,
                    opt => opt.MapFrom(src => src.Adres.PostNummerGet()))
                .ForMember(
                    dst => dst.StraatNaamNaam,
                    opt => opt.MapFrom(src => src.Adres.StraatGet()))
                .ForMember(
                    dst => dst.WoonPlaatsNaam,
                    opt => opt.MapFrom(src => src.Adres.WoonPlaatsGet()))
                .ForMember(dst => dst.LandNaam, opt => opt.MapFrom(src => src.Adres.LandGet()))
                .ForMember(dst => dst.PostCode, opt => opt.MapFrom(src => src.Adres.PostCodeGet()))
                .ForMember(dst => dst.IsBelgisch, opt => opt.MapFrom(src => src.Adres is BelgischAdres));

            Mapper.CreateMap<GelieerdePersoon, BewonersInfo>()
                .ForMember(dst => dst.GelieerdePersoonID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.AdresType, opt => opt.MapFrom(src => AdresTypeEnum.Overig));

            Mapper.CreateMap<PersoonsAdres, BewonersInfo>()
                .ForMember(
                    dst => dst.GelieerdePersoonID,
                    opt => opt.MapFrom(src => src.Persoon.GelieerdePersoon.FirstOrDefault() == null ? 0 : src.Persoon.GelieerdePersoon.First().ID));

            // TODO (#1050): Uitvissen deelnemer of begeleider op basis van kind/leiding werkt wel min of meer voor groepen,
            // maar kan op die manier niet gebruikt worden voor uitstappen van kaderploegen.

            Mapper.CreateMap<Deelnemer, DeelnemerDetail>()
                .ForMember(dst => dst.Afdelingen,
                    opt =>
                        opt.MapFrom(
                            src =>
                                Afdelingen(
                                    src.GelieerdePersoon.Lid.OrderByDescending(ld => ld.GroepsWerkJaar.WerkJaar)
                                        .FirstOrDefault())))
                .ForMember(dst => dst.DeelnemerID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.PersoonOverzicht, opt => opt.Ignore())
                .ForMember(dst => dst.FamilieNaam, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.Naam))
                .ForMember(dst => dst.VoorNaam, opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.VoorNaam))
                .ForMember(dst => dst.Type,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.IsLogistieker
                                    ? DeelnemerType.Logistiek
                                    : DeelnemerTypeBepalen(src.GelieerdePersoon, src.Uitstap.DatumVan)))
                .ForMember(dst => dst.IsContact,
                    opt => opt.MapFrom(src => src.UitstapWaarvoorVerantwoordelijk.FirstOrDefault() != null));

            Mapper.CreateMap<DeelnemerInfo, Deelnemer>()
                .ForMember(dst => dst.ID, opt => opt.Ignore())
                .ForMember(dst => dst.Versie, opt => opt.Ignore())
                .ForMember(dst => dst.GelieerdePersoon, opt => opt.Ignore())
                .ForMember(dst => dst.Uitstap, opt => opt.Ignore())
                .ForMember(dst => dst.UitstapWaarvoorVerantwoordelijk, opt => opt.Ignore())
                .ForMember(dst => dst.VersieString, opt => opt.Ignore());

            // Als de property's van de doelobjecten strategisch gekozen namen hebben, configureert
            // Automapper alles automatisch, zoals hieronder:

            Mapper.CreateMap<StraatNaam, StraatInfo>();
            Mapper.CreateMap<WoonPlaats, WoonPlaatsInfo>();
            Mapper.CreateMap<Land, LandInfo>();
            Mapper.CreateMap<CommunicatieType, CommunicatieTypeInfo>();
            Mapper.CreateMap<Categorie, CategorieInfo>();
            Mapper.CreateMap<PersoonsAdres, PersoonsAdresInfo2>();
            Mapper.CreateMap<CommunicatieVorm, CommunicatieInfo>();
            Mapper.CreateMap<CommunicatieVorm, CommunicatieDetail>();
            Mapper.CreateMap<Uitstap, UitstapInfo>();

            Mapper.CreateMap<Uitstap, UitstapOverzicht>()
                .ForMember(dst => dst.Adres, opt => opt.MapFrom(src => src.Plaats == null ? null : src.Plaats.Adres));

            Mapper.CreateMap<Groep, GroepInfo>()
                .ForMember(dst => dst.Plaats, opt => opt.MapFrom(
                    src => src is ChiroGroep ? (src as ChiroGroep).Plaats : Resources.NietVanToepassing))
                .ForMember(dst => dst.StamNummer, opt => opt.MapFrom(
                    src => src.Code == null ? String.Empty : src.Code.ToUpper()));

            Mapper.CreateMap<GroepsWerkJaar, GroepsWerkJaarDetail>()
                .ForMember(dst => dst.Status, opt => opt.MapFrom(src => WerkJaarStatus.Onbekend))
                .ForMember(dst => dst.WerkJaarID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.GroepPlaats,
                           opt => opt.MapFrom(
                            src => src.Groep is ChiroGroep ? (src.Groep as ChiroGroep).Plaats : Resources.NietVanToepassing));

            Mapper.CreateMap<CommunicatieInfo, CommunicatieVorm>()
                .ForMember(dst => dst.Versie, opt => opt.Ignore())
                .ForMember(dst => dst.GelieerdePersoon, opt => opt.Ignore())
                .ForMember(dst => dst.CommunicatieType, opt => opt.Ignore());

            Mapper.CreateMap<CommunicatieDetail, CommunicatieVorm>()
                .ForMember(dst => dst.Versie, opt => opt.Ignore())
                .ForMember(dst => dst.GelieerdePersoon, opt => opt.Ignore())
                .ForMember(dst => dst.CommunicatieType, opt => opt.Ignore());

            Mapper.CreateMap<AfdelingsJaar, AfdelingDetail>()
                .ForMember(
                    dst => dst.AfdelingsJaarID,
                    opt => opt.MapFrom(src => src.ID))
                .ForMember(
                    dst => dst.OfficieleAfdelingNaam,
                    opt => opt.MapFrom(src => src.OfficieleAfdeling.Naam))
                .ForMember(
                    dst => dst.AfdelingNaam,
                    opt => opt.MapFrom(src => src.Afdeling.Naam))
                .ForMember(
                    dst => dst.AfdelingAfkorting,
                    opt => opt.MapFrom(src => src.Afdeling.Afkorting));

            Mapper.CreateMap<AfdelingsJaar, ActieveAfdelingInfo>()
                .ForMember(dst => dst.Naam, opt => opt.MapFrom(src => src.Afdeling.Naam))
                .ForMember(dst => dst.Afkorting, opt => opt.MapFrom(src => src.Afdeling.Afkorting))
                .ForMember(dst => dst.AfdelingsJaarID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.ID, opt => opt.MapFrom(src => src.Afdeling.ID));

            Mapper.CreateMap<GroepsWerkJaar, WerkJaarInfo>();
            Mapper.CreateMap<AfdelingsJaar, AfdelingsJaarDetail>()
                .ForMember(dst => dst.AfdelingsJaarID, opt => opt.MapFrom(src => src.ID));

            Mapper.CreateMap<OfficieleAfdeling, OfficieleAfdelingDetail>()
                .ForMember(dst => dst.LeefTijdTot, opt => opt.MapFrom(src => src.LeefTijdTot))
                .ForMember(dst => dst.LeefTijdVan, opt => opt.MapFrom(src => src.LeefTijdVan))
                .ForMember(dst => dst.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dst => dst.Naam, opt => opt.MapFrom(src => src.Naam));


            // Let op: Gebruikersrechten worden niet automatisch gemapt, want dat staat
            // nog niet helemaal op punt.
            Mapper.CreateMap<GelieerdePersoon, PersoonLidInfo>()
                .ForMember(
                    dst => dst.PersoonDetail,
                    opt => opt.MapFrom(src => src))
                .ForMember(
                    dst => dst.PersoonsAdresInfo,
                    opt => opt.MapFrom(src => src.Persoon.PersoonsAdres))
                .ForMember(
                    dst => dst.CommunicatieInfo,
                    opt => opt.MapFrom(src => src.Communicatie))
                .ForMember(
                    dst => dst.LidInfo,
                    opt => opt.MapFrom(src => _ledenMgr.HuidigLidGet(src)))
                .ForMember(
                    dst => dst.DubbelpuntAbonnement,
                    // Voorlopig hebben we enkel Dubbelpunt als publicatie.
                    opt => opt.MapFrom(src => _abonnementenMgr.HuidigAbonnementTypeGet(src, 1)))
                .ForMember(
                    dst => dst.GebruikersInfo,
                    opt => opt.Ignore());

            Mapper.CreateMap<Lid, InschrijvingsVoorstel>()
                .ForMember(
                    dst => dst.GelieerdePersoonID,
                    opt => opt.MapFrom(src => src.GelieerdePersoon.ID))
                .ForMember(
                    dst => dst.AfdelingsJaarIrrelevant,
                    opt => opt.MapFrom(src => false))
                .ForMember(
                    dst => dst.AfdelingsJaarIDs,
                    opt => opt.MapFrom(src => src is Leiding ? (((Leiding)src).AfdelingsJaar.Select(e => e.ID)) : new List<int> { ((Kind)src).AfdelingsJaar.ID }))
                .ForMember(
                    dst => dst.LeidingMaken,
                    opt => opt.MapFrom(src => src is Leiding))
                .ForMember(
                    dst => dst.VolledigeNaam,
                    opt => opt.MapFrom(src => src.GelieerdePersoon.Persoon.VolledigeNaam))
                .ForMember(
                    dst => dst.FoutNummer,
                    opt => opt.UseValue(null));

            Mapper.CreateMap<Poco.Model.GebruikersRecht, GebruikersDetail>()
                .ForMember(dst => dst.IsVerlengbaar, opt => opt.MapFrom(src => src.IsVerlengbaar))
                .ForMember(dst => dst.GelieerdePersoonID, opt => opt.MapFrom(src => GelieerdePersoonIDGet(src)))
                .ForMember(dst => dst.PersoonID, opt => opt.MapFrom(src => src.Gav.Persoon.FirstOrDefault() == null ? 0 : src.Gav.Persoon.First().ID))
                .ForMember(dst => dst.FamilieNaam, opt => opt.MapFrom(src => src.Gav.Persoon.FirstOrDefault() == null ? String.Empty : src.Gav.Persoon.First().Naam))
                .ForMember(dst => dst.VoorNaam, opt => opt.MapFrom(src => src.Gav.Persoon.FirstOrDefault() == null ? String.Empty : src.Gav.Persoon.First().VoorNaam));

            Mapper.CreateMap<Poco.Model.GebruikersRecht, GebruikersInfo>();

            // Een gebruiker mappen naar GebruikersInfo mapt geen gebruikersrechten, omdat er maar rechten van 1
            // groep gemapt kunnen worden.
            Mapper.CreateMap<Gav, GebruikersInfo>()
                .ForMember(dst => dst.GavLogin, opt => opt.MapFrom(src => src.Login))
                .ForMember(dst => dst.IsVerlengbaar, opt => opt.MapFrom(src => false))
                .ForMember(dst => dst.Rol, opt => opt.MapFrom(src => Rol.Geen))
                .ForMember(dst => dst.VervalDatum, opt => opt.MapFrom(src => DateTime.Now.AddDays(-1)));

            #region mapping van datacontracts naar entity's

            // Alwat hieronder ignore krijgt, wordt niet meegenomen van een teruggestuurde
            // PersoonInfo.

            Mapper.CreateMap<PersoonInfo, Persoon>()
                  .ForMember(dst => dst.ID, opt => opt.Ignore())
                  .ForMember(dst => dst.VolledigeNaam, opt => opt.Ignore())
                  .ForMember(dst => dst.SterfDatum, opt => opt.Ignore())
                  .ForMember(dst => dst.Versie, opt => opt.Ignore())
                  .ForMember(dst => dst.GelieerdePersoon, opt => opt.Ignore())
                  .ForMember(dst => dst.PersoonsAdres, opt => opt.Ignore())
                  .ForMember(dst => dst.PersoonsVerzekering, opt => opt.Ignore())
                  .ForMember(dst => dst.InSync, opt => opt.MapFrom(src => src.AdNummer.HasValue))
                  .ForMember(dst => dst.LaatsteMembership, opt => opt.Ignore())
                  .ForMember(dst => dst.Gav, opt => opt.Ignore())
                  .ForMember(dst => dst.BerichtGebruiker, opt => opt.Ignore())
                  .ForMember(dst => dst.BerichtPersoon, opt => opt.Ignore())
                  .ForMember(dst => dst.SeNaam, opt => opt.Ignore())
                  .ForMember(dst => dst.SeVoornaam, opt => opt.Ignore())
                  .ForMember(dst => dst.NieuwsBrief, opt => opt.Ignore());

            Mapper.CreateMap<UitstapInfo, Uitstap>()
                .ForMember(dst => dst.GroepsWerkJaar, opt => opt.Ignore())
                .ForMember(dst => dst.Versie, opt => opt.Ignore())
                .ForMember(dst => dst.Plaats, opt => opt.Ignore())
                .ForMember(dst => dst.ContactDeelnemer, opt => opt.Ignore())
                .ForMember(dst => dst.Deelnemer, opt => opt.Ignore());

            #endregion

            #region Mapping van Exceptions naar Faults
            // TODO (#1052): Kan het mappen van die generics niet efficienter?

            Mapper.CreateMap<BestaatAlException<Categorie>,
                        BestaatAlFault<CategorieInfo>>();
            Mapper.CreateMap<BestaatAlException<Functie>,
                        BestaatAlFault<FunctieDetail>>();
            Mapper.CreateMap<OngeldigObjectException, OngeldigObjectFault>();
            Mapper.CreateMap<BlokkerendeObjectenException<GelieerdePersoon>,
                    BlokkerendeObjectenFault<PersoonDetail>>()
                .ForMember(
                    dst => dst.Objecten,
                    opt => opt.MapFrom(src => src.Objecten.Take(Settings.Default.KleinAantal)));
            Mapper.CreateMap<BlokkerendeObjectenException<PersoonsAdres>,
                    BlokkerendeObjectenFault<PersoonsAdresInfo2>>();
            Mapper.CreateMap<BlokkerendeObjectenException<Lid>,
                BlokkerendeObjectenFault<PersoonLidInfo>>()
                .ForMember(
                    dst => dst.Objecten,
                    opt => opt.MapFrom(src => src.Objecten.Take(Settings.Default.KleinAantal)));
            Mapper.CreateMap<BestaatAlException<Afdeling>,
                    BestaatAlFault<AfdelingInfo>>();
            #endregion

            // Wel even nakijken of die automagie overal gewerkt heeft:

            Mapper.AssertConfigurationIsValid();
        }

        #region Helperfuncties waarvan ik niet zeker ben of ze hier goed staan.

        /// <summary>
        /// Bepaalt het deelnemertype van een <paramref name="gelieerdePersoon"/> als hij voor een uitstap wordt
        /// ingeschreven die plaats vindt vanaf <paramref name="datum"/>.
        /// Als de person op dat moment lid is, dan is het deelnemer. Is de persoon op dat moment leiding, dan
        /// is het begeleiding, en anders is het onbekend.
        /// </summary>
        /// <param name="gelieerdePersoon">Gelieerde persoon met te bepalen DeelnemerType</param>
        /// <param name="datum">Datum voor deelnemertype</param>
        /// <returns>Als de persoon op de gegeven <paramref name="datum"/> lid is, dan Deelnemers. Is hij \
        /// leiding, dan Begeleiding. En anders onbekend.</returns>
        private DeelnemerType DeelnemerTypeBepalen(GelieerdePersoon gelieerdePersoon, DateTime datum)
        {
            int werkJaar = datum.Year;
            DateTime nieuwWerkJaarDatum = new DateTime(werkJaar, Settings.Default.NieuwWerkJaarMaand,
                Settings.Default.NieuwWerkJaarDag);

            if (datum < nieuwWerkJaarDatum)
            {
                --werkJaar;
            }

            var lid =
                (from l in gelieerdePersoon.Lid where l.GroepsWerkJaar.WerkJaar == werkJaar select l).FirstOrDefault();

            return (lid == null
                ? DeelnemerType.Onbekend
                : lid is Kind ? DeelnemerType.Deelnemer : DeelnemerType.Begeleiding);
        }

        /// <summary>
        /// Voorkeurtelefoonnr, voorkeure-maialdres,... van een gelieerde persoon
        /// </summary>
        /// <param name="gp">Gelieerde persoon</param>
        /// <param name="type">Communicatietype waarvan voorkeur gevraagd wordt.</param>
        /// <returns>Voorkeurtelefoonnr, -maiadres,... van de gelieerde persoon.  
        /// <c>null</c> indien onbestaand.</returns>
        private string VoorkeurCommunicatie(GelieerdePersoon gp, CommunicatieTypeEnum type)
        {
            var query = from c in gp.Communicatie
                        where (c.CommunicatieType.ID == (int)type) && c.Voorkeur
                        select c.Nummer;

            return query.FirstOrDefault();
        }

        /// <summary>
        /// Geeft de rij afdelingsjaren weer waaraan een lid gekoppeld is.  Voor een kind bevat de lijst precies
        /// 1 afdelingsjaar, voor leiding kunnen het er ook geen of veel zijn.
        /// </summary>
        /// <param name="l">Lid van wie we afdelingsjaren moeten ophalen</param>
        /// <returns>Rij afdelingsjaren van het lid <paramref name="l"/></returns>
        private IEnumerable<AfdelingsJaarInfo> AfdelingsJaren(Lid l)
        {
            if (l == null)
            {
                return new AfdelingsJaarInfo[0];
            }

            if (l is Kind)
            {
                return new[] { Mapper.Map<AfdelingsJaar, AfdelingsJaarInfo>((l as Kind).AfdelingsJaar) };
            }

            if (l is Leiding)
            {
                return
                    Mapper.Map<IEnumerable<AfdelingsJaar>, IEnumerable<AfdelingsJaarInfo>>((l as Leiding).AfdelingsJaar);
            }
            // Enkel kinderen en leiding
            throw new NotSupportedException();
        }

        /// <summary>
        /// Geeft de rij afdelingen weer waaraan een lid gekoppeld is.  Voor een kind bevat de lijst precies
        /// 1 afdeling, voor leiding kunnen het er ook geen of veel zijn.
        /// </summary>
        /// <param name="l">Lid van wie we afdelingen moeten ophalen</param>
        /// <returns>Rij afdelingen van het lid <paramref name="l"/></returns>
        private IEnumerable<AfdelingInfo> Afdelingen(Lid l)
        {
            if (l == null)
            {
                return new AfdelingInfo[0];
            }

            if (l is Kind)
            {
                return new[] { Mapper.Map<Afdeling, AfdelingInfo>((l as Kind).AfdelingsJaar.Afdeling) };
            }
            else if (l is Leiding)
            {
                return Mapper.Map<IEnumerable<Afdeling>, IEnumerable<AfdelingInfo>>((l as Leiding).AfdelingsJaar.Select(e => e.Afdeling));
            }
            // Enkel kinderen en leiding
            throw new NotSupportedException();
        }

        /// <summary>
        /// Bepaalt het GelieerdePersoonID van de gelieerde persoon die hoort bij het gebruikersrecht
        /// <paramref name="gebruikersRecht"/>.  <c>null</c> indien onbekend.
        /// </summary>
        /// <param name="gebruikersRecht">Gebruikersrecht waarvan we het corresponderende GelieerdePersoonID zoeken</param>
        /// <returns>GelieerdePersoonID van de gelieerde persoon die hoort bij het gebruikersrecht
        /// <paramref name="gebruikersRecht"/>.  <c>null</c> indien onbekend.</returns>
        private int? GelieerdePersoonIDGet(Poco.Model.GebruikersRecht gebruikersRecht)
        {
            if (!gebruikersRecht.Gav.Persoon.Any())
            {
                return null;
            }
            return (from gp in gebruikersRecht.Gav.Persoon.First().GelieerdePersoon
                    where gp.Groep.ID == gebruikersRecht.Groep.ID
                    select gp.ID).FirstOrDefault();
        }

        #endregion

    }

    #region Private extension methods om gemakkelijker adressen te mappen.

    /// <summary>
    /// Extension methods om adressen makkelijk te kunnen mappen.
    /// </summary>
    internal static class AdresExtensies
    {
        /// <summary>
        /// Bepaalt de straatnaam van een Adres
        /// </summary>
        /// <param name="a">Het adres</param>
        /// <returns>De straatnaam</returns>
        public static string StraatGet(this Adres a)
        {
            if (a is BelgischAdres)
            {
                var ba = a as BelgischAdres;

                return ba.StraatNaam != null ? ba.StraatNaam.Naam : null;
            }
            Debug.Assert(a is BuitenLandsAdres);
            return ((BuitenLandsAdres)a).Straat;
        }

        /// <summary>
        /// Bepaalt de woonplaats van een adres.
        /// </summary>
        /// <param name="a">Het adres</param>
        /// <returns>Naam van de woonplaats</returns>
        public static string WoonPlaatsGet(this Adres a)
        {
            if (a is BelgischAdres)
            {
                var ba = (a as BelgischAdres);
                return ba.WoonPlaats != null ? ba.WoonPlaats.Naam : null;
            }
            Debug.Assert(a is BuitenLandsAdres);
            return ((BuitenLandsAdres)a).WoonPlaats;
        }

        /// <summary>
        /// Bepaalt naam van het land van een adres
        /// </summary>
        /// <param name="a">Het adres</param>
        /// <returns>Naam van het land van het adres</returns>
        public static string LandGet(this Adres a)
        {
            if (a is BelgischAdres)
            {
                return Resources.Belgie;
            }
            Debug.Assert(a is BuitenLandsAdres);
            return ((BuitenLandsAdres)a).Land.Naam;
        }

        /// <summary>
        /// Geeft postcode voor een buitenlands adres, of <c>null</c> voor een Belgisch adres.
        /// </summary>
        /// <param name="a">Het adres</param>
        /// <returns>Als <paramref name="a"/> een buitenlands adres is, de postcode, 
        /// anders <c>null</c>.</returns>
        public static string PostCodeGet(this Adres a)
        {
            if (a is BelgischAdres)
            {
                return null;
            }
            Debug.Assert(a is BuitenLandsAdres);
            return ((BuitenLandsAdres)a).PostCode;
        }

        /// <summary>
        /// Bepaalt het postnummer van een adres.
        /// </summary>
        /// <param name="a">Het adres</param>
        /// <returns>Het postnummer</returns>
        public static int? PostNummerGet(this Adres a)
        {
            if (a is BelgischAdres)
            {
                var ba = a as BelgischAdres;
                if (ba.WoonPlaats != null)
                {
                    return ba.WoonPlaats.PostNummer;
                }
                if (ba.StraatNaam != null)
                {
                    return ba.StraatNaam.PostNummer;
                }
                return null;
            }
            Debug.Assert(a is BuitenLandsAdres);
            return ((BuitenLandsAdres)a).PostNummer;
        }
        #endregion

    }
}
