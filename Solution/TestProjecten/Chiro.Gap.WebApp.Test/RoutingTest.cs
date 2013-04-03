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
﻿
using System.Web;
using System.Web.Routing;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Chiro.Gap.WebApp.Test
{
	/// <summary>
	/// Bevat tests of routering goed gebeurt. Heeft dus niets te maken met de interne werking 
	/// van controllers en hun actions.
	/// </summary>
	[TestClass]
	public class RoutingTest
	{
		[TestMethod]
		public void Slash_Gaat_Naar_Gav_Index()
		{
			TestRoute("~/", new
			{
				controller = "Gav",
				action = "Index"
			});
		}

		[TestMethod]
		public void Enkel_GroepID_Gaat_Naar_Default()
		{
			TestRoute("~/15000",				// willekeurige groepID, hoeft niet te bestaan
				new								// dit zijn de defaults zoals ze ingesteld zijn in global.asax
				{
					controller = "Handleiding",
					action = "Index",
					groepID = 15000
				});
		}

		[TestMethod]
		public void Controller_Zonder_GroepID_Geeft_Error()
		{
			TestRoute("~/Leden",
					new
					{
						controller = "Error",
						action = "NietGevonden"
					});
		}

		[TestMethod]
		public void HandleidingController_Zonder_GroepID_Geeft_Handleiding()
		{
			TestRoute("~/Handleiding",
				new
				{
					controller = "Handleiding",
					action = "ViewTonen",
					helpBestand = "Index"
				});
		}

		[TestMethod]
		public void HandleidingController_Met_Helpbestand_Zonder_GroepID_Geeft_Handleiding()
		{
			TestRoute("~/Handleiding/Jaarovergang",
				new
				{
					controller = "Handleiding",
					action = "ViewTonen",
					helpBestand = "Jaarovergang"
				});

		}

		[TestMethod]
		public void HandleidingController_Met_Helpbestand_Geeft_Handleiding()
		{
			TestRoute("~/16000/Handleiding/Jaarovergang",
				new
				{
					controller = "Handleiding",
					action = "ViewTonen",
					helpBestand = "Jaarovergang"
				});

		}

		[TestMethod]
		public void Afdeling_Maken_In_Jaarovergang()
		{
			TestRoute("~/15000/JaarOvergang/AfdelingMaken",
					new
					{
						controller = "JaarOvergang",
						action = "AfdelingMaken",
						groepID = 15000
					});
		}

		[TestMethod]
		public void Afdeling_Bewerken_In_Jaarovergang()
		{
			// Ik vermoed dat deze test failt omdat de jaarovergang nog niet
			// geimplementeerd is?

			TestRoute("~/15000/JaarOvergang/AfdelingAanpassen/15999",
					new
					{
						controller = "JaarOvergang",
						action = "AfdelingAanpassen",
						groepID = 15000,
						id = 15999
					});
		}

		[TestMethod]
		public void Foutcontroller_Wordt_Juist_Opgeroepen()
		{
			TestRoute("~/Error/NietGevonden",
				new
					{
						controller = "Error",
						action = "NietGevonden"
					}
				);
		}

		private static void TestRoute(string url, object expectedValues)
		{
			// Arrange: Prepare the route collection and a mock request context
			var routes = new RouteCollection();
			MvcApplication.RegisterRoutes(routes);
			var mockHttpContext = new Mock<HttpContextBase>();
			var mockRequest = new Mock<HttpRequestBase>();
			mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
			mockRequest.Setup(x => x.AppRelativeCurrentExecutionFilePath).Returns(url);

			// Act: Get the mapped route
			RouteData routeData = routes.GetRouteData(mockHttpContext.Object);

			// Assert: Test the route values against expectations
			Assert.IsNotNull(routeData);
			var expectedDict = new RouteValueDictionary(expectedValues);
			foreach (var expectedVal in expectedDict)
			{
				if (expectedVal.Value == null)
				{ Assert.IsNull(routeData.Values[expectedVal.Key]); }
				else
				{ Assert.AreEqual(expectedVal.Value.ToString(), routeData.Values[expectedVal.Key].ToString()); }
			}
		}

	}
}
