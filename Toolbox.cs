// Toolbox.cs created with MonoDevelop
// User: torello at 12:54 09/10/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Gtk;

namespace Sugar
{
	public class Toolbox : Gtk.VBox
	{

		private Gtk.Notebook _notebook;
		
		public Toolbox()
		{
			_notebook=new Notebook();
			_notebook.TabPos=Gtk.PositionType.Bottom;
			_notebook.TabBorder=0;
			_notebook.ShowTabs=false;
			_notebook.TabVborder=Sugar.Style.TOOLBOX_TAB_VBORDER;
			_notebook.TabHborder=Sugar.Style.TOOLBOX_TAB_VBORDER;
			PackStart(_notebook);
			_notebook.Show();
			
			// TODO:
			// Creare il package hippo e importare questa routine.
/*			
        # FIXME improve gtk.Notebook and do this in the theme
        self._separator = hippo.Canvas()
        box = hippo.CanvasBox(
                    border_color=style.COLOR_BUTTON_GREY.get_int(),
                    background_color=style.COLOR_PANEL_GREY.get_int(),
                    box_height=style.TOOLBOX_SEPARATOR_HEIGHT,
                    border_bottom=style.LINE_WIDTH)
        self._separator.set_root(box)
        self.pack_start(self._separator, False)

        self._notebook.connect('notify::page', self._notify_page_cb)
*/
		}
		
		private void _notify_page_cb(object notebook, EventArgs pspec) {
			// self.emit('current-toolbar-changed', notebook.props.page)
		}

		private void _toolbar_box_expose_cb(object widget, EventArgs ev) {
			Toolbox tb=(Toolbox)widget;

			/*
        widget.style.paint_flat_box(widget.window,
                                    gtk.STATE_NORMAL, gtk.SHADOW_NONE,
                                    event.area, widget, 'toolbox',
                                    widget.allocation.x,
                                    widget.allocation.y,
                                    widget.allocation.width,
                                    widget.allocation.height)
			 */
		}

		public void add_toolbar(string name, Toolbar toolbar) {
			Gtk.Label label = new Label(name);
			label.SetSizeRequest((int)Sugar.Style.TOOLBOX_TAB_LABEL_WIDTH, -1);
			label.SetAlignment(0.0f, 0.5f);
			
			Gtk.HBox toolbar_box = new HBox();
			toolbar_box.PackStart(toolbar,true, true, Sugar.Style.TOOLBOX_HORIZONTAL_PADDING);

			toolbar_box.ExposeEvent += _toolbar_box_expose_cb;
			_notebook.AppendPage(toolbar_box, label);
			toolbar_box.Show();

			if (_notebook.NPages>1) {
				_notebook.ShowTabs=true;
//				_separator.Show();
			}
		}

		public void remove_toolbar(int index) {
			_notebook.RemovePage(index);
			if (_notebook.NPages<2) {				
				_notebook.ShowTabs=false;
//				_separator.Hide();
			}
		}

		public int set_current_toolbar { set { _notebook.CurrentPage=value; } }
	}
}
