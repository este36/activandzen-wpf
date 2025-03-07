using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shell;
using ActivAndZen.Components;
using ActivAndZen.Popups;
using System.Windows.Input;

namespace ActivAndZen;

public partial class MainWindow : Window
{
    public static double HeaderHeight {get;set;}
    public static Brush ?BackgroundColor {get;set;}
    public static double WinMargin {get;set;}
    public static readonly double WinHeight = 1080;
    public static readonly double WinWidth = 1920;
    public static bool TextBoxIsFocus = false;

    public WindowHeader Header;
    public ToolsHeader ToolsHeader;
    public SideNavigation SideNavigation;
    public Popups.Search ?PopupSearch;
    public Popups.NewEmployee ?PopupNewClient;
    public Popups.NewEmployee ?PopupNewEmployee;
    public TextBlock MainContent = new TextBlock {
         Background = Brushes.Honeydew,
         Text = "INIT"
    };

    public Button UnvalidButton = new Button() { // bouton invisible pour unfocus les textbox 
        Height = 0,
        Width = 0
    };

    private readonly Grid m_rootGrid;
    private List<Key> KeysDown;
    private PopupEnum _activePopup;

    public MainWindow() {
        _activePopup = PopupEnum.UNSET;
        WinMargin = 3;
        HeaderHeight = 40;
        BackgroundColor = Utils.RGBA("#f0edea");
        this.Background = BackgroundColor;

        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ResizeMode = ResizeMode.CanResize;
        Title = "Activ & Zen";
        MinHeight = WinHeight/2;
        MinWidth = WinWidth/2;
        Height = WinHeight/1.5;
        Width = WinWidth/1.5;
        PreviewMouseDown += MouseTracker.previewMouseDown;
        UseLayoutRounding = true;
        this.KeysDown = new();

        WindowChrome chrome = new() {
            CaptionHeight = HeaderHeight, // Supprime la zone de titre par défaut
            CornerRadius = new(0),
            GlassFrameThickness = new(1),
            ResizeBorderThickness = new Thickness(WinMargin), // Zone de redimensionnement invisible
        };
        WindowChrome.SetWindowChrome(this, chrome);

        this.ResizeMode = ResizeMode.CanResizeWithGrip;

        Header = new WindowHeader();
        ToolsHeader = new ToolsHeader();
        SideNavigation = new SideNavigation();

        m_rootGrid = new Grid() {
            Background = BackgroundColor,
            // Margin = new(WinMargin), // new(m_winMargin,0,m_winMargin,0),
            Children = {
                new GridExt {
                    Rows = [
                        new GridElement(new(HeaderHeight, GridUnitType.Pixel), Header) ,
                        new GridElement(new(1, GridUnitType.Star), 
                            new GridExt {
                                Rows = [
                                    new GridElement(new(45, GridUnitType.Pixel), ToolsHeader),
                                    new GridElement(new(1, GridUnitType.Star), new GridExt {
                                        Columns = [
                                            new GridElement(new(SideNavigation.Width, GridUnitType.Pixel), SideNavigation),
                                            new GridElement(new(1, GridUnitType.Star), new GridExt {
                                                Columns = [
                                                    new GridElement(new(1, GridUnitType.Star)),
                                                    new GridElement(new(this.MinWidth - SideNavigation.Width, GridUnitType.Pixel), MainContent),
                                                    new GridElement(new(1, GridUnitType.Star)),
                                                ]
                                            })
                                        ]
                                    }),
                                ],
                            }),
                        // bouton invisible pour unfocus les textbox 
                        new GridElement(new(0, GridUnitType.Pixel), UnvalidButton)
                    ]
                },
            }
        };

        this.Focusable = true;
        this.MouseDown += Window_MouseDown;
        Content = m_rootGrid;
        this.InvalidateVisual();
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        base.OnKeyDown(e);

        switch (e.Key) {
            case Key.Escape:
                if (this.KeysDown.Count > 0) this.KeysDown.Clear();
                if (TextBoxIsFocus) {
                    this.UnvalidButton.Focus();
                    TextBoxIsFocus = false;
                }
                break;
        }
    }

    protected override void OnStateChanged(EventArgs e)
    {
        base.OnStateChanged(e);
        if (WindowState == WindowState.Maximized) {
            m_rootGrid.Margin = new Thickness(5 + WinMargin);
        }
        else {
            m_rootGrid.Margin = new Thickness(0);
            this.Header.WinFullScreenToggle.Icon.SetIcon(App.Icons.WinFullScreenOn);
        }
    }


    private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
        // pour unfocus les textbox
        if (TextBoxIsFocus) {
            this.UnvalidButton.Focus();
            TextBoxIsFocus = false;
        }
    }

    public PopupEnum ActivePopup {
        get => _activePopup;
        set {
            if (value == _activePopup) return;
            // On dois désactiver le popup actuel d'abord
            switch(_activePopup) {
                case PopupEnum.SEARCH:          
                    if (this.PopupSearch != null) this.PopupSearch.IsOpen = false; break;
                case PopupEnum.NEW_EMPLOYEE:    
                    if (this.PopupNewEmployee != null) this.PopupNewEmployee.IsOpen = false; break;
                case PopupEnum.NEW_CLIENT:      
                    if (this.PopupNewClient != null) this.PopupNewClient.IsOpen = false; break;
            }
            // Puis on active le popup demandé
            switch (value) {
                case PopupEnum.SEARCH:
                    if (this.PopupSearch == null) this.PopupSearch = new();
                    this.PopupSearch.IsOpen = true; 
                    break;
                // case PopupEnum.NEW_EMPLOYEE:    this.PopupNewEmployee.IsOpen = true; break;
                // case PopupEnum.NEW_CLIENT:      this.PopupNewClient.IsOpen = true; break;
            }

            _activePopup = value;
            InvalidateVisual();
        }
    }

    public void ChangeContent(string s) {
        this.MainContent.Text = s; 
    }
}

