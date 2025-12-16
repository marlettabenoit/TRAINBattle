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
    // class frame qui sert à gérer une frame (bah oui)
    // par contre, une Frame peux être affiché durant plusieurs frames (images à l'écran)
    public class Frame
    {
        // Hearthbox -> la ou on a mal
        // heatbox -> la ou les autres ont mal
        public List<System.Drawing.Rectangle> HearthBoxs { get; set; }
        public List<System.Drawing.Rectangle> HitBoxs { get; set; }
        public Image Image { get; set; }
        public bool IsFlipped { get; private set; } = false;
        // Certaines frames font déplacer le joueur d'une certaine valeur en x durant leurs affichage (ex: dash)
        public int DeplacementX { get; set; } = 0;
        // 'base', 'protect', 'grab', ou 'tir'
        public string Type { get; set; } = "base";
        // Un petit tuple contenant les infos sur les tirs éventuels que génere la frame
        public (string imagePath, int x, int y, double dirX, double dirY, double speed, int damage, bool enCloche) ProjecInfo { get; set; }

        // Champ puissance avec encapsulation (pas refait après car on à pas le time)
        private int puissance;
        public int Puissance
        {
            get { return this.puissance; }
            set {
                if (value < 0 ) throw new ArgumentOutOfRangeException("Puissance < 0"); // On ne soigne pas l'ennemie
                this.puissance = value;
            }
        }
        // Elle aussi encapsule mais promis c'est la derniére
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
        public Frame(string imagePath, int duree, int puissance, int deplacementX)
        {
            // initialisation les listes
            HearthBoxs = new List<System.Drawing.Rectangle>();
            HitBoxs = new List<System.Drawing.Rectangle>();
            // Charge l'image
            string imagePathComplet = $"pack://application:,,,/img/{imagePath}";
            Image = new Image();
            Image.Source = new BitmapImage(new Uri(imagePathComplet));
            // Suprimer les 2 lignes suivantes crée des bugs
            Image.Width = ((BitmapImage)Image.Source).PixelWidth;
            Image.Height = ((BitmapImage)Image.Source).PixelHeight;
            // Autres parametres
            Duree = duree;
            Puissance = puissance;
            DeplacementX = deplacementX;
        }

        // Ajoute un projectil à la frame
        // Pour etre exact, ajoute les infos utiles à sa création
        public void AddProjectile(string imagePath, int x, int y, double dirX, double dirY, double speed, int damage, bool enCloche)
        {
            ProjecInfo = (imagePath, x, y, dirX, dirY, speed, damage, enCloche);
            Type = "tir";
        }

        // renvoi un nouveau projectil crée grace au tuple
        public Projectils GetProjectil()
        {
            return new Projectils(ProjecInfo.imagePath, ProjecInfo.x, ProjecInfo.y, ProjecInfo.dirX, ProjecInfo.dirY, ProjecInfo.speed, ProjecInfo.damage, ProjecInfo.enCloche);
        }

        // Surcharge pour les frames qui n'ont ni puissance ni deplacement
        public Frame(string imagePath, int duree)
            : this(imagePath, duree, 0, 0)
        {
        }

        // Ajoute une hitbox à la frame
        public void AddHitbox(int x, int y, int width, int height)
        {
            HitBoxs.Add(new System.Drawing.Rectangle(x, y, width, height));
        }

        // Ajoute une hearthbox à la frame
        public void AddHearthbox(int x, int y, int width, int height)
        {
            HearthBoxs.Add(new System.Drawing.Rectangle(x, y, width, height));
        }

        // Affiche sur le canvas à la position x,y
        // (x,y) est le point en bas à gauche
        public void Display(Canvas canvas, int posX, int posY)
        {
            // normalement c'est le cas car on clear le canvas à chaque frame
            if (!canvas.Children.Contains(Image))
                canvas.Children.Add(Image);

            // Récupérer la hauteur réelle de l'image | merci WPF de nous forcer à faire ca :(
            double imgHeight = ((BitmapImage)Image.Source).PixelWidth;
            double imgWidth = ((BitmapImage)Image.Source).PixelWidth;

            double topImage = posY - imgHeight;

            Canvas.SetLeft(Image, posX);
            Canvas.SetTop(Image, topImage);

#if DEBUG
            // Debug rectangles
            foreach (var rect in HitBoxs)
            {
                var box = CreateDebugRect(rect, System.Windows.Media.Brushes.Red);
                canvas.Children.Add(box);

                double top = topImage + rect.Y;
                top = posY - (rect.Y + rect.Height);

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
#endif
        }

        // Crée un rectangle graphique utile pour le débug (même très utile)
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

        // Retourne la frame
        public void Flip(int imageWidth)
        {
            IsFlipped = !IsFlipped; // met à jours la var qui permet de savoir si la frame est retourné
            DeplacementX=-DeplacementX; // Sinon, on fait comme Mc Jacson
            // Retourner visuellement l'image (miroir horizontal)
            if (IsFlipped)
                Image.LayoutTransform = new ScaleTransform(-1, 1);
            else
                Image.LayoutTransform = new ScaleTransform(1, 1);

            // Inversion des rectangles
            HitBoxs = FlipRectList(HitBoxs, imageWidth);
            HearthBoxs = FlipRectList(HearthBoxs, imageWidth);
        }

        // Inverse une liste de rectangle au sein de l'image
        private List<System.Drawing.Rectangle> FlipRectList(List<System.Drawing.Rectangle> list, int imageWidth)
        {
            var flipped = new List<System.Drawing.Rectangle>();

            foreach (var r in list)
            {
                int newX = imageWidth - (r.X + r.Width);
                flipped.Add(new System.Drawing.Rectangle(newX, r.Y, r.Width, r.Height));
            }

            return flipped;
        }

        // Apelle juste la fonction interne
        public void Flip()
        {
            this.Flip(((BitmapImage)Image.Source).PixelWidth);
        }

    }
}
