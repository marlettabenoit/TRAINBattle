using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TRAINBattle
{
    public class Personnage
    {
        public int X { get; set; }   // Position du personnage sur le canvas
        public int Y { get; set; } // hauteur par rapport au sol

        public bool OrientationDroite { get; private set; } = true; // true => droite

        public Dictionary<string, Animation> Animations { get; private set; }
        public Animation AnimationCourante { get; private set; }

        // Constructeur
        public Personnage(int x, int y)
        {
            X = x;
            Y = y;
            Animations = new Dictionary<string, Animation>();
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
                AnimationCourante = Animations[nom];
                AnimationCourante.Reset();
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
        public void Update()
        {
            if (AnimationCourante is null) return;
            X += AnimationCourante.GetCurrentFrame().DeplacementX;
            AnimationCourante.Update();
            if (AnimationCourante.IsPlaying == false) AnimationCourante.Reset();
        }

        // Tourner le personnage (droite ↔ gauche)
        public void Flip()
        {
            OrientationDroite = !OrientationDroite;

            foreach (var anim in Animations.Values)
                anim.Flip();
        }
    }
}
