using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// <param name="body">body van de mail</param>
        /// <returns><c>true</c> als het bericht verstuurd is, anders <c>false</c>.</returns>
        public bool Verzenden(string ontvanger, string onderwerp, string body)
        {
            MailServiceReference.BerichtStatus status;

            // Ik zou hier ook graag met ServiceHelper werken, maar dat kan niet direct omdat ik niet
            // direct de interface van de service heb.  Bovendien ben ik niet zeker of ServiceHelper
            // werkt als het geen WCF-services zijn.

            using (var msr = new MailServiceReference.MailServiceSoapClient())
            {
                status = msr.VerstuurGapMail(ontvanger, onderwerp, body);
            }
            return status.IsVerstuurd;
        }
    }
}
