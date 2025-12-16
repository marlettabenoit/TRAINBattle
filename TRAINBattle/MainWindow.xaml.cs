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
        // Ci dessous se trouvent un tas de variables qui ont le bon gout d'être acsécible et modifiable depuis n'importe ou
        public static double VolumeSon { get; set; } = 0.5; // finalement pas implémenté, mais pas enlevé pour que ce soit plus simple à ajouter dans le futur
        public static double VolumeMusique { get; set; } = 0.5;
        public static MediaPlayer MusicPlayer = new MediaPlayer();
        public static int PlayerTouchesModifie = 0; // Contien le numero du joueur dont on modifie les touches
        public static Key[,] Touches { get; set; } = new Key[2, 12];
        public static Key[,] TouchesParDefaut { get; set; } = new Key[2, 12];
        public static bool[,] TouchesActives { get; set;} = new bool[2, 12];
        public static int IndexPersoP1 { get; set; } = 0;
        public static int IndexPersoP2 { get; set; } = 1;
        public static bool BotActif { get; set; } = false;
        public static string TerrainActif { get; set; } = "terrain1.png";

        // Constructeur
        public MainWindow()
        {
            InitializeComponent();
            AfficheDemarage(null, null); // Car sinon, il se passera pas grand chose
            InitTouches();
            InitMusic(); // sinon, je l'ai fait pour rien, ce qui serai triste
        }

        private void InitMusic()
        {
            MusicPlayer.Open(new Uri("son/musicPrincipale.mp3", UriKind.Relative));
            MusicPlayer.MediaEnded += (s, e) =>
            {
                MusicPlayer.Position = TimeSpan.Zero; // reset
                MusicPlayer.Play();                  // relance
            };
            MusicPlayer.Volume = VolumeMusique;
            MusicPlayer.Play();
        }

        // initialise les touches par defaut et les vrais touches
        private void InitTouches()
        {
            // les touchess par defaut ne sont elles aussi pas implémentés, mais seront utiles pour de futurs maj
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
            TouchesParDefaut[1, 0] = Key.OemPlus;
            TouchesParDefaut[1, 1] = Key.OemOpenBrackets;
            TouchesParDefaut[1, 2] = Key.D0;
            TouchesParDefaut[1, 3] = Key.D9;
            TouchesParDefaut[1, 4] = Key.D8;
            TouchesParDefaut[1, 5] = Key.D7;
            TouchesParDefaut[1, 6] = Key.D6;
            TouchesParDefaut[1, 7] = Key.D5;
            TouchesParDefaut[1, 8] = Key.D4;
            TouchesParDefaut[1, 9] = Key.D3;
            TouchesParDefaut[1, 10] = Key.D2;
            TouchesParDefaut[1, 11] = Key.D1;
            for (int i = 0; i < TouchesParDefaut.GetLength(0); i++)
            {
                for (int j = 0; j<TouchesParDefaut.GetLength(1); j++)
                {
                    Touches[i, j] = TouchesParDefaut[i, j];
                }
            }
        }

        // Affiche l'uc demarage
        private void AfficheDemarage(object sender, RoutedEventArgs e)
        {
            UCDemarage uc = new UCDemarage();
            ZoneJeu.Content = uc;
            uc.butParametres.Click += AfficherParametres;
            uc.butAide.Click += AfficherAide;
            // En fonction du bouton, le bot est actif ou non
            uc.butVersus.Click += (s, e) =>
            {
                BotActif = false;
                AfficherChoixPerso(s, e);
            };
            uc.butSolo.Click += (s, e) =>
            {
                BotActif = true;
                AfficherChoixPerso(s, e);
            };
        }

        // Affiche l'uc parametres
        private void AfficherParametres(object sender, RoutedEventArgs e)
        {
            UCParametres uc = new UCParametres();
            ZoneJeu.Content = uc;
            uc.butRetour.Click += AfficheDemarage;
            uc.butPlayer1.Click += AfficherChoixTouches;
            uc.butPlayer2.Click += AfficherChoixTouches;
        }
        
        // Affiche l'uc aide
        private void AfficherAide(object sender, RoutedEventArgs e)
        {
            UCAide uc = new UCAide();
            ZoneJeu.Content = uc;
            uc.butRetour.Click += AfficheDemarage;
        }

        // affiche l'uc choix perso
        // public pour pouvoir etre appelé depuis ucjeu.xml.cs
        public void AfficherChoixPerso(object sender, RoutedEventArgs e)
        {
            UCChoixPerso uc = new UCChoixPerso();
            ZoneJeu.Content = uc;
            uc.butRetour.Click += AfficheDemarage;
            uc.butStart.Click += AfficherChoixTerrain;
        }

        // Affiche l'uc choi des touches
        private void AfficherChoixTouches(object sender, RoutedEventArgs e)
        {
            UCChoixTouches uc = new UCChoixTouches();
            ZoneJeu.Content = uc;
            uc.butRetour.Click += AfficherParametres;
        }

        // affiche l'uc de choi de terrin
        private void AfficherChoixTerrain(object sender, RoutedEventArgs e)
        {
            UCChoixTerrain uc = new UCChoixTerrain();
            ZoneJeu.Content = uc;
            uc.butRetour.Click += AfficherChoixPerso;
            // On change le terrin à afficher en fonction du terrin choisi
            uc.butTerrain1.Click += (s, e) =>
            {
                TerrainActif = "terrain1.png";
                AfficherJeu(s, e);
            };
            uc.butTerrain2.Click += (s, e) =>
            {
                TerrainActif = "terrain2.png";
                AfficherJeu(s, e);
            };
            uc.butTerrain3.Click += (s, e) =>
            {
                TerrainActif = "terrain3.png";
                AfficherJeu(s, e);
            };
            uc.butTerrain4.Click += (s, e) =>
            {
                TerrainActif = "terrain4.png";
                AfficherJeu(s, e);
            };
        }

        // Affiche l'uc jeu
        private void AfficherJeu(object sender, RoutedEventArgs e)
        {
            UCJeux uc = new UCJeux();
            ZoneJeu.Content = uc;
            //uc.butRetour.Click += AfficherChoixPerso;
        }
    }
}