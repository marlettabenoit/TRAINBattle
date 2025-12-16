using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TRAINBattle
{
    /// <summary>
    /// Logique d'interaction pour UCChoixPerso.xaml
    /// </summary>
    public partial class UCChoixPerso : UserControl
    {
        // Class si petite qu'en C se serai une structure
        // Pour une class de cette taille, on ne la fait pas dans un fichier à part
        private class PersoChoix
        {
            public int Id;
            public string Nom;
            public string ImageProfil;
        }

        // On déclare les variables utiles
        private List<PersoChoix> persos;
        private int choixP1 = 0;
        private int choixP2 = 1;

        // Constructeur de l'UCChoixPerso
        public UCChoixPerso()
        {
            InitializeComponent();
            InitPersos();
        }

        // Crée initialise la list de choix de personnages + met les choix par deffaut aux deux joueurs
        private void InitPersos()
        {
            persos = new List<PersoChoix>
            {
                new PersoChoix { Id = 0, Nom = "Zach", ImageProfil = "/img/train1/profil.png" },
                new PersoChoix { Id = 1, Nom = "ミク", ImageProfil = "/img/train2/profil.png" },
                new PersoChoix { Id = 2, Nom = "Série 4", ImageProfil = "/img/train3/profil.png" },
                new PersoChoix { Id = 3, Nom = "Abdelaziz", ImageProfil = "/img/train4/profil.png" },
            };
            MainWindow.IndexPersoP1 = choixP1;
            MainWindow.IndexPersoP2 = choixP2;
            AfficherPersoP1(persos[0]);
            AfficherPersoP2(persos[1]);
        }

        // Appele quand une image de perso est clique (click droit comme gauche)
        private void Perso_Click(object sender, MouseButtonEventArgs e)
        {
            // si l'appel n'est pas fait depuis une image ou qu'elle n'as pas de tag ou que la liste est pas initialisé, on ne fait rien
            if (sender is not Image img || img.Tag == null || persos == null)
                return;

            // On récupére l'id de l'image à partir du tag
            int id = int.Parse(img.Tag.ToString());

            // Si le perso est deja pris, on s'arette là
            if (e.ChangedButton == MouseButton.Left && choixP2 == id)
                return;

            if (e.ChangedButton == MouseButton.Right && choixP1 == id)
                return;

            PersoChoix perso = persos[id];

            // Si c'est une clic gauche, on passe le perso au player 1
            if (e.ChangedButton == MouseButton.Left)
            {
                choixP1 = id;
                MainWindow.IndexPersoP1 = choixP1;
                AfficherPersoP1(perso);
            }
            // sinon on le passe au player 2
            else if (e.ChangedButton == MouseButton.Right)
            {
                choixP2 = id;
                MainWindow.IndexPersoP2 = choixP2;
                AfficherPersoP2(perso);
            }
        }

        // Met à jouer l'ui du joueur 1
        private void AfficherPersoP1(PersoChoix perso)
        {
            imgP1.Source = new BitmapImage(new Uri(perso.ImageProfil, UriKind.Relative));
            lblNomP1.Content = perso.Nom;
        }

        // Met à jouer l'ui du joueur 2
        private void AfficherPersoP2(PersoChoix perso)
        {
            imgP2.Source = new BitmapImage(new Uri(perso.ImageProfil, UriKind.Relative));
            lblNomP2.Content = perso.Nom;
        }
    }
}
