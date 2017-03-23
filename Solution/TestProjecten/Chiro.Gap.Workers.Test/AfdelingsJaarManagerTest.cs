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

using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.Test;
using Chiro.Gap.WorkerInterfaces;
using NUnit.Framework;

namespace Chiro.Gap.Workers.Test
{
	/// <summary>
	/// Dit is een testclass voor AfdelingsJaarManagerTest,
	///to contain all AfdelingsJaarManagerTest Unit Tests
	/// </summary>
	[TestFixture]
	public class AfdelingsJaarManagerTest: ChiroTest
	{
		///<summary>
		///Probeer een afdeling te maken in een groepswerkjaar van een andere groep
		/// </summary>
		[Test]
		public void AfdelingGroepsWerkJaarMismatch()
		{
			var groep1 = new ChiroGroep { ID = 1 };
			var groep2 = new ChiroGroep { ID = 2 };

			var gwj = new GroepsWerkJaar { ID = 1, Groep = groep1 };
			var a = new Afdeling {ChiroGroep = groep2};
			var oa = new OfficieleAfdeling();

			var target = Factory.Maak<IAfdelingsJaarManager>();

			var ex = Assert.Throws<FoutNummerException>(() => target.Aanmaken(a, oa, gwj, 2001, 2002, GeslachtsType.Gemengd));
		}
	}
}
