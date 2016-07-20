using UnityEngine;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace DataManagement
{
	public sealed class DataManager
	{
		private static DataManager _instance = null;
		private static DataManager _Instance
		{
			get
			{
				if( _instance == null )
				{
					_instance = new DataManager();
				}
				return _instance;
			}
		}

		class TableInfo
		{
			public string path;
			public TableReader reader;
			public TableParser parser;
			public TableConverter converter;
			public Table table;
		}

		private Dictionary<string, TableInfo> _tableInfo = new Dictionary<string, TableInfo>();
		private Dictionary<string, ConstructorInfo> _constructor = new Dictionary<string, ConstructorInfo>();

		private DataManager()
		{
			// init table info
			TableInfo info = new TableInfo();
			_tableInfo[EDataManager.TABLE_CONFIG] = info;
			info.path = EDataManager.TABLE_PATH;
			info.reader = _GetConstroctor( EDataManager.CONFIG_READER ) as TableReader;
			info.parser = _GetConstroctor( EDataManager.CONFIG_PARSER ) as TableParser;
			info.converter = _GetConstroctor( EDataManager.CONFIG_CONVERTER ) as TableConverter;
			info.table = _LoadTable( EDataManager.TABLE_CONFIG );
			Table configTable = info.table;
			if( null != configTable )
			{
				foreach( object key in configTable.Keys )
				{
					string tableName = configTable.GetValue<string>( key, EDataManager.NAME );

					info = new TableInfo();
					_tableInfo[tableName] = info;
					info.path = configTable.GetValue<string>( key, EDataManager.PATH );
					info.reader = _GetConstroctor( configTable.GetValue<string>( key, EDataManager.READER ) ) as TableReader;
					info.parser = _GetConstroctor( configTable.GetValue<string>( key, EDataManager.PARSER ) ) as TableParser;
					info.converter = _GetConstroctor( configTable.GetValue<string>( key, EDataManager.CONVERTER ) ) as TableConverter;
					bool preload = configTable.GetValue<bool>( key, EDataManager.PRELOAD );

					if( preload )
					{
						info.table = _LoadTable( tableName );
					}
				}
			}
			else
			{
				Debug.LogError(  "Load failed:" + EDataManager.TABLE_CONFIG );
				return;
			}
		}

		private Table _LoadTable( string tableName )
		{
			if( string.IsNullOrEmpty( tableName ) )
			{
				Debug.LogError( "failed to get source; table=string.Empty" );
				return null;
			}
				
			TableInfo config = _GetTableInfo( tableName );
			string path = config.path;
			TableReader reader = config.reader;
			TableParser parser = config.parser;
			TableConverter converter = config.converter;
				
			if( reader == null )
			{
				Debug.LogError("failed to get reader; table=" + tableName );
				return null;
			}

			if( parser == null )
			{
				Debug.LogError("failed to get parser; table=" + tableName );
				return null;
			}

			if( converter == null )
			{
				Debug.LogError("failed to get converter; table=" + tableName );
				return null;
			}

			object text = reader.Get( path + tableName );
			object parsedData = parser.Parse( text );

			return converter.Convert( tableName, parsedData );
		}

		private TableInfo _GetTableInfo( string tableName )
		{
			TableInfo info = null;
			if( !_tableInfo.TryGetValue( tableName, out info ) )
			{
				info = new TableInfo();
				_tableInfo[tableName] = info;
				info.path = _tableInfo[EDataManager.TABLE_CONFIG].path;
				info.reader = _tableInfo[EDataManager.TABLE_CONFIG].reader;
				info.parser = _tableInfo[EDataManager.TABLE_CONFIG].parser;
				info.converter = _tableInfo[EDataManager.TABLE_CONFIG].converter;
			}

			return info;
		}

		private object _GetConstroctor( string className )
		{
			ConstructorInfo constructor;
			if( !_constructor.TryGetValue( className, out constructor ) )
			{
				Type type = Type.GetType( className );
				if( null == type )
				{
					Debug.LogError("this class name: " + className + " can't reflection(class must inherit CDataSource or CDataParser)" );
					return null;
				}
				
				constructor = type.GetConstructor( new Type[]{} );
				if( null == constructor )
				{
					Debug.LogError("there is not default constructor on class name: " + className );
					return null;
				}

				_constructor[className] = constructor;
			}
			
			return constructor.Invoke( null );
		}

		public static Table GetTable( string tableName )
		{
			return _Instance._GetTable( tableName );
		}

		private Table _GetTable( string tableName )
		{
			TableInfo config = _GetTableInfo( tableName );
			if( config.table == null )
			{
				config.table = _LoadTable( tableName );
				if( config.table == null )
				{
					Debug.LogError( "failed to get table: " + tableName );
					return null;
				}
			}
			
			return config.table.Copy();
		}

		public static Data GetData( string tableName, object key )
		{
			return _Instance._GetData( tableName, key );
		}

		private Data _GetData( string tableName, object key )
		{
			Table table = _GetTable( tableName );

			if( table == null )
				return null;
			
			return table.GetData( key );
		}
	}
}