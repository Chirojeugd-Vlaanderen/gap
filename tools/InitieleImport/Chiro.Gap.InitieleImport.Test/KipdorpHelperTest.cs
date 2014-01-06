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
﻿using Chiro.Pdox.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Chiro.Gap.InitieleImport.Test
{
    
    
    /// <summary>
    ///This is a test class for KipdorpHelperTest and is intended
    ///to contain all KipdorpHelperTest Unit Tests
    ///</summary>
	[TestClass()]
	public class KipdorpHelperTest
	{


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

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
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
		//[TestInitialize()]
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
		///A test for StamNrNaarBestand
		///</summary>
		[TestMethod()]
		public void StamNrNaarBestandTest()
		{
			KipdorpHelper target = new KipdorpHelper();
			string stamnr = "mg /0113";
			string expected = "mg_0113";
			string actual;

			actual = target.StamNrNaarBestand(stamnr);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for RecentsteAansluitingsBestand
		///</summary>
		[TestMethod()]
		public void RecentsteAansluitingsBestandTest()
		{
			KipdorpHelper target = new KipdorpHelper();
			string stamnr = "mg /0113";
			string expected = "//kip-fls05/ADMINISTRATIE-Data/Administratief/Aansluitingen/Email/2009/mg_0113/mg_0113.a3";
			string actual;
			actual = target.RecentsteAansluitingsBestand(stamnr);
			Assert.AreEqual(expected, actual, true);
		}
	}
}
