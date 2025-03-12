using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Data.Sqlite;
using System.IO;

namespace ActivAndZen;

/*-- Classes --*/
// App
// GridExt
// MouseTracker
// Utils

public partial class App : Application
{
    public static bool IsInit = false;
    public static MainWindow Window = new MainWindow();

    protected override void OnStartup(StartupEventArgs e) {
        // Init Database if Not Exist
        if (!File.Exists(Model.Settings.DatabaseFile)) {
            using (var connection = new SqliteConnection($"Data Source={Model.Settings.DatabaseFile}"))
            {
                connection.Open();
                string script = File.ReadAllText(Model.Settings.SqlFile);
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = script;
                    command.ExecuteNonQuery();
                }
            }
        }


        base.OnStartup(e);
        IsInit = true;
        Window.Show();
    }

    public static object GetResource(string xKey) {
        return Application.Current.Resources[xKey];
    }

    public static class ColorRefs 
    {
        public static string Main = "dd4124";
    }
}

public class GridExt : Grid
{
    private List<GridElement> ?_columns;
    private List<GridElement> ?_rows;
    
    public List<GridElement> Columns {
        set {
            _columns = value;
            this.ColumnDefinitions.Clear();    
            if (_columns != null) Utils.DefineCols(this, _columns);
        }
    }
    
    public List<GridElement> Rows {
        set {
            _rows = value;
            this.RowDefinitions.Clear();
            if (_rows != null) Utils.DefineRows(this, _rows);
        }
    }
}

public static class MouseTracker
{
    public static class LastClickDown
    {
        public static UIElement? UIElement;
        public static System.Windows.Documents.Paragraph? Paragraph;
    }

    public static void previewMouseDown(object sender, MouseButtonEventArgs e) {
        if (e.OriginalSource is UIElement ui) {
            LastClickDown.UIElement = ui;
            // ui.Focus();
            // MessageBox.Show(ui.ToString());
        } else if (e.OriginalSource is System.Windows.Documents.Paragraph p) {
            LastClickDown.Paragraph = p;
            LastClickDown.UIElement = null;
        }
    }

    private static bool WasDown(UIElement e) {
        return e == LastClickDown.UIElement;
    }

    private static void Reset() {
        LastClickDown.UIElement = null;
    }

    public static MouseButtonEventHandler DefineClick(Action action) {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        return (sender, e) => {
            if (MouseTracker.WasDown((UIElement)e.OriginalSource)) {
                action();
            }
            MouseTracker.Reset();
        };
    }
}

public struct GridElement
{
    public GridLength gridLength;
    public UIElement ?uiElement;

    public GridElement(GridLength gl, UIElement? ue = null) {
        this.gridLength = gl;
        this.uiElement = ue;
    }

    public static implicit operator List<object>(GridElement v)
    {
        throw new NotImplementedException();
    }
}

public static class Utils
{
    static public void DefineCols(Grid grid, List<GridElement> gridElements) {
        for (int i = 0; i < gridElements.Count; i++) {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = gridElements[i].gridLength });
            if (gridElements[i].uiElement != null) {
                Grid.SetColumn(gridElements[i].uiElement, i);
                grid.Children.Add(gridElements[i].uiElement);
            }
        };
    }

    static public void DefineRows(Grid grid, List<GridElement> gridElements) {
        for (int i = 0; i < gridElements.Count; i++) {
            grid.RowDefinitions.Add(new RowDefinition { Height = gridElements[i].gridLength});
            if (gridElements[i].uiElement != null) {
                Grid.SetRow(gridElements[i].uiElement, i);
                grid.Children.Add(gridElements[i].uiElement);
            }
        };
    }

    public static SolidColorBrush RGBA(string hexColor) {
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor));
    }

    public static uint TEMP_DEBUG = 0;

    public static void DEBUG() {
        TEMP_DEBUG++;
        MessageBox.Show("Debug: " + TEMP_DEBUG);
    }

    static public SolidColorBrush ?ChangeDrawingColor(DrawingBrush drawing, SolidColorBrush newColor) {
        SolidColorBrush ?originalColor = null; 

        if (drawing.Drawing is DrawingGroup d) {
            ChangeDrawingGroupColor(d, originalColor, newColor, false);
        }

        return originalColor;
    }

    private static void ChangeDrawingGroupColor(DrawingGroup dg, SolidColorBrush ?originalColor, SolidColorBrush newColor, bool found) {
        for (int i = 0; i < dg.Children.Count; i++) {
            if (dg.Children[i] is DrawingGroup nested) {
                ChangeDrawingGroupColor(nested, originalColor, newColor, found);
            } else if (dg.Children[i] is GeometryDrawing g) {
                if (!found && g.Brush != null && g.Brush is SolidColorBrush colorBrush) {
                    originalColor = colorBrush;
                    found = true;
                }

                if (g.Brush != null) g.Brush = newColor;
            }
        }
    }
}
