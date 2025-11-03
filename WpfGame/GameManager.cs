using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfGame
{
    public class GameManager
    {
        private Player player;
        private EnemyManager enemyManager;
        private BulletManager bulletManager;
        private ShieldManager shieldManager;
        private AnimationManager animationManager;
        private GameStateManager gameState;
        private UIManager uiManager;
        private CollisionManager collisionManager;
        private Random rnd = new Random();

        public GameManager(Player player, EnemyManager enemyManager, BulletManager bulletManager,
                         ShieldManager shieldManager, AnimationManager animationManager,
                         GameStateManager gameState, UIManager uiManager, CollisionManager collisionManager)
        {
            this.player = player;
            this.enemyManager = enemyManager;
            this.bulletManager = bulletManager;
            this.shieldManager = shieldManager;
            this.animationManager = animationManager;
            this.gameState = gameState;
            this.uiManager = uiManager;
            this.collisionManager = collisionManager;
        }

        public void UpdateGame()
        {
            player.UpdatePosition();

            if (player.Shooting)
            {
                var playerPos = Canvas.GetLeft(player.Visual);
                var playerTop = Canvas.GetTop(player.Visual);
                bulletManager.ShootPlayerBullet(playerPos, playerTop, player.Visual.Width);
                player.Shooting = false;
            }

            bulletManager.UpdateBullets(10, 6);

            double enemySpeed = 1.0 + (gameState.Level * 0.3);
            enemyManager.UpdateEnemies(enemySpeed);

            HandleEnemyShooting();

            collisionManager.CheckAllCollisions();

            CheckGameConditions();
        }

        private void HandleEnemyShooting()
        {
            if (!enemyManager.BossFight)
            {
                if (rnd.NextDouble() < 0.02 + (0.02 * gameState.Level))
                {
                    var shooter = enemyManager.GetRandomShooter();
                    if (shooter != null)
                    {
                        var shooterPos = Canvas.GetLeft(shooter.Visual);
                        var shooterTop = Canvas.GetTop(shooter.Visual);
                        bulletManager.ShootEnemyBullet(shooterPos, shooterTop,
                            shooter.Visual.Width, shooter.Visual.Height);
                    }
                }
            }
            else
            {
                if (enemyManager.IsBossAlive && rnd.NextDouble() < 0.05)
                {
                    var boss = enemyManager.Boss;
                    var position = boss.GetPosition();

                    if (position.HasValue)
                    {
                        bulletManager.ShootEnemyBullet(position.Value.x, position.Value.y,
                            boss.Visual.Width, boss.Visual.Height);
                    }
                }
            }
        }

        private void CheckGameConditions()
        {
            if (collisionManager.ShouldAdvanceLevel())
            {
                gameState.NextLevel();
                if (gameState.Level > 4)
                {
                    gameState.TriggerWinGame();
                }
                else
                {
                    SpawnEnemiesForLevel(gameState.Level);
                }
            }
        }

        public void SpawnEnemiesForLevel(int level)
        {
            enemyManager.SpawnEnemiesForLevel(level);
            uiManager.UpdateLevel(level);

            if (enemyManager.BossFight)
            {
                uiManager.ShowBossHealthBar();
                uiManager.UpdateBossHealthBarPosition((player.Visual.Parent as Canvas).ActualWidth);
                uiManager.UpdateBossHealthBar(1.0);
            }
            else
            {
                uiManager.HideBossHealthBar();
            }
        }

        public void ResetGame()
        {
            gameState.ResetGame();
            uiManager.UpdateScore(gameState.Score);
            uiManager.UpdateLives(player.Lives);
            uiManager.UpdateLevel(gameState.Level);

            bulletManager.ClearAllBullets();
            enemyManager.ClearEnemies();
            shieldManager.ClearShields();
            shieldManager.CreateShields();

            SpawnEnemiesForLevel(gameState.Level);
            player.AnimateSpawn();
            gameState.StartGame();
        }
    }
}