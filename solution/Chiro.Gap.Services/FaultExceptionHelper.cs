using System.ServiceModel;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Klasse die op te werpen FaultExceptions aflevert. Dit om de code leesbaarder te houden.
    /// </summary>
    public static class FaultExceptionHelper
    {
        /// <summary>
        /// Levert een GeenGav-fout op die de service kan throwen.
        /// </summary>
        /// <returns>Een GeenGav-fout</returns>
        public static FaultException<FoutNummerFault> GeenGav()
        {
            return new FaultException<FoutNummerFault>(new FoutNummerFault {FoutNummer = FoutNummer.GeenGav},
                                                       new FaultReason(Properties.Resources.GeenGav));
        }
    }
}
