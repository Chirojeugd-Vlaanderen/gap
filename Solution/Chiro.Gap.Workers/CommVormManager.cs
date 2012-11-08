// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

#if KIPDORP
using System.Linq;
using System.Transactions;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.Validatie;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

using CommunicatieType = Chiro.Gap.Poco.Model.CommunicatieType;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. communicatievormen bevat (telefoonnummer, mailadres, enz.)
    /// </summary>
    public class CommVormManager
    {
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly ICommunicatieSync _communicatieSync;
        private readonly IPersonenSync _personenSync;

        public CommVormManager(
            IAutorisatieManager autorisatieMgr, 
            ICommunicatieSync communicatieSync, 
            IPersonenSync personenSync)
        {
            _autorisatieMgr = autorisatieMgr;
            _communicatieSync = communicatieSync;
            _personenSync = personenSync;
        }

        /// <summary>
        /// Koppelt een communicatievorm aan een gelieerde persoon.
        /// </summary>
        /// <param name="gp">
        /// De gelieerde persoon voor wie de communicatievorm toegevoegd of aangepast wordt
        /// </param>
        /// <param name="nieuwecv">
        /// De nieuwe gegevens voor de communicatievorm
        /// </param>
        /// <remarks>
        /// Als de communicatievorm de eerste van een bepaald type is, dan wordt dat ook de voorkeur.
        /// </remarks>
        public void Koppelen(GelieerdePersoon gp, CommunicatieVorm nieuwecv)
        {
            if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            var cvValid = new CommunicatieVormValidator();

            if (!cvValid.Valideer(nieuwecv))
            {
                throw new ValidatieException(string.Format(Resources.CommunicatieVormValidatieFeedback, 
                                                           nieuwecv.Nummer, 
                                                           nieuwecv.CommunicatieType.Omschrijving));
            }

            Debug.Assert(nieuwecv.ID == 0);

            bool eersteVanType = (from c in gp.Communicatie
                                  where c.CommunicatieType.ID == nieuwecv.CommunicatieType.ID
                                  select c).FirstOrDefault() == null;

            gp.Communicatie.Add(nieuwecv);
            nieuwecv.GelieerdePersoon = gp;

            if (eersteVanType || nieuwecv.Voorkeur)
            {
                VoorkeurZetten(nieuwecv);
            }
        }

        /// <summary>
        /// Stelt de gegeven communicatievorm in als voorkeurscommunicatievorm voor zijn
        /// type en gelieerde persoon
        /// </summary>
        /// <param name="cv">
        /// Communicatievorm die voorkeurscommunicatievorm moet worden,
        /// gegeven zijn type en gelieerde persoon
        /// </param>
        public void VoorkeurZetten(CommunicatieVorm cv)
        {
            if (!_autorisatieMgr.IsGavCommVorm(cv.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            var validator = new CommunicatieVormValidator();

            if (!validator.Valideer(cv))
            {
                throw new ValidatieException(string.Format(Resources.CommunicatieVormValidatieFeedback, 
                                                           cv.Nummer, 
                                                           cv.CommunicatieType.Omschrijving));
            }

            foreach (
                var communicatieVorm in
                    cv.GelieerdePersoon.Communicatie.Where(c => c.CommunicatieType.ID == cv.CommunicatieType.ID).ToArray
                        ())
            {
                communicatieVorm.Voorkeur = communicatieVorm == cv;
            }
        }
    }
}