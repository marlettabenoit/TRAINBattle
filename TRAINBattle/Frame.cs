using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TRAINBattle
{
    public class Frame
    {
        public List<System.Drawing.Rectangle> HearthBoxs { get; set; }
        public List<System.Drawing.Rectangle> HitBoxs { get; set; }
        public Image Image { get; set; }

        // Champ puissance avec encapsulation
        private int puissance;
        public int Puissance
        {
            get { return this.puissance; }
            set {
                if (value < 0 ) throw new ArgumentOutOfRangeException("Puissance < 0");
                this.puissance = value;
            }
        }
        private int duree;
        public int Duree
        {
            get { return this.duree; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("Duree <= 0");
                this.duree = value;
            }
        }

        // constructeur
        public Frame(string imagePath, int duree, int puissance)
        {
            // initialisation des listes
            HearthBoxs = new List<System.Drawing.Rectangle>();
            HitBoxs = new List<System.Drawing.Rectangle>();
            // Charger l'image
            string imagePathComplet = $"pack://application:,,,/img/{imagePath}";
            Image = new Image();
            Image.Source = new BitmapImage(new Uri(imagePathComplet));
            // Eviter le redimentionement automatique
            Image.Stretch = Stretch.None;
            //Image.UseLayoutRounding = false;
            //Image.SnapsToDevicePixels = true;
            Image.Width = ((BitmapImage)Image.Source).PixelWidth;
            Image.Height = ((BitmapImage)Image.Source).PixelHeight;
#if DEBUG
            Console.WriteLine(Image.Height);
            Console.WriteLine(Image.Width);
#endif
            // Autres parametres
            Duree = duree;
            Puissance = puissance;
        }
        public Frame(string imagePath, int duree)
            : this(imagePath, duree, 0)
        {
        }

        public void AddHitbox(int x, int y, int width, int height)
        {
            HitBoxs.Add(new System.Drawing.Rectangle(x, y, width, height));
        }

        public void AddHearthbox(int x, int y, int width, int height)
        {
            HearthBoxs.Add(new System.Drawing.Rectangle(x, y, width, height));
        }

        public void Display(Canvas canvas, int posX, int posY)
        {
            if (!canvas.Children.Contains(Image))
                canvas.Children.Add(Image);

            // Récupérer la hauteur réelle de l'image
            double imgHeight = Image.Source.Height;
            double imgWidth = Image.Source.Width;

            double topImage = posY - imgHeight;

            Canvas.SetLeft(Image, posX);
            Canvas.SetTop(Image, topImage);

            // Debug rectangles
            foreach (var rect in HitBoxs)
            {
                var box = CreateDebugRect(rect, System.Windows.Media.Brushes.Red);
                canvas.Children.Add(box);

                double top = topImage + rect.Y;  // rect.Y = distance depuis le bas
                top = posY - (rect.Y + rect.Height); // ❗ Correction bas → top

                Canvas.SetLeft(box, posX + rect.X);
                Canvas.SetTop(box, top);
            }

            foreach (var rect in HearthBoxs)
            {
                var box = CreateDebugRect(rect, System.Windows.Media.Brushes.Green);
                canvas.Children.Add(box);

                double top = posY - (rect.Y + rect.Height);

                Canvas.SetLeft(box, posX + rect.X);
                Canvas.SetTop(box, top);
            }
        }
        private System.Windows.Shapes.Rectangle CreateDebugRect(System.Drawing.Rectangle r, Brush color)
        {
            return new System.Windows.Shapes.Rectangle
            {
                Width = r.Width,
                Height = r.Height,
                Stroke = color,
                StrokeThickness = 2,
                Fill = null
            };
        }


        public void Flip(int imageWidth)
        {
            // Retourner visuellement l'image (miroir horizontal)
            Image.LayoutTransform = new ScaleTransform(-1, 1);

            // Inversion des rectangles
            HitBoxs = FlipRectList(HitBoxs, imageWidth);
            HearthBoxs = FlipRectList(HearthBoxs, imageWidth);
        }

        private List<System.Drawing.Rectangle> FlipRectList(List<System.Drawing.Rectangle> list, int imageWidth)
        {
            var flipped = new List<System.Drawing.Rectangle>();

            foreach (var r in list)
            {
                int newX = imageWidth - (r.X + r.Width);
                //int newX = imageWidth - r.Width - r.X - imageWidth;
                flipped.Add(new System.Drawing.Rectangle(newX, r.Y, r.Width, r.Height));
            }

            return flipped;
        }
        public void Flip()
        {
            this.Flip((int)Image.Source.Width);
        }

    }
}
