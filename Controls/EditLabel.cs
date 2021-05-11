using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyLights.Controls
{
    public class EditLabel : Control
    {
        private TextBox inputElement;
        private TextBlock labelElement;

        #region Properties
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion


        #region Methods
        private void StartEditing()
        {
            labelElement.Visibility = Visibility.Collapsed;
            inputElement.Visibility = Visibility.Visible;

            var window = Window.GetWindow(this);
            window.PreviewMouseDown += Window_PreviewMouseDown;

            inputElement.Focus();
            inputElement.CaretIndex = inputElement.Text.Length;
            inputElement.SelectAll();
        }

        private void EndEditing()
        {
            var window = Window.GetWindow(this);
            window.PreviewMouseDown -= Window_PreviewMouseDown;

            this.Text = inputElement.Text;

            labelElement.Visibility = Visibility.Visible;
            inputElement.Visibility = Visibility.Collapsed;
        }
        #endregion


        #region Overrides
        public override void OnApplyTemplate()
        {
            inputElement = (TextBox)this.Template.FindName("input", this);
            inputElement.KeyDown += InputElement_KeyDown;

            inputElement.LostFocus += InputElement_LostFocus;

            labelElement = (TextBlock)this.Template.FindName("label", this);

            base.OnApplyTemplate();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            e.Handled = true;
            StartEditing();
        }
        #endregion


        #region EventHandlers
        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMouseOver)
                EndEditing();
        }

        private void InputElement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                inputElement.Text = this.Text;
                EndEditing();
            }

            else if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                EndEditing();
            }
        }

        private void InputElement_LostFocus(object sender, RoutedEventArgs e)
        {
            EndEditing();
        }
        #endregion


        #region PropertyChangedCallbacks
        private void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion


        #region Static

        static EditLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditLabel), new FrameworkPropertyMetadata(typeof(EditLabel)));
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EditLabel),
                new PropertyMetadata("", (s, e) => ((EditLabel)s).OnTextChanged(e)));
        #endregion
    }
}
