/*
 * Copyright 2008-2016 the GAP developers. See the NOTICE file at the 
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
using Chiro.Cdf.Mailer;
using Chiro.Gap.Test;
using NUnit.Framework;

// Unit tests zonder veel structuur.  Achteraf op te kuisen.
// Makkelijkste is om de loginservice te runnen vanuit 1 Visual Studio instance, en deze tests vanuit
// een andere.

namespace Chiro.Ad.Test
{
    [TestFixture]
    public class MailTest: ChiroTest
    {
        [Test]
        public void MailNaarGmail()
        {
            IMailer mailer = Factory.Maak<IMailer>();

            mailer.Verzenden("johan.vervloet@gmail.com", "unit test " + DateTime.Now, "unit test");

            Assert.IsTrue(true);
        }

        [Test]
        public void MailNaarChiro()
        {
            IMailer mailer = Factory.Maak<IMailer>();

            mailer.Verzenden("johan.vervloet@chiro.be", "unit test " + DateTime.Now, "unit test");

            Assert.IsTrue(true);
        }

    }
}
