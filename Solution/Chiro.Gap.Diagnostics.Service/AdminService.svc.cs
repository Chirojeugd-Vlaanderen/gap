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

        /// <summary>
        /// Constructor voor de AdminService.  De workers uit de backend (en ihb
        /// diens dependency's) worden geinjecteerd via de DependencyInjectionBehavior.
        /// </summary>
        /// <param name="groepenManager">Bevat groepsgerelateerde methods van de backend</param>
        public AdminService(GroepenManager groepenManager)
        {
            _groepenManager = groepenManager;
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

            // TODO: de contactinfo nog ophalen ;-)

            return result;
        }
    }
}
