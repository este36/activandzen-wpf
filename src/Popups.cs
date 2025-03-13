using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Threading.Tasks;
using ActivAndZen.Model;
using ActivAndZen.Components;

namespace ActivAndZen.Popups;

/*-- Popups Classes / Enums --*/

// PopupEnum
// Search
// InputTypeEnum
// Input
// Form

public abstract class PopupAdorner : Adorner
{
	private FrameworkElement? popupContent;
	private bool _isOpen;
	private AdornerLayer? adornerLayer;

	public double HorizontalOffset { get; set; }
	public double VerticalOffset { get; set; }

	protected PopupAdorner(UIElement adornedElement) : base(adornedElement)
	{
		HorizontalOffset = 0;
		if (adornedElement is FrameworkElement fe)
			VerticalOffset = fe.ActualHeight;
		else
			VerticalOffset = 0;
		
		// Obtenir la référence à l'AdornerLayer dès la création
		adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
		_isOpen = false;
	}

	protected abstract FrameworkElement CreatePopupContent();

	protected FrameworkElement PopupContent
	{
		get 
		{
			if (popupContent == null)
			{
				popupContent = CreatePopupContent();
				if (popupContent == null)
					throw new InvalidOperationException("CreatePopupContent ne peut pas retourner null.");
				AddVisualChild(popupContent);
			}
			return popupContent;
		}
	}

	protected override int VisualChildrenCount => popupContent == null ? 0 : 1;

	protected override Visual GetVisualChild(int index)
	{
		if (index == 0 && popupContent != null)
			return popupContent;
		throw new ArgumentOutOfRangeException(nameof(index));
	}

	protected override Size MeasureOverride(Size constraint)
	{
		PopupContent.Measure(constraint);
		return PopupContent.DesiredSize;
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		PopupContent.Arrange(new Rect(new Point(HorizontalOffset, VerticalOffset), PopupContent.DesiredSize));
		return finalSize;
	}

	public bool IsOpen
	{
		get => _isOpen;
		set
		{
			if (_isOpen == value)
				return;
				
			if (adornerLayer == null)
			{
				adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
				if (adornerLayer == null)
					return;
			}
			if (value)
				adornerLayer.Add(this);
			else
				adornerLayer.Remove(this);

			_isOpen = value;
		}
	}
}

public class CustomPopupAdorner : PopupAdorner
{
	private FrameworkElement popup;
	public UIElement Adorned { get; set; }

	public CustomPopupAdorner(UIElement adornedElement, FrameworkElement popupElement)
		: base(adornedElement)
	{
		this.popup = popupElement;
		this.Adorned = adornedElement;
	}

	protected override FrameworkElement CreatePopupContent() => popup;
}

public class Popup
{
	private CustomPopupAdorner? _adorner;
	private UIElement? _placementTarget;
	private FrameworkElement? _child;

	public UIElement? PlacementTarget
	{
		get => _placementTarget;
		set
		{
			_placementTarget = value;
			UpdateAdorner();
		}
	}

	public FrameworkElement? Child
	{
		get => _child;
		set
		{
			_child = value;
			UpdateAdorner();
		}
	}

	public bool IsOpen
	{
		get => _adorner?.IsOpen ?? false;
		set
		{
			if (_adorner != null)
				_adorner.IsOpen = value;
		}
	}

	private void UpdateAdorner()
	{
		bool wasOpen = _adorner?.IsOpen ?? false;
		if (_adorner != null && _adorner.IsOpen)
		{
			_adorner.IsOpen = false;
		}

		if (_placementTarget != null && _child != null)
		{
			_adorner = new CustomPopupAdorner(_placementTarget, _child);
			if (wasOpen)
				_adorner.IsOpen = true;
		}
		else
		{
			_adorner = null;
		}
	}
}

public class Search : Popup
{
	StackPanel Results;
	
