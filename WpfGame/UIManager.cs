using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfGame
{
    public class UIManager
    {
        public TextBlock ScoreText { get; private set; }
        public TextBlock LivesText { get; private set; }
        public TextBlock LevelText { get; private set; }
        public Border BossHealthBarBG { get; private set; }
        public Rectangle BossHealthBarFill { get; private set; }

        private Canvas gameCanvas;

        public UIManager(Canvas canvas)
        {
            gameCanvas = canvas;
            InitializeUI();
        }

        private void InitializeUI()
        {
            ScoreText = new TextBlock()
            {
                FontSize = 18,
                Foreground = Brushes.White
            };
            gameCanvas.Children.Add(ScoreText);
            Canvas.SetLeft(ScoreText, 8);
            Canvas.SetTop(ScoreText, 8);

            LivesText = new TextBlock()
            {
                FontSize = 18,
                Foreground = Brushes.White
            };
            gameCanvas.Children.Add(LivesText);
            Canvas.SetRight(LivesText, 8);
            Canvas.SetTop(LivesText, 8);

            LevelText = new TextBlock()
            {
                FontSize = 18,
                Foreground = Brushes.White
            };
            gameCanvas.Children.Add(LevelText);
            Canvas.SetRight(LevelText, 8);
            Canvas.SetTop(LevelText, 32);

            BossHealthBarBG = new Border()
            {
                Width = 250,
                Height = 18,
                Background = new SolidColorBrush(Color.FromRgb(40, 20, 60)),
                BorderBrush = Brushes.MediumPurple,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(8),
                Visibility = Visibility.Hidden
            };
            gameCanvas.Children.Add(BossHealthBarBG);
            Canvas.SetTop(BossHealthBarBG, 8);

            LinearGradientBrush gradient = new LinearGradientBrush();
            gradient.GradientStops.Add(new GradientStop(Color.FromRgb(220, 180, 255), 0));
            gradient.GradientStops.Add(new GradientStop(Color.FromRgb(160, 80, 255), 0.5));
            gradient.GradientStops.Add(new GradientStop(Color.FromRgb(100, 0, 200), 1));

            BossHealthBarFill = new Rectangle()
            {
                Width = 250,
                Height = 18,
                Fill = gradient,
                RadiusX = 8,
                RadiusY = 8,
                Visibility = Visibility.Hidden
            };
            gameCanvas.Children.Add(BossHealthBarFill);
            Canvas.SetTop(BossHealthBarFill, 8);
        }

        public void UpdateScore(int score)
        {
            ScoreText.Text = $"Score: {score}";
        }

        public void UpdateLives(int lives)
        {
            LivesText.Text = $"Lives: {lives}";
        }

        public void UpdateLevel(int level)
        {
            LevelText.Text = $"Level: {level}";
        }

        public void UpdateBossHealthBar(double ratio)
        {
            BossHealthBarFill.Width = 250 * ratio;
        }

        public void ShowBossHealthBar()
        {
            BossHealthBarBG.Visibility = Visibility.Visible;
            BossHealthBarFill.Visibility = Visibility.Visible;
        }

        public void HideBossHealthBar()
        {
            BossHealthBarBG.Visibility = Visibility.Hidden;
            BossHealthBarFill.Visibility = Visibility.Hidden;
        }

        public void UpdateBossHealthBarPosition(double canvasWidth)
        {
            if (canvasWidth > 0)
            {
                Canvas.SetLeft(BossHealthBarBG, (canvasWidth - BossHealthBarBG.Width) / 2);
                Canvas.SetLeft(BossHealthBarFill, (canvasWidth - BossHealthBarFill.Width) / 2);
            }
        }
    }
}