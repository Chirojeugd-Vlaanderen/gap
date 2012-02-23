// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Data;
using System.Data.Objects;

namespace Chiro.Cdf.Data.Entity
{
	/// <summary>
	/// An Entity Framework addition to manage optimistic
	/// concurrency using OptimisticConcurrencyAttributes.
	/// </summary>
	public class OptimisticConcurrencyManager : IDisposable
	{
		private ObjectContext _context;

		/// <summary>
		/// Instantiates a new OptimisticConcurrencyManager for the
		/// given ObjectContext.
		/// </summary>
		/// <param name="context"></param>
		public OptimisticConcurrencyManager(ObjectContext context)
		{
			_context = context;
			_context.SavingChanges += WhenSavingChanges;
		}

		/// <summary>
		/// Disposes the OptimisticConcurrencyManager, releasing it
		/// from the ObjectContext.
		/// </summary>
		public void Dispose()
		{
			if (_context != null)
			{
				_context.SavingChanges -= WhenSavingChanges;
				_context = null;
			}
		}

		/// <summary>
		/// Triggered when the attached ObjectContext saves changes.
		/// Changes optimistic concurrency attribute values.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void WhenSavingChanges(object sender, EventArgs e)
		{
			// Update the concurrency properties of modified entities:
			foreach (var item in _context.ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
			{
				object entity = item.Entity;
				foreach (var attr in OptimisticConcurrencyAttribute.GetConcurrencyAttributes(entity.GetType()))
				{
					// Verify the property was not yet updated, which could
					// indicate an optimistic concurrency violation:
					if (attr.HasPropertyChanged(_context, entity))
					{
						throw new OptimisticConcurrencyException(String.Format("Concurrency property {0}.{1} contains invalid update.", entity.GetType(), attr.PropertyName));
					}

					attr.UpdateInstance(entity);
				}
			}
		}
	}
}
