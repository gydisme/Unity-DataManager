using System;
using System.Collections;

namespace DataManagement
{
	public abstract class TableConverter
	{
		public abstract Table Convert( string name, object input );
	}
}