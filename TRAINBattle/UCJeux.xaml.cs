using System;
using System.Collections.Generic;
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
        private static DispatcherTimer minuterie;
        private static Personnage p1;
        
        public UCJeux()
        {
            InitializeComponent();
            InitializeTimer();
            //Tests de la fonction frame
            Frame f1 = new Frame("train1/profil.png", 2);
            f1.HearthBoxs.Add(new System.Drawing.Rectangle(0, 0, 200, 100));
            f1.HearthBoxs.Add(new System.Drawing.Rectangle(0, 100, 50, 50));
            Frame f2 = new Frame("train1/profil.png", 2);
            f2.HearthBoxs.Add(new System.Drawing.Rectangle(0, 0, 200, 100));
            f2.HearthBoxs.Add(new System.Drawing.Rectangle(0, 100, 70, 70));
            //f2.Flip();
            //f1.Display(canvasJeux, 0, 520);
            //Test de la fonction animation
            Animation a1 = new Animation("test");
            a1.AddFrame(f1);
            a1.AddFrame(f2);
            a1.Reset();
            p1 = new Personnage(0, 0);
            p1.AddAnimation("marche", a1);
            p1.SetAnimation("marche");

            this.Loaded += UCJeux_Loaded;
            this.KeyDown += UCJeux_KeyDown;
            this.Focusable = true;

        }

        private void InitializeTimer()
        {
            minuterie = new DispatcherTimer();
            // configure l'intervalle du Timer
            minuterie.Interval = TimeSpan.FromMilliseconds(33); // 30 fps
            // associe l’appel de la méthode Jeu à la fin de la minuterie
            minuterie.Tick += Jeu;
            // lancement du timer
            minuterie.Start();
        }

        private void Jeu(object? sender, EventArgs e)
        {
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
            p1.Display(canvasJeux, 520);
            p1.Update();

        }

        private void UCJeux_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);
        }


        private void UCJeux_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("WHOOOOOOOOOOO");

            if (e.Key == Key.Space)
            {
                p1.Flip();
                Console.WriteLine("WHAAAAAAAAAAAA");
            }
        }

    }
}
