using System.IO;
using System.Text;
using System.Collections.Generic;
using DataManagement;

public class TableWriterCsv : TableWriter
{
	public override void Write( string path, Table table )
	{
		StringBuilder builder = new StringBuilder();

		string str = string.Empty;
		foreach( string value in table.Fields )
			AppendValue( value, ref str );
		builder.AppendLine( str );

		str = string.Empty;
		foreach( string value in table.Types )
			AppendValue( value, ref str );
		builder.AppendLine( str );

		foreach( object key in table.Keys )
		{
			str = string.Empty;
			for( int i = 0; i < table.Fields.Count; ++i )
			{
				string value = DataManagerTools.ObjectToString( table.GetValue( key, table.Fields[i] ) );
				if( 0 <= value.IndexOf( EDataManager.BRACKET_LEFT_STRING ) || 0 <= value.IndexOf( EDataManager.LESS_THAN_STRING ) )
				{
//					Debug.Log( value );
					value = value.Substring( 1, value.Length - 2 );
				}
					
				AppendValue( value, ref str );
			}

			builder.AppendLine( str );
		}

		File.WriteAllText( path, builder.ToString(), Encoding.Default );
	}

	private void AppendValue( string value, ref string str )
	{
		value = value.Contains( EDataManager.COMMA_STRING ) ? EDataManager.DOUBLE_QUOTATION_MARKS_STRING + value + EDataManager.DOUBLE_QUOTATION_MARKS_STRING : value;
		str = string.IsNullOrEmpty( str ) ? value : str + EDataManager.COMMA_STRING + value;
	}
}