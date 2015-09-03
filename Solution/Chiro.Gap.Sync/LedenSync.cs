/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.Sync.Properties;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Interface voor synchronisatie van lidinfo naar Kipadmin
    /// </summary>
    public class LedenSync : BaseSync,ILedenSync
    {
        /// <summary>
        /// Constructor.
        /// 
        /// De ServiceHelper wordt geïnjecteerd door de dependency injection container. Wat de
        /// ServiceHelper precies zal opleveren, hangt af van welke IChannelProvider geregistreerd
        /// is bij de container.
        /// </summary>
        /// <param name="serviceHelper">ServiceHelper, nodig voor service calls.</param>
        public LedenSync(ServiceHelper serviceHelper) : base(serviceHelper) { }

        // TODO (#1058): Dit gaat waarschijnlijk ook met AutoMapper
        private readonly Dictionary<NationaleFunctie, FunctieEnum> _functieVertaling =
            new Dictionary<NationaleFunctie, FunctieEnum>
				{
					{ NationaleFunctie.ContactPersoon, FunctieEnum.ContactPersoon },
					{ NationaleFunctie.FinancieelVerantwoordelijke, FunctieEnum.FinancieelVerantwoordelijke },
					{ NationaleFunctie.Vb, FunctieEnum.Vb },
					{ NationaleFunctie.GroepsLeiding, FunctieEnum.GroepsLeiding },
					{ NationaleFunctie.JeugdRaad, FunctieEnum.JeugdRaad },
					{ NationaleFunctie.KookPloeg, FunctieEnum.KookPloeg },
					{ NationaleFunctie.Proost, FunctieEnum.Proost },
					{ NationaleFunctie.GroepsLeidingsBijeenkomsten, FunctieEnum.GroepsLeidingsBijeenkomsten },
					{ NationaleFunctie.SomVerantwoordelijke, FunctieEnum.SomVerantwoordelijke },
					{ NationaleFunctie.IkVerantwoordelijke, FunctieEnum.IkVerantwoordelijke },
					{ NationaleFunctie.RibbelVerantwoordelijke, FunctieEnum.RibbelVerantwoordelijke },
					{ NationaleFunctie.SpeelclubVerantwoordelijke, FunctieEnum.SpeelclubVerantwoordelijke },
					{ NationaleFunctie.RakwiVerantwoordelijke, FunctieEnum.RakwiVerantwoordelijke },
					{ NationaleFunctie.TitoVerantwoordelijke, FunctieEnum.TitoVerantwoordelijke },
					{ NationaleFunctie.KetiVerantwoordelijke, FunctieEnum.KetiVerantwoordelijke },
					{ NationaleFunctie.AspiVerantwoordelijke, FunctieEnum.AspiVerantwoordelijke },
					{ NationaleFunctie.SomGewesten, FunctieEnum.SomGewesten },
					{ NationaleFunctie.OpvolgingStadsGroepen, FunctieEnum.OpvolgingStadsGroepen },
					{ NationaleFunctie.Verbondsraad, FunctieEnum.Verbondsraad },
					{ NationaleFunctie.Verbondskern, FunctieEnum.Verbondskern },
					{ NationaleFunctie.StartDagVerantwoordelijker, FunctieEnum.StartDagVerantwoordelijker },
					{ NationaleFunctie.SbVerantwoordelijke, FunctieEnum.SbVerantwoordelijke }
				};

        private readonly Dictionary<NationaleAfdeling, AfdelingEnum> _afdelingVertaling =
            new Dictionary<NationaleAfdeling, AfdelingEnum>
				{
					{ NationaleAfdeling.Ribbels, AfdelingEnum.Ribbels },
					{ NationaleAfdeling.Speelclub, AfdelingEnum.Speelclub },
					{ NationaleAfdeling.Rakwis, AfdelingEnum.Rakwis },
					{ NationaleAfdeling.Titos, AfdelingEnum.Titos },
					{ NationaleAfdeling.Ketis, AfdelingEnum.Ketis },
					{ NationaleAfdeling.Aspis, AfdelingEnum.Aspis },
					{ NationaleAfdeling.Speciaal, AfdelingEnum.Speciaal }
				};

        private readonly Dictionary<LidType, LidTypeEnum> _lidTypeVertaling =
            new Dictionary<LidType, LidTypeEnum>
				{
					{ LidType.Kind, LidTypeEnum.Kind },
					{ LidType.Leiding, LidTypeEnum.Leiding }
				};

        /// <summary>
        /// Stuurt een lid naar Kipadmin. Als het lid inactief is, wordt een stopdatum op de lidrelatie gezet. 
        /// Om een lid te verwijderen, is er de method 'Verwijderen' 
        /// </summary>
        /// <param name="l">Te bewaren lid</param>
        /// <remarks>Voor het gemak gaan we ervan uit dat persoonsgegevens, adressen, afdelingen en functies al
        /// gekoppeld zijn.  Communicatie moeten we sowieso opnieuw ophalen, want kan ook gekoppeld
        /// zijn aan andere gelieerde personen.</remarks>
        public void Bewaren(Lid l)
        {
            Debug.Assert(l.GelieerdePersoon != null);
            Debug.Assert(l.GelieerdePersoon.Persoon != null);
            Debug.Assert(l.GelieerdePersoon.Persoon.InSync);
            Debug.Assert(l.GroepsWerkJaar != null);
            Debug.Assert(l.GroepsWerkJaar.Groep != null);
            var nationaleFuncties = (from f in l.Functie
                where f.IsNationaal
                select _functieVertaling[(NationaleFunctie) f.ID]).ToList();

            List<AfdelingEnum> officieleAfdelingen;

            if (l is Kind)
            {
                officieleAfdelingen = new List<AfdelingEnum>
                {
                    _afdelingVertaling[
                        (NationaleAfdeling) ((l as Kind).AfdelingsJaar.OfficieleAfdeling.ID)]
                };
            }
            else
            {
                Debug.Assert(l is Leiding);
                officieleAfdelingen = (from a in (l as Leiding).AfdelingsJaar
                    select _afdelingVertaling[(NationaleAfdeling) a.OfficieleAfdeling.ID]).ToList
                    ();
            }

            // Euh, en waarom heb ik hiervoor geen mapper gemaakt?

            var lidGedoe = new LidGedoe
            {
                StamNummer = l.GroepsWerkJaar.Groep.Code,
                WerkJaar = l.GroepsWerkJaar.WerkJaar,
                LidType = l is Kind ? LidTypeEnum.Kind : LidTypeEnum.Leiding,
                NationaleFuncties = nationaleFuncties,
                OfficieleAfdelingen = officieleAfdelingen,
                EindeInstapPeriode = l.EindeInstapPeriode,
                UitschrijfDatum = l.UitschrijfDatum,
            };

            if (l.GelieerdePersoon.Persoon.AdNummer != null)
            {
                // AD-nummer gekend. Persoon dus zeker gekend door Kipadmin.
                ServiceHelper.CallService<ISyncPersoonService>(
                    svc => svc.LidBewaren(l.GelieerdePersoon.Persoon.AdNummer.Value, lidGedoe));
            }
            else
            {
                var details = Mapper.Map<GelieerdePersoon, PersoonDetails>(l.GelieerdePersoon);

                // kijk na of de persoon al bewaard is in GAP:
                Debug.Assert(l.GelieerdePersoon.Persoon.ID != 0);

                ServiceHelper.CallService<ISyncPersoonService>(svc => svc.NieuwLidBewaren(details, lidGedoe));
            }
        }

        /// <summary>
        /// Updatet de functies van het lid in Kipadmin
        /// </summary>
        /// <param name="lid">Lid met functies en groep</param>
        /// <remarks>Als geen persoonsgegevens meegeleverd zijn, dan zoeken we die wel even op in de database.</remarks>
        public void FunctiesUpdaten(Lid lid)
        {
            Debug.Assert(lid.GroepsWerkJaar != null);
            Debug.Assert(lid.GroepsWerkJaar.Groep != null);
            Debug.Assert(lid.GelieerdePersoon.Persoon.InSync);

            // TODO (#555): Dit gaat problemen geven met oud-leidingsploegen

            var kipFunctieIDs = (from f in lid.Functie
                                 where f.IsNationaal
                                 select _functieVertaling[(NationaleFunctie)f.ID]).ToArray();

            ServiceHelper.CallService<ISyncPersoonService>(
                svc => svc.FunctiesUpdaten(Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(
                    lid.GelieerdePersoon.Persoon),
                                           lid.GroepsWerkJaar.Groep.Code,
                                           lid.GroepsWerkJaar.WerkJaar,
                                           kipFunctieIDs));
        }

        /// <summary>
        /// Updatet de afdelingen van <paramref name="lid"/> in Kipadmin
        /// </summary>
        /// <param name="lid">Het Lidobject</param>
        /// <remarks>Alle (!) relevante gegevens van het lidobject worden hier sowieso opnieuw opgehaald, anders was het
        /// te veel een gedoe.</remarks>
        public void AfdelingenUpdaten(Lid lid)
        {
            var chiroGroep = (lid.GroepsWerkJaar.Groep as ChiroGroep);
            // TODO (#555): Dit gaat problemen geven met oud-leidingsploegen

            Debug.Assert(chiroGroep != null);
            Debug.Assert(lid.GelieerdePersoon.Persoon.InSync);
            AfdelingEnum[] kipAfdelingen;

            if (lid is Kind)
            {
                kipAfdelingen = new[]
                                    {
                                        _afdelingVertaling[
                                            (NationaleAfdeling)
                                            ((lid as Kind).AfdelingsJaar.OfficieleAfdeling.ID)]
                                    };
            }
            else
            {
                var leiding = lid as Leiding;
                Debug.Assert(leiding != null);

                kipAfdelingen = (from aj in leiding.AfdelingsJaar
                                 select _afdelingVertaling[(NationaleAfdeling)(aj.OfficieleAfdeling.ID)]).ToArray();
            }

            ServiceHelper.CallService<ISyncPersoonService>(
                svc =>
                svc.AfdelingenUpdaten(
                    Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(lid.GelieerdePersoon.Persoon),
                    chiroGroep.Code, lid.GroepsWerkJaar.WerkJaar, kipAfdelingen));
        }

        /// <summary>
        /// Updatet het lidtype van <paramref name="lid"/> in Kipadmin
        /// </summary>
        /// <param name="lid">Lid waarvan het lidtype geupdatet moet worden</param>
        public void TypeUpdaten(Lid lid)
        {
            ServiceHelper.CallService<ISyncPersoonService>(svc => svc.LidTypeUpdaten(
                Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(lid.GelieerdePersoon.Persoon),
                lid.GroepsWerkJaar.Groep.Code,
                lid.GroepsWerkJaar.WerkJaar,
                _lidTypeVertaling[lid.Type]));
        }

        /// <summary>
        /// Verwijdert een lid uit Kipadmin
        /// </summary>
        /// <param name="lid">Te verwijderen lid</param>
        /// <remarks>We verwachten dat groep en  persoon gekoppeld zijn</remarks>
        public void Verwijderen(Lid lid)
        {
            Debug.Assert(lid != null);
            Debug.Assert(lid.GelieerdePersoon != null);
            Debug.Assert(lid.GelieerdePersoon.Persoon != null);
            Debug.Assert(lid.GroepsWerkJaar != null);
            Debug.Assert(lid.UitschrijfDatum != null);  // we schrijven enkel leden uit als ze uitgeschreven zijn. Ahem.

            var groep = lid.GelieerdePersoon.Groep ?? lid.GroepsWerkJaar.Groep;

            Debug.Assert(groep != null);

            if (lid.GelieerdePersoon.Persoon.AdNummer.HasValue)
            {
                ServiceHelper.CallService<ISyncPersoonService>(svc => svc.LidVerwijderen(
                    lid.GelieerdePersoon.Persoon.AdNummer.Value, 
                    groep.Code, lid.GroepsWerkJaar.WerkJaar, lid.UitschrijfDatum.Value));
            }
            else
            {
                ServiceHelper.CallService<ISyncPersoonService>(svc => svc.NieuwLidVerwijderen(
                    Mapper.Map<GelieerdePersoon, PersoonDetails>(lid.GelieerdePersoon),
                    groep.Code,
                    lid.GroepsWerkJaar.WerkJaar, lid.UitschrijfDatum.Value));
            }
        }

        /// <summary>
        /// Stuurt een aantal leden naar Kipadmin
        /// </summary>
        /// <param name="leden">Te bewaren leden</param>
        public void Bewaren(IList<Lid> leden)
        {
            foreach (var l in leden)
            {
                Bewaren(l);
            }
        }
    }
}
