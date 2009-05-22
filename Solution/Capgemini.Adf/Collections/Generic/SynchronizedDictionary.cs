using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Cg2.Adf.Collections.Generic
{
	/// <summary>
	/// Represents a thread safe collection of keys and values.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
	[Serializable]
	public class SynchronizedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly IDictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

		[NonSerialized] 
		private ReaderWriterLockSlim dictionaryLock;
		
		/// <summary>
		/// Initializes a new instance of the SynchronizedDictionary&lt;TKey, TValue&gt; class.
		/// </summary>
		public SynchronizedDictionary() { dictionaryLock = Locks.GetLockInstance(); }

		#region IDictionary<TKey,TValue> Members

		///<summary>
		///Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		///</summary>
		///
		///<returns>
		///true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
		///</returns>
		///
		///<param name="key">The key of the element to remove.</param>
		///<exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
		public virtual bool Remove(TKey key)
		{
			using (new WriteLock(dictionaryLock))
				return dict.Remove(key);
		}

		///<summary>
		///Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
		///</summary>
		///
		///<returns>
		///true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.
		///</returns>
		///
		///<param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
		///<exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
		public virtual bool ContainsKey(TKey key)
		{
			using (new ReadLock(dictionaryLock))
				return dict.ContainsKey(key);
		}

		///<summary>
		///Gets the value associated with the specified key.
		///</summary>
		///
		///<returns>
		///true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.
		///</returns>
		///
		///<param name="key">The key whose value to get.</param>
		///<param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
		///<exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
		public virtual bool TryGetValue(TKey key, out TValue value)
		{
			using (new ReadLock(dictionaryLock))
				return dict.TryGetValue(key, out value);
		}

		///<summary>
		///Gets or sets the element with the specified key.
		///</summary>
		///
		///<returns>
		///The element with the specified key.
		///</returns>
		///
		///<param name="key">The key of the element to get or set.</param>
		///<exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
		///<exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> is not found.</exception>
		///<exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
		public virtual TValue this[TKey key]
		{
			get
			{
				using (new ReadLock(dictionaryLock))
					return dict[key];
			}
			set
			{
				using (new WriteLock(dictionaryLock))
					dict[key] = value;
			}
		}

		///<summary>
		///Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
		///</returns>
		///
		public virtual ICollection<TKey> Keys
		{
			get { using (new ReadLock(dictionaryLock)) return new List<TKey>(dict.Keys); }
		}

		///<summary>
		///Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
		///</returns>
		///
		public virtual ICollection<TValue> Values
		{
			get
			{
				using (new ReadLock(dictionaryLock))
					return new List<TValue>(dict.Values);
			}
		}

		///<summary>
		///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		///</summary>
		///
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
		public virtual void Clear()
		{
			using (new WriteLock(dictionaryLock))
				dict.Clear();
		}

		///<summary>
		///Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		///</summary>
		///
		///<returns>
		///The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
		///</returns>
		///
		public virtual int Count
		{
			get
			{
				using (new ReadLock(dictionaryLock))
					return dict.Count;
			}
		}

		///<summary>
		///Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
		///</summary>
		///
		///<returns>
		///true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
		///</returns>
		///
		///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
		public virtual bool Contains(KeyValuePair<TKey, TValue> item)
		{
			using (new ReadLock(dictionaryLock))
				return dict.Contains(item);
		}

		///<summary>
		///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
		///</summary>
		///
		///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
		public virtual void Add(KeyValuePair<TKey, TValue> item)
		{
			using (new WriteLock(dictionaryLock))
				dict.Add(item);
		}

		///<summary>
		///Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		///</summary>
		///
		///<param name="key">The object to use as the key of the element to add.</param>
		///<param name="value">The object to use as the value of the element to add.</param>
		///<exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
		///<exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</exception>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.</exception>
		public virtual void Add(TKey key, TValue value)
		{
			using (new WriteLock(dictionaryLock))
				dict.Add(key, value);
		}

		///<summary>
		///Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
		///</summary>
		///
		///<returns>
		///true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
		///</returns>
		///
		///<param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
		///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
		public virtual bool Remove(KeyValuePair<TKey, TValue> item)
		{
			using (new WriteLock(dictionaryLock))
				return dict.Remove(item);
		}

		///<summary>
		///Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
		///</summary>
		///
		///<param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		///<param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		///<exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.</exception>
		///<exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
		///<exception cref="T:System.ArgumentException"><paramref name="array" /> is multidimensional.-or-<paramref name="arrayIndex" /> is equal to or greater than the length of <paramref name="array" />.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.-or-Type <paramref name="T" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
		public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			using (new ReadLock(dictionaryLock))
				dict.CopyTo(array, arrayIndex);
		}

		///<summary>
		///Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
		///</summary>
		///
		///<returns>
		///true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
		///</returns>
		///
		public virtual bool IsReadOnly
		{
			get
			{
				using (new ReadLock(dictionaryLock))
					return dict.IsReadOnly;
			}
		}

		///<summary>
		///Returns an enumerator that iterates through the collection.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>1</filterpriority>
		public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { throw new NotSupportedException("Cannot enumerate a threadsafe dictionary.  Instead, enumerate the keys or values collection"); }

		///<summary>
		///Returns an enumerator that iterates through a collection.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator() { throw new NotSupportedException("Cannot enumerate a threadsafe dictionary.  Instead, enumerate the keys or values collection"); }

		#endregion

		/// <summary>
		/// This is a blind remove. Prevents the need to check for existence first.
		/// </summary>
		/// <param name="key">Key to remove</param>
		public void RemoveSafe(TKey key)
		{
			using (new ReadLock(dictionaryLock))
				if (dict.ContainsKey(key))
					using (new WriteLock(dictionaryLock))
						dict.Remove(key);
		}

		/// <summary>
		/// Merge does a blind remove, and then add.  Basically a blind Upsert.  
		/// </summary>
		/// <param name="key">Key to lookup</param>
		/// <param name="newValue">New Value</param>
		public void MergeSafe(TKey key, TValue newValue)
		{
			using (new WriteLock(dictionaryLock)) // take a writelock immediately since we will always be writing
			{
				if (dict.ContainsKey(key))
					dict.Remove(key);

				dict.Add(key, newValue);
			}
		}
	}

	internal static class Locks
	{
		public static void GetReadLock(ref ReaderWriterLockSlim locks)
		{
			bool lockAcquired = false;
			while (!lockAcquired)
				lockAcquired = locks.TryEnterUpgradeableReadLock(1);
		}

		public static void GetWriteLock(ref ReaderWriterLockSlim locks)
		{
			bool lockAcquired = false;
			while (!lockAcquired)
				lockAcquired = locks.TryEnterWriteLock(1);
		}

		public static void ReleaseReadLock(ref ReaderWriterLockSlim locks)
		{
			if (locks.IsUpgradeableReadLockHeld)
				locks.ExitUpgradeableReadLock();
		}

		public static void ReleaseWriteLock(ref ReaderWriterLockSlim locks)
		{
			if (locks.IsWriteLockHeld)
				locks.ExitWriteLock();
		}

		public static void ReleaseLock(ref ReaderWriterLockSlim locks)
		{
			ReleaseWriteLock(ref locks);
			ReleaseReadLock(ref locks);
		}

		public static ReaderWriterLockSlim GetLockInstance() { return new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion); }
	}

	internal abstract class ObjectLock : IDisposable
	{
		protected ReaderWriterLockSlim _Locks;

		public ObjectLock(ReaderWriterLockSlim locks) { _Locks = locks; }

		#region IDisposable Members

		public abstract void Dispose();

		#endregion
	}

	internal class ReadLock : ObjectLock
	{
		public ReadLock(ReaderWriterLockSlim locks)
			: base(locks) { Locks.GetReadLock(ref _Locks); }

		public override void Dispose() { Locks.ReleaseReadLock(ref _Locks); }
	}

	internal class WriteLock : ObjectLock
	{
		public WriteLock(ReaderWriterLockSlim locks)
			: base(locks) { Locks.GetWriteLock(ref _Locks); }

		public override void Dispose() { Locks.ReleaseWriteLock(ref _Locks); }
	}
}