// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Data.Objects;
using System.Linq;
using System.Reflection;

namespace Chiro.Cdf.Data.Entity
{
	/// <summary>
	/// Declares a property for optimistic concurrency.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class OptimisticConcurrencyAttribute : Attribute
	{
		private string _propertyName;
		private Type _concurrencyResolverType;
		private readonly object _concurrencyResolver;

		/// <summary>
		/// Declares the optimistic concurrency attribute and its resolvertype.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="concurrencyResolverType"></param>
		public OptimisticConcurrencyAttribute(string propertyName, Type concurrencyResolverType)
		{
			_propertyName = propertyName;
			_concurrencyResolverType = concurrencyResolverType;
			_concurrencyResolver = Activator.CreateInstance(concurrencyResolverType);
		}

		/// <summary>
		/// Name of the concurrency property.
		/// </summary>
		public string PropertyName
		{
			get { return _propertyName; }
			set { _propertyName = value; }
		}

		/// <summary>
		/// Concurrency property value resolver type.
		/// </summary>
		public Type ConcurrencyResolverType
		{
			get { return _concurrencyResolverType; }
			set { _concurrencyResolverType = value; }
		}

		/// <summary>
		/// Whether the optimistic concurrency property has changed on the given instance.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		public bool HasPropertyChanged(ObjectContext context, object instance)
		{
			return context.ObjectStateManager.GetObjectStateEntry(instance).GetModifiedProperties().Contains(_propertyName);
		}

		/// <summary>
		/// Updates the concurrency property on the given instance.
		/// </summary>
		/// <param name="instance"></param>
		public void UpdateInstance(object instance)
		{
			// Retrieve the property instance:
			PropertyInfo property = instance.GetType().GetProperty(_propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (property == null)
			{
				throw new ArgumentException(String.Format("ConcurrencyAttribute's PropertyName \"{0}\" not found.", _propertyName));
			}

			// Invoke the NextValue method on the concurrency resolver, given the actual property value:
			object actualValue = property.GetValue(instance, null);
			Type iftype = typeof(IConcurrencyResolver<>).MakeGenericType(property.PropertyType);
			object nextValue = _concurrencyResolverType.GetInterfaceMap(iftype).TargetMethods[0].Invoke(_concurrencyResolver, new object[1] { actualValue });

			// Assign NextValue:
			property.SetValue(instance, nextValue, null);
		}

		/// <summary>
		/// Retrieves the OptimisticConcurrencyAttributes decorating the given entity type.
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
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
		/// <param name="actualValue"></param>
		/// <returns></returns>
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
