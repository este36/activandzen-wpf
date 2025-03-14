using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using ActivAndZen.Popups;
using ActivAndZen.Styles;
using ActivAndZen.Inputs;

namespace ActivAndZen.Components;

/*-- Components Classes --*/
// Container
// IconGrid
// WindowHandlerButton
// WindowHeader
// SearchBarText
// SearchBar
// Separator
// ToolsButton
// ToolsHeader
// SideNavigation


public class Container : Border
{
    private GridExt ?_mainGrid;
    private Brush ?_backgroundColor;
    private Brush ?_hoverBgColor;
    private Brush ?_borderColor;
    public bool IsDisable {get;set;}

    public Container() {
        this.Background = Brushes.Transparent;
        this.BorderBrush = Brushes.Transparent;
        this.IsDisable = false;
    }
    
    public GridExt MainGrid {
        set {
            _mainGrid = value;
            this.Child = _mainGrid;
        }
    }
    public Brush ?BackgroundColor {
        get => _backgroundColor;
        set {
            _backgroundColor = value;
            _backgroundColor?.Freeze();
            this.Background = _backgroundColor;
        }
    }

    public Brush ?HoverBgColor {
        get => _hoverBgColor;
        set {
            _hoverBgColor = value;
            _hoverBgColor?.Freeze();
        }
    }

    public Brush BorderColor {
        get => _borderColor != null ? _borderColor : Brushes.Transparent;
        set {
            _borderColor = value;
            _borderColor.Freeze();
            this.BorderBrush = _borderColor;
        }
    }

    protected override void OnMouseEnter(MouseEventArgs e) {
        base.OnMouseEnter(e);
        if (!this.IsDisable) {
            if(_hoverBgColor != null) this.Background = _hoverBgColor;
        }
    }

    protected override void OnMouseLeave(MouseEventArgs e) {
        base.OnMouseLeave(e);
        if (!this.IsDisable) {
            if (_backgroundColor != null) this.Background = _backgroundColor;
            else this.Background = this.Background = Brushes.Transparent;
            
            if (_borderColor != null) this.BorderBrush = _borderColor;
            else this.BorderBrush = Brushes.Transparent;
        }
    }
}

public class CustomTooltip : ToolTip 
{
    public CustomTooltip(string text) 
    {
        BorderThickness = new(0.8);
        BorderBrush = Brushes.LightGray;
        BorderBrush.Freeze();
        FontSize = 10;
        Foreground = Utils.RGBA("#444");
        Foreground.Freeze();
        Content = text;
        Padding = new(4);
    }
}

public class IconGrid : Grid
{
    public DrawingBrush Icon;
    public double IconHeight;
    public double IconWidth;

    public IconGrid(double width, double height, DrawingBrush iconRef) 
    {
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        Height = height;
        Width = width;

        IconHeight = height;
        IconWidth = width;
        Icon = iconRef;

        Children.Add(new Grid {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Width = width,
            Height = height,
            Background = Icon
        });
    }

    public void SetIcon(DrawingBrush iconRef) 
    {
        this.Icon = iconRef; // (DrawingBrush)Application.Current.Resources[iconRef];
        // this.Icon.Freeze();
        if (this.Children[0] is Grid g) {
            g.Background = this.Icon;
        }
    }
}

public class Separator : StackPanel 
{
    public Separator() 
    {
         VerticalAlignment = VerticalAlignment.Stretch;
         Width = 0.8;
         Background = Brushes.LightGray;
         Background.Freeze();
         Margin = new(4);
    }
}

public partial class WindowHandlerButton : Container
{
    public IconGrid Icon;
    private bool IsWinClose;

    public WindowHandlerButton(DrawingBrush iconRef,Brush hoverColor, Action btnAction, bool isWinClose) 
    {
        IsWinClose = isWinClose;
        Width = 50;
        Height = MainWindow.HeaderHeight;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Stretch;
       
        if (MainWindow.BackgroundColor != null) BackgroundColor = MainWindow.BackgroundColor;
        HoverBgColor = hoverColor;

        const double iconSize = 10.5;

        Icon = new IconGrid(iconSize, iconSize, iconRef);
        Child = Icon;
        Icon.Icon.Freeze();

        WindowChrome.SetIsHitTestVisibleInChrome(this, true);
        MouseLeftButtonDown += (s, e) => e.Handled = true;
        MouseLeftButtonUp += MouseTracker.DefineClick(btnAction);
    }

