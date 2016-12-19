/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp
{
    /// <summary>
    /// Extension methods om onze eigen sortering makkelijk te kunnen gebruiken.
    /// </summary>
    public static class GapSortering
    {
        #region private hulpdingen
        /// <summary>
        /// Levert de code van een van de categorieen van de persoon met gegeven <paramref name="detail"/>
        /// op, of <c>null</c> als er geen categorieen in <paramref name="detail"/> zijn.
        /// </summary>
        /// <param name="detail">Details van een persoon</param>
        /// <returns>
        /// De code van een van de categorieen van de persoon met gegeven <paramref name="detail"/>m
        /// of <c>null</c> als er geen categorieen in <paramref name="detail"/> zijn.
        /// </returns>
        private static string WillkeurigeCategorieCodeGet(PersoonDetail detail)
        {
            if (detail.CategorieLijst == null || detail.CategorieLijst.FirstOrDefault() == null)
            {
                return null;
            }
            return detail.CategorieLijst.First().Code;
        }

        /// <summary>
        /// Levert de code van een van de categorieen van de persoon met gegeven <paramref name="info"/>
        /// op, of <c>null</c> als er geen categorieen in <paramref name="info"/> zijn.
        /// </summary>
        /// <param name="info">Details van een persoon</param>
        /// <returns>
        /// De code van een van de categorieen van de persoon met gegeven <paramref name="info"/>m
        /// of <c>null</c> als er geen categorieen in <paramref name="info"/> zijn.
        /// </returns>
        private static string WillkeurigeCategorieCodeGet(PersoonLidInfo info)
        {
            if (info.PersoonDetail.CategorieLijst == null || info.PersoonDetail.CategorieLijst.FirstOrDefault() == null)
            {
                return null;
            }
            return info.PersoonDetail.CategorieLijst.First().Code;
        }
        #endregion

        /// <summary>
        /// Sorteert de <paramref name="details"/> volgens de gegeven <paramref name="sortering"/>
        /// </summary>
        /// <param name="details">details van personen</param>
        /// <param name="sortering">bepaalt de manier waarop de <paramref name="details gesorteerd moeten worden"/></param>
        /// <returns>De <paramref name="details"/>, gesorteerd volgens <paramref name="sortering"/></returns>
        public static IQueryable<PersoonDetail> Sorteren(this IQueryable<PersoonDetail> details,
                                                         PersoonSorteringsEnum sortering)
        {
            switch (sortering)
            {
                case PersoonSorteringsEnum.Categorie:
                    {
                        return details.OrderBy(det => WillkeurigeCategorieCodeGet(det));
                    }
                case PersoonSorteringsEnum.Leeftijd:
                    {
                        return details.OrderByDescending(dst => dst.GeboorteDatum);
                    }
                case PersoonSorteringsEnum.Naam:
                    {
                        return details.OrderBy(dst => dst.Naam).ThenBy(dst => dst.VoorNaam);
                    }
                default:
                    {
                        return details;
                    }
            }
        }

        /// <summary>
        /// Sorteert de <paramref name="info"/> volgens de gegeven <paramref name="sortering"/>
        /// </summary>
        /// <param name="info">details van personen</param>
        /// <param name="sortering">bepaalt de manier waarop de <paramref name="info gesorteerd moeten worden"/></param>
        /// <returns>De <paramref name="info"/>, gesorteerd volgens <paramref name="sortering"/></returns>
        public static IQueryable<PersoonLidInfo> Sorteren(this IQueryable<PersoonLidInfo> info,
                                                         PersoonSorteringsEnum sortering)
        {
            switch (sortering)
            {
                case PersoonSorteringsEnum.Categorie:
                    {
                        return info.OrderBy(det => WillkeurigeCategorieCodeGet(det));
                    }
                case PersoonSorteringsEnum.Leeftijd:
                    {
                        return info.OrderByDescending(dst => dst.PersoonDetail.GeboorteDatum);
                    }
                case PersoonSorteringsEnum.Naam:
                    {
                        return info.OrderBy(dst => dst.PersoonDetail.Naam).ThenBy(dst => dst.PersoonDetail.VoorNaam);
                    }
                default:
                    {
                        return info;
                    }
            }
        }

        /// <summary>
        /// Sorteert de <paramref name="details"/> volgens de gegeven <paramref name="sortering"/>
        /// </summary>
        /// <param name="details">details van personen</param>
        /// <param name="sortering">bepaalt de manier waarop de <paramref name="details gesorteerd moeten worden"/></param>
        /// <returns>De <paramref name="details"/>, gesorteerd volgens <paramref name="sortering"/></returns>
        public static IQueryable<PersoonOverzicht> Sorteren(this IQueryable<PersoonOverzicht> details,
                                                         PersoonSorteringsEnum sortering)
        {
            switch (sortering)
            {
                case PersoonSorteringsEnum.Categorie:
                    {
                        throw new NotSupportedException();
                    }
                case PersoonSorteringsEnum.Leeftijd:
                    {
                        return details.OrderByDescending(dst => dst.GeboorteDatum);
                    }
                case PersoonSorteringsEnum.Naam:
                    {
                        return details.OrderBy(dst => dst.Naam).ThenBy(dst => dst.VoorNaam);
                    }
                default:
                    {
                        return details;
                    }
            }
        }

        /// <summary>
        /// Sorteert de <paramref name="details"/> volgens de gegeven <paramref name="sortering"/>
        /// </summary>
        /// <param name="details">details van personen</param>
        /// <param name="sortering">bepaalt de manier waarop de <paramref name="details gesorteerd moeten worden"/></param>
        /// <returns>De <paramref name="details"/>, gesorteerd volgens <paramref name="sortering"/></returns>
        public static List<PersoonDetail> Sorteren(this IList<PersoonDetail> details, PersoonSorteringsEnum sortering)
        {
            return details.AsQueryable().Sorteren(sortering).ToList();
        }

        /// <summary>
        /// Sorteert de <paramref name="details"/> volgens de gegeven <paramref name="sortering"/>
        /// </summary>
        /// <param name="details">details van personen</param>
        /// <param name="sortering">bepaalt de manier waarop de <paramref name="details gesorteerd moeten worden"/></param>
        /// <returns>De <paramref name="details"/>, gesorteerd volgens <paramref name="sortering"/></returns>
        public static List<PersoonOverzicht> Sorteren(this IList<PersoonOverzicht> details, PersoonSorteringsEnum sortering)
        {
            return details.AsQueryable().Sorteren(sortering).ToList();
        }
    }
}