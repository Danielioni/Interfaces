namespace motGatewayTester
{
    partial class frmSynMed
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
            this.components = new System.ComponentModel.Container();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtPatientLastName = new System.Windows.Forms.TextBox();
            this.txtPatientDOB = new System.Windows.Forms.TextBox();
            this.txtCycleStartDate = new System.Windows.Forms.TextBox();
            this.txtCycleLength = new System.Windows.Forms.TextBox();
            this.txtOutFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPatientFirstName = new System.Windows.Forms.TextBox();
            this.txtPatientMI = new System.Windows.Forms.TextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(689, 359);
            this.btnGo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(249, 110);
            this.btnGo.TabIndex = 0;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtPatientLastName
            // 
            this.txtPatientLastName.Location = new System.Drawing.Point(180, 94);
            this.txtPatientLastName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPatientLastName.Name = "txtPatientLastName";
            this.txtPatientLastName.Size = new System.Drawing.Size(99, 22);
            this.txtPatientLastName.TabIndex = 1;
            // 
            // txtPatientDOB
            // 
            this.txtPatientDOB.Location = new System.Drawing.Point(180, 126);
            this.txtPatientDOB.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPatientDOB.Name = "txtPatientDOB";
            this.txtPatientDOB.Size = new System.Drawing.Size(244, 22);
            this.txtPatientDOB.TabIndex = 2;
            // 
            // txtCycleStartDate
            // 
            this.txtCycleStartDate.Location = new System.Drawing.Point(180, 158);
            this.txtCycleStartDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCycleStartDate.Name = "txtCycleStartDate";
            this.txtCycleStartDate.Size = new System.Drawing.Size(244, 22);
            this.txtCycleStartDate.TabIndex = 3;
            // 
            // txtCycleLength
            // 
            this.txtCycleLength.Location = new System.Drawing.Point(180, 190);
            this.txtCycleLength.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCycleLength.Name = "txtCycleLength";
            this.txtCycleLength.Size = new System.Drawing.Size(244, 22);
            this.txtCycleLength.TabIndex = 4;
            // 
            // txtOutFile
            // 
            this.txtOutFile.Location = new System.Drawing.Point(180, 222);
            this.txtOutFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtOutFile.Name = "txtOutFile";
            this.txtOutFile.Size = new System.Drawing.Size(244, 22);
            this.txtOutFile.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(79, 97);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Patient Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(85, 129);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Patient DOB";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(61, 161);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Cycle Start Date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(81, 193);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "Cycle Length";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(116, 225);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "Out File";
            // 
            // txtPatientFirstName
            // 
            this.txtPatientFirstName.Location = new System.Drawing.Point(287, 94);
            this.txtPatientFirstName.Margin = new System.Windows.Forms.Padding(4);
            this.txtPatientFirstName.Name = "txtPatientFirstName";
            this.txtPatientFirstName.Size = new System.Drawing.Size(99, 22);
            this.txtPatientFirstName.TabIndex = 11;
            // 
            // txtPatientMI
            // 
            this.txtPatientMI.Location = new System.Drawing.Point(394, 94);
            this.txtPatientMI.Margin = new System.Windows.Forms.Padding(4);
            this.txtPatientMI.Name = "txtPatientMI";
            this.txtPatientMI.Size = new System.Drawing.Size(26, 22);
            this.txtPatientMI.TabIndex = 12;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // frmSynMed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 510);
            this.Controls.Add(this.txtPatientMI);
            this.Controls.Add(this.txtPatientFirstName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutFile);
            this.Controls.Add(this.txtCycleLength);
            this.Controls.Add(this.txtCycleStartDate);
            this.Controls.Add(this.txtPatientDOB);
            this.Controls.Add(this.txtPatientLastName);
            this.Controls.Add(this.btnGo);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmSynMed";
            this.Text = "frmSynMed";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtPatientLastName;
        private System.Windows.Forms.TextBox txtPatientDOB;
        private System.Windows.Forms.TextBox txtCycleStartDate;
        private System.Windows.Forms.TextBox txtCycleLength;
        private System.Windows.Forms.TextBox txtOutFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPatientFirstName;
        private System.Windows.Forms.TextBox txtPatientMI;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}