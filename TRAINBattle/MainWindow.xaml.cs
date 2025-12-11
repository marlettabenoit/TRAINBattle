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
        public static double VolumeSon { get; set; }
        public static double VolumeMusique { get; set; }

        public static Key[,] Touches { get; set; } = new Key[2, 12];
        public static Key[,] TouchesParDefaut { get; set; } = new Key[2, 12];
        public static bool[,] TouchesActives { get; set;} = new bool[2, 12];

        public MainWindow()
        {
            InitializeComponent();
            AfficheDemarage(null, null);
            InitTouches();
        }

        private void InitTouches()
        {
            TouchesParDefaut[0, 0] = Key.OemBackslash;
            TouchesParDefaut[0, 1] = Key.W;
            TouchesParDefaut[0, 2] = Key.X;
            TouchesParDefaut[0, 3] = Key.C;
            TouchesParDefaut[0, 4] = Key.V;
            TouchesParDefaut[0, 5] = Key.B;
            TouchesParDefaut[0, 6] = Key.N;
            TouchesParDefaut[0, 7] = Key.OemComma;
            TouchesParDefaut[0, 8] = Key.OemPeriod;
            TouchesParDefaut[0, 9] = Key.OemQuestion;
            TouchesParDefaut[0, 10] = Key.Oem8;
            TouchesParDefaut[0, 11] = Key.RightShift;
            TouchesParDefaut[1, 0] = Key.D1;
            TouchesParDefaut[1, 1] = Key.D2;
            TouchesParDefaut[1, 2] = Key.D3;
            TouchesParDefaut[1, 3] = Key.D4;
            TouchesParDefaut[1, 4] = Key.D5;
            TouchesParDefaut[1, 5] = Key.D6;
            TouchesParDefaut[1, 6] = Key.D7;
            TouchesParDefaut[1, 7] = Key.D8;
            TouchesParDefaut[1, 8] = Key.D9;
            TouchesParDefaut[1, 9] = Key.D0;
            TouchesParDefaut[1, 10] = Key.OemOpenBrackets;
            TouchesParDefaut[1, 11] = Key.OemPlus;
            for (int i = 0; i < TouchesParDefaut.GetLength(0); i++)
            {
                for (int j = 0; j<TouchesParDefaut.GetLength(1); j++)
                {
                    Touches[i, j] = TouchesParDefaut[i, j];
                }
            }
        }

        private void AfficheDemarage(object sender, RoutedEventArgs e)
        {
            UCDemarage uc = new UCDemarage();
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
            UCChoixTerrain uc = new UCChoixTerrain();
            ZoneJeu.Content = uc;
            uc.butRetour.Click += AfficherChoixPerso;
            uc.butTerrain1.Click += AfficherJeu;
            uc.butTerrain2.Click += AfficherJeu;
            uc.butTerrain3.Click += AfficherJeu;
            uc.butTerrain4.Click += AfficherJeu;
        }
        private void AfficherJeu(object sender, RoutedEventArgs e)
        {
            UCJeux uc = new UCJeux();
            ZoneJeu.Content = uc;
            //uc.butRetour.Click += AfficherChoixPerso;
        }
    }
}