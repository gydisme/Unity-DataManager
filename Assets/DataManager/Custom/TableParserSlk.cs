using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using DataManagement;

public class TableParserSlk : TableParser
{
	public override object Parse( object input )
	{
		try
		{
			byte[] bytes = Encoding.UTF8.GetBytes( (string)input );
			using( MemoryStream memory = new MemoryStream( bytes ) )
			{
				using( StreamReader readFile = new StreamReader( memory ) )
				{
					return _Parse( readFile );
				}
			}
		}
		catch
		{
			TableTools.Log( TableTools.LogLevel.ERROR, "read text asset have an exception:" + input );
		}

		return new List<List<string>>();
	}

	private List<List<string>> _Parse( StreamReader reader )
	{
		Dictionary<string,List<string[]>> _data = new Dictionary<string, List<string[]>>();
		string line = string.Empty;

		while( null != ( line = reader.ReadLine() ) )
		{
			string[] currentRecord = line.Split( ';' );
			if( currentRecord.Length > 0 )
			{
				List<string[]> temp;
				if( !_data.TryGetValue( currentRecord[0], out temp ) )
				{
					temp = new List<string[]>();
					_data[currentRecord[0]] = temp;
				}
					
				temp.Add( currentRecord );
			}
		}

		// init result
		List<List<string>> result = null;
		List<string[]> records;
		if( _data.TryGetValue( "B", out records ) )
		{
			result = ParseRecord_B( records[0] );
		}
		else
			return new List<List<string>>();

		if( _data.TryGetValue( "C", out records ) )
			ParseRecord_C( records, ref result );
		else
			return new List<List<string>>();

		return result;
	}

	private List<List<string>> ParseRecord_B( string[] fields )
	{
		List<List<string>> result = null;
		int columns = 0;
		int rows = 0;
		for( int i=1,imax=fields.Length;i<imax;i++ )
		{
			string field = fields[i];
			char fieldType = field[0];
			if( fieldType == 'X' )
			{
				columns = Convert.ToInt32( field.Substring( 1 ) );
			}
			else if( fieldType == 'Y' )
			{
				rows = Convert.ToInt32( field.Substring( 1 ) );
			}
		}

		result = new List<List<string>>();
		for( int i=0;i<rows;i++ )
		{
			List<string> row = new List<string>();
			for( int j=0;j<columns;j++ )
			{
				row.Add( string.Empty );
			}
			result.Add( row );
		}

		return result;
	}

	private void ParseRecord_C( List<string[]> records, ref List<List<string>> result )
	{
		int column = 1;
		int row = 1;
		foreach( string[] fields in records )
		{
			for( int i=1,imax=fields.Length;i<imax;i++ )
			{
				string field = fields[i];
				char fieldType = field[0];
				if( fieldType == 'X' )
				{
					column = Convert.ToInt32( field.Substring( 1 ) );
				}
				else if( fieldType == 'Y' )
				{
					row = Convert.ToInt32( field.Substring( 1 ) );
				}
				else if( fieldType == 'K' )
				{
					result[row-1][column-1] = field.Substring( 1 ).Trim( new char[] { '\"' } );
				}
			}
		}
	}
}
