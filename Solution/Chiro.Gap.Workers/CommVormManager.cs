// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Validatie;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. communicatievormen bevat (telefoonnummer, mailadres, enz.)
	/// </summary>
	public class CommVormManager
	{
		private IDao<CommunicatieType> _typedao;
		private IDao<CommunicatieVorm> _dao;
		private IAutorisatieManager _autorisatieMgr;
		private IGelieerdePersonenDao _geldao;

		/// <summary>
		/// Deze constructor laat toe om een alternatieve repository voor
		/// de communicatievormen te gebruiken.  Nuttig voor mocking en testing.
		/// </summary>
		/// <param name="typedao">Repository voor communicatietypes</param>
		/// <param name="commdao">Repository voor communicatievormen</param>
		/// <param name="autorisatieMgr">Worker die autorisatie regelt</param>
		/// <param name="geldao">Repository voor gelieerde personen</param>
		public CommVormManager(IDao<CommunicatieType> typedao, IDao<CommunicatieVorm> commdao, IAutorisatieManager autorisatieMgr,
								IGelieerdePersonenDao geldao)
		{
			_typedao = typedao;
			_dao = commdao;
			_autorisatieMgr = autorisatieMgr;
			_geldao = geldao;
		}

		/// <summary>
		/// Haalt communicatievorm op, op basis van commvormID
		/// </summary>
		/// <param name="commvormID">ID op te halen communicatievorm</param>
		/// <returns>Gevraagde communicatievorm</returns>
		public CommunicatieVorm Ophalen(int commvormID)
		{
			if (_autorisatieMgr.IsGavCommVorm(commvormID))
			{
				return _dao.Ophalen(commvormID, foo => foo.CommunicatieType);
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavCommVorm);
			}
		}

		/// <summary>
		/// Persisteert communicatievorm in de database
		/// </summary>
		/// <param name="commvorm">Te persisteren communicatievorm</param>
		/// <returns>De bewaarde communicatievorm</returns>
		public CommunicatieVorm Bewaren(CommunicatieVorm commvorm)
		{
			if (_autorisatieMgr.IsGavCommVorm(commvorm.ID))
			{
				return _dao.Bewaren(commvorm);
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavCommVorm);
			}
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
				throw new GeenGavException(Resources.GeenGavCommVorm);
			}
		}

		/// <summary>
		/// Een nieuwe communicatievorm opslaan voor de gelieerde persoon met de opgegeven ID
		/// </summary>
		/// <param name="comm">De communicatievorm die je wilt opslaan</param>
		/// <param name="gelieerdePersoonID">De ID van de gelieerde persoon over wie het gaat</param>
		/// <param name="typeID">De ID van het type van de communicatievorm</param>
		public void CommunicatieVormToevoegen(CommunicatieVorm comm, int gelieerdePersoonID, int typeID)
		{
			if (!_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				throw new GeenGavException(Resources.GeenGavGroep);
			}
			GelieerdePersoon origineel = _geldao.Ophalen(gelieerdePersoonID, e => e.Persoon, e => e.Communicatie.First().CommunicatieType);
			CommunicatieType type = _typedao.Ophalen(typeID);
			CommunicatieVormValidator cvValid = new CommunicatieVormValidator();

			if (cvValid.Valideer(comm))
			{
				origineel.Communicatie.Add(comm);
				comm.CommunicatieType = type;
				_dao.Bewaren(comm, l => l.CommunicatieType.WithoutUpdate(), l => l.GelieerdePersoon.WithoutUpdate());
			}
			else
			{
				throw new ValidatieException(string.Format(Resources.CommunicatieVormValidatieFeedback, comm.Nummer, comm.CommunicatieType.Omschrijving));
			}
		}

		// FIXME: de parameter 'gelieerdePersoon' is overbodig; zie ticket #145.
		/// <summary>
		/// Verwijdert een communicatievorm, en persisteert.
		/// </summary>
		/// <param name="comm">Te verwijderen communicatievorm</param>
		/// <param name="origineel">Gekoppelde persoon.  Deze parameter moet verdwijnen; die informatie
		/// moet komen uit comm.GelieerdePersoon.</param>
		public void CommunicatieVormVerwijderen(CommunicatieVorm comm, GelieerdePersoon origineel)
		{
			if (!_autorisatieMgr.IsGavGelieerdePersoon(origineel.ID))
			{
				// Aangezien er niet getest wordt of de communicatievorm wel hoort bij de 
				// gegeven persoon, is deze test belachelijk.

				throw new GeenGavException(Resources.GeenGavGelieerdePersoon);
			}
			if (!_autorisatieMgr.IsGavCommVorm(comm.ID))
			{
				throw new GeenGavException(Resources.GeenGavCommVorm);
			}
			comm.TeVerwijderen = true;
			_dao.Bewaren(comm);
		}
	}
}
