using System;
using System.Collections;
using System.Collections.Generic;

namespace DataManagement
{
	public class Data
	{
		public object Key { get; private set; }

		public StringInfo Fields { get { return new StringInfo( _Fields ); } }
		public StringInfo Types { get { return new StringInfo( _Types ); } }

		private List<string> _Fields { get; set; }
		private List<string> _Types { get; set; }
		private List<object> _Values { get; set; }
		private List<bool> _HasValues { get; set; }

		private Dictionary<string, int> _field2Index = new Dictionary<string, int>();

		public void Initial( List<string> fields, List<string> types, object key, List<object> values, List<bool> hasValues )
		{
			_Fields = fields;
			_Types = types;
			Key = key;
			_Values = values;
			_HasValues = hasValues;
		}

		public object GetValue( string field )
		{
			int index = _GetFieldIndex( field, _Values );
			if( 0 > index )
				return null;

			return _Values[index];
		}

		public T GetValue<T>( string field )
		{
			object value = GetValue( field );
			if( value != null )
				return (T)Convert.ChangeType( value, typeof(T) );

			return default(T);
		}

		public string GetType( string field )
		{
			int index = _GetFieldIndex( field, _Types );
			if( 0 > index )
				return string.Empty;

			return _Types[index];
		}

		public bool HasValue( string field )
		{
			int index = _GetFieldIndex( field, _HasValues );
			if( 0 > index )
				return false;
			
			return _HasValues[index];
		}

		public bool HasField( string field )
		{
			int index = _GetFieldIndex( field, _Fields );
			if( 0 > index )
				return false;

			return true;
		}

		public void SetValue<T>( string field, T value )
		{
			string columnType = DataManagerTools.TypeToString( typeof(T) );
			
			SetValue( field, columnType, value );
		}

		public void SetValue( string field, string type, object value )
		{
			int index = _GetFieldIndex( field, _Fields );

			if( -1 == index )
			{
				_Fields.Add( field );
				_Types.Add( type );
				_Values.Add( value );
				_HasValues.Add( true );
				_field2Index.Clear(); // reset the cache
				return;
			}
				
			_Values[index] = value;
		}

		public Data Copy()
		{
			List<object> values = new List<object>();
			for( int i = 0; i < _Values.Count; ++i )
			{
				values.Add( _Copy( _Values[i] ) );
			}
		
			Data data = new Data();
			data.Initial( new List<string>( _Fields ), new List<string>( _Types ), Key, values, new List<bool>( _HasValues ) );
			
			return data;
		}

		public object _Copy( object value )
		{
			if( value is IList )
			{
				IList list = Activator.CreateInstance( value.GetType() ) as IList;
				IList listSource = value as IList;
				foreach( object val in listSource )
				{
					list.Add( _Copy( val ) );
				}
				
				return list;
			}
			else if( value is IDictionary )
			{
				IDictionary dict = Activator.CreateInstance( value.GetType() ) as IDictionary;
				IDictionary dictSource = value as IDictionary;
				foreach( DictionaryEntry pair in dictSource )
				{
					dict.Add( _Copy( pair.Key ), _Copy( pair.Value ) );
				}
				
				return dict;
			}
			else
			{
				return value;
			}
		}

		private int _GetFieldIndex( string field, IList list )
		{
			int index = -1;
			if( !_field2Index.TryGetValue( field, out index ) )
			{
				index = _Fields.IndexOf( field );
				if( 0 > index || list.Count <= index )
				{
					TableTools.Log( TableTools.LogLevel.ERROR, "index out of range: " + index + ", size:" + list.Count );
					index = -1;
				}

				_field2Index[field] = index;
			}
			return index;
		}

		public void Print()
		{
			TableTools.Log( TableTools.LogLevel.DEFAULT, "===== Key: " + Key + " =====" );

			for( int i = 0; i < _Values.Count; ++i )
			{
				TableTools.Log( TableTools.LogLevel.DEFAULT, "column name: " + _Fields[i] + ", column type: " + _Types[i] );

				_Print( _Values[i] );
			}

			TableTools.Log( TableTools.LogLevel.DEFAULT, "===== Data Print Completed =====" );
		}

		private void _Print( object value )
		{
			if( value is IList )
			{
				IList list = (IList)value;
				foreach( object obj in list )
				{
					_Print( obj );
				}
			}
			else if( value is IDictionary )
			{
				IDictionary dict = (IDictionary)value;
				foreach( DictionaryEntry pair in dict )
				{
					TableTools.Log( TableTools.LogLevel.DEFAULT, "key: " + pair.Key );
					_Print( pair.Value );
				}
			}
			else
			{
				TableTools.Log( TableTools.LogLevel.DEFAULT, "value: " + value );
			}
		}
	}
}