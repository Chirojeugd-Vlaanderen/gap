using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;

namespace Chiro.Gap.Dummies
{
	public class DummyDao<T> : IDao<T> where T : IEfBasisEntiteit
	{
		private static int _newID = 10000;	// hoog genoeg, om dubbelzinnigheid met gepredefinieerde
											// ID's te vermijden

		#region IDao<T> Members

		public T Ophalen(int id)
		{
			throw new NotImplementedException();
		}

		public T Ophalen(int id, params System.Linq.Expressions.Expression<Func<T, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<T> PaginaOphalen(int id, Expression<Func<T, int>> f, int pagina, int paginaGrootte, out int aantalTotaal, params Expression<Func<T, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		public IList<T> AllesOphalen()
		{
			throw new NotImplementedException();
		}

		public T Bewaren(T entiteit)
		{
			return Bewaren(entiteit, getConnectedEntities());
		}

		public T Bewaren(T entiteit, params System.Linq.Expressions.Expression<Func<T, object>>[] paths)
		{
			var resultaat = Bewaren(new T[] { entiteit }, paths);
			return (resultaat.Count() == 1) ? resultaat.First() : default(T);
		}

		/// <summary>
		/// Dummy voor bewaren.
		/// In de terugkeerwaarde zullen alle ID's die 0 waren vervangen zijn door een positief getal,
		/// en alle te verwijderen entiteiten verwijderd zijn
		/// </summary>
		/// <param name="es">Entiteiten die zogezegd gepersisteerd moeten worden</param>
		/// <param name="paths">Gerelateerde entiteiten om te persisteren</param>
		/// <returns>De niet-verwijderde entiteiten uit <paramref name="efs"/>, 0-ID's zijn vervangen
		/// door 'echte' ID's, en zonder te verwijderen entiteiten nodes in de graaf.</returns>
		public IEnumerable<T> Bewaren(IEnumerable<T> es, params System.Linq.Expressions.Expression<Func<T, object>>[] paths)
		{
			var resultaat = new List<T>();

			// wat zwarte magie om een soort van boom te krijgen op basis van de lambda-expressies
			// in 'paths' .
			var propertiesBoom = new TreeNode<ExtendedPropertyInfo>(null);
			foreach (var path in paths)
			{
				var members = new List<ExtendedPropertyInfo>();
				EntityFrameworkHelper.CollectRelationalMembers(path, members);
				propertiesBoom.AddPath(members);
			}

			foreach (T entiteit in es)
			{
				if (!entiteit.TeVerwijderen)
				{
					if (entiteit.ID == 0)
					{
						entiteit.ID = ++_newID;
					}

					Bewaren(entiteit, propertiesBoom);

					resultaat.Add(entiteit);
				}
			}
			return resultaat;
		}


		/// <summary>
		/// Dummy voor bewaren.
		/// Vervangt ID's die 0 zijn door een positief getal, en verwijdert entiteiten die te verwijderen
		/// gemarkeerd zijn.
		/// </summary>
		/// <param name="entiteit">Te 'bewaren' entiteit</param>
		/// <param name="propertiesBoom">Louche boomstructuur die aangeeft welke paden in de entitygraph
		/// bewaard moeten worden.</param>
		private void Bewaren(IEfBasisEntiteit entiteit, TreeNode<ExtendedPropertyInfo> propertiesBoom)
		{
			foreach (var kindNode in propertiesBoom.Children)
			{
				var property = kindNode.Item;
				var related = property.PropertyInfo.GetValue(entiteit, null);

				// related bevat nu de waarde van de property 'property' van de entiteit entiteit

				if (related is IEnumerable)
				{
					var teVerwijderen = new List<IEfBasisEntiteit>();

					// property bevat meerdere items (IEnumerable)
					foreach (var relatedInstance in (IEnumerable)related)
					{
						var be = relatedInstance as IEfBasisEntiteit;

						Debug.Assert(be != null);

						if (!be.TeVerwijderen)
						{
							if (be.ID == 0)
							{
								be.ID = (++_newID);
							}
						}
						else
						{
							teVerwijderen.Add(be);
						}
						Bewaren(be, kindNode);
					}

					foreach (var be in teVerwijderen)
					{
						// Als een entiteit te verwijderen kan zijn, moet de collectie
						// maar een method 'Remove' hebben.

						((object)related).PublicInvokeMethod("Remove", be);
					}
				}
				else if (!typeof(IEnumerable).IsAssignableFrom(property.PropertyInfo.PropertyType))
				{
					// property is 'atomisch'

					var relatedInstance = related as IEfBasisEntiteit;

					Debug.Assert(relatedInstance != null);

					if (!relatedInstance.TeVerwijderen)
					{
						if (relatedInstance.ID == 0)
						{
							relatedInstance.ID = (++_newID);
						}
					}
					else
					{
						property.PropertyInfo.SetValue(entiteit, null, null);
					}
					Bewaren(relatedInstance, kindNode);
				}
				else
				{
					// Niet verwacht.

					Debug.Assert(false);
				}
			}
		}

		/// <summary>
		/// Standaard geen 'connected entities'.
		/// (Ik ben precies nog altijd zo geen voorstander voor diie 'connected entities').
		/// </summary>
		/// <returns>Een lege array van connected entitiesa</returns>
		public System.Linq.Expressions.Expression<Func<T, object>>[] getConnectedEntities()
		{
			return new System.Linq.Expressions.Expression<Func<T, object>>[0];
		}

		public IList<T> Ophalen(IEnumerable<int> ids)
		{
			throw new NotImplementedException();
		}

		public IList<T> Ophalen(IEnumerable<int> ids, params System.Linq.Expressions.Expression<Func<T, object>>[] paths)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
