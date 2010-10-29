// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. Chirogroepen bevat
	/// </summary>
	public class ChiroGroepenManager
	{
		private readonly IDao<ChiroGroep> _dao;
		private readonly IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Creëert een ChiroGroepenManager
		/// </summary>
		/// <param name="dao">Repository voor Chirogroepen</param>
		/// <param name="autorisatieMgr">Regelt de autorisatie</param>
		public ChiroGroepenManager(IDao<ChiroGroep> dao, IAutorisatieManager autorisatieMgr)
		{
			_dao = dao;
			_autorisatieMgr = autorisatieMgr;
		}

		/// <summary>
		/// Haalt een groepsobject op
		/// </summary>
		/// <param name="groepID">ID van de op te halen groep</param>
		/// <param name="extras">Geeft aan of er gekoppelde entiteiten mee opgehaald moeten worden.</param>
		/// <returns>De Chirogroep met de opgegeven ID <paramref name="groepID"/></returns>
		public ChiroGroep Ophalen(int groepID, ChiroGroepsExtras extras)
		{
			if (!_autorisatieMgr.IsGavGroep(groepID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			var paths = ExtrasNaarLambdas(extras);
			return _dao.Ophalen(groepID, paths.ToArray());
		}

		/// <summary>
		/// Bewaart de Chirogroep <paramref name="chiroGroep"/>, met daaraan gekoppeld de
		/// gegeven <paramref name="extras"/>.
		/// </summary>
		/// <param name="chiroGroep">Te bewaren Chirogroep</param>
		/// <param name="extras">Bepaalt de mee te bewaren gekoppelde entiteiten</param>
		public ChiroGroep Bewaren(ChiroGroep chiroGroep, ChiroGroepsExtras extras)
		{
			if (!_autorisatieMgr.IsGavGroep(chiroGroep.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			var paths = ExtrasNaarLambdas(extras);
			return _dao.Bewaren(chiroGroep, paths.ToArray());
		}

		/// <summary>
		/// Maakt een nieuwe afdeling voor een Chirogroep, zonder te persisteren
		/// </summary>
		/// <param name="groep">Chirogroep waarvoor afdeling moet worden gemaakt, met daaraan gekoppeld
		/// de bestaande afdelingen</param>
		/// <param name="naam">Naam van de afdeling</param>
		/// <param name="afkorting">Handige afkorting voor in schemaatjes</param>
		/// <returns>De toegevoegde (maar nog niet gepersisteerde) afdeling</returns>
		public Afdeling AfdelingToevoegen(ChiroGroep groep, string naam, string afkorting)
		{
			if (!_autorisatieMgr.IsGavGroep(groep.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// Controleren of de afdeling nog niet bestaat

			var bestaand = from afd in groep.Afdeling
				       where String.Compare(afd.Afkorting, afkorting, true) == 0
				       || String.Compare(afd.Naam, naam, true) == 0
				       select afd;

			if (bestaand.FirstOrDefault() != null)
			{
				// TODO (#507): Check op bestaande afdeling door DB
				throw new BestaatAlException<Afdeling>(bestaand.FirstOrDefault());
			}

			var a = new Afdeling
			{
				Afkorting = afkorting,
				Naam = naam
			};

			a.ChiroGroep = groep;
			groep.Afdeling.Add(a);

			return a;
		}

		/// <summary>
		/// Converteert ChiroGroepsExtras <paramref name="extras"/> naar lambda-expresses voor een
		/// ChiroGroepenDao
		/// </summary>
		/// <param name="extras">Te converteren Chirogroepsextras</param>
		/// <returns>Lambda-expresses voor een KindDao</returns>
		private static IEnumerable<Expression<Func<ChiroGroep, object>>> ExtrasNaarLambdas(ChiroGroepsExtras extras)
		{
			var paths = new List<Expression<Func<ChiroGroep, object>>>();
			
			if ((extras & ChiroGroepsExtras.GroepsWerkJaren) != 0)
			{
				// Withoutupdate-truuk om te vermijden dat groepswerkjaar overschreven
				// wordt.
				paths.Add(gr => gr.GroepsWerkJaar.First().WithoutUpdate());
			}
			if ((extras & ChiroGroepsExtras.AlleAfdelingen) != 0)
			{
				paths.Add(gr => gr.Afdeling);
			}
			if ((extras & ChiroGroepsExtras.Categorieen) != 0)
			{
				paths.Add(gr => gr.Categorie);
			}

			if ((extras & ChiroGroepsExtras.Functies) != 0)
			{
				paths.Add(gr => gr.Functie);
			}
			return paths;
		}
	}
}
