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
        // ===== Modèle interne =====
        private class PersoChoix
        {
            public int Id;
            public string Nom;
            public string ImageProfil;
        }

        // ===== Données =====
        private List<PersoChoix> persos;
        private int choixP1 = 0;
        private int choixP2 = 1;

        public UCChoixPerso()
        {
            InitializeComponent();
            InitPersos();
        }

        private void InitPersos()
        {
            persos = new List<PersoChoix>
            {
                new PersoChoix { Id = 0, Nom = "George", ImageProfil = "/img/train1/profil.png" },
                new PersoChoix { Id = 1, Nom = "Christof", ImageProfil = "/img/train2/profil.png" },
                new PersoChoix { Id = 2, Nom = "Jack", ImageProfil = "/img/train3/profil.png" },
                new PersoChoix { Id = 3, Nom = "Walid", ImageProfil = "/img/train4/profil.png" },
            };
            MainWindow.IndexPersoP1 = choixP1;
            MainWindow.IndexPersoP2 = choixP2;
            AfficherPersoP1(persos[0]);
            AfficherPersoP2(persos[1]);
        }

        // ===== Clic sur un perso =====
        private void Perso_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image img || img.Tag == null || persos == null)
                return;

            int id = int.Parse(img.Tag.ToString());

            // Déjà pris par l'autre joueur
            if (e.ChangedButton == MouseButton.Left && choixP2 == id)
                return;

            if (e.ChangedButton == MouseButton.Right && choixP1 == id)
                return;

            PersoChoix perso = persos[id];

            // ===== Player 1 : clic gauche =====
            if (e.ChangedButton == MouseButton.Left)
            {
                choixP1 = id;
                MainWindow.IndexPersoP1 = choixP1;
                AfficherPersoP1(perso);
            }
            // ===== Player 2 : clic droit =====
            else if (e.ChangedButton == MouseButton.Right)
            {
                choixP2 = id;
                MainWindow.IndexPersoP2 = choixP2;
                AfficherPersoP2(perso);
            }
        }

        // ===== UI Joueur 1 =====
        private void AfficherPersoP1(PersoChoix perso)
        {
            imgP1.Source = new BitmapImage(new Uri(perso.ImageProfil, UriKind.Relative));
            lblNomP1.Content = perso.Nom;
        }

        // ===== UI Joueur 2 =====
        private void AfficherPersoP2(PersoChoix perso)
        {
            imgP2.Source = new BitmapImage(new Uri(perso.ImageProfil, UriKind.Relative));
            lblNomP2.Content = perso.Nom;
        }
    }
}
