﻿namespace HL7Tester
{
    partial class formMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeView = new System.Windows.Forms.TreeView();
            this.btnStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Location = new System.Drawing.Point(0, 34);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(634, 336);
            this.treeView.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(39, 383);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(132, 39);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 440);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.treeView);
            this.Name = "formMain";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Button btnStart;
    }
}

