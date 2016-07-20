using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using DataManagement;

public class TableParserCsv : TableParser
{
	public override object Parse( object input )
	{
		List<List<string>> result = new List<List<string>>();
		try
		{
			using( StringReader readFile = new StringReader( (string)input ) )
			{
				result = _Parse( readFile );
			}
		}
		catch
		{
			Debug.LogError( "read text asset have an exception:" + input );
		}

		return result;
	}

	private List<List<string>> _Parse( TextReader readFile )
	{
		List<List<string>> result = new List<List<string>>();
		List<string> lines;

		string line = string.Empty;
		bool inQuato = false;
		bool addQuato = false;
		string value = string.Empty;

		while( null != ( line = readFile.ReadLine() ) )
		{
			lines = new List<string>();
			value = string.Empty;

			foreach( char c in line )
			{
				if( addQuato && c.Equals( EDataManager.DOUBLE_QUOTATION_MARKS_CHAR ) ) { addQuato = false; }
				else if( addQuato && !c.Equals( EDataManager.DOUBLE_QUOTATION_MARKS_CHAR ) ) { addQuato = false; inQuato = false; }
				else if( inQuato && c.Equals( EDataManager.DOUBLE_QUOTATION_MARKS_CHAR ) ) { addQuato = true; continue; }
				else if( c.Equals( EDataManager.DOUBLE_QUOTATION_MARKS_CHAR ) ) { inQuato = true; continue; }

				if( !inQuato && c.Equals( EDataManager.COMMA_CHAR ) ) { lines.Add( value ); value = string.Empty; continue; }

				value += c.ToString();
			}

			lines.Add( value );
			result.Add( lines );
		}

		return result;
	}
}
