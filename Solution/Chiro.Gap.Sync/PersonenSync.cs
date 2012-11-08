// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;

using AutoMapper;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

using Adres = Chiro.Gap.Poco.Model.Adres;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync
{
	/// <summary>
	/// Klasse die de synchronisatie van persoonsgegevens naar Kipadmin regelt
	/// </summary>
	public class PersonenSync : IPersonenSync
	{
		/// <summary>
		/// Stuurt de persoonsgegevens, samen met eventueel adressen en/of communicatie, naar Kipadmin
		/// </summary>
		/// <param name="gp">Gelieerde persoon, persoonsinfo</param>
		/// <param name="metStandaardAdres">Stuurt ook het standaardadres mee (moet dan wel gekoppeld zijn)</param>
		/// <param name="metCommunicatie">Stuurt ook communicatie mee.  Hiervoor wordt expliciet alle
		/// communicatie-info opgehaald, omdat de workers typisch niet toestaan dat de gebruiker alle
		/// communicatie ophaalt.</param>
		public void Bewaren(GelieerdePersoon gp, bool metStandaardAdres, bool metCommunicatie)
		{
			// Wijzigingen van personen met ad-nummer worden doorgesluisd
			// naar Kipadmin.

			Debug.Assert(gp.Persoon.AdNummer != null || gp.Persoon.AdInAanvraag);

            var syncPersoon = Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(gp.Persoon);
			ServiceHelper.CallService<ISyncPersoonService>(svc => svc.PersoonUpdaten(syncPersoon));

			if (metStandaardAdres && gp.PersoonsAdres != null)
			{
				// Adressen worden mee bewaard.  Update standaardadres, als dat er is
				// (standaardadres is gelieerdePersoon.PersoonsAdres;
				// alle adressen in gelieerdePersoon.Persoon.PersoonsAdres).

				// TODO (#238): Buitenlandse adressen!
                var syncAdres = Mapper.Map<Adres, Kip.ServiceContracts.DataContracts.Adres>(gp.PersoonsAdres.Adres);

				var syncBewoner = new Bewoner
				{
                    Persoon = Mapper.Map<Persoon, Kip.ServiceContracts.DataContracts.Persoon>(gp.Persoon),
					AdresType = (AdresTypeEnum)gp.PersoonsAdres.AdresType
				};

				ServiceHelper.CallService<ISyncPersoonService>(svc => svc.StandaardAdresBewaren(syncAdres, new List<Bewoner> { syncBewoner }));
			}
			if (metCommunicatie)
			{
                throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Stuurt *alle* communicatie van de persoon gekoppeld aan <paramref name="gp"/> naar Kipadmin 
		/// (dus ook de communicatie gekend bij andere groepen)
		/// </summary>
		/// <param name="gp">Gelieerde persoon</param>
		public void CommunicatieUpdaten(GelieerdePersoon gp)
		{
            throw new NotImplementedException();
		}
	}
}
