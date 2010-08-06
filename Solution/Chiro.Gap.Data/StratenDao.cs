// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor straten
	/// </summary>
	public class StratenDao : Dao<StraatNaam, ChiroGroepEntities>, IStratenDao
	{
		/// <summary>
		/// Haalt het straatnaamobject op dat staat voor de gevraagde straat(naam) 
		/// in de gemeente met het gegeven postnummer
		/// </summary>
		/// <param name="naam">De straatnaam</param>
		/// <param name="postNr">Het postnummer</param>
		/// <returns>Het straatnaamobject waar de gegeven naam gekoppeld is aan het gegeven postnummer</returns>
		public StraatNaam Ophalen(string naam, int postNr)
		{
			StraatNaam resultaat = null;

			using (var db = new ChiroGroepEntities())
			{
				resultaat = (
					from StraatNaam s in db.StraatNaam
					where s.Naam == naam && s.PostNummer == postNr
					select s).FirstOrDefault();

				if (resultaat != null)
				{
					resultaat = Utility.DetachObjectGraph(resultaat);
				}
			}

			return resultaat;
		}

		/// <summary>
		/// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
		/// met het gegeven <paramref name="naamBegin"/>.
		/// </summary>
		/// <param name="naamBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNr">Postnummer waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		public IList<StraatNaam> MogelijkhedenOphalen(string naamBegin, int postNr)
		{
			return MogelijkhedenOphalen(naamBegin, new[] { postNr });
		}

		/// <summary>
		/// Haalt alle straten op uit een gegeven rij <paramref name="postNrs"/>, waarvan de naam begint
		/// met het gegeven <paramref name="naamBegin"/>.
		/// </summary>
		/// <param name="naamBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNrs">Postnummers waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		public IList<StraatNaam> MogelijkhedenOphalen(string naamBegin, IEnumerable<int> postNrs)
		{
			IList<StraatNaam> resultaat = null;

			using (var db = new ChiroGroepEntities())
			{
				resultaat = db.StraatNaam
					.Where(Utility.BuildContainsExpression<StraatNaam, int>(str => str.PostNummer, postNrs))
					.Where(str => str.Naam.StartsWith(naamBegin))
					.Select(str => str)
					.ToList();

				if (resultaat != null)
				{
					resultaat = Utility.DetachObjectGraph(resultaat);
				}
			}

			return resultaat;
		}
	}
}
