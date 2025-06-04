using Doodle_Jump.Properties;
using Model.Data;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Doodle_Jump
{
    public partial class MainMenuForm : Form
    {
        private ComboBox _cmbSaveFormat;
        private Label _lblInvalidFile;

        public MainMenuForm()
        {
            InitializeComponent();
            InitializeButtons();
            InitializeSaveFormatComboBox();
            InitializeSaveFolderSelector();

            Load += MainMenuForm_Load;
            UpdateContinueButton();
        }

        private void InitializeButtons()
        {
            var buttons = new[] { btn_new_game, btn_continue_game };
            foreach (var button in buttons.Where(b => b != null))
            {
                button.BackColor = Color.Wheat;
                button.ForeColor = Color.Black;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.UseVisualStyleBackColor = false;
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            var gameForm = new GameForm(false, _cmbSaveFormat.SelectedItem.ToString());
            gameForm.Show();
            Hide();
        }

        private void InitializeSaveFormatComboBox()
        {
            var lblFormat = new Label
            {
                Text = "Формат сохранения:",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };
            Controls.Add(lblFormat);

            _cmbSaveFormat = new ComboBox
            {
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };

            _cmbSaveFormat.Items.AddRange(new[] { "JSON", "XML" });
            _cmbSaveFormat.SelectedIndex = Settings.Default.LastSaveFormat == "XML" ? 1 : 0;
            _cmbSaveFormat.SelectedIndexChanged += (s, e) =>
            {
                Settings.Default.LastSaveFormat = _cmbSaveFormat.SelectedItem.ToString();
                Settings.Default.Save();
                UpdateContinueButton();
            };

            Controls.Add(_cmbSaveFormat);
            PositionControls();
        }

        private void InitializeSaveFolderSelector()
        {
            var btnSelectFolder = new Button
            {
                Text = "Выбор папки",
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = Color.Wheat,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            btnSelectFolder.FlatAppearance.BorderSize = 0;
            btnSelectFolder.Click += BtnSelectFolder_Click;
            Controls.Add(btnSelectFolder);

            _lblInvalidFile = new Label
            {
                Text = "Некорректный файл сохранения!",
                ForeColor = Color.Red,
                AutoSize = true,
                Visible = false,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            Controls.Add(_lblInvalidFile);
            PositionControls();
        }

        private void BtnSelectFolder_Click(object sender, EventArgs e)
        {
            var folderDialog = new FolderBrowserDialog
            {
                Description = "Выберите папку для сохранения игры",
                SelectedPath = Settings.Default.SaveFolder,
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.MyComputer
            };

            try
            {
                if (folderDialog.ShowDialog(this) == DialogResult.OK)
                {
                    Settings.Default.SaveFolder = folderDialog.SelectedPath;
                    Settings.Default.Save();
                    UpdateContinueButton();
                }
            }
            finally
            {
                folderDialog.Dispose();
            }
        }

        private void UpdateContinueButton()
        {
            try
            {
                if (string.IsNullOrEmpty(Settings.Default.SaveFolder))
                {
                    SetContinueButtonState(false, "Выберите папку");
                    _lblInvalidFile.Visible = false;
                    return;
                }

                string format = _cmbSaveFormat.SelectedItem?.ToString() ?? "JSON";
                string savePath = SaveManager.GetSavePath(format, Settings.Default.SaveFolder);

                bool exists = File.Exists(savePath);
                bool isValid = exists && IsValidSaveFile(savePath, format);

                SetContinueButtonState(isValid, isValid ? "Продолжить игру" : "Нет сохранений");
                _lblInvalidFile.Text = exists && !isValid ? "Выбранный файл имеет некорректный формат!" : "Некорректный файл сохранения!";
                _lblInvalidFile.Visible = exists && !isValid;
            }
            catch (Exception ex)
            {
                SetContinueButtonState(false, "Ошибка загрузки");
                _lblInvalidFile.Text = $"Ошибка: {ex.Message}";
                _lblInvalidFile.Visible = true;
            }
        }

        private void SetContinueButtonState(bool enabled, string text)
        {
            btn_continue_game.Enabled = enabled;
            btn_continue_game.Text = text;
            btn_continue_game.BackColor = Color.Wheat;
        }

        private bool IsValidSaveFile(string path, string format)
        {
            try
            {
                if (format == "JSON")
                {
                    var json = File.ReadAllText(path);
                    JsonConvert.DeserializeObject<GameState>(json);
                    return true;
                }

                if (format == "XML")
                {
                    var serializer = new XmlSerializer(typeof(GameState));
                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        serializer.Deserialize(stream);
                    }
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private void MainMenuForm_Load(object sender, EventArgs e)
        {
            UpdateContinueButton();
        }

        private void btn_new_game_Click(object sender, EventArgs e)
        {
            Hide();
            var gameForm = new GameForm(false, _cmbSaveFormat.SelectedItem.ToString());
            try
            {
                gameForm.ShowDialog();
            }
            finally
            {
                gameForm.Dispose();
                Show();
                UpdateContinueButton();
            }
        }

        private void btn_continue_game_Click(object sender, EventArgs e)
        {
            try
            {
                Hide();
                string format = _cmbSaveFormat.SelectedItem.ToString();
                var loadedState = SaveManager.Load(format, Settings.Default.SaveFolder);

                if (loadedState == null)
                {
                    MessageBox.Show("Не удалось загрузить сохранение. Возможно, файл поврежден.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Show();
                    UpdateContinueButton();
                    return;
                }

                var gameForm = new GameForm(true, format);
                try
                {
                    gameForm.ShowDialog();
                }
                finally
                {
                    gameForm.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при продолжении игры: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Show();
                UpdateContinueButton();
            }
        }

        private void PositionControls()
        {
            int centerX = (ClientSize.Width - 200) / 2;
            int startY = btn_continue_game.Bottom + 20;

            var lblFormat = Controls.OfType<Label>().FirstOrDefault(l => l.Text == "Формат сохранения:");
            lblFormat?.SetLocation(centerX, startY);

            _cmbSaveFormat?.SetLocation(centerX, startY + 25);

            var btnSelectFolder = Controls.OfType<Button>().FirstOrDefault(b => b.Text == "Выбор папки");
            btnSelectFolder?.SetLocation(centerX, startY + 50);

            _lblInvalidFile?.SetLocation(centerX, startY + 100);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionControls();
        }
    }

    internal static class ControlExtensions
    {
        public static void SetLocation(this Control control, int x, int y)
        {
            control.Location = new Point(x, y);
        }
    }
}