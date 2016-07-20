using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using DataManagement;

public class TableConverterList : TableConverter
{
	private Dictionary<string, ConstructorInfo> _name2ConstructorInfo = new Dictionary<string, ConstructorInfo>();	// Reflection Cache
	private Dictionary<string, Type> _name2Type = new Dictionary<string, Type>();	// Reflection Cache

	public override Table Convert(string name, object input)
	{
		return _Convert( name, (List<List<string>>)input );
	}

	private Table _Convert( string name, List<List<string>> parsedData )
	{
		if( null == parsedData || parsedData.Count == 0 )
		{
			Debug.LogError( "No Table or Empty: " + name );
			return null;
		}

		List<string> columnNames = parsedData[0];
		List<string> columnTypes = parsedData[1];

		#region Remove NonUse Column
		int columnNamesIndex = columnNames.FindIndex( delegate( string str ) { return string.IsNullOrEmpty( str ); } );
		if( -1 != columnNamesIndex )	columnNames.RemoveRange( columnNamesIndex, columnNames.Count - columnNamesIndex );
		
		int columnTypesIndex = columnTypes.FindIndex( delegate( string str ) { return string.IsNullOrEmpty( str ); } );
		if( -1 != columnTypesIndex )	columnTypes.RemoveRange( columnTypesIndex, columnTypes.Count - columnTypesIndex );
		
		if( columnNames.Count < columnTypes.Count )			columnTypes.RemoveRange( columnNames.Count, columnTypes.Count - columnNames.Count );
		else if( columnNames.Count > columnTypes.Count )	columnNames.RemoveRange( columnTypes.Count, columnNames.Count - columnTypes.Count );
		#endregion
		
		#region Read Key and Column Values
		Dictionary<object, Data> key2Data = new Dictionary<object, Data>();
		for( int i = 2; i < parsedData.Count; ++i )
		{
			List<object> values = new List<object>();
			List<bool> hasValues = new List<bool>();
			
			#region Index 0 is Key
			// key is null or empty
			if( string.IsNullOrEmpty( parsedData[i][0] ) )
			{
//				Debug.LogWarning( "there is null or empty key at row: " + i + " in table: " + name );
				break;
			}
			#endregion

			for( int j = 0; j < parsedData[i].Count; ++j )
			{
				if( columnTypes.Count > j )
				{
					object obj = null;
					_StringToObject( name, ( i + 1 ), columnNames[j], columnTypes[j], parsedData[i][j], ref obj );
					values.Add( obj );
					hasValues.Add( string.IsNullOrEmpty( parsedData[i][j] ) ? false : true );
				}
			}
			
			Data data = new Data();
			data.Initial( columnNames, columnTypes, values[0], values, hasValues );
			
			// this is duplicate key
			if( key2Data.ContainsKey( values[0] ) )
			{
				Debug.LogError( "there is duplicate key: " + values[0] + " at row: " + i + " in table: " + name );
				continue;
			}
			
			key2Data[values[0]] = data;
		}
		#endregion

		Table table = new Table();
		table.Initial( name, columnNames, columnTypes, key2Data );

		return table;
	}

