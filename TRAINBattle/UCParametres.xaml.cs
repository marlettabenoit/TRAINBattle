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
        public UCParametres()
        {
            InitializeComponent();
            sliderMusique.Value = MainWindow.VolumeMusique * 100;
            sliderSond.Value = MainWindow.VolumeSon * 100;

            butPlayer1.Click += ClickButPlayer;
            butPlayer2.Click += ClickButPlayer;

        }

        private void ClickButPlayer(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Content.ToString().Contains("1"))
                MainWindow.PlayerTouchesModifie = 0;
            if (btn.Content.ToString().Contains("2"))
                MainWindow.PlayerTouchesModifie = 1;
        }

        private void butReset_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.VolumeMusique = 0.5;
            MainWindow.VolumeSon = 0.5;
            sliderMusique.Value = 50;
            sliderSond.Value = 50;
        }

        private void butSauvegarde_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.VolumeMusique = sliderMusique.Value;
            MainWindow.VolumeSon = sliderSond.Value;
            Console.WriteLine(MainWindow.VolumeMusique);
            Console.WriteLine(sliderMusique.Value);

            //MainWindow.MusicPlayer.Stop();
            MainWindow.MusicPlayer.Volume = MainWindow.VolumeMusique / 100.0;
            //MainWindow.MusicPlayer.Play();
        }
    }
}
