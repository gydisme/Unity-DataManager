using System;

namespace DataManagement
{
	public abstract class TableReader
	{
		public abstract object Get( string path );
	}
}