    protected override HitTestResult ?HitTestCore(PointHitTestParameters h) 
    {
        double borderZone = MainWindow.WinMargin;
        Point hitPoint = h.HitPoint;
        bool isInResizeZone;

        if (this.IsWinClose) {
            isInResizeZone =    hitPoint.X > this.ActualWidth - borderZone || hitPoint.Y < borderZone;
        } else {
            isInResizeZone =    hitPoint.Y < borderZone;
        }

        if (isInResizeZone) return null;
        return base.HitTestCore(h);
    }
}

public partial class WindowHeader : Grid
{
    public WindowHandlerButton WinFullScreenToggle;
    public SearchBar SearchBar;

    public WindowHeader() 
    {
        Background = MainWindow.BackgroundColor;
        Background?.Freeze();

        Grid iconPanel = new() {
            Width = 80,
            Height = MainWindow.HeaderHeight,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Children = { new IconGrid(80 * 0.75, MainWindow.HeaderHeight * 0.75, App.Icons.Main) },
            Margin = new(0)
        };

        this.SearchBar = new() {
        };

        StackPanel winHandlerPanel = new() {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
        };

        Window parentWindow = Window.GetWindow(this);

        WinFullScreenToggle = new WindowHandlerButton(App.Icons.WinFullScreenOn, Brushes.LightGray, FullScreenToggle, false);

        winHandlerPanel.Children.Add(new WindowHandlerButton(App.Icons.WinMinimize, Brushes.LightGray, WindowMinimize, false));
        winHandlerPanel.Children.Add(WinFullScreenToggle);
        winHandlerPanel.Children.Add(new WindowHandlerButton(App.Icons.WinClose, Utils.RGBA("#ff5050"), WindowClose, true));

        Utils.DefineCols(this, [
            new GridElement(new(1, GridUnitType.Star), iconPanel),
            new GridElement(new(SearchBar.TotalWidth, GridUnitType.Pixel), SearchBar),
            new GridElement(new(1, GridUnitType.Star), winHandlerPanel),
        ]);
    }

    private void WindowClose() {
        Window parentWindow = Window.GetWindow(this);
        parentWindow?.Close();
    }

    private void FullScreenToggle() {
        Window parentWindow = Window.GetWindow(this);
        if (parentWindow != null) {
            
            parentWindow.WindowState = parentWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

            if (parentWindow.WindowState == WindowState.Maximized) {
                WinFullScreenToggle.Icon.SetIcon(App.Icons.WinFullScreenOff);
            } else {
                WinFullScreenToggle.Icon.SetIcon(App.Icons.WinFullScreenOn);
            }
        }
    }

    private void WindowMinimize() {
        Window parentWindow = Window.GetWindow(this);
        if (parentWindow != null) {
            parentWindow.WindowState = WindowState.Minimized;
        }
    }
}

public class SearchBar : TextInput
{
    public IconGrid Icon;
    public readonly double TotalWidth = 310;

    public SearchBar() {
        // specifique a la window header
        double _m = 4.5;
        Margin = new(0,_m+0.9,0,_m);

        // LEFT SEARCH ICON BEGIN
        DrawingBrush searchIcon = App.Icons.Search.Clone();
        Utils.ChangeDrawingColor(searchIcon, Utils.RGBA("#808080"));
        searchIcon.Freeze();

        double IconSideMargin = 12;
        double IconLength = 12.5;

        Icon = new(IconLength, IconLength, searchIcon);
        // LEFT SEARCH ICON END

        // TEXT AREA DEFINITION

        this.TextArea = new SearchTextField();
        TextArea.ParentContainer = this;

        // TEXT AREA DEFINITION

        WindowChrome.SetIsHitTestVisibleInChrome(this, true);

        Child = new GridExt() {
            Columns = [
                new GridElement(new(IconLength + IconSideMargin*2, GridUnitType.Pixel),Icon),
                new GridElement(new(1, GridUnitType.Star), TextArea),
                new GridElement(new(40, GridUnitType.Pixel), ResetButton)
            ]
        };
    }

    private class SearchTextField : TransparantTextBox
    {
        public SearchTextField() 
        {
            Placeholder = "Rechercher un(e) employé(e)";
        }

        protected override async void OnTextChanged(TextChangedEventArgs e) {
            if (!App.IsInit) return;
            base.OnTextChanged(e);

            if (this.IsFocused && this.Text.Length != 0) {
                await App.Window!.PopupSearch!.BuildResultList(this.Text);
                App.Window.ActivePopup = App.Window.PopupSearch;
            } else {
                App.Window.ActivePopup = null;



            }
        }

