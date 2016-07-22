#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

#if UNITY_EDITOR
namespace DataManagement
{
	public static class TableEditorTools
	{
		[MenuItem("Assets/DataManager/CreateTXT of Selected Files", false, 1)]
		public static void ConvterSelectedFiles()
		{
			Object[] objs = Selection.objects;
			foreach( Object obj in objs )
			{
				string path = AssetDatabase.GetAssetPath( obj );
				ConvertFile( path );
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static void ConvertSingleFile( string path )
		{

			ConvertFile( path );

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private static void ConvertFile( string path )
		{
			string toPath = Application.dataPath + "/Resources/Table/" + Path.GetFileNameWithoutExtension( path ) + ".txt";
			Directory.CreateDirectory( Path.GetDirectoryName( toPath ) );

			string tempFile = Application.dataPath + "~tableConverter.tmp";
			File.Copy( path, tempFile, true );

			byte[] data = Encoding.Convert( Encoding.Default, Encoding.UTF8, File.ReadAllBytes( tempFile ) );
			File.WriteAllBytes( toPath, data );

			File.Delete( tempFile );
		}
	}
}
#endif