
using System.Web;
using System.Web.Routing;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Chiro.Gap.WebApp.Test
{
	/// <summary>
	/// Bevat tests of routering goed gebeurt
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
			TestRoute("~/15000",				// zeker niet toegekende groepID
				new								// en dit zijn de defaults zoals ze ingesteld zijn in global.asax
				{
					controller = "GavTaken",
					action = "Index"
				});
		}

		[TestMethod]
		public void Controller_Zonder_GroepID_Geeft_Error()
		{
			TestRoute("~/Leden",
					new
					{
						controller = "Error",
						action = "NotFound"
					});
		}

		[TestMethod]
		public void Afdeling_Maken_In_Jaarovergang()
		{
			TestRoute("~/15000/JaarOvergang/AfdelingMaken",
					new
					{
						controller = "JaarOvergang",
						action = "AfdelingMaken"
					});
		}

		[TestMethod]
		public void Afdeling_Bewerken_In_Jaarovergang()
		{
			TestRoute("~/15000/JaarOvergang/AfdelingAanpassen/15000",
					new
					{
						controller = "JaarOvergang",
						action = "AfdelingAanpassen",
						afdelingID = 15000
					});
		}

		[TestMethod]
		public void Foutpagina_Wordt_Weergegeven()
		{
			TestRoute("~/Error/NotFound",
				new
					{
						controller = "Error",
						action = "NotFound"
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
