using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace krzywa_beziera
{
    class Beizer
    {
        public static List<Point> calculate(List<Point> controlPoints, double step)
        {
            List<Point> bezierPoints = new List<Point>();

            int degree = controlPoints.Count-1;
            for (double p = 0; p <= 1; p += step)
            {
                double xPoint = 0;
                for (int i = 0; i <= degree; ++i)
                {
                    double Newton = countNewton(degree, i); //Obliczamy wartość współczynnika Newtona
                    double ti = Math.Pow(p, i); //Pierwszy element wzoru t do potęgi i-tej
                    double lti = Math.Pow((1 - p), (degree - i)); //Drugi element wzoru (1-t) do potęgi (ni)
                    xPoint = xPoint + (Newton * ti * lti * controlPoints[i].X); //Tworzymy składową x-ową
                }


               
                double yPoint = 0;
                for (int i = 0; i <= degree; ++i)
                {
                    double yNewton = countNewton(degree, i);
                    double yti = Math.Pow(p, i);
                    double ylti = Math.Pow((1 - p), (degree - i));
                    yPoint = yPoint + (yNewton * yti * ylti * controlPoints[i].Y);
                }
                //if ((xPoint > controlPoints[controlPoints.Count - 1].X) && (yPoint > controlPoints[controlPoints.Count - 1].Y))
               // {
                //    break;
               // }
                bezierPoints.Add(new Point(xPoint, yPoint));
            }
            return bezierPoints;
        }

        private static double countNewton(int n,int i)
        {
            return countFactorial(n) / (countFactorial(i) * countFactorial(n - i));
        }

        private static double countFactorial(int n)
        {
            if(n>1)
            return n * countFactorial(n - 1);
            return 1;
        }
    }
}
