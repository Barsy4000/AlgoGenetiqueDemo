using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        /// Obtient la valeur de l'individu.
        /// </summary>
        public double Valeur
        {
            get
            {
                // Si la valeur est déjà calculée, ne la recalcule pas.
                if (valeur == 0)
                {
                    this.valeur += this.VoyageurCommerce.Point0.distanceSimple(this.Points[0]);

                    for (int i = 1; i < this.Points.Count(); i++)
                    {
                        this.valeur += this.Points[i - 1].distanceSimple(this.Points[i]);
                    }

                    this.valeur += this.Points[this.Points.Count() - 1].distanceSimple(this.VoyageurCommerce.Point0);
                }

                return this.valeur;
            }
        }

        /// <summary>
        /// Obtient la liste des points constituant l'individu.
        /// </summary>
        public Point[] Points { get; }

        /// <summary>
        /// Obtient le problème de voyageur de commerce lié à l'individu.
        /// </summary>
        public VoyageurCommerce VoyageurCommerce { get; }

        /// <summary>
        /// Obtient une valeur contenant la taille de l'individu.
        /// </summary>
        public int Taille => Points.Length;

        /// <summary>
        /// Initialise une instance d'individu vide.
        /// </summary>
        /// <param name="voyageurCommerce">Problème de voyageur de commerce lié à l'individu.</param>
        public Individu(VoyageurCommerce voyageurCommerce)
        {
            this.VoyageurCommerce = voyageurCommerce;
            this.Points = new Point[0];
        }

        /// <summary>
        /// Initialise une instance d'individu vide à partir d'un Enumerable de points existants.
        /// </summary>
        /// <param name="points">Enumerable de points à partir duquel générer l'individu.</param>
        /// <param name="voyageurCommerce">Problème de voyageur de commerce lié à l'individu.</param>
        public Individu(VoyageurCommerce voyageurCommerce, IEnumerable<Point> points)
        {
            this.VoyageurCommerce = voyageurCommerce;
            this.Points = points.ToArray();
        }
    }
}
