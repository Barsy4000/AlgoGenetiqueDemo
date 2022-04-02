using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        /// Thread pour exécuter l'algo de force brute.
        /// </summary>
        private Thread threadForceBrute;

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
        /// Obtient ou définit un tableau contenant les points de passage du voyageur de commerce.
        /// </summary>
        public Point[] pointsPassage { get; set; }

        /// <summary>
        /// Meilleur individu retourné par le calcul courant.
        /// </summary>
        private Individu meilleurIndividu;

        /// <summary>
        /// Point de départ et d'arrivée du parcours.
        /// </summary>
        public Point Point0 => this.pointsPassage[0];

        /// <summary>
        /// Obtient le meilleur individu retourné par le calcul courant.
        /// </summary>
        public Individu MeilleurIndividu
        {
            get { return this.meilleurIndividu; }
            set
            {
                this.meilleurIndividu = value;
                EventHandler<EventArgs> raiseEvent = UpdateMeilleurIndividu;

                if (raiseEvent != null)
                {
                    raiseEvent(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Evènement lors de la mise à jour du meilleur individu.
        /// </summary>
        public event EventHandler<EventArgs> UpdateMeilleurIndividu;

        /// <summary>
        /// Evènement lors de la fin du calcul.
        /// </summary>
        public event EventHandler<EventArgs> FinDeCalcul;

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
            this.pointsPassage = new Point[nombrePoints];

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

                pointsPassage[i] = new Point(x, y);
            }
        }

        /// <summary>
        /// Démarre le calcul du parcours à l'aide de la force brute.
        /// </summary>
        public void DemarrerForceBrute()
        {
            this.threadForceBrute = new Thread(this.CalculForceBrute);
            this.threadForceBrute.Start();
        }

        /// <summary>
        /// Arrête le calcul du parcours à l'aide de la force brute.
        /// </summary>
        public void StopperForceBrute()
        {
            try
            {
                if (this.threadForceBrute != null && this.threadForceBrute.IsAlive)
                {
                    this.threadForceBrute.Abort();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'arrêt du Thread de force brute : {ex.Message}");
            }

        }

        /// <summary>
        /// Calcule le problème par la méthode de la force brute.
        /// </summary>
        private void CalculForceBrute()
        {
            try
            {
                double meilleureValeur = double.MaxValue;

                IEnumerable<Point> points = this.pointsPassage.Skip(1);
                IEnumerable<IEnumerable<Point>> permutations = GetPermutations(points, points.Count());

                foreach (IEnumerable<Point> permutation in permutations)
                {
                    Individu individuCourant = new Individu(this, permutation);

                    if (individuCourant.Valeur < meilleureValeur)
                    {
                        meilleureValeur = individuCourant.Valeur;
                        Application.Current.Dispatcher.Invoke(new Action(() => this.MeilleurIndividu = individuCourant));
                    }
                }

                RaiseEventFinCalcul();
            }
            catch (ThreadAbortException ex)
            {
                Console.WriteLine($"Interruption du Thread : {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du calcul par force brute : {ex.Message}");
            }
        }

        /// <summary>
        /// Calcule toutes les permutations d'une liste.
        /// </summary>
        /// <typeparam name="T">Type d'éléments de la liste.</typeparam>
        /// <param name="list">Liste à partir de laquelle il faut calculer les permutations.</param>
        /// <param name="length">Taille de la liste.</param>
        /// <returns>Retourne l'ensemble des permutations.</returns>
        private IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        /// <summary>
        /// Déclenche l'évènement de fin de calcul.
        /// </summary>
        private void RaiseEventFinCalcul()
        {
            EventHandler<EventArgs> raiseEvent = FinDeCalcul;

            if (raiseEvent != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => raiseEvent(this, new EventArgs())));
            }
        }
    }
}
