// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Diagnostics;
using System.Linq;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// TODO (#190): documenteren
    /// </summary>
    public class DeelnemersManager
    {
        private readonly IAutorisatieManager _autorisatieMgr;

        public DeelnemersManager(IAutorisatieManager autorisatieManager)
        {
            _autorisatieMgr = autorisatieManager;
        }

        /// <summary>
        /// Stelt de gegeven <paramref name="deelnemer"/> in als contactpersoon voor de uitstap waaraan
        /// hij deelneemt.
        /// </summary>
        /// <param name="deelnemer">
        /// De als contactpersoon in te stellen deelnemer
        /// </param>
        /// <returns>
        /// Diezelfde deelnemer
        /// </returns>
        /// <remarks>
        /// De deelnemer moet aan zijn uitstap gekoppeld zijn
        /// </remarks>
        public Deelnemer InstellenAlsContact(Deelnemer deelnemer)
        {
            if (!_autorisatieMgr.IsGav(deelnemer))
            {
                throw new GeenGavException();
            }
            else
            {
                Debug.Assert(deelnemer.Uitstap != null);

                if (deelnemer.UitstapWaarvoorVerantwoordelijk.FirstOrDefault() == null)
                {
                    // Een deelnemer kan alleen contact voor zijn eigen uitstap zijn.  Is de deelnemer
                    // al contact voor een uitstap, dan volgt daaruit dat hij al contact is voor zijn
                    // eigen uitstap.
                    var vorigeVerantwoordelijke = deelnemer.Uitstap.ContactDeelnemer;

                    if (vorigeVerantwoordelijke != null)
                    {
                        vorigeVerantwoordelijke.UitstapWaarvoorVerantwoordelijk = null;
                    }

                    deelnemer.Uitstap.ContactDeelnemer = deelnemer;
                    deelnemer.UitstapWaarvoorVerantwoordelijk.Add(deelnemer.Uitstap);
                }
            }

            return deelnemer;
        }
    }
}