using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

//using TRAINBattle;

namespace TRAINBattle
{
    // Class qui gére les animations, en gros des listes de frames
    public class Animation
    {
        public string Name { get; set; }
        // Liste des frames
        public List<Frame> Frames { get; set; }
        // Frames ecoule depuis le debut de l'animation courant (nb images affiché à l'écran)
        public int CurrentFrame { get; set; } = 0;
        // Index de la frame actuel (!= nb images affichés à l'écran, certaines frames durent plusieurs frames)
        public int IndexFrameActuel { get; set; } = 0;

        public bool IsPlaying { get; set; }
        public bool IsFlip {  get; set; } = false;

        // Permet de charger et jouer des sons
        private SoundPlayer soundPlayer = null;
        // indique si le sond à déja été joué afin de ne pas le jouer pls fois en une meme animation
        private bool sonJoue = false;

        // Constructeur
        public Animation(string name)
        {
            Name = name;
            Frames = new List<Frame>();
            IsPlaying = false;
        }

        // renvoi le temps restant de la frame en cours
        public int TimeFrameRestant()
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

            if (Frames.Count <= 1) // Une animation à pls frames
                return false;

            CurrentFrame++;
            if (CurrentFrame == Frames[IndexFrameActuel].Duree)
            {
                IndexFrameActuel++;
                CurrentFrame = 0;
                if (IndexFrameActuel >= Frames.Count) // anim fini
                {
                        IsPlaying = false;
                }
            }
            return IsPlaying;
        }

        // Reset l'animation afin de pouvoir la rejouer
        public void Reset()
        {
            IndexFrameActuel = 0;
            CurrentFrame = 0;
            IsPlaying = true;
            sonJoue = false;   // autorise le son à se rejouer
        }

        // Permet de retourner toutes les frames de l'animation
        public void Flip()
        {
            IsFlip = !IsFlip;
            foreach (var f in Frames)
                f.Flip();
        }

        // Assigne un sond à la fonction (ce n'est pas nécessaire pour toutes les animations)
        public void AssignerSon(string path)
        {
            soundPlayer = new SoundPlayer(path);
            soundPlayer.LoadAsync(); // préchargement (optionnel mais bien)
        }

        // Joue le son assigné à la fonction si il y en as un
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
