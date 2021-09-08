using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MyLights.Controls
{
    public class EllipseSegment : FrameworkElement
    {
        public EllipseSegment()
        {
            CreateGeometry();
        }

        Path leftPath = new();
        Path rightPath = new();

        ArcSegment leftArc;
        ArcSegment rightArc;

        PathFigure leftFig;
        PathFigure rightFig;

        RectangleGeometry leftClip;
        RectangleGeometry rightClip;

        RotateTransform leftTransform;
        RotateTransform rightTransform;

        private void CreateGeometry()
        {
            var top = new Point(Width / 2.0, 0);
            var bottom = new Point(Width / 2.0, Height);

            leftArc = new ArcSegment(bottom, new Size(1, 1), 0, false, SweepDirection.Clockwise, true);
            leftFig = new PathFigure(top, leftArc.SingleEnumerator(), false);
                     
            leftPath.Data = new PathGeometry(leftFig.SingleEnumerator());
            leftTransform = new RotateTransform(0, Width / 2, Height / 2);
            leftClip = new RectangleGeometry(new Rect(0, 0, Width / 2, Height), 0, 0, leftTransform);
            leftPath.Clip = leftClip;

            rightArc = new ArcSegment(top, new Size(1, 1), 0, false, SweepDirection.Clockwise, true);
            rightFig = new PathFigure(bottom, leftArc.SingleEnumerator(), false);

            rightPath.Data = new PathGeometry(rightFig.SingleEnumerator());
            rightTransform = new RotateTransform(0, Width / 2, Height / 2);
            rightClip = new RectangleGeometry(new Rect(0, 0, Width / 2, Height), 0, 0, rightTransform);
            rightPath.Clip = rightClip;
            
        }

        protected override int VisualChildrenCount => 2;

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return leftPath;
            else if (index == 1)
                return rightPath;
            else
                throw new IndexOutOfRangeException();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double hw = finalSize.Width / 2.0;
            double hh = finalSize.Height / 2.0;

            Point top = new Point(hw, 0);
            Point bottom = new Point(hw, finalSize.Height);

            leftPath.Arrange(new Rect(0, 0, hw, finalSize.Height));
            leftFig.StartPoint = top;
            leftArc.Point = bottom;

            rightPath.Arrange(new Rect(hw, 0, hw, finalSize.Height));
            rightFig.StartPoint = bottom;
            rightArc.Point = top;

            return finalSize;
        }



        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(EllipseSegment),
                new PropertyMetadata(0.0, (s, e) => ((EllipseSegment)s).OnStartAngleChanged(e)));

        private void OnStartAngleChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }

        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(EllipseSegment),
                new PropertyMetadata(1.0, (s, e) => ((EllipseSegment)s).OnEndAngleChanged(e)));

        private void OnEndAngleChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(EllipseSegment),
                new PropertyMetadata(new SolidColorBrush(), (s, e) => ((EllipseSegment)s).OnStrokeChanged(e)));

        private void OnStrokeChanged(DependencyPropertyChangedEventArgs e)
        {
            leftPath.Stroke = Stroke;
            rightPath.Stroke = Stroke;
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(EllipseSegment),
                new PropertyMetadata(0.0, (s, e) => ((EllipseSegment)s).OnStrokeThicknessChanged(e)));

        private void OnStrokeThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            leftPath.StrokeThickness = StrokeThickness;
            rightPath.StrokeThickness = StrokeThickness;
        }



    }
}
