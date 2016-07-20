using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DataManagement
{
	public class LoadResourceAsync : MonoBehaviour
	{
		public float Progress { get;private set; }

		System.Action<UnityEngine.Object> _cb = null;
		ResourceRequest _request;

		void OnInit( string path, System.Action<UnityEngine.Object> cb )	
		{
			if( string.IsNullOrEmpty( path ) )
			{
				Debug.LogError( "try to load resource with empty path" );
				if( cb != null )
					cb( null );
				return;
			}

			_cb = cb;
			Progress = 0;
			_Get ( path );
		}


		void _Get( string path )
		{
			_request = Resources.LoadAsync( path );
			if( _request == null )
			{
				Debug.LogError( "failed to start load resource async:" + path );
				if( _cb != null )
					_cb( null );
				Destroy( gameObject );
			}
		}

		void Update()
		{
			if( _request != null )
			{
				Progress = _request.progress;

				if( _request.isDone )
				{
					if( _cb != null )
						_cb( _request.asset );
					
					_request = null;
					Destroy( gameObject );
				}
			}
		}

		static public void Get( string path, System.Action<UnityEngine.Object> cb )
		{
			GameObject obj = new GameObject( "LoadResourceAsync:" + path );
			LoadResourceAsync load = obj.AddComponent<LoadResourceAsync>();
			load.OnInit( path, cb );
		}
	}
}