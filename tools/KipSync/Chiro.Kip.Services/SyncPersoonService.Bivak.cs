using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Kip.Services
{
	public partial class SyncPersoonService
	{
		/// <summary>
		/// Bivakaangifte
		/// </summary>
		/// <param name="bivak">gegevens voor de bivakaangifte</param>
		public void BivakBewaren(Bivak bivak)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Bewaart <paramref name="plaatsNaam"/> en <paramref name="adres"/> voor een bivak
		/// in Kipadmin.
		/// </summary>
		/// <param name="uitstapID">ID van de uitstap in GAP</param>
		/// <param name="plaatsNaam">naam van de bivakplaats</param>
		/// <param name="adres">adres van de bivakplaats</param>
		public void BivakPlaatsBewaren(int uitstapID, string plaatsNaam, Adres adres)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Stelt de persoon met gegeven <paramref name="adNummer"/> in als contactpersoon voor
		/// het bivak met gegeven <paramref name="uitstapID"/>
		/// </summary>
		/// <param name="uitstapID">UitstapID (GAP) voor het bivak</param>
		/// <param name="adNummer">AD-nummer contactpersoon bivak</param>
		public void BivakContactBewaren(int uitstapID, int adNummer)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Stelt de persoon met gegeven <paramref name="details"/> in als contactpersoon voor
		/// het bivak met gegeven <paramref name="uitstapID"/>
		/// </summary>
		/// <param name="uitstapID">UitstapID (GAP) voor het bivak</param>
		/// <param name="details">gegevens van de persoon</param>
		/// <remarks>Deze method mag enkel gebruikt worden als het ad-nummer van de
		/// persoon onbestaand of onbekend is.</remarks>
		public void BivakContactBewarenAdOnbekend(int uitstapID, PersoonDetails details)
		{
			throw new NotImplementedException();
		}
	}
}
