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
    class Projectils
    {
        // Position actuelle du projectile
        public float X { get; set; }
        public float Y { get; set; }

        // Direction de déplacement (normalisée)
        public float DirX { get; set; }

        // Vitesse en unités par seconde
        public float Speed { get; set; }

        // Dégâts infligés lors d’un impact
        public int Damage { get; set; }

        // Indique si le projectile est encore actif
        public bool IsActive { get; set; } = true;

        public Image Image { get; set; }
      
        public void Projectile(float x, float y, float dirX, float speed, int damage)
        {
            X = x;
            Y = y;

            // Optionnel : normaliser la direction
            float magnitude = (float)Math.Sqrt(dirX * dirX);
            DirX = dirX / magnitude;

            Speed = speed;
            Damage = damage;

            Image = new Image();
            // !!! Changer ligne du dessous
            //Image.Source = new BitmapImage(new Uri())
        }


        // Mise à jour par frame (deltaTime = temps écoulé)
        public void Update(float deltaTime)
        {
            if (!IsActive) return;

            // Déplacement
            X += DirX * Speed * deltaTime;
        }

        // Quand le projectile touche quelque chose
        public void Hit()
        {
            IsActive = false;
        }

       // public void Affiche(Canvas canvas)
        //{
            
          //  Image = new Image()
            //{
              //  Source = bitmap,
               // Width = bitmap.PixelWidth,
              //  Height = bitmap.PixelHeight
           // };

          //  canvas.Children.Add(_sprite);
            // Mise à jour de la position dans le Canvas
          //  Canvas.SetLeft(sprite, X);
          //  Canvas.SetTop(sprite, Y);
      //  }
    } 
}
