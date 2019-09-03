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

namespace TrafficWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point startPos = new Point(-1, -1);
        public List<Road> roads = new List<Road>();
        List<Source> sources = new List<Source>();
        List<Destination> destinations = new List<Destination>();
        Line currentRoad;
        Road startRoad;
        Source startSource;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainCanvas_LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!RoadButton.IsEnabled)
            {
                if (startPos == new Point(-1, -1)) StartBuildingRoad(e);
                else FinishBuildingRoad(e);
            }
            else if (!TestButton.IsEnabled)
            {
                ColorRoad(e);
            } else if (!SourceButton.IsEnabled)
            {
                BuildSource(e);
            } else if (!DestinationButton.IsEnabled)
            {
                BuildDestination(e);
            }
        }

        void StartBuildingRoad(MouseButtonEventArgs _e)
        {
            startPos = _e.GetPosition(this);
            currentRoad = new Line();
            MainCanvas.Children.Add(currentRoad);
            currentRoad.Stroke = new SolidColorBrush(Colors.White);
            currentRoad.X1 = startPos.X;
            currentRoad.Y1 = startPos.Y;
            currentRoad.X2 = startPos.X;
            currentRoad.Y2 = startPos.Y;
            double width = 10;
            currentRoad.StrokeThickness = width;

            // See if there are other roads
            Road closestRoad = GetClosestRoad(new Point(currentRoad.X1, currentRoad.Y2));
            double distanceToClosestRoad = -1;
            if (closestRoad != null)
            {
                // See if the closest road is close enough
                Point closestPoint = pDistance(currentRoad.X1, currentRoad.Y1, closestRoad.line.X1, closestRoad.line.Y1, closestRoad.line.X2, closestRoad.line.Y2);
                distanceToClosestRoad = Distance(new Point(currentRoad.X1, currentRoad.Y1), closestPoint);
                if (distanceToClosestRoad < closestRoad.width * 2)
                {
                    startRoad = closestRoad;
                    currentRoad.X1 = closestPoint.X;
                    currentRoad.Y1 = closestPoint.Y;
                }
            }
            Source closestSource = GetClosestSource(new Point(_e.GetPosition(this).X, _e.GetPosition(this).Y));
            if (closestSource != null)
            {
                startRoad = null;
                startSource = closestSource;
                currentRoad.X1 = closestSource.ellipse.Margin.Left + closestSource.ellipse.Width / 2;
                currentRoad.Y1 = closestSource.ellipse.Margin.Top + closestSource.ellipse.Height / 2;
            } 
        }

        void FinishBuildingRoad(MouseButtonEventArgs _e)
        {
            startPos = new Point(-1, -1);
            Road road = new Road(currentRoad, currentRoad.StrokeThickness);
            roads.Add(road);

            Destination closestDestination = GetClosestDestination(new Point(_e.GetPosition(this).X, _e.GetPosition(this).Y));
            if (closestDestination != null)
            {
                road.connectedDestination = closestDestination;
                closestDestination.connectedRoads.Add(road);
                if (startRoad != null)
                {
                    startRoad.connectedRoads.Add(road);
                } else if (startSource != null)
                {
                    road.connectedSource = startSource;
                }
                currentRoad = null;
                startRoad = null;
                startSource = null;
                return;
            }


            Road closestRoad = GetClosestRoad(new Point(currentRoad.X2, currentRoad.Y2));
            if (closestRoad != null)
            {
                road.connectedRoads.Add(closestRoad);
                closestRoad.connectedRoads.Add(road);
                if (startRoad != null)
                {
                    startRoad.connectedRoads.Add(road);
                    road.connectedRoads.Add(startRoad);
                } else if (startSource != null)
                {
                    startSource.connectedRoads.Add(road);
                    road.connectedSource = startSource;
                }
            }
            currentRoad = null;
            startRoad = null;
            startSource = null;
        }

        void BuildSource(MouseButtonEventArgs _e)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 50;
            ellipse.Height = 50;
            ellipse.Margin = new Thickness(_e.GetPosition(this).X - ellipse.Width / 2, _e.GetPosition(this).Y - ellipse.Height / 2, 0, 0);
            MainCanvas.Children.Add(ellipse);
            ellipse.Fill = new SolidColorBrush(Colors.White);
            Source source = new Source(ellipse);
            sources.Add(source);
        }

        void BuildDestination(MouseButtonEventArgs _e)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 50;
            rectangle.Height = 50;
            rectangle.Margin = new Thickness(_e.GetPosition(this).X - rectangle.Width / 2, _e.GetPosition(this).Y - rectangle.Height / 2, 0, 0);
            MainCanvas.Children.Add(rectangle);
            rectangle.Fill = new SolidColorBrush(Colors.White);
            Destination destination = new Destination(rectangle);
            destinations.Add(destination);
        }

        void ColorRoad(MouseButtonEventArgs _e)
        {
            if (roads.Count > 0)
            {
                foreach (Road road in roads)
                {
                    Point closestPoint = pDistance(_e.GetPosition(this).X, _e.GetPosition(this).Y, road.line.X1, road.line.Y1, road.line.X2, road.line.Y2);
                    if (Distance(_e.GetPosition(this), closestPoint) < road.width)
                    {
                        List<Road> blankList = new List<Road>();
                        ResetRoadColors();
                        ColorConnectedRoads(road, blankList, 0);
                        return;
                    }
                }
            }
        }

        private void MainCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPos = new Point(-1, -1);
            MainCanvas.Children.Remove(currentRoad);
            startRoad = null;
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentRoad != null)
            {
                Destination closestDestination = GetClosestDestination(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));

                if (closestDestination != null)
                {
                    currentRoad.X2 = closestDestination.rectangle.Margin.Left + closestDestination.rectangle.Width / 2;
                    currentRoad.Y2 = closestDestination.rectangle.Margin.Top + closestDestination.rectangle.Height / 2;
                    return;
                }

                if (roads.Count > 0)
                {
                    foreach(Road road in roads)
                    {
                        Point closestPoint = pDistance(e.GetPosition(this).X, e.GetPosition(this).Y, road.line.X1, road.line.Y1, road.line.X2, road.line.Y2);
                        if (Distance(e.GetPosition(this), closestPoint) < road.width)
                        {
                            currentRoad.X2 = closestPoint.X;
                            currentRoad.Y2 = closestPoint.Y;
                            return;
                        }
                    }
                }

                currentRoad.X2 = e.GetPosition(this).X;
                currentRoad.Y2 = e.GetPosition(this).Y;
            }
        }
        
        Destination GetClosestDestination(Point _point)
        {
            Destination closestDestination = null;
            double distance = -1;
            foreach (Destination destination in destinations)
            {
                Rectangle rectangle = destination.rectangle;
                double xPos = rectangle.Margin.Left + rectangle.Width / 2;
                double yPos = rectangle.Margin.Top + rectangle.Height / 2;
                if (distance == -1) distance = rectangle.Width;
                double tempDistance = Distance(new Point(xPos, yPos), _point);
                if (tempDistance < distance)
                {
                    closestDestination = destination;
                    distance = tempDistance;
                }
            }
            return closestDestination;
        }

        Source GetClosestSource(Point _point)
        {
            Source closestSource = null;
            double distance = -1;
            foreach(Source source in sources)
            {
                Ellipse ellipse = source.ellipse;
                double xPos = ellipse.Margin.Left + source.ellipse.Width / 2;
                double yPos = ellipse.Margin.Top + source.ellipse.Height / 2;
                if (distance == -1) distance = ellipse.Width;
                double tempDistance = Distance(new Point(xPos, yPos), _point);
                if (tempDistance < distance)
                {
                    closestSource = source;
                    distance = tempDistance;
                }
            }
            return closestSource;
        }

        Road GetClosestRoad(Point _point)
        {
            if (roads.Count > 0)
            {
                foreach (Road road in roads)
                {
                    Point closestPoint = pDistance(_point.X, _point.Y, road.line.X1, road.line.Y1, road.line.X2, road.line.Y2);
                    if (Distance(new Point(_point.X, _point.Y), closestPoint) < road.width)
                    {
                        return road;
                    }
                }
            }
            return null;
        }

        double Distance(Point _from, Point _to)
        {
            
            double dx = _from.X - _to.X;
            double dy = _from.Y - _to.Y;
            return Math.Sqrt(dx * dx + dy * dy);
            
            //return (_to - _from).LengthSquared;
        }
        
        Point pDistance(double x, double y, double x1, double y1, double x2, double y2)
        {

            double A = x - x1;
            double B = y - y1;
            double C = x2 - x1;
            double D = y2 - y1;

            var dot = A * C + B * D;
            var len_sq = C * C + D * D;
            double param = -1;
            if (len_sq != 0) //in case of 0 length line
                param = dot / len_sq;

            double xx;
            double yy;

            if (param < 0)
            {
                xx = x1;
                yy = y1;
            }
            else if (param > 1)
            {
                xx = x2;
                yy = y2;
            }
            else
            {
                xx = x1 + param * C;
                yy = y1 + param * D;
            }

            var dx = x - xx;
            var dy = y - yy;
            return new Point(xx, yy);
        }

        void ResetButtons()
        {
            RoadButton.IsEnabled = true;
            TestButton.IsEnabled = true;
            SourceButton.IsEnabled = true;
            DestinationButton.IsEnabled = true;
            ResetRoadColors();
        }

        private void RoadButton_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            RoadButton.IsEnabled = false;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            TestButton.IsEnabled = false;
            startPos = new Point(-1, -1);
            if (currentRoad != null) MainCanvas.Children.Remove(currentRoad);
        }

        private void SourceButton_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            SourceButton.IsEnabled = false;
        }

        private void DestinationButton_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            DestinationButton.IsEnabled = false;
        }

        void ResetRoadColors()
        {
            foreach(Road road in roads)
            {
                road.line.Stroke = new SolidColorBrush(Colors.White);
            }
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, (byte)v, (byte)t, (byte)p);
            else if (hi == 1)
                return Color.FromArgb(255, (byte)q, (byte)v, (byte)p);
            else if (hi == 2)
                return Color.FromArgb(255, (byte)p, (byte)v, (byte)t);
            else if (hi == 3)
                return Color.FromArgb(255, (byte)p, (byte)q, (byte)v);
            else if (hi == 4)
                return Color.FromArgb(255, (byte)t, (byte)p, (byte)v);
            else
                return Color.FromArgb(255, (byte)v, (byte)p, (byte)q);
        }

        void ColorConnectedRoads(Road _roadNode, List<Road> _checkedRoads, int _depth)
        {
            double hue = 15 * _depth;
            if (hue > 345) hue = 345;
            double saturation = 100;
            double value = 70;


            SolidColorBrush solidColorBrush = new SolidColorBrush(ColorFromHSV(hue, saturation, value));
            


            _roadNode.line.Stroke = solidColorBrush;
            //MainCanvas.Children.Remove(_roadNode.line);
            List<Road> checkedRoads = _checkedRoads;
            checkedRoads.Add(_roadNode);
            int newDepth = _depth + 1;
            foreach(Road road in _roadNode.connectedRoads)
            {
                if (!checkedRoads.Contains(road) || road.depth > newDepth)
                {
                    road.depth = newDepth;
                    ColorConnectedRoads(road, checkedRoads, newDepth);
                }
            }
        }

        
    }
}
