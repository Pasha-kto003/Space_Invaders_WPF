using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace WpfGame
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer gameTimer;

        // Менеджеры
        private Player player;
        private EnemyManager enemyManager;
        private BulletManager bulletManager;
        private ShieldManager shieldManager;
        private AnimationManager animationManager;
        private UIManager uiManager;
        private GameStateManager gameState;
        private CollisionManager collisionManager;
        private GameManager gameManager;
        private PauseMenuManager pauseMenuManager;

        public MainWindow()
        {
            InitializeComponent();

            InitializeManagers();
            InitializeGame();

            gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void InitializeManagers()
        {
            gameTimer = new DispatcherTimer();

            player = new Player(GameCanvas);
            enemyManager = new EnemyManager(GameCanvas);
            bulletManager = new BulletManager(GameCanvas);
            shieldManager = new ShieldManager(GameCanvas);
            animationManager = new AnimationManager(GameCanvas);
            uiManager = new UIManager(GameCanvas);
            gameState = new GameStateManager(gameTimer);

            gameState.GameOver += OnGameOver;
            gameState.GameWon += OnGameWon;

            collisionManager = new CollisionManager(player, enemyManager, bulletManager, shieldManager,
                                                  animationManager, gameState, uiManager);
            gameManager = new GameManager(player, enemyManager, bulletManager, shieldManager,
                                        animationManager, gameState, uiManager, collisionManager);


            pauseMenuManager = new PauseMenuManager(GameCanvas, gameState);
        }

        private void OnGameOver()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                var rnd = new Random();
                for (int i = 0; i < 5; i++)
                {
                    double delaySeconds = i * 0.2;
                    DispatcherTimer explosionTimer = new DispatcherTimer()
                    {
                        Interval = TimeSpan.FromSeconds(delaySeconds)
                    };
                    explosionTimer.Tick += (s, e) =>
                    {
                        animationManager.CreateExplosion(
                            rnd.NextDouble() * GameCanvas.ActualWidth,
                            rnd.NextDouble() * GameCanvas.ActualHeight,
                            40
                        );
                        explosionTimer.Stop();
                    };
                    explosionTimer.Start();
                }

                DispatcherTimer messageTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(1.0)
                };
                messageTimer.Tick += (s, e) =>
                {
                    messageTimer.Stop();
                    MessageBox.Show($"Game over! Score: {gameState.Score}", "Game Over",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    ResetGame();
                };
                messageTimer.Start();
            }));
        }

        private void OnGameWon()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {

                var rnd = new Random();
                for (int i = 0; i < 10; i++)
                {
                    double delaySeconds = i * 0.15;
                    DispatcherTimer explosionTimer = new DispatcherTimer()
                    {
                        Interval = TimeSpan.FromSeconds(delaySeconds)
                    };
                    explosionTimer.Tick += (s, e) =>
                    {
                        animationManager.CreateExplosion(
                            rnd.NextDouble() * GameCanvas.ActualWidth,
                            rnd.NextDouble() * GameCanvas.ActualHeight,
                            50
                        );
                        explosionTimer.Stop();
                    };
                    explosionTimer.Start();
                }


                DispatcherTimer messageTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(1.5)
                };
                messageTimer.Tick += (s, e) =>
                {
                    messageTimer.Stop();
                    MessageBox.Show($"Поздравляем! Вы победили всех противников! Итоговый счёт: {gameState.Score}",
                        "Победа", MessageBoxButton.OK, MessageBoxImage.Information);
                    ResetGame();
                };
                messageTimer.Start();
            }));
        }

        private void ResetGame()
        {
            gameManager.ResetGame();
        }

        private void InitializeGame()
        {

            uiManager.UpdateScore(0);
            uiManager.UpdateLives(player.Lives);
            uiManager.UpdateLevel(1);


            Loaded += (s, e) =>
            {
                pauseMenuManager.UpdatePosition(GameCanvas.ActualWidth, GameCanvas.ActualHeight);
                uiManager.UpdateBossHealthBarPosition(GameCanvas.ActualWidth);

                player.SetPosition((GameCanvas.ActualWidth - player.Visual.Width) / 2,
                                 GameCanvas.ActualHeight - player.Visual.Height - 20);

                shieldManager.CreateShields();
                gameManager.SpawnEnemiesForLevel(1);
            };


            SizeChanged += (s, e) =>
            {
                pauseMenuManager.UpdatePosition(GameCanvas.ActualWidth, GameCanvas.ActualHeight);
                uiManager.UpdateBossHealthBarPosition(GameCanvas.ActualWidth);
            };
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (gameState.IsPaused) return;
            gameManager.UpdateGame();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.P)
            {
                pauseMenuManager.TogglePause();
                return;
            }

            if (gameState.IsPaused) return;
            player.HandleKeyDown(e.Key);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (gameState.IsPaused) return;
            player.HandleKeyUp(e.Key);
        }
    }
}