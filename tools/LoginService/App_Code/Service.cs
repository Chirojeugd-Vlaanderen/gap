// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2009-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Net.Mail;
using Resources;

public class Service : IService
{
    public string LoginAanvragen(int adNr, string voornaam, string familienaam, MailAddress mailadres)
    {
        var gebruiker = new GapLogin(adNr, voornaam, familienaam);

        // We controleren of het mailadres hetzelfde is als hier opgegeven.
        if (gebruiker.Mailadres != mailadres.Address)
        {
            // Het is een ander, dus sturen we ook nog een mailtje naar het hier opgegeven adres,
            // voor de zekerheid, met instructies om het wachtwoord van de login te kunnen aanpassen.
            var msr = new MailServiceReference.MailServiceSoapClient();
            string boodschap;

            boodschap = String.Format(Mailopbouw.VerschillendWachtwoordMail, voornaam);
            msr.VerstuurMail("Helpdesk@chiro.be", mailadres.Address, "Je Chirologin", boodschap, "IntranetService");
        }
        return gebruiker.Login;
    }
}