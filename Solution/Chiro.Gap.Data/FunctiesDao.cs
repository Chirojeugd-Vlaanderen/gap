using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Data access object voor functies
	/// </summary>
	/// <remarks>Probeer met een functie ALTIJD ZIJN GROEP mee op te halen.  Want een functie met groep null,
	/// is een nationaal gedefinieerde functie.</remarks>
	public class FunctiesDao: Dao<Functie, ChiroGroepEntities>, IFunctiesDao
	{
		#region IFunctiesDao Members

		/// <summary>
		/// Standaard wordt de link met de Groep vastgelegd in ConnectedEntities
		/// </summary>
		public FunctiesDao()
		{
			connectedEntities = new System.Linq.Expressions.Expression<Func<Functie, object>>[]{
				fnc => fnc.Groep};
		}


		/// <summary>
		/// Haalt een gepredefinieerde functie op
		/// </summary>
		/// <param name="f">GepredefinieerdeFunctieType dat de op te halen functie bepaalt</param>
		/// <returns>De gevraagde gepredefinieerde functie</returns>
		public Functie Ophalen(GepredefinieerdeFunctieType f)
		{
			return Ophalen((int)f);
		}

		#endregion
	}
}
