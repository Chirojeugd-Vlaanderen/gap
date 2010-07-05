using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Cdf.Ioc.Test
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class LedenDaoCollectieTest
	{
		[ClassInitialize]
		static public void InitialiseerTests(TestContext tc)
		{
			Factory.ContainerInit();
		}

		[ClassCleanup]
		static public void AfsluitenTests()
		{
			Factory.Dispose();
		}

		/// <summary>
		/// Kijkt na of een nieuwe LedenDaoCollectie een LedenDao krijgt.
		/// </summary>
		[TestMethod]
		public void InstantieerCollectie()
		{
			var daos = Factory.Maak<LedenDaoCollectie>();
            Assert.IsTrue(daos.LedenDao != null);
		}
	}
}