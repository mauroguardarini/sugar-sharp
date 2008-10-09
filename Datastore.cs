// Datastore.cs  
// User: torello at 08:46 31/08/2008
//

using System;
using NDesk.DBus;
using org.freedesktop.DBus;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace Sugar {

	public class DSMetadata {

		private Dictionary<string,object> _props=null;

		public DSMetadata() {
			initialize(null);
		}

		public DSMetadata(Dictionary<string,object> props) {
			initialize(props);
		}

		private void initialize(Dictionary<string,object> props) {

			String[] default_keys = { "activity", "activity_id", "mime_type", "title_set_by_user" };

			if (props==null) {
				_props=new Dictionary<string,object>();
			} else {
				_props=(Dictionary<string,object>)cloneProps(props);
			}

			IEnumerator en=default_keys.GetEnumerator();
			while (en.MoveNext()) {
				if (!_props.ContainsKey(en.Current.ToString())) {
					_props.Add(en.Current.ToString(),"");
				}
			}
		}

		public delegate void updateEvent (string key);
		event DSMetadata.updateEvent updated;

		public Object getItem(String key) {
			return _props[key];
		}

		public void setItem(String key, Object val) {
			if (!_props.ContainsKey(key) || _props[key] != val) {
				_props[key]=val;				
				if (updated!=null) {
					updated(key);
				}
			}
		}

		public void DelItem(String key) {
			_props.Remove(key);
		}


		public bool HasKey(String key) {
			return _props.ContainsKey(key);
		}

		public ICollection Keys() {
			return _props.Keys;
		}

		public Dictionary<String,object> GetDictionary() {
			return _props;
		}
    
		public DSMetadata Clone() {
			return new DSMetadata(_props);
		}

		public Dictionary<string,object> cloneProps(Dictionary<string,object> source) {
			String key=null;
			Dictionary<string,object> props=new Dictionary<string,object>();
			IEnumerator en=source.Keys.GetEnumerator();
			while (en.MoveNext()) {
				key=(String)en.Current;
				props[key]=source[key];
			}
			return props;
		}

		public Object get(String key, Object noValue) {
			if (_props.ContainsKey(key)) {
				return _props[key];
			} else {
				return noValue;
			}
		}

	}


	public class DSObject : IDisposable {
		private object _object_id;
		private DSMetadata _metadata;
		private object _file_path;
		private bool _destroyed;
		private bool _owns_file;
		private static DataStoreInterface dbus_helpers=null;

		public DSObject(object object_id) {
			Initialize(object_id, null, null);
		}

		public DSObject(object object_id, DSMetadata metadata) {
			Initialize(object_id, metadata, null);
		}

		public DSObject(object object_id, DSMetadata metadata, object file_path) {
			Initialize(object_id, metadata, file_path);
		}

		private void Initialize(object object_id, DSMetadata metadata, object file_path) {
		        this._object_id = object_id;
        		this._metadata = metadata;
		        this._file_path = file_path;
		        this._destroyed = false;
		        this._owns_file = false;

			if (dbus_helpers==null) {
				DBusDatastore datastore=new DBusDatastore();
				dbus_helpers=datastore.getDataStore();
			}
		}

		public object object_id {
			get {
				return _object_id;
			}
			set {
				_object_id=value;
			}
		}

		public DSMetadata Metadata { 
			get {
				if (_metadata==null && _object_id!=null) {
			            DSMetadata metadata = new DSMetadata((Dictionary<String, object>)dbus_helpers.get_properties(_object_id.ToString()));
			            this._metadata = metadata;
				}
				return _metadata;
			}
			set {
				if (_metadata != value) {
					_metadata=value;
				}
			}
		}

		public String FilePath {
			get {
				if (_object_id != null) {
					FilePath=dbus_helpers.get_filename(_object_id.ToString());
					_owns_file=true;
				}
				return _file_path.ToString();
			}
			set {
				if (_file_path==null || !_file_path.Equals(value)) {
					if (_file_path!=null && _owns_file) {
						if (File.Exists(_file_path.ToString())) {
							File.Delete(_file_path.ToString());
						}
						_owns_file=false;
					}
					_file_path=value;
				}
			}
		}

		private Object _get_activities_for_mime(String mime_type) {
/*
        registry = activity.get_registry()
        result = registry.get_activities_for_type(mime_type)
        if not result:
            for parent_mime in mime.get_mime_parents(mime_type):
                result.extend(registry.get_activities_for_type(parent_mime))
        return result
*/
			return null;
		}

		public Object GetActivities() {

			ArrayList activities=new ArrayList();

			Object bundle_id = Metadata.get("activity", "");
			if (bundle_id!=null) {

//				String activity_info = activity.get_registry().get_activity(bundle_id);
//				if (activity_info!=null) {
//					activities.Add(activity_info);
//				}

			}
/*
			String mime_type = Metadata.get("mime_type", "");
			if (mime_type!=null) {

				String activities_info = _get_activities_for_mime(mime_type);
				foreach (ActivityInfo element in activities_info) {
					if (element.bundle_id.Equals(bundle_id)) {
						activities.append(activity_info);
					}
				}

			}
*/
		        return activities;

		}



/*

    def is_activity_bundle(self):
        return self.metadata['mime_type'] in \
               [ActivityBundle.MIME_TYPE, ActivityBundle.DEPRECATED_MIME_TYPE]
*/

/*
		public bool IsContentBundle() {
			return (Metadata.get("mime_type") == ContentBundle.MIME_TYPE);
		}

		public bool IsBundle() {
			return (IsActivityBundle() || IsContentBundle());
		}

		public Resume() {
			return Resume(null);
		}
*/
/*
		public void Resume(String bundle_id) {
			if (IsActivityBundle() && bundle_id==null) {
				Logging.debug("Creating activity bundle");
				bundle = ActivityBundle(file_path);
				if (!bundle.IsInstalled()) {
			                Logging.debug("Installing activity bundle");
			                bundle.install();
				} else if (bundle.need_upgrade()) {
			                Logging.debug("Upgrading activity bundle");
					bundle.upgrade();
				}

		            	Logging.debug("activityfactory.creating bundle with id %r",bundle.get_bundle_id());
				ActivityFactory.create(bundle.get_bundle_id());

			} else if (IsContentBundle() && bundle_id==null) {
				Logging.debug("Creating content bundle");
				bundle = ContentBundle(file_path);
				if (!bundle.is_installed()) {
					Logging.debug("Installing content bundle");
			                bundle.install();
				}

				activities = _get_activities_for_mime("text/html");
				if (activities.length == 0) {
					Logging.warning("No activity can open HTML content bundles");
			                return;

				uri = bundle.get_start_uri();
				Logging.debug("activityfactory.creating with uri %s", uri);
				ActivityFactory.create_with_uri(activities[0].bundle_id,bundle.get_start_uri());

			} else {
				if (!get_activities() && bundle_id == null) {
					Logging.warning("No activity can open this object, %s.", Metadata.get("mime_type", null));
					return;
				}

				if (bundle_id == null) {
					bundle_id = get_activities()[0].bundle_id;
				}

				activity_id = Metadata['activity_id'];
				object_id = this.object_id;

				if (activity_id!=null) {
					handle = ActivityHandle(object_id,activity_id);	
					ActivityFactory.create(bundle_id, handle);
				} else {
					ActivityFactory.create_with_object_id(bundle_id, object_id);
				}
			}
		}
*/
		public void Dispose() {
			if (_destroyed) {
//				Logging.warning("This DSObject has already been destroyed!.");
				return;
			}
			_destroyed = true;
			if (_file_path!=null && _owns_file) {
				if (File.Exists(_file_path.ToString())) {
					File.Delete(_file_path.ToString());
				}
				_owns_file=false;
			}
			_file_path=null;
		}

		public DSObject Clone() {
			return new DSObject(null,_metadata.Clone(), _file_path);
		}

/*



    def destroy(self):
        if self._destroyed:
            logging.warning('This DSObject has already been destroyed!.')
            return
        self._destroyed = True
        if self._file_path and self._owns_file:
            if os.path.isfile(self._file_path):
                os.remove(self._file_path)
            self._owns_file = False
        self._file_path = None

    def __del__(self):
        if not self._destroyed:
            logging.warning('DSObject was deleted without cleaning up first. ' \
                            'Call DSObject.destroy() before disposing it.')
            self.destroy()

    def copy(self):
        return DSObject(None, self._metadata.copy(), self._file_path)
*/
	}

	public class Datastore {

		private static DataStoreInterface _dbus_helpers=null;

		private static DataStoreInterface dbus_helpers{
			get {
				if (_dbus_helpers==null) {
					DBusDatastore datastore=new DBusDatastore();
					_dbus_helpers=datastore.getDataStore();
				}
				return _dbus_helpers;
			}
		}

		static DSObject get(Object object_id) {
//			Loggin.debug("Datastore.get");
			Dictionary<String,object> metadata=(Dictionary<String,object>)dbus_helpers.get_properties(object_id.ToString());

			DSObject ds_object = new DSObject(object_id, new DSMetadata(metadata), null);

			// TODO in Python source code: register the object for updates
			return ds_object;
		}

		static public DSObject Create() {
			DSMetadata metadata = new DSMetadata();
			metadata.setItem("mtime",DateTime.Now.ToString("s"));
			metadata.setItem("timestamp", DateTime.Now.Ticks);

			return new DSObject(null, metadata, null);
		}

		static public void write(DSObject ds_object) {
			_write(ds_object, true, false, null, null, -1);
		}

		static public void write(DSObject ds_object,bool update_mtime) {
			_write(ds_object, update_mtime, false, null, null, -1);
		}

		static public void write(DSObject ds_object,bool update_mtime, bool transfer_ownership) {
			_write(ds_object, update_mtime, transfer_ownership, null, null, -1);
		}

		static public void write(DSObject ds_object,bool update_mtime, bool transfer_ownership, Object reply_handler) {
			_write(ds_object, update_mtime, transfer_ownership, reply_handler, null, -1);
		}

		static public void write(DSObject ds_object,bool update_mtime, bool transfer_ownership, Object reply_handler, Object error_handler) {
			_write(ds_object, update_mtime, transfer_ownership, reply_handler, error_handler, -1);
		}

		static public void write(DSObject ds_object,bool update_mtime, bool transfer_ownership, Object reply_handler, Object error_handler, int timeout) {
			_write(ds_object, update_mtime, transfer_ownership, reply_handler, error_handler, timeout);
		}

		static private void _write(DSObject ds_object, bool update_mtime, bool transfer_ownership, Object reply_handler, Object error_handler, int timeout) {
//			Logging.debug("datastore.write");
			String file_path;

//			Dictionary<String,object> properties = (Dictionary<String,object>)ds_object.Metadata.GetDictionary().Clone();
			Dictionary<String,object> properties = (Dictionary<String,object>)ds_object.Metadata.GetDictionary();

			if (update_mtime) {
				properties["mtime"] = DateTime.Now.ToString("s");
				properties["timestamp"] = DateTime.Now.Ticks;
			}

			if (ds_object.FilePath==null) {
				file_path = "";
			} else {
				file_path = ds_object.FilePath;
			}


			// FIXME in Python source code: this func will be sync for creates regardless of the handlers
			// supplied. This is very bad API, need to decide what to do here.
			if (ds_object.object_id!=null) {

				// Note: the reply_handler, error_handler and timeout is not handled by DBus interface. We need to implement in other manner
//				dbus_helpers.update(ds_object.object_id, properties, file_path, transfer_ownership, reply_handler=reply_handler,  error_handler=error_handler, timeout=timeout);
				dbus_helpers.update(ds_object.object_id.ToString(), properties, file_path, transfer_ownership);

			} else {
				if (reply_handler!=null || error_handler != null) {
//					Logging.warning("Datastore.write() cannot currently be called' async for creates, see ticket 3071");
				} else {
//					ds_object.object_id = dbus_helpers.create(properties, file_path, transfer_ownership);
					ds_object.object_id = dbus_helpers.create(properties, file_path, transfer_ownership);
				}
			}

			// TODO in Python source code: register the object for updates
//			Logging.debug("Written object %s to the datastore.", ds_object.object_id);

		}

		static void delete(Object object_id) {
//			Logging.debug("datastore.delete");
			dbus_helpers.delete(object_id.ToString());
		}

		static public ArrayList find(Dictionary<String,object> query) {
			return _find(query, null, null, null, null);
		}

		static public ArrayList find(Dictionary<String,object> query, object sorting) {
			return _find(query, sorting, null, null, null);
		}

		static public ArrayList find(Dictionary<String,object> query, object sorting, object limit) {
			return _find(query, sorting, limit, null, null);
		}

		static public ArrayList find(Dictionary<String,object> query, object sorting, object limit, object offset) {
			return _find(query, sorting, limit, offset, null);
		}

		static public ArrayList find(Dictionary<String,object> query, object sorting, object limit, object offset, String[] properties) {
			return _find(query, sorting, limit, offset, properties);
		}

		static private ArrayList _find(Dictionary<String,object> query, Object sorting, object limit, object offset, String[] properties) {

//		    query = query.copy()

  			if (properties==null) {
				properties = new String[0];
			}

			if (sorting!=null) {
				query["order_by"] = sorting;
			}
			if (limit!=null) {
				query["limit"] = limit;
			}
			if (offset!=null) {
				query["offset"] = offset;
			}
    
			findResult result= dbus_helpers.find(query, properties);

			// Note: the reply_handler, error_handler and timeout is not handled by DBus interface. We need to implement in other manner
//			props_list, total_count = dbus_helpers.find(query, properties, reply_handler, error_handler)    

			ArrayList objects=new ArrayList();
			int i=0;
			foreach (Dictionary<string,object> props in result.results) {
				object object_id=props["uid"];
//			        del props['uid']

				DSObject ds_object=new DSObject(object_id, new DSMetadata(props));
				objects.Add(ds_object);
				++i;
			}

			return objects;
		}


		public static void copy(DSObject jobject, String mount_point) {

			DSObject new_jobject = jobject.Clone();
			new_jobject.Metadata.setItem("mountpoint",mount_point);

			if (jobject.Metadata.HasKey("title")) {
				String filename = jobject.Metadata.getItem("title").ToString();

				if (jobject.Metadata.HasKey("mime_type")) {
					object mime_type = jobject.Metadata.getItem("mime_type");
//					extension = mime.get_primary_extension(mime_type);
//					if extension:
//						filename += '.' + extension
				}

        			new_jobject.Metadata.setItem("suggested_filename",filename);
			}

			// this will cause the file be retrieved from the DS
			new_jobject.FilePath = jobject.FilePath;

			write(new_jobject);
		}

		public static string mount(string uri, IDictionary<string,object> options) {
			// Note: the timeout attribute is not handled by DBus interface. We need to implement in other manner
			//			return dbus_helpers.mount(uri, options, timeout=timeout);
			return dbus_helpers.mount(uri, options);
		}

		public static void unmount(string mount_point_id) {
			dbus_helpers.unmount(mount_point_id);
		}

		public static IDictionary<string,object>[] mounts() {
			return dbus_helpers.mounts();
		}

		public static void complete_indexing() {
			// This method need to have a return value, we need check on dbus_helper
			dbus_helpers.complete_indexing();
		}

		public static object get_unique_values(string key) {
			// I not find the method on the dbus structure
//			return dbus_helpers.get_unique_values(key);
			return null;
		}
	}
}
