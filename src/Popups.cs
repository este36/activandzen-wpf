using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
// using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
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

	/// <summary>
	/// Met à jour l’adorner en fonction des propriétés PlacementTarget et Child.
	/// Si l’adorner était ouvert, il est fermé, recréé et réouvert.
	/// </summary>
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
	private class ItemCard : StackPanel
	{
		
	}
	
    public Search () {
        PlacementTarget = App.Window.Header.SearchBar;
        IsOpen = false;

    	this.Child = new Border {
			Width = App.Window.Header.SearchBar.TotalWidth,
			Background = Brushes.WhiteSmoke,
			CornerRadius = new(8),
			BorderThickness = new(0.5),
			BorderBrush = Brushes.LightGray,
			Padding = new(10),
			Child = new StackPanel {
				Children = {
					new StackPanel {
						Orientation = Orientation.Horizontal,
						Children = {
							new TextBlock {
								Text = "it"
							},
							new TextBlock {
								Text = "e",
								FontWeight = FontWeights.Bold
							},
							new TextBlock {
								Text = "m"
							}							
						}
						
					}
				}
			}
		};
    }
}

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