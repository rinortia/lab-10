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
            // Подпись для ComboBox
            var lblFormat = new Label
            {
                Text = "Формат сохранения:",
                Location = new System.Drawing.Point(200, 250),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular)
            };
            this.Controls.Add(lblFormat);

            // Создаем ComboBox для выбора формата
            cmbSaveFormat = new ComboBox
            {
                Location = new System.Drawing.Point(200, 275),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new System.Drawing.Font("Segoe UI", 10F)
            };

            // Добавляем форматы JSON и XML
            cmbSaveFormat.Items.AddRange(new object[] { "JSON", "XML" });

            // Выбираем последний сохраненный формат, либо JSON по умолчанию
            cmbSaveFormat.SelectedIndex = Properties.Settings.Default.LastSaveFormat == "XML" ? 1 : 0;

            // Сохраняем выбор и обновляем кнопку при изменении
            cmbSaveFormat.SelectedIndexChanged += (s, e) =>
            {
                Properties.Settings.Default.LastSaveFormat = cmbSaveFormat.SelectedItem.ToString();
                Properties.Settings.Default.Save();
                UpdateContinueButton();
            };

            this.Controls.Add(cmbSaveFormat);
        }

        private void UpdateContinueButton()
        {
            try
            {
                string format = cmbSaveFormat.SelectedItem?.ToString() ?? "JSON";
                bool exists = SaveManager.SaveExists(format);
                btn_continue_game.Enabled = exists;
                btn_continue_game.Text = exists ? "Продолжить игру" : "Нет сохранений";
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
