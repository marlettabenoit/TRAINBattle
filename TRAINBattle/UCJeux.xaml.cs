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
        public UCJeux()
        {
            InitializeComponent();
            CompositionTarget.Rendering += Jeu;
            stopwatch.Start();
            ProjectilsEnJeu = new List<Projectils>();
            InitialisePersonages();
            players = new Personnage[2];
            players[0]=personnages[0];
            players[1]=personnages[1];
            players[0].SetAnimation("attente");
            players[1].SetAnimation("attente");
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
                personnages[i].Animations["bouclier"].Frames[3].Type = "protect";
                personnages[i].Animations["bouclier"].AddFrame(new Frame($"train{i + 1}/bouclier4.png", 1));
                personnages[i].Animations["bouclier"].Frames[4].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["bouclier"].Frames[4].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["bouclier"].Frames[4].Type = "protect";
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
                personnages[i].Animations["tirleger"].Frames[0].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["tirleger"].Frames[0].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["tirleger"].AddFrame(new Frame($"train{i + 1}/deplacement0.png", 1));
                personnages[i].Animations["tirleger"].Frames[1].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["tirleger"].Frames[1].AddHearthbox(0, 100, 100, 50);
                personnages[i].Animations["tirleger"].Frames[1].AddProjectile("train1/tir0.png", 0, 60, 1, 0, 300, 3, false);

                personnages[i].Animations["tirleger"].AddFrame(new Frame($"train{i + 1}/deplacement0.png", 7));
                personnages[i].Animations["tirleger"].Frames[2].AddHearthbox(0, 0, 180, 100);
                personnages[i].Animations["tirleger"].Frames[2].AddHearthbox(0, 100, 100, 50);


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

#if DEBUG
            Console.WriteLine(delta);
#endif
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
                    projectil.Affiche(canvasJeux, 200);
                else
                {

                    ProjectilsEnJeu.Remove(projectil);
                }
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
