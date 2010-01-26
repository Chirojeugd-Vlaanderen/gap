using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Workers;

namespace Chiro.WatiN.Test
{
	/// <summary>
	/// Een aantal helperfuncties voor de WatiN-tests
	/// </summary>
	public static class TestHelper
	{
		/// <summary>
		/// Voor de opkuis gebruiken we de IOC-container.  Bij de constructie van deze
		/// klasse willen we dus dat die geinitialiseerd is.
		/// </summary>
		static TestHelper()
		{
			Factory.ContainerInit();
		}

		/// <summary>
		/// Roept de businesslaag rechtstreeks op (zonder user interface) om de alle (gelieerde) personen
		/// van een testgroep te verwijderen
		/// </summary>
		/// <param name="groepID">ID van de op te kuisen groep</param>
		public static void KuisOp(int groepID)
		{
			GroepenManager gMgr = Factory.Maak<GroepenManager>();

			gMgr.GelieerdePersonenVerwijderen(groepID, true);
		}
	}
}
