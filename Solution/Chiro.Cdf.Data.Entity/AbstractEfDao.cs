using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data;
using System.Data.Objects.DataClasses;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

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

		protected Expression<Func<TEntiteit, object>>[] connectedEntities;

		public Dao()
		{
			connectedEntities = new Expression<Func<TEntiteit, object>>[0];
		}

		public virtual Expression<Func<TEntiteit, object>>[] getConnectedEntities()
		{
			return connectedEntities;
		}


		/// <summary>
		/// Ophalen van een entity op basis van ID
		/// </summary>
		/// <param name="id">ID van op te halen object</param>
		/// <returns></returns>
		public virtual TEntiteit Ophalen(int id)
		{
			Expression<Func<TEntiteit, object>>[] paths = getConnectedEntities();
			return Ophalen(id, paths);
		}

		public virtual TEntiteit Ophalen(int id, params Expression<Func<TEntiteit, object>>[] paths)
		{
			TEntiteit result;

			using (TContext db = new TContext())
			{
				// Constructie db.CreateQuery<T>("[" + db.GetEntitySetName(typeof(T)) + "]").OfType<T>()
				// zorgt ervoor dat dit ook werkt voor overervende types.  (In dat geval is
				// de EntitySetName niet gelijk aan de naam van het type.)

				ObjectQuery<TEntiteit> query = (from t in db.CreateQuery<TEntiteit>("[" + db.GetEntitySetName(typeof(TEntiteit)) + "]").OfType<TEntiteit>()
								where t.ID == id
								select t) as ObjectQuery<TEntiteit>;

				// query.MergeOption = MergeOption.NoTracking;
				// Je zou denken dat bovenstaande lijn ervoor zorgt dat de opgehaalde
				// objecten gedetacht zijn.  Maar dat is niet het geval.  (??) Vandaar
				// dat ik verderop toch DetachObjectGraph gebruik.

				result = (IncludesToepassen(query, paths)).FirstOrDefault<TEntiteit>();

			}

			if (result != null)
			{
				result = Utility.DetachObjectGraph(result);
			}


			return result;
		}

		/// <summary>
		/// 'Includet' gerelateerde entiteiten aan een objectquery.
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

				foreach (var v in paths)
				{
					resultaat = resultaat.Include(v);
				}
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
			using (TContext db = new TContext())
			{
				// Constructie met OfType zodat overerving ook werkt.
				ObjectQuery<TEntiteit> oq = db.CreateQuery<TEntiteit>("[" + db.GetEntitySetName(typeof(TEntiteit)) + "]").OfType<TEntiteit>();
				oq.MergeOption = MergeOption.NoTracking;
				result = oq.ToList<TEntiteit>();
			}
			return result;
		}

		/// <summary>
		/// Bewaart/Updatet entiteit in database
		/// </summary>
		/// <param name="nieuweEntiteit">entiteit met nieuwe gegevens</param>
		/// <returns>De geupdatete entiteit</returns>
		/// <remarks>Deze functie mag ook gebruikt worden voor het toevoegen
		/// van een nieuwe entiteit, of het verwijderen van een bestaande.
		/// In dat laatste geval is de terugkeerwaarde null.</remarks>
		public virtual TEntiteit Bewaren(TEntiteit entiteit)
		{
			return Bewaren(entiteit, getConnectedEntities());
		}

		/// <summary>
		/// Bewaart/Updatet entiteit in database
		/// </summary>
		/// <param name="nieuweEntiteit">entiteit met nieuwe gegevens</param>
		/// <returns>De geupdatete entiteit</returns>
		/// <remarks>Nieuwe methode, die altijd attachobject graph gebruikt en dus als argument een array van lambda
		/// expressions meekrijgt</remarks>
		public virtual TEntiteit Bewaren(TEntiteit entiteit, params Expression<Func<TEntiteit, object>>[] paths)
		{
			using (TContext db = new TContext())
			{
				entiteit = db.AttachObjectGraph(entiteit, paths);

				// SetAllModified is niet meer nodig na AttachObjectGraph

				db.SaveChanges();

			}

			// Door Utility.DetachObjectGraph te gebruiken wanneer de context
			// niet meer bestaat, is het resultaat *echt* gedetacht.

			entiteit = Utility.DetachObjectGraph(entiteit);
			return entiteit;
		}


		/// <summary>
		/// Bewaren van een lijst entiteiten
		/// </summary>
		/// <param name="es">lijst entiteiten</param>
		/// <param name="paths">paden met gerelateerde te bewaren
		/// entiteiten</param>
		/// <returns>De bewaarde entiteiten, maar gedetacht</returns>
		public virtual IEnumerable<TEntiteit> Bewaren(IEnumerable<TEntiteit> es, params Expression<Func<TEntiteit, object>>[] paths)
		{
			using (TContext db = new TContext())
			{
				db.AttachObjectGraphs(es);
				db.SaveChanges();
			}

			es = Utility.DetachObjectGraph(es);
			return es;
		}


		/// <summary>
		/// Markeert entity als 'volledig gewijzigd'
		/// </summary>
		/// <param name="key">Key van te markeren entity</param>
		/// <param name="context">Context om wijzigingen in te markeren</param>
		/// <remarks>Deze functie staat hier mogelijk niet op zijn plaats</remarks>
		protected static void SetAllModified(EntityKey key, ObjectContext context)
		{
			var stateEntry = context.ObjectStateManager.GetObjectStateEntry(key);
			var propertyNameList = stateEntry.CurrentValues.DataRecordInfo.FieldMetadata.Select
			  (pn => pn.FieldType.Name);
			foreach (var propName in propertyNameList)
			{
				try
				{
					stateEntry.SetModifiedProperty(propName);
				}
				catch (InvalidOperationException)
				{
					// WTF???
				}

			}
		}

		#endregion


	}


}
