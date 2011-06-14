// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Diagnostics;
using System.Linq;

using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// TODO (#190): documenteren
    /// </summary>
    public class DeelnemersManager
    {
        private readonly IDeelnemersDao _deelnemersDao;
        private readonly IAutorisatieManager _autorisatieMgr;

        /// <summary>
        /// Dependency injection via deze constructor
        /// </summary>
        /// <param name="deelnemersDao">deelnemersdao</param>
        /// <param name="autorisatieManager">autorisatiemanager</param>
        public DeelnemersManager(IDeelnemersDao deelnemersDao, IAutorisatieManager autorisatieManager)
        {
            _deelnemersDao = deelnemersDao;
            _autorisatieMgr = autorisatieManager;
        }

        /// <summary>
        /// Haalt een deelnemer op, inclusief persoon en uitstap.
        /// </summary>
        /// <param name="deelnemerID">ID op te halen deelnemer</param>
        /// <returns>Deelnemer, inclusief persoon, uitstap, groepswerkjaar, groep</returns>
        public Deelnemer Ophalen(int deelnemerID)
        {
            if (!_autorisatieMgr.IsGavDeelnemer(deelnemerID))
            {
                throw new GeenGavException();
            }
            else
            {
                return _deelnemersDao.Ophalen(
                    deelnemerID,
                    d => d.GelieerdePersoon.Persoon,
                    d => d.Uitstap.GroepsWerkJaar.Groep);
            }
        }

        /// <summary>
        /// Stelt de gegeven <paramref name="deelnemer"/> in als contactpersoon voor de uitstap waaraan
        /// hij deelneemt.
        /// </summary>
        /// <param name="deelnemer">De als contactpersoon in te stellen deelnemer</param>
        /// <returns>Diezelfde deelnemer</returns>
        /// <remarks>De deelnemer moet aan zijn uitstap gekoppeld zijn</remarks>
        public Deelnemer InstellenAlsContact(Deelnemer deelnemer)
        {
            if (!_autorisatieMgr.IsGavDeelnemer(deelnemer.ID))
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

        /// <summary>
        /// Verwijdert de gegeven <paramref name="deelnemer"/> van zijn uitstap.  Persisteert
        /// </summary>
        /// <param name="deelnemer">Te verwijderen deelnemer.</param>
        public void Verwijderen(Deelnemer deelnemer)
        {
            if (!_autorisatieMgr.IsGavDeelnemer(deelnemer.ID))
            {
                throw new GeenGavException();
            }

            // Ik ga die deelnemer verwijderen via een stored procedure, omdat ik het anders
            // niet goed krijg als de deelnemer ook contact is (denk ik).

            _deelnemersDao.Verwijderen(deelnemer);
        }

        /// <summary>
        /// Bewaart de deelnemer, en enkel de deelnemer.  Koppelingen met andere entiteiten worden
        /// momenteel genegeerd.
        /// </summary>
        /// <param name="deelnemer">Te bewaren deelnemer</param>
        public void Bewaren(Deelnemer deelnemer)
        {
            if (!_autorisatieMgr.IsGavDeelnemer(deelnemer.ID))
            {
                throw new GeenGavException();
            }

            // Bewaren

            _deelnemersDao.Bewaren(deelnemer);
        }
    }
}
