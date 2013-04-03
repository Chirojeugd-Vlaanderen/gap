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
﻿using System;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.Workers.Test
{
	[TestClass]
	public class UitstappenAanmaken
	{
		#region Additional test attributes
		[ClassInitialize]
		static public void InitialiseerTests(TestContext tc)
		{
			Factory.ContainerInit();
		}

		[ClassCleanup]
		static public void AfsluitenTests()
		{
			
		}
		#endregion

		/// <summary>
		/// TODO zou een non-dummy test moeten worden, zodat complexer gedrag getest kan worden
		/// </summary>
		[TestMethod]
		public void UitstappenInHuidigWerkjaar()
		{
            //var testData = new DummyData();

            //var um = Factory.Maak<UitstappenManager>();

            //var uitstap = new Uitstap();
            //uitstap.IsBivak = true;
            //uitstap.Naam = "Testbivak";
            //uitstap.DatumVan = DateTime.Today.AddDays(1);
            //uitstap.DatumTot = DateTime.Today.AddDays(1);

            //var dummygwj = new GroepsWerkJaar();
            //dummygwj.WerkJaar = DateTime.Today.Year;
            //uitstap.GroepsWerkJaar = dummygwj;

            //um.Bewaren(uitstap, UitstapExtras.Geen, false);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
		}
	}
}