	private string _StringToObject( string name, int row, string columnName, string typeStr, string valueStr, ref object value )
	{
		string realTypeStr = string.Empty;
		if( 0 <= typeStr.IndexOf( EDataManager.BRACKET_LEFT_STRING ) )
		{
			realTypeStr += ( EDataManager.NAMESPACE_STRING + EDataManager.LIST_TYPE_STRING + EDataManager.BRACKET_LEFT_STRING );
			typeStr = typeStr.Substring( 1 );
			typeStr = typeStr.Remove( typeStr.Length - 1 );

			List<string> values = _StringToList( valueStr );

			System.Collections.IList list = null;
			foreach( string v in values )
			{
				string valueType = _StringToObject( name, row, columnName, typeStr, v, ref value );

				if( null == list ) 
				{
					realTypeStr += valueType;
					realTypeStr += EDataManager.BRACKET_RIGHT_STRING;

					ConstructorInfo constructor = null;
					if( !_name2ConstructorInfo.TryGetValue( realTypeStr, out constructor ) )
					{
						Type listType = Type.GetType( realTypeStr );
						constructor = listType.GetConstructor( new Type[]{} );
						_name2ConstructorInfo[realTypeStr] = constructor;
					}
					object o = constructor.Invoke( null );
					list = (System.Collections.IList)o;

					if( string.IsNullOrEmpty( valueStr ) ) break;
				}

				list.Add( value );
			}

			value = list;
		}
		else if( 0 <= typeStr.IndexOf( EDataManager.LESS_THAN_STRING ) )
		{
			realTypeStr += ( EDataManager.NAMESPACE_STRING + EDataManager.DICT_TYPE_STRING + EDataManager.BRACKET_LEFT_STRING );
			typeStr = typeStr.Substring( 1 );
			typeStr = typeStr.Remove( typeStr.Length - 1 );

			Dictionary<string, string> values = null;
			try
			{
				values = _StringToDict( valueStr );
			}
			catch( Exception e )
			{
				Debug.LogError( "table=" + name + ", row=" + row + ", column=" + columnName + ", value=" + valueStr + ", exception=" + e.ToString() );
			}

			System.Collections.IDictionary dict = null;
			foreach( KeyValuePair<string, string> pair in values )
			{
				int index = typeStr.IndexOf( EDataManager.COMMA_CHAR );

				object key = null;
				string keyType = _StringToObject( name, row, columnName, typeStr.Remove( index ), pair.Key, ref key );

				object val = null;
				string valueType = _StringToObject( name, row, columnName, typeStr.Substring( index + 1 ), pair.Value, ref val );

				if( null == dict )
				{
					realTypeStr += ( keyType + EDataManager.COMMA_STRING + valueType );
					realTypeStr += EDataManager.BRACKET_RIGHT_STRING;
			
					ConstructorInfo constructor = null;
					if( !_name2ConstructorInfo.TryGetValue( realTypeStr, out constructor ) )
					{
						Type dictType = Type.GetType( realTypeStr );
						constructor = dictType.GetConstructor( new Type[]{} );
						_name2ConstructorInfo[realTypeStr] = constructor;
					}
					object o = constructor.Invoke( null );
					dict = (System.Collections.IDictionary)o;

					if( string.IsNullOrEmpty( valueStr ) ) break;
				}
			
				dict.Add( key, val );
			}

			value = dict;
		}
		else
		{
			realTypeStr += ( EDataManager.SYSTEM_DOT_STRING + typeStr );

			Type valueType;
			if( !_name2Type.TryGetValue( realTypeStr, out valueType ) )
			{
				valueType = Type.GetType( realTypeStr );
				_name2Type[realTypeStr] = valueType;
			}

			if( string.IsNullOrEmpty( valueStr ) && null != valueType && typeof( System.String ) != valueType )
			{
				try
				{
					// FormatterServices可以實體化物件並且不呼叫建構式, 而由於內建型別本身並無建構式, 採用此方式來取代Activator.CreateInstance( valueType )提升效能
					// https://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.formatterservices.getuninitializedobject(v=vs.110).aspx
					value = FormatterServices.GetUninitializedObject( valueType );
				}
				catch( Exception )
				{
					Debug.LogError( "there is exception at row: " + row + " and column name: "+ columnName + " in table: " + name );
				}
			}
			else
			{
				try
				{
					value = System.Convert.ChangeType( valueStr, valueType );
				}
				catch( FormatException )
				{
					Debug.LogError( "there is error value: " + valueStr + " at row: " + row + " and column name: " + columnName + " in table: " + name );
				}
				catch( ArgumentNullException )
				{
					Debug.LogError( "there is error type: " + typeStr + " at column name: " + columnName + " in table: " + name );
				}
				catch( Exception )
				{
					Debug.LogError( "there is exception at row: " + row + " and column name: "+ columnName + " in table: " + name );
				}
			}
		}

		return realTypeStr;
	}

	public T StringToObject<T>( string valueStr )
	{
		string typeStr = DataManagerTools.TypeToString( typeof( T ) );

		object value = null;
		_StringToObject( typeStr, valueStr, ref value );

		return (T)System.Convert.ChangeType( value, typeof( T ) );
	}

