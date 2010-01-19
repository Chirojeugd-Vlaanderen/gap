using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Collectie van DAO-objecten die vaak nodig zijn voor ledenbeheer
	/// </summary>
	public class LedenDaoCollectie
	{
		public ILedenDao LedenDao { get; set; }
		public IKindDao KindDao { get; set; }
		public ILeidingDao LeidingDao { get; set; }
		public IGroepenDao GroepenDao { get; set; }
		public IGelieerdePersonenDao GelieerdePersoonDao { get; set; }
		public IAfdelingsJarenDao AfdelingsJaarDao { get; set; }
		public IGroepsWerkJaarDao GroepsWerkJaarDao { get; set; }

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
