using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyLights.Controls
{
    public class RenameText : Control
    {
        public RenameText()
        {
        }

        private bool isEditing;

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
            isEditing = true;
            labelElement.Visibility = Visibility.Collapsed;
            inputElement.Visibility = Visibility.Visible;

            FocusManager.SetFocusedElement(inputElement, inputElement);
            inputElement.CaretIndex = inputElement.Text.Length;
            inputElement.SelectAll();
        }

        private void EndEditing()
        {
            this.Text = inputElement.Text;

            inputElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            isEditing = false;
            labelElement.Visibility = Visibility.Visible;
            inputElement.Visibility = Visibility.Collapsed;
        }
        #endregion


        #region Overrides
        public override void OnApplyTemplate()
        {
            inputElement = (TextBox)this.Template.FindName("input", this);
            inputElement.TextChanged += InputElement_TextChanged;
            inputElement.KeyDown += InputElement_KeyDown;

            labelElement = (TextBlock)this.Template.FindName("label", this);
            base.OnApplyTemplate();
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


        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            StartEditing();
        }
        #endregion


        #region EventHandlers
        private void InputElement_TextChanged(object sender, TextChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        #endregion


        #region PropertyChangedCallbacks
        private void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion


        #region Static

        static RenameText()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RenameText), new FrameworkPropertyMetadata(typeof(RenameText)));
        }



        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RenameText),
                new PropertyMetadata("", (s, e) => ((RenameText)s).OnTextChanged(e)));

        
        #endregion

    }
}
