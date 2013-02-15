using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        /// Sorteert de <paramref name="details"/> volgens de gegeven <paramref name="sortering"/>
        /// </summary>
        /// <param name="details">details van personen</param>
        /// <param name="sortering">bepaalt de manier waarop de <paramref name="details gesorteerd moeten worden"/></param>
        /// <returns>De <paramref name="details"/>, gesorteerd volgens <paramref name="sortering"/></returns>
        public static List<PersoonDetail> Sorteren(this IList<PersoonDetail> details, PersoonSorteringsEnum sortering)
        {
            return details.AsQueryable().Sorteren(sortering).ToList();
        }
    }
}