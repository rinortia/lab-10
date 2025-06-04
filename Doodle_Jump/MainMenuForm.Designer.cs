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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                backgroundImage?.Dispose();
            }
            base.Dispose(disposing);
        }

        private Label lbl_title;

        private void InitializeComponent()
        {
            this.btn_new_game = new Button();
            this.btn_continue_game = new Button();
            var buttonFont = new Font("Comic Sans MS", 13F, FontStyle.Regular);
            this.SuspendLayout();

            try
            {
                backgroundImage = Image.FromFile("Resources/background1.png");
            }
            catch
            {
            }

            this.lbl_title = new Label();
            this.lbl_title.Text = "DOODLE JUMP";
            this.lbl_title.Font = new Font("Comic Sans MS", 24F, FontStyle.Bold);
            this.lbl_title.ForeColor = Color.Orange;
            this.lbl_title.BackColor = Color.Transparent;
            this.lbl_title.AutoSize = true;
            this.lbl_title.Location = new Point(150, 80);
            this.Controls.Add(this.lbl_title);

            this.AutoScaleDimensions = new SizeF(8F, 16F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(600, 450);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Text = "Doodle Jump - Меню";

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
            this.btn_new_game.FlatStyle = FlatStyle.Flat;
            this.btn_new_game.FlatAppearance.BorderSize = 0;

            this.btn_continue_game.Location = new Point(320, 180);
            this.btn_continue_game.Name = "btn_continue_game";
            this.btn_continue_game.Size = new Size(180, 60);
            this.btn_continue_game.TabIndex = 1;
            this.btn_continue_game.Text = "Продолжить";
            this.btn_continue_game.BackColor = Color.Wheat;
            this.btn_continue_game.ForeColor = Color.Black;
            this.btn_continue_game.FlatStyle = FlatStyle.Flat;
            this.btn_continue_game.FlatAppearance.BorderSize = 0;
            this.btn_continue_game.Font = buttonFont;
            this.btn_continue_game.Cursor = Cursors.Hand;
            this.btn_continue_game.UseCompatibleTextRendering = true;
            this.btn_continue_game.Click += new EventHandler(this.btn_continue_game_Click);
            this.Controls.Add(this.btn_new_game);
            this.Controls.Add(this.btn_continue_game);

            this.Load += new EventHandler(this.MainMenuForm_Load);

            this.ResumeLayout(false);
        }

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
