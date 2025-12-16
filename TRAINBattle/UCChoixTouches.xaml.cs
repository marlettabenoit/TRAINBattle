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
    /// Logique d'interaction pour UCChoixTouches.xaml
    /// </summary>
    public partial class UCChoixTouches : UserControl
    {
        public UCChoixTouches()
        {
            InitializeComponent();
            InitContent();
        }

        // Initialise le contenu de l'uc
        private void InitContent()
        {
            // On affiche pour chaque action le nom de l'action et la touche dedie cote à cote
            string[] indications =
            {
        "inutile", "dash", "gauche", "saut", "droite",
        "inutile", "inutile", "coupcontact", "coupdistance",
        "protection", "saisie", "inutile"
            };
            int total = MainWindow.Touches.GetLength(1);
            int moitié = total / 2;
            stackDroite.Children.Clear();
            stackGauche.Children.Clear();
            for (int i = 0; i < total; i++)
            {
                Grid grid = new Grid();
                grid.Margin = new Thickness(10);

                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                Button button = new Button();
                button.Content = MainWindow.Touches[MainWindow.PlayerTouchesModifie, i].ToString();
                button.Width = 200;
                // On ajoute au bouton sa fonction qui permet de bind la touche
                button.Click += ClickButton;

                Grid.SetColumn(button, 0);
                grid.Children.Add(button);

                Label label = new Label();
                label.Content = indications[i];
                label.FontSize = 24;
                label.VerticalAlignment = VerticalAlignment.Center;
                label.Margin = new Thickness(30, 0, 0, 0);

                Grid.SetColumn(label, 1);
                grid.Children.Add(label);

                if (i < moitié)
                    stackGauche.Children.Add(grid);
                else
                    stackDroite.Children.Add(grid);
            }
        }

        // Fonction appelé quand un des boutons est clique (sauf retour)
        // Elle se sert de ChangementTouche pour changer la touche
        private void ClickButton(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            ChangementTouche changementToucheWindow = new ChangementTouche();
            bool? reponse = changementToucheWindow.ShowDialog();
            if (reponse == true)
            {
                // On échange l'ancienne touche avec la nouvelle
                int indexInverse = -1; // index nouvelle touche
                int indexOrigine = -1; // index anciene touche
                for (int i = 0; i < MainWindow.Touches.GetLength(1); i++)
                {
                    if (MainWindow.Touches[MainWindow.PlayerTouchesModifie, i].ToString() == button.Content.ToString())
                    {
                        indexOrigine = i;
                    }
                    if (MainWindow.Touches[MainWindow.PlayerTouchesModifie, i].ToString() == changementToucheWindow.ToucheSelectione)
                    {
                        indexInverse = i;
                    }
                }
                Key tempKey = MainWindow.Touches[MainWindow.PlayerTouchesModifie, indexInverse];
                MainWindow.Touches[MainWindow.PlayerTouchesModifie, indexInverse] = MainWindow.Touches[MainWindow.PlayerTouchesModifie, indexOrigine];
                MainWindow.Touches[MainWindow.PlayerTouchesModifie, indexOrigine] = tempKey;
            }
            // On recharge les boutons pour les remetres à jours
            InitContent();
        }
    }
}
