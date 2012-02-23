// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Cdf.Mailer
{
    /// <summary>
    /// Interface voor de mailservice
    /// </summary>
    public interface IMailer
    {
        /// <summary>
        /// Verstuurt een mail naar <paramref name="ontvanger"/>, met gegeven <paramref name="onderwerp"/> en
        /// <paramref name="ontvanger"/>
        /// </summary>
        /// <param name="ontvanger">E-mailadres van de geadresseerde</param>
        /// <param name="onderwerp">Onderwerp van de mail</param>
        /// <param name="body">Inhoud van de mail</param>
        /// <returns><c>True</c> als het bericht verstuurd is, anders <c>false</c>.</returns>
        bool Verzenden(string ontvanger, string onderwerp, string body);
    }
}
