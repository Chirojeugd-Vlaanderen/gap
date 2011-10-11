using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Cdf.Mailer
{
    public interface IMailer
    {
        /// <summary>
        /// Verzendt een mail naar <paramref name="ontvanger"/>, met gegeven <paramref name="onderwerp"/> en
        /// <paramref name="ontvanger"/>
        /// </summary>
        /// <param name="ontvanger">e-mailadres van de geadresseerde</param>
        /// <param name="onderwerp">onderwerp van de mail</param>
        /// <param name="body">body van de mail</param>
        /// <returns><c>true</c> als het bericht verstuurd is, anders <c>false</c>.</returns>
        bool Verzenden(string ontvanger, string onderwerp, string body);
    }
}
