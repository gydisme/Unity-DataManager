#if UNITY_EDITOR
using UnityEngine;
#endif
using System;
using DataManagement;

public class TableReaderResource : TableReader
{
	public override object Get( string path )
	{
		if( string.IsNullOrEmpty( path ) )
			return null;
		
#if UNITY_EDITOR
		TextAsset result = Resources.Load( path ) as TextAsset;
		return ( null == result ) ? string.Empty : result.text;
#else
		// todo
		return null;
#endif

	}
}
