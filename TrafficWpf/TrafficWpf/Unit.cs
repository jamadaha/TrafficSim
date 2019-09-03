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

        public void Move()
        {
            double m = ((route[currentIndex].line.Y2 - ellipse.Margin.Top - ellipse.Height / 2) / (route[currentIndex].line.X2 - ellipse.Margin.Left - ellipse.Width / 2));

            double xMovement = 0.1;

            //ellipse.Margin = new Thickness(route[currentIndex].line.X1 - ellipse.Width / 2, route[currentIndex].line.Y1 - ellipse.Height / 2, 0, 0);

            bool xFlip; 
            if (route[currentIndex].line.X2 > ellipse.Margin.Left)
            {
                xFlip = (ellipse.Margin.Left + xMovement >= route[currentIndex].line.X2 - ellipse.Width / 2);
            } else
            {
                xMovement *= -1;
                xFlip = (ellipse.Margin.Left + xMovement <= route[currentIndex].line.X2 - ellipse.Width / 2);
            }
            bool yFlip;
            if (route[currentIndex].line.Y2 < ellipse.Margin.Top)
            {
                yFlip = (ellipse.Margin.Top + xMovement * m <= route[currentIndex].line.Y2 - ellipse.Height / 2);
            }
            else
            {
                yFlip = (ellipse.Margin.Top + xMovement * m >= route[currentIndex].line.Y2 - ellipse.Height / 2);
            }


            if (xFlip || yFlip)
            {
                if (currentIndex == route.Length - 1) return;
                currentIndex++;
                
                ellipse.Margin = new Thickness(route[currentIndex].line.X1 - ellipse.Width / 2, route[currentIndex].line.Y1 - ellipse.Height / 2, 0, 0);
                return;
            }
            
            ellipse.Margin = new Thickness(ellipse.Margin.Left + xMovement, ellipse.Margin.Top + xMovement * m, 0, 0);
        }
    }
}
