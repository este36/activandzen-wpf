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
    private Grid RootGrid;

    public MainWindow()
    {
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        this.Title = "Activ & Zen";
        this.MinHeight = 600;
        this.Height = 600;
        this.MinWidth = 800;
        this.Width = 800;

        RootGrid = new Grid();
        this.Content = RootGrid;

        DefineCols(RootGrid, new List<GridLength>
        {
            new GridLength(25, GridUnitType.Star),
            new GridLength(75, GridUnitType.Star)
        });

        System.Windows.Media.Brush bgColor = Brushes.WhiteSmoke;

        Border border = new Border();
        border.BorderThickness = new Thickness(0);
        border.CornerRadius = new CornerRadius(20);
        border.Background = bgColor;
        border.Margin = new Thickness(10);

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

        border.Child = cout;
        Grid.SetColumn(border, 1);
        RootGrid.Children.Add(border);
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

