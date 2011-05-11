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
		/// <param name="contactAd">AD-nummer contactpersoon bivak</param>
		/// <param name="bivak">gegevens voor de bivakaangifte</param>
		public void BivakAangeven(int contactAd, Bivak bivak)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Bivakaangifte
		/// </summary>
		/// <param name="contact">gegevens contactpersoon bivak</param>
		/// <param name="bivak">gegevens voor de bivakaangifte</param>
		/// <remarks>Enkel te gebruiken als het ad-nummer van de contactpersoon niet gekend is.</remarks>
		public void BivakAangevenAdOnbekend(PersoonDetails contact, Bivak bivak)
		{
			throw new NotImplementedException();
		}
	}
}
