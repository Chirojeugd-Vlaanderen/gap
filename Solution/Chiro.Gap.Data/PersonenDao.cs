// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	public class PersonenDao : Dao<Persoon, ChiroGroepEntities>, IPersonenDao
	{
		#region IPersonenDao Members

		// TODO: onderstaande misschien doen via GroepsWerkJaar ipv via aparte persoon- en groepID?

		public IList<Persoon> LijstOphalen(IList<int> personenIDs)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.Persoon.MergeOption = MergeOption.NoTracking;

				return (
					from p in db.Persoon.Where(Utility.BuildContainsExpression<Persoon, int>(p => p.ID, personenIDs))
					select p).ToList();
			}
		}

		/// <summary>
		/// Haalt alle Personen op die op een zelfde
		/// adres wonen als de gelieerde persoon met het gegeven ID.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gegeven gelieerde
		/// persoon.</param>
		/// <returns>Lijst met Personen (inc. persoonsinfo)</returns>
		/// <remarks>Als de persoon nergens woont, is hij toch zijn eigen
		/// huisgenoot.  Ik geef hier enkel Personen, geen GelieerdePersonen,
		/// omdat ik niet geinteresseerd ben in eventuele dubbels als ik 
		/// GAV ben van verschillende groepen.</remarks>
		public IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
		{
			List<PersoonsAdres> paLijst;
			List<Persoon> resultaat;

			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.PersoonsAdres.MergeOption = MergeOption.NoTracking;

				// FIXME: enkel persoonsadressen van personen van groepen
				// waarvan je GAV bent

				var persoonsAdressen = (
					from pa in db.PersoonsAdres.Include("Persoon")
					where pa.Adres.PersoonsAdres.Any(
					l => l.Persoon.GelieerdePersoon.Any(
					gp => gp.ID == gelieerdePersoonID))
					select pa);

				// Het zou interessant zijn als ik hierboven al 
				// pa.GelieerdePersoon zou 'selecten'.  Maar gek genoeg wordt
				// dan GelieerdePersoon.Persoon niet meegenomen.
				// Als workaround selecteer ik zodadelijk uit
				// persoonsAdressen de GelieerdePersonen.

				paLijst = persoonsAdressen.ToList();

				// Als de persoon nergens woont, dan is deze lijst leeg.  In dat geval
				// halen we gewoon de gelieerde persoon zelf op.
			}

			resultaat = (from pa in paLijst
						 select pa.Persoon).Distinct().ToList();

			if (resultaat.Count == 0)
			{
				// Als de persoon toevallig geen adressen heeft, is het resultaat
				// leeg.  Dat willen we niet; ieder is zijn eigen huisgenoot,
				// ook al woont hij/zij nergens.  Ipv een leeg resultaat,
				// wordt dan gewoon de gevraagde persoon opgehaald.

				resultaat.Add(CorresponderendePersoonOphalen(gelieerdePersoonID));

				// FIXME: Er wordt veel te veel info opgehaald.
			}

			return resultaat;
		}

		/// <summary>
		/// Haalt de persoon op die correspondeert met een gelieerde persoon.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde
		/// persoon.</param>
		/// <returns>Persoon inclusief adresinfo</returns>
		public Persoon CorresponderendePersoonOphalen(int gelieerdePersoonID)
		{
			using (ChiroGroepEntities db = new ChiroGroepEntities())
			{
				db.Persoon.MergeOption = MergeOption.NoTracking;

				return (from foo in db.Persoon.Include("PersoonsAdres.Adres.Straat").Include("PersoonsAdres.Adres.Subgemeente")
						where foo.GelieerdePersoon.Any(bar => bar.ID == gelieerdePersoonID)
						select foo).FirstOrDefault();
			}
		}

		#endregion
	}
}
