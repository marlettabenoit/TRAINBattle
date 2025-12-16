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
    /// Logique d'interaction pour UCParametres.xaml
    /// </summary>
    
    public partial class UCParametres : UserControl
    {
        // la partie pour les sonds à été laissé pour de futurs maj et n'est pas encore utilisé
        public UCParametres()
        {
            InitializeComponent();
            sliderMusique.Value = MainWindow.VolumeMusique * 100;
            sliderSond.Value = MainWindow.VolumeSon * 100;

            butPlayer1.Click += ClickButPlayer;
            butPlayer2.Click += ClickButPlayer;

        }

        // Permet de rediriger vers les options de touches du bon joueur
        private void ClickButPlayer(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Content.ToString().Contains("1"))
                MainWindow.PlayerTouchesModifie = 0;
            if (btn.Content.ToString().Contains("2"))
                MainWindow.PlayerTouchesModifie = 1;
        }

        // Reset les parametres (mais pas les touches)
        private void butReset_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.VolumeMusique = 0.5;
            MainWindow.VolumeSon = 0.5;
            sliderMusique.Value = 50;
            sliderSond.Value = 50;
        }

        // Sauvegarde les choix faits
        private void butSauvegarde_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.VolumeMusique = sliderMusique.Value;
            MainWindow.VolumeSon = sliderSond.Value;
            MainWindow.MusicPlayer.Volume = MainWindow.VolumeMusique / 100.0;
        }
    }
}
