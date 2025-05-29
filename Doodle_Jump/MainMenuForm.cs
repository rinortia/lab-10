using System;
using System.Windows.Forms;
using Model.Data;

namespace Doodle_Jump
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
            this.Load += MainMenuForm_Load; // <-- важно!
            UpdateContinueButton();
        }

        private void UpdateContinueButton()
        {
            try
            {
                btn_continue_game.Enabled = SaveManager.SaveExists();
                btn_continue_game.Text = SaveManager.SaveExists()
                    ? "Продолжить игру"
                    : "Нет сохранений";
            }
            catch
            {
                btn_continue_game.Enabled = false;
                btn_continue_game.Text = "Ошибка загрузки";
            }
        }

        private void MainMenuForm_Load(object sender, EventArgs e)
        {
            UpdateContinueButton();
        }

        private void btn_new_game_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var gameForm = new GameForm(false))
                gameForm.ShowDialog();
            this.Show();
            UpdateContinueButton();
        }

        private void btn_continue_game_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var gameForm = new GameForm(true))
                gameForm.ShowDialog();
            this.Show();
            UpdateContinueButton();
        }
    }
}
