using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using ActivAndZen.Popups;
using ActivAndZen.Styles;
using ActivAndZen.Components;

namespace ActivAndZen.Inputs;

/*-- Inputs Classes --*/
// TransparantTextBox
// TextInput

public class TransparantTextBox : TextBox
{
	public TextInput ?ParentContainer;
    private string ?_placeholder;
    public bool IsTextPlaceholder;

    public string Placeholder
    {
        get => _placeholder != null ? _placeholder : string.Empty;
        set {
            this.Text = value;
            _placeholder = value;
            IsTextPlaceholder = true;
        }
    }

    public TransparantTextBox() {
        this.IsTextPlaceholder = false;
        this.FontSize = Spacings.FontSize.Medium;
        this.Background = Brushes.Transparent;
        this.Background.Freeze();
        this.BorderThickness = new(0);
        this.VerticalAlignment = VerticalAlignment.Center;
    }

    public void SetPlaceholder()
    {
        this.Text =_placeholder;
        IsTextPlaceholder = true;
    }

    public void UnSetPlaceholder()
    {
        this.Text = string.Empty;
        IsTextPlaceholder = false;

    }

    protected override void OnGotFocus(RoutedEventArgs e) {
        base.OnGotFocus(e);
        MainWindow.TextBoxIsFocus = true;
        if (IsTextPlaceholder){
            UnSetPlaceholder();
        }
    }

    public bool ShouldLoseFocus()
    {
    	if (ParentContainer != null) {
    		return	MouseTracker.LastClickDown.UIElement 		!= ParentContainer
	         		&&  MouseTracker.LastClickDown.UIElement 	!= ParentContainer.ResetButton
		         	&&  ParentContainer.ResetButton.Child 		is Grid resetButtonGrid
		         	&&  resetButtonGrid.Children[0] 			is Grid resetButtonGridChild
		         	&&  MouseTracker.LastClickDown.UIElement 	!= resetButtonGridChild;
    	}
    	return false;
    }

    protected override void OnLostFocus(RoutedEventArgs e) {
		base.OnLostFocus(e);
	    if (Text.Length == 0){
	        SetPlaceholder();
	    }
	    // App.Window.ActivePopup = null;
    }

    protected override void OnTextChanged(TextChangedEventArgs e) {
        if (!App.IsInit) return;

        if (this.IsFocused && this.Text.Length != 0) {
            ParentContainer!.ResetButton.Visibility = Visibility.Visible;
        } else {
            ParentContainer!.ResetButton.Visibility = Visibility.Collapsed;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        // e.Handled = true;
        //base.OnKeyDown(e);
    }
}

public class Input : Container
{

    public virtual void Clear() {}
    public virtual bool Validate() { return true; }
    public virtual string GetData() { return string.Empty; }
}

public class TextInput : Input
{
    private Brush ?_focusBorderColor;

    public Brush FocusBorderColor
    {
        get => _focusBorderColor != null ? _focusBorderColor : Brushes.Transparent;
        set => _focusBorderColor = value;
    }

