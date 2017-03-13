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
            this.txtCycleStartDate = new System.Windows.Forms.TextBox();
            this.txtCycleLength = new System.Windows.Forms.TextBox();
            this.txtOutFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPatientFirstName = new System.Windows.Forms.TextBox();
            this.txtPatientMI = new System.Windows.Forms.TextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbUseLegacy = new System.Windows.Forms.RadioButton();
            this.rbUseNext = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(689, 359);
            this.btnGo.Margin = new System.Windows.Forms.Padding(4);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(249, 110);
            this.btnGo.TabIndex = 0;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtPatientLastName
            // 
            this.txtPatientLastName.Location = new System.Drawing.Point(180, 128);
            this.txtPatientLastName.Margin = new System.Windows.Forms.Padding(4);
            this.txtPatientLastName.Name = "txtPatientLastName";
            this.txtPatientLastName.Size = new System.Drawing.Size(99, 22);
            this.txtPatientLastName.TabIndex = 1;
            // 
            // txtCycleStartDate
            // 
            this.txtCycleStartDate.Location = new System.Drawing.Point(180, 158);
            this.txtCycleStartDate.Margin = new System.Windows.Forms.Padding(4);
            this.txtCycleStartDate.Name = "txtCycleStartDate";
            this.txtCycleStartDate.Size = new System.Drawing.Size(244, 22);
            this.txtCycleStartDate.TabIndex = 3;
            this.txtCycleStartDate.Text = "3/11/2017";
            this.txtCycleStartDate.TextChanged += new System.EventHandler(this.txtCycleStartDate_TextChanged);
            // 
            // txtCycleLength
            // 
            this.txtCycleLength.Location = new System.Drawing.Point(180, 190);
            this.txtCycleLength.Margin = new System.Windows.Forms.Padding(4);
            this.txtCycleLength.Name = "txtCycleLength";
            this.txtCycleLength.Size = new System.Drawing.Size(244, 22);
            this.txtCycleLength.TabIndex = 4;
            // 
            // txtOutFile
            // 
            this.txtOutFile.Location = new System.Drawing.Point(180, 222);
            this.txtOutFile.Margin = new System.Windows.Forms.Padding(4);
            this.txtOutFile.Name = "txtOutFile";
            this.txtOutFile.Size = new System.Drawing.Size(244, 22);
            this.txtOutFile.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(79, 131);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Patient Name";
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
            this.txtPatientFirstName.Location = new System.Drawing.Point(287, 128);
            this.txtPatientFirstName.Margin = new System.Windows.Forms.Padding(4);
            this.txtPatientFirstName.Name = "txtPatientFirstName";
            this.txtPatientFirstName.Size = new System.Drawing.Size(99, 22);
            this.txtPatientFirstName.TabIndex = 11;
            // 
            // txtPatientMI
            // 
            this.txtPatientMI.Location = new System.Drawing.Point(394, 128);
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
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(637, 125);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(270, 22);
            this.txtUserName.TabIndex = 13;
            this.txtUserName.Text = "dba";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(637, 153);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPassword.Size = new System.Drawing.Size(270, 22);
            this.txtPassword.TabIndex = 14;
            this.txtPassword.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(537, 125);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 17);
            this.label2.TabIndex = 15;
            this.label2.Text = "User Name";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(537, 153);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 17);
            this.label6.TabIndex = 16;
            this.label6.Text = "Password";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbUseLegacy);
            this.groupBox1.Controls.Add(this.rbUseNext);
            this.groupBox1.Location = new System.Drawing.Point(82, 328);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "[ Database ]";
            // 
            // rbUseLegacy
            // 
            this.rbUseLegacy.AutoSize = true;
            this.rbUseLegacy.Checked = true;
            this.rbUseLegacy.Location = new System.Drawing.Point(25, 59);
            this.rbUseLegacy.Name = "rbUseLegacy";
            this.rbUseLegacy.Size = new System.Drawing.Size(127, 21);
            this.rbUseLegacy.TabIndex = 1;
            this.rbUseLegacy.TabStop = true;
            this.rbUseLegacy.Text = "Use motLegacy";
            this.rbUseLegacy.UseVisualStyleBackColor = true;
            // 
            // rbUseNext
            // 
            this.rbUseNext.AutoSize = true;
            this.rbUseNext.Location = new System.Drawing.Point(25, 31);
            this.rbUseNext.Name = "rbUseNext";
            this.rbUseNext.Size = new System.Drawing.Size(109, 21);
            this.rbUseNext.TabIndex = 0;
            this.rbUseNext.Text = "Use motNext";
            this.rbUseNext.UseVisualStyleBackColor = true;
            // 
            // frmSynMed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 510);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.txtPatientMI);
            this.Controls.Add(this.txtPatientFirstName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutFile);
            this.Controls.Add(this.txtCycleLength);
            this.Controls.Add(this.txtCycleStartDate);
            this.Controls.Add(this.txtPatientLastName);
            this.Controls.Add(this.btnGo);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmSynMed";
            this.Text = "frmSynMed";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtPatientLastName;
        private System.Windows.Forms.TextBox txtCycleStartDate;
        private System.Windows.Forms.TextBox txtCycleLength;
        private System.Windows.Forms.TextBox txtOutFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPatientFirstName;
        private System.Windows.Forms.TextBox txtPatientMI;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbUseLegacy;
        private System.Windows.Forms.RadioButton rbUseNext;
    }
}