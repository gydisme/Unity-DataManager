using System.Collections.Generic;

namespace DataManagement
{
	public static class EDataManager
	{
		#region Common Definition
		public static readonly string DEFAULT_TABLE_PATH = "";
		public static readonly string TABLE_CONFIG = "TableConfig";
		public static readonly string DEFAULT_READER = "TableReaderResource";
		public static readonly string DEFAULT_PARSER = "TableParserCsv";
		public static readonly string DEFAULT_CONVERTER = "TableConverterList";
		#endregion

		#region Column Name
		public static readonly string NAME = "name";
		public static readonly string PATH = "path";
		public static readonly string PARSER = "parser";
		public static readonly string READER = "reader";
		public static readonly string CONVERTER= "converter";
		public static readonly string PRELOAD = "preload";
		#endregion

		#region Punctuation Definition (String)
		public static readonly string BRACKET_LEFT_STRING = "[";
		public static readonly string BRACKET_RIGHT_STRING = "]";
		public static readonly string LESS_THAN_STRING = "<";
		public static readonly string GREAT_THAN_STRING = ">";
		public static readonly string COMMA_STRING = ",";
		public static readonly string COLON_STRING = ":";
		public static readonly string DOUBLE_QUOTATION_MARKS_STRING = "\"";
		public static readonly string SINGLE_QUOTATION_MARKS_STRING = "'";
		#endregion

		#region Punctuation Definition (Char)
		public static readonly char BRACKET_LEFT_CHAR = '[';
		public static readonly char BRACKET_RIGHT_CHAR = ']';
		public static readonly char LESS_THAN_CHAR = '<';
		public static readonly char GREAT_THAN_CHAR = '>';
		public static readonly char COMMA_CHAR = ',';
		public static readonly char UNDER_LINE_CHAR = '_';
		public static readonly char COLON_CHAR = ':';
		public static readonly char DOUBLE_QUOTATION_MARKS_CHAR = '"';
		#endregion
		
		#region Type String Definition
		public static readonly string SYSTEM_DOT_STRING = "System.";
		public static readonly string NAMESPACE_STRING = "System.Collections.Generic.";
		public static readonly string LIST_TYPE_STRING = "List`1";
		public static readonly string DICT_TYPE_STRING = "Dictionary`2";
		#endregion
	}
}
