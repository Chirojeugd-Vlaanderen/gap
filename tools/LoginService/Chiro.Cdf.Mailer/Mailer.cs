using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Chiro.Cdf.Mailer
{
    public class Mailer: IMailer
    {
        /// <summary>
        /// Verzendt een mail naar <paramref name="ontvanger"/>, met gegeven <paramref name="onderwerp"/> en
        /// <paramref name="ontvanger"/>
        /// </summary>
        /// <param name="ontvanger">e-mailadres van de geadresseerde</param>
        /// <param name="onderwerp">onderwerp van de mail</param>
        /// <param name="body">body van de mail.  Enkel tekst!</param>
        /// <returns><c>true</c> als het bericht verstuurd is, anders <c>false</c>.</returns>
        /// <remarks>Ik werk voorlopig text only om te vermijden dat een user html zou kunnen injecteren in het mailtje.</remarks>
        public bool Verzenden(string ontvanger, string onderwerp, string body)
        {
            MailServiceReference.BerichtStatus status;

            // Ik zou hier ook graag met ServiceHelper werken, maar dat kan niet direct omdat ik niet
            // direct de interface van de service heb.  Bovendien ben ik niet zeker of ServiceHelper
            // werkt als het geen WCF-services zijn.

            using (var msr = new MailServiceReference.MailServiceSoapClient())
            {
                // Met de mails die via de mailer service worden verstuurd, gebeurt iets raar met de inhoud.
                // De constructie met <pre>...</pre> lost dit op voor html-gebaseerde mail clients

                status = msr.VerstuurGapMail(ontvanger, onderwerp, String.Format("<pre>\n{0}\n</pre>", HttpUtility.HtmlEncode(body)));
            }
            return status.IsVerstuurd;
        }
    }
}