	private string _StringToObject( string typeStr, string valueStr, ref object value )
	{
		string realTypeStr = string.Empty;
		if( 0 <= typeStr.IndexOf( EDataManager.BRACKET_LEFT_STRING ) )
		{
			realTypeStr += ( EDataManager.NAMESPACE_STRING + EDataManager.LIST_TYPE_STRING + EDataManager.BRACKET_LEFT_STRING );
			typeStr = typeStr.Substring( 1 );
			typeStr = typeStr.Remove( typeStr.Length - 1 );
			
			List<string> values = _StringToList( valueStr );
			
			System.Collections.IList list = null;
			foreach( string v in values )
			{
				string valueType = _StringToObject( typeStr, v, ref value );
				
				if( null == list ) 
				{
					realTypeStr += valueType;
					realTypeStr += EDataManager.BRACKET_RIGHT_STRING;
					
					ConstructorInfo constructor = null;
					if( !_name2ConstructorInfo.TryGetValue( realTypeStr, out constructor ) )
					{
						Type listType = Type.GetType( realTypeStr );
						constructor = listType.GetConstructor( new Type[]{} );
						_name2ConstructorInfo[realTypeStr] = constructor;
					}
					object o = constructor.Invoke( null );
					list = (System.Collections.IList)o;
					
					if( string.IsNullOrEmpty( valueStr ) ) break;
				}
				
				list.Add( value );
			}
			
			value = list;
		}
		else if( 0 <= typeStr.IndexOf( EDataManager.LESS_THAN_STRING ) )
		{
			realTypeStr += ( EDataManager.NAMESPACE_STRING + EDataManager.DICT_TYPE_STRING + EDataManager.BRACKET_LEFT_STRING );
			typeStr = typeStr.Substring( 1 );
			typeStr = typeStr.Remove( typeStr.Length - 1 );
			
			Dictionary<string, string> values = _StringToDict( valueStr );
			
			System.Collections.IDictionary dict = null;
			foreach( KeyValuePair<string, string> pair in values )
			{
				int index = typeStr.IndexOf( EDataManager.COMMA_CHAR );
				
				object key = null;
				string keyType = _StringToObject( typeStr.Remove( index ), pair.Key, ref key );
				
				object val = null;
				string valueType = _StringToObject( typeStr.Substring( index + 1 ), pair.Value, ref val );
				
				if( null == dict )
				{
					realTypeStr += ( keyType + EDataManager.COMMA_STRING + valueType );
					realTypeStr += EDataManager.BRACKET_RIGHT_STRING;
					
					ConstructorInfo constructor = null;
					if( !_name2ConstructorInfo.TryGetValue( realTypeStr, out constructor ) )
					{
						Type dictType = Type.GetType( realTypeStr );
						constructor = dictType.GetConstructor( new Type[]{} );
						_name2ConstructorInfo[realTypeStr] = constructor;
					}
					object o = constructor.Invoke( null );
					dict = (System.Collections.IDictionary)o;
					
					if( string.IsNullOrEmpty( valueStr ) ) break;
				}
				
				dict.Add( key, val );
			}
			
			value = dict;
		}
		else
		{
			realTypeStr += ( EDataManager.SYSTEM_DOT_STRING + typeStr );
			
			Type valueType;
			if( !_name2Type.TryGetValue( realTypeStr, out valueType ) )
			{
				valueType = Type.GetType( realTypeStr );
				_name2Type[realTypeStr] = valueType;
			}

			if( string.IsNullOrEmpty( valueStr ) && null != valueType && typeof( System.String ) != valueType )
			{
				try
				{
					// FormatterServices可以實體化物件並且不呼叫建構式, 而由於內建型別本身並無建構式, 採用此方式來取代Activator.CreateInstance( valueType )提升效能
					// https://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.formatterservices.getuninitializedobject(v=vs.110).aspx
					value = FormatterServices.GetUninitializedObject( valueType );
				}
				catch( Exception )
				{
					Debug.LogError( "there is exception, value: " + valueStr + ", type: " + valueType );
				}
			}
			else
			{
				try
				{
					value = System.Convert.ChangeType( valueStr, valueType );
				}
				catch( FormatException )
				{
					Debug.LogError( "there is error value: " + valueStr );
				}
				catch( ArgumentNullException )
				{
					Debug.LogError( "there is error type: " + typeStr );
				}
				catch( Exception )
				{
					Debug.LogError( "there is exception, value: " + valueStr + ", type: " + valueType );
				}
			}
		}
		
		return realTypeStr;
	}

	private List<string> _StringToList( string valueStr )
	{
		List<string> values = new List<string>();

		int count = 0;
		bool hasSign = false;
		string value = string.Empty;
		foreach( char c in valueStr )
		{
			if( hasSign && ( c.Equals( EDataManager.BRACKET_LEFT_CHAR ) || c.Equals( EDataManager.LESS_THAN_CHAR ) ) ) ++count;
			else if( hasSign && 0 < count && ( c.Equals( EDataManager.BRACKET_RIGHT_CHAR ) || c.Equals( EDataManager.GREAT_THAN_CHAR ) ) ) --count;
			else if( !hasSign && c.Equals( EDataManager.COMMA_CHAR ) ) { values.Add( value ); value = string.Empty; continue; }
			else if( c.Equals( EDataManager.BRACKET_LEFT_CHAR ) || c.Equals( EDataManager.LESS_THAN_CHAR ) ) { hasSign = true; continue; }
			else if( c.Equals( EDataManager.BRACKET_RIGHT_CHAR ) || c.Equals( EDataManager.GREAT_THAN_CHAR ) ) { hasSign = false; continue; }

			value += c.ToString();
		}
		values.Add( value );

		return values;
	}

