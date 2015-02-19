﻿namespace GalaxyConquest.Tactics
{
    partial class CombatForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureMap = new System.Windows.Forms.PictureBox();
            this.boxDescription = new System.Windows.Forms.Label();
            this.btnEndTurn = new System.Windows.Forms.Button();
            this.lblTurn = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBlueShips = new System.Windows.Forms.Label();
            this.txtRedShips = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureMap)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureMap
            // 
            this.pictureMap.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureMap.Location = new System.Drawing.Point(153, 0);
            this.pictureMap.Name = "pictureMap";
            this.pictureMap.Size = new System.Drawing.Size(1225, 792);
            this.pictureMap.TabIndex = 0;
            this.pictureMap.TabStop = false;
            this.pictureMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureMap_MouseClick);
            // 
            // boxDescription
            // 
            this.boxDescription.AutoSize = true;
            this.boxDescription.Enabled = false;
            this.boxDescription.Location = new System.Drawing.Point(12, 39);
            this.boxDescription.Name = "boxDescription";
            this.boxDescription.Size = new System.Drawing.Size(0, 13);
            this.boxDescription.TabIndex = 1;
            // 
            // btnEndTurn
            // 
            this.btnEndTurn.Location = new System.Drawing.Point(35, 237);
            this.btnEndTurn.Name = "btnEndTurn";
            this.btnEndTurn.Size = new System.Drawing.Size(75, 23);
            this.btnEndTurn.TabIndex = 2;
            this.btnEndTurn.Text = "End Turn";
            this.btnEndTurn.UseVisualStyleBackColor = true;
            this.btnEndTurn.Click += new System.EventHandler(this.btnEndTurn_Click);
            // 
            // lblTurn
            // 
            this.lblTurn.AutoSize = true;
            this.lblTurn.Location = new System.Drawing.Point(32, 210);
            this.lblTurn.Name = "lblTurn";
            this.lblTurn.Size = new System.Drawing.Size(87, 13);
            this.lblTurn.TabIndex = 3;
            this.lblTurn.Text = "Ходит 1-й игрок";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 272);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Кораблей на поле боя:";
            // 
            // txtBlueShips
            // 
            this.txtBlueShips.AutoSize = true;
            this.txtBlueShips.ForeColor = System.Drawing.Color.Blue;
            this.txtBlueShips.Location = new System.Drawing.Point(32, 295);
            this.txtBlueShips.Name = "txtBlueShips";
            this.txtBlueShips.Size = new System.Drawing.Size(0, 13);
            this.txtBlueShips.TabIndex = 6;
            // 
            // txtRedShips
            // 
            this.txtRedShips.AutoSize = true;
            this.txtRedShips.ForeColor = System.Drawing.Color.Red;
            this.txtRedShips.Location = new System.Drawing.Point(82, 295);
            this.txtRedShips.Name = "txtRedShips";
            this.txtRedShips.Size = new System.Drawing.Size(0, 13);
            this.txtRedShips.TabIndex = 7;
            // 
            // CombatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(641, 459);
            this.Controls.Add(this.txtRedShips);
            this.Controls.Add(this.txtBlueShips);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblTurn);
            this.Controls.Add(this.btnEndTurn);
            this.Controls.Add(this.boxDescription);
            this.Controls.Add(this.pictureMap);
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "CombatForm";
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label boxDescription;
        private System.Windows.Forms.Button btnEndTurn;
        private System.Windows.Forms.Label lblTurn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label txtBlueShips;
        private System.Windows.Forms.Label txtRedShips;
        public System.Windows.Forms.PictureBox pictureMap;
    }
}

