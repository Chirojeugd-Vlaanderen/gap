// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
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
	/// Worker die alle businesslogica i.v.m. afdelingsjaren bevat
	/// </summary>
	public class AfdelingsJaarManager
	{
		private readonly IAfdelingsJarenDao _afdJarenDao;
		private readonly IAfdelingenDao _afdelingenDao;
		private readonly IGroepsWerkJaarDao _groepsWjDao;
		private readonly IKindDao _kindDao;
		private readonly ILeidingDao _leidingDao;
		private readonly ILedenSync _ledenSync;

		private readonly IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Deze constructor laat toe om een alternatieve repository voor
		/// de groepen te gebruiken.  Nuttig voor mocking en testing.
		/// </summary>
		/// <param name="ajDao">Zorgt voor data-access ivm afdelnigsjaren</param>
		/// <param name="afdDao">Zorgt voor afdelingsgerelateerde data-access</param>
		/// <param name="gwjDao">Zorgt voor groepswerkjaargerelateerde data-access</param>
		/// <param name="kindDao">Zorgt voor kindgerelateerde data-access</param>
		/// <param name="leidingDao">Zorgt voor leidinggerelateerde data-access</param>
		/// <param name="autorisatieMgr">Alternatieve autorisatiemanager</param>
		public AfdelingsJaarManager(
			IAfdelingsJarenDao ajDao, 
			IAfdelingenDao afdDao,
			IGroepsWerkJaarDao gwjDao,
			IKindDao kindDao,
			ILeidingDao leidingDao,
			IAutorisatieManager autorisatieMgr,
			ILedenSync ledenSync)
		{
			_afdJarenDao = ajDao;
			_afdelingenDao = afdDao;
			_groepsWjDao = gwjDao;
			_kindDao = kindDao;
			_leidingDao = leidingDao;
			_autorisatieMgr = autorisatieMgr;
			_ledenSync = ledenSync;
		}

		/// <summary>
		/// Maakt een afdelingsjaar voor een groep en een afdeling, persisteert niet.
		/// </summary>
		/// <param name="a">Afdeling voor nieuw afdelingsjaar</param>
		/// <param name="oa">Te koppelen officiële afdeling</param>
		/// <param name="gwj">Groepswerkjaar (koppelt de afdeling aan een groep en een werkjaar)</param>
		/// <param name="geboorteJaarBegin">Geboortejaar van</param>
		/// <param name="geboorteJaarEind">Geboortejaar tot</param>
		/// <param name="geslacht">Bepaalt of de afdeling een jongensafdeling, meisjesafdeling of
		/// gemengde afdeling is.</param>
		/// <param name="geenAutomatischeVerdeling">Indien <c>true</c>, wordt dit afdelingsjaar genegeerd
		/// bij de automatische afdelingsverdeling.</param>
		/// <returns>Het aangemaakte afdelingsjaar</returns>
		public AfdelingsJaar Aanmaken(
			Afdeling a,
			OfficieleAfdeling oa,
			GroepsWerkJaar gwj,
			int geboorteJaarBegin,
			int geboorteJaarEind,
			GeslachtsType geslacht,
			bool geenAutomatischeVerdeling)
		{
			if (!_autorisatieMgr.IsGavAfdeling(a.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// Leden moeten minstens in het 1ste leerjaar zitten voor we ze inschrijven.
			// De maximumleeftijd is arbitrair nattevingerwerk. :-)
			if (!(gwj.WerkJaar - geboorteJaarEind >= Properties.Settings.Default.MinLidLeefTijd)
				|| !(gwj.WerkJaar - geboorteJaarBegin <= Properties.Settings.Default.MaxLidLeefTijd)
				|| !(geboorteJaarBegin <= geboorteJaarEind))
			{
				throw new ValidatieException(Properties.Resources.OngeldigeGeboortejarenVoorAfdeling, FoutNummer.FouteGeboortejarenVoorAfdeling);
			}

			var afdelingsJaar = new AfdelingsJaar
			                    	{
			                    		OfficieleAfdeling = oa,
			                    		Afdeling = a,
			                    		GroepsWerkJaar = gwj,
			                    		GeboorteJaarVan = geboorteJaarBegin,
			                    		GeboorteJaarTot = geboorteJaarEind,
			                    		Geslacht = geslacht,
			                    		GeenAutoVerdeling = geenAutomatischeVerdeling
			                    	};

			// TODO check if no conflicts with existing afdelingsjaar

			a.AfdelingsJaar.Add(afdelingsJaar);
			oa.AfdelingsJaar.Add(afdelingsJaar);
			gwj.AfdelingsJaar.Add(afdelingsJaar);

			return afdelingsJaar;
		}

		/// <summary>
		/// Op basis van een ID een afdelingsjaar ophalen
		/// </summary>
		/// <param name="afdelingsJaarID">De ID van het afdelingsjaar</param>
		/// <param name="extras">Bepaalt welke gerelateerde entiteiten mee opgehaald moeten worden.</param>
		/// <returns>Het afdelingsjaar met de opgegeven ID</returns>
		public AfdelingsJaar Ophalen(int afdelingsJaarID, AfdelingsJaarExtras extras)
		{
			var paths = new List<Expression<Func<AfdelingsJaar, object>>>();

			if ((extras & AfdelingsJaarExtras.Afdeling) != 0)
			{
				paths.Add(aj => aj.Afdeling);
			}
			if ((extras & AfdelingsJaarExtras.GroepsWerkJaar) != 0)
			{
				paths.Add(aj => aj.GroepsWerkJaar);
			}
			if ((extras & AfdelingsJaarExtras.Leden) != 0)
			{
				paths.Add(aj => aj.Kind);
				paths.Add(aj => aj.Leiding);
			}
			if ((extras & AfdelingsJaarExtras.OfficieleAfdeling) != 0)
			{
				paths.Add(aj => aj.OfficieleAfdeling);
			}

			if (_autorisatieMgr.IsGavAfdelingsJaar(afdelingsJaarID))
			{
				return _afdJarenDao.Ophalen(afdelingsJaarID, paths.ToArray());
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Verwijdert AfdelingsJaar uit database
		/// </summary>
		/// <param name="afdelingsJaarID">ID van het AfdelingsJaar</param>
		/// <returns><c>True</c> on successful</returns>
		public bool Verwijderen(int afdelingsJaarID)
		{
			AfdelingsJaar aj = _afdJarenDao.Ophalen(afdelingsJaarID, a => a.Afdeling, a => a.Leiding, a => a.Kind);

			if (!_autorisatieMgr.IsGavAfdeling(aj.Afdeling.ID))
							{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			if (aj.Kind.Count != 0 || aj.Leiding.Count != 0)
			{
				throw new InvalidOperationException(Properties.Resources.AfdelingsJaarBevatLeden);
			}
			
			aj.TeVerwijderen = true;
			_afdJarenDao.Bewaren(aj);
			return true;
		}

		/// <summary>
		/// Het opgegeven afdelingsjaar opslaan
		/// </summary>
		/// <param name="aj">Het afdelingsjaar dat opgeslagen moet worden</param>
		public void Bewaren(Afdeling aj)
		{
			if (!_autorisatieMgr.IsGavAfdeling(aj.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);	
			}

			_afdelingenDao.Bewaren(aj);
		}

		/// <summary>
		/// Het opgegeven afdelingsjaar opslaan
		/// </summary>
		/// <param name="aj">Het afdelingsjaar dat opgeslagen moet worden</param>
		public void Bewaren(AfdelingsJaar aj)
		{
			if (_autorisatieMgr.IsGavAfdeling(aj.Afdeling.ID))
			{
				_afdJarenDao.Bewaren(aj);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Wijzigt de property's van <paramref name="afdelingsJaar"/>
		/// </summary>
		/// <param name="afdelingsJaar">Te wijzigen afdelingsjaar</param>
		/// <param name="officieleAfdeling">Officiele afdeling</param>
		/// <param name="geboorteJaarVan">Ondergrens geboortejaar</param>
		/// <param name="geboorteJaarTot">Bovengrens geboortejaar</param>
		/// <param name="geslachtsType">Jongensafdeling, meisjesafdeling of gemengde afdeling</param>
		/// <param name="versieString">Versiestring uit database voor concurrency controle</param>
		/// <param name="geenAutomatischeVerdeling">Indien <c>true</c>, wordt dit afdelingsjaar genegeerd
		/// bij de automatische afdelingsverdeling.</param>
		/// <remarks>Groepswerkjaar en afdeling kunnen niet gewijzigd worden.  Verwijder hiervoor het
		/// afdelingsjaar, en maak een nieuw.</remarks>
		public void Wijzigen(
			AfdelingsJaar afdelingsJaar, 
			OfficieleAfdeling officieleAfdeling, 
			int geboorteJaarVan, 
			int geboorteJaarTot, 
			GeslachtsType geslachtsType, 
			string versieString,
			bool geenAutomatischeVerdeling)
		{
			if (_autorisatieMgr.IsGavAfdelingsJaar(afdelingsJaar.ID))
			{
				if (officieleAfdeling.ID != afdelingsJaar.OfficieleAfdeling.ID)
				{
					// vervang officiele afdeling.  Een beetje gepruts om in de mate van het
					// mogelijke de state van de entity's consistent te houden.

					// verwijder link vorige off. afdeling - afdelingsjaar
					afdelingsJaar.OfficieleAfdeling.AfdelingsJaar.Remove(afdelingsJaar);

					// nieuwe link
					afdelingsJaar.OfficieleAfdeling = officieleAfdeling;
					officieleAfdeling.AfdelingsJaar.Add(afdelingsJaar);
				}
				afdelingsJaar.GeboorteJaarVan = geboorteJaarVan;
				afdelingsJaar.GeboorteJaarTot = geboorteJaarTot;
				afdelingsJaar.Geslacht = geslachtsType;
				afdelingsJaar.VersieString = versieString;
				afdelingsJaar.GeenAutoVerdeling = geenAutomatischeVerdeling;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt een afdeling op, op basis van <paramref name="afdelingID"/>
		/// </summary>
		/// <param name="afdelingID">ID van op te halen afdeling</param>
		/// <returns>De gevraagde afdeling</returns>
		public Afdeling AfdelingOphalen(int afdelingID)
		{
			if (_autorisatieMgr.IsGavAfdeling(afdelingID))
			{
				return _afdelingenDao.Ophalen(afdelingID);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt lijst officiële afdelingen op.
		/// </summary>
		/// <returns>Lijst officiële afdelingen</returns>
		public IList<OfficieleAfdeling> OfficieleAfdelingenOphalen()
		{
			// Iedereen heeft het recht deze op te halen.
			return _afdelingenDao.OfficieleAfdelingenOphalen();
		}

		/// <summary>
		/// Haalt een officiele afdeling op, op basis van zijn ID
		/// </summary>
		/// <param name="officieleAfdelingID">ID van de op te halen officiele afdeling</param>
		/// <returns>Opgehaalde officiele afdeling</returns>
		public OfficieleAfdeling OfficieleAfdelingOphalen(int officieleAfdelingID)
		{
			return _afdelingenDao.OfficieleAfdelingOphalen(officieleAfdelingID);
		}

		/// <summary>
		/// De afdelingen van het gegeven lid worden aangepast van whatever momenteel zijn afdelingen zijn naar
		/// de gegeven lijst nieuwe afdelingen.
		/// Een kind mag maar 1 afdeling hebben, voor een leider staan daar geen constraints op.
		/// Persisteert, want ingeval van leiding kan het zijn dat er links lid->afdelingsjaar moeten 
		/// verdwijnen.
		/// </summary>
		/// <param name="l">Lid, geladen met groepswerkjaar met afdelingsjaren</param>
		/// <param name="afdelingsJaren">De afdelingsjaren waarvan het kind lid is</param>
		/// <returns>Lidobject met gekoppeld(e) afdelingsja(a)r(en)</returns>
		public Lid Vervangen(Lid l, IEnumerable<AfdelingsJaar> afdelingsJaren)
		{
			Debug.Assert(l.GroepsWerkJaar != null);
			Debug.Assert(l.GroepsWerkJaar.Groep != null);

			Lid resultaat;

			if (!_autorisatieMgr.IsGavLid(l.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			else if (l.GroepsWerkJaar.ID != _groepsWjDao.RecentsteOphalen(l.GroepsWerkJaar.Groep.ID).ID)
			{
				throw new FoutNummerException(
					FoutNummer.GroepsWerkJaarNietBeschikbaar,
					Properties.Resources.GroepsWerkJaarVoorbij);
			}

			var probleemgevallen = from aj in afdelingsJaren
					       where aj.GroepsWerkJaar.ID != l.GroepsWerkJaar.ID
					       select aj;

			if (probleemgevallen.FirstOrDefault() != null)
			{
				throw new FoutNummerException(
					FoutNummer.AfdelingNietVanGroep,
					Properties.Resources.FoutieveGroepAfdeling);
			}
#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif

				if (l is Kind)
				{
					var kind = (Kind) l;
					if (afdelingsJaren.Count() != 1)
					{
						throw new NotSupportedException("Slechts 1 afdeling per kind.");
					}

					if (kind.AfdelingsJaar.ID != afdelingsJaren.First().ID)
					{
						// afdeling moet verwijderd worden.

						afdelingsJaren.First().Kind.Add(kind);
						kind.AfdelingsJaar = afdelingsJaren.First();
					}

					_kindDao.Bewaren(kind, knd => knd.AfdelingsJaar.WithoutUpdate());

					// omdat bovenstaande bewaren geen nieuwe ID's zal toekennen, en geen links
					// zal verwijderen, kunnen we met een gerust geweten het originele kind
					// opleveren.

					resultaat = kind;
				}
				else
				{
					var leiding = (Leiding) l;

					// Verwijder ontbrekende afdelingen;
					var teVerwijderenAfdelingen = from aj in leiding.AfdelingsJaar
					                              where !afdelingsJaren.Any(aj2 => aj2.ID == aj.ID)
					                              select aj;

					foreach (var aj in teVerwijderenAfdelingen)
					{
						aj.TeVerwijderen = true;
					}

					// Ken nieuwe afdelingen toe
					var nieuweAfdelingen = from aj in afdelingsJaren
					                       where !leiding.AfdelingsJaar.Any(aj2 => aj2.ID == aj.ID)
					                       select aj;

					foreach (var aj in nieuweAfdelingen)
					{
						leiding.AfdelingsJaar.Add(aj);
						aj.Leiding.Add(leiding);
					}

					// Hier moet je wel met de terugkeerwaarde van 'Bewaren' werken, want anders
					// stuur je afdelingsjaren met TeVerwijderen=true over de lijn. (brr)

					// WithoutUpdate mag niet in dit geval, omdat anders te verwijderen afdelingsjaren niet
					// verwijderd worden.
					// Dit is een bug in AttachObjectGraph. (#116)

					resultaat = _leidingDao.Bewaren(leiding, ldng => ldng.AfdelingsJaar.First());
				}
				if (l.IsOvergezet)
				{
					_ledenSync.AfdelingenUpdaten(resultaat);
				}
#if KIPDORP
				tx.Complete();
			}
#endif
			return resultaat;
		}
	}
}
