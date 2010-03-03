// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
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
		private ObjectContext context;

		/// <summary>
		/// Instantiates a new OptimisticConcurrencyManager for the
		/// given ObjectContext.
		/// </summary>
		/// <param name="context"></param>
		public OptimisticConcurrencyManager(ObjectContext context)
		{
			this.context = context;
			this.context.SavingChanges += new EventHandler(WhenSavingChanges);
		}

		/// <summary>
		/// Disposes the OptimisticConcurrencyManager, releasing it
		/// from the ObjectContext.
		/// </summary>
		public void Dispose()
		{
			if (this.context != null)
			{
				this.context.SavingChanges -= new EventHandler(WhenSavingChanges);
				this.context = null;
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
			foreach (var item in this.context.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Modified))
			{
				object entity = item.Entity;
				foreach (var attr in OptimisticConcurrencyAttribute.GetConcurrencyAttributes(entity.GetType()))
				{
					// Verify the property was not yet updated, which could
					// indicate an optimistic concurrency violation:
					if (attr.HasPropertyChanged(this.context, entity))
					{
						throw new OptimisticConcurrencyException(String.Format("Concurrency property {0}.{1} contains invalid update.", entity.GetType(), attr.PropertyName));
					}

					attr.UpdateInstance(entity);
				}
			}
		}
	}
}
