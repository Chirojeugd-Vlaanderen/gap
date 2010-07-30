// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Collectie van data access objecten die vaak nodig zijn voor ledenbeheer
	/// </summary>
	public class LedenDaoCollectie
	{
		/// <summary>
		/// Data Access Object voor leden (= ingeschreven kinderen/leiding)
		/// </summary>
		public ILedenDao LedenDao { get; set; }

		/// <summary>
		/// Data Access Object voor kinderen
		/// </summary>
		public IKindDao KindDao { get; set; }

		/// <summary>
		/// Data Access Object voor leiding
		/// </summary>
		public ILeidingDao LeidingDao { get; set; }

		/// <summary>
		/// Data Access Object voor groepen
		/// </summary>
		public IGroepenDao GroepenDao { get; set; }

		/// <summary>
		/// Data Access Object voor gelieerde personen
		/// </summary>
		public IGelieerdePersonenDao GelieerdePersoonDao { get; set; }

		/// <summary>
		/// Data Access Object voor afdelingsjaren
		/// </summary>
		public IAfdelingsJarenDao AfdelingsJaarDao { get; set; }

		/// <summary>
		/// Data Access Object voor groepswerkjaren
		/// </summary>
		public IGroepsWerkJaarDao GroepsWerkJaarDao { get; set; }

		/// <summary>
		/// Instantieert een LedenDaoCollectie-object
		/// </summary>
		/// <param name="ledenDao">Een gegevenstoegangsobject voor leden</param>
		/// <param name="kindDao">Een gegevenstoegangsobject voor kinderen</param>
		/// <param name="leidingDao">Een gegevenstoegangsobject voor leiding</param>
		/// <param name="groepenDao">Een gegevenstoegangsobject voor groepen</param>
		/// <param name="gpDao">Een gegevenstoegangsobject voor gelieerde personen</param>
		/// <param name="ajDao">Een gegevenstoegangsobject voor afdelingsjaren</param>
		/// <param name="groepsWjDao">Een gegevenstoegangsobject voor groepswerkjaren</param>
		public LedenDaoCollectie(
			ILedenDao ledenDao,
			IKindDao kindDao,
			ILeidingDao leidingDao,
			IGroepenDao groepenDao,
			IGelieerdePersonenDao gpDao,
			IAfdelingsJarenDao ajDao,
			IGroepsWerkJaarDao groepsWjDao)
		{
			LedenDao = ledenDao;
			KindDao = kindDao;
			LeidingDao = leidingDao;
			GroepenDao = groepenDao;
			GelieerdePersoonDao = gpDao;
			AfdelingsJaarDao = ajDao;
			GroepsWerkJaarDao = groepsWjDao;
		}
	}
}
