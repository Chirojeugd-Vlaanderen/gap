using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;

using AutoMapper;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Services
{
	public class UitstappenService : IUitstappenService
	{
		private readonly UitstappenManager _uitstappenMgr;
		private readonly GroepsWerkJaarManager _groepsWerkJaarMgr;
		private readonly PlaatsenManager _plaatsenMgr;
		private readonly AdressenManager _adressenMgr;

		/// <summary>
		/// Constructor.  De managers moeten m.b.v. dependency injection gecreeerd worden.
		/// </summary>
		/// <param name="uMgr">Uitstappenmanager</param>
		/// <param name="gwjMgr">Groepswerkjaarmanager</param>
		/// <param name="plMgr">Plaatsenmanager</param>
		/// <param name="adMgr">Adressenmanager</param>
		public UitstappenService(UitstappenManager uMgr, GroepsWerkJaarManager gwjMgr, PlaatsenManager plMgr, AdressenManager adMgr)
		{
			_uitstappenMgr = uMgr;
			_groepsWerkJaarMgr = gwjMgr;
			_plaatsenMgr = plMgr;
			_adressenMgr = adMgr;
		}

		/// <summary>
		/// Bewaart een uitstap aan voor de groep met gegeven <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep horende bij de uitstap.
		///   Is eigenlijk enkel relevant als het om een nieuwe uitstap gaat.</param>
		/// <param name="info">Details over de uitstap.  Als <c>uitstap.ID</c> <c>0</c> is,
		///   dan wordt een nieuwe uitstap gemaakt.  Anders wordt de bestaande overschreven.</param>
		/// <returns>ID van de uitstap</returns>
		public int Bewaren(int groepID, UitstapInfo info)
		{
			// Als de uitstap een ID heeft, moet een bestaande uitstap worden opgehaald.
			// Anders maken we een nieuwe.

			Uitstap uitstap;

			if (info.ID == 0)
			{
				var gwj = _groepsWerkJaarMgr.RecentsteOphalen(groepID, GroepsWerkJaarExtras.Groep);
				uitstap = Mapper.Map<UitstapInfo, Uitstap>(info);
				_uitstappenMgr.Koppelen(uitstap, gwj);
			}
			else
			{
				// haal origineel op, gekoppeld aan groepswerkjaar
				uitstap = _uitstappenMgr.Ophalen(info.ID, UitstapExtras.GroepsWerkJaar);
				// overschrijf met gegevens uit 'info'
				Mapper.Map(info, uitstap);
			}

			try
			{
				return _uitstappenMgr.Bewaren(uitstap,  UitstapExtras.Geen).ID;
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

		/// <summary>
		/// Haalt alle uitstappen van een gegeven groep op.
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <returns>Details van uitstappen</returns>
		public IEnumerable<UitstapInfo> OphalenVanGroep(int groepID)
		{
			try
			{
				return Mapper.Map<IEnumerable<Uitstap>, IEnumerable<UitstapInfo>>(_uitstappenMgr.OphalenVanGroep(groepID));
			}
			catch (GeenGavException ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				throw;
			}
		}

		/// <summary>
		/// Haalt details over uitstap met gegeven <paramref name="uitstapID"/> op.
		/// </summary>
		/// <param name="uitstapID">ID van de uitstap</param>
		/// <returns>Details over de uitstap</returns>
		public UitstapDetail DetailsOphalen(int uitstapID)
		{
			try
			{
				var uitstap = _uitstappenMgr.Ophalen(uitstapID, UitstapExtras.GroepsWerkJaar | UitstapExtras.Plaats);
				var uitstapDetail = Mapper.Map<Uitstap, UitstapDetail>(uitstap);
				return uitstapDetail;
			}
			catch (GeenGavException ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				throw;
			}
		}

		/// <summary>
		/// Updatet de plaats voor de uitstap met gegeven <paramref name="uitstapID"/>
		/// </summary>
		/// <param name="uitstapID">ID van uitstap waarvan plaats geüpdatet moet worden</param>
		/// <param name="plaatsNaam">Naam van de plaats</param>
		/// <param name="adresInfo">Adres van de plaats</param>
		public void PlaatsBewaren(int uitstapID, string plaatsNaam, AdresInfo adresInfo)
		{
			Uitstap uitstap;
			Plaats plaats;

			try
			{
				uitstap = _uitstappenMgr.Ophalen(uitstapID, UitstapExtras.Groep|UitstapExtras.Plaats);
				if (uitstap == null)
				{
					// Als er geen uitstap is gevonden, dan is de gebruiker waarschijnlijk iets
					// aan het doen dat niet mag.  Bij deze een algemene exception.
					throw new GeenGavException();
				}
			}
			catch (GeenGavException ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				throw;
			}

			Debug.Assert(uitstap != null);

			Adres adres = _adressenMgr.ZoekenOfMaken(adresInfo);

			try
			{
				plaats = _plaatsenMgr.ZoekenOfMaken(uitstap.GroepsWerkJaar.Groep.ID, plaatsNaam, adres.ID);
			}
			catch (OngeldigObjectException ex)
			{
				// Verkeerde straatnaam of zo
				var fault = Mapper.Map<OngeldigObjectException, OngeldigObjectFault>(ex);
				throw new FaultException<OngeldigObjectFault>(fault);
			}

			Debug.Assert(plaats != null);

			try
			{
				uitstap = _uitstappenMgr.Koppelen(uitstap, plaats);
			}
			catch (BlokkerendeObjectenException<Plaats> ex)
			{
				// Was al gekoppeld

				var fault = Mapper.Map<BlokkerendeObjectenException<Plaats>, BlokkerendeObjectenFault<PlaatsInfo>>(ex);
				throw new FaultException<BlokkerendeObjectenFault<PlaatsInfo>>(fault);
			}

			_uitstappenMgr.Bewaren(uitstap, UitstapExtras.Plaats);
		}
	}
}
