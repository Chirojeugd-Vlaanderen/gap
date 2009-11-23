using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Chiro.Cdf.Data
{
	/// <summary>
	/// Algemeen contract voor CRUD-operaties in de ORM-laag.  Implementeert een 'Repository'.
	/// 
	/// Aangepast uit 'Pro LINQ Object Relational Mapping with C# 2008'.
	/// </summary>
	/// <typeparam name="T">Entiteit die 'geDaot' moet worden</typeparam>
	public interface IDao<T>
	{
		/// <summary>
		/// Haalt entiteit op, op basis van <paramref name="id" />
		/// </summary>
		/// <param name="id">ID op te halen entiteit</param>
		/// <returns>Entiteit, op basis van <paramref name="id" /></returns>
		T Ophalen(int id);

		/// <summary>
		/// Haalt entiteit op met gekoppelde entiteiten
		/// </summary>
		/// <param name="id">ID op te halen entiteit</param>
		/// <param name="paths">lambda-expressies die aangeven welke gekoppelde entiteiten mee opgehaald 
		/// moeten worden</param>
		/// <returns>De gevraagde entiteit, met de gekoppelde entiteiten gespecifieerd in
		/// <paramref name="paths"/></returns>
		T Ophalen(int id, params Expression<Func<T, object>>[] paths);

        /// <summary>
        /// Haalt een lijst entiteiten op, met gegeven <paramref name="ids"/>
        /// </summary>
        /// <param name="ids">rij ID's op te halen entiteiten</param>
        /// <returns>Lijst opgehaalde entiteiten</returns>
        IList<T> Ophalen(IEnumerable<int> ids);

        /// <summary>
        /// Haalt een lijst entiteiten op, met gekoppelde entiteiten
        /// </summary>
        /// <param name="ids">ID's van op te halen entiteiten</param>
        /// <param name="paths">geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden</param>
        /// <returns>Lijst met opgehaalde ID's</returns>
        IList<T> Ophalen(IEnumerable<int> ids, params Expression<Func<T, object>>[] paths);

		/// <summary>
		/// Haalt alle entiteiten van het type <typeparamref name="T"/> op.
		/// </summary>
		/// <returns>Alle entiteiten van het type <typeparamref name="T"/></returns>
		IList<T> AllesOphalen();

		/// <summary>
		/// Bewaart of updatet de gegeven <paramref name="entiteit"/>
		/// </summary>
		/// <param name="entiteit">te bewaren entiteit</param>
		/// <returns>Een kloon van de bewaarde entiteit, met juiste ID's</returns>
		T Bewaren(T entiteit);

		/// <summary>
		/// Bewaart of updatet de <paramref name="entiteit"/> en gekoppelde andere entiteiten
		/// </summary>
		/// <param name="entiteit">Te bewaren entiteit</param>
		/// <param name="paths">lambda-expressie die bepaalt welke andere entiteiten mee bewaard moeten worden.</param>
		/// <returns>Een kloon van de bewaarde entiteit, met de juiste ID's</returns>
		T Bewaren(T entiteit, params Expression<Func<T, object>>[] paths);

		/// <summary>
		/// Bewaart of updatet de entiteiten <paramref name="es"/>, inclusief gekoppelde andere entiteiten
		/// </summary>
		/// <param name="es">Te bewaren entiteiten</param>
		/// <param name="paths">lambda-expressie de mee te bewaren gekoppelde entiteiten bepaalt</param>
		/// <returns>Een kloon van <paramref name="es"/> met de juiste ID's</returns>
		IEnumerable<T> Bewaren(IEnumerable<T> es, params Expression<Func<T, object>>[] paths);

		/// <summary>
		/// Opvragen van de standaard meegeleverde gekoppelde entiteiten
		/// </summary>
		/// <returns>Lambda-expressies die bepalen welke gekoppelde entiteiten er standaard mee bewaard
		/// of opgevraagd worden.</returns>
		Expression<Func<T, object>>[] getConnectedEntities();
	}

}
