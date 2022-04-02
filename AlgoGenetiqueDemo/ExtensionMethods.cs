using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AlgoGenetiqueDemo
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Calcule la distance entre deux points.
        /// </summary>
        /// <param name="point1">Premier point.</param>
        /// <param name="point2">Second point.</param>
        /// <returns>Retourne la distance entre les deux points.</returns>
        public static double distance(this Point point1, Point point2)
        {
            double diffX = point1.X - point2.X;
            double diffY = point1.Y - point2.Y;
            return Math.Sqrt(diffX * diffX + diffY * diffY);
        }

        /// <summary>
        /// Calcule la distance simple entre deux points (sans la racine carrée).
        /// </summary>
        /// <param name="point1">Premier point.</param>
        /// <param name="point2">Second point.</param>
        /// <returns>Retourne la distance simple entre les deux points.</returns>
        public static double distanceSimple(this Point point1, Point point2)
        {
            double diffX = point1.X - point2.X;
            double diffY = point1.Y - point2.Y;
            return diffX * diffX + diffY * diffY;
        }
    }
}
