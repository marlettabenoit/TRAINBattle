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
        // Constante
        private const int HauteurSol = 520;
        // Déclaration des variables
        Stopwatch stopwatch = new Stopwatch();
        long lastTick = 0;
        private static Personnage p1;
        private static Personnage[] personnages;
        private static Personnage[] players;
        private static List<Projectil> ProjectilsEnJeu;
        private static Bot bot;
        private bool jeuEncours = false;
        private long gameOverStartTime = 0;
        private const int GAME_OVER_DURATION = 3000; // 3 secondes
        private int scoreP1 = 0;
        private int scoreP2 = 0;

        public UCJeux()
        {
            InitializeComponent();
            CompositionTarget.Rendering += Jeu; // abonne jeu au rendering (en gros on apelle jeu ultra souvent)
            stopwatch.Start();
            ProjectilsEnJeu = new List<Projectil>();
            ChargerTerrain();
            InitialisePersonages();
            players = new Personnage[2];
            players[0]=personnages[MainWindow.IndexPersoP1];
            players[1]=personnages[MainWindow.IndexPersoP2];
            players[0].Number = 0;
            players[1].Number = 1;
            players[0].FlipHealthBar(1);
            players[1].FlipHealthBar(-1);
            // Si y'à le bot d'actif
            if (MainWindow.BotActif)
                bot = new Bot(players[1], players[0], ProjectilsEnJeu);
            // On met tout à plat
            ResetGame();
            ResetTouchesActives();
            // On fait plein d'abonnements
            this.Loaded += UCJeux_Loaded; // Petite astuce detaille plus bas
            this.KeyDown += UCJeux_KeyDown; // gestion appuis
            this.KeyUp += UCJeux_KeyUp; // gestion relache
            this.Focusable = true; // permet de donner le focus à l'uc
            // On affiche un message bien badas
            blockInfo.Visibility = Visibility.Visible;
            blockInfo.Text = "3 rounds\n1 winner";
        }

        // Permet de reset le jeu
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
            ProjectilsEnJeu.Clear();
        }

        // Initialise les personnages
        private void InitialisePersonages()
        {
            personnages = new Personnage[4] {
            new Personnage(0, 0),new Personnage(0, 0),new Personnage(0, 0),new Personnage(0, 0)
            };
            // Perso 1
            int j = 0;
            // attente
            personnages[j].AddAnimation("attente", new Animation("attente"));
            personnages[j].Animations["attente"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 2));
            personnages[j].Animations["attente"].AddFrame(new Frame($"train{j + 1}/deplacement1.png", 2));
            // marche
            personnages[j].AddAnimation("marche", new Animation("marche"));
            personnages[j].Animations["marche"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 2, 0, 10));
            personnages[j].Animations["marche"].AddFrame(new Frame($"train{j + 1}/deplacement1.png", 2, 0, 10));
            // coup leger
            personnages[j].AddAnimation("coupleger", new Animation("coupleger"));
            personnages[j].Animations["coupleger"].AssignerSon("son\\train1attaqueleger.wav");
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/poing0.png", 3));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/poing1.png", 2));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/poing2.png", 1, 3, 0));
            personnages[j].Animations["coupleger"].Frames[2].AddHitbox(204, 68, 28, 32);
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/poing3.png", 2, 5, 0));
            personnages[j].Animations["coupleger"].Frames[3].AddHitbox(216, 68, 28, 32);
            // coup lourd
            personnages[j].AddAnimation("couplourd", new Animation("couplourd"));
            personnages[j].Animations["couplourd"].AssignerSon("son\\train1attaquelourd.wav");
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd0.png", 3));
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd1.png", 2));
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd2.png", 2));
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd3.png", 2));
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd4.png", 2, 15, 0));
            personnages[j].Animations["couplourd"].Frames[4].AddHitbox(220, 60, 28, 60);
            // bouclier
            personnages[j].AddAnimation("bouclier", new Animation("bouclier"));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier0.png", 1));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier1.png", 1));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier2.png", 1));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier3.png", 1));
            personnages[j].Animations["bouclier"].Frames[3].AddHearthbox(208, 4, 40, 152);
            personnages[j].Animations["bouclier"].Frames[3].Type = "protect";
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier4.png", 1));
            personnages[j].Animations["bouclier"].Frames[4].Type = "protect";
            personnages[j].Animations["bouclier"].Frames[4].AddHearthbox(208, 4, 40, 152);
            // saisie
            personnages[j].AddAnimation("saisie", new Animation("saisie"));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab0.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab1.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab2.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab3.png", 1));
            personnages[j].Animations["saisie"].Frames[3].Type = "grab";
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab4.png", 1));
            personnages[j].Animations["saisie"].Frames[4].Type = "grab";
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab5.png", 1));
            personnages[j].Animations["saisie"].Frames[5].Type = "grab";
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab6.png", 2, 10, 0));
            personnages[j].Animations["saisie"].Frames[6].AddHitbox(225, 39, 24, 36);
            personnages[j].Animations["saisie"].Frames[6].Type = "grab";
            // dash
            personnages[j].AddAnimation("dash", new Animation("dash"));
            personnages[j].Animations["dash"].AssignerSon("son\\train1dash.wav");
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash0.png", 1, 0, 2));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash1.png", 1, 0, 5));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash2.png", 1, 0, 8));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash3.png", 2, 3, 15));
            personnages[j].Animations["dash"].Frames[3].AddHitbox(200, 3, 52, 60);
            personnages[j].Animations["dash"].Frames[3].Type = "protect";
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash4.png", 3, 4, 25));
            personnages[j].Animations["dash"].Frames[4].AddHitbox(200, 3, 52, 60);
            personnages[j].Animations["dash"].Frames[4].Type = "protect";
            // tirleger
            personnages[j].AddAnimation("tirleger", new Animation("tirleger"));
            personnages[j].Animations["tirleger"].AssignerSon("son\\train1tir.wav");
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 3));
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 1));
            personnages[j].Animations["tirleger"].Frames[1].AddProjectile("train1/tir.png", 0, 60, 1, 0, 300, 3, false);
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 9));

            personnages[j].AddHearthtboxToAllAnimation(
                new System.Drawing.Rectangle[] { new System.Drawing.Rectangle(0, 0, 180, 100), new System.Drawing.Rectangle(0, 100, 100, 50) }
            );

            // Perso 2
            j = 1;
            // attente
            personnages[j].AddAnimation("attente", new Animation("attente"));
            personnages[j].Animations["attente"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 2));
            personnages[j].Animations["attente"].AddFrame(new Frame($"train{j + 1}/deplacement1.png", 2));
            // marche
            personnages[j].AddAnimation("marche", new Animation("marche"));
            personnages[j].Animations["marche"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 2, 0, 10));
            personnages[j].Animations["marche"].AddFrame(new Frame($"train{j + 1}/deplacement1.png", 2, 0, 10));
            // coup leger
            personnages[j].AddAnimation("coupleger", new Animation("coupleger"));
            personnages[j].Animations["coupleger"].AssignerSon("son\\train2attaqueleger.wav");
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/poing0.png", 3));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/poing1.png", 2));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/poing2.png", 1));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/poing3.png", 2, 6, 0));
            personnages[j].Animations["coupleger"].Frames[3].AddHitbox(176, 32, 80, 76);
            // coup lourd
            personnages[j].AddAnimation("couplourd", new Animation("couplourd"));
            personnages[j].Animations["couplourd"].AssignerSon("son\\train2attaquelourd.wav");
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd0.png", 3));
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd1.png", 2));
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd2.png", 2));
            personnages[j].Animations["couplourd"].Frames[2].AddHitbox(176, 28, 56, 96);
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd3.png", 2, 14, 0));
            personnages[j].Animations["couplourd"].Frames[3].AddHitbox(176, 20, 76, 116);
            // bouclier
            personnages[j].AddAnimation("bouclier", new Animation("bouclier"));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier0.png", 1));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier1.png", 1));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier2.png", 1));
            personnages[j].Animations["bouclier"].Frames[2].AddHearthbox(192, 12, 44, 112);
            personnages[j].Animations["bouclier"].Frames[2].Type = "protect";
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier3.png", 1));
            personnages[j].Animations["bouclier"].Frames[3].AddHearthbox(192, 12, 48, 112);
            personnages[j].Animations["bouclier"].Frames[3].Type = "protect";
            // saisie
            personnages[j].AddAnimation("saisie", new Animation("saisie"));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab0.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab1.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab2.png", 1));
            personnages[j].Animations["saisie"].Frames[2].AddHitbox(180, 56, 24, 24);
            personnages[j].Animations["saisie"].Frames[2].Type = "grab";
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab3.png", 1, 20, 0));
            personnages[j].Animations["saisie"].Frames[3].AddHitbox(208, 40, 24, 24);
            personnages[j].Animations["saisie"].Frames[3].Type = "grab";
            // dash
            personnages[j].AddAnimation("dash", new Animation("dash"));
            personnages[j].Animations["dash"].AssignerSon("son\\train2dash.wav");
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash0.png", 1, 0, 2));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash1.png", 1, 0, 5));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash2.png", 1, 0, 8));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash3.png", 2, 0, 15));
            personnages[j].Animations["dash"].Frames[3].Type = "protect";
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash4.png", 3, 4, 25));
            personnages[j].Animations["dash"].Frames[4].AddHitbox(172, 4, 60, 44);
            personnages[j].Animations["dash"].Frames[4].Type = "protect";
            // tirleger
            personnages[j].AddAnimation("tirleger", new Animation("tirleger"));
            personnages[j].Animations["tirleger"].AssignerSon("son\\train2tir.wav");
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 3));
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 1));
            personnages[j].Animations["tirleger"].Frames[1].AddProjectile("train2/tir.png", -100, 68, 1, 0, 300, 3, false);
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 9));

            personnages[j].AddHearthtboxToAllAnimation(
                new System.Drawing.Rectangle[] { new System.Drawing.Rectangle(0, 0, 116, 68), new System.Drawing.Rectangle(116, 0, 72, 40) }
            );

            // Perso 3
            j = 2;
            // attente
            personnages[j].AddAnimation("attente", new Animation("attente"));
            personnages[j].Animations["attente"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 2));
            personnages[j].Animations["attente"].AddFrame(new Frame($"train{j + 1}/deplacement1.png", 2));
            // marche
            personnages[j].AddAnimation("marche", new Animation("marche"));
            personnages[j].Animations["marche"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 2, 0, 10));
            personnages[j].Animations["marche"].AddFrame(new Frame($"train{j + 1}/deplacement1.png", 2, 0, 10));
            // coup leger
            personnages[j].AddAnimation("coupleger", new Animation("coupleger"));
            personnages[j].Animations["coupleger"].AssignerSon("son\\train3attaqueleger.wav");
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger0.png", 3));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger1.png", 2));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger2.png", 1, 3, 0));
            personnages[j].Animations["coupleger"].Frames[2].AddHitbox(184, 60, 48, 20);
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger3.png", 2, 5, 0));
            personnages[j].Animations["coupleger"].Frames[3].AddHitbox(184, 65, 48, 16);
            personnages[j].Animations["coupleger"].Frames[3].AddHitbox(228, 68, 20, 8);
            // coup leger
            personnages[j].AddAnimation("couplourd", new Animation("couplourd"));
            personnages[j].Animations["couplourd"].AssignerSon("son\\train3attaquelourd.wav");
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd0.png", 3));
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd1.png", 2));
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd2.png", 1, 7, 0));
            personnages[j].Animations["couplourd"].Frames[2].AddHitbox(220, 0, 36, 144);
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd3.png", 2, 8, 0));
            personnages[j].Animations["couplourd"].Frames[3].AddHitbox(220, 0, 36, 144);
            // bouclier
            personnages[j].AddAnimation("bouclier", new Animation("bouclier"));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier0.png", 1));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier1.png", 1));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier2.png", 1));
            personnages[j].Animations["bouclier"].Frames[2].AddHearthbox(208, 4, 40, 152);
            personnages[j].Animations["bouclier"].Frames[2].Type = "protect";
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier3.png", 1));
            personnages[j].Animations["bouclier"].Frames[3].AddHearthbox(208, 4, 40, 152);
            personnages[j].Animations["bouclier"].Frames[3].Type = "protect";
            // saisie
            personnages[j].AddAnimation("saisie", new Animation("saisie"));
            personnages[j].Animations["saisie"].AssignerSon("son\\train3grab.wav");
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab0.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab1.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab2.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab3.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab4.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab5.png", 1));
            personnages[j].Animations["saisie"].Frames[5].AddHitbox(227, 28, 24, 56);
            personnages[j].Animations["saisie"].Frames[5].Type = "grab";
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab6.png", 2, 10, 0));
            personnages[j].Animations["saisie"].Frames[6].AddHitbox(207, 64, 48, 52);
            personnages[j].Animations["saisie"].Frames[6].Type = "grab";
            // dash
            personnages[j].AddAnimation("dash", new Animation("dash"));
            personnages[j].Animations["dash"].AssignerSon("son\\train3dash.wav");
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash0.png", 1, 0, 2));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash1.png", 1, 0, 5));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash2.png", 1, 0, 8));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash3.png", 1, 0, 18));
            personnages[j].Animations["dash"].Frames[3].Type = "protect";
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash4.png", 4, 3, 30));
            personnages[j].Animations["dash"].Frames[4].AddHitbox(184, 40, 32, 68);
            personnages[j].Animations["dash"].Frames[4].Type = "protect";
            // tirleger
            personnages[j].AddAnimation("tirleger", new Animation("tirleger"));
            personnages[j].Animations["tirleger"].AssignerSon("son\\train3tir.wav");
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 3));
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 1));
            personnages[j].Animations["tirleger"].Frames[1].AddProjectile("train3/tir.png", 0, 60, 1, 0, 300, 3, false);
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 7));

            personnages[j].AddHearthtboxToAllAnimation(
                new System.Drawing.Rectangle[] { new System.Drawing.Rectangle(0, 0, 184, 100), new System.Drawing.Rectangle(0, 100, 56, 16) }
            );

            // Perso 4
            j = 3;
            // attente
            personnages[j].AddAnimation("attente", new Animation("attente"));
            personnages[j].Animations["attente"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 2));
            personnages[j].Animations["attente"].AddFrame(new Frame($"train{j + 1}/deplacement1.png", 2));
            // marche
            personnages[j].AddAnimation("marche", new Animation("marche"));
            personnages[j].Animations["marche"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 2, 0, 10));
            personnages[j].Animations["marche"].AddFrame(new Frame($"train{j + 1}/deplacement1.png", 2, 0, 10));
            // coup leger
            personnages[j].AddAnimation("coupleger", new Animation("coupleger"));
            personnages[j].Animations["coupleger"].AssignerSon("son\\train4attaqueleger.wav");
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger0.png", 3));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger1.png", 2));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger2.png", 1, 3, 0));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger3.png", 2, 5, 0));
            personnages[j].Animations["coupleger"].AddFrame(new Frame($"train{j + 1}/attaqueleger3.png", 2, 10, 0));
            personnages[j].Animations["coupleger"].Frames[4].AddHitbox(108, 92, 44, 20);
            personnages[j].Animations["coupleger"].Frames[4].AddHitbox(144, 76, 40, 20);
            personnages[j].Animations["coupleger"].Frames[4].AddHitbox(144, 76, 40, 20);
            personnages[j].Animations["coupleger"].Frames[4].AddHitbox(184, 60, 28, 16);
            personnages[j].Animations["coupleger"].Frames[4].AddHitbox(216, 44, 20, 16);
            personnages[j].Animations["coupleger"].Frames[4].AddHitbox(232, 40, 20, 8);
            // coup lourd
            personnages[j].AddAnimation("couplourd", new Animation("couplourd"));
            //personnages[j].Animations["couplourd"].AssignerSon("son\\train4attaquelourd.wav");
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd0.png", 3));
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd1.png", 2));
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd2.png", 1, 3, 0));
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd3.png", 2, 5, 0));
            personnages[j].Animations["couplourd"].AddFrame(new Frame($"train{j + 1}/attaquelourd3.png", 2, 20, 0));
            personnages[j].Animations["couplourd"].Frames[4].AddHitbox(216, 0, 36, 143);
            // bouclier
            personnages[j].AddAnimation("bouclier", new Animation("bouclier"));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier0.png", 1));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier1.png", 1));
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier2.png", 1));
            personnages[j].Animations["bouclier"].Frames[2].AddHearthbox(188, 0, 68, 124);
            personnages[j].Animations["bouclier"].Frames[2].Type = "protect";
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier3.png", 1));
            personnages[j].Animations["bouclier"].Frames[3].AddHearthbox(188, 0, 68, 124);
            personnages[j].Animations["bouclier"].Frames[3].Type = "protect";
            personnages[j].Animations["bouclier"].AddFrame(new Frame($"train{j + 1}/bouclier4.png", 1));
            personnages[j].Animations["bouclier"].Frames[4].AddHearthbox(188, 0, 68, 124);
            personnages[j].Animations["bouclier"].Frames[4].Type = "protect";
            // saisie
            personnages[j].AddAnimation("saisie", new Animation("saisie"));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab0.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab1.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab2.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab3.png", 1));
            personnages[j].Animations["saisie"].AddFrame(new Frame($"train{j + 1}/grab4.png", 1, 8, 0));
            personnages[j].Animations["saisie"].Frames[4].AddHitbox(216, 27, 40, 48);
            personnages[j].Animations["saisie"].Frames[4].Type = "grab";
            // dash
            personnages[j].AddAnimation("dash", new Animation("dash"));
            personnages[j].Animations["dash"].AssignerSon("son\\train4dash.wav");
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash0.png", 1, 0, 2));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash1.png", 1, 0, 6));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash2.png", 1, 0, 10));
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash3.png", 2, 4, 18));
            personnages[j].Animations["dash"].Frames[3].AddHitbox(184, 0, 56, 60);
            personnages[j].Animations["dash"].Frames[3].Type = "protect";
            personnages[j].Animations["dash"].AddFrame(new Frame($"train{j + 1}/dash4.png", 4, 8, 30));
            personnages[j].Animations["dash"].Frames[4].AddHitbox(184, 0, 56, 60);
            personnages[j].Animations["dash"].Frames[4].Type = "protect";
            // tirleger
            personnages[j].AddAnimation("tirleger", new Animation("tirleger"));
            personnages[j].Animations["tirleger"].AssignerSon("son\\train4tir.wav");
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 3));
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 1));
            personnages[j].Animations["tirleger"].Frames[1].AddProjectile("train4/tir.png", 0, 60, 1, 0, 300, 3, false);
            personnages[j].Animations["tirleger"].AddFrame(new Frame($"train{j + 1}/deplacement0.png", 7));

            personnages[j].AddHearthtboxToAllAnimation(
                new System.Drawing.Rectangle[] { new System.Drawing.Rectangle(0, 0, 140, 72), new System.Drawing.Rectangle(140, 4, 76, 48) }
            );
        }

        // Fonction qui gére le coeur du jeu est qui est constament appelé
        private void Jeu(object? sender, EventArgs e)
        {
            long now = stopwatch.ElapsedMilliseconds;
            long delta = now - lastTick;

            // On limite à 30 fps
            if (delta < 1000.0 / 30.0)
                return;

            lastTick = now;

            // remet le focus sur le jeu
            this.Focus();
            Keyboard.Focus(this);

            // On reset le canvas
            canvasJeux.Children.Clear();

            // Si le jeu est pas actif
            if (!jeuEncours)
            {
                // quand 3 secondes ecoules, on relance le jeu
                if (stopwatch.ElapsedMilliseconds - gameOverStartTime >= GAME_OVER_DURATION)
                {
                    jeuEncours = true;
                    blockInfo.Visibility = Visibility.Hidden;
                    ResetGame();
                    // Si un des joueurs à gagné, c'est fini et on revien vers UCChoixperso
                    if (scoreP1>=2 || scoreP2>=2)
                    {
                        Cleanup();
                        var mainWindow = Application.Current.MainWindow as MainWindow;
                        mainWindow?.AfficherChoixPerso(this, null);
                    }
                }
                return;
            }

            // Au besoin update du bot
            if (MainWindow.BotActif)
                bot.Update();

            // Pour chacun des 2 joueurs
            for (int i = 0; i < 2; i++)
            {
                // On affiche le perso
                players[i].Display(canvasJeux, HauteurSol);
                bool vienDeFinir = false; // sert à savoir si on peux faire une nouvelle action
                if (!players[i].Update(ProjectilsEnJeu))
                {
                    vienDeFinir = true;
                }

                // dash
                if (MainWindow.TouchesActives[i, 1])
                {
                    if (vienDeFinir)
                    {
                        players[i].SetAnimation("dash");
                        vienDeFinir = false;
                    }
                }
                // coup
                if (MainWindow.TouchesActives[i, 7])
                {
                    if (vienDeFinir)
                    {
                        // Appui sur une direction
                        if (MainWindow.TouchesActives[i, 4] || MainWindow.TouchesActives[i, 2])
                        {
                            players[i].SetAnimation("couplourd");
                        }
                        else
                        {
                            players[i].SetAnimation("coupleger");
                        }
                        vienDeFinir = false;
                    }
                }
                // gauche
                if (MainWindow.TouchesActives[i, 2])
                {
                    if (vienDeFinir) {
                        // on retourne au besoin
                        if (players[i].OrientationDroite)
                            players[i].Flip();

                        players[i].SetAnimation("marche");
                        vienDeFinir=false;
                    }
                }
                // droite
                if (MainWindow.TouchesActives[i, 4])
                {
                    if (vienDeFinir)
                    {
                        // On retourne au besoin
                        if (!players[i].OrientationDroite)
                            players[i].Flip();
                        
                        players[i].SetAnimation("marche");
                        vienDeFinir = false;
                    }
                }
                // tir
                if (MainWindow.TouchesActives[i, 8])
                {
                    if (vienDeFinir)
                    {
                        players[i].SetAnimation("tirleger");
                        vienDeFinir = false;
                    }
                }
                // bouclier
                if (MainWindow.TouchesActives[i, 9])
                {
                    if (vienDeFinir)
                    {
                        // si c'était deja un bouclier, on prolonge sa durrée
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
                }
                // saisie
                if (MainWindow.TouchesActives[i, 10])
                {
                    if (vienDeFinir)
                    {
                        players[i].SetAnimation("saisie");
                        vienDeFinir = false;
                    }
                }
                // saut
                if (MainWindow.TouchesActives[i, 3])
                {
                    players[i].Jump();
                }

                // Si l'action est fini et qu'il y en as pas de nouvelle, on met celle par defaut
                if (vienDeFinir)
                {
                    players[i].SetAnimation("attente");
                }

                bool stop = false;
                // Si le joueur peux taper l'autre joueur
                if (players[(i+1)%2].AnimationCourante.GetCurrentFrame().Type != "protect"  || players[i].AnimationCourante.GetCurrentFrame().Type == "grab")
                {
                    // Pour chacune de ses hitbox
                    foreach (var hitbox in players[i].GetHitboxs())
                    {
                        if (players[(i + 1) % 2].StoneTime > 0) break; // si le joueur ennemi est sonné, on ne quite la boucle
                        // pour chacune des hitbox du joueur ennemie
                        foreach (var hearthbox in players[(i + 1) % 2].GetHearthboxs())
                        {
                            // Si il est touché
                            if (hitbox.IntersectsWith(hearthbox))
                            {
                                // Dégats
                                players[(i + 1) % 2].InfligeDegat(players[i].AnimationCourante.GetCurrentFrame().Puissance, players[i].AnimationCourante.TimeFrameRestant());
                                // Si grab, on le retourne :)
                                if (players[i].AnimationCourante.GetCurrentFrame().Type == "grab")
                                {
                                    players[(i + 1) % 2].Flip();
                                }
                                // On quite les boucles
                                stop = true; break;
                            }
                            if (stop) break;
                        }
                    }
                }
                // On affiche la barre e vie du joueur
                players[i].DrawHealthBar(canvasJeux, 300 + i * 480, 50);
            }

            // On parcour les projectils
            foreach (var projectil in ProjectilsEnJeu.ToList()) // le tolist fait qu'on ne modif pas la liste pendent le foreach
            {
                // Si après mise à jours ils ne sortent pas de l'écran
                if (projectil.Update())
                {
                    //Affiche
                    projectil.Affiche(canvasJeux, 200);
                    int i = (projectil.OwnerNumber + 1) % 2;
                    //Si il touche une hitbox d'un autre que son créateur : dégats + remove
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
                    // remove
                    ProjectilsEnJeu.Remove(projectil);
                }
            }
            // Si un des joueurs est mort, on met un petit message à l'écran
            if (players[0].EstMort() || players[1].EstMort())
            {
                jeuEncours = false;
                gameOverStartTime = stopwatch.ElapsedMilliseconds;
                string text = "";
                if (players[0].EstMort())
                {
                    text += "Player 1 KO";
                    scoreP2++;
                }
                if (players[1].EstMort())
                {
                    text += "Player 2 KO";
                    scoreP1++;
                }
                text = $"Round {scoreP1 + scoreP2}\n{scoreP1} : {scoreP2}\n" + text;
                
                // Message spécial si la partie est fini
                if (scoreP1>=2 || scoreP2 >= 2)
                {
                    if (scoreP1 >= 2)
                    {
                        text = "P1 Win !!!";
                    }
                    else
                    {
                        text = "P2 Win !!!";
                    }
                }
                blockInfo.Text = text;
                blockInfo.Visibility = Visibility.Visible;
            }
        }

        // Quand la feunetre est chargé, on lui donne le focus (une fois de plus au cas ou)
        private void UCJeux_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);
        }


        // Si une touche est appuyé, on met à jours le tableau dédié
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
        }

        // Si une touche est relaché, on met à jours le tableau dédié
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

        // Suprime les abonnements
        public void Cleanup()
        {
            CompositionTarget.Rendering -= Jeu;

            this.Loaded -= UCJeux_Loaded;
            this.KeyDown -= UCJeux_KeyDown;
            this.KeyUp -= UCJeux_KeyUp;

            stopwatch.Stop();

            ProjectilsEnJeu?.Clear();
        }

        // Remet toute les touches à false
        private void ResetTouchesActives()
        {
            for (int i = 0; i < MainWindow.TouchesActives.GetLength(0); i++)
            {
                for (int j = 0; j < MainWindow.TouchesActives.GetLength(1); j++)
                {
                    MainWindow.TouchesActives[i, j] = false;
                }
            }
        }

        private void ChargerTerrain()
        {
            imgBG.Source = new BitmapImage(
                new Uri("/img/terrain/" + MainWindow.TerrainActif, UriKind.Relative)
            );
        }
    }
}
