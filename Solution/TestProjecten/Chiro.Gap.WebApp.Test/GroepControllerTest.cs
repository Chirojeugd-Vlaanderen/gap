/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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

using System.Web.Mvc;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.Test;
using Chiro.Gap.WebApp.Controllers;
using NUnit.Framework;
using Moq;

namespace Chiro.Gap.WebApp.Test
{
    /// <summary>
    /// Dit is een testclass voor Unit Tests van GroepControllerTest,
    /// to contain all GroepControllerTest Unit Tests
    /// </summary>
	[TestFixture]
	public class GroepControllerTest: ChiroTest
	{
		/// <summary>
		/// Controleert of de action Groep/CategorieVerwijderen de categorieenservice aanroept,
		/// en of achteraf terug de view met categorieen getoond wordt.
		/// </summary>
		[Test]
		public void CategorieVerwijderenTest()
		{
			const int DUMMYCATID = 9;
			const int DUMMYGROEPID = 1;

			var groepenServiceMock = new Mock<IGroepenService>();
			groepenServiceMock.Setup(mock=>mock.CategorieVerwijderen(It.IsAny<int>(), It.IsAny<bool>()));
            var channelProviderMock = new Mock<IChannelProvider>();
            channelProviderMock.Setup(mock => mock.GetChannel<IGroepenService>()).Returns(groepenServiceMock.Object);

            var veelGebruiktMock = new Mock<IVeelGebruikt>();

            Factory.InstantieRegistreren(channelProviderMock.Object);
            Factory.InstantieRegistreren(veelGebruiktMock.Object);

            var target = Factory.Maak<CategorieenController>();

			var actual = target.CategorieVerwijderen(DUMMYGROEPID, DUMMYCATID) as RedirectToRouteResult;

			// Controleer of verwachte service call wel is gebeurd
			groepenServiceMock.VerifyAll();

			// Verwacht de view met de groepsinstellingen.
			Assert.IsNotNull(actual);
			Assert.AreEqual("Categorieen", actual.RouteValues["action"]);
		}

	}
}
