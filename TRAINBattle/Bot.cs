using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRAINBattle
{
    class Bot
    {
        private Personnage bot;
        private Personnage joueur;

        private Random rng = new Random();

        private enum EtatBot
        {
            Neutral,
            Offense,
            Defense,
            Punish,
            Recovery
        }

        private EtatBot etatActuel = EtatBot.Neutral;

        public Bot(Personnage bot, Personnage joueur)
        {
            this.bot = bot;
            this.joueur = joueur;
        }

        public void Update()
        {
            ResetInputs();

            //if (!PeutAgir())
            //    return;

            MetAJourEtat();
            Console.WriteLine(etatActuel);
            PrendreDecision();
        }

        // méthodes internes ↓↓↓
        private void ResetInputs()
        {
            for (int i = 0; i < MainWindow.TouchesActives.GetLength(1); i++)
                MainWindow.TouchesActives[1, i] = false;
        }

        private void Press(int index)
        {
            MainWindow.TouchesActives[1, index] = true;
        }
        //private bool PeutAgir()
        //{
        //    if (bot.StoneTime > 0)
        //        return false;

        //    // attendre la fin d'une animation
        //    return bot.AnimationCourante.IsPlaying == false;
        //}
        private double Distance()
        {
            return Math.Abs(bot.X - joueur.X);
        }

        private bool JoueurAttaque()
        {
            return joueur.AnimationCourante.GetCurrentFrame().HitBoxs.Count > 0;
        }

        private bool JoueurBloque()
        {
            return joueur.AnimationCourante.GetCurrentFrame().Type == "protect";
        }
        private void MetAJourEtat()
        {
            etatActuel = EtatBot.Neutral;

            if (bot.StoneTime > 0)
            {
                etatActuel = EtatBot.Recovery;
                return;
            }

            if (joueur.StoneTime > 0)
            {
                etatActuel = EtatBot.Punish;
                return;
            }

            if (JoueurAttaque())
            {
                etatActuel = EtatBot.Defense;
                return;
            }

            if (Distance() < 150)
            {
                etatActuel = EtatBot.Offense;
                return;
            }

        }
        private void PrendreDecision()
        {
            switch (etatActuel)
            {
                case EtatBot.Neutral:
                    ComportementNeutral();
                    break;

                case EtatBot.Offense:
                    ComportementOffense();
                    break;

                case EtatBot.Defense:
                    ComportementDefense();
                    break;

                case EtatBot.Punish:
                    ComportementPunish();
                    break;

                case EtatBot.Recovery:
                    // ne rien faire
                    break;
            }
        }

        private bool OrienteVersJoueur()
        {
            // bot à gauche du joueur
            if (bot.X < joueur.X && !bot.OrientationDroite)
            {
                return false;
            }
            // bot à droite du joueur
            else if (bot.X > joueur.X && bot.OrientationDroite)
            {
                return false;
            }
            return true;
        }

        private void Avancer()
        {
            if (bot.OrientationDroite) Press(4); // droite
            else Press(2); // gauche
        }

        private void ComportementNeutral()
        {
            if (OrienteVersJoueur()==false)
            { // On change de direction
                if (bot.OrientationDroite) Press(2); // gauche
                else Press(4); // droite

            }
            else if (Distance() > 500)
            {
                if (rng.NextDouble() < 0.3)
                    Press(8); // tir léger
            }
            else
            {
                if (rng.NextDouble() < 0.2)
                    Avancer();
                else if (rng.NextDouble() < 0.2)
                    Press(1); // dash
                if (rng.NextDouble() < 0.05)
                    Press(3); // saut
            }
        }
        private void ComportementOffense()
        {
            if (JoueurBloque())
            {
                Press(10); // saisie
                return;
            }

            if (rng.NextDouble() < 0.7)
                Press(7); // coup léger
            else
                Press(1); // dash
        }
        private void ComportementDefense()
        {
            double choix = rng.NextDouble();

            if (choix < 0.6)
                Press(9); // bouclier
            else if (choix < 0.8)
                Press(3); // saut
            else
                Press(1); // dash arrière
        }
        private void ComportementPunish()
        {
            if (Distance() < 150)
                Press(7); // coup léger
            else
                Press(1); // dash pour se rapprocher
        }


    }
}
