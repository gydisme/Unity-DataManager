Unity-DataManager
=
The DataManager allows users to load ANY type of data to Unity 3D games, and the use is easy.

- Author: Gyd Tseng
- Email: kingterrygyd@gmail.com
- Twitter: @kingterrygyd
- Facebook: facebook.com/barbariangyd
- Donation: <a href='https://pledgie.com/campaigns/32250'><img alt='Click here to lend your support to: Unity-DataManager and make a donation at pledgie.com !' src='https://pledgie.com/campaigns/32250.png?skin_name=chrome' border='0' ></a>

Installation
=
Download the Assets/DataManager/ folder and add to your Unity 3D project.

QuickStart
=
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

Create Table for Unity (the step depends on the requirements of the reader )
```
1. Select the datasource files in Unity
2. Right-click on slected files
3. Choose DataManager/CreateTXT of Selected Files
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




Quickstart
----------
