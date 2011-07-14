// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Resources;

/// <summary>
/// Class waarin de mogelijkheden van de service uitgewerkt zijn
/// </summary>
public class Service : IService
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
    ///     Dit is een voorbeeld van hoe deze service aangeroepen kan worden: 
    ///   <code>
    ///     string login;
    ///     ServiceClient client = null;
    ///     try
    ///     {
    ///         client = new ServiceClient();
    ///         login = client.GapLoginAanvragen(Int32.Parse(AdNrTextBox.Text), VoornaamTextBox.Text, NaamTextBox.Text, MailadresTextBox.Text);
    ///     }
    ///     catch
    ///     {
    ///         // Fout afhandelen
    ///     }
    ///     finally
    ///     {
    ///         if (client != null)
    ///         {
    ///             // Altijd de client afsluiten!
    ///             client.Close();
    ///         }
    ///     }
    ///   </code>
    /// </example>
    public string GapLoginAanvragen(int adNr, string voornaam, string familienaam, string mailadres)
    {
        // Validatie
        if (!Regex.IsMatch(mailadres, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
        {
            throw new FaultException<ArgumentException>(new ArgumentException(), "Ongeldig mailadres");
        }

        // Verwerking
        GapLogin gebruiker;
        var msr = new MailServiceReference.MailServiceSoapClient();

        try
        {
            gebruiker = new GapLogin(adNr, voornaam, familienaam);

            // We controleren of het mailadres hetzelfde is als hier opgegeven.
            if (gebruiker.Mailadres != string.Empty && gebruiker.Mailadres != mailadres)
            {
                // Het is een ander, dus sturen we ook nog een mailtje naar het hier opgegeven adres,
                // voor de zekerheid, met instructies om het wachtwoord van de login te kunnen aanpassen.
                string boodschap = String.Format(Mailopbouw.VerschillendWachtwoordMail, voornaam);
                msr.VerstuurMail("Helpdesk@chiro.be", mailadres, "Je Chirologin", boodschap, "IntranetService");
            }
            else if (gebruiker.Mailadres == string.Empty)
            {
                // Nog geen mailadres ingevuld, dus is het een nieuwe login
                gebruiker.Mailadres = mailadres;
                gebruiker.Opslaan();
                gebruiker.ActiverenEnMailen(false);
            }
            return gebruiker.Login;
        }
        catch (FormatException ex)
        {
            throw new FaultException<FormatException>(ex, ex.Message);
        }
        catch (ArgumentException ex)
        {
            throw new FaultException<ArgumentException>(ex, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            throw new FaultException<InvalidOperationException>(ex, ex.Message);
        }
    }
}