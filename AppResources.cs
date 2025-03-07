using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;

namespace ActivAndZen;

public partial class App {
	public static class Icons {
		public static DrawingBrush ArrowToLeft = (DrawingBrush)Application.Current.Resources["db_ArrowToLeftIcon"];
		public static DrawingBrush ArrowToRight = (DrawingBrush)Application.Current.Resources["db_ArrowToRightIcon"];
		public static DrawingBrush Bin = (DrawingBrush)Application.Current.Resources["db_BinIcon"];
		public static DrawingBrush Building = (DrawingBrush)Application.Current.Resources["db_BuildingIcon"];
		public static DrawingBrush CheckboxNo = (DrawingBrush)Application.Current.Resources["db_CheckboxNoIcon"];
		public static DrawingBrush CheckboxYes = (DrawingBrush)Application.Current.Resources["db_CheckboxYesIcon"];
		public static DrawingBrush Copy = (DrawingBrush)Application.Current.Resources["db_CopyIcon"];
		public static DrawingBrush Cross = (DrawingBrush)Application.Current.Resources["db_CrossIcon"];
		public static DrawingBrush Dots = (DrawingBrush)Application.Current.Resources["db_DotsIcon"];
		public static DrawingBrush Email = (DrawingBrush)Application.Current.Resources["db_EmailIcon"];
		public static DrawingBrush File = (DrawingBrush)Application.Current.Resources["db_FileIcon"];
		public static DrawingBrush ListOpen = (DrawingBrush)Application.Current.Resources["db_ListOpenIcon"];
		public static DrawingBrush Main = (DrawingBrush)Application.Current.Resources["db_MainIcon"];
		public static DrawingBrush Modify = (DrawingBrush)Application.Current.Resources["db_ModifyIcon"];
		public static DrawingBrush Person = (DrawingBrush)Application.Current.Resources["db_PersonIcon"];
		public static DrawingBrush Phone = (DrawingBrush)Application.Current.Resources["db_PhoneIcon"];
		public static DrawingBrush Plus = (DrawingBrush)Application.Current.Resources["db_PlusIcon"];
		public static DrawingBrush PointerDown = (DrawingBrush)Application.Current.Resources["db_PointerDownIcon"];
		public static DrawingBrush Refresh = (DrawingBrush)Application.Current.Resources["db_RefreshIcon"];
		public static DrawingBrush Search = (DrawingBrush)Application.Current.Resources["db_SearchIcon"];
		public static DrawingBrush WinClose = (DrawingBrush)Application.Current.Resources["db_WinCloseIcon"];
		public static DrawingBrush WinFullScreenOff = (DrawingBrush)Application.Current.Resources["db_WinFullScreenOffIcon"];
		public static DrawingBrush WinFullScreenOn = (DrawingBrush)Application.Current.Resources["db_WinFullScreenOnIcon"];
		public static DrawingBrush WinMinimize = (DrawingBrush)Application.Current.Resources["db_WinMinimizeIcon"];
		public static DrawingBrush Yes = (DrawingBrush)Application.Current.Resources["db_YesIcon"];
	}
}
