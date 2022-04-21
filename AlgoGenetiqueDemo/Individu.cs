using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AlgoGenetiqueDemo.Outils;

namespace AlgoGenetiqueDemo
{
    /// <summary>
    /// Classe permettant d'instancier un individu solution du problème.
    /// </summary>
    internal class Individu
    {
        /// <summary>
        /// Valeur de l'individu.
        /// </summary>
        private double valeur = 0;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Individu"/>.
        /// </summary>
        /// <param name="point0">Point de départ et d'arrivée du problème.</param>
        /// <param name="taille">Taille de l'individu.</param>
        public Individu(Point point0, int taille)
        {
            this.Point0 = point0;
            this.Points = new Point[taille];
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Individu"/>.
        /// </summary>
        /// <param name="point0">Point de départ et d'arrivée du problème.</param>
        /// <param name="points">Enumerable de points à partir duquel générer l'individu.</param>
        public Individu(Point point0, IEnumerable<Point> points)
        {
            this.Point0 = point0;
            this.Points = points.ToArray();
        }

        /// <summary>
        /// Obtient le point de départ et d'arrivée du problème.
        /// </summary>
        public Point Point0 { get; }

        /// <summary>
        /// Obtient la valeur de l'individu.
        /// </summary>
        public double Valeur
        {
            get
            {
                // Si la valeur est déjà calculée, ne la recalcule pas.
                if (this.valeur == 0)
                {
                    this.valeur += this.Point0.DistanceSimple(this.Points[0]);

                    for (int i = 1; i < this.Points.Count(); i++)
                    {
                        this.valeur += this.Points[i - 1].DistanceSimple(this.Points[i]);
                    }

                    this.valeur += this.Points[this.Points.Count() - 1].DistanceSimple(this.Point0);
                }

                return this.valeur;
            }
        }

        /// <summary>
        /// Obtient la valeur réelle de l'individu pour affichage.
        /// </summary>
        public double ValeurPourAffichage
        {
            get
            {
                double result = 0;

                result += this.Point0.Distance(this.Points[0]);

                for (int i = 1; i < this.Points.Count(); i++)
                {
                    result += this.Points[i - 1].Distance(this.Points[i]);
                }

                result += this.Points[this.Points.Count() - 1].Distance(this.Point0);

                return result;
            }
        }

        /// <summary>
        /// Obtient la liste des points constituant l'individu.
        /// </summary>
        public Point[] Points { get; }

        /// <summary>
        /// Obtient une valeur contenant la taille de l'individu.
        /// </summary>
        public int Taille => this.Points.Length;
    }
}
