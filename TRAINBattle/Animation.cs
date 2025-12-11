using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using TRAINBattle;

namespace TRAINBattle
{
    public class Animation
    {
        public string Name { get; set; }

        // Liste des frames
        public List<Frame> Frames { get; set; }

        // Frames ecoule depuis le debut de l'animation courant
        public int CurrentFrame { get; set; } = 0;

        // Index de la frame actuel
        public int IndexFrameActuel { get; set; } = 0;

        public bool IsPlaying { get; set; }

        public Animation(string name)
        {
            Name = name;
            Frames = new List<Frame>();
            IsPlaying = false;
        }

        // Ajouter un frame
        public void AddFrame(Frame f)
        {
            Frames.Add(f);
        }

        // Renvoie le frame courant
        public Frame GetCurrentFrame()
        {
            if (Frames.Count == 0) throw new ArgumentOutOfRangeException("Mais l'animation est vide");
            return Frames[IndexFrameActuel];
        }

        // Mise à jour automatique de l'animation renvoi false si anim fini
        public bool Update()
        {
            if (Frames.Count <= 1)
                return false;

            CurrentFrame++;
            if (CurrentFrame == Frames[IndexFrameActuel].Duree)
            {
                IndexFrameActuel++;
#if DEBUG
                Console.WriteLine("next frame");
#endif
                CurrentFrame = 0;
                if (IndexFrameActuel >= Frames.Count)
                {
                        IsPlaying = false;
                }

            }
            return IsPlaying;

        }

        //public void Play() => IsPlaying = true;

        public void Reset()
        {
            IndexFrameActuel = 0;
            CurrentFrame = 0;
            IsPlaying = true;
        }

        // Permet de retourner TOUTE l'animation d’un coup
        public void Flip()
        {
            foreach (var f in Frames)
                f.Flip();
        }
    }

}
