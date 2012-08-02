// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

#if KIPDORP
using System.Transactions;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Chiro.Cdf.Data;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Validatie;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.Properties;

using CommunicatieType = Chiro.Gap.Orm.CommunicatieType;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. communicatievormen bevat (telefoonnummer, mailadres, enz.)
    /// </summary>
    public class CommVormManager
    {
        private readonly IDao<CommunicatieType> _typedao;
        private readonly ICommunicatieVormDao _dao;
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly ICommunicatieSync _communicatieSync;
        private readonly IPersonenSync _personenSync;

        /// <summary>
        /// Deze constructor laat toe om een alternatieve repository voor
        /// de communicatievormen te gebruiken.  Nuttig voor mocking en testing.
        /// </summary>
        /// <param name="typedao">
        /// Repository voor communicatietypes
        /// </param>
        /// <param name="commdao">
        /// Repository voor communicatievormen
        /// </param>
        /// <param name="autorisatieMgr">
        /// Worker die autorisatie regelt
        /// </param>
        /// <param name="communicatieSync">
        /// Syncer naar Kipadmin voor communicatiemiddelen
        /// </param>
        /// <param name="personenSync">
        /// De service die instaat voor synchronisatie van persoonsgegevens
        /// </param>
        public CommVormManager(
            IDao<CommunicatieType> typedao, 
            ICommunicatieVormDao commdao, 
            IAutorisatieManager autorisatieMgr, 
            ICommunicatieSync communicatieSync, 
            IPersonenSync personenSync)
        {
            _typedao = typedao;
            _dao = commdao;
            _autorisatieMgr = autorisatieMgr;
            _communicatieSync = communicatieSync;
            _personenSync = personenSync;
        }

        /// <summary>
        /// Haalt communicatievorm op, op basis van commvormID
        /// </summary>
        /// <param name="commvormID">
        /// ID op te halen communicatievorm
        /// </param>
        /// <returns>
        /// Gevraagde communicatievorm
        /// </returns>
        public CommunicatieVorm Ophalen(int commvormID)
        {
            if (!_autorisatieMgr.IsGavCommVorm(commvormID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            return _dao.Ophalen(commvormID, foo => foo.CommunicatieType);
        }

        /// <summary>
        /// Haalt gelieerdepersoon en zijn gelinkte commvormen, gegeven een ID van een van zijn commvormen
        /// </summary>
        /// <param name="commvormID">
        /// ID van een van de persoon zijn communicatievormen
        /// </param>
        /// <returns>
        /// Gevraagde communicatievorm
        /// </returns>
        public CommunicatieVorm OphalenMetGelieerdePersoon(int commvormID)
        {
            if (!_autorisatieMgr.IsGavCommVorm(commvormID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            return _dao.Ophalen(commvormID, 
                                foo => foo.CommunicatieType, 
                                foo => foo.GelieerdePersoon.Persoon, 
                                foo => foo.GelieerdePersoon.Communicatie, 
                                foo => foo.GelieerdePersoon.Communicatie.First().CommunicatieType);
        }

        /// <summary>
        /// Persisteert communicatievorm in de database
        /// </summary>
        /// <param name="commvorm">
        /// Te persisteren communicatievorm
        /// </param>
        /// <returns>
        /// De bewaarde communicatievorm
        /// </returns>
        /// <remarks>
        /// Persoon moet gekoppeld zijn
        /// </remarks>
        public CommunicatieVorm Bewaren(CommunicatieVorm commvorm)
        {
            bool isNieuw = commvorm.ID == 0;

            CommunicatieVorm resultaat;

            Debug.Assert(commvorm.GelieerdePersoon != null);
            Debug.Assert(commvorm.GelieerdePersoon.Persoon != null);

            if (!_autorisatieMgr.IsGavCommVorm(commvorm.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            var cvValidator = new CommunicatieVormValidator();

            if (!cvValidator.Valideer(commvorm))
            {
                throw new ValidatieException(string.Format(Resources.CommunicatieVormValidatieFeedback, 
                                                           commvorm.Nummer, 
                                                           commvorm.CommunicatieType.Omschrijving));
            }

#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
            resultaat = _dao.Bewaren(
                commvorm, 
                cv => cv.GelieerdePersoon.WithoutUpdate(), 
                cv => cv.CommunicatieType.WithoutUpdate());
            if (commvorm.GelieerdePersoon.Persoon.AdNummer != null || commvorm.GelieerdePersoon.Persoon.AdInAanvraag)
            {
                if (isNieuw)
                {
                    _communicatieSync.Toevoegen(commvorm);
                }
                else
                {
                    // bij update moet *alle* communicatie opnieuw naar Kipadmin, omdat we niet weten welke 
                    // precies de te vervangen communicatievorm is.
                    _personenSync.CommunicatieUpdaten(commvorm.GelieerdePersoon);
                }
            }

#if KIPDORP
				tx.Complete();
			}
#endif
            return resultaat;
        }

        /// <summary>
        /// Een communicatietype ophalen op basis van de opgegeven ID
        /// </summary>
        /// <param name="commTypeID">
        /// De ID van het communicatietype dat we nodig hebben
        /// </param>
        /// <returns>
        /// Het communicatietype met de opgegeven ID
        /// </returns>
        public CommunicatieType CommunicatieTypeOphalen(int commTypeID)
        {
            return _typedao.Ophalen(commTypeID);
        }

        /// <summary>
        /// Een collectie ophalen van alle gekende communicatietypes
        /// </summary>
        /// <returns>
        /// Een collectie van communicatietypes
        /// </returns>
        public IEnumerable<CommunicatieType> CommunicatieTypesOphalen()
        {
            return _typedao.AllesOphalen();
        }

        /// <summary>
        /// De communicatievorm met de opgegeven ID ophalen
        /// </summary>
        /// <param name="commvormID">
        /// De ID van de communicatievorm die je nodig hebt
        /// </param>
        /// <returns>
        /// De communicatievorm met de opgegeven ID
        /// </returns>
        public CommunicatieVorm CommunicatieVormOphalen(int commvormID)
        {
            if (_autorisatieMgr.IsGavCommVorm(commvormID))
            {
                return _dao.Ophalen(commvormID, e => e.CommunicatieType);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Verwijdert een communicatievorm, en persisteert.
        /// </summary>
        /// <param name="comm">
        /// Te verwijderen communicatievorm
        /// </param>
        public void CommunicatieVormVerwijderen(CommunicatieVorm comm)
        {
            if (!_autorisatieMgr.IsGavGelieerdePersoon(comm.GelieerdePersoon.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            if (!_autorisatieMgr.IsGavCommVorm(comm.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            comm.TeVerwijderen = true;
#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif

            // Indien ad-nummer: syncen
            if (comm.GelieerdePersoon.Persoon.AdNummer != null || comm.GelieerdePersoon.Persoon.AdInAanvraag)
            {
                _communicatieSync.Verwijderen(comm);
            }

            _dao.Bewaren(comm);
#if KIPDORP
				tx.Complete();
			}
#endif
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