        protected override async void OnGotFocus(RoutedEventArgs e) {
            base.OnGotFocus(e);
            if (!IsTextPlaceholder && this.Text.Length > 0) {
                await App.Window!.PopupSearch!.BuildResultList(this.Text);
                App.Window.ActivePopup = App.Window.PopupSearch;
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e) {
            if ( ShouldLoseFocus() &&  App.Window.Header.SearchBar.Icon is Grid g &&  g.Children[0] is Grid c &&  MouseTracker.LastClickDown.UIElement != c ) {
                base.OnLostFocus(e);
                App.Window.ActivePopup = null;
            }
        }   
    }
}

public class ToolsButton : Container
{
    public IconGrid IconGrid;
    public bool IconIsMonoColor;

    public ToolsButton(double iconSize, DrawingBrush icon, bool isMonoColor) {

        HoverBgColor = Utils.RGBA("#efefef");
        Margin = new(2);
        CornerRadius = new(4);
        Height = 35;
        Width = 35;

        IconGrid = new IconGrid(iconSize,iconSize,icon);
        this.Child = IconGrid;

        IconIsMonoColor = isMonoColor;
        if (IconIsMonoColor) {
            Disable();
        }
    }

    public void Enable() {
        if (IconIsMonoColor) {
            DrawingBrush newIcon = IconGrid.Icon.Clone();
            Utils.ChangeDrawingColor(newIcon, Utils.RGBA("#444"));
            newIcon.Freeze();
            IconGrid.SetIcon(newIcon);
            // Utils.ChangeDrawingColor(this.IconGrid.Icon, Utils.RGBA("#aaa"));
        }
        // InvalidateVisual();
    }

    public void Disable() {
        this.IsDisable = true;
        if (IconIsMonoColor) {
            DrawingBrush newIcon = IconGrid.Icon.Clone();
            Utils.ChangeDrawingColor(newIcon, Utils.RGBA("#aaa"));
            newIcon.Freeze();
            IconGrid.SetIcon(newIcon);
            // Utils.ChangeDrawingColor(this.IconGrid.Icon, Utils.RGBA("#aaa"));
        }

        // this.InvalidateVisual();
    }
}

public class ToolsHeader : Border
{
    public ToolsButton NewEmployeeButton;
    public ToolsButton NewClientButton;
    public ToolsButton ImportDataButton;

    public ToolsHeader() {
        NewEmployeeButton = new ToolsButton(24, App.Icons.Person, false) {
            ToolTip = new CustomTooltip("Ajouter un employé"), 
            Cursor = Cursors.Hand
        };
        NewEmployeeButton.MouseUp += MouseTracker.DefineClick(() => App.Window.ActivePopup = App.Window.PopupNewEmployee);

        NewClientButton = new ToolsButton(24, App.Icons.Building, false) {
            ToolTip = new CustomTooltip("Ajouter un client"),
            Cursor = Cursors.Hand
        };
        NewClientButton.MouseUp += MouseTracker.DefineClick(() => App.Window.ActivePopup = App.Window.PopupNewClient);

        ImportDataButton = new ToolsButton(24, App.Icons.File, false) {
            ToolTip = new CustomTooltip("Importer des données..."), 
            Cursor = Cursors.Hand
        };

        double b = 0.5;
        BorderThickness = new(0,b,0,b);
        CornerRadius = new(0);
        BorderBrush = Brushes.LightGray; //  Utils.RGBA("#b0b0b0");
        Background = Brushes.White;// Utils.RGBA("#f9f9f9");

        Child = new StackPanel {
            Orientation = Orientation.Horizontal,
            Children = {
                new ToolsButton(16, App.Icons.ArrowToLeft, true),
                new ToolsButton(16, App.Icons.ArrowToRight, true),
                new ToolsButton(16, App.Icons.Refresh, true),
                new Separator(),
                NewEmployeeButton,
                NewClientButton,
                ImportDataButton,
            },
            Margin = new(2,0,0,0)
        };

        
    }
}

public class SideNavigation : GridExt
{
    public SideNavigation() {
        this.Width = 210;
        this.Background = Brushes.Chocolate;
    }
}


public abstract class InputBase : Container
{
    public string KeyName;

    public InputBase(string keyName)
    {
        KeyName = keyName;
    }

    private void Validate()
    {
    }
}