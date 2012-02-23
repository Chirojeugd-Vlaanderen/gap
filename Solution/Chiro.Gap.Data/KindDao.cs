// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor kinderen
	/// </summary>
	public class KindDao : Dao<Kind, ChiroGroepEntities>, IKindDao
	{
		/// <summary>
		/// Instantieert een gegevenstoegangsobject voor kinderen
		/// </summary>
		public KindDao()
		{
			ConnectedEntities = new Expression<Func<Kind, object>>[] 
			{ 
				e => e.GroepsWerkJaar.WithoutUpdate(), 
				e => e.GelieerdePersoon.WithoutUpdate(), 
				e => e.GelieerdePersoon.Persoon.WithoutUpdate(), 
				e => e.AfdelingsJaar.WithoutUpdate(),
				e => e.AfdelingsJaar.Afdeling.WithoutUpdate() 
			};
		}

		/// <summary>
		/// Zoekt ingeschreven kinderen op, op basis van de gegeven <paramref name="filter"/>.
		/// </summary>
		/// <param name="filter">De niet-nulle properties van de filter
		/// bepalen waarop gezocht moet worden</param>
		/// <param name="extras">Bepaalt de mee op te halen gekoppelde entiteiten. 
		/// (Adressen ophalen vertraagt aanzienlijk.)
		/// </param>
		/// <returns>Lijst met info over gevonden kinderen</returns>
		/// <remarks>
		/// Er worden enkel actieve leden opgehaald
		/// </remarks>
		public IEnumerable<Kind> Zoeken(LidFilter filter, LidExtras extras)
		{
			Kind[] resultaat;

			if ((filter.LidType & LidType.Kind) == 0)
			{
				// Als er niet op kinderen gezocht wordt, lever dan niets op
				return new Kind[0];
			}
			
			using (var db = new ChiroGroepEntities())
			{
				IQueryable<Kind> query;

				if (filter.AfdelingID != null)
				{
					if (filter.GroepsWerkJaarID != null)
					{
						// Er zal nogal vaak gefilterd worden op een afdeling en een groepswerkjaar.
						// Daarom beginnen we in dit specifieke geval met een geoptimaliseerde
						// query.

						query = db.AfdelingsJaar
							.Where(aj => aj.GroepsWerkJaar.ID == filter.GroepsWerkJaarID && aj.Afdeling.ID == filter.AfdelingID)
							.SelectMany(aj => aj.Kind);
					}
					else
					{
						query = db.AfdelingsJaar
							.Where(aj => aj.Afdeling.ID == filter.AfdelingID)
							.SelectMany(aj => aj.Kind);
					}
				}
				else
				{
					if (filter.GroepsWerkJaarID != null)
					{
						// Alle leden van een gegeven groepswerkjaar zal ook veel voorkomend zijn.

						query = from l in db.Lid.OfType<Kind>()
							where l.GroepsWerkJaar.ID == filter.GroepsWerkJaarID
							select l;
					}
					else
					{
						// Als er geen groepswerkjaar of afdelingsjaar gegeven is, beginnen
						// we vanuit een standaardquery met alle leiding

						query = from ld in db.Lid.OfType<Kind>() select ld;
					}
				}

				// Zo nodig zoekopdracht verfijnen

				if (filter.FunctieID != null)
				{
					query = query.Where(ld => ld.Functie.Any(fn => fn.ID == filter.FunctieID));
				}

				if (filter.GroepID != null)
				{
					query = query.Where(ld => ld.GroepsWerkJaar.Groep.ID == filter.GroepID);
				}

				if (filter.ProbeerPeriodeNa != null)
				{
					query = query.Where(ld => ld.EindeInstapPeriode > filter.ProbeerPeriodeNa);
				}

				if (filter.HeeftVoorkeurAdres != null)
				{
					query = query.Where(ld => filter.HeeftVoorkeurAdres.Value ?
						ld.GelieerdePersoon.PersoonsAdres != null :
						ld.GelieerdePersoon.PersoonsAdres == null);
				}

				if (filter.HeeftTelefoonNummer != null)
				{
					query = query.Where(ld => filter.HeeftTelefoonNummer.Value
									? ld.GelieerdePersoon.Communicatie.Any(
										cm => cm.CommunicatieType.ID == (int)CommunicatieTypeEnum.TelefoonNummer)
									: !ld.GelieerdePersoon.Communicatie.Any(
										cm => cm.CommunicatieType.ID == (int)CommunicatieTypeEnum.TelefoonNummer));
				}

				if (filter.HeeftEmailAdres != null)
				{
					query = query.Where(ld => filter.HeeftEmailAdres.Value
									? ld.GelieerdePersoon.Communicatie.Any(
										cm => cm.CommunicatieType.ID == (int)CommunicatieTypeEnum.Email)
									: !ld.GelieerdePersoon.Communicatie.Any(
										cm => cm.CommunicatieType.ID == (int)CommunicatieTypeEnum.Email));
				}

				var paths = LedenDao.ExtrasNaarLambdasKind(extras);
				resultaat = IncludesToepassen(query as ObjectQuery<Kind>, paths).ToArray();

				if ((extras & LidExtras.Adressen) != 0)
				{
					AdresHelper.AlleAdressenKoppelen(from l in resultaat select l.GelieerdePersoon);
				}
				else if ((extras & LidExtras.VoorkeurAdres) != 0)
				{
					AdresHelper.VoorkeursAdresKoppelen(from l in resultaat select l.GelieerdePersoon);
				}
			}

			return Utility.DetachObjectGraph<Kind>(resultaat);
		}

	    /// <summary>
	    /// Bewaart een kind, inclusief de extras gegeven in <paramref name="extras"/>
	    /// </summary>
	    /// <param name="kind">Het kind dat bewaard moet worden</param>
	    /// <param name="extras">Bepaalt de gekoppelde entiteiten die mee bewaard moeten worden</param>
	    /// <returns>Kopie van het bewaarde kind</returns>
	    public Kind Bewaren(Kind kind, LidExtras extras)
		{
			return Bewaren(kind, LedenDao.ExtrasNaarLambdasKind(extras));
		}
	}
}
