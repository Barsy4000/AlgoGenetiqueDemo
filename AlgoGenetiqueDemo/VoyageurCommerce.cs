using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AlgoGenetiqueDemo
{
    /// <summary>
    /// Classe permettant d'instancier un problème du voyageur de commerce.
    /// </summary>
    internal class VoyageurCommerce
    {
        /// <summary>
        /// Nombre de points maximal qui peuvent être saisis.
        /// </summary>
        public const int NOMBRE_POINTS_MAX = 1000;

        /// <summary>
        /// Distance minimale entre deux points de passage.
        /// </summary>
        public const int DISTANCE_MIN_ENTRE_POINTS = 10;

        /// <summary>
        /// Générateur de nombre aléatoire.
        /// </summary>
        private Random rand = new Random();

        /// <summary>
        /// Obtient ou définit une liste contenant lkes points de passage du voyageur de commerce.
        /// </summary>
        public List<Point> pointsPassage { get; set; }

        /// <summary>
        /// Initialise une instance de la classe VoyageurCommerce.
        /// </summary>
        /// <param name="nombrePoints">Nombre de points de passage à parcourir.</param>
        /// <param name="xMin">Valeur minimale de la position X du point.</param>
        /// <param name="xMax">Valeur maximale de la position X du point.</param>
        /// <param name="yMin">Valeur minimale de la position Y du point.</param>
        /// <param name="yMax">Valeur maximale de la position Y du point.</param>
        public VoyageurCommerce(int nombrePoints, int xMin, int xMax, int yMin, int yMax)
        {
            this.pointsPassage = new List<Point>();

            for (int i = 0; i < nombrePoints; i++)
            {
                int x;
                int y;
                Point nouveauPoint;
                
                do
                {
                    x = rand.Next(xMin, xMax);
                    y = rand.Next(yMin, yMax);
                    nouveauPoint = new Point(x, y);
                } while (pointsPassage.Any(point => point.distance(nouveauPoint) < DISTANCE_MIN_ENTRE_POINTS));

                pointsPassage.Add(new Point(x, y));
            }
        }
    }
}
