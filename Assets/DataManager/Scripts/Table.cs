using System;
using System.Collections.Generic;

namespace DataManagement
{
	public class Table
	{
		public string Name { get; private set; }

		public ObjectInfo Keys { get { return new ObjectInfo( _Keys ); } }
		public StringInfo Fields { get { return new StringInfo( _Fields ); } }
		public StringInfo Types { get { return new StringInfo( _Types ); } }

		private List<object> _Keys { get; set; }
		private List<string> _Fields { get; set; }
		private List<string> _Types { get; set; }

		private Dictionary<object, Data> _key2Data = new Dictionary<object, Data>();
		private Dictionary<string, int> _field2Index = new Dictionary<string, int>();

		public void Initial( string name, List<string> fields, List<string> types, Dictionary<object, Data> key2Data )
		{
			Initial( name, fields, types, new List<object>( key2Data.Keys ), key2Data );
		}

		public void Initial( string name, List<string> fields, List<string> types, List<object> keys, Dictionary<object, Data> key2Data )
		{
			Name = name;

			_Fields = fields;
			_Types = types;
			_Keys = keys;
			_key2Data = key2Data;
		}

		public Data GetData( object key )
		{
			Data data;
			if( _key2Data.TryGetValue( key, out data ) )
			{
				return data.Copy();
			}

			TableTools.Log( TableTools.LogLevel.WARNING, "there is not key: " + key + " in table: " + Name );
			return null;
		}

		public object GetValue( object key, string field )
		{
			Data data = GetData( key );
			if( data != null )
				return data.GetValue( field );

			TableTools.Log( TableTools.LogLevel.WARNING, "there is not key: " + key + " in table: " + Name );
			return null;
		}

		public T GetValue<T>( object key, string field )
		{
			object value = GetValue( key, field );
			if( value != null )
				return (T)Convert.ChangeType( value, typeof(T) );

			return default(T);
		}

		public string GetType( string field )
		{
			int index = _GetFieldIndex( field );
			if( 0 > index || _Types.Count <= index )
			{
				TableTools.Log( TableTools.LogLevel.WARNING, "there is not field name: " + field + " in table: " + Name );
				return string.Empty;
			}
			
			return _Types[index];
		}

		public bool HasValue( object key, string field )
		{
			Data data = GetData( key );
			if( data != null )
			{
				return data.HasValue( field );
			}
			
			return false;
		}

		public bool HasField( string field )
		{
			int index = _GetFieldIndex( field );
			if( 0 > index || _Types.Count <= index )
			{
				TableTools.Log( TableTools.LogLevel.WARNING, "there is not field name: " + field + " in table: " + Name );
				return false;
			}

			return true;
		}

		private int _GetFieldIndex( string field )
		{
			int index = -1;
			if( !_field2Index.TryGetValue( field, out index ) )
			{
				index = _Fields.IndexOf( field );
				_field2Index[field] = index;
			}
			return index;
		}

		public Table Copy()
		{
			Table table = new Table();
			table.Initial( Name, new List<string>( _Fields ), new List<string>( _Types ), new List<object>( _Keys ), _key2Data );

			return table;
		}

		public void Print()
		{
			TableTools.Log( TableTools.LogLevel.WARNING, "===== Table: " + Name + " =====" );
			foreach( KeyValuePair<object, Data> pair in _key2Data )
			{
				pair.Value.Print();
			}
			TableTools.Log( TableTools.LogLevel.WARNING, "===== Table Print Completed =====" );
		}
	}
}