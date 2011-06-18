// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2009-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Resources;

// using Chiro.Validatie;

/// <summary>
/// Class voor bewerkingen op een GapLogin
/// </summary>
public class GapLogin
{
    readonly String domein = ConfigurationManager.AppSettings["LdapRoot"];
    readonly String ouPad = ConfigurationManager.AppSettings["GapOU"];

    #region  properties

    /// <summary>
    /// De account zelf
    /// </summary>
    private readonly DirectoryEntry _gebruiker;

    /// <summary>
    /// De login van de gebruiker
    /// </summary>
    public String Login
    {
        get
        {
            return _gebruiker.Properties["sAMAccountName"].Value.ToString();
        }
    }

    /// <summary>
    /// De naam van de gebruiker
    /// </summary>
    public String Naam
    {
        get
        {
            // Verwijder "CN=" uit de info die AD geeft, de rest is de naam
            return _gebruiker.Name.Remove(0, 3);
        }
    }

    /// <summary>
    /// Het AdNr van de gebruiker
    /// </summary>
    public Int32 AdNr
    {
        get
        {
            return _gebruiker.Properties["pager"].Value == null ? 0 : Int32.Parse(_gebruiker.Properties["pager"].Value.ToString());
        }
        set
        {
            _gebruiker.Properties["pager"].Value = value;
        }
    }

    /// <summary>
    /// Het mailadres van de gebruiker
    /// </summary>
    public String Mailadres
    {
        get
        {
            if (_gebruiker.Properties["mail"].Value != null)
            {
                return _gebruiker.Properties["mail"].Value.ToString();
            }
            return string.Empty;
        }
        set
        {
            if (!Regex.IsMatch(value, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
            {
                _gebruiker.Properties["mail"].Value = value;
            }
            else
            {
                throw new FormatException(Feedback.OngeldigMailadresFout);
            }
        }
    }

    /// <summary>
    /// De beschrijving (Description) van de gebruiker
    /// </summary>
    public String Beschrijving
    {
        get
        {
            if (_gebruiker.Properties["description"].Value != null)
            {
                return _gebruiker.Properties["description"].Value.ToString();
            }
            return string.Empty;
        }
        set
        {
            _gebruiker.Properties["description"].Value = value;
        }
    }

    /// <summary>
    /// Waar bevindt de account zich precies in Active Directory?
    /// </summary>
    public String Path
    {
        get
        {
            return _gebruiker.Path;
        }
    }

    /// <summary>
    /// Is de account enabled?
    /// </summary>
    public bool IsActief
    {
        get
        {
            return (int)_gebruiker.Properties["userAccountControl"].Value == 66048;
        }
        private set
        {
            if (value == false)
            {
                _gebruiker.Properties["userAccountControl"].Value = 66050; // disabled
            }
        }
    }

    /// <summary>
    /// Het AD-wachtwoord van de gebruiker
    /// </summary>
    public String Wachtwoord { get; private set; }

    public ArrayList SecurityGroepen
    {
        get
        {
            var groepenlijst = new ArrayList();
            PropertyValueCollection collectie = _gebruiker.Properties["memberOf"];
            IEnumerator en = collectie.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Current != null)
                {
                    var groep = en.Current.ToString();
                    // en.Current heeft de vorm CN=Naam,OU=xxx,OU=yyy
                    // Het is Naam dat we eruit willen halen, dus de eerste drie tekens verwijderen, en dan lengte min die drie
                    groep = groep.Substring(3, groep.IndexOf(",") - 3);

                    if (!groepenlijst.Contains(groep))
                    {
                        groepenlijst.Add(groep);
                    }
                }
            }
            return groepenlijst;
        }
    }

    public bool BestondAl { get; set; }

    #endregion

