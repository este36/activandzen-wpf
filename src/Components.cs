using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using ActivAndZen.Popups;

namespace ActivAndZen.Components;

/*-- Components Classes --*/
// Container
// IconGrid
// WindowHandlerButton
// WindowHeader
// TransparantTextBox
// SearchBarText
// SearchBar
// Separator
// ToolsButton
// ToolsHeader
// SideNavigation


abstract public class Container : Border
{
    private GridExt ?_mainGrid;
    private Brush ?_backgroundColor;
    private Brush ?_hoverBgColor;
    private Brush ?_borderColor;
    private Brush ?_hoverBorderColor;
    private double ?_defaultHeight;
    private double ?_defaultWidth;
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
        set {
            _borderColor = value;
            _borderColor.Freeze();
            this.BorderBrush = _borderColor;
        }
    }

    public Brush HoverBorderColor {
        set {
            _hoverBorderColor = value;
            _hoverBorderColor.Freeze();
        }
    }

    public double DefaultHeight {
        set {
            _defaultHeight = value;
            this.Height = value;
        }
    }
    public double DefaultWidth {
        set {
            _defaultWidth = value;
            this.Width = value;
        }
    }

    protected override void OnMouseEnter(MouseEventArgs e) {
        base.OnMouseEnter(e);
        if (!this.IsDisable) {
            if(_hoverBgColor != null) this.Background = _hoverBgColor;
            if(_hoverBorderColor != null) this.BorderBrush = _hoverBorderColor;    
        }
    }

    protected override void OnMouseLeave(MouseEventArgs e) {
        base.OnMouseLeave(e);
        if (!this.IsDisable) {
            if (_backgroundColor != null) this.Background = _backgroundColor;
            else this.Background = this.Background = Brushes.Transparent;
            
            if (_borderColor != null) this.BorderBrush = _borderColor;
            else this.BorderBrush = Brushes.Transparent;

            if (_defaultHeight != null) this.Height = (double)_defaultHeight;
            if (_defaultWidth != null) this.Height = (double)_defaultWidth;
        }
    }
}

