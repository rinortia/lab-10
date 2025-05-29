namespace Doodle_Jump
{
    partial class MainMenuForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Код конструктора

        private void InitializeComponent()
        {
            this.btn_new_game = new System.Windows.Forms.Button();
            this.btn_continue_game = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_new_game
            // 
            this.btn_new_game.Location = new System.Drawing.Point(130, 193);
            this.btn_new_game.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_new_game.Name = "btn_new_game";
            this.btn_new_game.Size = new System.Drawing.Size(118, 19);
            this.btn_new_game.TabIndex = 0;
            this.btn_new_game.Text = "Новая игра";
            this.btn_new_game.UseVisualStyleBackColor = true;
            this.btn_new_game.Click += new System.EventHandler(this.btn_new_game_Click);
            // 
            // btn_continue_game
            // 
            this.btn_continue_game.Location = new System.Drawing.Point(270, 193);
            this.btn_continue_game.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_continue_game.Name = "btn_continue_game";
            this.btn_continue_game.Size = new System.Drawing.Size(159, 19);
            this.btn_continue_game.TabIndex = 1;
            this.btn_continue_game.Text = "Продолжить игру";
            this.btn_continue_game.UseVisualStyleBackColor = true;
            this.btn_continue_game.Click += new System.EventHandler(this.btn_continue_game_Click);
            // 
            // MainMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.btn_continue_game);
            this.Controls.Add(this.btn_new_game);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MainMenuForm";
            this.Text = "Doodle Jump";
            this.Load += new System.EventHandler(this.MainMenuForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_new_game;
        private System.Windows.Forms.Button btn_continue_game;
    }
}