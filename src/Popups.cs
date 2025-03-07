using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using ActivAndZen.Components;
using ActivAndZen;
using System.Windows.Input;

namespace ActivAndZen.Popups;

/*-- Popups Classes --*/
// SearchEmployee
// NewEmployee
// NewClient

public enum PopupEnum 
{
    UNSET,
    SEARCH,
    NEW_EMPLOYEE,
    NEW_CLIENT
}

public class Search : Popup 
{
	Grid mainGrid;
    public Search () {
    	this.Focusable = true;
		PlacementTarget = App.Window.Header.SearchBar;
        Placement = PlacementMode.Bottom;
        IsOpen = false;

    	this.mainGrid = new Grid {
    		Background = Brushes.WhiteSmoke,
    		Height = 40,
            Width = App.Window.Header.SearchBar.TotalWidth,
    	};

    	this.Child = mainGrid;
    }

    protected override void OnGotFocus(RoutedEventArgs e) {
        base.OnGotFocus(e);
        this.mainGrid.Background = Brushes.Blue;
    }

    protected override void OnLostFocus(RoutedEventArgs e) {
        base.OnGotFocus(e);
        this.mainGrid.Background = Brushes.Red;
    }
}

public class NewEmployee : Popup
{
    public NewEmployee () {
       this.Focusable = true;
		PlacementTarget = App.Window.Header.SearchBar;
        Placement = PlacementMode.Bottom;
        IsOpen = false;

    	this.Child = new Grid {
    		Background = Brushes.WhiteSmoke,
    		Height = 40,
            Width = App.Window.Header.SearchBar.TotalWidth,
    	};
    }
}

public class NewClient : Popup
{
    public NewClient () {
        this.Focusable = true;
		PlacementTarget = App.Window.Header.SearchBar;
        Placement = PlacementMode.Bottom;
        IsOpen = false;

    	this.Child = new Grid {
    		Background = Brushes.WhiteSmoke,
    		Height = 40,
            Width = App.Window.Header.SearchBar.TotalWidth,
    	};
    }
}