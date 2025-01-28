using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ActivAndZen;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Model m_model;
    private Grid m_rootGrid;

    public MainWindow(Model model)
    {
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        this.Title = "Activ & Zen";
        this.MinHeight = 600;
        this.Height = 600;
        this.MinWidth = 800;
        this.Width = 800;
        this.PreviewMouseDown += MouseTracker.previewMouseDown;

        this.m_model = model;
        this.m_rootGrid = new Grid();
        this.Content = this.m_rootGrid;

        DefineCols(m_rootGrid, new List<GridLength>
        {
            new GridLength(25, GridUnitType.Star),
            new GridLength(75, GridUnitType.Star)
        });

        //System.Windows.Media.Brush bgColor = (Brush)App.GetResource("MyBrush");
        System.Windows.Media.Brush bgColor = Brushes.WhiteSmoke;

        Border cout_border = new Border();
        cout_border.BorderThickness = new Thickness(0);
        cout_border.CornerRadius = new CornerRadius(20);
        cout_border.Background = bgColor;
        cout_border.Margin = new Thickness(10);

        TextBox cout = new TextBox();
        cout.Background = bgColor;
        cout.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        cout.Margin = new Thickness(20);
        cout.BorderThickness = new Thickness(0);
        cout.TextWrapping = TextWrapping.Wrap;
        cout.FontSize = 14.5;
        cout.IsReadOnly = true;
        cout.Text = "Hello, World !";
        cout.Cursor = Cursors.Arrow;

        cout_border.Child = cout;
        Grid.SetColumn(cout_border, 1);
        m_rootGrid.Children.Add(cout_border);

        Grid leftgrid = new Grid();
        DefineRows(leftgrid, new List<GridLength>
        {
            new GridLength(20, GridUnitType.Star),
            new GridLength(80, GridUnitType.Star)
        });
        Grid.SetColumn(leftgrid, 0);

        int mrgn = 35;
        Border debugBTN = new Border
        {
            CornerRadius = new CornerRadius(25),
            Background = new SolidColorBrush(Color.FromRgb(0xff, 0xcc, 0x33)),
            Margin = new Thickness(mrgn,mrgn,mrgn - 10,mrgn),
            Child = new TextBlock
            {
                Background = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 20,
                Text = "Click"
            },
        };

        debugBTN.MouseUp += MouseTracker.DefineClick(() =>
        {
            MessageBox.Show("Hello, Custom BTN");
        });
        debugBTN.MouseEnter += (sender, e) =>
        {
            debugBTN.Background = Brushes.Gold;
            debugBTN.Cursor = Cursors.Hand;
        };
        debugBTN.MouseLeave += (sender, e) =>
        {
            debugBTN.Background = new SolidColorBrush(Color.FromRgb(0xff, 0xcc, 0x33));
            debugBTN.Cursor = Cursors.Arrow;
        };

        leftgrid.Children.Add(debugBTN);
        m_rootGrid.Children.Add(leftgrid);
    }

    public void btnTest_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Hello !");
    }

    public void DefineCols(Grid grid, List<GridLength> gridLengths)
    {
        gridLengths.ForEach(e =>
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = e });
        });
    }

    public void DefineRows(Grid grid, List<GridLength> gridLengths)
    {
        gridLengths.ForEach(e =>
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = e });
        });
    }
}

