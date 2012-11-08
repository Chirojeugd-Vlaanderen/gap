// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Diagnostics;

using AutoMapper;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

using CommunicatieType = Chiro.Kip.ServiceContracts.DataContracts.CommunicatieType;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Regelt de synchronisatie van communicatiemiddelen naar Kipadmin
	/// </summary>
	public class CommunicatieSync : ICommunicatieSync
	{
		/// <summary>
		/// Verwijdert een communicatievorm uit Kipadmin
		/// </summary>
		/// <param name="communicatieVorm">Te verwijderen communicatievorm, gekoppeld aan een gelieerde persoon 
		/// met ad-nummer.</param>
		public void Verwijderen(CommunicatieVorm communicatieVorm)
		{
			Debug.Assert(communicatieVorm.GelieerdePersoon != null);
			Debug.Assert(communicatieVorm.GelieerdePersoon.Persoon != null);

            ServiceHelper.CallService<ISyncPersoonService>(
		        svc =>
		        svc.CommunicatieVerwijderen(
		            Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(communicatieVorm.GelieerdePersoon.Persoon),
		            new CommunicatieMiddel
		                {
		                    Type = (CommunicatieType)communicatieVorm.CommunicatieType.ID,
		                    Waarde = communicatieVorm.Nummer
		                }));
		}

		/// <summary>
		/// Bewaart een communicatievorm in Kipadmin
		/// </summary>
        /// <param name="communicatieVorm">Te bewaren communicatievorm, gekoppeld aan persoon</param>
		public void Toevoegen(CommunicatieVorm communicatieVorm)
		{
			Debug.Assert(communicatieVorm.GelieerdePersoon != null);
			Debug.Assert(communicatieVorm.GelieerdePersoon.Persoon != null);

		    ServiceHelper.CallService<ISyncPersoonService>(
		        svc =>
		        svc.CommunicatieToevoegen(
		            Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(
		                communicatieVorm.GelieerdePersoon.Persoon),
		            new CommunicatieMiddel
		                {
		                    Type = (CommunicatieType)communicatieVorm.CommunicatieType.ID,
		                    Waarde = communicatieVorm.Nummer,
		                    GeenMailings = !communicatieVorm.IsVoorOptIn
		                }));
		}
	}
}
