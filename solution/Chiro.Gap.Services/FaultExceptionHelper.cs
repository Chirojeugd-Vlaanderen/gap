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
            return new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = Domain.FoutNummer.GeenGav },
                                                       new FaultReason(Properties.Resources.GeenGav));
        }

        /// <summary>
        /// Levert een 'BestaatAl-fout' op, die de service can throwen
        /// </summary>
        /// <typeparam name="TInfo">Een datacontract van de servie</typeparam>
        /// <param name="bestaande">info over het ding dat al bestaat</param>
        /// <returns>Een 'BestaatAl-fout'</returns>
        public static FaultException<BestaatAlFault<TInfo>> BestaatAl<TInfo>(TInfo bestaande)
        {
            // Hier kan misschien best een overload van gemaakt worden, zodat je een reason kunt
            // meegeven.
            return new FaultException<BestaatAlFault<TInfo>>(new BestaatAlFault<TInfo> { Bestaande = bestaande },
                                                             new FaultReason(Properties.Resources.EntiteitBestondAl));
        }

        public static FaultException<FoutNummerFault> FoutNummer(FoutNummer nummer, string reason)
        {
            return new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = nummer }, new FaultReason(reason));
        }

        /*
                    if (ex is GeenGavException)
            {
                throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = FoutNummer.GeenGav }, new FaultReason(ex.Message));
            }
            if (ex is EntityException | ex is EntityCommandExecutionException)
            {
                throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = FoutNummer.GeenDatabaseVerbinding }, new FaultReason(ex.Message));
            }
            if (ex is OptimisticConcurrencyException)
            {
                throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = FoutNummer.Concurrency }, new FaultReason(ex.Message));
            }
            if (ex is FoutNummerException)
            {
                throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = (ex as FoutNummerException).FoutNummer }, new FaultReason(ex.Message));
            }
         */
    }
}
