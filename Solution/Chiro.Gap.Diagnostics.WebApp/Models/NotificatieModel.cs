using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.Diagnostics.ServiceContracts.DataContracts;

namespace Chiro.Gap.Diagnostics.WebApp.Models
{
    /// <summary>
    /// Model met informatie over wie van de groep een 
    /// notificatie krijgt dat de gebruiker aan het prutsen is :-)
    /// </summary>
    public class NotificatieModel
    {
        /// <summary>
        /// Algemene informatie over de groep en contact-e-mailadressen
        /// </summary>
        public GroepContactInfo GroepContactInfo { get; set; }

        /// <summary>
        /// De gebruiker kan een reden meegeven die mee opgenomen zal worden in het mailtje dat naar
        /// de contactpersoon gestuurd zal worden.
        /// </summary>
        public string Reden { get; set; }
    }
}