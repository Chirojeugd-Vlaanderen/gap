// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Chiro.Cdf.Data.Entity
{
	/// <summary>
	/// Algemene implementatie van IDao; generieke CRUD-operaties voor
	/// een DAO-object.
	/// </summary>
	/// <typeparam name="TEntiteit">Type van entiteit waarvoor deze DAO een repository is</typeparam>
	/// <typeparam name="TContext">Type van de objectcontext die de entiteiten zal ophalen/bewaren</typeparam>
	/// <remarks>Alle entity's die de DAO oplevert, moeten gedetacht zijn.
	/// Dit kan via MergeOption.NoTracking, of via Utility.DetachObjectGraph.
	/// </remarks>
	public class Dao<TEntiteit, TContext> : IDao<TEntiteit>
		where TEntiteit : EntityObject, IEfBasisEntiteit
		where TContext : ObjectContext, new()
	{
		#region IDao<T> Members

		protected Expression<Func<TEntiteit, object>>[] ConnectedEntities;

		public Dao()
		{
			ConnectedEntities = new Expression<Func<TEntiteit, object>>[0];
		}

		public virtual Expression<Func<TEntiteit, object>>[] GetConnectedEntities()
		{
			return ConnectedEntities;
		}

		/// <summary>
		/// Ophalen van een entity op basis van ID
		/// </summary>
		/// <param name="id">ID van op te halen object</param>
		/// <returns></returns>
		public virtual TEntiteit Ophalen(int id)
		{
			Expression<Func<TEntiteit, object>>[] paths = GetConnectedEntities();
			return Ophalen(id, paths);
		}

		/// <summary>
		/// Ophalen van een aantal entiteiten
		/// </summary>
		/// <param name="ids">ID's van op te halen entiteiten</param>
		/// <returns>Lijst opgehaalde entiteiten</returns>
		/// <remarks>Virtueel gemaakt omdat PersonenDao overridet</remarks>
		public virtual IList<TEntiteit> Ophalen(IEnumerable<int> ids)
		{
			return Ophalen(ids, GetConnectedEntities());
		}

		public virtual TEntiteit Ophalen(int id, params Expression<Func<TEntiteit, object>>[] paths)
		{
			TEntiteit result;

			using (var db = new TContext())
			{
				// Constructie db.CreateQuery<T>("[" + db.GetEntitySetName(typeof(T)) + "]").OfType<T>()
				// zorgt ervoor dat dit ook werkt voor overervende types.  (In dat geval is
				// de EntitySetName niet gelijk aan de naam van het type.)

				var query = (from t in db.CreateQuery<TEntiteit>("[" + db.GetEntitySetName(typeof(TEntiteit)) + "]").OfType<TEntiteit>()
												where t.ID == id
												select t) as ObjectQuery<TEntiteit>;

				// query.MergeOption = MergeOption.NoTracking;
				// Je zou denken dat bovenstaande lijn ervoor zorgt dat de opgehaalde
				// objecten gedetacht zijn.  Maar dat is niet het geval.  (??) Vandaar
				// dat ik verderop toch DetachObjectGraph gebruik.

				result = (IncludesToepassen(query, paths)).FirstOrDefault();
			}

			if (result != null)
			{
				result = Utility.DetachObjectGraph(result);
			}

			return result;
		}

		/// <summary>
		/// Haalt een pagina entiteiten op, op basis van een ID van een gerelateerde entiteit
		/// </summary>
		/// <param name="id">Te vergelijken ID</param>
		/// <param name="f">Lambda-expressie die uitgaande van de op te zoeken entiteit de ID bepaalt 
		/// waarmee <paramref name="id"/> vergeleken moet worden.</param>
		/// <param name="pagina">Gevraagde pagina</param>
		/// <param name="paginaGrootte">Aantal entiteiten op de pagina</param>
		/// <param name="aantalTotaal">Out-parameter met totaal aantal entiteiten die voldoen</param>
		/// <param name="paths">Lambda-expressies voor gerelateerde entiteiten</param>
		/// <returns></returns>
		public virtual IList<TEntiteit> PaginaOphalen(int id, Expression<Func<TEntiteit, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<TEntiteit, object>>[] paths)
		{
			IList<TEntiteit> result;

			var list = new List<int>();
			list.Add(id);

			using (var db = new TContext())
			{
				aantalTotaal = (from t in db.CreateQuery<TEntiteit>("[" + db.GetEntitySetName(typeof(TEntiteit)) + "]").OfType<TEntiteit>()
						.Where(Utility.BuildContainsExpression(f, list))
								select t).Count();

				var query =
					(from t in db.CreateQuery<TEntiteit>("[" + db.GetEntitySetName(typeof(TEntiteit)) + "]").OfType<TEntiteit>()
					 select t).OrderBy(basisent => basisent.ID).Skip((pagina - 1) * paginaGrootte).Take(paginaGrootte) as ObjectQuery<TEntiteit>;

				result = (IncludesToepassen(query, paths)).ToList();
			}

			result = Utility.DetachObjectGraph(result);

			return result;
		}

		/// <summary>
		/// Ophalen van een lijst entiteiten met gekoppelde entiteiten
		/// </summary>
		/// <param name="ids">ID's van op te halen entiteiten</param>
		/// <param name="paths">Omschrijft mee op te halen gekoppelde entiteiten</param>
		/// <returns>Een lijst opgehaalde entiteiten</returns>
		public IList<TEntiteit> Ophalen(IEnumerable<int> ids, params Expression<Func<TEntiteit, object>>[] paths)
		{
			IList<TEntiteit> result;

			using (var db = new TContext())
			{
				// onderstaande lukt niet, omdat Contains niet werkt voor LINQ to entities:
				// ObjectQuery<TEntiteit> query = (from t in db.CreateQuery<TEntiteit>("[" + db.GetEntitySetName(typeof(TEntiteit)) + "]").OfType<TEntiteit>()
				//                                where ids.Contains(t.ID)
				//                                select t) as ObjectQuery<TEntiteit>;

				// Constructie db.CreateQuery<T>("[" + db.GetEntitySetName(typeof(T)) + "]").OfType<T>()
				// zorgt ervoor dat dit ook werkt voor overervende types.  (In dat geval is
				// de EntitySetName niet gelijk aan de naam van het type.)

				var query = (from t in db.CreateQuery<TEntiteit>("[" + db.GetEntitySetName(typeof(TEntiteit)) + "]").OfType<TEntiteit>()
								.Where(Utility.BuildContainsExpression<TEntiteit, int>(ent => ent.ID, ids))
												select t) as ObjectQuery<TEntiteit>;

				result = (IncludesToepassen(query, paths)).ToList();
			}

			result = Utility.DetachObjectGraph(result);

			return result;
		}

		/// <summary>
		/// Includet gerelateerde entiteiten in een objectquery
		/// </summary>
		/// <param name="query">Betreffende query</param>
		/// <param name="paths">Paden naar mee op te halen gerelateerde entiteiten</param>
		/// <returns>Nieuwe query die de gevraagde entiteiten Includet</returns>
		protected ObjectQuery<TEntiteit> IncludesToepassen(ObjectQuery<TEntiteit> query, Expression<Func<TEntiteit, object>>[] paths)
		{
			ObjectQuery<TEntiteit> resultaat = query;

			if (paths != null)
			{
				// Workaround om de Includes te laten werken
				// overgenomen van 
				// http://blogs.msdn.com/alexj/archive/2009/06/02/tip-22-how-to-make-include-really-include.aspx
				// en daarna via ReSharper omgezet in een Linq-expressie
				resultaat = paths.Aggregate(resultaat, (current, v) => current.Include(v));
			}

			return resultaat;
		}

		/// <summary>
		/// Alle entity's van het gegeven type ophalen
		/// </summary>
		/// <returns>Een lijst met objecten</returns>
		public virtual IList<TEntiteit> AllesOphalen()
		{
			List<TEntiteit> result;
			using (var db = new TContext())
			{
				// Constructie met OfType zodat overerving ook werkt.
				ObjectQuery<TEntiteit> oq = db.CreateQuery<TEntiteit>("[" + db.GetEntitySetName(typeof(TEntiteit)) + "]").OfType<TEntiteit>();
				oq.MergeOption = MergeOption.NoTracking;
				result = oq.ToList();
			}
			return result;
		}

		/// <summary>
		/// Bewaart/Updatet entiteit in database
		/// </summary>
		/// <param name="entiteit">Entiteit met nieuwe gegevens</param>
		/// <returns>De geüpdatete entiteit</returns>
		/// <remarks>Deze functie mag ook gebruikt worden voor het toevoegen
		/// van een nieuwe entiteit, of het verwijderen van een bestaande.
		/// In dat laatste geval is de terugkeerwaarde null.</remarks>
		public virtual TEntiteit Bewaren(TEntiteit entiteit)
		{
			return Bewaren(entiteit, GetConnectedEntities());
		}

		/// <summary>
		/// Bewaart/Updatet entiteit in database
		/// </summary>
		/// <param name="entiteit">Entiteit met nieuwe gegevens</param>
		/// <param name="paths"></param>
		/// <returns>De geüpdatete entiteit</returns>
		/// <remarks>Nieuwe methode, die altijd attachobject graph gebruikt en dus als argument een array van lambda
		/// expressions meekrijgt</remarks>
		public virtual TEntiteit Bewaren(TEntiteit entiteit, params Expression<Func<TEntiteit, object>>[] paths)
		{
			var resultaat = Bewaren(new[] { entiteit }, paths);
			return (resultaat.Count() == 1) ? resultaat.First() : null;
		}

		/// <summary>
		/// Bewaren van een lijst entiteiten
		/// </summary>
		/// <param name="es">Lijst entiteiten</param>
		/// <param name="paths">Paden met gerelateerde te bewaren
		/// entiteiten</param>
		/// <returns>De bewaarde entiteiten, maar gedetacht</returns>
		public virtual IEnumerable<TEntiteit> Bewaren(IEnumerable<TEntiteit> es, params Expression<Func<TEntiteit, object>>[] paths)
		{
			using (var db = new TContext())
			{
				es = db.AttachObjectGraphs(es, paths);
				// Geattachte graph toekennen aan es, zodat we achteraf de goeie detachen.

				try
				{
					// Unit tests crashen hier als DTC niet geënabled is.
					// Zie https://develop.chiro.be/trac/cg2/wiki/DistributedTransactions
					db.SaveChanges();
				}
				catch (UpdateException ex)
				{
					var inner = ex.InnerException as SqlException;

					if (inner != null && (inner.Number == 2601 || inner.Number == 2627))
					{
						// We weten op dit moment jammer genoeg niet welke entiteit er nu precies
						// problemen geeft.
						throw new DubbeleEntiteitException<TEntiteit>();
					}
					throw;
				}	
			}

			es = Utility.DetachObjectGraph(es);

			// Als er 'root'-entiteiten te verwijderen waren, dan halen we die ook uit de lijst

			return (from entiteit in es
					where entiteit.TeVerwijderen == false
					select entiteit).ToArray();
		}
		#endregion
	}
}
