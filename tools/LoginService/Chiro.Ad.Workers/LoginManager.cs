﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Ad.Domain;
using Chiro.Cdf.Mailer;

namespace Chiro.Ad.Workers
{
    /// <summary>
    /// De bedoeling is dat de businesslogica voor de logins verhuist naar deze klasse.
    /// Zie ticket #1152.
    /// </summary>
    public class LoginManager
    {
        private readonly IMailer _mailer;

        /// <summary>
        /// Standaardconstructor
        /// </summary>
        /// <param name="mailer">IMailer via dewelke mails verstuurd zullen worden.</param>
        public LoginManager(IMailer mailer)
        {
            _mailer = mailer;
        }


        /// <summary>
        /// Het opgegeven wachtwoord instellen en de account enablen
        /// </summary>
        /// <param name="login">Login waarvoor wachtwoord moet worden ingesteld</param>
        /// <param name="nieuwWachtWoord">Het wachtwoord dat ingesteld moet worden. Als dat een lege
        /// string is, wordt een willekeurig wachtwoord gegenereerd.</param>
        public void Activeren(GapLogin login, string nieuwWachtWoord)
        {
            var wachtWoord = nieuwWachtWoord == string.Empty ? WachtWoordMaken() : nieuwWachtWoord;
            login.Activeren();
            login.Wachtwoord = wachtWoord;
            login.Opslaan();
        }

        /// <summary>
        /// Maakt een nieuw willekeurig wachtwoord
        /// </summary>
        /// <returns>Het wachtwoord</returns>
        private static string WachtWoordMaken()
        {
            return RandomPassword.Generate();
        }


        /// <summary>
        /// Activeert de gegeven <paramref name="login"/>, en stuurt de gebruiker een mailtje
        /// </summary>
        /// <param name="login">Te activeren login</param>
        public void ActiverenEnMailen (GapLogin login)
        {
            bool mailVerstuurd = false;
            string boodschap;

            // Alleen als de gebruiker niet-actief is, moet er nog een wachtwoord ingesteld worden
            if (login.IsActief == false)
            {
                string wachtWoord = WachtWoordMaken();
                Activeren(login, wachtWoord);

                boodschap = string.Format(Properties.Resources.GapAccountInfoMail, login.Naam, login.Login, wachtWoord);
            }
            else // Bestaande account
            {
                // OPM: als de account al bestond en alleen GAP-rechten had, dan klopt dat mailtje niet
                var b = new StringBuilder();
                b.Append(string.Format(Properties.Resources.AccountUitbreidingMailAanhef, login.Naam));
                b.Append(Properties.Resources.RechtenUitbreidingGap);
                b.Append(Properties.Resources.AccountMailAfsluiting);

                boodschap = b.ToString();
            }

            mailVerstuurd = _mailer.Verzenden(login.Mailadres, "Je GAP-login", boodschap);

            if (!mailVerstuurd)
            {
                throw new ApplicationException(Properties.Resources.MailNietVerstuurd);
            }            
        }
    }
}