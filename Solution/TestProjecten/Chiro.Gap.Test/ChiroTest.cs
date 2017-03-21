/*
 * Copyright 2017 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the
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
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.Test
{
    /// <summary>
    /// Base class voor tests, die een eigen container voorziet.
    /// </summary>
    public class ChiroTest: IDisposable
    {
        /// <summary>
        /// Shared container voor alle unit tests in a fixture.
        /// </summary>
        /// <remarks>
        /// Als je te hard foefelt met de container in test A, dan kan dat ongewenste gevolgen hebben voor test B,
        /// als die dezelfde container gebruikt. Het is eigenlijk beter om een aparte container te hebben voor iedere
        /// test; deze gedelelde container is eigenlijk maar een overgangsmaatregel voor #5663.
        /// </remarks>
        protected IDiContainer Factory { get; private set; }

        public ChiroTest()
        {
            Factory = new UnityDiContainer();
            Factory.InitVolgensConfigFile();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Factory?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Levert een nieuwe container op voor gebruik in een unit test.
        /// </summary>
        /// <returns>Container voor dependency injection.</returns>
        /// <remarks>
        /// Zeker als je plant je container te herconfigureren, is het te verkiezen om een container te laten maken
        /// door deze functie (ipv de gedeelde container te gebruiken). Vergeet hem niet te disposen na gebruik!
        /// </remarks>
        public IDiContainer ContainerCreate()
        {
            var container = new UnityDiContainer();
            container.InitVolgensConfigFile();
            return container;
        }
    }
}