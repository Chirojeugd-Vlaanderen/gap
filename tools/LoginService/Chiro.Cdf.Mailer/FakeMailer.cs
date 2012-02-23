// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Cdf.Mailer
{
    /// <summary>
    /// Class die de mailservice mockt
    /// </summary>
    public class FakeMailer : IMailer
    {
        /// <summary>
        /// Doet alsof het een mailtje verstuurt
        /// </summary>
        /// <param name="ontvanger">
        /// Het adres van degene naar wie de mail verstuurd moet worden
        /// </param>
        /// <param name="onderwerp">
        /// Het onderwerp van het mailtje
        /// </param>
        /// <param name="body">
        /// De inhoud van het mailtje
        /// </param>
        /// <returns>
        /// <c>True</c> als het mailtje verstuurd is (wat in deze mockversie
        /// natuurlijk altijd het geval is)
        /// </returns>
        public bool Verzenden(string ontvanger, string onderwerp, string body)
        {
            return true;
        }
    }
}
