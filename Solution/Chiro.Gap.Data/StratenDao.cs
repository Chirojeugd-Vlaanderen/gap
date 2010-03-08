// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	public class StratenDao : Dao<Straat, ChiroGroepEntities>, IStratenDao
	{
		public Straat Ophalen(string naam, int postNr)
		{
			Straat resultaat = null;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				resultaat = (
					from Straat s in db.Straat
					where s.Naam == naam && s.PostNr == postNr
					select s).FirstOrDefault<Straat>();

				if (resultaat != null)
				{
					resultaat = Utility.DetachObjectGraph(resultaat);
				}
			}

			return resultaat;
		}

		/// <summary>
		/// Haalt alle straten op uit een gegeven <paramref name="postNr"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="naamBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNr">Postnummer waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		public IList<Straat> MogelijkhedenOphalen(string naamBegin, int postNr)
		{
			return MogelijkhedenOphalen(naamBegin, new int[] { postNr });
		}

		/// <summary>
		/// Haalt alle straten op uit een gegeven rij <paramref name="postNrs"/>, waarvan de naam begint
		/// met het gegeven <paramref name="straatBegin"/>.
		/// </summary>
		/// <param name="naamBegin">Eerste letters van de te zoeken straatnamen</param>
		/// <param name="postNrs">Postnummers waarin te zoeken</param>
		/// <returns>Gegevens van de gevonden straten</returns>
		public IList<Straat> MogelijkhedenOphalen(string naamBegin, IEnumerable<int> postNrs)
		{
			IList<Straat> resultaat = null;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				resultaat = db.Straat
					.Where(Utility.BuildContainsExpression<Straat, int>(str => str.PostNr, postNrs))
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
