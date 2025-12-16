using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
        public bool IsFlip {  get; set; } = false;

        private SoundPlayer soundPlayer = null;
        private bool sonJoue = false;

        public Animation(string name)
        {
            Name = name;
            Frames = new List<Frame>();
            IsPlaying = false;
        }

        public int TimeFrameRestant() // renvoi le temps restant de la frame
        {
            return GetCurrentFrame().Duree - CurrentFrame;
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
            JouerSon(); // ne se fera que si sonjoue est false:

            if (Frames.Count <= 1)
                return false;

            CurrentFrame++;
            if (CurrentFrame == Frames[IndexFrameActuel].Duree)
            {
                IndexFrameActuel++;
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
            sonJoue = false;   // autorise le son à rejouer
        }

        // Permet de retourner TOUTE l'animation d’un coup
        public void Flip()
        {
            IsFlip = !IsFlip;
            foreach (var f in Frames)
                f.Flip();
        }

        public void AssignerSon(string path)
        {
            soundPlayer = new SoundPlayer(path);
            soundPlayer.LoadAsync(); // préchargement (optionnel mais bien)
        }

        private void JouerSon()
        {
            if (soundPlayer != null && !sonJoue)
            {
                soundPlayer.Play();
                sonJoue = true;
            }
        }

    }

}
