using System;
using System.Collections.Generic;
using System.Collections;

//using System.Collections.Immutable;
using System.Linq;

namespace Lucene.Net
{
	public static class Collections
	{
		public static ISet<T> Singleton<T>(T o)
		{
			//return ImmutableHashSet.Create(o);
			var Set = new HashSet<T>();
			Set.Add(o);
			return new ReadonlySet<T>(Set);
		}

		public static IList<T> EmptyList<T>()
		{
			return new List<T>().AsReadOnly();
		}

		public static IList<T> UnmodifiableList<T>(IEnumerable<T> items)
		{
			return new List<T>(items.ToArray()).AsReadOnly();
		}

		public static IList<T> UnmodifiableList<T>(List<T> items)
		{
			return items.AsReadOnly();
		}

		public static ISet<T> UnmodifiableSet<T>(IEnumerable<T> items)
		{
			return new ReadonlySet<T>(items.ToArray());
		}

		public static IDictionary<T, TS> UnmodifiableMap<T, TS>(IDictionary<T, TS> d)
		{
			return new ReadonlyDic<T, TS>(d);
		}


		private class ReadonlyDic<TKey,TValue> : IDictionary<TKey,TValue>
		{

			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			{
				return inner.GetEnumerator();
			}

			IEnumerator  IEnumerable.GetEnumerator()
			{
				return inner.GetEnumerator();
			}

			//
			// Properties
			//
			public int Count {
				get { 
					return inner.Count;
				}
			}

			public bool IsReadOnly {
				get { 
					return true;
				}
			}

			//
			// Methods
			//
			public void Add(KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException();

			}

			public void Clear()
			{
				throw new NotImplementedException();
			}

			public bool Contains(KeyValuePair<TKey, TValue> item)
			{
				return inner.Contains(item);
			}

			public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
				inner.CopyTo(array, arrayIndex);
			}

			public bool Remove(KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException();
			}

			public ReadonlyDic(IDictionary<TKey,TValue> dic)
			{
				inner = new Dictionary<TKey, TValue>(dic);
			}

			private IDictionary<TKey, TValue> inner;

			//
			// Properties
			//
			public ICollection<TKey> Keys {
				get { 
					return inner.Keys;
				}
			}

			public ICollection<TValue> Values {
				get { 
					return inner.Values;
				}
			}

			//
			// Indexer
			//
			public TValue this [TKey key] {
				get { 
					return inner[key];
				}
				set {
					throw new NotSupportedException();
				}
			}

			//
			// Methods
			//
			public void Add(TKey key, TValue value)
			{
				throw new NotSupportedException();
			}

			public bool ContainsKey(TKey key)
			{
				return inner.ContainsKey(key);
			}

			public bool Remove(TKey key)
			{
				throw new NotSupportedException();
			}

			public bool TryGetValue(TKey key, out TValue value)
			{
				return inner.TryGetValue(key, out value);
			}
		}

		private class ReadonlySet<T> : ISet<T>
		{

			void ICollection<T>.Add(T item)
			{
				throw new NotSupportedException();
			}

			public IEnumerator<T> GetEnumerator()
			{
				return inner.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return inner.GetEnumerator();
			}

			//
			// Properties
			//
			public int Count {
				get { 
					return inner.Count;
				}
			}

			public bool IsReadOnly {
				get { 
					return inner.IsReadOnly;
				}
			}

			//
			// Methods
			//
			public bool Add(T item)
			{
				throw new NotSupportedException();

			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool Contains(T item)
			{
				return inner.Contains(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				inner.CopyTo(array, arrayIndex);
			}

			public bool Remove(T item)
			{
				throw new NotSupportedException();
			}

			public ReadonlySet(IEnumerable<T> inner)
			{
				this.inner = new HashSet<T>(inner);
			}

			private ISet<T> inner;

			public void ExceptWith(IEnumerable<T> other)
			{
				throw new NotSupportedException();
			}

			public void IntersectWith(IEnumerable<T> other)
			{
				throw new NotSupportedException();
			}

			public bool IsProperSubsetOf(IEnumerable<T> other)
			{
				return inner.IsProperSubsetOf(other);
			}

			public bool IsProperSupersetOf(IEnumerable<T> other)
			{
				return inner.IsProperSupersetOf(other);
			}

			public bool IsSubsetOf(IEnumerable<T> other)
			{
				return inner.IsSubsetOf(other);
			}

			public bool IsSupersetOf(IEnumerable<T> other)
			{
				return inner.IsSupersetOf(other);
			}

			public bool Overlaps(IEnumerable<T> other)
			{
				return inner.Overlaps(other);
			}

			public bool SetEquals(IEnumerable<T> other)
			{
				return inner.SetEquals(other);
			}

			public void SymmetricExceptWith(IEnumerable<T> other)
			{
				throw new NotSupportedException();
			}

			public void UnionWith(IEnumerable<T> other)
			{
				throw new NotSupportedException();
			}

		}

	}
}
