using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using AlgoGenetiqueDemo.Outils;

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
        public const int NombrePointsMax = 1000;

        /// <summary>
        /// Distance minimale entre deux points de passage.
        /// </summary>
        public const int DistanceMinEntrePoints = 10;

        /// <summary>
        /// Thread pour exécuter l'algo de force brute.
        /// </summary>
        private Thread threadForceBrute;

        /// <summary>
        /// Thread pour exécuter l'algo génétique.
        /// </summary>
        private Thread threadAlgoGenetique;

        /// <summary>
        /// Meilleur individu retourné par le calcul courant.
        /// </summary>
        private Individu meilleurIndividu;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="VoyageurCommerce"/>.
        /// </summary>
        /// <param name="nombrePoints">Nombre de points de passage à parcourir.</param>
        /// <param name="xMin">Valeur minimale de la position X du point.</param>
        /// <param name="xMax">Valeur maximale de la position X du point.</param>
        /// <param name="yMin">Valeur minimale de la position Y du point.</param>
        /// <param name="yMax">Valeur maximale de la position Y du point.</param>
        public VoyageurCommerce(int nombrePoints, int xMin, int xMax, int yMin, int yMax)
        {
            this.Rand = new Random();

            this.PointsPassage = new Point[nombrePoints];

            for (int i = 0; i < nombrePoints; i++)
            {
                int x;
                int y;
                Point nouveauPoint;

                do
                {
                    x = this.Rand.Next(xMin, xMax);
                    y = this.Rand.Next(yMin, yMax);
                    nouveauPoint = new Point(x, y);
                }
                while (this.PointsPassage.Any(point => point.Distance(nouveauPoint) < DistanceMinEntrePoints));

                this.PointsPassage[i] = new Point(x, y);
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
        /// Evènement lors de la mise à jour du nombre de générations.
        /// </summary>
        public event EventHandler<UpdateGenerationEventArgs> UpdateNombreGeneration;

        /// <summary>
        /// Obtient le générateur de nombre aléatoire.
        /// </summary>
        public Random Rand { get; }

        /// <summary>
        /// Obtient ou définit un tableau contenant les points de passage du voyageur de commerce.
        /// </summary>
        public Point[] PointsPassage { get; set; }

        /// <summary>
        /// Obtient le point de départ et d'arrivée du parcours.
        /// </summary>
        public Point Point0 => this.PointsPassage[0];

        /// <summary>
        /// Obtient ou définit le meilleur individu retourné par le calcul courant.
        /// </summary>
        public Individu MeilleurIndividu
        {
            get
            {
                return this.meilleurIndividu;
            }

            set
            {
                this.meilleurIndividu = value;
                EventHandler<EventArgs> raiseEvent = this.UpdateMeilleurIndividu;

                if (raiseEvent != null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => raiseEvent(this, new EventArgs())));
                }
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
        /// Démarre le calcul du parcours à l'aide de l'algorithme génétique.
        /// </summary>
        /// <param name="taillePopulation">Taille de la population.</param>
        /// <param name="nombreCombattants">Nombre de combattants pour la sélection par tournois.</param>
        /// <param name="probabiliteMutation">Probabilité de mutation d'un gène.</param>
        public void DemarrerAlgoGenetique(int taillePopulation, int nombreCombattants,  double probabiliteMutation)
        {
            this.threadAlgoGenetique = new Thread(() => this.CalculAlgoGenetique(taillePopulation, nombreCombattants, probabiliteMutation));
            this.threadAlgoGenetique.Start();
        }

        /// <summary>
        /// Arrête le calcul du parcours à l'aide de l'algorithme génétique.
        /// </summary>
        public void StopperAlgoGenetique()
        {
            try
            {
                if (this.threadAlgoGenetique != null && this.threadAlgoGenetique.IsAlive)
                {
                    this.threadAlgoGenetique.Abort();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'arrêt du Thread de l'algo génétique : {ex.Message}");
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

                IEnumerable<Point> points = this.PointsPassage.Skip(1);
                IEnumerable<IEnumerable<Point>> permutations = Util.GetPermutations(points, points.Count());

                foreach (IEnumerable<Point> permutation in permutations)
                {
                    Individu individuCourant = new Individu(this.Point0, permutation);

                    if (individuCourant.Valeur < meilleureValeur)
                    {
                        meilleureValeur = individuCourant.Valeur;
                        this.MeilleurIndividu = individuCourant;
                    }
                }

                this.RaiseEventFinCalcul();
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
        /// Calcule le problème par la méthode de l'algorithme génétique.
        /// </summary>
        /// <param name="taillePopulation">Taille de la population.</param>
        /// <param name="nombreCombattants">Nombre de combattants pour la sélection par tournois.</param>
        /// <param name="probabiliteMutation">Probabilité de mutation d'un gène.</param>
        private void CalculAlgoGenetique(int taillePopulation, int nombreCombattants, double probabiliteMutation)
        {
            this.MeilleurIndividu = null;
            Population population = new Population(this.Point0, this.PointsPassage.Skip(1), taillePopulation);
            population.GenerationAleatoire();
            int nombreGeneration = 0;
            double meilleureValeur = double.MaxValue;

            while (true)
            {
                nombreGeneration++;

                EventHandler<UpdateGenerationEventArgs> raiseEvent = this.UpdateNombreGeneration;

                if (raiseEvent != null)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => raiseEvent(this, new UpdateGenerationEventArgs(nombreGeneration))));
                }

                if (this.MeilleurIndividu == null || population.MeilleurIndividu.Valeur < meilleureValeur)
                {
                    this.MeilleurIndividu = population.MeilleurIndividu;
                    meilleureValeur = this.MeilleurIndividu.Valeur;
                }

                IEnumerable<Individu>[] couples = population.SelectionTournoi(nombreCombattants);
                population.Reproduction(couples);

                if (probabiliteMutation != 0)
                {
                    population.Mutation(probabiliteMutation);
                }
            }
        }

        /// <summary>
        /// Déclenche l'évènement de fin de calcul.
        /// </summary>
        private void RaiseEventFinCalcul()
        {
            EventHandler<EventArgs> raiseEvent = this.FinDeCalcul;

            if (raiseEvent != null)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => raiseEvent(this, new EventArgs())));
            }
        }
    }
}
