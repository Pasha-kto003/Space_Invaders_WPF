using System;
using System.Windows.Threading;

namespace WpfGame
{
    public class GameStateManager
    {
        public int Score { get; set; }
        public int Level { get; set; } = 1;
        public bool IsPaused { get; set; }
        public bool IsGameOver { get; private set; }
        public bool IsGameWon { get; private set; }

        public event Action GameOver;
        public event Action GameWon;

        private DispatcherTimer gameTimer;

        public GameStateManager(DispatcherTimer timer)
        {
            gameTimer = timer;
        }

        public void IncreaseScore(int points)
        {
            Score += points;
        }

        public void NextLevel()
        {
            Level++;
        }

        public void ResetGame()
        {
            Score = 0;
            Level = 1;
            IsGameOver = false;
            IsGameWon = false;
            IsPaused = false;
        }

        public void TriggerGameOver()
        {
            IsGameOver = true;
            gameTimer.Stop();
            GameOver?.Invoke();
        }

        public void TriggerWinGame()
        {
            IsGameWon = true;
            gameTimer.Stop();
            GameWon?.Invoke();
        }

        public void StartGame()
        {
            IsGameOver = false;
            IsGameWon = false;
            gameTimer.Start();
        }

        public bool ShouldSpawnBoss()
        {
            return Level == 4;
        }

        public bool IsBossLevel()
        {
            return Level >= 4;
        }
    }
}