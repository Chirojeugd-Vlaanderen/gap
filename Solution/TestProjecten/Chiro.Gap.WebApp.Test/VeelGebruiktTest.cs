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
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using Chiro.Cdf.Ioc;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

using Moq;

namespace Chiro.Gap.WebApp.Test
{

	[TestClass]
	public class VeelGebruiktTest
	{
		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();

			// zorgt ervoor dat de dummy service helper een lege lijst
			// landinfo oplevert als er IEnumerable<LandInfo> gevraagd wordt.
			// (een beetje een hack)
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		///<summary>
		///Kijkt na of de lijst met landen goed wordt gecachet
		/// </summary>
		[TestMethod]
		public void LandenOphalenTest()
		{
			// Arrange

		    var groepenServiceMock = new Mock<IGroepenService>();
		    groepenServiceMock.Setup(mock => mock.LandenOphalen()).Returns(new List<LandInfo>());
            Factory.InstantieRegistreren(groepenServiceMock.Object);

			var veelGebruikt = new VeelGebruikt();

			// Act

			var res1 = veelGebruikt.LandenOphalen();
			var res2 = veelGebruikt.LandenOphalen();

			// Assert

			groepenServiceMock.Verify(mock => mock.LandenOphalen(), Times.Exactly(1));
		}
	}
}
