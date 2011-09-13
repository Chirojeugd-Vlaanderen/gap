// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Linq;
// ReSharper disable RedundantUsingDirective
using System.Transactions;
// ReSharper restore RedundantUsingDirective

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Manager voor al wat met verzekeringen te maken heeft.
    /// TODO (#1045): Dit was misschien beter een 'PersoonsVerzekeringenManager' geweest?
    /// </summary>
    public class VerzekeringenManager
    {
        private readonly IDao<VerzekeringsType> _verzekeringenDao;
        private readonly IDao<PersoonsVerzekering> _persoonsVerzekeringenDao;
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly IVerzekeringenSync _sync;

        /// <summary>
        /// Construeert een nieuwe verzekeringenmanager
        /// </summary>
        /// <param name="vdao">Data Access Object voor verzekeringstypes</param>
        /// <param name="pvdao">Data Access Object voor persoonsverzekeringen</param>
        /// <param name="auMgr">Data Access Object voor autorisatie</param>
        /// <param name="sync">Proxy naar service om verzekeringen te syncen met Kipadmin</param>
        public VerzekeringenManager(
            IDao<VerzekeringsType> vdao,
            IDao<PersoonsVerzekering> pvdao,
            IAutorisatieManager auMgr,
            IVerzekeringenSync sync)
        {
            _verzekeringenDao = vdao;
            _persoonsVerzekeringenDao = pvdao;
            _autorisatieMgr = auMgr;
            _sync = sync;
        }

        /// <summary>
        /// Haalt een verzekeringstype op uit de database
        /// </summary>
        /// <param name="verzekering">ID van het type verzekering</param>
        /// <returns>Het verzekeringstype</returns>
        public VerzekeringsType Ophalen(Verzekering verzekering)
        {
            // GAV-rechten zijn hier irrelevant.
            return _verzekeringenDao.Ophalen((int)verzekering);
        }

        /// <summary>
        /// Verzekert een lid
        /// </summary>
        /// <param name="l">Te verzekeren lid, met daaraan gekoppeld alle verzekeringen</param>
        /// <param name="verz">Type van de verzekering</param>
        /// <param name="beginDatum">Begindatum van de verzekering; moet in de toekomst liggen.</param>
        /// <param name="eindDatum">Einddatum van de verzekering</param>
        /// <returns>Het gecreeerde PersoonsVerzekringsobject.</returns>
        public PersoonsVerzekering Verzekeren(Lid l, VerzekeringsType verz, DateTime beginDatum, DateTime eindDatum)
        {
            if (!_autorisatieMgr.IsGavLid(l.ID))
            {
                throw new GeenGavException();
            }

            if (beginDatum > eindDatum)
            {
                throw new FoutNummerException(FoutNummer.ChronologieFout, Properties.Resources.FouteDatumVolgorde);
            }

            if (beginDatum > DateTime.Now)
            {
                throw new FoutNummerException(FoutNummer.ChronologieFout, Properties.Resources.VerzekeringInVerleden);
            }

            if (verz.TotEindeWerkJaar && eindDatum != GroepsWerkJaarManager.EindDatum(l.GroepsWerkJaar))
            {
                throw new FoutNummerException(
                    FoutNummer.ValidatieFout,
                    Properties.Resources.OngeldigeEindDatumVerzekering);
            }

            // Onderstaande controle op 'al bestaande' verzekering, gebeurt niet door de database, omdat
            // dit meer inhoudt dan een unique index/unique constraint.

            var query = from v in l.GelieerdePersoon.Persoon.PersoonsVerzekering
                        where v.VerzekeringsType.ID == verz.ID
                          && (v.Tot > beginDatum && beginDatum >= v.Van || v.Van < eindDatum && eindDatum <= v.Tot)
                        select v;

            var bestaande = query.FirstOrDefault();

            if (bestaande != null)
            {
                throw new BlokkerendeObjectenException<PersoonsVerzekering>(
                    bestaande,
                    Properties.Resources.OverlappendeVerzekering);
            }

            var pv = new PersoonsVerzekering
                                        {
                                            Van = beginDatum,
                                            Tot = eindDatum,
                                            Persoon = l.GelieerdePersoon.Persoon,
                                            VerzekeringsType = verz
                                        };

            l.GelieerdePersoon.Persoon.PersoonsVerzekering.Add(pv);
            verz.PersoonsVerzekering.Add(pv);

            return pv;
        }

        /// <summary>
        /// Persisteert een persoonsverzekering, inclusief koppeling naar persoon en verzekeringstype
        /// </summary>
        /// <param name="verzekering">Te persisteren persoonsverzekering</param>
        /// <param name="gwj">Bepaalt werkjaar en groep die de factuur zal krijgen (Groep moet meegeleverd zijn)</param>
        /// <returns>De bewaarde versie van de persoonsverzekering</returns>
        public PersoonsVerzekering PersoonsVerzekeringBewaren(PersoonsVerzekering verzekering, GroepsWerkJaar gwj)
        {
            if (_autorisatieMgr.IsGavPersoon(verzekering.Persoon.ID))
            {
                PersoonsVerzekering resultaat;

#if KIPDORP
                using (var tx = new TransactionScope())
                {
#endif
                    if (verzekering.ID == 0)
                    {
                        resultaat = _persoonsVerzekeringenDao.Bewaren(verzekering,
                                                                      pv => pv.Persoon.WithoutUpdate(),
                                                                      pv => pv.VerzekeringsType.WithoutUpdate());
                    }
                    else
                    {
                        // Verzekeringen mogen niet vervangen worden.
                        // TODO (#1046): Exception throwen
                        // Voorlopig throwen we die exception niet, opdat we via louche truken
                        // op gemakkelijke wijze 'verloren' verzekeringen opnieuw zouden kunnen 
                        // 'kipsyncen'.

                        resultaat = verzekering;
                    }

                    _sync.Bewaren(resultaat, gwj);
#if KIPDORP
                    tx.Complete();
                }
#endif
                return resultaat;
            }
            else
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }
        }

        /// <summary>
        /// Haalt een persoonsverzekering op, op basis van zijn ID
        /// </summary>
        /// <param name="persoonsVerzekeringID">ID op te halen persoonsverzekering</param>
        /// <returns></returns>
        public PersoonsVerzekering PersoonsVerzekeringOphalen(int persoonsVerzekeringID)
        {
            return _persoonsVerzekeringenDao.Ophalen(persoonsVerzekeringID, pv => pv.Persoon, pv => pv.VerzekeringsType);
        }
    }
}
