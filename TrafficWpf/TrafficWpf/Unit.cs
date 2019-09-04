using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;

namespace TrafficWpf
{
    class Unit
    {
        public Road[] route { get; set; }
        public Point position { get; set; }
        public Ellipse ellipse { get; set; }
        int currentIndex = 0;

        public Unit (Road[] _route, Ellipse _ellipse)
        {
            route = _route;
            ellipse = _ellipse;
        }

        public bool Move()
        {
            Point targetPos;
            if (currentIndex == route.Length - 1)
            {
                targetPos = new Point(route[currentIndex].connectedDestination.rectangle.Margin.Left + route[currentIndex].connectedDestination.rectangle.Width / 2, route[currentIndex].connectedDestination.rectangle.Margin.Top + route[currentIndex].connectedDestination.rectangle.Height / 2);
            } else
            {
                if (route[currentIndex].line.X2 == route[currentIndex + 1].line.X1 && route[currentIndex].line.Y2 == route[currentIndex + 1].line.Y1)
                    targetPos = new Point(route[currentIndex + 1].line.X1, route[currentIndex + 1].line.Y1);
                else
                    targetPos = new Point(route[currentIndex].line.X2, route[currentIndex].line.Y2);
            }


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
                currentIndex++;
                
                ellipse.Margin = new Thickness(targetPos.X - ellipse.Width / 2, targetPos.Y - ellipse.Height / 2, 0, 0);
                return true;
            }

            Vector vector = new Vector(xMovement, xMovement * m);
            vector.Normalize();

            ellipse.Margin = new Thickness(ellipse.Margin.Left + vector.X, ellipse.Margin.Top + vector.Y, 0, 0);
            return true;
        }
    }
}
