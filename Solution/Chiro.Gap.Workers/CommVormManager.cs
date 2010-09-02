// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if KIPDORP
using System.Transactions;
#endif

using Chiro.Cdf.Data;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Validatie;
using Chiro.Gap.Workers.Exceptions;

using CommunicatieType = Chiro.Gap.Orm.CommunicatieType;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. communicatievormen bevat (telefoonnummer, mailadres, enz.)
	/// </summary>
	public class CommVormManager
	{
		private readonly IDao<CommunicatieType> _typedao;
		private readonly IDao<CommunicatieVorm> _dao;
		private readonly IAutorisatieManager _autorisatieMgr;
		private readonly ICommunicatieSync _communicatieSync;
		private readonly IPersonenSync _personenSync;

		/// <summary>
		/// Deze constructor laat toe om een alternatieve repository voor
		/// de communicatievormen te gebruiken.  Nuttig voor mocking en testing.
		/// </summary>
		/// <param name="typedao">Repository voor communicatietypes</param>
		/// <param name="commdao">Repository voor communicatievormen</param>
		/// <param name="autorisatieMgr">Worker die autorisatie regelt</param>
		/// <param name="communicatieSync">Syncer naar Kipadmin voor communicatiemiddelen</param>
		public CommVormManager(
			IDao<CommunicatieType> typedao, 
			IDao<CommunicatieVorm> commdao, 
			IAutorisatieManager autorisatieMgr,
			ICommunicatieSync communicatieSync,
			IPersonenSync personenSync)
		{
			_typedao = typedao;
			_dao = commdao;
			_autorisatieMgr = autorisatieMgr;
			_communicatieSync = communicatieSync;
			_personenSync = personenSync;
		}

		/// <summary>
		/// Haalt communicatievorm op, op basis van commvormID
		/// </summary>
		/// <param name="commvormID">ID op te halen communicatievorm</param>
		/// <returns>Gevraagde communicatievorm</returns>
		public CommunicatieVorm Ophalen(int commvormID)
		{
			if (!_autorisatieMgr.IsGavCommVorm(commvormID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			return _dao.Ophalen(commvormID, foo => foo.CommunicatieType);
		}

		/// <summary>
		/// Haalt gelieerdepersoon en zijn gelinkte commvormen, gegeven een ID van een van zijn commvormen
		/// </summary>
		/// <param name="commvormID">ID van een van de persoon zijn communicatievormen</param>
		/// <returns>Gevraagde communicatievorm</returns>
		public CommunicatieVorm OphalenMetGelieerdePersoon(int commvormID)
		{
			if (!_autorisatieMgr.IsGavCommVorm(commvormID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			return _dao.Ophalen(commvormID, foo => foo.CommunicatieType,
											foo => foo.GelieerdePersoon.Persoon,
											foo => foo.GelieerdePersoon.Communicatie,
											foo => foo.GelieerdePersoon.Communicatie.First().CommunicatieType);
		}

		/// <summary>
		/// Persisteert communicatievorm in de database
		/// </summary>
		/// <param name="commvorm">Te persisteren communicatievorm</param>
		/// <returns>De bewaarde communicatievorm</returns>
		/// <remarks>Persoon moet gekoppeld zijn</remarks>
		public CommunicatieVorm Bewaren(CommunicatieVorm commvorm)
		{
			bool isNieuw = (commvorm.ID == 0);

			CommunicatieVorm resultaat;

			Debug.Assert(commvorm.GelieerdePersoon != null);
			Debug.Assert(commvorm.GelieerdePersoon.Persoon != null);

			if (!_autorisatieMgr.IsGavCommVorm(commvorm.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
				resultaat = _dao.Bewaren(
					commvorm, 
					cv => cv.GelieerdePersoon.WithoutUpdate(),
					cv => cv.CommunicatieType.WithoutUpdate());
				if (commvorm.GelieerdePersoon.Persoon.AdNummer != null || commvorm.GelieerdePersoon.Persoon.AdInAanvraag)
				{
					if (isNieuw)
					{
						_communicatieSync.Toevoegen(commvorm);
					}
					else
					{
						// bij update moet *alle* communicatie opnieuw naar Kipadmin, omdat we niet weten welke 
						// precies de te vervangen communicatievorm is.

						_personenSync.CommunicatieUpdaten(commvorm.GelieerdePersoon);
					}
				}
#if KIPDORP
				tx.Complete();
			}
#endif
			return resultaat;
		}

		/// <summary>
		/// Een communicatietype ophalen op basis van de opgegeven ID
		/// </summary>
		/// <param name="commTypeID">De ID van het communicatietype dat we nodig hebben</param>
		/// <returns>Het communicatietype met de opgegeven ID</returns>
		public CommunicatieType CommunicatieTypeOphalen(int commTypeID)
		{
			return _typedao.Ophalen(commTypeID);
		}

		/// <summary>
		/// Een collectie ophalen van alle gekende communicatietypes
		/// </summary>
		/// <returns>Een collectie van communicatietypes</returns>
		public IEnumerable<CommunicatieType> CommunicatieTypesOphalen()
		{
			return _typedao.AllesOphalen();
		}

		/// <summary>
		/// De communicatievorm met de opgegeven ID ophalen
		/// </summary>
		/// <param name="commvormID">De ID van de communicatievorm die je nodig hebt</param>
		/// <returns>De communicatievorm met de opgegeven ID</returns>
		public CommunicatieVorm CommunicatieVormOphalen(int commvormID)
		{
			if (_autorisatieMgr.IsGavCommVorm(commvormID))
			{
				return _dao.Ophalen(commvormID, e => e.CommunicatieType);
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Verwijdert een communicatievorm, en persisteert.
		/// </summary>
		/// <param name="comm">Te verwijderen communicatievorm</param>
		public void CommunicatieVormVerwijderen(CommunicatieVorm comm)
		{
			if (!_autorisatieMgr.IsGavGelieerdePersoon(comm.GelieerdePersoon.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			if (!_autorisatieMgr.IsGavCommVorm(comm.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			comm.TeVerwijderen = true;
#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
				// Indien ad-nummer: syncen
				if (comm.GelieerdePersoon.Persoon.AdNummer != null || comm.GelieerdePersoon.Persoon.AdInAanvraag)
				{
					_communicatieSync.Verwijderen(comm);
				}
				_dao.Bewaren(comm);
#if KIPDORP
				tx.Complete();
			}
#endif
		}

		/// <summary>
		/// Vervangt eventueel de oude gegevens van de communicatievorm door de nieuwe,
		/// gaat na of er al een andere communicatievorm van dat type is, om te garanderen
		/// dat er een voorkeurscommunicatievorm van dat type is
		/// </summary>
		/// <param name="gp">De gelieerde persoon voor wie de communicatievorm toegevoegd of aangepast wordt</param>
		/// <param name="nieuwecv">De nieuwe gegevens voor de communicatievorm</param>
		public void AanpassingenDoorvoeren(GelieerdePersoon gp, CommunicatieVorm nieuwecv)
		{
			if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			CommunicatieVorm oudecv = gp.Communicatie.Where(e => e.ID == nieuwecv.ID).FirstOrDefault();

			var cvValid = new CommunicatieVormValidator();

			if (!cvValid.Valideer(nieuwecv))
			{
				throw new ValidatieException(string.Format(Properties.Resources.CommunicatieVormValidatieFeedback, nieuwecv.Nummer, nieuwecv.CommunicatieType.Omschrijving));
			}

			if (oudecv != null)
			{
				gp.Communicatie.Remove(oudecv);
			}

			gp.Communicatie.Add(nieuwecv);
			nieuwecv.GelieerdePersoon = gp;

			bool seen = false;
			foreach (CommunicatieVorm cv in gp.Communicatie)
			{
				if (cv.CommunicatieType.ID == nieuwecv.CommunicatieType.ID)
				{
					if (cv.Voorkeur && seen)
					{
						cv.Voorkeur = false;
					}
					if (!seen && cv.Voorkeur)
					{
						seen = true;
					}
				}
			}
			if (!seen)
			{
				nieuwecv.Voorkeur = true;
			}
		}
	}
}
