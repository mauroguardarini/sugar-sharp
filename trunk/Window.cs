using System;
using Gdk;
using System.Runtime.InteropServices;
using Gtk;

namespace Sugar
{
	public class Window : Gtk.Window
	{
		string _activityId, _bundleId;
		bool _state=false;
		
		public Window(string title, string activityId, string bundleId) : base (title)
		{
			_activityId=activityId;
			_bundleId=bundleId;
			
			DBusActivity dbus=new DBusActivity(this);
			
			this.Realized += realizeEventHandler;
			
		}

		public string activityId { get { return _activityId; } }
		public string bundleId   { get { return _bundleId; } }

		public delegate void SetActiveHandler (bool setActiveState);

		public event SetActiveHandler SetActiveEvent;

		public void SetActive(bool active) {
			if (SetActiveEvent==null) 
				return;
			
			SetActiveEvent(active);
		}
		
		public void realizeEventHandler(object sender, EventArgs e) {
			System.Console.Out.WriteLine("Dove sono: "+System.Environment.CurrentDirectory);
			Window window=(Window)sender;

			sugarX11UtilSetStringProperty(window.GdkWindow,"_SUGAR_ACTIVITY_ID", _activityId);
			sugarX11UtilSetStringProperty(window.GdkWindow,"_SUGAR_BUNDLE_ID", _bundleId);
		}
			
		
		static private void sugarX11UtilSetStringProperty(Gdk.Window window, string property,string value) {
			sugar_x11_util_set_string_property(window.Handle,property,value);
			return;
		}
		
		public byte[] getScreenShot() {
			int width, height;
			this.GetSize(out width, out height);
			Gdk.Pixbuf screenShot=new Gdk.Pixbuf(Gdk.Colorspace.Rgb, false, 8, width, height);
			screenShot=screenShot.CreateFromDrawable (this.GdkWindow, this.GdkWindow.Colormap, 0, 0, 0, 0, width, height);
			screenShot.ScaleSimple(100,80,Gdk.InterpType.Bilinear);
			
			return screenShot.SaveToBuffer("png");			
		}
		
		const string libUISugar = "./bin/uiX11Util.so";
		[DllImport (libUISugar)]
		static public extern void sugar_x11_util_set_string_property(IntPtr window,string property,string value);
	}
}
