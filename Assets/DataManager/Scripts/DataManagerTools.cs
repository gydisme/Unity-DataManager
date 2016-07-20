using System;
using System.Collections;

namespace DataManagement
{
	public static class DataManagerTools
	{
		public static string TypeToString( Type type )
		{
			string typeStr = string.Empty;
			if( type.Name.Equals( EDataManager.LIST_TYPE_STRING ) || type.Name.Equals( EDataManager.DICT_TYPE_STRING ) )
			{
				typeStr += ( type.Name.Equals( EDataManager.LIST_TYPE_STRING ) ? EDataManager.BRACKET_LEFT_STRING : EDataManager.LESS_THAN_STRING );
				foreach( Type innerType in type.GetGenericArguments() )
				{
					typeStr += ( TypeToString( innerType ) + EDataManager.COMMA_STRING );
				}
				typeStr = typeStr.TrimEnd( EDataManager.COMMA_CHAR );
				typeStr += ( type.Name.Equals( EDataManager.LIST_TYPE_STRING ) ? EDataManager.BRACKET_RIGHT_STRING : EDataManager.GREAT_THAN_STRING );
			}
			else
			{
				typeStr += type.Name;
			}

			return typeStr;
		}

		public static string ObjectToString( object value )
		{
			string str = string.Empty;
			if( value is IList )
			{
				IList list = (IList)value;
				foreach( object o in list )
				{
					str = string.IsNullOrEmpty( str ) ? ObjectToString( o ) : str + EDataManager.COMMA_STRING + ObjectToString( o );
				}

				str = EDataManager.BRACKET_LEFT_STRING + str + EDataManager.BRACKET_RIGHT_STRING;
			}
			else if( value is IDictionary )
			{
				IDictionary dict = (IDictionary)value;
				foreach( DictionaryEntry pair in dict )
				{
					string key = ObjectToString( pair.Key );
					string val = ObjectToString( pair.Value );

					str = string.IsNullOrEmpty( str ) ? key + EDataManager.COLON_STRING + val : str + EDataManager.COMMA_STRING + key + EDataManager.COLON_STRING + val;
				}

				if( !string.IsNullOrEmpty( str ) ) str = EDataManager.LESS_THAN_STRING + str + EDataManager.GREAT_THAN_STRING;
			}
			else
			{
				str = ( null == value ) ? string.Empty : value.ToString();
			}

			return str;
		}
	}
}