public class CustomTooltip : ToolTip 
{
    public CustomTooltip(string text) {
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

    public IconGrid(double width, double height, DrawingBrush iconRef) {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
        IconHeight = height;
        IconWidth = width;
        Icon = iconRef;
        // Icon.Freeze();

        Children.Add(new Grid {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Width = width,
            Height = height,
            Background = Icon
        });
    }

        public void SetIcon(DrawingBrush iconRef) {
        this.Icon = iconRef; // (DrawingBrush)Application.Current.Resources[iconRef];
        // this.Icon.Freeze();
        if (this.Children[0] is Grid g) {
            g.Background = this.Icon;
        }
    }

    
}

public class Separator : StackPanel 
{
    public Separator() {
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

    public WindowHandlerButton(DrawingBrush iconRef,Brush hoverColor, Action btnAction, bool isWinClose) {
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

    protected override HitTestResult ?HitTestCore(PointHitTestParameters h) {
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

    public WindowHeader() {
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

        this.SearchBar = new();

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

public class TransparantTextBox : TextBox
{
    public Action ?TextChange;

    public TransparantTextBox() {
        this.FontSize = 13;
        this.Background = Brushes.Transparent;
        this.Background.Freeze();
        this.BorderThickness = new(0);
        this.VerticalAlignment = VerticalAlignment.Center;
    }

    protected override void OnGotFocus(RoutedEventArgs e) {
        MainWindow.TextBoxIsFocus = true;
        base.OnGotFocus(e);
    }
}

public class SearchBarText : TransparantTextBox
{
    public static readonly string Placeholder = "Rechercher un(e) employé(e)";

    protected override void OnTextChanged(TextChangedEventArgs e){
        base.OnTextChanged(e);
        if (!App.IsInit) return;

        if (this.Text != Placeholder) {
            if (this.Text.Length == 0) {
                App.Window.Header.SearchBar.ResetButton.Width = 0;
                App.Window.ActivePopup = PopupEnum.UNSET;
            } else {
                App.Window.Header.SearchBar.ResetButton.Width = 20;
                App.Window.ActivePopup = PopupEnum.SEARCH;
            }
        } else {
            App.Window.Header.SearchBar.ResetButton.Width = 0;
            App.Window.ActivePopup = PopupEnum.UNSET;
        }

        
    }

    protected override void OnGotFocus(RoutedEventArgs e) {
        base.OnGotFocus(e);
        if (this.Text == Placeholder) {
            this.Text = "";
        } else if (this.Text != "") {
            App.Window.ActivePopup = PopupEnum.SEARCH;
        }

        App.Window.Header.SearchBar.Background = App.Window.Header.SearchBar.HoverBgColor;
    }

    protected override void OnLostFocus(RoutedEventArgs e) {
        base.OnLostFocus(e);

        if (    MouseTracker.LastClickDown.UIElement != App.Window.Header.SearchBar 
            &&  MouseTracker.LastClickDown.UIElement != App.Window.Header.SearchBar.ResetButton
            &&  App.Window.Header.SearchBar.ResetButton.Child is Grid resetButtonGrid
            &&  resetButtonGrid.Children[0] is Grid resetButtonGridChild
            &&  MouseTracker.LastClickDown.UIElement != resetButtonGridChild
            &&  App.Window.Header.SearchBar.Icon is Grid g
            &&  g.Children[0] is Grid c
            &&  MouseTracker.LastClickDown.UIElement != c
            ) {
            App.Window.Header.SearchBar.Background = App.Window.Header.SearchBar.BackgroundColor;
            if (this.Text == "") this.Text = Placeholder;
            App.Window.ActivePopup = PopupEnum.UNSET;
        }
    }
}

public class SearchBarResetButton : Container 
{
    public SearchBarResetButton() {
        double m = 2.5;
        this.Margin = new(m,m*1.5,m,m*1.5);
        this.HoverBgColor = Brushes.LightGray;

        // this.BorderThickness = new(0.5);
        // this.HoverBorderColor = Brushes.LightGray;
        this.CornerRadius = new(2);

        double iconSize = 8.5;
        IconGrid icon = new IconGrid(iconSize,iconSize, App.Icons.Cross);
        icon.Icon.Freeze();
        this.Child = icon;

        this.MouseUp += MouseTracker.DefineClick(() => {
            App.Window.Header.SearchBar.TextArea.Focus();
            App.Window.Header.SearchBar.TextArea.Text = "";
        });
    }

    protected override void OnMouseEnter(MouseEventArgs e){
        base.OnMouseEnter(e);
        this.Cursor = Cursors.Hand;
    }

    protected override void OnMouseLeave(MouseEventArgs e) {
        base.OnMouseLeave(e);
        this.Cursor = Cursors.IBeam;
    }
}

public class SearchBar : Container
{
    public SearchBarText TextArea;
    public IconGrid Icon;
    public SearchBarResetButton ResetButton; 
    public double TotalWidth;

    public SearchBar() {
        this.TotalWidth = 310;
        this.Cursor = Cursors.IBeam;
        BackgroundColor = Utils.RGBA("#f0f0f0");
        HoverBgColor = Brushes.WhiteSmoke;// Utils.RGBA("#f9f9f9");
        BorderColor = Brushes.LightGray;
        BorderThickness = new(0.5);
        CornerRadius = new(10);
        double _m = 4.5;
        Margin = new(0,_m+0.9,0,_m);

        DrawingBrush searchIcon = App.Icons.Search.Clone();
        Utils.ChangeDrawingColor(searchIcon, Utils.RGBA("#808080"));
        searchIcon.Freeze();

        double IconSideMargin = 12;
        double IconLength = 12.5;

        Icon = new(IconLength, IconLength, searchIcon){
            Margin = new(IconSideMargin,0,IconSideMargin,0)
        };

        TextArea = new SearchBarText() {
            Text = SearchBarText.Placeholder,
        };

        ResetButton = new() {Width = 0};
        WindowChrome.SetIsHitTestVisibleInChrome(this, true);

        Child = new GridExt() {
            Columns = [
                new GridElement(new(IconLength + IconSideMargin*2, GridUnitType.Pixel),Icon),
                new GridElement(new(1, GridUnitType.Star), TextArea),
                new GridElement(new(40, GridUnitType.Pixel), ResetButton)
            ]
        };

        this.MouseUp += MouseTracker.DefineClick(() => {
            if (!this.TextArea.IsFocused) this.TextArea.Focus();
        });
    }

    protected override void OnMouseLeave(MouseEventArgs e){
        if (!this.TextArea.IsFocused) {
            base.OnMouseLeave(e);
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
    public ToolsHeader() {
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
                new ToolsButton(24, App.Icons.Person, false) {ToolTip = new CustomTooltip("Ajouter un employé"), Cursor = Cursors.Hand},
                new ToolsButton(24, App.Icons.Building, false) {ToolTip = new CustomTooltip("Ajouter un client"), Cursor = Cursors.Hand},
                new ToolsButton(24, App.Icons.File, false) {ToolTip = new CustomTooltip("Importer des données..."), Cursor = Cursors.Hand},
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