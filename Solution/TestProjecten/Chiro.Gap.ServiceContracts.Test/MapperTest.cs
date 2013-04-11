/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
