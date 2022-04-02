using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AlgoGenetiqueDemo
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
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
        public const int TAILLE_POINT = 7;

        /// <summary>
        /// Epaisseur des lignes.
        /// </summary>
        public const int EPAISSEUR_LIGNE = 3;

        /// <summary>
        /// Problème du voyageur de commerce.
        /// </summary>
        private VoyageurCommerce voyageurCommerce;

        /// <summary>
        /// Initialise la fenêtre principale.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
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
            }
        }

        /// <summary>
        /// Clic sur le bouton de génération d'un nouveau parcours.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void btnGenererParcours_Click(object sender, RoutedEventArgs e)
        {
            this.cnvDrawZone.Children.Clear();

            int nombrePoints = 0;

            if (!int.TryParse(txtNombrePoints.Text, out nombrePoints))
            {
                MessageBox.Show("Le nombre de points doit être renseigné", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int xMax = (int)this.cnvDrawZone.ActualWidth - MARGE;
            int yMax = (int)this.cnvDrawZone.ActualHeight - MARGE;

            this.voyageurCommerce = new VoyageurCommerce(nombrePoints, MARGE, xMax, MARGE, yMax);
            voyageurCommerce.UpdateMeilleurIndividu += VoyageurCommerce_UpdateMeilleurIndividu;
            voyageurCommerce.FinDeCalcul += VoyageurCommerce_FinDeCalcul;

            this.DrawVoyageurCommerce();
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
                this.btnGenererParcours.IsEnabled = true;
                this.btnDemarrerForceBrute.IsEnabled = true;
                this.btnArreterForceBrute.IsEnabled = false;
                this.voyageurCommerce.StopperForceBrute();
            }
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
        /// Dessine le problème du voyageur de commerce.
        /// </summary>
        private void DrawVoyageurCommerce()
        {
            Color couleurPoint = Colors.Red;

            foreach (Point pointPassage in this.voyageurCommerce.pointsPassage)
            {
                Ellipse ellipse = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush(couleurPoint);
                ellipse.Fill = mySolidColorBrush;
                ellipse.Width = TAILLE_POINT;
                ellipse.Height = TAILLE_POINT;
                Canvas.SetLeft(ellipse, pointPassage.X - TAILLE_POINT / 2);
                Canvas.SetTop(ellipse, pointPassage.Y - TAILLE_POINT / 2);
                this.cnvDrawZone.Children.Add(ellipse);

                couleurPoint = Colors.Black;
            }
        }

        /// <summary>
        /// Dessine un individu sur la carte.
        /// </summary>
        /// <param name="individu"></param>
        private void DrawIndividu(Individu individu)
        {
            DrawLine(this.voyageurCommerce.Point0, individu.Points[0]);

            for (int i = 1; i < individu.Points.Count(); i++)
            {
                DrawLine(individu.Points[i - 1], individu.Points[i]);
            }

            DrawLine(individu.Points[individu.Points.Count() - 1], this.voyageurCommerce.Point0);

            this.lblResultValeurIndividu.Content = String.Format("{0:### ### ### ###}", individu.Valeur);
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
            lineFirst.StrokeThickness = EPAISSEUR_LIGNE;
            this.cnvDrawZone.Children.Add(lineFirst);
        }

        /// <summary>
        /// Démarre le calcul par force brute.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void btnDemarrerForceBrute_Click(object sender, RoutedEventArgs e)
        {
            if (this.voyageurCommerce != null)
            {
                this.btnGenererParcours.IsEnabled = false;
                this.btnDemarrerForceBrute.IsEnabled = false;
                this.btnArreterForceBrute.IsEnabled = true;
                this.voyageurCommerce.DemarrerForceBrute();
            }
        }

        /// <summary>
        /// Arrête le calcul par force brute.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void btnArreterForceBrute_Click(object sender, RoutedEventArgs e)
        {
            if (this.voyageurCommerce != null)
            {
                this.btnGenererParcours.IsEnabled = true;
                this.btnDemarrerForceBrute.IsEnabled = true;
                this.btnArreterForceBrute.IsEnabled = false;
                this.voyageurCommerce.StopperForceBrute();
            }
        }
    }
}
