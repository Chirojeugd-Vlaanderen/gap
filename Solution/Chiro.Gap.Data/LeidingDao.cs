// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
    /// <summary>
    /// Gegevenstoegangsobject voor leiding
    /// </summary>
    public class LeidingDao : Dao<Leiding, ChiroGroepEntities>, ILeidingDao
    {
        /// <summary>
        /// Instantieert een gegevenstoegangsobject voor leiding
        /// </summary>
        public LeidingDao()
        {
            ConnectedEntities = new Expression<Func<Leiding, object>>[] 
			{
				e => e.GroepsWerkJaar.WithoutUpdate(),
				e => e.GelieerdePersoon, 
				e => e.GelieerdePersoon.Persoon, 
				e => e.AfdelingsJaar.First(),
				e => e.AfdelingsJaar.First().Afdeling.WithoutUpdate()
			};
        }

        /// <summary>
        /// Zoekt leiding op, op basis van de gegeven <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">De niet-nulle properties van de filter
        /// bepalen waarop gezocht moet worden</param>
        /// <param name="extras">Bepaalt de mee op te halen gekoppelde entiteiten. 
        /// (Adressen ophalen vertraagt aanzienlijk.)
        /// </param>
        /// <returns>Lijst met info over gevonden leiding</returns>
        /// <remarks>
        /// Er wordt enkel actieve leiding opgehaald
        /// </remarks>
        public IEnumerable<Leiding> Zoeken(LidFilter filter, LidExtras extras)
        {
            Leiding[] resultaat;

            if ((filter.LidType & LidType.Leiding) == 0)
            {
                // Als er niet op leiding gezocht wordt, lever dan niets op
                return new Leiding[0];
            }

            using (var db = new ChiroGroepEntities())
            {
                IQueryable<Leiding> query;

                if (filter.AfdelingID != null)
                {
                    if (filter.GroepsWerkJaarID != null)
                    {
                        // Er zal nogal vaak gefilterd worden op een afdeling en een groepswerkjaar.
                        // Daarom beginnen we in dit specifieke geval met een geoptimaliseerde
                        // query.

                        query = db.AfdelingsJaar
                            .Where(aj => aj.GroepsWerkJaar.ID == filter.GroepsWerkJaarID && aj.Afdeling.ID == filter.AfdelingID)
                            .SelectMany(aj => aj.Leiding);
                    }
                    else
                    {
                        query = db.AfdelingsJaar
                            .Where(aj => aj.Afdeling.ID == filter.AfdelingID)
                            .SelectMany(aj => aj.Leiding);
                    }
                }
                else
                {
                    if (filter.GroepsWerkJaarID != null)
                    {
                        // Alle leden van een gegeven groepswerkjaar zal ook veel voorkomend zijn.

                        query = from l in db.Lid.OfType<Leiding>()
                                where l.GroepsWerkJaar.ID == filter.GroepsWerkJaarID
                                select l;
                    }
                    else
                    {
                        // Als er geen groepswerkjaar of afdelingsjaar gegeven is, beginnen
                        // we vanuit een standaardquery met alle leiding

                        query = from ld in db.Lid.OfType<Leiding>() select ld;
                    }
                }

                // Zo nodig zoekopdracht verfijnen

                if (filter.FunctieID != null)
                {
                    query = query.Where(ld => ld.Functie.Any(fn => fn.ID == filter.FunctieID));
                }

                if (filter.GroepID != null)
                {
                    query = query.Where(ld => ld.GroepsWerkJaar.Groep.ID == filter.GroepID);
                }

                if (filter.ProbeerPeriodeNa != null)
                {
                    query = query.Where(ld => ld.EindeInstapPeriode > filter.ProbeerPeriodeNa);
                }

                if (filter.HeeftVoorkeurAdres != null)
                {
                    query = query.Where(ld => filter.HeeftVoorkeurAdres.Value ?
                        ld.GelieerdePersoon.PersoonsAdres != null :
                        ld.GelieerdePersoon.PersoonsAdres == null);
                }

                if (filter.HeeftTelefoonNummer != null)
                {
                    query = query.Where(ld => filter.HeeftTelefoonNummer.Value
                                                ? ld.GelieerdePersoon.Communicatie.Any(
                                                    cm => cm.CommunicatieType.ID == (int)CommunicatieTypeEnum.TelefoonNummer)
                                                : !ld.GelieerdePersoon.Communicatie.Any(
                                                    cm => cm.CommunicatieType.ID == (int)CommunicatieTypeEnum.TelefoonNummer));
                }

                if (filter.HeeftEmailAdres != null)
                {
                    query = query.Where(ld => filter.HeeftEmailAdres.Value
                                    ? ld.GelieerdePersoon.Communicatie.Any(
                                        cm => cm.CommunicatieType.ID == (int)CommunicatieTypeEnum.Email)
                                    : !ld.GelieerdePersoon.Communicatie.Any(
                                        cm => cm.CommunicatieType.ID == (int)CommunicatieTypeEnum.Email));
                }

                var paths = LedenDao.ExtrasNaarLambdasLeiding(extras);
                resultaat = IncludesToepassen(query as ObjectQuery<Leiding>, paths).ToArray();

                if ((extras & LidExtras.Adressen) != 0)
                {
                    AdresHelper.AlleAdressenKoppelen(from l in resultaat select l.GelieerdePersoon);
                }
                else if ((extras & LidExtras.VoorkeurAdres) != 0)
                {
                    AdresHelper.VoorkeursAdresKoppelen(from l in resultaat select l.GelieerdePersoon);
                }
            }

            return Utility.DetachObjectGraph<Leiding>(resultaat);
        }

        /// <summary>
        /// Bewaart een leid(st)er, inclusief de extras gegeven in <paramref name="extras"/>
        /// </summary>
        /// <param name="entiteit">Te bewaren leid(st)er</param>
        /// <param name="extras">Bepaalt de gekoppelde entiteiten die mee bewaard moeten worden</param>
        /// <returns>Kopie van de bewaarde leid(st)er</returns>
        public Leiding Bewaren(Leiding entiteit, LidExtras extras)
        {
            return Bewaren(entiteit, LedenDao.ExtrasNaarLambdasLeiding(extras));
        }
    }
}
