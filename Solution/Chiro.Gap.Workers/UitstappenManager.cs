using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
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
		private readonly IBivakSync _sync;

		/// <summary>
		/// Constructor.  De parameters moeten 'ingevuld' via dependency inejction
		/// </summary>
		/// <param name="uitstappenDao"></param>
		/// <param name="groepsWerkJaarDao">Data access voor groepswerkjaren</param>
		/// <param name="autorisatieManager">Businesslogica voor autorisatie</param>
		/// <param name="sync">Proxy naar service om bivakaangiftes te syncen met Kipadmin</param>
		public UitstappenManager(
			IUitstappenDao uitstappenDao,
			IGroepsWerkJaarDao groepsWerkJaarDao,
			IAutorisatieManager autorisatieManager,
			IBivakSync sync)
		{
			_uitstappenDao = uitstappenDao;
			_groepsWerkJaarDao = groepsWerkJaarDao;
			_autorisatieManager = autorisatieManager;
			_sync = sync;
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

				if ((extras & UitstapExtras.Contact) == UitstapExtras.Contact)
				{
					paths.Add(u => u.ContactDeelnemer.WithoutUpdate());
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
		/// <param name="sync">Als <c>true</c>, wordt de uitstap gesynct naar Kipadmin.</param>
		/// <remarks>Groepswerkjaar wordt altijd mee bewaard.
		/// De parameter <paramref name="sync"/> staat erbij om te vermijden dat voor het
		/// bewaren van een koppeling een (ongewijzigd) bivak mee over de lijn moet.
		/// </remarks>
		public Uitstap Bewaren(Uitstap uitstap, UitstapExtras extras, bool sync)
		{
			// kopieer eerst een aantal gekoppelde entiteiten (voor sync straks), want na het bewaren van 
			// het bivak zijn we die kwijt...

			var groep = uitstap.GroepsWerkJaar == null ? null : uitstap.GroepsWerkJaar.Groep;
			var plaats = uitstap.Plaats ?? null;

			Debug.Assert(uitstap.GroepsWerkJaar != null);


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
				// Sowieso groepswerkjaar koppelen.

				Debug.Assert(uitstap.GroepsWerkJaar != null);
				var koppelingen = new List<Expression<Func<Uitstap, object>>> { u => u.GroepsWerkJaar.WithoutUpdate() };

				if ((extras & UitstapExtras.Plaats) != 0)
				{
					koppelingen.Add(u => u.Plaats);
				}
				if ((extras & UitstapExtras.Deelnemers) == UitstapExtras.Deelnemers)
				{
					koppelingen.Add(u => u.Deelnemer.First().GelieerdePersoon.WithoutUpdate());
				}
				if ((extras & UitstapExtras.Contact) == UitstapExtras.Contact)
				{
					koppelingen.Add(u => u.ContactDeelnemer.WithoutUpdate());
				}
#if KIPDORP
				using (var tx = new TransactionScope())
				{
#endif
					uitstap = _uitstappenDao.Bewaren(uitstap, koppelingen.ToArray());
					if (uitstap.IsBivak && sync)
					{
						// De 'Bewaren' hierboven heeft tot gevolg dat de groep niet
						// meer gekoppeld is (wegens onderliggende beperkingen van
						// AttachObjectGraph).  Vandaar dat we voor het gemak
						// die groep hier opnieuw koppelen.
						// Idem voor plaats

						// Opgelet.  Dit is inconsistent gedrag van de software :-(

						uitstap.GroepsWerkJaar.Groep = groep;
						uitstap.Plaats = plaats;
						_sync.Bewaren(uitstap);
					}
					else if (sync)
					{
						// Dit om op te vangen dat een bivak afgevinkt wordt als bivak.
						// TODO: betere manier bedenken

						_sync.Verwijderen(uitstap.ID);
					}
#if KIPDORP
					tx.Complete();
				}
#endif
				return uitstap;
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

		/// <summary>
		/// Haalt de deelnemers (incl. lidgegevens van het betreffende groepswerkjaar)
		/// van de gegeven uitstap op.
		/// </summary>
		/// <param name="uitstapID">ID van uitstap waarvan deelnemers op te halen zijn</param>
		/// <returns>De deelnemers van de gevraagde uitstap.</returns>
		public IEnumerable<Deelnemer> DeelnemersOphalen(int uitstapID)
		{
			if (!_autorisatieManager.IsGavUitstap(uitstapID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else
			{
				return _uitstappenDao.DeelnemersOphalen(uitstapID);
			}
		}

		/// <summary>
		/// Stuurt alle bivakken van werkjaar <paramref name="werkjaar"/> opnieuw naar
		/// kipadmin.
		/// </summary>
		/// <param name="werkjaar">Werkjaar</param>
		public void OpnieuwSyncen(int werkjaar)
		{
			if (!_autorisatieManager.IsSuperGav())
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else
			{
				var alles = _uitstappenDao.AlleBivakkenOphalen(werkjaar);

				foreach (var bivak in alles)
				{
					_sync.Bewaren(bivak);
				}
			}
		}
	}
}
