using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TRAINBattle
{
    // class projectile qui gére les projectiles (bah oui, c'est facile le code finalement)
    public class Projectil
    {
        // Position actuelle du projectile
        public int X { get; set; }
        public int Y { get; set; }

        // Direction de déplacement
        public double DirX { get; set; }
        public double DirY { get; set; }

        // Vitesse
        public double Speed { get; set; }

        // Puissance
        public int Damage { get; set; }

        // Image
        public Image Image { get; set; }

        // Gravité ou non
        public bool EnCloche { get; set; }
        
        // 'id' du tireur
        public int OwnerNumber { get; set; }

        // constructeur
        public Projectil(string imagePath, int x, int y, double dirX, double dirY, double speed, int damage, bool enCloche)
        {
            X = x;
            Y = y;

            DirX =  dirX;
            DirY = dirY;

            Speed = speed;
            Damage = damage;

            EnCloche = enCloche;

            // On charge l'image
            string imagePathComplet = $"pack://application:,,,/img/{imagePath}";
            Image = new Image();
            Image.Source = new BitmapImage(new Uri(imagePathComplet));
            Image.Width = ((BitmapImage)Image.Source).PixelWidth;
            Image.Height = ((BitmapImage)Image.Source).PixelHeight;
        }

        // Renvoi la hitbox du projectil
        public System.Drawing.Rectangle GetHitbox()
        {
            System.Drawing.Rectangle hitbox = new System.Drawing.Rectangle(
                X,
                Y,
                ((BitmapImage)Image.Source).PixelWidth,
                ((BitmapImage)Image.Source).PixelHeight);
            return hitbox;
        }

        // Retourne le projectil
        public void Flip()
        {
            Image.LayoutTransform = new ScaleTransform(-1, 1);
        }

        // Met à jours le projectil, renvoi false si le projectil sort de l'écran
        public bool Update()
        {
            // Déplacement
            X += (int)( DirX * Speed * 0.033 );
            if (EnCloche) {
                Y += (int)(DirY * Speed * 0.033);
                DirY -= 0.02;
            }
            // gestion sortie écran
            if (Y < 0) return false;
            if (X < 0) return false ;
            if (X + ((BitmapImage)Image.Source).PixelWidth > 1280) return false;
            return true;
        }

        // Affiche le projectil
        public void Affiche(Canvas canvas, int niveauSol)
        {
            canvas.Children.Add(Image);
            Canvas.SetLeft(Image, X);
            Canvas.SetBottom(Image, niveauSol+Y);
        }
    } 
}
