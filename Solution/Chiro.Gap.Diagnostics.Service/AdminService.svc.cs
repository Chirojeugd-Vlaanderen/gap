using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using AutoMapper;

using Chiro.Gap.Diagnostics.ServiceContracts;
using Chiro.Gap.Diagnostics.ServiceContracts.DataContracts;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;

namespace Chiro.Gap.Diagnostics.Service
{
    /// <summary>
    /// Webservice voor diagnostische en administratieve zaken
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly GroepenManager _groepenManager;
        private readonly GebruikersRechtenManager _gebruikersRechtenManager;
        private readonly LedenManager _ledenManager;
        private readonly GelieerdePersonenManager _gelieerdePersonenManager;

        /// <summary>
        /// Constructor voor de AdminService.  De workers uit de backend (en ihb
        /// diens dependency's) worden geinjecteerd via de DependencyInjectionBehavior.
        /// </summary>
        /// <param name="groepenManager">Bevat groepsgerelateerde methods van de backend</param>
        /// <param name="gebruikersRechtenManager">Bevat gebruikersrechtgerelateerde methods van de backend</param>
        /// <param name="ledenManager">Bevat ledengerelateerde methods van de backend</param>
        /// <param name="gelieerdePersonenManager">Methods van backend m.b.t. gelieerde personen</param>
        public AdminService(
            GroepenManager groepenManager, 
            GebruikersRechtenManager gebruikersRechtenManager,
            GelieerdePersonenManager gelieerdePersonenManager,
            LedenManager ledenManager)
        {
            _groepenManager = groepenManager;
            _gebruikersRechtenManager = gebruikersRechtenManager;
            _ledenManager = ledenManager;
            _gelieerdePersonenManager = gelieerdePersonenManager;
        }

        /// <summary>
        /// Haalt basisgegevens van de groep met stamnr <paramref name="code"/> op, 
        /// samen met de e-mailadressen van contactpersoon en gekende GAV's
        /// </summary>
        /// <param name="code">stamnummer van de groep</param>
        /// <returns>basisgegevens van de groep, en e-mailadressen van contactpersoon
        /// en gekende GAV's</returns>
        public GroepContactInfo ContactInfoOphalen(string code)
        {
            var groep = _groepenManager.Ophalen(code);
            var result = Mapper.Map<Groep, GroepContactInfo>(groep);

            var contactpersonenMetEmail =
                (from l in _ledenManager.Zoeken(new LidFilter
                                                    {
                                                        FunctieID = (int) NationaleFunctie.ContactPersoon,
                                                        GroepID = groep.ID,
                                                        LidType = LidType.Leiding
                                                    },
                                                LidExtras.Communicatie|LidExtras.Persoon)
                 where !String.IsNullOrEmpty(_gelieerdePersonenManager.EMailKiezen(l.GelieerdePersoon))
                 select new MailContactInfo
                            {
                                GelieerdePersoonID = l.GelieerdePersoon.ID,
                                EmailAdres = _gelieerdePersonenManager.EMailKiezen(l.GelieerdePersoon),
                                IsContact = false,
                                IsGav = false,
                                VolledigeNaam = l.GelieerdePersoon.Persoon.VolledigeNaam
                            }).ToArray();

            var gekendeGavs = (from gp in _gebruikersRechtenManager.GavGelieerdePersonenOphalen(groep.ID)
                               where !String.IsNullOrEmpty(_gelieerdePersonenManager.EMailKiezen(gp))
                               select new MailContactInfo
                                          {
                                              GelieerdePersoonID = gp.ID,
                                              EmailAdres = _gelieerdePersonenManager.EMailKiezen(gp),
                                              IsContact = false,
                                              IsGav = false,
                                              VolledigeNaam = gp.Persoon.VolledigeNaam
                                          }).ToArray();

            result.Contacten = contactpersonenMetEmail.Union(gekendeGavs).Distinct().ToArray();

            // omslachtige loops, maar normaal gezien zal het over een beperkt aantal gaan

            foreach (var c in result.Contacten)
            {
                int gpid = c.GelieerdePersoonID;
                c.IsContact = (contactpersonenMetEmail.Any(cp => cp.GelieerdePersoonID == gpid));
                c.IsGav = (gekendeGavs.Any(cp => cp.GelieerdePersoonID == gpid));
            }
            
            return result;
        }
    }
}
