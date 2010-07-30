// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.ServiceModel;

using Chiro.Gap.Domain;
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
			switch (ex.GetType().Name)
			{
				/*
				 * Hier worden algemene fouten opgevangen en op de goede manier doorgegeven. Als de debugger hier ergens breakt, 
				 * mag je gewoon op F5 duwen om verder te gaan.
				 */
				case "GeenGavException":
					throw new FaultException<GapFault>(new FoutNummerFault { FoutNummer = FoutNummer.GeenGav }, new FaultReason(ex.Message));
				case "EntityException":
				case "EntityCommandExecutionException":
					throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = FoutNummer.GeenDatabaseVerbinding }, new FaultReason(ex.Message));
				case "OptimisticConcurrencyException":
					throw new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = FoutNummer.Concurrecncy }, new FaultReason(ex.Message));
				default:
					throw new FaultException<GapFault>(new GapFault(), new FaultReason(ex.Message));
			}
		}
	}
}
