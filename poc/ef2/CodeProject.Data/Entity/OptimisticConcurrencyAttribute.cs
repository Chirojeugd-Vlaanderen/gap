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
using System.Data.Objects;
using System.Linq;
using System.Reflection;

namespace CodeProject.Data.Entity
{
	/// <summary>
	/// Declares a property for optimistic concurrency.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class OptimisticConcurrencyAttribute : Attribute
	{
		private string propertyName;
		private Type concurrencyResolverType;
		private object concurrencyResolver;

		/// <summary>
		/// Declares the optimistic concurrency attribute and its resolvertype.
		/// </summary>
		public OptimisticConcurrencyAttribute(string propertyName, Type concurrencyResolverType)
		{
			this.propertyName = propertyName;
			this.concurrencyResolverType = concurrencyResolverType;
			this.concurrencyResolver = Activator.CreateInstance(concurrencyResolverType);
		}

		/// <summary>
		/// Name of the concurrency property.
		/// </summary>
		public string PropertyName
		{
			get { return this.propertyName; }
			set { this.propertyName = value; }
		}

		/// <summary>
		/// Concurrency property value resolver type.
		/// </summary>
		public Type ConcurrencyResolverType
		{
			get { return this.concurrencyResolverType; }
			set { this.concurrencyResolverType = value; }
		}

		/// <summary>
		/// Whether the optimistic concurrency property has changed on the given instance.
		/// </summary>
		public bool HasPropertyChanged(ObjectContext context, object instance)
		{
			return context.ObjectStateManager.GetObjectStateEntry(instance).GetModifiedProperties().Contains(this.propertyName);
		}

		/// <summary>
		/// Updates the concurrency property on the given instance.
		/// </summary>
		public void UpdateInstance(object instance)
		{
			// Retrieve the property instance:
			PropertyInfo property = instance.GetType().GetProperty(this.propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (property == null) throw new ArgumentException(String.Format("ConcurrencyAttribute's PropertyName \"{0}\" not found.", this.propertyName));

			// Invoke the NextValue method on the concurrency resolver, given the actual property value:
			object actualValue = property.GetValue(instance, null);
			Type iftype = typeof(IConcurrencyResolver<>).MakeGenericType(property.PropertyType);
			object nextValue = this.concurrencyResolverType.GetInterfaceMap(iftype).TargetMethods[0].Invoke(this.concurrencyResolver, new object[1] { actualValue });
			
			// Assign NextValue:
			property.SetValue(instance, nextValue, null);
		}

		/// <summary>
		/// Retrieves the OptimisticConcurrencyAttributes decorating the given entity type.
		/// </summary>
		public static OptimisticConcurrencyAttribute[] GetConcurrencyAttributes(Type entityType)
		{
			return (OptimisticConcurrencyAttribute[])entityType.GetCustomAttributes(typeof(OptimisticConcurrencyAttribute), true);
		}
	}

	/// <summary>
	/// Concurrency Resolver definition.
	/// </summary>
	/// <typeparam name="T">Type of the optimistic concurrency property</typeparam>
	public interface IConcurrencyResolver<T>
	{
		/// <summary>
		/// Provide the next value of the optimistic locking field, given
		/// it's actual value.
		/// </summary>
		T NextValue(T actualValue);
	}

	/// <summary>
	/// A ConcurrencyResolver for DateTime? properties containing LocalDateTime
	/// of last change.
	/// </summary>
	public class LocalDateTimeConcurrencyResolver : IConcurrencyResolver<DateTime?>
	{
		DateTime? IConcurrencyResolver<DateTime?>.NextValue(DateTime? actualValue)
		{
			return DateTime.Now;
		}
	}

	/// <summary>
	/// A ConcurrencyResolver for DateTime? properties containing UniversalDateTime
	/// of last change.
	/// </summary>
	public class UniversalDateTimeConcurrencyResolver : IConcurrencyResolver<DateTime?>
	{
		DateTime? IConcurrencyResolver<DateTime?>.NextValue(DateTime? actualValue)
		{
			return DateTime.UtcNow;
		}
	}

	/// <summary>
	/// A ConcurrencyResolver for Int64 properties containing a sequence number
	/// of last change.
	/// </summary>
	public class VersionNumberConcurrencyResolver : IConcurrencyResolver<long>
	{
		long IConcurrencyResolver<long>.NextValue(long actualValue)
		{
			return actualValue++;
		}
	}
}
