using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

using Chiro.Cdf.ServiceHelper;
using Chiro.Cdf.Ioc;
using Chiro.Gap.ServiceContracts.DataContracts;

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

			Factory.InstantieRegistreren<IEnumerable<LandInfo>>(new List<LandInfo>());
		}


		/// <summary>
		/// Deze dummyservicehelper zal bijhouden hoe vaak de service
		/// aangeroepen wordt.  (Ik kreeg dit niet gearrangeerd met Moq.)
		/// </summary>
		private class DummyServiceHelper : IServiceHelper
		{
			private int _aantalCalls = 0;

			internal int AantalCalls
			{
				get { return _aantalCalls; }
			}

			public T CallService<I, T>(Func<I, T> operation) where I: class
			{
				++_aantalCalls;

				if (typeof(T) == typeof(bool))
				{
					return default(T);
				}
				else
				{


					// Ik wil vermijden dat het resultaat null is.  Daarom
					// misbruik ik de IoC container om een resultaat te genereren.

					return Factory.Maak<T>();
				}
			}

			public void CallService<I>(Action<I> operation) where I : class
			{
				++_aantalCalls;
			}
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
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


		/// <summary>
		///A test for IsLive
		///</summary>
		[TestMethod]
		public void IsLiveTest()
		{
			// Arrange

			var serviceHelper = new DummyServiceHelper();
			var veelGebruikt = new VeelGebruikt(serviceHelper);

			// Act

			var res1 = veelGebruikt.IsLive();
			var res2 = veelGebruikt.IsLive();

			// Assert

			Assert.AreEqual(serviceHelper.AantalCalls, 1);
		}

		///<summary>
		///Kijkt na of de lijst met landen goed wordt gecachet
		///</summary>
		[TestMethod]
		public void LandenOphalenTest()
		{
			// Arrange

			var serviceHelper = new DummyServiceHelper();
			var veelGebruikt = new VeelGebruikt(serviceHelper);

			// Act

			var res1 = veelGebruikt.LandenOphalen();
			var res2 = veelGebruikt.LandenOphalen();

			// Assert

			Assert.AreEqual(serviceHelper.AantalCalls, 1);
		}
	}
}
