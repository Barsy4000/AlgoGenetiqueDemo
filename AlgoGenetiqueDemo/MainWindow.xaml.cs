using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using AlgoGenetiqueDemo.Outils;

namespace AlgoGenetiqueDemo
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Marge pour éviter de positionner les points trop au bord.
        /// </summary>
        public const int MARGE = 10;

        /// <summary>
        /// Taille d'affichage de chaque points.
        /// </summary>
        public const int TaillePoint = 7;

        /// <summary>
        /// Epaisseur des lignes.
        /// </summary>
        public const int EpaisseurLigne = 3;

        /// <summary>
        /// Problème du voyageur de commerce.
        /// </summary>
        private VoyageurCommerce voyageurCommerce;

        /// <summary>
        /// Chronomètre pour mesurer le temps d'exécution des algos.
        /// </summary>
        private Timer chronometre;

        /// <summary>
        /// Date de démarrage du calcul.
        /// </summary>
        private DateTime dateDebutCalcul;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.chronometre = new Timer();
            this.chronometre.Interval = 0.0001;
            this.chronometre.Elapsed += this.Chronometre_Tick;
        }

        /// <summary>
        /// Chronomètre la durée d'exécution des algos.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void Chronometre_Tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => this.lblChronometre.Content = (DateTime.Now - this.dateDebutCalcul).ToString("h':'mm':'ss")));
        }

        /// <summary>
        /// Evenement déclenché lors de la fermeture de la fenêtre.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.voyageurCommerce != null)
            {
                this.voyageurCommerce.StopperForceBrute();
                this.voyageurCommerce.StopperAlgoGenetique();
            }

            this.chronometre.Stop();
        }

        /// <summary>
        /// Clic sur le bouton de génération d'un nouveau parcours.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void BtnGenererParcours_Click(object sender, RoutedEventArgs e)
        {
            this.cnvDrawZone.Children.Clear();

            int nombrePoints = 0;

            if (!int.TryParse(this.txtNombrePoints.Text, out nombrePoints))
            {
                MessageBox.Show("Le nombre de points doit être renseigné", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int xMax = (int)this.cnvDrawZone.ActualWidth - MARGE;
            int yMax = (int)this.cnvDrawZone.ActualHeight - MARGE;

            this.voyageurCommerce = new VoyageurCommerce(nombrePoints, MARGE, xMax, MARGE, yMax);
            this.voyageurCommerce.UpdateMeilleurIndividu += this.VoyageurCommerce_UpdateMeilleurIndividu;
            this.voyageurCommerce.FinDeCalcul += this.VoyageurCommerce_FinDeCalcul;
            this.voyageurCommerce.UpdateNombreGeneration += this.VoyageurCommerce_UpdateNombreGeneration;

            this.DrawVoyageurCommerce();
        }

        /// <summary>
        /// Force la saisie de nombre dans la zone de texte.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Mise à jour du meilleur individu dans le voyageur de commerce.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void VoyageurCommerce_UpdateMeilleurIndividu(object sender, EventArgs e)
        {
            this.cnvDrawZone.Children.Clear();
            this.DrawIndividu(this.voyageurCommerce.MeilleurIndividu);
            this.DrawVoyageurCommerce();
        }

        /// <summary>
        /// Mise à jour du nombre de générations lors du calcul par algorithme génétique.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void VoyageurCommerce_UpdateNombreGeneration(object sender, UpdateGenerationEventArgs e)
        {
            this.lblNombreGenerations.Content = e.NombreGenerations;
        }

        /// <summary>
        /// Dessine le problème du voyageur de commerce.
        /// </summary>
        private void DrawVoyageurCommerce()
        {
            Color couleurPoint = Colors.Red;

            foreach (Point pointPassage in this.voyageurCommerce.PointsPassage)
            {
                Ellipse ellipse = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush(couleurPoint);
                ellipse.Fill = mySolidColorBrush;
                ellipse.Width = TaillePoint;
                ellipse.Height = TaillePoint;
                Canvas.SetLeft(ellipse, pointPassage.X - (TaillePoint / 2));
                Canvas.SetTop(ellipse, pointPassage.Y - (TaillePoint / 2));
                this.cnvDrawZone.Children.Add(ellipse);

                couleurPoint = Colors.Black;
            }
        }

        /// <summary>
        /// Dessine un individu sur la carte.
        /// </summary>
        /// <param name="individu">Individu à représenter sur la carte.</param>
        private void DrawIndividu(Individu individu)
        {
            if (individu != null)
            {
                this.DrawLine(this.voyageurCommerce.Point0, individu.Points[0]);

                for (int i = 1; i < individu.Points.Count(); i++)
                {
                    this.DrawLine(individu.Points[i - 1], individu.Points[i]);
                }

                this.DrawLine(individu.Points[individu.Points.Count() - 1], this.voyageurCommerce.Point0);

                this.lblResultValeurIndividu.Content = string.Format("{0:### ### ### ###}", individu.ValeurPourAffichage);
            }
        }

        /// <summary>
        /// Dessine une ligne entre deux points.
        /// </summary>
        /// <param name="point1">Point de départ de la ligne.</param>
        /// <param name="point2">Point d'arrivée de la ligne.</param>
        private void DrawLine(Point point1, Point point2)
        {
            Line lineFirst = new Line();
            lineFirst.Stroke = Brushes.Gray;
            lineFirst.X1 = point1.X;
            lineFirst.X2 = point2.X;
            lineFirst.Y1 = point1.Y;
            lineFirst.Y2 = point2.Y;
            lineFirst.StrokeThickness = EpaisseurLigne;
            this.cnvDrawZone.Children.Add(lineFirst);
        }

        /// <summary>
        /// Démarre le calcul par force brute.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void BtnDemarrerForceBrute_Click(object sender, RoutedEventArgs e)
        {
            if (this.voyageurCommerce != null)
            {
                this.DesactiveBoutonsSauf(this.btnArreterForceBrute);
                this.dateDebutCalcul = DateTime.Now;
                this.chronometre.Start();
                this.voyageurCommerce.DemarrerForceBrute();
            }
        }

        /// <summary>
        /// Arrête le calcul par force brute.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void BtnArreterForceBrute_Click(object sender, RoutedEventArgs e)
        {
            if (this.voyageurCommerce != null)
            {
                this.ReinitialiseBoutons();
                this.chronometre.Stop();
                this.voyageurCommerce.StopperForceBrute();
            }
        }

        /// <summary>
        /// Démarre le calcul par algo génétique.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void BtnDemarrerAlgoGenetique_Click(object sender, RoutedEventArgs e)
        {
            if (this.voyageurCommerce != null)
            {
                int taillePopulation;
                if (!int.TryParse(this.txtTaillePopulation.Text, out taillePopulation))
                {
                    taillePopulation = 0;
                }

                int nombreCombattants;
                if (!int.TryParse(this.txtNombreCombattants.Text, out nombreCombattants))
                {
                    nombreCombattants = 0;
                }

                double probabiliteMutation;
                if (!double.TryParse(this.txtProbabiliteMutation.Text, out probabiliteMutation))
                {
                    probabiliteMutation = 0;
                }

                this.DesactiveBoutonsSauf(this.btnArreterAlgoGenetique);
                this.dateDebutCalcul = DateTime.Now;
                this.chronometre.Start();
                this.voyageurCommerce.DemarrerAlgoGenetique(taillePopulation, nombreCombattants, probabiliteMutation);
            }
        }

        /// <summary>
        /// Arrête le calcul par algo génétique.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void BtnArreterAlgoGenetique_Click(object sender, RoutedEventArgs e)
        {
            if (this.voyageurCommerce != null)
            {
                this.ReinitialiseBoutons();
                this.chronometre.Stop();
                this.voyageurCommerce.StopperAlgoGenetique();
            }
        }

        /// <summary>
        /// Evènement de fin de calcul.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void VoyageurCommerce_FinDeCalcul(object sender, EventArgs e)
        {
            if (this.voyageurCommerce != null)
            {
                this.ReinitialiseBoutons();
                this.chronometre.Stop();
                this.voyageurCommerce.StopperForceBrute();
            }
        }

        /// <summary>
        /// Réinitialise l'état des boutons.
        /// </summary>
        private void ReinitialiseBoutons()
        {
            this.btnGenererParcours.IsEnabled = true;
            this.btnDemarrerForceBrute.IsEnabled = true;
            this.btnArreterForceBrute.IsEnabled = false;
            this.btnDemarrerAlgoGenetique.IsEnabled = true;
            this.btnArreterAlgoGenetique.IsEnabled = false;
        }

        /// <summary>
        /// Désactive tous les boutons sauf un.
        /// </summary>
        /// <param name="boutonActif">Bouton à garder actif.</param>
        private void DesactiveBoutonsSauf(Button boutonActif)
        {
            this.btnGenererParcours.IsEnabled = false;
            this.btnDemarrerForceBrute.IsEnabled = false;
            this.btnArreterForceBrute.IsEnabled = false;
            this.btnDemarrerAlgoGenetique.IsEnabled = false;
            this.btnArreterAlgoGenetique.IsEnabled = false;

            boutonActif.IsEnabled = true;
        }
    }
}
