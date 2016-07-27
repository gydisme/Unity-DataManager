#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataManagement
{
	public static class TableTools
	{
		public enum LogLevel
		{
			DEFAULT,
			WARNING,
			ERROR
		}

		public static void Log( LogLevel level, string log )
		{
			#if UNITY_EDITOR
			switch( level )
			{
				case LogLevel.DEFAULT:
					Debug.Log( log );
					break;
				case LogLevel.WARNING:
					Debug.LogWarning( log );
					break;
				case LogLevel.ERROR:
					Debug.LogError( log );
					break;
			}
			#else
			log = level.ToString() + " - " + log;
			System.Console.WriteLine( log );
			#endif
		}
	}
}

