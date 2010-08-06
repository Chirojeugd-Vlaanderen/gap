// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;

namespace Chiro.Cdf.Data.Entity
{
	/// <summary>
	/// Workaround voor 'where in' in linq to EF
	/// </summary>
	public static class Utility
	{
		/// <summary>
		/// Deze nifty functie creëert een expressie die in het
		/// 'where'-gedeelte van een linq-expressie gebruikt kan
		/// worden.  De bedoeling is om een 'where-in'-constructie
		/// te simuleren.
		/// </summary>
		/// <typeparam name="TElement">Klasse van het object dat 'gewhereind' moet worden</typeparam>
		/// <typeparam name="TValue">Type van het veld waarop 'gewhereind' moet worden</typeparam>
		/// <param name="valueSelector">Lambda-expressie die het bovenvermelde veld selecteert uit een TElement</param>
		/// <param name="values">Collectie waarin het vermelde veld gezocht moet worden</param>
		/// <returns>Expressie die in de 'where' van een linq-expressie gebruikt kan worden</returns>
		public static Expression<Func<TElement, bool>> BuildContainsExpression<TElement, TValue>(Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
		{
			if (valueSelector == null)
			{
				throw new ArgumentNullException("valueSelector");
			}
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}

			ParameterExpression p = valueSelector.Parameters.Single();

			if (!values.Any())
			{
				return e => false;
			}

			var equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
			var body = equals.Aggregate(Expression.Or);

			return Expression.Lambda<Func<TElement, bool>>(body, p);
		}

		/// <summary>
		/// Detaches an objectgraph given it's root object.
		/// </summary>
		/// <param name="entity"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>The detached root object.</returns>
		/// <remarks>
		///     1. Enkel als de oorspronkelijke context niet meer bestaat,
		///        zullen de entity's daadwerkelijk EntityState=Detached
		///        hebben.
		///     2. Als er circulaire relaties voorkomen, dan werkt het
		///        niet.
		///     3. Blijkbaar ook niet voor many-to-many?
		/// </remarks>
		public static T DetachObjectGraph<T>(T entity) where T : IEfBasisEntiteit
		{
			T gedetacht;

			using (var stream = new MemoryStream())
			{
				// NetDataContractSerializer serializer = new NetDataContractSerializer();

				// Met een DataContractSerializer werkt het blijkbaar niet.
				// Met een binaryFormatter wel.

				var serializer = new BinaryFormatter();

				serializer.Serialize(stream, entity);
				stream.Position = 0;

				gedetacht = (T)serializer.Deserialize(stream);
			}

			return gedetacht;
		}

		public static IEnumerable<T> DetachObjectGraph<T>(IEnumerable<T> entities) where T : IEfBasisEntiteit
		{
			using (var stream = new MemoryStream())
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, entities);
				stream.Position = 0;
				return (IEnumerable<T>)serializer.Deserialize(stream);
			}
		}

		public static IList<T> DetachObjectGraph<T>(IList<T> entities) where T : IEfBasisEntiteit
		{
			using (var stream = new MemoryStream())
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, entities);
				stream.Position = 0;
				return (IList<T>)serializer.Deserialize(stream);
			}
		}
	}
}
