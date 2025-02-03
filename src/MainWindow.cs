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
    private Grid m_rootGrid;
    private TextBox cout;

    public MainWindow()
    {
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        this.Title = "Activ & Zen";
        this.MinHeight = 600;
        this.Height = 600;
        this.MinWidth = 800;
        this.Width = 800;
        this.PreviewMouseDown += MouseTracker.previewMouseDown;

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

        this.cout = new TextBox();
        this.cout.Background = bgColor;
        this.cout.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        this.cout.Margin = new Thickness(20);
        this.cout.BorderThickness = new Thickness(0);
        this.cout.TextWrapping = TextWrapping.Wrap;
        this.cout.FontSize = 14.5;
        this.cout.IsReadOnly = true;
        this.cout.Cursor = Cursors.Arrow;

        cout_border.Child = this.cout;
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

        debugBTN.MouseUp += MouseTracker.DefineClick(async () =>
        {
            this.cout.AppendText("Asking Database...\r\n");
            string dbresult = await AskDatabase();
            this.cout.AppendText(dbresult);
        });

        leftgrid.Children.Add(debugBTN);
        m_rootGrid.Children.Add(leftgrid);
    }

    public async Task<string> AskDatabase()
    {
        await Task.Delay(3000);

        List<string> result_query = Model.Ask<string>("SELECT id, name FROM clients", (l, dr) =>
        {
            int id = dr.GetInt32(0);  // Récupère la colonne 0 (id)
            string name = dr.GetString(1); // Récupère la colonne 1 (name)
            l.Add($"ID: {id}, Nom: {name} ");
        });

        string result_str = "";

        if (result_query.Count == 0)
        {
            this.cout.AppendText("No data received.");
        } else
        {
            result_query.ForEach((s) => result_str += s + "\r\n");
        }
        return await Task.FromResult(result_str);
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

