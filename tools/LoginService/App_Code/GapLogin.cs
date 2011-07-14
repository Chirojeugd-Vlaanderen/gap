// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Configuration;
using System.DirectoryServices;
using System.Text;
using App_Code;
using Resources;

// using Chiro.Validatie;

/// <summary>
/// Class voor bewerkingen op een GapLogin
/// </summary>
public class GapLogin : Chirologin
{
    /// <summary>
    /// AD-pad van de node waar nieuwe logins aangemaakt worden voor GAP-accounts
    /// </summary>
    readonly static String ouPad = ConfigurationManager.AppSettings["GapOU"];

    /// <summary>
    /// Zoekt of maakt een account in Active Directory.
    /// </summary>
    /// <param name="adNr">AD-nummer van de persoon in kwestie</param>
    /// <param name="voornaam">Voornaam van die persoon</param>
    /// <param name="familienaam">Familienaam van die persoon</param>
    /// <remarks>De account is nog niet actief en bevat nog geen mailadres.</remarks>
    public GapLogin(Int32 adNr, String voornaam, String familienaam)
        : base(DomeinEnum.Wereld, ouPad, adNr, voornaam, familienaam)
    {
        RechtenToekennen();
    }

    /// <summary>
    /// Geef de login de nodige rechten in Active Directory om aan de GAP-toepassing te kunnen
    /// </summary>
    private void RechtenToekennen()
    {
        if (!SecurityGroepen.Contains(ConfigurationManager.AppSettings["GapGebruikersGroep"]))
        {
            DirectoryEntry groep;
            var zoeker = new Zoekresultaat(Domein + ConfigurationManager.AppSettings["GapGroepenOU"],
                                           "Name=" + ConfigurationManager.AppSettings["GapGebruikersGroep"]);

            groep = zoeker.UniekResultaat;
            AanSecuritygroepToevoegen(groep);
        }
    }

    /// <summary>
    /// Maak een willekeurig samengesteld wachtwoord dat aan de geldende regels voldoet
    /// </summary>
    /// <returns>Een wachtwoord</returns>
    private static string WachtwoordMaken()
    {
        // regels: minstens 3 van de 4 volgende tekens gebruiken: kleine letters, hoofdletters, leestekens en cijfers
        // praktijk: begin met 3 hoofdletters (begin van de applicatienaam), dan een leesteken, en daarna zes kleine letters
        var sb = new StringBuilder();
        sb.Append("GAP");
        sb.Append("!");
        sb.Append(System.IO.Path.GetRandomFileName().Substring(0, 6));
        return sb.ToString();
    }

    /// <summary>
    /// Het wachtwoord resetten, de account enablen en de accounteigenaar een mailtje sturen 
    /// om hem of haar de (nieuwe) logingegevens te bezorgen
    /// </summary>
    /// <param name="isReset">
    /// Wordt de account gereset (<c>true</c>) of gewoon geactiveerd (<c>false</c>)? Dat bepaalt mee wat er in het mailtje komt.</param>
    public void ActiverenEnMailen(bool isReset)
    {
        var msr = new MailServiceReference.MailServiceSoapClient();
        MailServiceReference.BerichtStatus status;
        string boodschap;

        // Alleen als de gebruiker niet-actief is, moet er nog een wachtwoord ingesteld worden
        if (IsActief == false)
        {
            Activeren(WachtwoordMaken());

            boodschap = string.Format(Mailopbouw.GapAccountInfoMail, Naam, Login, Wachtwoord);
            status = msr.VerstuurMail("Helpdesk@chiro.be", Mailadres, "Je GAP-login", boodschap, "IntranetService");
        }
        else // Bestaande account
        {
            // OPM: als de account al bestond en alleen GAP-rechten had, dan klopt dat mailtje niet
            boodschap = string.Format(Mailopbouw.AccountUitbreidingMailAanhef, Naam);

            boodschap += Mailopbouw.RechtenUitbreidingGAP + Mailopbouw.AccountMailAfsluiting;
            status = msr.VerstuurMail("Het GAP-team", Mailadres, "Je Chirologin", boodschap, "GAP");
        }

        if (status.IsVerstuurd == false)
        {
            throw new ApplicationException(status.LogBoodschap);
        }
    }
}