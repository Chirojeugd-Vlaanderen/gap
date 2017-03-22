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

using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model.Exceptions;
using NUnit.Framework;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    /// Dit is een testclass voor Unit Tests van GapExceptionTest,
    /// to contain all GapExceptionTest Unit Tests
    /// </summary>
	[TestFixture]
	public class GapExceptionTest
	{
		/// <summary>
		///Test de (de)serializatie van de GapException
		/// </summary>
		[Test]
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

