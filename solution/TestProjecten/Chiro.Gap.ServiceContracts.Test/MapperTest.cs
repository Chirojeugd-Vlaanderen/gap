﻿using AutoMapper;

using System.Collections.Generic;
using System.Linq;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.DataContracts;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Chiro.Cdf.Ioc;		// is nodig voor dummydata in gap.dummies.  dummydata zou beter ergens anders zitten.
using Chiro.Gap.ServiceContracts.Mappers;

namespace Chiro.Gap.ServiceContracts.Test
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class MapperTest
	{
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
		    var huidigGwj = new GroepsWerkJaar {Lid = new List<Lid> {new Kind(), new Kind()}};
			var lidInfoLijst = Mapper.Map<IEnumerable<Lid>, IList<LidInfo>>(huidigGwj.Lid);

			Assert.IsTrue(lidInfoLijst.Count > 0);
		}

		/// <summary>
		/// Test voor de mapping van een groep naar GroepInfo
		/// </summary>
		[TestMethod]
		public void MapGroepGroepInfo()
		{
		    var groep = new ChiroGroep {Code = "tst/0001"};
			GroepInfo gi = Mapper.Map<Groep, GroepInfo>(groep);

			Assert.IsTrue(gi.StamNummer != string.Empty);
		}

		/// <summary>
		/// Controleert mapping Functie -> FunctieDetail
		/// </summary>
		[TestMethod]
		public void MapFunctieInfo()
		{
		    var functie = new Functie {Code = "BLA"};

			FunctieDetail fi = Mapper.Map<Functie, FunctieDetail>(functie);
			Assert.AreEqual(fi.Code, functie.Code);
		}

		/// <summary>
		/// Controleert of functies goed mee gemapt worden met lidinfo.
		/// </summary>
		[TestMethod]
		public void MapLidInfoFuncties()
		{
		    var leiderJos = new Leiding {Functie = new List<Functie> {new Functie()}};
			LidInfo li = Mapper.Map<Lid, LidInfo>(leiderJos);
			Assert.IsTrue(li.Functies.Any());
		}
	}
}