    private TransparantTextBox ?_textArea;
    public TransparantTextBox TextArea {
        get {
            if (_textArea != null) return _textArea;
            else throw new NotImplementedException();
        }
        set {

            /* -------------------- TEXT AREA DEFINITION BEGIN --------------------*/

            _textArea = value;

            _textArea.GotFocus += (e,s) => {
                this.BorderBrush = this.FocusBorderColor;
                this.Background = HoverBgColor;
            };

            this.TextArea.LostFocus += (e,s) => {
                this.BorderBrush = this.BorderColor;
                this.Background = this.BackgroundColor;
            };

            this.MouseLeftButtonUp += MouseTracker.DefineClick(() => {
                if (!this.TextArea.IsFocused) this.TextArea.Focus();
            });

            /* -------------------- TEXT AREA DEFINITION END --------------------*/


            /* -------------------- RESET BUTTON DEFINITION BEGIN --------------------*/

            double reset_btn_iconSize = 9;
            double reset_btn_Size = reset_btn_iconSize + 13;

            this.ResetButton = new Container {
                // VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,

                Height = reset_btn_Size,
                Width = reset_btn_Size,
                //BorderThickness = new(0.5),
                //BorderColor = Brushes.Black,
                //Margin = new(reset_btn_Size *0.2),
                HoverBgColor = Brushes.LightGray,
                CornerRadius = BorderRadius.r0,
                Child = new IconGrid(reset_btn_iconSize, reset_btn_iconSize, App.Icons.Cross)
            };

            ResetButton.MouseLeftButtonUp += MouseTracker.DefineClick(() => {
                this.TextArea.Text = string.Empty;
                this.TextArea.Focus();
                this.TextArea.IsTextPlaceholder = false;
            });

            ResetButton.MouseEnter += (e,s) => this.Cursor = Cursors.Hand;
            ResetButton.MouseLeave += (e,s) => this.Cursor = Cursors.IBeam;

            ResetButton.Visibility = Visibility.Collapsed;

            /* -------------------- RESET BUTTON DEFINITION END -------------------- */
        }
    }
    private Container ?_resetButton;
    public Container ResetButton
    {
        get {
            if (_resetButton != null) return _resetButton;
            else throw new NotImplementedException();
        }
        set => _resetButton = value;
    }

    private bool _defaultImplementation = false;
    public bool DefaultImplementation
    {
        get => _defaultImplementation;
        set {
            _defaultImplementation = value;
            if (_defaultImplementation) {
                this.TextArea = new(){
                    ParentContainer = this
                };

                Child = new GridExt {
                    Columns = [
                        new GridElement(new(1, GridUnitType.Star), TextArea),
                        new GridElement(new(40, GridUnitType.Pixel), ResetButton)
                    ]
                };
            }
        }
    }

    public TextInput()
    {    	
        Cursor = Cursors.IBeam;

        Margin = new(4);

        BackgroundColor = Utils.RGBA("#f0f0f0");
        HoverBgColor = Brushes.WhiteSmoke;
        BorderColor = Brushes.LightGray;
        FocusBorderColor = Brushes.Gray;

        BorderThickness = new(Spacings.Border.Medium);
        CornerRadius = BorderRadius.r0;
    }

    public override string GetData()
    {
        return TextArea.Text;
    }

    public override void Clear()
    {
        this.TextArea.SetPlaceholder();
    }

    protected override void OnMouseLeave(MouseEventArgs e){
        if (!this.TextArea.IsFocused) {
            base.OnMouseLeave(e);
        }
    }
}


public class Form : Container
{
    private Input[] ?_inputs;

    public Input[]? Inputs
    {
        get => _inputs;
        set
        {
            if (value == null || value.Length == 0)
            {
                _inputs = null;
                return;
            }

            _inputs = value;


            Child = new StackPanel() {
                Orientation = Orientation.Vertical,
                Margin = new(Spacings.Margin.Medium)
            };

            if (Child is StackPanel sp) {
                foreach(Input i in _inputs) sp.Children.Add(i);
                sp.Children.Add(SubmitBtn);

                FocusManager.SetIsFocusScope(sp, true);
                KeyboardNavigation.SetTabNavigation(sp, KeyboardNavigationMode.Cycle);
            }
        }
    }


    public Form.SubmitButton SubmitBtn;

    public Form()
    {
        SubmitBtn = new();
        Padding = new(Spacings.Padding.Large);
        BackgroundColor = Brushes.White;
        CornerRadius = new(4);
        BorderThickness = new(1);
        BorderColor = Brushes.Gray;
    }

    public class SubmitButton : Input
    {
        public SubmitButton()
        {
            Height = 20;
            Width = 60;
            BorderThickness = new(Spacings.Border.Medium);
            BackgroundColor = Brushes.AliceBlue;
            CornerRadius = BorderRadius.r1;
            Focusable = true;
            FocusVisualStyle = null;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            // base.OnGotFocus(e);
            // MessageBox.Show("JustGotFocused");
            //e.Handled = true;
            BackgroundColor = Brushes.FloralWhite;
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            // base.OnGotFocus(e);
            // MessageBox.Show("JustGotFocused");
            //e.Handled = true;
            BackgroundColor = Brushes.Green;
        }
    }
}