namespace motGatewayTester
{
    partial class frmDoseSchedule
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDoseSchedule));
            this.label7 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.cbDelimitedTestRecord = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbTaggedTestRecord = new System.Windows.Forms.CheckBox();
            this.rbDelete = new System.Windows.Forms.RadioButton();
            this.rbChange = new System.Windows.Forms.RadioButton();
            this.rbAdd = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.valDoseTimesQtys = new System.Windows.Forms.Label();
            this.valDoseScheduleName = new System.Windows.Forms.Label();
            this.valRxSys_LocID = new System.Windows.Forms.Label();
            this.gbSwitches = new System.Windows.Forms.GroupBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtDoseTimesQtys = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDoseScheduleName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRxSys_LocID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.gbSwitches.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(408, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 17);
            this.label7.TabIndex = 3;
            this.label7.Text = "Filename: ";
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(487, 22);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(215, 22);
            this.txtFileName.TabIndex = 2;
            // 
            // cbDelimitedTestRecord
            // 
            this.cbDelimitedTestRecord.AutoSize = true;
            this.cbDelimitedTestRecord.Location = new System.Drawing.Point(53, 24);
            this.cbDelimitedTestRecord.Name = "cbDelimitedTestRecord";
            this.cbDelimitedTestRecord.Size = new System.Drawing.Size(88, 21);
            this.cbDelimitedTestRecord.TabIndex = 0;
            this.cbDelimitedTestRecord.Text = "Delimited";
            this.cbDelimitedTestRecord.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtFileName);
            this.groupBox2.Controls.Add(this.cbTaggedTestRecord);
            this.groupBox2.Controls.Add(this.cbDelimitedTestRecord);
            this.groupBox2.Location = new System.Drawing.Point(166, 287);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(731, 56);
            this.groupBox2.TabIndex = 289;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "[ Generate Test Record ]";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 17);
            this.label4.TabIndex = 4;
            // 
            // cbTaggedTestRecord
            // 
            this.cbTaggedTestRecord.AutoSize = true;
            this.cbTaggedTestRecord.Location = new System.Drawing.Point(224, 24);
            this.cbTaggedTestRecord.Name = "cbTaggedTestRecord";
            this.cbTaggedTestRecord.Size = new System.Drawing.Size(79, 21);
            this.cbTaggedTestRecord.TabIndex = 1;
            this.cbTaggedTestRecord.Text = "Tagged";
            this.cbTaggedTestRecord.UseVisualStyleBackColor = true;
            // 
            // rbDelete
            // 
            this.rbDelete.AutoSize = true;
            this.rbDelete.Location = new System.Drawing.Point(241, 22);
            this.rbDelete.Name = "rbDelete";
            this.rbDelete.Size = new System.Drawing.Size(70, 21);
            this.rbDelete.TabIndex = 2;
            this.rbDelete.TabStop = true;
            this.rbDelete.Text = "Delete";
            this.rbDelete.UseVisualStyleBackColor = true;
            // 
            // rbChange
            // 
            this.rbChange.AutoSize = true;
            this.rbChange.Location = new System.Drawing.Point(134, 21);
            this.rbChange.Name = "rbChange";
            this.rbChange.Size = new System.Drawing.Size(78, 21);
            this.rbChange.TabIndex = 1;
            this.rbChange.TabStop = true;
            this.rbChange.Text = "Change";
            this.rbChange.UseVisualStyleBackColor = true;
            // 
            // rbAdd
            // 
            this.rbAdd.AutoSize = true;
            this.rbAdd.Checked = true;
            this.rbAdd.Location = new System.Drawing.Point(29, 22);
            this.rbAdd.Name = "rbAdd";
            this.rbAdd.Size = new System.Drawing.Size(54, 21);
            this.rbAdd.TabIndex = 0;
            this.rbAdd.TabStop = true;
            this.rbAdd.Text = "Add";
            this.rbAdd.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(30, 23);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(161, 21);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Auto Truncate Fields";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // valDoseTimesQtys
            // 
            this.valDoseTimesQtys.AutoSize = true;
            this.valDoseTimesQtys.Location = new System.Drawing.Point(755, 210);
            this.valDoseTimesQtys.Name = "valDoseTimesQtys";
            this.valDoseTimesQtys.Size = new System.Drawing.Size(16, 17);
            this.valDoseTimesQtys.TabIndex = 281;
            this.valDoseTimesQtys.Text = "0";
            // 
            // valDoseScheduleName
            // 
            this.valDoseScheduleName.AutoSize = true;
            this.valDoseScheduleName.Location = new System.Drawing.Point(755, 182);
            this.valDoseScheduleName.Name = "valDoseScheduleName";
            this.valDoseScheduleName.Size = new System.Drawing.Size(16, 17);
            this.valDoseScheduleName.TabIndex = 280;
            this.valDoseScheduleName.Text = "0";
            // 
            // valRxSys_LocID
            // 
            this.valRxSys_LocID.AutoSize = true;
            this.valRxSys_LocID.Location = new System.Drawing.Point(755, 154);
            this.valRxSys_LocID.Name = "valRxSys_LocID";
            this.valRxSys_LocID.Size = new System.Drawing.Size(16, 17);
            this.valRxSys_LocID.TabIndex = 279;
            this.valRxSys_LocID.Text = "0";
            // 
            // gbSwitches
            // 
            this.gbSwitches.Controls.Add(this.checkBox1);
            this.gbSwitches.Location = new System.Drawing.Point(482, 35);
            this.gbSwitches.Name = "gbSwitches";
            this.gbSwitches.Size = new System.Drawing.Size(393, 63);
            this.gbSwitches.TabIndex = 278;
            this.gbSwitches.TabStop = false;
            this.gbSwitches.Text = "[ Options ]";
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(886, 35);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(189, 63);
            this.btnGo.TabIndex = 277;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtDoseTimesQtys
            // 
            this.txtDoseTimesQtys.Location = new System.Drawing.Point(422, 205);
            this.txtDoseTimesQtys.Name = "txtDoseTimesQtys";
            this.txtDoseTimesQtys.Size = new System.Drawing.Size(319, 22);
            this.txtDoseTimesQtys.TabIndex = 262;
            this.txtDoseTimesQtys.TextChanged += new System.EventHandler(this.DoseTimesQtys_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(271, 205);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 17);
            this.label3.TabIndex = 261;
            this.label3.Text = "DoseTimes and Qtys";
            // 
            // txtDoseScheduleName
            // 
            this.txtDoseScheduleName.Location = new System.Drawing.Point(422, 177);
            this.txtDoseScheduleName.Name = "txtDoseScheduleName";
            this.txtDoseScheduleName.Size = new System.Drawing.Size(319, 22);
            this.txtDoseScheduleName.TabIndex = 260;
            this.txtDoseScheduleName.TextChanged += new System.EventHandler(this.txtDoseScheduleName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 177);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 17);
            this.label2.TabIndex = 259;
            this.label2.Text = "Dose Schedule Name";
            // 
            // txtRxSys_LocID
            // 
            this.txtRxSys_LocID.Location = new System.Drawing.Point(422, 149);
            this.txtRxSys_LocID.Name = "txtRxSys_LocID";
            this.txtRxSys_LocID.Size = new System.Drawing.Size(319, 22);
            this.txtRxSys_LocID.TabIndex = 258;
            this.txtRxSys_LocID.TextChanged += new System.EventHandler(this.txtRxSys_LocID_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(271, 149);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 17);
            this.label1.TabIndex = 257;
            this.label1.Text = "RxSys_LocID";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDelete);
            this.groupBox1.Controls.Add(this.rbChange);
            this.groupBox1.Controls.Add(this.rbAdd);
            this.groupBox1.Location = new System.Drawing.Point(83, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(393, 63);
            this.groupBox1.TabIndex = 256;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "[ Action ]";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(271, 242);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(525, 17);
            this.label5.TabIndex = 290;
            this.label5.Text = "HHMM00.00 Where HHMM is the Hour and Minute and 00.00 is the Dose Quantity";
            // 
            // frmDoseSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1159, 389);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.valDoseTimesQtys);
            this.Controls.Add(this.valDoseScheduleName);
            this.Controls.Add(this.valRxSys_LocID);
            this.Controls.Add(this.gbSwitches);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtDoseTimesQtys);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDoseScheduleName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRxSys_LocID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDoseSchedule";
            this.Text = "frmDoseSchedule";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbSwitches.ResumeLayout(false);
            this.gbSwitches.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.CheckBox cbDelimitedTestRecord;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbTaggedTestRecord;
        private System.Windows.Forms.RadioButton rbDelete;
        private System.Windows.Forms.RadioButton rbChange;
        private System.Windows.Forms.RadioButton rbAdd;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label valDoseTimesQtys;
        private System.Windows.Forms.Label valDoseScheduleName;
        private System.Windows.Forms.Label valRxSys_LocID;
        private System.Windows.Forms.GroupBox gbSwitches;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtDoseTimesQtys;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDoseScheduleName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRxSys_LocID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}