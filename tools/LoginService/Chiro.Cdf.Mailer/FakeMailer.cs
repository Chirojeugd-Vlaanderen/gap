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
namespace Chiro.Cdf.Mailer
{
    /// <summary>
    /// Class die de mailservice mockt
    /// </summary>
    public class FakeMailer : IMailer
    {
        /// <summary>
        /// Doet alsof het een mailtje verstuurt
        /// </summary>
        /// <param name="ontvanger">
        /// Het adres van degene naar wie de mail verstuurd moet worden
        /// </param>
        /// <param name="onderwerp">
        /// Het onderwerp van het mailtje
        /// </param>
        /// <param name="body">
        /// De inhoud van het mailtje
        /// </param>
        /// <returns>
        /// <c>True</c> als het mailtje verstuurd is (wat in deze mockversie
        /// natuurlijk altijd het geval is)
        /// </returns>
        public bool Verzenden(string ontvanger, string onderwerp, string body)
        {
            return true;
        }
    }
}
