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

using System.Collections.Generic;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.Test;
using NUnit.Framework;
using Moq;

namespace Chiro.Gap.WebApp.Test
{
	[TestFixture]
	public class VeelGebruiktTest: ChiroTest
	{
	    ///<summary>
		///Kijkt na of de lijst met landen goed wordt gecachet
		/// </summary>
		[Test]
		public void LandenOphalenTest()
		{
			// Arrange

		    var groepenServiceMock = new Mock<IGroepenService>();
		    groepenServiceMock.Setup(mock => mock.LandenOphalen()).Returns(new List<LandInfo>());

            var channelProviderMock = new Mock<IChannelProvider>();
            channelProviderMock.Setup(mock => mock.GetChannel<IGroepenService>()).Returns(groepenServiceMock.Object);

            Factory.InstantieRegistreren(channelProviderMock.Object);

            var veelGebruikt = Factory.Maak<VeelGebruikt>();

			// Act

			var res1 = veelGebruikt.LandenOphalen();
			var res2 = veelGebruikt.LandenOphalen();

			// Assert

			groepenServiceMock.Verify(mock => mock.LandenOphalen(), Times.Exactly(1));
		}
	}
}
