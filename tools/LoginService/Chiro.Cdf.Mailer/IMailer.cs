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
    /// Interface voor de mailservice
    /// </summary>
    public interface IMailer
    {
        /// <summary>
        /// Verstuurt een mail naar <paramref name="ontvanger"/>, met gegeven <paramref name="onderwerp"/> en
        /// <paramref name="ontvanger"/>
        /// </summary>
        /// <param name="ontvanger">E-mailadres van de geadresseerde</param>
        /// <param name="onderwerp">Onderwerp van de mail</param>
        /// <param name="body">Inhoud van de mail</param>
        void Verzenden(string ontvanger, string onderwerp, string body);

        /// <summary>
        /// Verstuurt een mail naar <paramref name="ontvanger"/>, met gegeven <paramref name="onderwerp"/> en
        /// <paramref name="ontvanger"/>
        /// </summary>
        /// <param name="afzender">E-mailadres van de afzender.</param>
        /// <param name="ontvanger">E-mailadres van de geadresseerde</param>
        /// <param name="onderwerp">Onderwerp van de mail</param>
        /// <param name="body">Inhoud van de mail</param>
        /// <returns><c>True</c> als het bericht verstuurd is, anders <c>false</c>.</returns>
        void Verzenden(string afzender, string ontvanger, string onderwerp, string body);
    }
}