    public Search () {
        PlacementTarget = App.Window.Header.SearchBar;
        IsOpen = false;
        Results = new() {
        	Children = {
        		new TextBlock {
        			Margin = new(6,0,6,0),
        			Text = "Chargement...",
        			Foreground = Brushes.Gray
        		}
        	}
        };
    	this.Child = new Border {
			Width = App.Window.Header.SearchBar.TotalWidth,
			Background = Brushes.WhiteSmoke,
			CornerRadius = new(8),
			BorderThickness = new(0),
			BorderBrush = Brushes.Gray,
			Padding = new(0,6,0,6),
			Child = Results
		};
    }

    public async Task BuildResultList(string input) {
    	 
    	Results.Children.Clear();

        Employees employees_query = await Task.Run(() => Queries.GetPossibleEmployees(input));
        input.Trim();

        Results.Children.Clear();

		if (employees_query.Count == 0) {
			Results.Children.Add( new TextBlock {
				Margin = new(6,0,6,0),
				Text = "Aucun résultat",
    			Foreground = Brushes.Gray
			});
			return;
		}

		employees_query.SortOrderByFilter(input);		

		for (int i = 0; i < employees_query.Count && i < 5; i++) {
			string clientName = Queries.GetClientNameFromId(employees_query.client_id[i]);
			string fullName = $"{employees_query.first_name[i]} {employees_query.last_name[i]}";

			int substr_index = fullName.IndexOf(input, StringComparison.CurrentCultureIgnoreCase);

			ResultElement resultElement = new(employees_query.id[i], clientName);

			if (substr_index == 0) {
				// dans ce cas le bold est juste au début, juste besoin de deux append
				resultElement.AppendToName(true, fullName.Substring(0, input.Length));
				if (input.Length < fullName.Length) resultElement.AppendToName(false, fullName.Substring(input.Length));
			} else {
				// un non-bold , un bold, puis un non-bold
				int first_occ_index = fullName.IndexOf(input, StringComparison.CurrentCultureIgnoreCase);
				resultElement.AppendToName(false, fullName.Substring(0, first_occ_index));
				resultElement.AppendToName(true, fullName.Substring(first_occ_index, input.Length));
				if (input.Length + first_occ_index < fullName.Length) resultElement.AppendToName(false, fullName.Substring(first_occ_index + input.Length));
			}

			Results.Children.Add(resultElement);
		}
    }

    private class ResultElement : Container
	{
		private int _employeeId;
		private StackPanel _employeeName;
		public ResultElement(int id, string clientName)
		{
			_employeeName = new() {
				Orientation = Orientation.Horizontal
			};
			_employeeId = id;

			HoverBgColor = Brushes.White;
			CornerRadius = new(4);
			Padding = new(6);
			Margin = new(6,0,6,0);

			Child = new StackPanel {
				Orientation = Orientation.Horizontal,
				Children = {
					_employeeName,
					new TextBlock {
						Margin = new(8,0,0,0),
						VerticalAlignment = VerticalAlignment.Center,
						Text = clientName,
						FontStyle = FontStyles.Italic,
						Foreground = Brushes.Gray
					}
				}
			};
		}

		public void AppendToName(bool isBold, string str) 
		{
			this._employeeName.Children.Add( new TextBlock {
				Text = str,
				FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal
			});
		}
	}
}


// public class Form : Popup
// {
// 	public Input[] Inputs;

// 	public Form(int inputs_qty)
// 	{
// 		Inputs = new Input[inputs_qty];
// 	}

// 	protected class Submit : Container
// 	{

// 	}
	
// 	protected void AddInput(Input e)
// 	{
// 	}
	
// }

public class NewClient : Popup
{
    public NewClient() : base() {
		Child = new Border {
			CornerRadius = new(4),
			Background = Brushes.WhiteSmoke,
			Child = new TextBlock {
				Margin = new(10),
				Text = "NewClient"
			}
		};
		
        PlacementTarget = App.Window.ToolsHeader.NewClientButton;
    }
}

public class NewEmployee : Popup
{
    public NewEmployee() {
		Child = new TextBlock {
			Text = "NewEmployee"
		};
        PlacementTarget = App.Window.ToolsHeader.NewEmployeeButton;
    }
}