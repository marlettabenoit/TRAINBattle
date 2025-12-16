using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TRAINBattle
{
    // Class qui gére les personnages
    public class Personnage
    {
        public int X { get; set; }
        public int Y { get; set; } // y est le heuteur du bas du perso par rapport au sol
        public bool OrientationDroite { get; private set; } = true; // true => droite
        private Image healthBarOverlay;
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

            healthBarOverlay = new Image
            {
                Source = new BitmapImage(
                    new Uri("pack://application:,,,/img/vie/barreviemodifie.png")
                ),
                Stretch = Stretch.Fill,  // ces      evitent   problémes
                IsHitTestVisible = false //    lignes       des         !!!
            };
        }

        // Renvoi true si le perso est mort
        public bool EstMort()
        {
            return (Vie<=0);
        }

        // Inflige n degats au perso et l'empéche d'agir pour n frames
        public void InfligeDegat(int n, int stun)
        {
            //Console.WriteLine(stun);
            Vie -= n;
            StoneTime = stun;
            SetAnimation("attente");
        }

        // Renvoi un tableau contenant les hitbox de la frame courrantd de l'annimation courrante
        public System.Drawing.Rectangle[] GetHitboxs()
        {
            System.Drawing.Rectangle[] hitboxs = new System.Drawing.Rectangle[AnimationCourante.GetCurrentFrame().HitBoxs.Count];
            for (int i = 0; i < AnimationCourante.GetCurrentFrame().HitBoxs.Count; i++)
            {
                System.Drawing.Rectangle ancienneHitbox = AnimationCourante.GetCurrentFrame().HitBoxs[i];
                // On pense à rajouter la pos x et y du perso
                hitboxs[i] = new System.Drawing.Rectangle(ancienneHitbox.X + X, ancienneHitbox.Y + Y, ancienneHitbox.Width, ancienneHitbox.Height);
            }
            return hitboxs;
        }

        // Pareil que la méthode du dessus
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
                // Si on change d'animation ou que l'animation est fini => evite de relancer une meme animation deja lancé
                if (!(Animations[nom] == AnimationCourante) || !AnimationCourante.IsPlaying)
                {
                    AnimationCourante = Animations[nom];
                    AnimationCourante.Reset();
                }
            }
        }

        // Affiche le personnage
        public void Display(Canvas canvas, int solHauteur)
        {
            if (AnimationCourante == null)
                return;

            Frame frame = AnimationCourante.GetCurrentFrame();
            frame.Display(canvas, X, solHauteur-Y);
        }

        // Met la barre dans le sens classique si x=1 et inverse si x=-1
        public void FlipHealthBar(int x)
        {
            healthBarOverlay.LayoutTransform = new ScaleTransform(x, 1);
        }

        // Met à jours le joueur et renvoi false si son animation est fini
        public bool Update(List<Projectils> ProjectilsEnJeu)
        {
            // gravité
            Y += (int)AccelerationY;
            if (!AuSol)
            {
                AccelerationY -= 2;
                if (Y <= 0)
                {
                    AuSol = true;
                    Y = 0;
                    AccelerationY = 0;
                }
            }

            // On peux rien faire
            if (StoneTime > 0) { StoneTime --; return true; }

            if (AnimationCourante is null) return false; // pas d'anim donc on s'arette

            // On se déplace si l'animation le demmande
            X += AnimationCourante.GetCurrentFrame().DeplacementX;
            // Evite les sortis de terrain
            X = Math.Min(Math.Max(0, X), 1280 - ((BitmapImage)AnimationCourante.GetCurrentFrame().Image.Source).PixelWidth);

            AnimationCourante.Update(); // On met à jours l'animation
            if (AnimationCourante.IsPlaying)
            {
                // Cas ou y'à un tir
                if (AnimationCourante.GetCurrentFrame().Type == "tir")
                {
                    Projectils projectil = AnimationCourante.GetCurrentFrame().GetProjectil();
                    // On ajoute la position du perso
                    projectil.X += X;
                    projectil.Y += Y;
                    // On lui passe l'id du perso
                    projectil.OwnerNumber = Number;
                    // Au besoin on le décale et au besoin on le flip 
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
                    // Enfin on l'ajoute
                    ProjectilsEnJeu.Add(projectil);
                }
            }
            return AnimationCourante.IsPlaying;
        }

        // Déclanche un saut
        public void Jump()
        {
            if (AuSol) // On saute pas depuis le ciel
            {
                AccelerationY = 25;
                AuSol = false;
            }
        }

        // Retourne le personnage
        public void Flip()
        {
            OrientationDroite = !OrientationDroite;

            foreach (var anim in Animations.Values)
                anim.Flip();
        }

        // Affiche la barre de vie à la pos (x,y)
        public void DrawHealthBar(Canvas canvas, int x, int y)
        {
            double width = 100;
            double height = 20;
            // % de la barre pleine
            double ratio = Math.Max(0, Vie) / 100.0;

            // barre de fond (vide)
            var back = new System.Windows.Shapes.Rectangle
            {
                Width = width,
                Height = height,
                Fill = System.Windows.Media.Brushes.Black
            };

            // Barre de devant (verte)
            var front = new System.Windows.Shapes.Rectangle
            {
                Width = width * ratio,
                Height = height,
                Fill = System.Windows.Media.Brushes.LimeGreen
            };

            // Les positiones de maniére à ce qu'elles soient 'dans' l'image de contour
            Canvas.SetLeft(back, x + 47);
            Canvas.SetTop(back, y + 31);
            Canvas.SetLeft(front, x + 47);
            Canvas.SetTop(front, y + 31);

            canvas.Children.Add(back);
            canvas.Children.Add(front);

            // positionne affiche l'image de contour de la barre
            Canvas.SetLeft(healthBarOverlay, x);
            Canvas.SetTop(healthBarOverlay, y);

            // que si besoin, mais dans les faits, y'à tjrs besoin
            if (!canvas.Children.Contains(healthBarOverlay))
                canvas.Children.Add(healthBarOverlay);
        }

        // Ajoute une hearthbox à chaque frame de chaque animation
        // Utile pour la partie du train qui bouge pas
        public void AddHearthtboxToAllAnimation(System.Drawing.Rectangle[] hearthboxs)
        {
            foreach (Animation animation in Animations.Values)
            {
                foreach (Frame frame in animation.Frames)
                {
                    foreach (System.Drawing.Rectangle hearthbox in hearthboxs)
                    {
                        frame.AddHearthbox(hearthbox.X, hearthbox.Y, hearthbox.Width, hearthbox.Height);
                    }
                }
            }
        }
    }
}
