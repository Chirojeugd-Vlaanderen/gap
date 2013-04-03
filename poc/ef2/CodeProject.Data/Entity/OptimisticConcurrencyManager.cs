/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Data;
using System.Data.Objects;

namespace CodeProject.Data.Entity
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
						throw new OptimisticConcurrencyException(String.Format("Concurrency property {0}.{1} contains invalid update.", entity.GetType(), attr.PropertyName));

					attr.UpdateInstance(entity);
				}
			}
		}
	}
}