	private Dictionary<string,string> _StringToDict( string valueStr )
	{
		Dictionary<string, string> values = new Dictionary<string, string>();

		int count = 0;
		bool hasSign = false;
		string key = string.Empty;
		string value = string.Empty;

		try
		{
			foreach( char c in valueStr )
			{
				if( hasSign && ( c.Equals( EDataManager.BRACKET_LEFT_CHAR ) || c.Equals( EDataManager.LESS_THAN_CHAR ) ) ) ++count;
				else if( hasSign && 0 < count && ( c.Equals( EDataManager.BRACKET_RIGHT_CHAR ) || c.Equals( EDataManager.GREAT_THAN_CHAR ) ) ) --count;
				else if( !hasSign && c.Equals( EDataManager.COMMA_CHAR ) ) { values.Add( key, value ); key = string.Empty; value = string.Empty; continue; }
				else if( !hasSign && c.Equals( EDataManager.COLON_CHAR ) ) { key = value; value = string.Empty; continue; }
				else if( c.Equals( EDataManager.BRACKET_LEFT_CHAR ) || c.Equals( EDataManager.LESS_THAN_CHAR ) ) { hasSign = true; continue; }
				else if( c.Equals( EDataManager.BRACKET_RIGHT_CHAR ) || c.Equals( EDataManager.GREAT_THAN_CHAR ) ) { hasSign = false; continue; }
				
				value += c.ToString();
			}
			values.Add( key, value );
		}
		catch( Exception e )
		{
			throw( e );
		}

		return values;
	}

	public Type StringToType( string typeStr )
	{
		string realTypeStr = _StringToType( typeStr );
		
		return Type.GetType( realTypeStr );
	}

	private string _StringToType( string typeStr )
	{
		string realTypeStr = string.Empty;
		if( 0 <= typeStr.IndexOf( EDataManager.BRACKET_LEFT_STRING ) )
		{
			realTypeStr += ( EDataManager.NAMESPACE_STRING + EDataManager.LIST_TYPE_STRING + EDataManager.BRACKET_LEFT_STRING );
			typeStr = typeStr.Substring( 1 );
			typeStr = typeStr.Remove( typeStr.Length - 1 );
			realTypeStr += _StringToType( typeStr );
			realTypeStr += EDataManager.BRACKET_RIGHT_STRING;
		}
		else if( 0 <= typeStr.IndexOf( EDataManager.LESS_THAN_STRING ) )
		{
			realTypeStr += ( EDataManager.NAMESPACE_STRING + EDataManager.DICT_TYPE_STRING + EDataManager.BRACKET_LEFT_STRING );
			typeStr = typeStr.Substring( 1 );
			typeStr = typeStr.Remove( typeStr.Length - 1 );

			int index = typeStr.IndexOf( EDataManager.COMMA_CHAR );
			string keyType = _StringToType( typeStr.Remove( index ) );
			string valueType = _StringToType( typeStr.Substring( index + 1 ) );

			realTypeStr += ( keyType + EDataManager.COMMA_STRING + valueType );
			realTypeStr += EDataManager.BRACKET_RIGHT_STRING;
		}
		else
		{
			realTypeStr += ( EDataManager.SYSTEM_DOT_STRING + typeStr );
		}
		
		return realTypeStr;
	}

	public List<List<string>> Convert( Table table )
	{
		return _Convert( table );
	}

	private List<List<string>> _Convert( Table table )
	{
		List<List<string>> parsedData = new List<List<string>>();
		parsedData.Add( table.Fields.ToList() );
		parsedData.Add( table.Types.ToList() );

		foreach( object key in table.Keys )
		{
			List<string> datas = new List<string>();
			for( int i = 0,imax=table.Fields.Count;i<imax;i++ )
			{
				string str = DataManagerTools.ObjectToString( table.GetValue( key, table.Fields[i] ) );
				if( 0 <= table.Types[i].IndexOf( EDataManager.BRACKET_LEFT_STRING ) || 0 <= table.Types[i].IndexOf( EDataManager.LESS_THAN_STRING ) )
				{
					str = str.Substring( 1, str.Length - 2 );
				}

				datas.Add( str );
			}

			parsedData.Add( datas );
		}

		return parsedData;
	}

	public List<string> Convert( Data data )
	{
		return _Convert( data );
	}

	private List<string> _Convert( Data data )
	{
		List<string> line = new List<string>();
		for( int i = 0, imax=data.Fields.Count;i<imax; i++ )
		{
			string str = DataManagerTools.ObjectToString( data.GetValue( data.Fields[i] ) );
			if( 0 <= data.Types.IndexOf( EDataManager.BRACKET_LEFT_STRING ) || 0 <= data.Types.IndexOf( EDataManager.LESS_THAN_STRING ) )
			{
				str = string.IsNullOrEmpty( str ) ? str : str.Substring( 1, str.Length - 2 );
			}
			
			line.Add( str );
		}
		
		return line;
	}


}
