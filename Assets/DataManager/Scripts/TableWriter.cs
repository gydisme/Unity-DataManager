using System.Collections.Generic;

namespace DataManagement
{
	public abstract class TableWriter
	{
		public abstract void Write( string path, Table table );
	}
}
