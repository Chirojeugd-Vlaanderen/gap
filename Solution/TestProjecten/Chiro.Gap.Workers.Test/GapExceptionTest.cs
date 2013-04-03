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
using System.Linq;
using Chiro.Gap.Poco.Model.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    /// Dit is een testclass voor Unit Tests van GapExceptionTest,
    /// to contain all GapExceptionTest Unit Tests
    /// </summary>
	[TestClass]
	public class GapExceptionTest
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
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
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


		/// <summary>
		///Test de (de)serializatie van de GapException
		/// </summary>
		[TestMethod]
		public void GapExceptionConstructorTest()
		{
			// Arrange

			var target = new FoutNummerException() {FoutNummer = FoutNummer.GeenGav, Items = new string[] {"een", "twee"}};


			// Act

			using (Stream s = new MemoryStream())
			{
				var formatter = new BinaryFormatter();

				formatter.Serialize(s, target);
				s.Position = 0;

				target = (FoutNummerException)formatter.Deserialize(s);
			}

			Assert.AreEqual(target.FoutNummer, FoutNummer.GeenGav);
			Assert.AreEqual(String.Compare(target.Items.First(), "een"), 0);
			Assert.AreEqual(String.Compare(target.Items.Last(), "twee"), 0);

		}
	}
}

