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

namespace krzywa_beziera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       private List<Point> points;
        private List<Line> lines;
        Ellipse pressedShape;
        Point selectedShapePoint;
        Point startPoint = new Point();
        Point ellipse;
        public MainWindow()
        {
            InitializeComponent();
            points = new List<Point>();
            lines = new List<Line>();
        }

        private void Canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                startPoint = e.GetPosition(this);
            var canvas = sender as Canvas;
            if (canvas == null)
                return;
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, e.GetPosition(canvas));
            var element = hitTestResult.VisualHit;
            if (element.GetType() == typeof(Ellipse))
            {
                pressedShape = (Ellipse)element;
                selectedShapePoint.Y = Canvas.GetTop(pressedShape)+4;
                selectedShapePoint.X = Canvas.GetLeft(pressedShape)+4;
                XCoordinateTextBox.Text = selectedShapePoint.X.ToString();
                YCoordinateTextBox.Text = selectedShapePoint.Y.ToString();
                ellipse = points.Where(s => Contains(pressedShape, s)).Single();
            }
            else
            {
                pressedShape = null;

                Ellipse shape = new Ellipse { Width = 8, Height = 8 };
                Canvas.SetTop(shape, e.GetPosition(paintSurface).Y - 4);
                Canvas.SetLeft(shape, e.GetPosition(paintSurface).X - 4);
                shape.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                shape.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                shape.StrokeThickness = 2;
                paintSurface.Children.Add(shape);
                points.Add(new Point(e.GetPosition(paintSurface).X, e.GetPosition(paintSurface).Y));
                if (points.Count > 1)
                    drawBezier();
            }
            
        }
        private void Canvas_Mouse_Move(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && pressedShape != null)
                if (pressedShape.GetType() == typeof(Ellipse))
                {
                    Point newPoint = new Point(selectedShapePoint.X - (startPoint.X - e.GetPosition(this).X), selectedShapePoint.Y - (startPoint.Y - e.GetPosition(this).Y));

                    ellipse = points.Where(s => Contains(pressedShape,s)).Single();
                    int index=points.IndexOf(ellipse);
                    points.RemoveAt(index);
                    points.Insert(index, newPoint);

                    XCoordinateTextBox.Text = (newPoint.X).ToString();
                    YCoordinateTextBox.Text = (newPoint.Y).ToString();
                    Canvas.SetTop(pressedShape, newPoint.Y -4);
                    Canvas.SetLeft(pressedShape, newPoint.X-4 );
                    if (points.Count > 1)
                        drawBezier();

                }

        }
        public bool Contains(Ellipse Ellipse, Point location)
        {
            Point center = new Point(
                  Canvas.GetLeft(Ellipse) + (Ellipse.Width / 2),
                  Canvas.GetTop(Ellipse) + (Ellipse.Height / 2));

            double _xRadius = Ellipse.Width / 2;
            double _yRadius = Ellipse.Height / 2;


            if (_xRadius <= 0.0 || _yRadius <= 0.0)
                return false;
            /* This is a more general form of the circle equation
             *
             * X^2/a^2 + Y^2/b^2 <= 1
             */

            Point normalized = new Point(location.X - center.X,
                                         location.Y - center.Y);

            return ((double)(normalized.X * normalized.X)
                     / (_xRadius * _xRadius)) + ((double)(normalized.Y * normalized.Y) / (_yRadius * _yRadius))
                <= 1.0;
        }

        private void Draw_Bezier_Click(object sender, RoutedEventArgs e)
        {
            drawBezier();

        }

        public void drawBezier()
        {
            for (int i = 0; i < lines.Count; i++)
                paintSurface.Children.Remove(lines[i]);
            lines.Clear();
            List<Point> beizerPoints = Beizer.calculate(points, 0.01);
            for (int i = 0; i < beizerPoints.Count - 1; i++)
            {
                Shape shape = new Line { X1 = beizerPoints[i].X, Y1 = beizerPoints[i].Y, X2 = beizerPoints[i + 1].X, Y2 = beizerPoints[i + 1].Y };
                lines.Add((Line)shape);
                shape.Stroke = SystemColors.WindowFrameBrush;
                shape.StrokeThickness = 2;
                paintSurface.Children.Add(shape);
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            Point newPoint = new Point(Int32.Parse(XCoordinateTextBox.Text), Int32.Parse(YCoordinateTextBox.Text));
            int index = points.IndexOf(ellipse);
            points.RemoveAt(index);
            points.Insert(index,newPoint);
            Canvas.SetTop(pressedShape, newPoint.Y - 4);
            Canvas.SetLeft(pressedShape, newPoint.X - 4);
            if (points.Count > 1)
                drawBezier();
        }
    }
}
