DataManager
=
The DataManager allows users to load ANY type of data, and it's easy to use. This project is created for Unity 3D games, but it could be used in any projects! Try it, and give me feedbacks :)

- Author: Gyd Tseng
- Email: kingterrygyd@gmail.com
- Twitter: @kingterrygyd
- Facebook: facebook.com/barbariangyd
- [![Donate via PayPal](https://img.shields.io/badge/Donate-PayPal-blue.svg)](https://paypal.me/gydisme)

Installation
=
Download the lastest release: https://github.com/gydisme/Unity-DataManager/releases
Start your project with the release or add Assets/DataManager/ folder to your project.

QuickStart
=
Supported tables
```
csv / slk
```

Use the DataManager in your project
```
1. using DataManagement;
2. Table table = DataManager.GetTable( tableName );
3. Data data = DataManager.GetData( tableName, key );
   Data data = table.GetData( key );
4. object value = table.GetValue( key, field );
   object value = data.GetValue( field );
   The value should be cast to your real type.
   For Example, string itemName = (string)value; or use:
   T value = table.GetValue<T>( key, field );
   T value = data.GetValue<T>( field );
   When the original type of value could be converted to the T type, it will be converted to T.
```

The Table requirements when initializing
```
1. Name of the table
2. Fields
3. Types
```

The tables for DataManager
```
1. Support System Types:String/Boolean/Single/Int32..etc.
2. Support Dictionary by set the type "<>", and List is "[]"
   For Example: "<String,Int32>" means Dictionary<string,int>
                "[Single]" means List<float>
3. Enum type is not supported for now ( it will be in the future ).
4. The first field will always be the key, the value of key should be Unique.
```

Create Table for Unity( the step depends on the requirements of the reader )
```
1. The supported text formats(TextAsset) of Unity 3D are:
   txt/html/htm/xml/bytes/json/csv/yaml/fnt
   (https://docs.unity3d.com/Manual/class-TextAsset.html)
   if your table format is not on the list, go 2, otherwise just add/copy it to Resource/Table/
2. Select the datasource files in Unity
3. Right-click on slected files
4. Choose DataManager/CreateTXT of Selected Files
```

Config the tables
```
1. Open the Data/TableConfig.csv
2. Add a new line of your table
   > name: the name of the table
   > path: the path of the table.
     For example, Table/ represent get the table from Assets/Resources/Table/.
   > reader: use which type of class to get the table.
     For example, TableReaderResource use Resource.Load to load the table.
   > parser: use which type of class to parse the data of table.
   > converter: use which converter of class to conver the parsed data to Table.
   > preload: load the table when DataManage on or not.
3. When the table is not list in TableConfig,
   It's default to use TableReaderResource/TableParserCsv/TableConverterList.
   And so is TableConfig itself.
```

Create your own reader/parser/converter
```
1. Create a class that inherit TableReader/TableParser.
2. Override the Get/Parse method.
3. If your data could be parsed to List<List<string>>,
   You could just use the TableConverterList to convert to Table.
```

Support
=
Email Me, Let's talk :)

Contributing
=
Your contribution will be licensed under the MIT license for this project.
