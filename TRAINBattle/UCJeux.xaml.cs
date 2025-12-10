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
    /// Logique d'interaction pour UCJeux.xaml
    /// </summary>
    public partial class UCJeux : UserControl
    {
        public UCJeux()
        {
            InitializeComponent();
            Frame f1 = new Frame("train1/profil.png",2);
            f1.HearthBoxs.Add(new System.Drawing.Rectangle(0, 0, 200, 100));
            f1.HearthBoxs.Add(new System.Drawing.Rectangle(0, 100, 50, 50));
            //f1.Flip();
            f1.Display(canvasJeux, 0, 520);
        }
    }
}
