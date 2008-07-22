using System;
using NDesk.DBus;
using org.freedesktop.DBus;

namespace Sugar
{
	
	public class DBusActivity
	{
		static OlpcDBusObject olpcDbusObj=null;
		static Bus bus=null;
		static string bus_name;
		static ObjectPath path;
		static bool dbusActive=false;
		Sugar.Window _window=null;
		Sugar.Activity _activity=null;

		public DBusActivity(Sugar.Activity activity) {			

			if (activity==null) return;
			
			_activity = activity;
System.Console.Out.WriteLine("Activity DBus ctor ActivityID: "+activity.activityId+"<<");
			createDBusLoopEvent(activity.activityId);
		}
		
		public DBusActivity(Sugar.Window window)
		{
			if (window==null) return;
			
			_window=window;
System.Console.Out.WriteLine("Window DBus ctor ActivityID: "+window.activityId+"<<");
			createDBusLoopEvent(window.activityId);
		}

		private void createDBusLoopEvent(string activityId) {
			
			if (olpcDbusObj!=null) {
				return;
			}

			try {
				if (activityId==null || activityId.Trim().Length<1) {
					activityId="123456789";
				}
				NDesk.DBus.BusG.Init ();
				
				bus = Bus.Session;

				bus_name = "org.laptop.Activity"+activityId;
				path = new ObjectPath ("/org/laptop/Activity/"+activityId);

				if (bus.RequestName (bus_name) == RequestNameReply.PrimaryOwner) {

					//create a new instance of the object to be exported
					if (_window!=null) {
						olpcDbusObj = new OlpcDBusObject (_window);
					} else if (_activity != null) {
						olpcDbusObj = new OlpcDBusObject (_activity);
					}
					bus.Register (path, olpcDbusObj);

				} else {
					//import a remote to a local proxy
					olpcDbusObj = bus.GetObject<OlpcDBusObject> (bus_name, path);
				}
			} catch (Exception ex) {

				System.Console.Out.WriteLine("Exception during the DBus Instance creation: "+ex.Message);
				System.Console.Out.WriteLine("Stack: "+ex.StackTrace);
			}
		}
		

		static bool isActivityRegistered(string activityId) {
			try {

				Connection conn;
			
				conn = Bus.Session;

//				ObjectPath opath = new ObjectPath ("/org/freedesktop/DBus");
//				string name = "org.freedesktop.DBus";
				ObjectPath opath = new ObjectPath ("/org/laptop/Activity");
				string name = "org.laptop.Activity";

					
				IBus bus = conn.GetObject<IBus> (name, opath);
				if (bus==null) {
					System.Console.Out.WriteLine("Return during the DBus process acquire.");
					return false;
				} else {
					if (bus.NameHasOwner("org/laptop/Activity"+activityId))
						return true;
					else
						return false;
				}
			
			} catch (Exception ex) {
				System.Console.Out.WriteLine("Exception in the DBus process acquire of an activity: "+ex.Message);
			}
			return false;
		}

	}
}

public interface IActivity {
	void SetActive(System.Boolean active);
	void Invite(object buddyKey);
}


[Interface ("org.laptop.Activity")]
public class OlpcDBusObject : MarshalByRefObject
{
	Sugar.Window _window=null;
	Sugar.Activity _activity=null;
	
	public OlpcDBusObject(Sugar.Window window)
	{
			_window=window;
	}
	
	public OlpcDBusObject(Sugar.Activity activity)
	{
			_activity=activity;
	}

	public void SetActive(System.Boolean active) {
		if (_window!=null) {
			// Try to generate some Sugar.Window Event
			_window.SetActive(active);
		} else if (_activity!=null) {
			_activity.SetActive(active);
		}
	}

	public void Invite(object buddyKey) {
		System.Console.WriteLine("Invite.....");
		System.Console.WriteLine("Invite: Passato il messaggio di tipo" + buddyKey.GetType().ToString() + " con valore " + buddyKey.ToString());
	}

}
