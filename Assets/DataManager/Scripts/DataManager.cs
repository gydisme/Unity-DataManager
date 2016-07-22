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

			public TableInfo()
			{
				path = EDataManager.DEFAULT_TABLE_PATH;
				reader = _Instance._GetConstroctor( EDataManager.DEFAULT_READER ) as TableReader;
				parser = _Instance._GetConstroctor( EDataManager.DEFAULT_PARSER ) as TableParser;
				converter = _Instance._GetConstroctor( EDataManager.DEFAULT_CONVERTER ) as TableConverter;
			}
		}

		private Dictionary<string, TableInfo> _tableInfo = new Dictionary<string, TableInfo>();
		private Dictionary<string, ConstructorInfo> _constructor = new Dictionary<string, ConstructorInfo>();

		private DataManager()
		{
			_Restart();
		}

		public static void Restart()
		{
			_Instance._Restart();
		}

		private void _Restart()
		{
			_tableInfo.Clear();
		}

		public static void Preload()
		{
			_Instance._Preload();
		}

		private void _Preload()
		{
			Table configTable = GetTable( EDataManager.TABLE_CONFIG );
			foreach( string tableName in configTable.Keys )
			{
				TableInfo info = _GetTableInfo( tableName );
				if( info.table == null && configTable.GetValue<bool>( tableName, EDataManager.PRELOAD ) )
				{
					info.table = _LoadTable( tableName );
				}
			}
		}

		private Table _LoadTable( string tableName )
		{
			if( string.IsNullOrEmpty( tableName ) )
			{
				TableTools.Log( TableTools.LogLevel.ERROR, "failed to get source; table=string.Empty" );
				return null;
			}
				
			TableInfo info = _GetTableInfo( tableName );
			if( info.table != null )
				return info.table;
			
			string path = info.path;
			TableReader reader = info.reader;
			TableParser parser = info.parser;
			TableConverter converter = info.converter;
				
			if( reader == null )
			{
				TableTools.Log( TableTools.LogLevel.ERROR,"failed to get reader; table=" + tableName );
				return null;
			}

			if( parser == null )
			{
				TableTools.Log( TableTools.LogLevel.ERROR,"failed to get parser; table=" + tableName );
				return null;
			}

			if( converter == null )
			{
				TableTools.Log( TableTools.LogLevel.ERROR,"failed to get converter; table=" + tableName );
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

				if( string.Compare( tableName, EDataManager.TABLE_CONFIG ) == 0)
				{
					info.table = _LoadTable( tableName );
				}
				else
				{
					Table configTable = GetTable( EDataManager.TABLE_CONFIG );
					if( configTable.Keys.Contains( tableName ) )
					{
						info.path = configTable.GetValue<string>( tableName, EDataManager.PATH );
						info.reader = _GetConstroctor( configTable.GetValue<string>( tableName, EDataManager.READER ) ) as TableReader;
						info.parser = _GetConstroctor( configTable.GetValue<string>( tableName, EDataManager.PARSER ) ) as TableParser;
						info.converter = _GetConstroctor( configTable.GetValue<string>( tableName, EDataManager.CONVERTER ) ) as TableConverter;
					}
				}
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
					TableTools.Log( TableTools.LogLevel.ERROR,"this class name: " + className + " can't reflection(class must inherit CDataSource or CDataParser)" );
					return null;
				}
				
				constructor = type.GetConstructor( new Type[]{} );
				if( null == constructor )
				{
					TableTools.Log( TableTools.LogLevel.ERROR,"there is not default constructor on class name: " + className );
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
					TableTools.Log( TableTools.LogLevel.ERROR, "failed to get table: " + tableName );
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