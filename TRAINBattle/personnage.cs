using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TRAINBattle
{
    public class Personnage
    {
        public int X { get; set; }   // Position du personnage sur le canvas
        public int Y { get; set; } // hauteur par rapport au sol

        public bool OrientationDroite { get; private set; } = true; // true => droite

        public Dictionary<string, Animation> Animations { get; private set; }
        public Animation AnimationCourante { get; private set; }
        public int Vie {  get; set; }
        public double AccelerationY { get; set; }
        public int StoneTime { get; set; } = 0;
        public bool AuSol { get; set; } = true;
        public int Number { get; set; } = -1; // sorte d'ID
        // Constructeur
        public Personnage(int x, int y)
        {
            X = x;
            Y = y;
            AccelerationY = 0; 
            Animations = new Dictionary<string, Animation>();
            Vie = 100;
        }

        public void InfligeDegat(int n, int stun)
        {
            //Console.WriteLine(stun);
            Vie -= n;
            StoneTime = stun;
            SetAnimation("attente");
        }

        public System.Drawing.Rectangle[] GetHitboxs()
        {
            System.Drawing.Rectangle[] hitboxs = new System.Drawing.Rectangle[AnimationCourante.GetCurrentFrame().HitBoxs.Count];
            for (int i = 0; i < AnimationCourante.GetCurrentFrame().HitBoxs.Count; i++)
            {
                System.Drawing.Rectangle ancienneHitbox = AnimationCourante.GetCurrentFrame().HitBoxs[i];
                hitboxs[i] = new System.Drawing.Rectangle(ancienneHitbox.X + X, ancienneHitbox.Y + Y, ancienneHitbox.Width, ancienneHitbox.Height);
            }
            return hitboxs;
        }

        public System.Drawing.Rectangle[] GetHearthboxs()
        {
            System.Drawing.Rectangle[] hearthboxs = new System.Drawing.Rectangle[AnimationCourante.GetCurrentFrame().HearthBoxs.Count];
            for (int i = 0; i < AnimationCourante.GetCurrentFrame().HearthBoxs.Count; i++)
            {
                System.Drawing.Rectangle ancienneHearthbox = AnimationCourante.GetCurrentFrame().HearthBoxs[i];
                hearthboxs[i] = new System.Drawing.Rectangle(ancienneHearthbox.X + X, ancienneHearthbox.Y + Y, ancienneHearthbox.Width, ancienneHearthbox.Height);
            }
            return hearthboxs;
        }


        // Ajouter une animation
        public void AddAnimation(string nom, Animation anim)
        {
            if (!Animations.ContainsKey(nom))
                Animations.Add(nom, anim);
        }

        // Changer l'animation active
        public void SetAnimation(string nom)
        {
            if (Animations.ContainsKey(nom))
            {
                if (!(Animations[nom] == AnimationCourante) || !AnimationCourante.IsPlaying)
                {

                    AnimationCourante = Animations[nom];
                    AnimationCourante.Reset();
                }
            }
        }

        // Afficher le personnage
        public void Display(Canvas canvas, int solHauteur)
        {
            if (AnimationCourante == null)
                return;

            Frame frame = AnimationCourante.GetCurrentFrame();
            frame.Display(canvas, X, solHauteur-Y);
        }

        // Mise à jour (pour faire avancer l'animation)
        // renvoi false si animation fini
        public bool Update(List<Projectils> ProjectilsEnJeu)
        {
            if (StoneTime > 0) { StoneTime --; return true; }

            if (AnimationCourante is null) return false;
            X += AnimationCourante.GetCurrentFrame().DeplacementX;
            AnimationCourante.Update();
            if (AnimationCourante.IsPlaying)
            {
                X = Math.Min(Math.Max(0, X), 1280 - ((BitmapImage)AnimationCourante.GetCurrentFrame().Image.Source).PixelWidth);
                if (AnimationCourante.GetCurrentFrame().Type == "tir")
                {
                    Projectils projectil = AnimationCourante.GetCurrentFrame().GetProjectil();
                    projectil.X += X;
                    projectil.Y += Y;
                    projectil.OwnerNumber = Number;
                    if (OrientationDroite)
                    {
                        projectil.X += ((BitmapImage)AnimationCourante.GetCurrentFrame().Image.Source).PixelWidth
                            -50; // Valeur arbitraire car les trains font rarement toute l'image
                    }
                    else
                    {
                        projectil.Flip();
                        projectil.DirX *= -1;
                        projectil.X += 50 - ((BitmapImage)projectil.Image.Source).PixelWidth; // meme raison
                    }
                    ProjectilsEnJeu.Add(projectil);

                }
            }

            //for (int i = 0; i < 2; i++)
            //{
            //    if (MainWindow.TouchesActives[i, 3])
            //    {

            //    }

            //}

            //Console.WriteLine($"y{Y}accy{AccelerationY}");
            Y += (int)AccelerationY;
            if (!AuSol)
            {
                AccelerationY -= 2;
                if (Y <= 0)
                {
                    AuSol = true;
                    Y=0;
                    AccelerationY=0;
                }
            }
            else
            {
            }

            return AnimationCourante.IsPlaying;
        }

        public void Jump()
        {
            if ( AuSol)
            {
                AccelerationY = 25;
                AuSol = false;

            }
        }

        // Tourner le personnage (droite ↔ gauche)
        public void Flip()
        {
            OrientationDroite = !OrientationDroite;

            foreach (var anim in Animations.Values)
                anim.Flip();
        }

        public void DrawHealthBar(Canvas canvas, int x, int y)
        {
            double width = 100;     // largeur de la barre
            double height = 20;     // hauteur de la barre
            double ratio = Math.Max(0, Vie) / 100.0;

            // Barre vide (rouge)
            var back = new System.Windows.Shapes.Rectangle
            {
                Width = width,
                Height = height,
                Fill = System.Windows.Media.Brushes.DarkRed
            };

            // Barre pleine (vert)
            var front = new System.Windows.Shapes.Rectangle
            {
                Width = width * ratio,
                Height = height,
                Fill = System.Windows.Media.Brushes.LimeGreen
            };

            // Positionnement dans le canvas
            Canvas.SetLeft(back, x);
            Canvas.SetTop(back, y);

            Canvas.SetLeft(front, x);
            Canvas.SetTop(front, y);

            canvas.Children.Add(back);
            canvas.Children.Add(front);
        }

    }
}
