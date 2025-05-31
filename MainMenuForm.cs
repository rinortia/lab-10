using System;
using System.Windows.Forms;
using Model.Data;

namespace Doodle_Jump
{
    public partial class MainMenuForm : Form
    {
        private ComboBox cmbSaveFormat;

        public MainMenuForm()
        {
            InitializeComponent();
            InitializeSaveFormatComboBox();
            this.Load += MainMenuForm_Load;
            UpdateContinueButton();
        }

        private void InitializeSaveFormatComboBox()
        {
            // Создаем ComboBox для выбора формата
            cmbSaveFormat = new ComboBox
            {
                Location = new System.Drawing.Point(50, 150),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Добавляем только JSON и XML форматы
            cmbSaveFormat.Items.AddRange(new object[] {
                "JSON",
                "XML"
            });

            // Устанавливаем сохраненный формат или JSON по умолчанию
            cmbSaveFormat.SelectedIndex = Properties.Settings.Default.LastSaveFormat == "XML" ? 1 : 0;

            // Сохраняем выбор при изменении
            cmbSaveFormat.SelectedIndexChanged += (s, e) =>
            {
                Properties.Settings.Default.LastSaveFormat = cmbSaveFormat.SelectedItem.ToString();
                Properties.Settings.Default.Save();
            };

            // Добавляем на форму
            this.Controls.Add(cmbSaveFormat);

            // Подпись для ComboBox
            var lblFormat = new Label
            {
                Text = "Формат сохранения:",
                Location = new System.Drawing.Point(50, 130),
                AutoSize = true
            };
            this.Controls.Add(lblFormat);
        }

        private void UpdateContinueButton()
        {
            try
            {
                string format = cmbSaveFormat.SelectedItem?.ToString() ?? "JSON";
                bool exists = SaveManager.SaveExists(format);
                btn_continue_game.Enabled = exists;
                btn_continue_game.Text = exists
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
            using (var gameForm = new GameForm(false, cmbSaveFormat.SelectedItem.ToString()))
                gameForm.ShowDialog();
            this.Show();
            UpdateContinueButton();
        }

        private void btn_continue_game_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var gameForm = new GameForm(true, cmbSaveFormat.SelectedItem.ToString()))
                gameForm.ShowDialog();
            this.Show();
            UpdateContinueButton();
        }
    }
}