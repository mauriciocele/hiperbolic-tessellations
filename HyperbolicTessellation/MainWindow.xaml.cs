using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Numerics;
using System.Diagnostics;
using System.ComponentModel;

namespace HyperbolicTessellation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Tessellation tessellation;
        bool IsMoving = false;
        bool IsRotating = false;
        Point previusPoint = new Point(0, 0);
        Brush EvenBrush = new LinearGradientBrush(Colors.Navy, Colors.LightBlue, 0);
        Brush OddBrush = new LinearGradientBrush(Colors.Red, Colors.LightPink, 0);

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            this.SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);
            this.MouseMove += new MouseEventHandler(MainWindow_MouseMove);
            this.MouseDown += new MouseButtonEventHandler(MainWindow_MouseDown);
            this.MouseUp += new MouseButtonEventHandler(MainWindow_MouseUp);
        }

        void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                IsMoving = false;
            if (e.ChangedButton == MouseButton.Right)
                IsRotating = false;
        }

        void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                IsMoving = true;
            if (e.ChangedButton == MouseButton.Right)
                IsRotating = true;
        }

        void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (tessellation != null)
            {
                var point = e.GetPosition(MyCanvas);
                if (IsMoving)
                    tessellation.Move(point, previusPoint);
                if (IsRotating)
                    tessellation.Rotate(point, previusPoint);

                if (IsMoving || IsRotating)
                    Draw(tessellation, EvenBrush, OddBrush);
            }

			previusPoint = e.GetPosition(MyCanvas);
        }

        private void Generar_Click(object sender, RoutedEventArgs e)
        {
            var radius = Math.Min(MyCanvas.ActualHeight, MyCanvas.ActualWidth) * 0.5;
            var center = new Point(MyCanvas.ActualWidth * 0.5, MyCanvas.ActualHeight * 0.5);
            var circle = new Circle(center, radius);
            tessellation = new Tessellation(circle);
            tessellation.Tessellate(int.Parse(P.Text), int.Parse(Q.Text), int.Parse(Level.Text));
            Draw(tessellation, EvenBrush, OddBrush);
        }

        private void Dual_Click(object sender, RoutedEventArgs e)
        {
            if(tessellation != null)
                Draw(tessellation, EvenBrush, OddBrush);            
        }

        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var radius = Math.Min(MyCanvas.ActualHeight, MyCanvas.ActualWidth) * 0.5;
            var center = new Point(MyCanvas.ActualWidth * 0.5, MyCanvas.ActualHeight * 0.5);
            var circle = new Circle(center, radius);
            if (tessellation != null)
            {
                tessellation.Resize(circle);
                Draw(tessellation, EvenBrush, OddBrush);
            }
        }

        void Draw(Tessellation tessellation, Brush evenBrush, Brush oddBrush)
        {
            this.MyCanvas.Children.Clear();
            this.MyCanvas.Children.Add(new Path
            {
                Stroke = new SolidColorBrush(Colors.LightGray),
                StrokeThickness = 1.0,
                Data = new EllipseGeometry(tessellation.Disk.Center, tessellation.Disk.Radius, tessellation.Disk.Radius),
            });

            foreach (var poly in tessellation.Polygons)
            {
                Path path = new Path()
                {
                    Stroke = new SolidColorBrush(Colors.Yellow),
                    StrokeThickness = 1.0,
                };
                if (poly.level % 2 == 0)
                    path.Fill = evenBrush;
                else
                    path.Fill = oddBrush;

                PathFigure figure = new PathFigure() { IsClosed = true, IsFilled = true };
                for (int e = 0; e < poly.edges.Count; ++e)
                {
                    var geodesic = tessellation.GetGeodesicLine(poly.edges[e]);
                    if(e == 0)
                        figure.StartPoint = geodesic.A;
                    figure.Segments.Add(HyperbolicLineSegment(geodesic));
                }
                path.Data = new PathGeometry(new PathFigure[] { figure });
                this.MyCanvas.Children.Add(path);
            }
            
            if(Dual.IsChecked.HasValue && (bool)Dual.IsChecked)
                DrawDual(tessellation);
        }

        void DrawDual(Tessellation tessellation)
        {
            foreach (var poly in tessellation.Polygons)
            {
                Path path = new Path()
                {
                    Stroke = new SolidColorBrush(Colors.Yellow),
                    StrokeThickness = 1.0,
                    Data = new PathGeometry()
                };

                for (int e = 0; e < poly.edges.Count; ++e)
                {
                    if (poly.edges[e].Neighbors.Count == 2)
                    {
                        PathFigure figure = new PathFigure() { IsClosed = false, IsFilled = false };
                        var geodesic = tessellation.GetGeodesicLine(new Edge(poly.edges[e].Neighbors[0], poly.edges[e].Neighbors[1]));
                        figure.StartPoint = geodesic.A;
                        figure.Segments.Add(HyperbolicLineSegment(geodesic));
                        (path.Data as PathGeometry).Figures.Add(figure);
                    }
                }                
                this.MyCanvas.Children.Add(path);
            }
        }

        PathSegment HyperbolicLineSegment(Geodesic geodesic)
        {
            if (geodesic.IsLine)
                return new LineSegment(geodesic.B, true);

            SweepDirection sweepDirection;
            if (Vector.AngleBetween(geodesic.A - geodesic.C.Center, geodesic.B - geodesic.C.Center) < 0)
                sweepDirection = SweepDirection.Counterclockwise;
            else
                sweepDirection = SweepDirection.Clockwise;

            return new ArcSegment(geodesic.B, new Size(geodesic.C.Radius, geodesic.C.Radius), 0, false, sweepDirection, true);
        }
    }
}
