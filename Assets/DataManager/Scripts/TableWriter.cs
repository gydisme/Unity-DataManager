using System.Collections.Generic;

namespace DataManagement
{
	public abstract class TableWriter
	{
	#if UNITY_EDITOR
		public abstract void Write( string path, Table table );
	#endif
	}
}
