using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoMapper;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Dummies;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts.Mappers;

namespace Chiro.Gap.ServiceContracts.Test
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class MapperTest
	{
		public MapperTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[ClassInitialize]
		public static void Init(TestContext ctx)
		{
			Factory.ContainerInit();

			MappingHelper.MappingsDefinieren();
		}

		/// <summary>
		/// Test die probeert de leden uit huidig groepswerkjaar te mappen op een lijst van LidInfo.
		/// </summary>
		[TestMethod]
		public void MapLijstLeden()
		{
			var LidInfoLijst = Mapper.Map<IEnumerable<Lid>, IList<LidInfo>>(DummyData.HuidigGwj.Lid);

			Assert.IsTrue(LidInfoLijst.Count > 0);
		}
	}
}
