using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using FPMO = System.Windows.FrameworkPropertyMetadataOptions;

namespace MyLights.Controls
{
    public class CircleSegment : FrameworkElement
    {
        public CircleSegment()
        {
            CreateClip();
            Loaded += CircleSegment_Loaded;
        }

        private void CircleSegment_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateClip();
            InvalidateVisual();
        }

        PathGeometry clip;
        PolyLineSegment poly;
        PathFigure fig;
        RotateTransform clipTransform;

        protected override void OnRender(DrawingContext dc)
        {
            double centerX = RenderSize.Width / 2.0;
            double centerY = RenderSize.Height / 2.0;
            double radius = Math.Min(centerX, centerY) - StrokeThickness / 2.0;

            Pen pen = new Pen(Stroke, StrokeThickness);

            dc.PushClip(clip);

            dc.DrawEllipse(Fill, pen, new Point(centerX, centerY), radius, radius);

            dc.Pop();

            if (ShowInnerStroke)
            {
                var offset = new Vector(centerX, centerY);
                dc.DrawLine(pen, OnCircle(radius + StrokeThickness / 2.0, StartAngle) + offset, new Point(centerX, centerY));
                dc.DrawLine(pen, OnCircle(radius + StrokeThickness / 2.0, StartAngle + ArcLength) + offset, new Point(centerX, centerY));
            }

            base.OnRender(dc);
        }

        private void CreateClip()
        {
            clip = new PathGeometry();
            clip.Figures = new PathFigureCollection();

            poly = new PolyLineSegment();

            fig = new PathFigure();
            fig.Segments.Add(poly);

            clip.Figures.Add(fig);

            clipTransform = new RotateTransform();
            clip.Transform = clipTransform;
        }

        private void UpdateClip()
        {
            UpdateClip(ActualWidth, ActualHeight);
        }

        private void UpdateClip(double width, double height)
        {
            Vector center = new Vector(width / 2.0, height / 2.0);
            fig.StartPoint = new Point(center.X, center.Y);

            double radius = Math.Min(width, height);

            var points = poly.Points;
            points.Clear();

            double angle = ArcLength;

            int full = (int)(angle / 45);

            for (int i = 0; i <= full; i++)
            {
                points.Add(OnCircle(radius, i * 45.0) + center);
            }

            points.Add(OnCircle(radius, angle) + center);

            clipTransform.CenterX = center.X;
            clipTransform.CenterY = center.Y;
            clipTransform.Angle = StartAngle;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double side = Math.Min(availableSize.Width, availableSize.Height);

            if (double.IsInfinity(side))
                side = 100.0;

            return new Size(side, side);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            UpdateClip(finalSize.Width, finalSize.Height);
            return base.ArrangeOverride(finalSize);
        }

        private Point OnCircle(double radius, double degrees)
        {
            double angle = degrees / 360.0 * 2.0 * Math.PI;

            double x = radius * Math.Cos(angle);
            double y = radius * Math.Sin(angle);

            return new Point(x, y);
        }

        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(CircleSegment),
                new FrameworkPropertyMetadata(0.0, FPMO.AffectsRender, (s, e) => ((CircleSegment)s).UpdateClip()));

        public double ArcLength
        {
            get { return (double)GetValue(ArcLengthProperty); }
            set { SetValue(ArcLengthProperty, value); }
        }

        public static readonly DependencyProperty ArcLengthProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(CircleSegment),
                new FrameworkPropertyMetadata(180.0, FPMO.AffectsRender, (s, e) => ((CircleSegment)s).UpdateClip()));

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(CircleSegment),
                new FrameworkPropertyMetadata(new SolidColorBrush(), FPMO.AffectsRender));

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(CircleSegment),
                new FrameworkPropertyMetadata(0.0, FPMO.AffectsRender));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(CircleSegment),
                new FrameworkPropertyMetadata(new SolidColorBrush(), FPMO.AffectsRender));

        public bool ShowInnerStroke
        {
            get { return (bool)GetValue(ShowInnerStrokeProperty); }
            set { SetValue(ShowInnerStrokeProperty, value); }
        }

        public static readonly DependencyProperty ShowInnerStrokeProperty =
            DependencyProperty.Register("ShowInnerStroke", typeof(bool), typeof(CircleSegment),
                new FrameworkPropertyMetadata(true, FPMO.AffectsRender));

    }
}
