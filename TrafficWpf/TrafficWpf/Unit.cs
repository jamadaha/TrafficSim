using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;

namespace TrafficWpf
{
    public class Unit
    {
        public Road[] route { get; set; }
        public Road currentRoad { get; set; }
        public Ellipse ellipse { get; set; }
        int currentIndex = 0;

        public Unit (Road[] _route, Ellipse _ellipse)
        {
            route = _route;
            ellipse = _ellipse;
            route[0].unitsOnRoad.Add(this);
            currentRoad = route[0];
        }

        public bool Move(Unit[] _units)
        {
            Point targetPos;
            if (currentIndex == route.Length - 1)
            {
                targetPos = new Point(route[currentIndex].connectedDestination.rectangle.Margin.Left + route[currentIndex].connectedDestination.rectangle.Width / 2, route[currentIndex].connectedDestination.rectangle.Margin.Top + route[currentIndex].connectedDestination.rectangle.Height / 2);
            } else
            {
                if (pDistance(route[currentIndex].line.X2, route[currentIndex].line.Y2, route[currentIndex + 1].line.X1, route[currentIndex + 1].line.Y1, route[currentIndex + 1].line.X2, route[currentIndex + 1].line.Y2) < 1)
                {
                    targetPos = new Point(route[currentIndex].line.X2, route[currentIndex].line.Y2);
                } else
                {
                    targetPos = new Point(route[currentIndex + 1].line.X1, route[currentIndex + 1].line.Y1);
                }
            }

            double distanceToTargetPos = Math.Sqrt(Math.Pow((targetPos.X - ellipse.Margin.Left), 2) + Math.Pow((targetPos.Y - ellipse.Margin.Top), 2));

            double m = ((targetPos.Y - ellipse.Margin.Top - ellipse.Height / 2) / (targetPos.X - ellipse.Margin.Left - ellipse.Width / 2));

            double xMovement = 1;

            //ellipse.Margin = new Thickness(route[currentIndex].line.X1 - ellipse.Width / 2, route[currentIndex].line.Y1 - ellipse.Height / 2, 0, 0);

            bool xFlip = false; 
            if (targetPos.X > ellipse.Margin.Left + ellipse.Width / 2)
            {
                //ellipse.Margin = new Thickness(route[currentIndex].line.X1 - ellipse.Width / 2, route[currentIndex].line.Y1 - ellipse.Height / 2, 0, 0);
                xFlip = (ellipse.Margin.Left + xMovement >= route[currentIndex].line.X2 - ellipse.Width / 2);
            }
            if (targetPos.X < ellipse.Margin.Left + ellipse.Width / 2)
            {
                //
                xMovement *= -1;
                xFlip = ((ellipse.Margin.Left + xMovement) - (route[currentIndex].line.X2 - ellipse.Width / 2) <= 0);
            }


            bool yFlip = false;
            if (targetPos.Y < ellipse.Margin.Top + ellipse.Height / 2)
            {
                
                yFlip = (ellipse.Margin.Top + xMovement * m <= route[currentIndex].line.Y2 - ellipse.Height / 2);
            }
            if (targetPos.Y > ellipse.Margin.Top + ellipse.Height / 2)
            {
                yFlip = (ellipse.Margin.Top + xMovement * m >= route[currentIndex].line.Y2 - ellipse.Height / 2);
            }


            if (xFlip || yFlip)
            {
                if (currentIndex == route.Length - 1)
                {
                    StaticVar.mainCanvas.Children.Remove(ellipse);
                    return false;
                }
                route[currentIndex].unitsOnRoad.Remove(this);
                currentIndex++;
                route[currentIndex].unitsOnRoad.Add(this);
                currentRoad = route[currentIndex];
                ellipse.Margin = new Thickness(targetPos.X - ellipse.Width / 2, targetPos.Y - ellipse.Height / 2, 0, 0);
                return true;
            }

            Vector vector = new Vector(xMovement, xMovement * m);
            vector.Normalize();

            double xPos = ellipse.Margin.Left + vector.X;
            double yPos = ellipse.Margin.Top + vector.Y;

            for (int i = 0; i < _units.Length; i++)
            {
                if (_units[i] != this)
                {
                    double xDiff = _units[i].ellipse.Margin.Left - xPos;
                    double yDiff = _units[i].ellipse.Margin.Top - yPos;
                    double distance = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
                    if (distance < ellipse.Width * 8) {
                        if (_units[i].currentRoad == currentRoad || currentRoad.connectedRoads.Contains(_units[i].currentRoad)) {


                            // Check if on same road
                            if (_units[i].currentRoad == currentRoad)
                            {
                                // Check if other point is further ahead on the road
                                double otherDistanceToEndPoint = Math.Sqrt(Math.Pow(currentRoad.line.X2 - _units[i].ellipse.Margin.Left, 2) + Math.Pow(currentRoad.line.Y2 - _units[i].ellipse.Margin.Top, 2));
                                double distanceToEndPoint = Math.Sqrt(Math.Pow(currentRoad.line.X2 - ellipse.Margin.Left, 2) + Math.Pow(currentRoad.line.Y2 - ellipse.Margin.Top, 2));

                                if (distanceToEndPoint > otherDistanceToEndPoint)
                                    if (distance < ellipse.Height / 2 + _units[i].ellipse.Height / 2) return true;
                            }

                            // Check if on next road
                            if (currentIndex != route.Length - 1)
                            {
                                if (_units[i].currentRoad == route[currentIndex + 1])
                                {
                                    if (distance < ellipse.Height + _units[i].ellipse.Height / 2) return true;
                                }
                            }
                        }
                    }
                }
            }

            ellipse.Margin = new Thickness(xPos, yPos, 0, 0);
            return true;
        }

        double pDistance(double x, double y, double x1, double y1, double x2, double y2)
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
            return Math.Sqrt(dx * dx + dy * dy);
        }



    }
}
