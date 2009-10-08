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
using System.IO;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Cg2.EfWrapper
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

        /// <summary>
        /// Detaches an objectgraph given it's root object.
        /// </summary>
        /// <returns>The detached root object.</returns>
        /// <remarks>
        ///     1. Enkel als de oorspronkelijke context niet meer bestaat,
        ///        zullen de entity's daadwerkelijk EntityState=Detached
        ///        hebben.
        ///     2. Als er circulaire relaties voorkomen, dan werkt het
        ///        niet.
        ///     3. Blijkbaar ook niet voor many-to-many?
        /// </remarks>
        public static T DetachObjectGraph<T>(T entity) where T:IBasisEntiteit
        {
            T gedetacht;

            using (MemoryStream stream = new MemoryStream())
            {

                NetDataContractSerializer serializer = new NetDataContractSerializer();
                serializer.Serialize(stream, entity);
                stream.Position = 0;

                #region opkuisen
                // TODO: opkuis
                var tr = new StreamReader(stream);
                string xml = tr.ReadToEnd();

                Debug.WriteLine(xml);


                stream.Position = 0;
                #endregion


                gedetacht = (T)serializer.Deserialize(stream);
            }

            return gedetacht;
        }
        
        public static IList<T> DetachObjectGraph<T>(IList<T> entities) where T : IBasisEntiteit
        {
            using (MemoryStream stream = new MemoryStream())
            {

                NetDataContractSerializer serializer = new NetDataContractSerializer();
                serializer.Serialize(stream, entities);
                stream.Position = 0;
                return (IList<T>)serializer.Deserialize(stream);
            }
        }
    }
}
