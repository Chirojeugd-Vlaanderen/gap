using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Businesslogica wat betreft uitstappen en bivakken
	/// </summary>
	public class UitstappenManager
	{
		private readonly IGroepsWerkJaarDao _groepsWerkJaarDao;
		private readonly IAutorisatieManager _autorisatieManager;
		private readonly IUitstappenDao _uitstappenDao;

	    /// <summary>
	    /// Constructor.  De parameters moeten 'ingevuld' via dependency inejction
	    /// </summary>
	    /// <param name="uitstappenDao"></param>
	    /// <param name="groepsWerkJaarDao">Data access voor groepswerkjaren</param>
	    /// <param name="autorisatieManager">Businesslogica voor autorisatie</param>
	    public UitstappenManager(
			IUitstappenDao uitstappenDao, 
			IGroepsWerkJaarDao groepsWerkJaarDao, 
			IAutorisatieManager autorisatieManager)
		{
			_uitstappenDao = uitstappenDao;
			_groepsWerkJaarDao = groepsWerkJaarDao;
			_autorisatieManager = autorisatieManager;
		}

		/// <summary>
		/// Koppelt een uitstap aan een groepswerkjaar.  Dit moet typisch
		/// enkel gebeuren bij een nieuwe uitstap.
		/// </summary>
		/// <param name="uitstap">Te koppelen uitstap</param>
		/// <param name="gwj">Te koppelen groepswerkjaar</param>
		/// <returns><paramref name="uitstap"/>, gekoppeld aan <paramref name="gwj"/>.</returns>
		public Uitstap Koppelen(Uitstap uitstap, GroepsWerkJaar gwj)
		{
			if (!_autorisatieManager.IsGavGroepsWerkJaar(gwj.ID) || !_autorisatieManager.IsGavUitstap(uitstap.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else if (uitstap.GroepsWerkJaar != null && uitstap.GroepsWerkJaar != gwj)
			{
				throw new BlokkerendeObjectenException<GroepsWerkJaar>(
					uitstap.GroepsWerkJaar,
				        Properties.Resources.UitstapAlGekoppeld);
			}
			else if (!_groepsWerkJaarDao.IsRecentste(gwj.ID))
			{
				throw new FoutNummerException(FoutNummer.GroepsWerkJaarNietBeschikbaar, Properties.Resources.GroepsWerkJaarVoorbij);
			}
			else
			{
				uitstap.GroepsWerkJaar = gwj;
				gwj.Uitstap.Add(uitstap);
				return uitstap;
			}
		}

		/// <summary>
		/// Haalt de uitstap met gegeven <paramref name="uitstapID"/> op, inclusief
		/// het gekoppelde groepswerkjaar.
		/// </summary>
		/// <param name="uitstapID">ID op te halen uitstap</param>
		/// <param name="extras"></param>
		/// <returns>De uitstap, met daaraan gekoppeld het groepswerkjaar.</returns>
		public Uitstap Ophalen(int uitstapID, UitstapExtras extras)
		{
			var paths = new List<Expression<Func<Uitstap, object>>>();

			if (!_autorisatieManager.IsGavUitstap(uitstapID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else
			{
				if ((extras & UitstapExtras.Groep) == UitstapExtras.Groep)
				{
					paths.Add(u => u.GroepsWerkJaar.Groep);
				}
				else if ((extras & UitstapExtras.GroepsWerkJaar) == UitstapExtras.GroepsWerkJaar)
				{
					paths.Add(u => u.GroepsWerkJaar);
				}

				if ((extras & UitstapExtras.Deelnemers) == UitstapExtras.Deelnemers)
				{
					paths.Add(u => u.Deelnemer.First().GelieerdePersoon.Persoon.WithoutUpdate());
				}

				if ((extras & UitstapExtras.Plaats) != 0)
				{
					paths.Add(u => u.Plaats.Adres);
				}
				return _uitstappenDao.Ophalen(uitstapID, paths.ToArray());
			}
		}

		/// <summary>
		/// Bewaart de uitstap met het gekoppelde groepswerkjaar
		/// </summary>
		/// <param name="uitstap">Te bewaren uitstap</param>
		/// <param name="extras">Bepaalt de mee te bewaren koppelingen</param>
		public Uitstap Bewaren(Uitstap uitstap, UitstapExtras extras)
		{
			if (!_autorisatieManager.IsGavUitstap(uitstap.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else if (!_groepsWerkJaarDao.IsRecentste(uitstap.GroepsWerkJaar.ID))
			{
				throw new FoutNummerException(FoutNummer.GroepsWerkJaarNietBeschikbaar, Properties.Resources.GroepsWerkJaarVoorbij);
			}
			else
			{
				var koppelingen = new List<Expression<Func<Uitstap, object>>>();

				if ((extras & UitstapExtras.GroepsWerkJaar) != 0)
				{
					koppelingen.Add(u => u.GroepsWerkJaar.WithoutUpdate());
				}
				if ((extras & UitstapExtras.Plaats) != 0)
				{
					koppelingen.Add(u => u.Plaats);
				}
				if ((extras & UitstapExtras.Deelnemers) == UitstapExtras.Deelnemers)
				{
					koppelingen.Add(u => u.Deelnemer.First().GelieerdePersoon.WithoutUpdate());
				}


				return _uitstappenDao.Bewaren(uitstap, koppelingen.ToArray());
			}
		}

		/// <summary>
		/// Haalt alle uitstappen van een gegeven groep op.
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <param name="inschrijvenMogelijk">Als dit <c>true</c> is, worden enkel de gegevens opgehaald
		/// van uitstappen waarvoor nog ingeschreven kan worden.</param>
		/// <returns>Details van uitstappen</returns>
		/// <remarks>Om maar iets te doen, ordenen we voorlopig op einddatum</remarks>
		public IEnumerable<Uitstap> OphalenVanGroep(int groepID, bool inschrijvenMogelijk)
		{
			if (!_autorisatieManager.IsGavGroep(groepID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else
			{
				return _uitstappenDao.OphalenVanGroep(groepID, inschrijvenMogelijk).OrderByDescending(u => u.DatumTot);
			}
		}

		/// <summary>
		/// Koppelt een plaats aan een uitstap
		/// </summary>
		/// <param name="uitstap">Te koppelen uitstap</param>
		/// <param name="plaats">Te koppelen plaats</param>
		/// <returns>Uitstap, met plaats gekoppeld.  Persisteert niet</returns>
		public Uitstap Koppelen(Uitstap uitstap, Plaats plaats)
		{
			if (!_autorisatieManager.IsGavUitstap(uitstap.ID) || !_autorisatieManager.IsGavPlaats(plaats.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else
			{
				plaats.Uitstap.Add(uitstap);
				uitstap.Plaats = plaats;

				return uitstap;
			}
		}

		/// <summary>
		/// Schrijft de gegeven <paramref name="gelieerdePersonen"/> in voor de gegeven
		/// <paramref name="uitstap"/>, al dan niet als <paramref name="logistiekDeelnemer"/>.
		/// </summary>
		/// <param name="uitstap">Uitstap waarvoor in te schrijven, gekoppeld aan groep</param>
		/// <param name="gelieerdePersonen">In te schrijven gelieerde personen, gekoppeld aan groep</param>
		/// <param name="logistiekDeelnemer">Als <c>true</c>, dan worden de 
		/// <paramref name="gelieerdePersonen"/> ingeschreven als logistiek deelnemer.</param>
		public void Inschrijven(Uitstap uitstap, IEnumerable<GelieerdePersoon> gelieerdePersonen, bool logistiekDeelnemer)
		{
			var alleGpIDs = (from gp in gelieerdePersonen select gp.ID).Distinct();
			var mijnGpIDs = _autorisatieManager.EnkelMijnGelieerdePersonen(alleGpIDs);

			if (alleGpIDs.Count() != mijnGpIDs.Count() || !_autorisatieManager.IsGavUitstap(uitstap.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();
			
			Debug.Assert(groepen.Count() > 0); // De gelieerde personen moeten aan een groep gekoppeld zijn.
			Debug.Assert(uitstap.GroepsWerkJaar != null);
			Debug.Assert(uitstap.GroepsWerkJaar.Groep != null);

			// Als er meer dan 1 groep is, dan is er minstens een groep verschillend van de groep
			// van de uitstap (duivenkotenprincipe));););

			if (groepen.Count() > 1 || groepen.First().ID != uitstap.GroepsWerkJaar.Groep.ID)
			{
				throw new FoutNummerException(
					FoutNummer.UitstapNietVanGroep,
					Properties.Resources.FoutieveGroepUitstap);
			}

			if (!_groepsWerkJaarDao.IsRecentste(uitstap.GroepsWerkJaar.ID))
			{
				throw new FoutNummerException(
					FoutNummer.GroepsWerkJaarNietBeschikbaar,
					Properties.Resources.GroepsWerkJaarVoorbij);
			}

			// Als er nu nog geen exception gethrowd is, dan worden eindelijk de deelnemers gemaakt.
			// (koppel enkel de gelieerde personen die nog niet aan de uitstap gekoppeld zijn)

			foreach (var gp in gelieerdePersonen.Where(gp => !gp.Deelnemer.Any(d => d.Uitstap.ID == uitstap.ID)))
			{
				var deelnemer = new Deelnemer
				                	{
				                		GelieerdePersoon = gp,
								Uitstap = uitstap,
				                		HeeftBetaald = false,
				                		IsLogistieker = logistiekDeelnemer,
				                		MedischeFicheOk = false
				                	};
				gp.Deelnemer.Add(deelnemer);
				uitstap.Deelnemer.Add(deelnemer);
			}
		
		}
	}
}
