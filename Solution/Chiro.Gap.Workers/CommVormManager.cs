using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Validatie;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    public class CommVormManager
    {
        private IDao<CommunicatieType> _typedao;
        private IDao<CommunicatieVorm> _dao;
        private IAutorisatieManager _autorisatieMgr;
        private IGelieerdePersonenDao _geldao;

        /// <summary>
        /// Deze constructor laat toe om een alternatieve repository voor
        /// de communicatievormen te gebruiken.  Nuttig voor mocking en testing.
        /// </summary>
        /// <param name="dao">Alternatieve dao</param>
        public CommVormManager(IDao<CommunicatieType> typedao, IDao<CommunicatieVorm> commdao, IAutorisatieManager autorisatieMgr,
                                IGelieerdePersonenDao geldao)
        {
            _typedao = typedao;
            _dao = commdao;
            _autorisatieMgr = autorisatieMgr;
            _geldao = geldao;
        }

        /// <summary>
        /// Haalt communicatievorm op, op basis van commvormID
        /// </summary>
        /// <param name="commvormID">ID op te halen communicatievorm</param>
        /// <returns>gevraagde communicatievorm</returns>
        public CommunicatieVorm Ophalen(int commvormID)
        {
            if (_autorisatieMgr.IsGavCommVorm(commvormID))
            {
                return _dao.Ophalen(commvormID, foo => foo.CommunicatieType);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGavCommVorm);
            }
        }

        /// <summary>
        /// Persisteert communicatievorm in de database
        /// </summary>
        /// <param name="g">Te persisteren communicatievorm</param>
        /// <returns>De bewaarde communicatievorm</returns>
        public CommunicatieVorm Bewaren(CommunicatieVorm commvorm)
        {
            if (_autorisatieMgr.IsGavCommVorm(commvorm.ID))
            {
                return _dao.Bewaren(commvorm);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGavCommVorm);
            }
        }

        public CommunicatieType CommunicatieTypeOphalen(int commTypeID)
        {
            return _typedao.Ophalen(commTypeID);
        }

        public IEnumerable<CommunicatieType> CommunicatieTypesOphalen()
        {
            return _typedao.AllesOphalen();
        }

        public CommunicatieVorm CommunicatieVormOphalen(int commvormID)
        {
            if (_autorisatieMgr.IsGavCommVorm(commvormID))
            {
                return _dao.Ophalen(commvormID, e => e.CommunicatieType);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGavCommVorm);
            }
        }

        public void CommunicatieVormToevoegen(CommunicatieVorm comm, int gelieerdePersoonID, int typeID)
        {
            if (!_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                throw new GeenGavException(Resources.GeenGavGroep);
            }
            GelieerdePersoon origineel = _geldao.Ophalen(gelieerdePersoonID, e => e.Persoon, e => e.Communicatie.First().CommunicatieType);
            CommunicatieType type = _typedao.Ophalen(typeID);
            CommunicatieVormValidator cvValid = new CommunicatieVormValidator();

            if (cvValid.Valideer(comm))
            {
                origineel.Communicatie.Add(comm);
                comm.CommunicatieType = type;
                _dao.Bewaren(comm, l => l.CommunicatieType.WithoutUpdate(), l => l.GelieerdePersoon.WithoutUpdate());
            }
            else
            {
                throw new ValidatieException(string.Format(Resources.CommunicatieVormValidatieFeedback, comm.Nummer, comm.CommunicatieType.Omschrijving));
            }
        }

	// FIXME: de parameter 'gelieerdePersoonID' is overbodig; zie ticket #145.
        public void CommunicatieVormVerwijderen(CommunicatieVorm comm, GelieerdePersoon origineel)
        {
            if (!_autorisatieMgr.IsGavGelieerdePersoon(origineel.ID))
            {
                throw new GeenGavException(Resources.GeenGavGroep);
            }
            if (!_autorisatieMgr.IsGavCommVorm(comm.ID))
            {
                throw new GeenGavException(Resources.GeenGavCommVorm);
            }
            comm.TeVerwijderen = true;
        }
    }
}
