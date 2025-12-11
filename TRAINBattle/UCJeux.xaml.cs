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
        private static List<Projectils> projectils;
        //private bool personnagesInitialise = false;
        private static Personnage[] personnages;
        private static Personnage[] players;
        
        public UCJeux()
        {
            InitializeComponent();
            CompositionTarget.Rendering += Jeu;
            stopwatch.Start();
            InitialisePersonages();
            players = new Personnage[2];
            players[0]=personnages[0];
            players[1]=personnages[1];
            ////InitializeTimer();
            ////Tests de la fonction frame
            //Frame f1 = new Frame("train1/deplacement0.png", 2, 0, 10);
            ////f1.HearthBoxs.Add(new System.Drawing.Rectangle(0, 0, 200, 100));
            ////f1.HearthBoxs.Add(new System.Drawing.Rectangle(0, 100, 50, 50));
            //Frame f2 = new Frame("train1/deplacement1.png", 2, 0, 10);
            ////f2.HearthBoxs.Add(new System.Drawing.Rectangle(0, 0, 200, 100));
            ////f2.HearthBoxs.Add(new System.Drawing.Rectangle(0, 100, 70, 70));
            ////f2.Flip();
            ////f1.Display(canvasJeux, 0, 520);
            ////Test de la fonction animation
            //Animation a1 = new Animation("marche");
            //a1.AddFrame(f1);
            //a1.AddFrame(f2);
            ////a1.Reset();

            //Frame f3 = new Frame("train1/deplacement0.png", 2, 0, 0);
            //Frame f4 = new Frame("train1/deplacement1.png", 2, 0, 0);
            //Animation a2 = new Animation("attente");
            //a2.AddFrame(f3);
            //a2.AddFrame(f4);
            ////a1.Reset();

            //p1 = new Personnage(0, 0);
            //p1.AddAnimation("marche", a1);
            //p1.AddAnimation("attente", a2);
            //p1.SetAnimation("attente");

            projectils = new List<Projectils>();

            Projectils pr1 = new Projectils("train1/profil.png", 200, 50, 1, 0.1, 1000, 12, true);
            projectils.Add(pr1);
            //Projectils pr2 = new Projectils("train1/profil.png", 200, 0, 1, 0, 1000, 12, false);
            //projectils.Add(pr2);

            this.Loaded += UCJeux_Loaded;
            this.KeyDown += UCJeux_KeyDown;
            this.KeyUp += UCJeux_KeyUp;
            this.Focusable = true;

        }

        private void InitialisePersonages()
        {
            personnages = new Personnage[4] {
            new Personnage(0, 0),new Personnage(0, 0),new Personnage(0, 0),new Personnage(0, 0)
            };
            //for (int i = 0; i < personnages.Length; i++)
            for (int i = 0; i < 2; i++)
            {
                personnages[i].AddAnimation("attente", new Animation("attente"));
                personnages[i].Animations["attente"].AddFrame(new Frame($"train{i + 1}/deplacement0.png", 2));
                personnages[i].Animations["attente"].Frames[0].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["attente"].Frames[0].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["attente"].AddFrame(new Frame($"train{i + 1}/deplacement1.png", 2));
                personnages[i].Animations["attente"].Frames[1].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["attente"].Frames[1].AddHearthbox(0, 100, 100, 50);
                personnages[i].AddAnimation("marche", new Animation("marche"));
                personnages[i].Animations["marche"].AddFrame(new Frame($"train{i + 1}/deplacement0.png", 2, 0, 10));
                personnages[i].Animations["marche"].Frames[0].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["marche"].Frames[0].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["marche"].AddFrame(new Frame($"train{i + 1}/deplacement1.png", 2, 0, 10));
                personnages[i].Animations["marche"].Frames[1].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["marche"].Frames[1].AddHearthbox(0, 100, 100, 50);
                personnages[i].AddAnimation("coupleger", new Animation("marche"));
                personnages[i].Animations["coupleger"].AddFrame(new Frame($"train{i + 1}/deplacement0.png", 5));
                personnages[i].Animations["coupleger"].Frames[0].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["coupleger"].Frames[0].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["coupleger"].AddFrame(new Frame($"train{i + 1}/deplacement1.png", 4, 5, 0));
                personnages[i].Animations["coupleger"].Frames[1].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["coupleger"].Frames[1].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["coupleger"].Frames[1].AddHitbox(180, 50, 20, 50);
                //personnages[i].AddAnimation("attente", new Animation("attente"));
                //personnages[i].Animations["attente"].AddFrame(new Frame($"train{1}/deplacement0.png", 2));
                //personnages[i].Animations["attente"].AddFrame(new Frame($"train{1}/deplacement1.png", 2));
                //personnages[i].AddAnimation("marche", new Animation("marche"));
                //personnages[i].Animations["marche"].AddFrame(new Frame($"train{1}/deplacement0.png", 2, 0, 10));
                //personnages[i].Animations["marche"].AddFrame(new Frame($"train{1}/deplacement1.png", 2, 0, 10));
                
            }
        }

        //private void InitializeTimer()
        //{
        //    minuterie = new DispatcherTimer();
        //    // configure l'intervalle du Timer
        //    minuterie.Interval = TimeSpan.FromMilliseconds(33); // 30 fps
        //    // associe l’appel de la méthode Jeu à la fin de la minuterie
        //    minuterie.Tick += Jeu;
        //    // lancement du timer
        //    minuterie.Start();
        //}

        private void Jeu(object? sender, EventArgs e)
        {
            long now = stopwatch.ElapsedMilliseconds;
            long delta = now - lastTick;

            // Si pas assez de temps écoulé → ignorer cette frame
            if (delta < 1000.0 / 30.0)
                return;

            lastTick = now;
            //double dt = delta / 1000.0; // en secondes

            // remet le focus sur le jeu
            this.Focus();
            Keyboard.Focus(this);
            canvasJeux.Children.Clear();

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
                if (!players[i].Update())
                    players[i].SetAnimation("attente");

                //for (int i = 0; i < 2; i++)
                //{
                //}

                if (MainWindow.TouchesActives[i, 2])
                {
                    if (players[i].OrientationDroite)
                        players[i].Flip();

                    players[i].SetAnimation("marche");
                }
                if (MainWindow.TouchesActives[i, 4])
                {
                    if (!players[i].OrientationDroite)
                        players[i].Flip();
                    players[i].SetAnimation("marche");
                }
                if (MainWindow.TouchesActives[i, 7])
                {
                    players[i].SetAnimation("coupleger");
                }

                bool stop = false;
                foreach (var hitbox in players[i].GetHitboxs())
                {
                    foreach (var hearthbox in players[(i+1)%2].GetHearthboxs())
                    {
                        if (hitbox.IntersectsWith(hearthbox))
                        {
                            Console.WriteLine("Hit");
                            players[(i + 1) % 2].InfligeDegat(players[i].AnimationCourante.GetCurrentFrame().Puissance);
                            stop = true; break;
                        }
                        if (stop) break;
                    }
                }

                players[i].DrawHealthBar(canvasJeux, 300 + i * 480, 50);
                //players[i].InfligeDegat(1);

            }

            foreach (var projectil in projectils)
            {
                if (projectil.Update())
                    projectil.Affiche(canvasJeux, 200);
                // faudrai les suprimer à l'avenir    
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
