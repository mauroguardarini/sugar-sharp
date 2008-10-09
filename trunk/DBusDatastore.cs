// DBusDatastore.cs created with MonoDevelop
// User: torello at 22:30 03/12/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using NDesk.DBus;
using org.freedesktop.DBus;
using System.Collections;
using System.Collections.Generic;

namespace Sugar
{
	
	public class DBusDatastore
	{
		static DataStoreInterface dataStoreInterfaceObj=null;
		static Bus bus=null;
		static string bus_name;
		static ObjectPath path;
		static bool dbusActive=false;
		Sugar.Window _window=null;
		
		public DBusDatastore()
		{
			string activityId="";
			
			if (dataStoreInterfaceObj!=null) {
				return;
			}

			try {
				bus = Bus.Session;
				bus_name = "org.laptop.sugar.DataStore";
				path = new ObjectPath ("/org/laptop/sugar/DataStore");

				if (bus.RequestName (bus_name) == RequestNameReply.PrimaryOwner) {

					System.Console.Out.WriteLine("Non found the service DataStore.");
					
				} else {
					IBus ibus = bus.GetObject<IBus> (bus_name, path);

					//import a remote to a local proxy
					dataStoreInterfaceObj = bus.GetObject<DataStoreInterface> (bus_name, path);
				}
			} catch (Exception ex) {
				System.Console.Out.WriteLine("Exception during the DBus  Datastore Instance handling: "+ex.Message);
				System.Console.Out.WriteLine("Stack: "+ex.StackTrace);
			}
		}
		
		public DataStoreInterface getDataStore() { return dataStoreInterfaceObj; }

		public delegate void datastoreCreatedEvent (string uid);
		public delegate void datastoreUpdatedEvent (string uid);
		public delegate void datastoreDeletedEvent (string uid);
		public delegate void datastoreMountedEvent (IDictionary<string,object> descriptor);
		public delegate void datastoreUmountedEvent (IDictionary<string,object> descriptor);
		public delegate void datastoreStoppedEvent ();
	}

	public struct findResult {
		public IDictionary<string,object>[] results;
		public uint counter;
	};
	
	[Interface ("org.laptop.sugar.DataStore")]
	public interface DataStoreInterface : Introspectable
	{
		IDictionary<string,object>[] mounts();
		void complete_indexing();
		IDictionary<string,object> get_properties(string uid);
		string create(IDictionary<string,object> props, string filelike, bool transfer_ownership);
		void update(string uid, IDictionary<string,object> props, string filelike, bool transfer_ownership);
		string get_filename(string uid);
		void delete(string uid);
		string mount(string uri, IDictionary<string,object> options); // <=== DA PROVARE
		void unmount(string mountpoint_id);
		string[] ids(string mountpoint);
		string[] get_uniquevaluesfor(object propertyname, IDictionary<string,object>query);
		findResult find(IDictionary<string,object> query, string[] properties);

		
		event DBusDatastore.datastoreCreatedEvent Created;
		event DBusDatastore.datastoreUpdatedEvent Updated;
		event DBusDatastore.datastoreDeletedEvent Deleted;
		event DBusDatastore.datastoreStoppedEvent Stopped;
		event DBusDatastore.datastoreMountedEvent Mounted;
		event DBusDatastore.datastoreUmountedEvent Unmounted;
		
	}

}
