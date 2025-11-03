using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace WpfGame
{
    public class Enemy
    {
        public Rectangle Visual { get; private set; }
        public bool IsAlive => Visual != null;
        public int Level { get; private set; }

        public Enemy(Canvas canvas, double x, double y, int level)
        {
            Level = level;
            CreateVisual(canvas, x, y, level);
        }

        private void CreateVisual(Canvas canvas, double x, double y, int level)
        {
            Visual = new Rectangle()
            {
                Width = 34,
                Height = 34, // Квадратная форма для изображений
                Fill = CreateEnemyAppearance(level),
                Opacity = 1
            };
            canvas.Children.Add(Visual);
            Canvas.SetLeft(Visual, x);
            Canvas.SetTop(Visual, y);
        }

        private Brush CreateEnemyAppearance(int level)
        {
            try
            {
                var imageBrush = new ImageBrush();
                string imagePath = GetEnemyImagePath(level);
                imageBrush.ImageSource = new System.Windows.Media.Imaging.BitmapImage(
                    new System.Uri(imagePath));
                imageBrush.Stretch = Stretch.Uniform;
                return imageBrush;
            }
            catch
            {
                // Fallback цвета для разных уровней
                Color fallbackColor = level switch
                {
                    1 => Colors.Red,
                    2 => Colors.Orange,
                    3 => Colors.Yellow,
                    _ => Colors.Purple
                };
                return new SolidColorBrush(fallbackColor);
            }
        }

        private string GetEnemyImagePath(int level)
        {
            return level switch
            {
                1 => "pack://application:,,,/Images/1.png",
                2 => "pack://application:,,,/Images/2.png",
                3 => "pack://application:,,,/Images/3.png",
                _ => "pack://application:,,,/Images/1.png"
            };
        }


        public void Move(double deltaX)
        {
            if (Visual != null)
            {
                Canvas.SetLeft(Visual, Canvas.GetLeft(Visual) + deltaX);
            }
        }

        public void Remove(Canvas canvas)
        {
            if (Visual != null)
            {
                canvas.Children.Remove(Visual);
                Visual = null;
            }
        }

        public Rect GetBounds()
        {
            return new Rect(Canvas.GetLeft(Visual), Canvas.GetTop(Visual), Visual.Width, Visual.Height);
        }
    }
}