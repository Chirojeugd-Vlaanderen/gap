// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.ServiceModel;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Services
{
	public static class FoutAfhandelaar
	{
		public static void FoutAfhandelen(Exception ex)
		{
			switch (ex.GetType().Name)
			{
				case "GeenGavException":
					throw new FaultException<GapFault>(new FoutNummerFault { FoutNummer = FoutNummer.GeenGav }, new FaultReason(ex.Message));
				case "EntityException":
				case "EntityCommandExecutionException":
					throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = FoutNummer.GeenDatabaseVerbinding }, new FaultReason(ex.Message));
					// de volgende throws moeten nog een fault meegeven
				case "OptimisticConcurrencyException":
					throw new FaultException(ex.Message, new FaultCode("Optimistic Concurrency Exception"));
				default:
					throw new GapException("Niet-geanticipeerde fout");
			}
		}
	}
}
