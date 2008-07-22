using System;
using NDesk.DBus;
using org.freedesktop.DBus;
using System.Runtime.InteropServices;

namespace Sugar
{
	
	
	public class Activity
	{
		string _activityId, _bundleId;
		bool _state=false;
		Gtk.Window _wnd=null;
		
		public Activity(Gtk.Window form, string activityId, string bundleId)
		{
			_activityId=activityId;
			_bundleId=bundleId;
			_wnd=form;
			
			DBusActivity dbus=new DBusActivity(this);
			
			if (form.IsRealized) {
				System.Console.Out.WriteLine("Error, the Window is already Realized so it is possible that the form is not sugarized.");
			}
			_wnd.Realized += realizeEventHandler;
		}

		public string activityId { get { return _activityId; } }
		public string bundleId   { get { return _bundleId; } }

		public delegate void SetActiveHandler (bool setActiveState);

		public event SetActiveHandler SetActiveEvent;

		public void SetActive(bool active) {
			if (SetActiveEvent==null) 
				return;
			_state=active;
			SetActiveEvent(active);
		}
		
		public void realizeEventHandler(object sender, EventArgs e) {
			System.Console.Out.WriteLine("Dove sono: "+System.Environment.CurrentDirectory);
//			Window window=(Window)sender;

			sugarX11UtilSetStringProperty(_wnd.GdkWindow,"_SUGAR_ACTIVITY_ID", _activityId);
			sugarX11UtilSetStringProperty(_wnd.GdkWindow,"_SUGAR_BUNDLE_ID", _bundleId);
		}
			
		
		static private void sugarX11UtilSetStringProperty(Gdk.Window window, string property,string value) {
			sugar_x11_util_set_string_property(window.Handle,property,value);
			return;
		}
		
		const string libUISugar = "./bin/uiX11Util.so";
		[DllImport (libUISugar)]
		static public extern void sugar_x11_util_set_string_property(IntPtr window,string property,string value);
	}
	
}
