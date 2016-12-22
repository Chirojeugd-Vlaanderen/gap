/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkte authenticatie Copyright 2014 Johan Vervloet
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
using System.ServiceModel;

namespace Chiro.Ad.ServiceContracts
{
    /// <summary>
    /// Interface voor de service die hier aangeboden wordt
    /// </summary>
    [ServiceContract]
    public interface IAdService
    {
        /// <summary>
        /// Vraagt aan Active Directory om een account aan te maken met rechten om GAP
        /// te gebruiken. Als de persoon in kwestie al een account heeft, worden de 
        /// rechten uitgebreid.
        /// </summary>
        /// <param name="adNr">AD-nummer van de persoon die een login moet krijgen</param>
        /// <param name="voornaam">Voornaam van de persoon die een login moet krijgen</param>
        /// <param name="familienaam">Naam van de persoon die een login moet krijgen</param>
        /// <param name="mailadres">Mailadres van de persoon die een login moet krijgen</param>
        /// <returns>Als alles goed gaat: de loginnaam. Het kan zijn dat die al bestond - in dat geval 
        /// worden de GAP-rechten toegekend. Ofwel werd hij aangemaakt. In beide gevallen krijgt
        /// de persoon in kwestie een mailtje zodat hij of zij op de hoogte is.</returns>
        /// <example>
        ///    Dit is een voorbeeld van hoe deze service aangeroepen kan worden: 
        ///  <code>
        ///    string login;
        ///    ServiceClient client = null;
        ///    try
        ///    {
        ///        client = new ServiceClient();
        ///        login = client.GapLoginAanvragen(Int32.Parse(AdNrTextBox.Text), VoornaamTextBox.Text, NaamTextBox.Text, MailadresTextBox.Text);
        ///    }
        ///    catch
        ///    {
        ///        // Fout afhandelen
        ///    }
        ///    finally
        ///    {
        ///        if (client != null)
        ///        {
        ///            // Altijd de client afsluiten!
        ///            client.Close();
        ///        }
        ///    }
        ///  </code>
        /// </example>
        [OperationContract]
        [FaultContract(typeof(FaultException<ArgumentException>))]
        [FaultContract(typeof(FaultException<FormatException>))]
        [FaultContract(typeof(FaultException<InvalidOperationException>))]
        string GapLoginAanvragen(int adNr, string voornaam, string familienaam, string mailadres);

        /// <summary>
        /// Haalt AD-nummer op van gebruiker met gegeven <paramref name="userName"/>.
        /// </summary>
        /// <param name="userName">Gebruikersnaam.</param>
        /// <returns>AD-nummer van gebruiker met gegeven gebruikersnaam.</returns>
        [OperationContract]
        int? AdNummerOpHalen(string userName);

        /// <summary>
        /// Haalt de gebruikersnaam op van de user met gegeven <paramref name="adNummer"/>.
        /// </summary>
        /// <param name="adNummer">AD-nummer van user waarvan gebruikersnaam gevraagd is.</param>
        /// <returns>Gebruikersnaam van de user met gegeven <paramref name="adNummer"/>.</returns>
        [OperationContract]
        string GebruikersNaamOphalen(int adNummer);
    }
}