﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security;
using System.Text.RegularExpressions;
using App_Code;
using Resources;

/// <summary>
/// Algemene class voor beheer van Chirologins
/// </summary>
public class Chirologin
{
    #region  properties

    /// <summary>
    /// De account zelf
    /// </summary>
    private readonly DirectoryEntry _gebruiker;

    /// <summary>
    /// Het domein in Active Directory waar de account zich bevindt
    /// </summary>
    public String Domein { get; set; }

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
    /// Het AD-nummer van de gebruiker
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
            if (Regex.IsMatch(value, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
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

    /// <summary>
    /// Securitygroepen waar de account member van is
    /// </summary>
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

    /// <summary>
    /// Geeft aan of de account al bestond of dat hij nieuw aangemaakt is
    /// </summary>
    public bool BestondAl { get; set; }

    #endregion

    /// <summary>
    /// De standaardconstructor
    /// </summary>
    /// <param name="domein">Rootpad in Active Directory</param>
    /// <param name="ouPad">Relatief pad naar de organisational unit waar nieuwe accounts in aangemaakt worden</param>
    /// <param name="adNr">Het AD-nummer van de nieuwe gebruiker</param>
    /// <param name="voornaam">De voornaam van de nieuwe gebruiker</param>
    /// <param name="familienaam">De naam van de nieuwe gebruiker</param>
    public Chirologin(DomeinEnum domein, String ouPad, Int32 adNr, String voornaam, String familienaam)
    {
        string naamVoluit = String.Concat(voornaam + " " + familienaam);
        DirectoryEntry ou;

        // Controleer of er nog geen account bestaat met die login, en suggereer eventueel een andere
        const int voornaamSeed = 2;
        const int familienaamSeed = 5;
        Zoekresultaat zoeker;

        switch (domein)
        {
            case DomeinEnum.Lokaal:
                Domein = ConfigurationManager.AppSettings["LdapLokaalRoot"];
                break;
            case DomeinEnum.Wereld:
                Domein = ConfigurationManager.AppSettings["LdapWereldRoot"];
                break;
            default:
                throw new ArgumentOutOfRangeException("domein");
        }

        zoeker = new Zoekresultaat(Domein, string.Format("(pager={0})", adNr));

        if (zoeker.UniekResultaat != null)
        {
            // 't Is den diene!
            _gebruiker = zoeker.UniekResultaat;
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                string login = LoginSuggereren(voornaam, familienaam, familienaamSeed - i, voornaamSeed + i);

                // Controleer of er nog geen account bestaat met die login
                zoeker = new Zoekresultaat(Domein, string.Concat("sAMAccountName=", login));

                // Als we niets vinden, kunnen we hiermee verder.
                if (zoeker.UniekResultaat == null)
                {
                    // Tweede controle: bestaat er al iemand met die naam? Kan anders namelijk niet toegevoegd worden.
                    zoeker = new Zoekresultaat(Domein, string.Format("(&(givenName={0})(sn={1}))", voornaam, familienaam));
                    if (zoeker.UniekResultaat == null)
                    {
                        // Zoek de 'organisational unit' op waar de account in terecht moet komen en zet de account erin.    
                        ou = new DirectoryEntry(Domein + ouPad);
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
                        break;
                    }
                    else
                    {
                        throw new ArgumentException("We hebben al een account voor iemand met dezelfde naam");
                    }
                }
                else if (i == 3)
                {
                    // We vinden iemand, maar het is iemand anders, en we zitten in de laatste loop.
                    throw new InvalidOperationException(
                        "We kunnen die persoon geen login meer geven met de automatische procedure. Neem contact op met het nationaal secretariaat voor manuele verwerking.");
                }
            }
        }
    }

    /// <summary>
    /// Login samenstellen op basis van voornaam en naam, en het opgegeven aantal letters van elk
    /// </summary>
    /// <param name="voornaam">De voornaam</param>
    /// <param name="familienaam">De familienaam</param>
    /// <param name="familienaamSeed">Het aantal letters dat we gebruiken van de familienaam</param>
    /// <param name="voornaamSeed">Het aantal letters dat we gebruiken van de voornaam</param>
    /// <returns>Een mogelijke gebruikersnaam, samengesteld op basis van de opgegeven parameters</returns>
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
        using (var pc = new PrincipalContext(ContextType.Domain, Domein.Substring(7)))
        {
            // Login van een account operator valideren
            // TODO: beter oplossen! (of web.config encrypteren)
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

    /// <summary>
    /// Het opgegeven wachtwoord instellen en de account enablen
    /// </summary>
    /// <param name="nieuwWachtwoord">Het wachtwoord dat ingesteld moet worden. Als dat een lege
    /// string is, gebruiken we "Chirojeugd!" - alleen voor intern gebruik, dus.</param>
    public void Activeren(String nieuwWachtwoord)
    {
        Wachtwoord = nieuwWachtwoord == string.Empty ? "Chirojeugd!" : nieuwWachtwoord;
        _gebruiker.Properties["userAccountControl"].Value = 66048; // enabled
        _gebruiker.Invoke("setPassword", Wachtwoord);
        Opslaan();
    }

    /// <summary>
    /// De account toevoegen aan de opgegeven securitygroep
    /// </summary>
    /// <param name="groep">De node in Active Directory waar de securitygroep zich bevindt</param>
    public void AanSecuritygroepToevoegen(DirectoryEntry groep)
    {
        groep.Invoke("Add", _gebruiker.Path);
        groep.CommitChanges();
    }
}