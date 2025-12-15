using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace TRAINBattle
{
    /// <summary>
    /// Logique d'interaction pour UCJeux.xaml
    /// </summary>
    public partial class UCJeux : UserControl
    {
        //private static DispatcherTimer minuterie;
        Stopwatch stopwatch = new Stopwatch();
        long lastTick = 0;
        private static Personnage p1;
        //private bool personnagesInitialise = false;
        private static Personnage[] personnages;
        private static Personnage[] players;
        private static List<Projectils> ProjectilsEnJeu;
        private static Bot bot;

        private bool jeuEncours = false;
        private long gameOverStartTime = 0;
        private const int GAME_OVER_DURATION = 3000; // 3 secondes

        public UCJeux()
        {
            InitializeComponent();
            CompositionTarget.Rendering += Jeu;
            stopwatch.Start();
            ProjectilsEnJeu = new List<Projectils>();
            InitialisePersonages();
            players = new Personnage[2];
            players[0]=personnages[2];
            players[1]=personnages[1];
            players[0].Number = 0;
            players[1].Number = 1;
            players[0].FlipHealthBar(1);
            players[1].FlipHealthBar(-1);
            // if solo
            bot = new Bot(players[1], players[0], ProjectilsEnJeu);
            ResetGame();
            //players[0].Number = 0;
            //players[1].Number = 1;
            //players[0].X = 50;
            //players[1].X = 975;
            //players[0].FlipHealthBar(1);
            //players[1].FlipHealthBar(-1);
            //personnages[1].Flip();
            //players[0].SetAnimation("attente");
            //players[1].SetAnimation("attente");

            this.Loaded += UCJeux_Loaded;
            this.KeyDown += UCJeux_KeyDown;
            this.KeyUp += UCJeux_KeyUp;
            this.Focusable = true;

        }

        private void ResetGame()
        {
            players[0].X = 50;
            players[1].X = 975;
            players[0].Y = 0;
            players[1].Y = 0;
            if (!players[0].OrientationDroite)
            {
                players[0].Flip();
            }
            if (players[1].OrientationDroite)
            {
                players[1].Flip();
            }
            players[0].SetAnimation("attente");
            players[1].SetAnimation("attente");
            players[0].Vie = 100;
            players[1].Vie = 100;
        }

        private void InitialisePersonages()
        {
            personnages = new Personnage[4] {
            new Personnage(0, 0),new Personnage(0, 0),new Personnage(0, 0),new Personnage(0, 0)
            };
            //for (int i = 0; i < personnages.Length; i++)
            for (int i = 0; i < 2; i++) // car les 2 premiers perso sont juste des colors swap
            {
                // attente 2-0-0 => 2
                personnages[i].AddAnimation("attente", new Animation("attente"));
                personnages[i].Animations["attente"].AddFrame(new Frame($"train{i + 1}/deplacement0.png", 2));
                personnages[i].Animations["attente"].Frames[0].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["attente"].Frames[0].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["attente"].AddFrame(new Frame($"train{i + 1}/deplacement1.png", 2));
                personnages[i].Animations["attente"].Frames[1].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["attente"].Frames[1].AddHearthbox(0, 100, 100, 50);
                // marche 2-0-0 => 2
                personnages[i].AddAnimation("marche", new Animation("marche"));
                personnages[i].Animations["marche"].AddFrame(new Frame($"train{i + 1}/deplacement0.png", 2, 0, 10));
                personnages[i].Animations["marche"].Frames[0].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["marche"].Frames[0].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["marche"].AddFrame(new Frame($"train{i + 1}/deplacement1.png", 2, 0, 10));
                personnages[i].Animations["marche"].Frames[1].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["marche"].Frames[1].AddHearthbox(0, 100, 100, 50);
                // coup leger 5-3-0 => 8
                personnages[i].AddAnimation("coupleger", new Animation("marche"));
                personnages[i].Animations["coupleger"].AddFrame(new Frame($"train{i + 1}/poing0.png", 3));
                personnages[i].Animations["coupleger"].Frames[0].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["coupleger"].Frames[0].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["coupleger"].AddFrame(new Frame($"train{i + 1}/poing1.png", 2));
                personnages[i].Animations["coupleger"].Frames[1].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["coupleger"].Frames[1].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["coupleger"].AddFrame(new Frame($"train{i + 1}/poing2.png", 1, 3, 0));
                personnages[i].Animations["coupleger"].Frames[2].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["coupleger"].Frames[2].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["coupleger"].Frames[2].AddHitbox(204, 68, 28, 32);
                personnages[i].Animations["coupleger"].AddFrame(new Frame($"train{i + 1}/poing3.png", 2, 5, 0));
                personnages[i].Animations["coupleger"].Frames[3].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["coupleger"].Frames[3].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["coupleger"].Frames[3].AddHitbox(216, 68, 28, 32);
                // bouclier 3-2+-0 => 5+
                personnages[i].AddAnimation("bouclier", new Animation("bouclier"));
                personnages[i].Animations["bouclier"].AddFrame(new Frame($"train{i + 1}/bouclier0.png", 1));
                personnages[i].Animations["bouclier"].Frames[0].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["bouclier"].Frames[0].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["bouclier"].AddFrame(new Frame($"train{i + 1}/bouclier1.png", 1));
                personnages[i].Animations["bouclier"].Frames[1].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["bouclier"].Frames[1].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["bouclier"].AddFrame(new Frame($"train{i + 1}/bouclier2.png", 1));
                personnages[i].Animations["bouclier"].Frames[2].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["bouclier"].Frames[2].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["bouclier"].AddFrame(new Frame($"train{i + 1}/bouclier3.png", 1));
                personnages[i].Animations["bouclier"].Frames[3].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["bouclier"].Frames[3].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["bouclier"].Frames[3].AddHearthbox(208, 4, 40, 152);
                personnages[i].Animations["bouclier"].Frames[3].Type = "protect";
                personnages[i].Animations["bouclier"].AddFrame(new Frame($"train{i + 1}/bouclier4.png", 1));
                personnages[i].Animations["bouclier"].Frames[4].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["bouclier"].Frames[4].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["bouclier"].Frames[4].Type = "protect";
                personnages[i].Animations["bouclier"].Frames[4].AddHearthbox(208, 4, 40, 152);
                // saisie 6-2-0 => 8
                personnages[i].AddAnimation("saisie", new Animation("saisie"));
                personnages[i].Animations["saisie"].AddFrame(new Frame($"train{i + 1}/grab0.png", 1));
                personnages[i].Animations["saisie"].Frames[0].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["saisie"].Frames[0].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["saisie"].AddFrame(new Frame($"train{i + 1}/grab1.png", 1));
                personnages[i].Animations["saisie"].Frames[1].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["saisie"].Frames[1].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["saisie"].AddFrame(new Frame($"train{i + 1}/grab2.png", 1));
                personnages[i].Animations["saisie"].Frames[2].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["saisie"].Frames[2].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["saisie"].AddFrame(new Frame($"train{i + 1}/grab3.png", 1));
                personnages[i].Animations["saisie"].Frames[3].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["saisie"].Frames[3].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["saisie"].Frames[3].Type = "grab";
                personnages[i].Animations["saisie"].AddFrame(new Frame($"train{i + 1}/grab4.png", 1));
                personnages[i].Animations["saisie"].Frames[4].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["saisie"].Frames[4].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["saisie"].Frames[4].Type = "grab";
                personnages[i].Animations["saisie"].AddFrame(new Frame($"train{i + 1}/grab5.png", 1));
                personnages[i].Animations["saisie"].Frames[5].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["saisie"].Frames[5].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["saisie"].Frames[5].Type = "grab";
                personnages[i].Animations["saisie"].AddFrame(new Frame($"train{i + 1}/grab6.png", 2, 10, 0));
                personnages[i].Animations["saisie"].Frames[6].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["saisie"].Frames[6].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["saisie"].Frames[6].AddHitbox(225, 39, 24, 36);
                personnages[i].Animations["saisie"].Frames[6].Type = "grab";
                // dash 10-0-0 => 10
                personnages[i].AddAnimation("dash", new Animation("dash"));
                personnages[i].Animations["dash"].AddFrame(new Frame($"train{i + 1}/dash0.png", 1, 0, 2));
                personnages[i].Animations["dash"].Frames[0].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["dash"].Frames[0].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["dash"].AddFrame(new Frame($"train{i + 1}/dash1.png", 1, 0, 5));
                personnages[i].Animations["dash"].Frames[1].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["dash"].Frames[1].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["dash"].AddFrame(new Frame($"train{i + 1}/dash2.png", 1, 0, 8));
                personnages[i].Animations["dash"].Frames[2].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["dash"].Frames[2].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["dash"].AddFrame(new Frame($"train{i + 1}/dash3.png", 2, 3, 15));
                personnages[i].Animations["dash"].Frames[3].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["dash"].Frames[3].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["dash"].Frames[3].AddHitbox(200, 3, 52, 60);
                personnages[i].Animations["dash"].Frames[3].Type = "protect";
                personnages[i].Animations["dash"].AddFrame(new Frame($"train{i + 1}/dash4.png", 3, 4, 25));
                personnages[i].Animations["dash"].Frames[4].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["dash"].Frames[4].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["dash"].Frames[4].AddHitbox(200, 3, 52, 60);
                personnages[i].Animations["dash"].Frames[4].Type = "protect";
                // tirleger 3-1-7 => 11
                personnages[i].AddAnimation("tirleger", new Animation("tirleger"));
                personnages[i].Animations["tirleger"].AddFrame(new Frame($"train{i + 1}/deplacement0.png", 3));
                personnages[i].Animations["dash"].Frames[4].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["dash"].Frames[4].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["tirleger"].AddFrame(new Frame($"train{i + 1}/deplacement0.png", 1));
                personnages[i].Animations["dash"].Frames[4].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["dash"].Frames[4].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["tirleger"].Frames[1].AddProjectile("train1/tir0.png", 0, 60, 1, 0, 300, 3, false);
                personnages[i].Animations["tirleger"].AddFrame(new Frame($"train{i + 1}/deplacement0.png", 9));
                personnages[i].Animations["dash"].Frames[4].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["dash"].Frames[4].AddHearthbox(0, 100, 100, 50);
            }
            int j = 2;
            // attente 2-0-0 => 2
            personnages[j].AddAnimation("attente", new Animation("attente"));
            personnages[j].Animations["attente"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 2));
            personnages[j].Animations["attente"].Frames[0].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["attente"].Frames[0].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["attente"].AddFrame(new Frame($"train{j + 1}/deplacement1.png", 2));
            personnages[j].Animations["attente"].Frames[1].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["attente"].Frames[1].AddHearthbox(0, 100, 56, 16);
            // marche 2-0-0 => 2
            personnages[j].AddAnimation("marche", new Animation("marche"));
            personnages[j].Animations["marche"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 2, 0, 10));
            personnages[j].Animations["marche"].Frames[0].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["marche"].Frames[0].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["marche"].AddFrame(new Frame($"train{j + 1}/deplacement1.png", 2, 0, 10));
            personnages[j].Animations["marche"].Frames[1].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["marche"].Frames[1].AddHearthbox(0, 100, 56, 16);
            // coup leger 5-3-0 => 8
            personnages[j].AddAnimation("coupleger", new Animation("marche"));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger0.png", 3));
            personnages[j].Animations["coupleger"].Frames[0].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["coupleger"].Frames[0].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger1.png", 2));
            personnages[j].Animations["coupleger"].Frames[1].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["coupleger"].Frames[1].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger2.png", 1, 3, 0));
            personnages[j].Animations["coupleger"].Frames[2].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["coupleger"].Frames[2].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["coupleger"].Frames[2].AddHitbox(184, 60, 48, 20);
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger3.png", 2, 5, 0));
            personnages[j].Animations["coupleger"].Frames[3].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["coupleger"].Frames[3].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["coupleger"].Frames[3].AddHitbox(184, 65, 48, 16);
            personnages[j].Animations["coupleger"].Frames[3].AddHitbox(228, 68, 20, 8);
            // bouclier 3-2+-0 => 5+
            personnages[j].AddAnimation("bouclier", new Animation("bouclier"));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier0.png", 1));
            personnages[j].Animations["bouclier"].Frames[0].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["bouclier"].Frames[0].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier1.png", 1));
            personnages[j].Animations["bouclier"].Frames[1].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["bouclier"].Frames[1].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier2.png", 1));
            personnages[j].Animations["bouclier"].Frames[2].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["bouclier"].Frames[2].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["bouclier"].Frames[2].AddHearthbox(208, 4, 40, 152);
            personnages[j].Animations["bouclier"].Frames[2].Type = "protect";
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier3.png", 1));
            personnages[j].Animations["bouclier"].Frames[3].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["bouclier"].Frames[3].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["bouclier"].Frames[3].AddHearthbox(208, 4, 40, 152);
            personnages[j].Animations["bouclier"].Frames[3].Type = "protect";
            // saisie 6-2-0 => 8
            personnages[j].AddAnimation("saisie", new Animation("saisie"));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab0.png", 1));
            personnages[j].Animations["saisie"].Frames[0].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["saisie"].Frames[0].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab1.png", 1));
            personnages[j].Animations["saisie"].Frames[1].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["saisie"].Frames[1].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab2.png", 1));
            personnages[j].Animations["saisie"].Frames[2].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["saisie"].Frames[2].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab3.png", 1));
            personnages[j].Animations["saisie"].Frames[3].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["saisie"].Frames[3].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab4.png", 1));
            personnages[j].Animations["saisie"].Frames[4].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["saisie"].Frames[4].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab5.png", 1));
            personnages[j].Animations["saisie"].Frames[5].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["saisie"].Frames[5].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["saisie"].Frames[5].AddHitbox(227, 28, 24, 56);
            personnages[j].Animations["saisie"].Frames[5].Type = "grab";
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab6.png", 2, 10, 0));
            personnages[j].Animations["saisie"].Frames[6].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["saisie"].Frames[6].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["saisie"].Frames[6].AddHitbox(207, 64, 48, 52);
            personnages[j].Animations["saisie"].Frames[6].Type = "grab";
            // dash 10-0-0 => 10
            personnages[j].AddAnimation("dash", new Animation("dash"));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash0.png", 1, 0, 2));
            personnages[j].Animations["dash"].Frames[0].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["dash"].Frames[0].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash1.png", 1, 0, 5));
            personnages[j].Animations["dash"].Frames[1].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["dash"].Frames[1].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash2.png", 1, 0, 8));
            personnages[j].Animations["dash"].Frames[2].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["dash"].Frames[2].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash3.png", 1, 0, 18));
            personnages[j].Animations["dash"].Frames[3].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["dash"].Frames[3].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["dash"].Frames[3].Type = "protect";
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash4.png", 4, 3, 30));
            personnages[j].Animations["dash"].Frames[4].AddHearthbox(0, 0, 184, 100);
            personnages[j].Animations["dash"].Frames[4].AddHearthbox(0, 100, 56, 16);
            personnages[j].Animations["dash"].Frames[4].AddHitbox(184, 40, 32, 68);
            personnages[j].Animations["dash"].Frames[4].Type = "protect";
            // tirleger 3-1-7 => 11
            personnages[j].AddAnimation("tirleger", new Animation("tirleger"));
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 3));
            personnages[j].Animations["tirleger"].Frames[0].AddHearthbox(0, 0, 180, 100);
            personnages[j].Animations["tirleger"].Frames[0].AddHearthbox(0, 100, 100, 50);
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 1));
            personnages[j].Animations["tirleger"].Frames[1].AddHearthbox(0, 0, 180, 100);
            personnages[j].Animations["tirleger"].Frames[1].AddHearthbox(0, 100, 100, 50);
            personnages[j].Animations["tirleger"].Frames[1].AddProjectile("train1/tir0.png", 0, 60, 1, 0, 300, 3, false);
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 7));
            personnages[j].Animations["tirleger"].Frames[2].AddHearthbox(0, 0, 180, 100);
            personnages[j].Animations["tirleger"].Frames[2].AddHearthbox(0, 100, 100, 50);

            j = 3;
            // attente 2-0-0 => 2
            personnages[j].AddAnimation("attente", new Animation("attente"));
            personnages[j].Animations["attente"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 2));
            personnages[j].Animations["attente"].Frames[0].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["attente"].Frames[0].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["attente"].AddFrame(new Frame($"train{j + 1}/deplacement1.png", 2));
            personnages[j].Animations["attente"].Frames[1].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["attente"].Frames[1].AddHearthbox(140, 4, 76, 48);
            // marche 2-0-0 => 2
            personnages[j].AddAnimation("marche", new Animation("marche"));
            personnages[j].Animations["marche"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 2, 0, 10));
            personnages[j].Animations["attente"].Frames[0].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["attente"].Frames[0].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["marche"].AddFrame(new Frame($"train{j + 1}/deplacement1.png", 2, 0, 10));
            personnages[j].Animations["attente"].Frames[1].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["attente"].Frames[1].AddHearthbox(140, 4, 76, 48);
            // coup leger 5-3-0 => 8
            personnages[j].AddAnimation("coupleger", new Animation("marche"));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger0.png", 3));
            personnages[j].Animations["coupleger"].Frames[0].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["coupleger"].Frames[0].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger1.png", 2));
            personnages[j].Animations["coupleger"].Frames[1].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["coupleger"].Frames[1].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger2.png", 1, 3, 0));
            personnages[j].Animations["coupleger"].Frames[2].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["coupleger"].Frames[2].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger3.png", 2, 5, 0));
            personnages[j].Animations["coupleger"].Frames[3].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["coupleger"].Frames[3].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger3.png", 2, 10, 0));
            personnages[j].Animations["coupleger"].Frames[4].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["coupleger"].Frames[4].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["coupleger"].Frames[4].AddHitbox(108, 92, 44, 20);
            personnages[j].Animations["coupleger"].Frames[4].AddHitbox(144, 76, 40, 20);
            personnages[j].Animations["coupleger"].Frames[4].AddHitbox(144, 76, 40, 20);
            personnages[j].Animations["coupleger"].Frames[4].AddHitbox(184, 60, 28, 16);
            personnages[j].Animations["coupleger"].Frames[4].AddHitbox(216, 44, 20, 16);
            personnages[j].Animations["coupleger"].Frames[4].AddHitbox(232, 40, 20, 8);
            // bouclier 3-2+-0 => 5+
            personnages[j].AddAnimation("bouclier", new Animation("bouclier"));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier0.png", 1));
            personnages[j].Animations["bouclier"].Frames[0].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["bouclier"].Frames[0].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier1.png", 1));
            personnages[j].Animations["bouclier"].Frames[1].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["bouclier"].Frames[1].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier2.png", 1));
            personnages[j].Animations["bouclier"].Frames[2].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["bouclier"].Frames[2].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["bouclier"].Frames[2].AddHearthbox(188, 0, 68, 124);
            personnages[j].Animations["bouclier"].Frames[2].Type = "protect";
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier3.png", 1));
            personnages[j].Animations["bouclier"].Frames[3].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["bouclier"].Frames[3].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["bouclier"].Frames[3].AddHearthbox(188, 0, 68, 124);
            personnages[j].Animations["bouclier"].Frames[3].Type = "protect";
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier4.png", 1));
            personnages[j].Animations["bouclier"].Frames[4].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["bouclier"].Frames[4].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["bouclier"].Frames[4].AddHearthbox(188, 0, 68, 124);
            personnages[j].Animations["bouclier"].Frames[4].Type = "protect";
            // saisie 6-2-0 => 8
            personnages[j].AddAnimation("saisie", new Animation("saisie"));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab0.png", 1));
            personnages[j].Animations["saisie"].Frames[0].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["saisie"].Frames[0].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab1.png", 1));
            personnages[j].Animations["saisie"].Frames[1].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["saisie"].Frames[1].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab2.png", 1));
            personnages[j].Animations["saisie"].Frames[2].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["saisie"].Frames[2].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab3.png", 1));
            personnages[j].Animations["saisie"].Frames[3].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["saisie"].Frames[3].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab4.png", 1, 8, 0));
            personnages[j].Animations["saisie"].Frames[4].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["saisie"].Frames[4].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["saisie"].Frames[4].AddHitbox(216, 27, 40, 48);
            personnages[j].Animations["saisie"].Frames[4].Type = "grab";
            // dash 10-0-0 => 10
            personnages[j].AddAnimation("dash", new Animation("dash"));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash0.png", 1, 0, 2));
            personnages[j].Animations["dash"].Frames[0].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["dash"].Frames[0].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash1.png", 1, 0, 6));
            personnages[j].Animations["dash"].Frames[1].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["dash"].Frames[1].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash2.png", 1, 0, 10));
            personnages[j].Animations["dash"].Frames[2].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["dash"].Frames[2].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash3.png", 2, 4, 18));
            personnages[j].Animations["dash"].Frames[3].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["dash"].Frames[3].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["dash"].Frames[3].AddHitbox(184, 0, 56, 60);
            personnages[j].Animations["dash"].Frames[3].Type = "protect";
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash4.png", 4, 8, 30));
            personnages[j].Animations["dash"].Frames[4].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["dash"].Frames[4].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["dash"].Frames[4].AddHitbox(184, 0, 56, 60);
            personnages[j].Animations["dash"].Frames[4].Type = "protect";
            // tirleger 3-1-7 => 11
            personnages[j].AddAnimation("tirleger", new Animation("tirleger"));
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 3));
            personnages[j].Animations["dash"].Frames[1].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["dash"].Frames[2].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 1));
            personnages[j].Animations["dash"].Frames[2].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["dash"].Frames[2].AddHearthbox(140, 4, 76, 48);
            personnages[j].Animations["tirleger"].Frames[1].AddProjectile("train1/tir0.png", 0, 60, 1, 0, 300, 3, false);
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 7));
            personnages[j].Animations["dash"].Frames[3].AddHearthbox(0, 0, 140, 72);
            personnages[j].Animations["dash"].Frames[3].AddHearthbox(140, 4, 76, 48);
        }

        private void Jeu(object? sender, EventArgs e)
        {
            if (!jeuEncours)
            {
                canvasJeux.Children.Clear();

                // Affichage du message
                TextBlock txt = new TextBlock
                {
                    Text = players[0].EstMort() ? "JOUEUR 2 GAGNE !" : "JOUEUR 1 GAGNE !",
                    FontSize = 48,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White
                };

                Canvas.SetLeft(txt, 400);
                Canvas.SetTop(txt, 250);
                canvasJeux.Children.Add(txt);

                // Après 3 secondes → reset
                if (stopwatch.ElapsedMilliseconds - gameOverStartTime >= GAME_OVER_DURATION)
                {
                    jeuEncours = true;
                    ResetGame();
                }

                return; // ⛔ on ne joue plus pendant l’écran de fin
            }


            long now = stopwatch.ElapsedMilliseconds;
            long delta = now - lastTick;

            // Si pas assez de temps écoulé → ignorer cette frame
            if (delta < 1000.0 / 30.0)
                return;

            lastTick = now;
            //double dt = delta / 1000.0; // en secondes

#if DEBUG
            Console.WriteLine(delta);
#endif
            // remet le focus sur le jeu
            this.Focus();
            Keyboard.Focus(this);
            canvasJeux.Children.Clear();

            // if 2 joueurs
            bot.Update();

            //a1.GetCurrentFrame().Display(canvasJeux, 0, 520);
            //a1.Update();
            //Console.WriteLine($"{a1.CurrentFrame} {a1.IndexFrameActuel}");
            //if (a1.IsPlaying == false)
            //{
            //    a1.Reset();
            //}
            //System.Threading.Thread.Sleep(200);
            for (int i = 0; i < 2; i++)
            {
                players[i].Display(canvasJeux, 520);
                bool vienDeFinir = false;
                if (!players[i].Update(ProjectilsEnJeu))
                {
                    vienDeFinir = true;
                }

                if (MainWindow.TouchesActives[i, 1])
                {
                    if (vienDeFinir)
                    {
                        players[i].SetAnimation("dash");
                        vienDeFinir = false;
                    }
                }
                if (MainWindow.TouchesActives[i, 2])
                {
                    if (vienDeFinir) {
                        if (players[i].OrientationDroite)
                            players[i].Flip();

                        players[i].SetAnimation("marche");
                        vienDeFinir=false;
                    }
                }
                if (MainWindow.TouchesActives[i, 4])
                {
                    if (vienDeFinir)
                    {
                        players[i].SetAnimation("coupleger");
                        {
                            if (!players[i].OrientationDroite)
                                players[i].Flip();
                            players[i].SetAnimation("marche");
                        }
                        vienDeFinir = false;
                    }
                }
                if (MainWindow.TouchesActives[i, 7])
                {
                    if (vienDeFinir)
                    {
                        players[i].SetAnimation("coupleger");
                        vienDeFinir = false;
                    }
                }
                if (MainWindow.TouchesActives[i, 8])
                {
                    if (vienDeFinir)
                    {
                        players[i].SetAnimation("tirleger");
                        vienDeFinir = false;
                    }
                }
                if (MainWindow.TouchesActives[i, 9])
                {
                    if (vienDeFinir)
                    {
                        if (players[i].AnimationCourante.Name == "bouclier")
                        {
                            players[i].AnimationCourante.IsPlaying = true;
                            players[i].AnimationCourante.IndexFrameActuel -= 1;
                            //Console.WriteLine("here");
                        }
                        else
                        {
                            players[i].SetAnimation("bouclier");
                        }
                        vienDeFinir = false;
                    }
                    //if (players[i].AnimationCourante.)
                }
                if (MainWindow.TouchesActives[i, 10])
                {
                    if (vienDeFinir)
                    {
                        players[i].SetAnimation("saisie");
                        vienDeFinir = false;
                    }
                }

                if (MainWindow.TouchesActives[i, 3])
                {
                    players[i].Jump();
                }

                if (vienDeFinir)
                {
                    players[i].SetAnimation("attente");
                }

                bool stop = false;

                //players[(i + 1) % 2].AnimationCourante.GetCurrentFrame();

                //if (players[(i + 1) % 2].AnimationCourante.GetCurrentFrame().Type == "base" || players[(i + 1) % 2].AnimationCourante.GetCurrentFrame().Type == "grab" || players[i].AnimationCourante.GetCurrentFrame().Type == "grab")
                if (players[(i+1)%2].AnimationCourante.GetCurrentFrame().Type != "protect"  || players[i].AnimationCourante.GetCurrentFrame().Type == "grab")
                {
                    foreach (var hitbox in players[i].GetHitboxs())
                    {
                        //Console.WriteLine(players[(i + 1) % 2].StoneTime);
                        if (players[(i + 1) % 2].StoneTime > 0) break;
                        foreach (var hearthbox in players[(i + 1) % 2].GetHearthboxs())
                        {
                            if (hitbox.IntersectsWith(hearthbox))
                            {

                                players[(i + 1) % 2].InfligeDegat(players[i].AnimationCourante.GetCurrentFrame().Puissance, players[i].AnimationCourante.TimeFrameRestant());
                                if (players[i].AnimationCourante.GetCurrentFrame().Type == "grab")
                                {
                                    players[(i + 1) % 2].Flip();
                                }
                                stop = true; break;
                            }
                            if (stop) break;
                        }
                    }
                }
                players[i].DrawHealthBar(canvasJeux, 300 + i * 480, 50);
                //players[i].InfligeDegat(1);

            }

            foreach (var projectil in ProjectilsEnJeu.ToList()) // le tolist fait qu'on ne modif pas la liste pendent le foreach
            {
                if (projectil.Update())
                {
                    //Console.WriteLine(projectil.GetHitbox());
                    projectil.Affiche(canvasJeux, 200);
                    int i = (projectil.OwnerNumber + 1) % 2;
                    foreach (var hearthbox in players[i].GetHearthboxs())
                    {
                        if (projectil.GetHitbox().IntersectsWith(hearthbox))
                        {
                            if (players[i].StoneTime <= 0 && players[i].AnimationCourante.GetCurrentFrame().Type != "protect")
                            {
                                players[i].InfligeDegat(projectil.Damage, projectil.Damage);
                            }
                            ProjectilsEnJeu.Remove(projectil);
                            break;
                        }
                    }
                }
                else
                {
                    ProjectilsEnJeu.Remove(projectil);
                }
            }
            Console.WriteLine(players[0].EstMort());
            Console.WriteLine(players[1].EstMort());
            if (players[0].EstMort() || players[1].EstMort())
            {
                jeuEncours = false;
                gameOverStartTime = stopwatch.ElapsedMilliseconds;
            }
        }

        private void UCJeux_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);
        }


        private void UCJeux_KeyDown(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < MainWindow.Touches.GetLength(0); i++)
            { 
                for (int j = 0; j < MainWindow.Touches.GetLength(1); j++)
                {
                    if (MainWindow.Touches[i, j] == e.Key) {
                        MainWindow.TouchesActives[i, j] = true;
                    }
                }
            }
            
            //Console.WriteLine("WHOOOOOOOOOOO");

            //if (e.Key == Key.Space)
            //{
            //    p1.Flip();
            //    Console.WriteLine("WHAAAAAAAAAAAA");
            //}

        }

        private void UCJeux_KeyUp(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < MainWindow.Touches.GetLength(0); i++)
            {
                for (int j = 0; j < MainWindow.Touches.GetLength(1); j++)
                {
                    if (MainWindow.Touches[i, j] == e.Key)
                    {
                        MainWindow.TouchesActives[i, j] = false;
                    }
                }
            }
        }

    }
}
