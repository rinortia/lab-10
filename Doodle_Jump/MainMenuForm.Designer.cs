using System;
using System.Drawing;
using System.Windows.Forms;

namespace Doodle_Jump
{
    partial class MainMenuForm
    {
        private System.ComponentModel.IContainer components = null;
        private Button btn_new_game;
        private Button btn_continue_game;
        private Image backgroundImage;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                backgroundImage?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Инициализация компонентов формы.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_new_game = new Button();
            this.btn_continue_game = new Button();

            var buttonFont = new Font("Comic Sans MS", 16F, FontStyle.Regular);

            this.SuspendLayout();

            // Загрузка картинки из файла Resources/background.png
            backgroundImage = Image.FromFile("Resources/background.png");

            // 
            // MainMenuForm
            // 
            this.AutoScaleDimensions = new SizeF(8F, 16F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(600, 450);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.Text = "Doodle Jump - Меню";

            // Убираем стандартный фон — фон будет отрисовываться картинкой
            this.BackColor = Color.White;

            //
            // btn_new_game
            //
            this.btn_new_game.Location = new Point(100, 180);
            this.btn_new_game.Name = "btn_new_game";
            this.btn_new_game.Size = new Size(180, 60);
            this.btn_new_game.TabIndex = 0;
            this.btn_new_game.Text = "Новая игра";
            this.btn_new_game.BackColor = Color.Wheat;
            this.btn_new_game.ForeColor = Color.Black;
            this.btn_new_game.FlatStyle = FlatStyle.Flat;
            this.btn_new_game.FlatAppearance.BorderSize = 0;
            this.btn_new_game.Font = buttonFont;
            this.btn_new_game.Cursor = Cursors.Hand;
            this.btn_new_game.UseCompatibleTextRendering = true;
            this.btn_new_game.Click += new EventHandler(this.btn_new_game_Click);

            //
            // btn_continue_game
            //
            this.btn_continue_game.Location = new Point(320, 180);
            this.btn_continue_game.Name = "btn_continue_game";
            this.btn_continue_game.Size = new Size(180, 60);
            this.btn_continue_game.TabIndex = 1;
            this.btn_continue_game.Text = "Продолжить";
            this.btn_continue_game.BackColor = Color.BurlyWood;
            this.btn_continue_game.ForeColor = Color.Black;
            this.btn_continue_game.FlatStyle = FlatStyle.Flat;
            this.btn_continue_game.FlatAppearance.BorderSize = 0;
            this.btn_continue_game.Font = buttonFont;
            this.btn_continue_game.Cursor = Cursors.Hand;
            this.btn_continue_game.UseCompatibleTextRendering = true;
            this.btn_continue_game.Click += new EventHandler(this.btn_continue_game_Click);

            //
            // Добавляем кнопки на форму
            //
            this.Controls.Add(this.btn_new_game);
            this.Controls.Add(this.btn_continue_game);

            // Подписка на событие загрузки формы (одна подписка)
            this.Load += new EventHandler(this.MainMenuForm_Load);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Перерисовка фона — рисуем картинку backgroundImage
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (backgroundImage != null)
            {
                e.Graphics.DrawImage(backgroundImage, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }
    }
}
