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
        /// Clic sur le bouton de génération d'un nouveau parcours.
        /// </summary>
        /// <param name="sender">Contrôle dans lequel se déclenche l'évènement.</param>
        /// <param name="e">Informations sur l'évènement.</param>
        private void btnGenererParcours_Click(object sender, RoutedEventArgs e)
        {
            int nombrePoints = 0;

            if(!int.TryParse(txtNombrePoints.Text, out nombrePoints))
            {
                MessageBox.Show("Le nombre de points doit être renseigné", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int xMax = (int)this.cnvDrawZone.ActualWidth;
            int yMax = (int)this.cnvDrawZone.ActualHeight;

            this.voyageurCommerce = new VoyageurCommerce(nombrePoints, 0, xMax, 0, yMax);

            foreach (Point pointPassage in this.voyageurCommerce.pointsPassage)
            {
                Ellipse myEllipse = new Ellipse();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Black);
                myEllipse.Fill = mySolidColorBrush;
                myEllipse.Width = 5;
                myEllipse.Height = 5;
                Canvas.SetLeft(myEllipse, pointPassage.X);
                Canvas.SetTop(myEllipse, pointPassage.Y);
                cnvDrawZone.Children.Add(myEllipse);
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
    }
}
