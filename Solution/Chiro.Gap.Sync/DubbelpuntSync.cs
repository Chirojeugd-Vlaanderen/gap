// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Diagnostics;
using AutoMapper;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Klasse voor de synchronisatie van Dubbelpuntabonnementen
	/// </summary>
	public class DubbelpuntSync : IDubbelpuntSync
	{
	    /// <summary>
	    /// Synct een Dubbelpuntabonnement naar Kipadmin
	    /// </summary>
        /// <param name="abonnement">Te syncen abonnement</param>
	    public void Abonneren(Abonnement abonnement)
		{
			Debug.Assert(abonnement.GelieerdePersoon != null);
			Debug.Assert(abonnement.GelieerdePersoon.Persoon != null);
            Debug.Assert(abonnement.GroepsWerkJaar != null);
            Debug.Assert(abonnement.GroepsWerkJaar.Groep != null);

	        var gp = abonnement.GelieerdePersoon;
	        var huidigWj = abonnement.GroepsWerkJaar;

			if (gp.Persoon.AdNummer != null)
			{
				// Jeeej! We hebben een AD-nummer! Dubbelpuntabonnement is een fluitje van een cent.

				ServiceHelper.CallService<ISyncPersoonService>(svc => svc.DubbelpuntBestellen(
					gp.Persoon.AdNummer ?? 0,
					huidigWj.Groep.Code,
					huidigWj.WerkJaar));
			}
			else
			{
                throw new NotImplementedException();
			}
		}
	}
}
