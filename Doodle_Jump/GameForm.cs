using System;
using System.Drawing;
using System.Windows.Forms;
using Model.Core;
using Model.Data;
using Newtonsoft.Json;

namespace Doodle_Jump
{
    public partial class GameForm : Form
    {
        private GameWorld _gameWorld;
        private readonly bool _loadSavedGame;
        private readonly string _saveFormat;
        private readonly string _saveFolder;
        private Func<int, string> _scoreDisplayFormatter;


        public GameForm(bool loadSavedGame, string saveFormat = "JSON")
        {
            InitializeComponent();
            _loadSavedGame = loadSavedGame;
            _saveFormat = saveFormat;
            _saveFolder = Properties.Settings.Default.SaveFolder;
            _scoreDisplayFormatter = score => $"Score: {score}";


            InitializeGameTimer();
            InitializeFormSettings();
            InitializeGameWorld();
        }

        private void InitializeGameTimer()
        {
            game_timer.Interval = 16;
            game_timer.Tick += GameTimer_Tick;
        }

        private void InitializeFormSettings()
        {
            ClientSize = new Size(400, 600);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
        }

        private void InitializeGameWorld()
        {
            var player = new Player(150, ClientSize.Height - 20 - Player.Height);
            player.SetVelocityY(-10f);
            player.SetIsOnGround(false);

            _gameWorld = new GameWorld(player);
            _gameWorld.GameOver += OnGameOver;
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            if (_loadSavedGame)
                LoadGame();
            else
                StartNewGame();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            _gameWorld.Update(ClientSize);
            Invalidate();
        }

        private void StartNewGame()
        {
            _gameWorld.StartNewGame(ClientSize);
            game_timer.Start();
        }

        private void LoadGame()
        {
            _gameWorld.LoadGameState(_saveFormat, ClientSize, _saveFolder);
            game_timer.Start();
        }

        private void SaveGame()
        {
            _gameWorld.SaveGameState(_saveFormat, _saveFolder);
        }

        private void OnGameOver()
        {
            game_timer.Stop();
            SaveGame();
            MessageBox.Show($"Игра окончена! Счёт: {_gameWorld.Score}");
            Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            foreach (var platform in _gameWorld.Platforms)
                platform.Draw(g);

            _gameWorld.Player.Draw(g);
            g.DrawString(_scoreDisplayFormatter(_gameWorld.Score),
                    Font, Brushes.Black, 10, 10);
        }

        public void SetScoreFormatter(Func<int, string> formatter)
        {
            _scoreDisplayFormatter = formatter;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.A:
                    _gameWorld.Player.MoveLeft();
                    break;
                case Keys.Right:
                case Keys.D:
                    _gameWorld.Player.MoveRight();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_gameWorld.IsGameOver)
            {
                SaveGame();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }
    }
}
