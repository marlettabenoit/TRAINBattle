using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TRAINBattle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AfficheDemarage(null, null);
        }

        private void AfficheDemarage(object sender, RoutedEventArgs e)
        {
            // crée et charge l'écran de démarrage
            UCDemarage uc = new UCDemarage();
            // associe l'écran au conteneur
            ZoneJeu.Content = uc;
            uc.butParametres.Click += AfficherParametres;
            uc.butAide.Click += AfficherAide;
            uc.butVersus.Click += AfficherChoixPerso;
            uc.butSolo.Click += AfficherChoixPerso;
        }

        private void AfficherParametres(object sender, RoutedEventArgs e)
        {
            UCParametres uc = new UCParametres();
            ZoneJeu.Content = uc;
            uc.butRetour.Click += AfficheDemarage;
            uc.butPlayer1.Click += AfficherChoixTouches;
            uc.butPlayer2.Click += AfficherChoixTouches;
        }
        private void AfficherAide(object sender, RoutedEventArgs e)
        {
            UCAide uc = new UCAide();
            ZoneJeu.Content = uc;
            uc.butRetour.Click += AfficheDemarage;
        }
        private void AfficherChoixPerso(object sender, RoutedEventArgs e)
        {
            UCChoixPerso uc = new UCChoixPerso();
            ZoneJeu.Content = uc;
            uc.butRetour.Click += AfficheDemarage;
            uc.butStart.Click += AfficherChoixTerrain;
        }
        private void AfficherChoixTouches(object sender, RoutedEventArgs e)
        {
            UCChoixTouches uc = new UCChoixTouches();
            ZoneJeu.Content = uc;
            uc.butRetour.Click += AfficherParametres;
        }
        private void AfficherChoixTerrain(object sender, RoutedEventArgs e)
        {
            UCChoixTouches uc = new UCChoixTouches();
            ZoneJeu.Content = uc;
            uc.butRetour.Click += AfficherParametres;
        }
    }
}