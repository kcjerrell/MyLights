using MyLights.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MyLights.AltViews
{
    public class ArcPanel : Panel
    {
        public ArcPanel()
        {
            //var figure = new PathFigure() { StartPoint = new Point(0, 0) };
            //figure.Segments.Add(arcSegment);


            //unflattened.Figures.Add(figure);

            //ArcGeometry = unflattened.GetFlattenedPathGeometry();


        }

        //private ArcSegment arcSegment =
        //    new ArcSegment(new Point(0.0, 100.0), new Size(1.0, 2.0), 0.0, false, SweepDirection.Clockwise, true);

        private PathGeometry flattened;
        private List<Point> pathPoints;

        void UpdateArc()
        {
            flattened = ArcGeometry.GetFlattenedPathGeometry();
            pathPoints = GetPathPoints(flattened);

            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (flattened == null)
            {
                foreach (UIElement child in Children)
                {
                    child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));
                }

                return finalSize;
            }

            double distance = GetSegmentLengths(pathPoints).Sum() / (Children.Count + 1);

            if (distance == 0)
                return finalSize;

            var segLengths = new double[Children.Count + 1].Fill(distance);

            var childPositions = GeometryHelper.GetIntersectionPoints(pathPoints, segLengths);


            int i = 0;
            foreach (UIElement child in Children)
            {
                double offset = GetOffset(child);
                var center = new Vector(offset, child.DesiredSize.Height / 2.0);
                var position = childPositions[i + 1] - center;

                child.Arrange(new Rect(position, child.DesiredSize));

                i++;
            }

            return finalSize;
        }

        public static readonly DependencyProperty ArcGeometryProperty =
            DependencyProperty.Register("ArcGeometry", typeof(PathGeometry), typeof(ArcPanel),
                new PropertyMetadata(null, (s, e) => ((ArcPanel)s).ArcGeometryChanged(e)));

        private void ArcGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is PathGeometry oldValue)
            {
                oldValue.Changed -= ArcGeometryChanged;
            }

            if (e.NewValue is PathGeometry newValue)
            {
                newValue.Changed += ArcGeometryChanged;
            }

            UpdateArc();
        }

        private void ArcGeometryChanged(object sender, EventArgs e)
        {
            UpdateArc();
        }

        public PathGeometry ArcGeometry
        {
            get { return (PathGeometry)GetValue(ArcGeometryProperty); }
            set { SetValue(ArcGeometryProperty, value); }
        }



        public static double GetOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(OffsetProperty);
        }

        public static void SetOffset(DependencyObject obj, double value)
        {
            obj.SetValue(OffsetProperty, value);
        }

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.RegisterAttached("Offset", typeof(double), typeof(ArcPanel), new PropertyMetadata(0.0));



        private static List<Point> GetPathPoints(PathGeometry flattened)
        {
            if (flattened.Figures.Count > 1)
            {
                throw new Exception("wrong.");
            }

            var figure = flattened.Figures[0];

            var flattenedPoints = new List<Point>();

            foreach (PathSegment segment in figure.Segments)
            {
                if (segment is PolyLineSegment poly)
                {
                    foreach (Point point in poly.Points)
                    {
                        flattenedPoints.Add(point);
                    }
                }

                else if (segment is LineSegment line)
                {
                    flattenedPoints.Add(line.Point);
                }

                else
                {
                    throw new Exception("whoah wrong kind of segment");
                }
            }

            return flattenedPoints;
        }

        private static List<double> GetSegmentLengths(List<Point> pathPoints)
        {
            var segLengths = new List<double>(pathPoints.Count);

            for (int i = 1; i < pathPoints.Count; i++)
            {
                segLengths.Add(pathPoints[i].DistanceFrom(pathPoints[i - 1]));
            }

            return segLengths;
        }


        //private static List<Point> DividePath(List<Point> pathPoints, List<double> segLengths, int subdivisions)
        //{
        //    var subPoints = new List<Point>(n + 1);
        //    subPoints.Add(pathPoints[0]);
        //
        //    double pathLength = segLengths.Sum();
        //    double subLength = pathLength / subdivisions;
        //
        //    double distance = 0.0;
        //    double travelled = 0.0; // fuck it i'm just going to use the metaphor of travelling down a path
        //
        //    int sub = 1;
        //
        //    double next = sub * subLength;
        //
        //
        //    for (int i = 0; i < segLengths.Count; i++)
        //    {
        //        distance += segLengths[i];
        //
        //        while (distance > next)
        //        {
        //            double legR = distance 
        //        }
        //    }
        //
        //
        //
        //}
    }
}
