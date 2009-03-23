// BuildContainsExpression.cs - workaround voor 'where in' in linq to EF
//
// Met dank aan http://www.velocityreviews.com/forums/showpost.php?p=3683755&postcount=6
//
// Linq to entities biedt geen ondersteuning voor bijv.
//
// List ids = new List();
// ids.Insert(1); ids.Insert(2);
// var res = from Persoon p in context.Persoon
//           where ids.Contains(p.ID)
//           select p
//
// Dankzij onderstaande hack, kan gelijkaardige functionaliteit
// als volgt verkregen worden:
//
// var res = context.Persoon
//              .Where(BuildContainsExpression<Persoon, int>(pers => pers.ID, ids))

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Cg2.Orm
{
    public static class Utility
    {
        /// <summary>
        /// Deze nifty functie creert een expressie die in het
        /// 'where' gedeelte van een linqexpressie gebruikt kan
        /// worden.  De bedoeling is om een 'where-in' constructie
        /// te simuleren.
        /// </summary>
        /// <typeparam name="TElement">klasse van het object dat 'gewhereind' moet worden</typeparam>
        /// <typeparam name="TValue">type van het veld waarop 'gewhereind' moet worden</typeparam>
        /// <param name="valueSelector">lambda-expressie die het bovenvermelde veld selecteert uit een TElement</param>
        /// <param name="values">collectie waarin het vermelde veld gezocht moet worden</param>
        /// <returns>experssie die in de 'where' van een linkexpressie gebruikt kan worden</returns>
        public static Expression<Func<TElement, bool>> BuildContainsExpression<TElement, TValue>(Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (valueSelector == null) throw new ArgumentNullException("valueSelector");
            if (values == null) throw new ArgumentNullException("values");

            ParameterExpression p = valueSelector.Parameters.Single();

            if (!values.Any())
            {
                return e => false;
            }

            var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }
    }
}
