using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TRAINBattle
{
    class Bot
    {
        // Class qui permet de gérer le bot
        // On récupérera le perso du bot et celui du joueur ainsi que la liste des projectiles dans ces variables
        private Personnage bot;
        private Personnage joueur;
        private List<Projectils> projectils;

        // On crée le génerateur une bonne fois pour toute
        private Random rng = new Random();

        // crée un énum pour stocker l'état du bot
        private enum EtatBot
        {
            Neutral,
            Offense,
            Defense,
            Punish,
            Recovery
        }

        private EtatBot etatActuel = EtatBot.Neutral; // par defaut neutral

        // constructeur de la classe, nécessite le perso attribué au bot, celui de joueur, et la list des projectils
        public Bot(Personnage bot, Personnage joueur, List<Projectils> projectils)
        {
            this.bot = bot;
            this.joueur = joueur;
            this.projectils = projectils;
        }

        // Met à jours le bot
        public void Update()
        {
            ResetInputs();
            MetAJourEtat();
            Console.WriteLine(etatActuel);
            PrendreDecision();
        }

        // 'Relache' toutes les 'touches'
        private void ResetInputs()
        {
            for (int i = 0; i < MainWindow.TouchesActives.GetLength(1); i++)
                MainWindow.TouchesActives[1, i] = false;
        }

        // permet 'd'appuyer' sur une touche
        private void Press(int index)
        {
            MainWindow.TouchesActives[1, index] = true;
        }

        // Renvoi la distance au joueur
        // On pourrait calculer ca avec les centres des élements, mais notre méthode fonctionne amplement
        private double Distance()
        {
            return Math.Abs(bot.X - joueur.X);
        }

        // Permet de savoir si le joueur est en TRAIN d'attaquer
        private bool JoueurAttaque()
        {
            return joueur.AnimationCourante.GetCurrentFrame().HitBoxs.Count > 0;
        }

        // Permet de savoir si le joueur se protége
        private bool JoueurBloque()
        {
            return joueur.AnimationCourante.GetCurrentFrame().Type == "protect";
        }

        // Définit la posture que le bot doit adopter vis à vis des actions du joueur
        // (mais sans input reading car on est pas méchants)
        private void MetAJourEtat()
        {
            // par defaut neutre
            etatActuel = EtatBot.Neutral;

            if (bot.StoneTime > 0) // on est en train de subir des degats
            {
                etatActuel = EtatBot.Recovery;
                return;
            }

            if (joueur.StoneTime > 0) // le joueur est mal, on l'enchaine
            {
                etatActuel = EtatBot.Punish;
                return;
            }


            if (Distance() < 150) // à peux près au contact
            {
                if (JoueurAttaque()) // on se met sur la deffancive
                {
                    etatActuel = EtatBot.Defense;
                    return;
                }

                etatActuel = EtatBot.Offense; // A l'attaque !!!
                return;
            }
        }
        
        // On se sert du résultat du dessus pour appeler la fonction adéquoite
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
                    ComportementRecovery();
                    break;
            }
        }

        // renvoi true si le bot est oriente vers le joueur, false sinon
        private bool OrienteVersJoueur()
        {   // pourait être plus précis, mais des observations empiriques ont montré que c'est sufisant

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

        // Appui sur la touche qui permet d'avancer dans la direction vers laquelle le bot regarde
        private void Avancer()
        {
            if (bot.OrientationDroite) Press(4); // droite
            else Press(2); // gauche
        }

        // Indique la distance qu'à le bot avec le projectil le plus proche suceptible de le toucher
        private double DistanceProjectilDangereux()
        {
            double distanceMin = double.MaxValue;

            foreach (Projectils p in this.projectils)
            {
                // ignorer les projectiles du bot
                if (p.OwnerNumber == 1)
                    continue;

                // vérifier que le projectile va vers le bot
                bool vaVersBot =
                    (p.DirX > 0 && p.X < bot.X) ||
                    (p.DirX < 0 && p.X > bot.X);

                if (!vaVersBot)
                    continue;

                // vérifier la hauteur
                if (Math.Abs(p.Y - bot.Y) > 120)
                    continue;

                // calcul distance
                double distance = Math.Abs(p.X - bot.X);

                if (distance < distanceMin)
                    distanceMin = distance;
            }

            return distanceMin;
        }


        // Comportement quand y'à rien de mieux à faire
        private void ComportementNeutral()
        {
            if (DistanceProjectilDangereux()<200)
            {
                Press(9); // bouclier
                return;
            }

            if (OrienteVersJoueur()==false)
            { // On change de direction
                if (bot.OrientationDroite) Press(2); // gauche
                else Press(4); // droite
            }
            else if (Distance() > 500) // A distance, on spam un peux ;)
            {
                if (rng.NextDouble() < 0.4)
                    Press(8); // tir léger
                else if (rng.NextDouble() < 0.09)
                    Press(1); // dash
                if (rng.NextDouble() < 0.03)
                    Press(3); // saut
            }
            else // A mis distance, on se raproche afin d'engager le combat
            {
                if (rng.NextDouble() < 0.2)
                    Avancer();
                else if (rng.NextDouble() < 0.2)
                    Press(1); // dash
                if (rng.NextDouble() < 0.05)
                    Press(3); // saut
            }
        }
        
        // On est attaque l'ennemie
        private void ComportementOffense()
        {
            if (OrienteVersJoueur() == false)
            { // On change de direction car ce serai ballo de taper dans le vide
                if (bot.OrientationDroite) Press(2); // gauche
                else Press(4); // droite
                return; // on execute pas le reste
            }
            if (JoueurBloque())
            {
                Press(10); // saisie
                return;
            }
            if (rng.NextDouble() < 0.7)
            {
                Press(7); // coup léger
                if (rng.NextDouble() < 0.3)
                {
                    Avancer(); // car avancer + attaque => attaque lourde
                }
            }
            else
                Press(1); // dash
        }

        // Quand le joueur attaque, en vrai sert à rien car le bot vas de toute facon sortir le bouclier trop tard
        // car la hitbox est deja la, mais ca sert au moins à "faire semblan" que le bot se défend
        private void ComportementDefense()
        {
            if (rng.NextDouble() < 0.6)
                Press(9); // bouclier
            if (rng.NextDouble() < 0.3)
                Press(3); // saut
        }

        // Quand on est mal
        private void ComportementRecovery()
        {
            if (rng.NextDouble() < 0.3)
                Press(3); // saut, un des meilleurs moyens de s'en sortir + la seul action à marcher dans cette situation
        }

        // Si le joueur souffre, on tape plus fort jusqu'à ce qu'il ne ressante plus la douleur...
        private void ComportementPunish()
        {
            if (Distance() < 150)
            {
                Press(7); // coup léger
                if (rng.NextDouble() < 0.3)
                {
                    Avancer(); // car avancer + attaque => attaque lourde
                }
            }
            else
                Press(1); // dash pour se rapprocher
        }
    }
}
