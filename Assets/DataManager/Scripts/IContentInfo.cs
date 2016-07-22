using System.Collections;
using System.Collections.Generic;
using System;

namespace DataManagement
{
	public abstract class IContentInfo<T> : IEnumerable
	{
		protected List<T> _List { get; set; }

		public int Count { get { return _List.Count; } }
		public T this[int i] { get { return _List[i]; } }

		public List<T> ToList()
		{
			return new List<T>( _List );
		}

		public int IndexOf( T item )
		{
			return _List.IndexOf( item );
		}

		public bool Contains( T item )
		{
			return _List.Contains( item );
		}

		public bool IsFixedSize { get { return false; } }

		public IEnumerator GetEnumerator()
		{
			return new IContentInfoEnumerator<T>( this );
		}
	}
		
	class IContentInfoEnumerator<T> : IEnumerator
	{
		private IContentInfo<T> _info;
		private int _index = -1;

		public IContentInfoEnumerator( IContentInfo<T> info )
		{
			_info = info;
		}

		public void Reset()
		{
			_index = -1;
		}

		public object Current
		{
			get
			{
				if (_index == -1 || _index >= _info.Count )
				{
					throw new InvalidOperationException();
				}

				return _info[_index];
			}
		}

		public bool MoveNext()
		{
			this._index++;
			return _index < _info.Count;
		}
	}

	public class StringInfo : IContentInfo<string>
	{
		public StringInfo( List<string> list )
		{
			_List = list;
		}
	}

	public class ObjectInfo : IContentInfo<object>
	{
		public ObjectInfo( List<object> list )
		{
			_List = list;
		}
	}
}