using System.Collections.Generic;

namespace DataManagement
{
	public abstract class TableParser
	{
		public abstract object Parse( object input );
	}
}