    /// <summary>
    /// Zoekt of maakt een account in Active Directory
    /// </summary>
    /// <param name="adNr">AD-nummer van de persoon in kwestie</param>
    /// <param name="voornaam">Voornaam van die persoon</param>
    /// <param name="familienaam">Familienaam van die persoon</param>
    public GapLogin(Int32 adNr, String voornaam, String familienaam)
    {
        string nieuweLogin;
        string naamVoluit = String.Concat(voornaam + " " + familienaam);
        DirectoryEntry ou;

        // Controleer of er nog geen account bestaat met die login, en suggereer eventueel een andere
        const int voornaamSeed = 2;
        const int familienaamSeed = 5;
        string login;
        Zoekresultaat zoeker;

        zoeker = new Zoekresultaat(domein, string.Format("(pager={0})", adNr));

        if (zoeker.UniekResultaat != null)
        {
            // 't Is den diene!
            _gebruiker = zoeker.UniekResultaat;
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                login = LoginSuggereren(voornaam, familienaam, familienaamSeed - i, voornaamSeed + i);
                // Controleer of er nog geen account bestaat met die login
                zoeker = new Zoekresultaat(domein, string.Concat("sAMAccountName=", login));

                // Als we niets vinden, kunnen we hiermee verder.
                if (zoeker.UniekResultaat == null)
                {
                    // Tweede controle: bestaat er al iemand met die naam? Kan anders namelijk niet toegevoegd worden.
                    zoeker = new Zoekresultaat(domein, string.Format("(givenName={0}&&sn={1})", voornaam, familienaam));
                    if (zoeker.UniekResultaat == null)
                    {
                        // Zoek de 'organisational unit' op waar de account in terecht moet komen en zet de account erin.    
                        ou = new DirectoryEntry(ouPad);
                        _gebruiker = ou.Children.Add("CN=" + naamVoluit, "user");

                        // Vul de nodige gegevens in
                        // sAMAccountName is een verplicht veld, userPrincipalName is nodig voor ADAM
                        _gebruiker.Properties["sAMAccountName"].Value = login;
                        _gebruiker.Properties["userPrincipalName"].Value = login;

                        _gebruiker.Properties["givenName"].Value = voornaam;
                        _gebruiker.Properties["sn"].Value = familienaam;

                        _gebruiker.Properties["pager"].Value = adNr;

                        // Pad opslaan in description zodat ze makkelijker terug te vinden zijn bij opzoekingen in AD-tool
                        _gebruiker.Properties["description"].Value = _gebruiker.Path;

                        Opslaan();
                        ActiverenEnMailen();

                        break;
                    }
                }
                else if (i == 3)
                {
                    // We vinden iemand, maar het is iemand anders, en we zitten in de laatste loop.
                    throw new InvalidOperationException("We kunnen die persoon geen login meer geven met de automatische procedure. Neem contact op met het nationaal secretariaat voor manuele verwerking.");
                }
            }

            RechtenToekennen();
        }
    }

    private static string LoginSuggereren(string voornaam, string familienaam, int familienaamSeed, int voornaamSeed)
    {
        String login;

        if (familienaam.ToUpper().StartsWith("DE "))
        {
            familienaam = familienaam.Remove(0, 3);
        }
        if (familienaam.ToUpper().StartsWith("VAN DER "))
        {
            familienaam = familienaam.Remove(0, 8);
        }
        if (familienaam.ToUpper().StartsWith("VAN DEN "))
        {
            familienaam = familienaam.Remove(0, 8);
        }
        if (familienaam.ToUpper().StartsWith("VAN DE "))
        {
            familienaam = familienaam.Remove(0, 7);
        }
        if (familienaam.ToUpper().StartsWith("VAN "))
        {
            familienaam = familienaam.Remove(0, 4);
        }

        // haal spaties, weglatingstekens en koppeltekens uit de naam
        voornaam = voornaam.Trim().Replace("'", string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);
        familienaam = familienaam.Trim().Replace("'", string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty);

        // vervang lettertekens met accenten of een trema door andere
        voornaam = voornaam.Replace("é", "e").Replace("è", "e").Replace("à", "a").Replace("ù", "u").Replace("î", "i").Replace("ï", "i").Replace("ê", "e").Replace("ë", "e");
        familienaam = familienaam.Replace("é", "e").Replace("è", "e").Replace("à", "a").Replace("ù", "u").Replace("î", "i").Replace("ï", "i").Replace("ê", "e").Replace("ë", "e");

        // vervang andere tekens
        voornaam = voornaam.Replace("ç", "c");
        familienaam = familienaam.Replace("ç", "c");

        if (familienaam.Length > familienaamSeed)
        {
            login = string.Concat(familienaam.Substring(0, familienaamSeed), voornaam.Substring(0, voornaamSeed));
        }
        else
        {
            Int32 lengte = familienaamSeed - familienaam.Length;
            if (voornaam.Length >= voornaamSeed + lengte)
            {
                login = string.Concat(familienaam, voornaam.Substring(0, voornaamSeed + lengte));
            }
            else
            {
                login = string.Concat(familienaam, voornaam);
            }
        }
        return login;
    }

    /// <summary>
    /// De wijzigingen persisteren in Active Directory
    /// </summary>
    public void Opslaan()
    {
        // Securitycontext opzetten voor schrijfoperatie)
        using (var pc = new PrincipalContext(ContextType.Domain, domein.Substring(7)))
        {
            // Login van een account operator valideren
            // TODO: beter oplossen?
            if (pc.ValidateCredentials(ConfigurationManager.AppSettings["ADLogin"], ConfigurationManager.AppSettings["ADPwd"]))
            {
                _gebruiker.CommitChanges();
            }
            else
            {
                throw new SecurityException("Kon niet authenticeren");
            }
        }
    }

    private void RechtenToekennen()
    {
        if (!SecurityGroepen.Contains(ConfigurationManager.AppSettings["GapGebruikersGroep"]))
        {
            DirectoryEntry groep;
            var zoeker = new Zoekresultaat(domein + ConfigurationManager.AppSettings["GapGroepenOU"],
                                           "Name=" + ConfigurationManager.AppSettings["GapGebruikersGroep"]);

            groep = zoeker.UniekResultaat;
            groep.Invoke("Add", _gebruiker.Path);
            groep.CommitChanges();
        }
    }

    /// <summary>
    /// Maak een willekeurig samengesteld wachtwoord dat aan de geldende regels voldoet
    /// </summary>
    /// <returns>Een wachtwoord</returns>
    private static String WachtwoordMaken()
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
    /// Het opgegeven wachtwoord instellen en de account enablen
    /// </summary>
    private void Activeren(String wachtwoord)
    {
        Wachtwoord = wachtwoord;
        _gebruiker.Properties["userAccountControl"].Value = 66048; // enabled
        _gebruiker.Invoke("setPassword", wachtwoord);
        Opslaan();
    }

    /// <summary>
    /// Het wachtwoord resetten, de account enablen en de accounteigenaar een mailtje sturen 
    /// om hem of haar de (nieuwe) logingegevens te bezorgen
    /// </summary>
    /// <param name="isReset">
    /// Wordt de account gereset (<c>true</c>) of gewoon geactiveerd (<c>false</c>)? Dat bepaalt mee wat er in het mailtje komt.</param>
    /// <param name="soort">
    /// Waarvoor moet de account dienen? Dat bepaalt mee wat er in het mailtje komt 
    /// (o.a. de url's voor de applicatie en voor wachtwoordwijzigingen).</param>
    public void ActiverenEnMailen()
    {
        var msr = new MailServiceReference.MailServiceSoapClient();
        MailServiceReference.BerichtStatus status;
        string boodschap;

        // Alleen als de gebruiker niet-actief is, moet er nog een wachtwoord ingesteld worden
        if (IsActief == false)
        {
            Wachtwoord = WachtwoordMaken();
            _gebruiker.Properties["userAccountControl"].Value = 66048; // enabled
            _gebruiker.Invoke("setPassword", Wachtwoord);
            Opslaan();

            boodschap = string.Format(Mailopbouw.AccountInfoMailAanhef, Naam, Login, Wachtwoord) +
                        Mailopbouw.AccountMailAfsluiting;
            status = msr.VerstuurMail("Helpdesk@chiro.be", Mailadres, "Je Chirologin", boodschap, "IntranetService");
        }
        else // Bestaande account
        {
            boodschap = string.Format(Mailopbouw.AccountUitbreidingMailAanhef, Naam);

            boodschap += Mailopbouw.RechtenUitbreidingGAP + Mailopbouw.AccountMailAfsluiting;
            status = msr.VerstuurMail("Het GAP-team", Mailadres, "Je GAP-login", boodschap, "GAP");
        }

        if (status.IsVerstuurd == false)
        {
            throw new ApplicationException(status.LogBoodschap);
        }
    }
}