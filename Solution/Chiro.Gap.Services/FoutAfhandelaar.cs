// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Data;
using System.ServiceModel;

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Static class om algemene fouten te behandelen. De Exception moet als FaultException doorgegeven worden.
    /// </summary>
    public static class FoutAfhandelaar
    {
        /// <summary>
        /// Throw de juiste FaultException naargelang de opgevangen Exception.
        /// </summary>
        /// <param name="ex">De opgevangen Exception</param>
        public static void FoutAfhandelen(Exception ex)
        {
            /*
             * Hier worden algemene fouten opgevangen en op de goede manier doorgegeven. Als de debugger hier ergens breakt, mag je gewoon op F5 duwen om verder te gaan.
             */

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
            
			throw ex;
        }
    }
}
