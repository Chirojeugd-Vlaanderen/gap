// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
    }
}