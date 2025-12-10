using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAINBattle
{
    public class Animation
    {
        //public string Name { get; private set; }

        //// Liste des frames
        //private List<Frame> Frames = new List<Frame>();

        //// Index du frame courant
        //private int currentIndex = 0;

        //// Durée de chaque frame en millisecondes
        //public int FrameDuration { get; set; } = 100;

        //// Animation en boucle ?
        //public bool Loop { get; set; } = true;

        //// Animation jouée ?
        //public bool IsPlaying { get; private set; } = true;

        //// Temps accumulé (pour avancer automatiquement)
        //private double timer = 0;

        //public Animation(string name, int frameDuration = 100, bool loop = true)
        //{
        //    Name = name;
        //    FrameDuration = frameDuration;
        //    Loop = loop;
        //}

        //// Ajouter un frame
        //public void AddFrame(Frame f)
        //{
        //    Frames.Add(f);
        //}

        //// Renvoie le frame courant
        //public Frame GetCurrentFrame()
        //{
        //    if (Frames.Count == 0) return null;
        //    return Frames[currentIndex];
        //}

        //// Mise à jour automatique de l'animation (dt = deltaTime en ms)
        //public void Update(double dt)
        //{
        //    if (!IsPlaying || Frames.Count <= 1)
        //        return;

        //    timer += dt;

        //    if (timer >= FrameDuration)
        //    {
        //        timer -= FrameDuration;
        //        currentIndex++;

        //        if (currentIndex >= Frames.Count)
        //        {
        //            if (Loop)
        //                currentIndex = 0;
        //            else
        //            {
        //                currentIndex = Frames.Count - 1;
        //                IsPlaying = false;
        //            }
        //        }
        //    }
        //}

        //public void Play() => IsPlaying = true;
        //public void Pause() => IsPlaying = false;

        //public void Reset()
        //{
        //    currentIndex = 0;
        //    timer = 0;
        //    IsPlaying = true;
        //}

        //// Permet de retourner TOUTE l'animation d’un coup
        //public void FlipAllFrames()
        //{
        //    foreach (var f in Frames)
        //        f.Flip();
        //}
    }

}
