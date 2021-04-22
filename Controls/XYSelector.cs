using MyLights.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyLights.Controls
{
    public class XYSelector : Control
    {
        public XYSelector()
        {
            Loaded += XYSelector_Loaded;
        }


        FrameworkElement thumb;
        TranslateTransform transform;
        bool isDragging = false;

        private void XYSelector_Loaded(object sender, RoutedEventArgs e)
        {
            thumb = (FrameworkElement)GetTemplateChild("thumb");
            if (thumb != null)
            {
                //thumb.DragDelta += Thumb_DragDelta;
                //thumb.DragStarted += Thumb_DragStarted;
                //thumb.DragOver += Thumb_DragOver;

                transform = new TranslateTransform();
                thumb.RenderTransform = transform;

                UpdateThumb(x: ValueX, y: ValueY);
                // UpdateValues();
            }
        }

        public Point ThumbOffset
        {
            get
            {
                if (transform == null)
                    return new Point(0.0, 0.0);
                return new Point(transform.X, transform.Y);
            }
            set
            {
                if (transform == null)
                    return;

                transform.X = value.X.Clamp(TrackWidth / -2.0, TrackWidth / 2.0);
                transform.Y = value.Y.Clamp(TrackHeight / -2.0, TrackHeight / 2.0);

                UpdateValues();
            }
        }
        public double TrackWidth
        {
            get
            {
                return ActualWidth - ThumbSize / 2.0;
            }
        }
        public double TrackHeight
        {
            get
            {
                return ActualHeight - ThumbSize / 2.0;
            }
        }
        [Category("Appearance")]
        public double ThumbSize
        {
            get { return (double)GetValue(ThumbSizeProperty); }
            set { SetValue(ThumbSizeProperty, value); }
        }
        [Category("Appearance")]
        public double ThumbStrokeSize
        {
            get { return (double)GetValue(ThumbStrokeSizeProperty); }
            set { SetValue(ThumbStrokeSizeProperty, value); }
        }
        [Category("Brush")]
        public Brush ThumbFill
        {
            get { return (Brush)GetValue(ThumbFillProperty); }
            set { SetValue(ThumbFillProperty, value); }
        }
        [Category("Brush")]
        public Brush ThumbStroke
        {
            get { return (Brush)GetValue(ThumbStrokeProperty); }
            set { SetValue(ThumbStrokeProperty, value); }
        }
        [Category("Common")]
        public double ValueX
        {
            get { return (double)GetValue(ValueXProperty); }
            set { SetValue(ValueXProperty, value); }
        }
        [Category("Common")]
        public double ValueY
        {
            get { return (double)GetValue(ValueYProperty); }
            set { SetValue(ValueYProperty, value); }
        }
        [Category("Common")]
        public Point Value
        {
            get { return (Point)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        [Category("Common")]
        public Rect ValueRange
        {
            get { return (Rect)GetValue(ValueRangeProperty); }
            set { SetValue(ValueRangeProperty, value); }
        }
        [Category("Appearance")]
        public Visibility TextVisibility
        {
            get { return (Visibility)GetValue(TextVisibilityProperty); }
            set { SetValue(TextVisibilityProperty, value); }
        }
        public string DisplayValue
        {
            get { return (string)GetValue(DisplayValueProperty); }
            private set { SetValue(DisplayValuePropertyKey, value); }
        }

        private void UpdateValues()
        {
            //CoerceValue(ValueXProperty);
            //CoerceValue(ValueYProperty);
            //CoerceValue(ValueProperty);

            double xr = ThumbOffset.X / TrackWidth + 0.5;
            double xrange = ValueRange.Right - ValueRange.Left;
            ValueX = xr * xrange + ValueRange.Left;

            double yr = ThumbOffset.Y / TrackHeight + 0.5;
            double yrange = ValueRange.Bottom - ValueRange.Top;
            ValueY = yr * yrange + ValueRange.Top;
        }

        private void UpdateThumb(double x = double.NaN, double y = double.NaN)
        {
            double tx = ThumbOffset.X;
            double ty = ThumbOffset.Y;

            if (!double.IsNaN(x))
            {
                double width = ValueRange.Right - ValueRange.Left;
                double xr = (x - ValueRange.Left) / width;
                tx = (x - 0.5) * TrackWidth;
            }
            if (!double.IsNaN(y))
            {
                double height = ValueRange.Bottom - ValueRange.Top;
                double yr = (y - ValueRange.Top) / height;
                ty = (y - 0.5) * TrackHeight;
            }

            ThumbOffset = new Point(tx, ty);
        }

        private void MoveThumb(Point position)
        {
            ThumbOffset = new Point(position.X - TrackWidth / 2.0, position.Y - TrackHeight / 2.0);
        }

        #region Overrides
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (CaptureMouse())
            {
                isDragging = true;
                MoveThumb(e.GetPosition(this));
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging)
            {
                MoveThumb(e.GetPosition(this));
            }
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            isDragging = false;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
                MoveThumb(e.GetPosition(this));
                isDragging = false;
            }
        }


        #endregion

        #region PropertyChangedCallbacks
        private void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            //double width = ValueRange.Right - ValueRange.Left;
            //double xr = (ValueX - ValueRange.Left) / width;

            //double height = ValueRange.Bottom - ValueRange.Top;
            //double yr = (ValueY - ValueRange.Top) / height;

            //UpdateThumb(x: xr, y: yr);

            if (ValueX != Value.X)
                ValueX = Value.X;

            if (ValueY != Value.Y)
                ValueY = Value.Y;

            if (!isDragging)
                UpdateThumb(Value.X, Value.Y);

            CoerceValue(DisplayValueProperty);
        }

        private void OnValueXChanged(DependencyPropertyChangedEventArgs e)
        {
            //double width = ValueRange.Right - ValueRange.Left;
            //double xr = (ValueX - ValueRange.Left) / width;

            if (Value.X != ValueX)
                Value = new Point(ValueX, Value.Y);

            if (!isDragging)
                UpdateThumb(x: ValueX);
        }

        private void OnValueYChanged(DependencyPropertyChangedEventArgs e)
        {
            //double height = ValueRange.Bottom - ValueRange.Top;
            //double yr = (ValueY - ValueRange.Top) / height;

            if (Value.Y != ValueY)
                Value = new Point(Value.X, ValueY);

            if (!isDragging)
                UpdateThumb(y: ValueY);
        }


        private void OnValueRangeChanged(DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion


        #region CoerceValueCallbacks
        private object CoerceValue(Point value)
        {
            return new Point((double)CoerceValueX(value.X), (double)CoerceValueY(value.Y));
        }

        private object CoerceValueX(double value)
        {
            //double xr = ThumbOffset.X / TrackWidth + 0.5;
            //double range = ValueRange.Right - ValueRange.Left;

            //return xr * range + ValueRange.Left;

            return value.Clamp(ValueRange.Left, ValueRange.Right);
        }

        private object CoerceValueY(double value)
        {
            //double yr = ThumbOffset.Y / TrackHeight + 0.5;
            //double range = ValueRange.Bottom - ValueRange.Top;

            //return yr * range + ValueRange.Top;

            return value.Clamp(ValueRange.Top, ValueRange.Bottom);
        }

        private object CoerceDisplayText(object value)
        {
            return $"({ValueX.ToString("F3")}, {ValueY.ToString("F3")})";
        }
        #endregion


        #region Static
        static XYSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XYSelector), new FrameworkPropertyMetadata(typeof(XYSelector)));
        }

        public static readonly DependencyProperty ThumbSizeProperty =
            DependencyProperty.Register("ThumbSize", typeof(double), typeof(XYSelector),
                new PropertyMetadata(10.0));

        public static readonly DependencyProperty ThumbFillProperty =
            DependencyProperty.Register("ThumbFill", typeof(Brush), typeof(XYSelector),
                new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty ThumbStrokeProperty =
            DependencyProperty.Register("ThumbStroke", typeof(Brush), typeof(XYSelector),
                new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty ThumbStrokeSizeProperty =
            DependencyProperty.Register("ThumbStrokeSize", typeof(double), typeof(XYSelector),
                new PropertyMetadata(1.0));

        public static readonly DependencyProperty ValueXProperty =
            DependencyProperty.Register("ValueX", typeof(double), typeof(XYSelector),
                new PropertyMetadata(0.0, (s, e) => ((XYSelector)s).OnValueXChanged(e),
                                             (s, value) => ((XYSelector)s).CoerceValueX((double)value)));

        public static readonly DependencyProperty ValueYProperty =
            DependencyProperty.Register("ValueY", typeof(double), typeof(XYSelector),
                new PropertyMetadata(0.0, (s, e) => ((XYSelector)s).OnValueYChanged(e),
                    (s, value) => ((XYSelector)s).CoerceValueY((double)value)));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Point), typeof(XYSelector),
                new PropertyMetadata(default(Point), (s, e) => ((XYSelector)s).OnValueChanged(e),
                    (s, value) => ((XYSelector)s).CoerceValue((Point)value)));

        public static readonly DependencyProperty ValueRangeProperty =
            DependencyProperty.Register("ValueRange", typeof(Rect), typeof(XYSelector),
                new PropertyMetadata(new Rect(0, 0, 10, 10), (s, e) => ((XYSelector)s).OnValueRangeChanged(e)));

        public static readonly DependencyProperty TextVisibilityProperty =
            DependencyProperty.Register("TextVisibility", typeof(Visibility), typeof(XYSelector),
                new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyPropertyKey DisplayValuePropertyKey =
            DependencyProperty.RegisterReadOnly("DisplayValue", typeof(string), typeof(XYSelector),
                new PropertyMetadata("", null, (s, value) => ((XYSelector)s).CoerceDisplayText(value)));

        public static readonly DependencyProperty DisplayValueProperty = DisplayValuePropertyKey.DependencyProperty;
        #endregion
    }
}
