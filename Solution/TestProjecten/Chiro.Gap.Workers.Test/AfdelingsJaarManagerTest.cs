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

using Chiro.Cdf.Ioc.Factory;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Gap.Workers.Test
{


	/// <summary>
	/// Dit is een testclass voor AfdelingsJaarManagerTest,
	///to contain all AfdelingsJaarManagerTest Unit Tests
	/// </summary>
	[TestClass]
	public class AfdelingsJaarManagerTest
	{


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

		#region Additional test attributes

		//You can use the following additional attributes as you write your tests:

		//Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();
		}


		//Use ClassCleanup to run code after all tests in a class have run
		[ClassCleanup()]
		public static void MyClassCleanup()
		{
			
		}

		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		///<summary>
		///Probeer een afdeling te maken in een groepswerkjaar van een andere groep
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(FoutNummerException))]
		public void AfdelingGroepsWerkJaarMismatch()
		{
			var groep1 = new ChiroGroep { ID = 1 };
			var groep2 = new ChiroGroep { ID = 2 };

			var gwj = new GroepsWerkJaar { ID = 1, Groep = groep1 };
			var a = new Afdeling {ChiroGroep = groep2};
			var oa = new OfficieleAfdeling();

			var target = Factory.Maak<IAfdelingsJaarManager>();

			target.Aanmaken(a, oa, gwj, 2001, 2002, GeslachtsType.Gemengd);

			// Als we hier geraken, is de verwachte exception niet opgeworpen

			Assert.IsTrue(false);
		}
	}
}
