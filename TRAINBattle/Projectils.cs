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
    public class Projectils
    {
        // Position actuelle du projectile
        public int X { get; set; }
        public int Y { get; set; }

        // Direction de déplacement
        public double DirX { get; set; }
        public double DirY { get; set; }

        // Vitesse en unités par seconde
        public double Speed { get; set; }

        // Dégâts infligés lors d’un impact
        public int Damage { get; set; }

        public Image Image { get; set; }

        public bool EnCloche { get; set; }

        public Projectils(string imagePath, int x, int y, double dirX, double dirY, double speed, int damage, bool enCloche)
        {
            X = x;
            Y = y;

            DirX =  dirX;
            DirY = dirY;

            Speed = speed;
            Damage = damage;

            EnCloche = enCloche;

            string imagePathComplet = $"pack://application:,,,/img/{imagePath}";
            Image = new Image();
            Image.Source = new BitmapImage(new Uri(imagePathComplet));
            Image.Width = ((BitmapImage)Image.Source).PixelWidth;
            Image.Height = ((BitmapImage)Image.Source).PixelHeight;

        }

        public void Flip()
        {
            Image.LayoutTransform = new ScaleTransform(-1, 1);
        }

        // 
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

        public void Affiche(Canvas canvas, int niveauSol)
        {
            canvas.Children.Add(Image);
            Canvas.SetLeft(Image, X);
            Canvas.SetBottom(Image, niveauSol+Y);
        }
    } 
}
