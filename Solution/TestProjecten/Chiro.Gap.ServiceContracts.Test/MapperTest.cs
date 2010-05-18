using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.ServiceContracts.DataContracts;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Chiro.Cdf.Ioc;		// is nodig voor dummydata in gap.dummies.  dummydata zou beter ergens anders zitten.
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
			var testData = new DummyData();
			var LidInfoLijst = Mapper.Map<IEnumerable<Lid>, IList<LidInfo>>(testData.HuidigGwj.Lid);

			Assert.IsTrue(LidInfoLijst.Count > 0);
		}

		/// <summary>
		/// Test voor de mapping van een groep naar GroepInfo
		/// </summary>
		[TestMethod]
		public void MapGroepGroepInfo()
		{
			var testData = new DummyData();
			GroepInfo gi = Mapper.Map<Groep, GroepInfo>(testData.DummyGroep);

			Assert.IsTrue(gi.StamNummer != string.Empty);
		}

		/// <summary>
		/// Controleert mapping Functie -> FunctieInfo
		/// </summary>
		[TestMethod]
		public void MapFunctieInfo()
		{
			var testData = new DummyData();
			FunctieInfo fi = Mapper.Map<Functie, FunctieInfo>(testData.UniekeFunctie);
			Assert.AreEqual(fi.Code, testData.UniekeFunctie.Code);
		}

		/// <summary>
		/// Controleert of functies goed mee gemapt worden met lidinfo.
		/// </summary>
		[TestMethod]
		public void MapLidInfoFuncties()
		{
			var testData = new DummyData();
			LidInfo li = Mapper.Map<Lid, LidInfo>(testData.LidJos);
			Assert.IsTrue(li.Functies.Count() > 0);
		}
	}
}
