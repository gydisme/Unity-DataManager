using UnityEngine;
using System;
using DataManagement;

public class TableReaderResource : TableReader
{
	public override object Get( string path )
	{
		if( string.IsNullOrEmpty( path ) )
			return null;

		TextAsset result = Resources.Load( path ) as TextAsset;
		return ( null == result ) ? string.Empty : result.text;
	}
}
