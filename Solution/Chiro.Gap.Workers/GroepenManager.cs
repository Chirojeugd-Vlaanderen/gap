// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;      // NIET VERWIJDEREN, nodig voor live deploy!
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. groepen bevat (dat is breder dan 'Chirogroepen', bv. satellieten)
    /// </summary>
    public class GroepenManager : IGroepenManager
    {
        private readonly IVeelGebruikt _veelGebruikt;
        private readonly IAutorisatieManager _autorisatieMgr;

        public GroepenManager(
            IVeelGebruikt veelGebruikt, 
            IAutorisatieManager autorisatieMgr)
        {
            _autorisatieMgr = autorisatieMgr;
            _veelGebruikt = veelGebruikt;
        }


        #region categorieën

        /// <summary>
        /// Maakt een nieuwe categorie, en koppelt die aan een bestaande groep (met daaraan
        /// gekoppeld zijn categorieën)
        /// </summary>
        /// <param name="g">
        /// Groep waarvoor de categorie gemaakt wordt.  Als bestaande categorieën
        /// gekoppeld zijn, wordt op dubbels gecontroleerd
        /// </param>
        /// <param name="categorieNaam">
        /// Naam voor de nieuwe categorie
        /// </param>
        /// <param name="categorieCode">
        /// Code voor de nieuwe categorie
        /// </param>
        /// <returns>
        /// De toegevoegde categorie
        /// </returns>
        public Categorie CategorieToevoegen(Groep g, string categorieNaam, string categorieCode)
        {
            if (!_autorisatieMgr.IsGavGroep(g.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }
            else
            {
                // Is er al een categorie met die code?
                Categorie bestaande = (from ctg in g.Categorie
                                       where string.Compare(ctg.Code, categorieCode, true) == 0
                                             || string.Compare(ctg.Naam, categorieNaam, true) == 0
                                       select ctg).FirstOrDefault();

                if (bestaande != null)
                {
                    // TODO (#507): Check op bestaande afdeling door DB
                    // OPM: we krijgen pas een DubbeleEntiteitException op het moment dat we bewaren,
                    // maar hier doen we alleen een .Add
                    throw new BestaatAlException<Categorie>(bestaande);
                }
                else
                {
                    var c = new Categorie();
                    c.Naam = categorieNaam;
                    c.Code = categorieCode;
                    c.Groep = g;
                    g.Categorie.Add(c);
                    return c;
                }
            }
        }

        /// <summary>
        /// Zoekt in de eigen functies de gegeven <paramref name="groep"/> en in de nationale functies een
        /// functie met gegeven <paramref name="code"/>.
        /// </summary>
        /// <param name="groep">Groep waarvoor functie gezocht moet worden</param>
        /// <param name="code">Code van de te zoeken functie</param>
        /// <param name="functieRepo">Repository die gebruikt kan worden om functies in op te zoeken</param>
        /// <remarks>De repository wordt bewust niet via de constructor meegeleverd, om te vermijden dat de
        /// IOC-container een nieuwe context zou aanmaken.</remarks>
        public Functie FunctieZoeken(Groep groep, string code, IRepository<Functie> functieRepo)
        {
            // Zoek eerst naar een nationale functie met gegeven code, want dat is
            // gecachet, en bijgevolg snel.

            var bestaandeNationaleFunctie = (from f in _veelGebruikt.NationaleFunctiesOphalen(functieRepo)
                                             where String.Compare(f.Code, code, StringComparison.OrdinalIgnoreCase) == 0
                                             select f).FirstOrDefault();

            if (bestaandeNationaleFunctie != null)
            {
                return bestaandeNationaleFunctie;
            }

            // Niet gevonden: zoek nog eens in eigen functies

            return (from f in groep.Functie
                    where String.Compare(f.Code, code, StringComparison.OrdinalIgnoreCase) == 0
                    select f).FirstOrDefault();
        }

        /// <summary>
        /// Converteert een <paramref name="lidType"/> naar een niveau, gegeven het niveau van de
        /// groep (<paramref name="groepsNiveau"/>)
        /// </summary>
        /// <param name="lidType">Leden, Leiding of allebei</param>
        /// <param name="groepsNiveau">Plaatselijke groep, gewestploeg, verbondsploeg, satelliet</param>
        /// <returns>Niveau van het <paramref name="lidType"/> voor een groep met gegeven <paramref name="groepsNiveau"/></returns>
        public Niveau LidTypeNaarMiveau(LidType lidType, Niveau groepsNiveau)
        {
            if ((groepsNiveau & Niveau.Groep) == 0)
            {
                // Geen plaatselijke groep? groepsNiveau is wat we zoeken.
                return groepsNiveau;
            }

            // Voor een plaastelijke groep is er een onderscheid lid/leiding.

            switch (lidType)
            {
                case LidType.Kind:
                    return Niveau.LidInGroep;
                case LidType.Leiding:
                    return Niveau.LeidingInGroep;
                default:
                    return Niveau.Groep;
            }
        }

        /// <summary>
        /// Bepaalt het recentste groepswerkjaar van de gegeven <paramref name="groep"/>
        /// </summary>
        /// <param name="groep">De groep waarvoor het recentste werkjaar gevraagd is</param>
        /// <returns>Recente groepswerkjaar van de groep</returns>
        public GroepsWerkJaar RecentsteWerkJaar(Groep groep)
        {
            return (from wj in groep.GroepsWerkJaar
                    orderby wj.WerkJaar
                    select wj).LastOrDefault();
        }

        #endregion categorieën
    }
}
