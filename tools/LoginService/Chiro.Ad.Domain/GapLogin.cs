// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.DirectoryServices;


namespace Chiro.Ad.Domain
{
    /// <summary>
    /// Class voor bewerkingen op een GapLogin
    /// </summary>
    public class GapLogin : Chirologin
    {
        /// <summary>
        /// AD-pad van de node waar nieuwe logins aangemaakt worden voor GAP-accounts
        /// </summary>
        readonly static String ouPad =Properties.Settings.Default.GapOU;

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
            if (!SecurityGroepen.Contains(Properties.Settings.Default.GapGebruikersGroep))
            {
                DirectoryEntry groep = LdapHelper.ZoekenUniek(Domein + Properties.Settings.Default.GapGroepenOU,
                                                            "Name=" + Properties.Settings.Default.GapGebruikersGroep);

                AanSecuritygroepToevoegen(groep);
            }
        }
    }
}