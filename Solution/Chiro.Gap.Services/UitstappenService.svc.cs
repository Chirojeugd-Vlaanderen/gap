using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using AutoMapper;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Services
{
	public class UitstappenService : IUitstappenService
	{
		private readonly UitstappenManager _uitstappenMgr;
		private readonly GroepsWerkJaarManager _groepsWerkJaarMgr;


		/// <summary>
		/// Constructor.  De managers moeten m.b.v. dependency injection gecreeerd worden.
		/// </summary>
		/// <param name="uMgr">Uitstappenmanager</param>
		/// <param name="gwjMgr">Groepswerkjaarmanager</param>
		public UitstappenService(UitstappenManager uMgr, GroepsWerkJaarManager gwjMgr)
		{
			_uitstappenMgr = uMgr;
			_groepsWerkJaarMgr = gwjMgr;
		}

		/// <summary>
		/// Bewaart een uitstap aan voor de groep met gegeven <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep horende bij de uitstap.
		/// Is eigenlijk enkel relevant als het om een nieuwe uitstap gaat.</param>
		/// <param name="detail">Details over de uitstap.  Als <c>uitstap.ID</c> <c>0</c> is,
		/// dan wordt een nieuwe uitstap gemaakt.  Anders wordt de bestaande overschreven.</param>
		/// <returns>ID van de uitstap</returns>
		public int Bewaren(int groepID, UitstapDetail detail)
		{
			// Als de uitstap een ID heeft, moet een bestaande uitstap worden opgehaald.
			// Anders maken we een nieuwe.

			Uitstap uitstap;

			if (detail.ID == 0)
			{
				var gwj = _groepsWerkJaarMgr.RecentsteOphalen(groepID, GroepsWerkJaarExtras.Groep);
				uitstap = Mapper.Map<UitstapDetail, Uitstap>(detail);
				_uitstappenMgr.Koppelen(uitstap, gwj);
			}
			else
			{
				// haal origineel op, gekoppeld aan groepswerkjaar
				uitstap = _uitstappenMgr.Ophalen(detail.ID);
				// overschrijf met gegevens uit 'detail'
				Mapper.Map<UitstapDetail, Uitstap>(detail, uitstap);
			}

			try
			{
				return _uitstappenMgr.Bewaren(uitstap).ID;
			}
			// Afhandelen van verwachte exceptions
			catch (GeenGavException ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);

				throw;	
				// Enkel pro forma, want FoutAfhandelen throwt normaal gesproeken
				// een fault.
			}
			catch (FoutNummerException ex)
			{
				if (ex.FoutNummer == FoutNummer.GroepsWerkJaarNietBeschikbaar)
				{
					FoutAfhandelaar.FoutAfhandelen(ex);
				}

				// Onverwachte exceptions gewoon terug throwen, zodat we ze tegen komen
				// bij het debuggen.

				throw;
			}
		}
	}
}
