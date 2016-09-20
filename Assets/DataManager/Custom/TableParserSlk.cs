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

	int _x = 1;
	int _y = 1;
	int _xMax = 0;
	int _yMax = 0;
	delegate void _recordParserDelegate( string[] fields, ref List<List<string>> result );
	static Dictionary<string,_recordParserDelegate> _recordParser;

	private List<List<string>> _Parse( StreamReader reader )
	{
		if( _recordParser == null )
		{
			_recordParser = new Dictionary<string, _recordParserDelegate>();
			_recordParser["B"] = ParseRecord_B;
			_recordParser["C"] = ParseRecord_C;
			_recordParser["F"] = ParseRecord_F;
		}

		string line = string.Empty;
		List<List<string>> result = new List<List<string>>();

		while( null != ( line = reader.ReadLine() ) )
		{
			string[] currentRecord = line.Split( ';' );
			if( currentRecord.Length > 0 )
			{
				string recordType = currentRecord[0];
				_recordParserDelegate d;
				if( _recordParser.TryGetValue( recordType, out d ) )
					d( currentRecord, ref result );
			}
		}
		return result;
	}

	private void ParseRecord_B( string[] record, ref List<List<string>> result )
	{
		for( int i=1,imax=record.Length;i<imax;i++ )
		{
			string field = record[i];
			char fieldType = field[0];
			if( fieldType == 'X' )
			{
				_xMax = Convert.ToInt32( field.Substring( 1 ) );
			}
			else if( fieldType == 'Y' )
			{
				_yMax = Convert.ToInt32( field.Substring( 1 ) );
			}
		}

		result = new List<List<string>>();
		if( _xMax > 0 && _yMax > 0 )
		{
			for( int i=0;i<_yMax;i++ )
			{
				List<string> row = new List<string>();
				for( int j=0;j<_xMax;j++ )
				{
					row.Add( string.Empty );
				}
				result.Add( row );
			}
		}
	}

	private void ParseRecord_C( string[] record, ref List<List<string>> result )
	{
		for( int i=1,imax=record.Length;i<imax;i++ )
		{
			string field = record[i];
			char fieldType = field[0];
			if( fieldType == 'X' )
			{
				_x = Convert.ToInt32( field.Substring( 1 ) );
			}
			else if( fieldType == 'Y' )
			{
				_y = Convert.ToInt32( field.Substring( 1 ) );
			}
			else if( fieldType == 'K' )
			{
				result[_y-1][_x-1] = field.Substring( 1 ).Trim( new char[] { '\"' } );
			}
		}
	}

	private void ParseRecord_F( string[] record, ref List<List<string>> result )
	{
		for( int i=1,imax=record.Length;i<imax;i++ )
		{
			string field = record[i];
			char fieldType = field[0];
			if( fieldType == 'X' )
			{
				_x = Convert.ToInt32( field.Substring( 1 ) );
			}
			else if( fieldType == 'Y' )
			{
				_y = Convert.ToInt32( field.Substring( 1 ) );
			}
		}
	}
}
