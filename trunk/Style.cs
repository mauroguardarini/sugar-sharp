// Style.cs created with MonoDevelop
// User: torello at 13:01 09/10/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Gtk;
using Pango;

namespace Sugar
{
	public class Style
	{
		
		static public float _XO_DPI = 200.0f;
		static public int _FOCUS_LINE_WIDTH = 2;
		static public int _TAB_CURVATURE = 1;

		public Style() {
		}

		public static float _get_screen_dpi() {

			Gtk.Settings setting=Gtk.Settings.Default;
				
			// TODO:
			//   I must read this from property
			//
			// xft_dpi = gtk.settings_get_default().get_property('gtk-xft-dpi')
			float xft_dpi=200;
			return (xft_dpi / 1024);
		}

		public static float _compute_zoom_factor() {
			// TODO:
			//   I must read the os environment
			//
			//if (_get_screen_dpi() == 96.0) {
			//	if not os.environ.has_key('SUGAR_XO_STYLE') or \
			//		not os.environ['SUGAR_XO_STYLE'] == 'yes':
			//			return 0.72
			//
			//return 1.0
			return 0.72f;
		}
			
		public static uint _compute_font_height(Pango.FontDescription font) {
			Label widget=new Gtk.Label("");
				
			Pango.Context context=widget.PangoContext;
			Pango.Font pangoFont=context.LoadFont(font);
			Pango.FontMetrics metrics=pangoFont.GetMetrics(null);

			return ((uint)(metrics.Ascent+metrics.Descent));
		}

		public static uint zoom(double units) {
			return (uint)(ZOOM_FACTOR * units);
		}

		public static float ZOOM_FACTOR = _compute_zoom_factor();

		public static uint DEFAULT_SPACING = zoom(8);
		public static uint DEFAULT_PADDING = zoom(6);
		public static uint GRID_CELL_SIZE = zoom(75);
		public static uint LINE_WIDTH = zoom(2);

		public static uint STANDARD_ICON_SIZE = zoom(55);
		public static uint SMALL_ICON_SIZE = zoom(55 * 0.5);
		public static uint MEDIUM_ICON_SIZE = zoom(55 * 1.5);
		public static uint LARGE_ICON_SIZE = zoom(55 * 2.0);
		public static uint XLARGE_ICON_SIZE = zoom(55 * 2.75);

		public static uint FONT_SIZE = zoom(7 * _XO_DPI / _get_screen_dpi());
		public static Sugar.Font FONT_NORMAL = new Sugar.Font("Bitstream Vera Sans "+FONT_SIZE);
		public static Sugar.Font FONT_BOLD = new Sugar.Font("Bitstream Vera Sans bold "+FONT_SIZE);
		public static uint FONT_NORMAL_H = _compute_font_height(FONT_NORMAL.get_pango_desc());
		public static uint FONT_BOLD_H = _compute_font_height(FONT_BOLD.get_pango_desc());

		public static uint TOOLBOX_SEPARATOR_HEIGHT = zoom(9);
		public static uint TOOLBOX_HORIZONTAL_PADDING = zoom(75);
		public static uint TOOLBOX_TAB_VBORDER = (uint)((zoom(36) - FONT_NORMAL_H - _FOCUS_LINE_WIDTH) / 2);
		public static uint TOOLBOX_TAB_HBORDER = (uint)(zoom(15) - _FOCUS_LINE_WIDTH - _TAB_CURVATURE);
		public static uint TOOLBOX_TAB_LABEL_WIDTH = zoom(150 - 15 * 2);

		public static Sugar.Color COLOR_BLACK = new Color("#000000");
		public static Sugar.Color COLOR_WHITE = new Color("#FFFFFF");
		public static Sugar.Color COLOR_TRANSPARENT = new Color("#FFFFFF", 0.0f);
		public static Sugar.Color COLOR_PANEL_GREY = new Color("#C0C0C0");
		public static Sugar.Color COLOR_SELECTION_GREY = new Color("#A6A6A6");
		public static Sugar.Color COLOR_TOOLBAR_GREY = new Color("#404040");
		public static Sugar.Color COLOR_BUTTON_GREY = new Color("#808080");
		public static Sugar.Color COLOR_INACTIVE_FILL = new Color("#9D9FA1");
		public static Sugar.Color COLOR_INACTIVE_STROKE = new Color("#757575");
		public static Sugar.Color COLOR_TEXT_FIELD_GREY = new Color("#E5E5E5");
	}
	
	
	public class Font {
		private string _desc;
		public Font(string desc) {
			_desc=desc;
		}
		
		public string ToString() {
			return _desc;
		}

		public Pango.FontDescription get_pango_desc() {
			return Pango.FontDescription.FromString(_desc);
		}
	}

	public class Color {
		float _r, _g, _b, _a;
		
		public Color(string rgb) {
			fieldInitializer(rgb,1.0f);
		}

		public Color(string rgb, float alpha) {
			fieldInitializer(rgb,alpha);
		}
		
		private void fieldInitializer(string rgb, float alpha) {
			float[] _rgb=_html_to_rgb(rgb);
			_r=_rgb[0];
			_g=_rgb[1];
			_b=_rgb[2];
			_a=alpha;
		}
		
		public float[] get_rgba() {
			return new float[] {_r, _g, _b, _a};
		}
		
		public int  get_int() {
			return (int)(_a * 255) + ((int)(_b * 255) << 8) + ((int)(_g * 255) << 16) + ((int)(_r * 255) << 24);
		}
		
		public Gdk.Color get_gdk_color() {
			return new Gdk.Color((byte)(_r * 65535), (byte)(_g * 65535), (byte)(_b * 65535));
		}

		public string get_html() {
//			return '#%02x%02x%02x' % (self._r * 255, self._g * 255, self._b * 255)
//			string txt="#%02x%02x%02x";
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.AppendFormat("#{2x0}{2x1}{2x2}",(int)(_r*255),(int)(_g*255),(int)(_b*255));
			return sb.ToString();
		}

		private float[] _html_to_rgb(string html_color) {

			string[] colours=new string[3];
			html_color = html_color.Trim();
			if (html_color[0] == '#') {
				html_color = html_color.Substring(1);
			}
			if (html_color.Length != 6) {
//				raise ValueError, "input #%s is not in #RRGGBB format" % html_color
			}

			colours[0]=html_color.Substring(0,2);
			colours[1]=html_color.Substring(2,2);
			colours[2]=html_color.Substring(4,2);
			
			return new float[] { float.Parse(colours[0], System.Globalization.NumberStyles.HexNumber)/255,
				                   float.Parse(colours[1], System.Globalization.NumberStyles.HexNumber)/255,
			                       float.Parse(colours[2], System.Globalization.NumberStyles.HexNumber)/255
                                 };
		}
					
		public string get_svg() {
			if (_a == 0.0) {
				return "none";
			} else {
				return get_html();
			}
		}
	}
}
