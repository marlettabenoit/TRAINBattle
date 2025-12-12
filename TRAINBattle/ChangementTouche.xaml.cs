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
using System.Windows.Shapes;

namespace TRAINBattle
{
    /// <summary>
    /// Logique d'interaction pour ChangementTouche.xaml
    /// </summary>
    public partial class ChangementTouche : Window
    {
        public string ToucheSelectione { get; set; }

        public ChangementTouche()
        {
            InitializeComponent();
            for (int i = 0; i < MainWindow.Touches.GetLength(1); i++) {

                Button button = new Button();
                button.Content = MainWindow.Touches[MainWindow.PlayerTouchesModifie, i].ToString();
                button.Margin = new Thickness(10);
                button.Click += AppuiSurTouche;
                stackTouches.Children.Add(button);
            }
        }

        private void AppuiSurTouche(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ToucheSelectione = btn.Content.ToString();
            this.DialogResult = true;
        }
    }
}
