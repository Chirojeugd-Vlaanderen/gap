// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;      // NIET VERWIJDEREN, nodig voor live deploy!
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

        #endregion categorieën

        /// <summary>
        /// Maakt een nieuwe (groepseigen) functie voor groep <paramref name="g"/>.  Persisteert niet.
        /// </summary>
        /// <param name="g">
        /// Groep waarvoor de functie gemaakt wordt, inclusief minstens het recentste werkJaar
        /// </param>
        /// <param name="naam">
        /// Naam van de functie
        /// </param>
        /// <param name="code">
        /// Code van de functie
        /// </param>
        /// <param name="maxAantal">
        /// Maximumaantal leden in de categorie.  Onbeperkt indien null.
        /// </param>
        /// <param name="minAantal">
        /// Minimumaantal leden in de categorie.
        /// </param>
        /// <param name="lidType">
        /// LidType waarvoor de functie van toepassing is
        /// </param>
        /// <returns>
        /// De nieuwe (gekoppelde) functie
        /// </returns>
        public Functie FunctieToevoegen(
            Groep g, 
            string naam, 
            string code, 
            int? maxAantal, 
            int minAantal, 
            LidType lidType)
        {
            if (!_autorisatieMgr.IsGavGroep(g.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            // Controleer op dubbele code
            var bestaande = (from fun in g.Functie
                             where string.Compare(fun.Code, code, true) == 0
                                   || string.Compare(fun.Naam, naam, true) == 0
                             select fun).FirstOrDefault();

            if (bestaande != null)
            {
                // TODO (#507): Check op bestaande afdeling door DB
                // OPM: we krijgen pas een DubbeleEntiteitException op het moment dat we bewaren,
                // maar hier doen we alleen een .Add
                throw new BestaatAlException<Functie>(bestaande);
            }

            // Zonder problemen hier geraakt.  Dan kunnen we verder.
            Niveau niveau = g.Niveau;
            if ((g.Niveau & Niveau.Groep) != 0)
            {
                if ((lidType & LidType.Leiding) == 0)
                {
                    niveau &= ~Niveau.LeidingInGroep;
                }

                if ((lidType & LidType.Kind) == 0)
                {
                    niveau &= ~Niveau.LidInGroep;
                }
            }

            var f = new Functie
                        {
                            Code = code, 
                            Groep = g, 
                            MaxAantal = maxAantal, 
                            MinAantal = minAantal, 
                            Niveau = niveau, 
                            Naam = naam, 
                            WerkJaarTot = null, 
                            WerkJaarVan = g.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).First().WerkJaar, 
                            IsNationaal = false
                        };

            g.Functie.Add(f);

            return f;
        }
    }
}
