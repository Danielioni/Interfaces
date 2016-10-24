namespace motGatewayTester
{
    partial class frmLocationRecord
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLocationRecord));
            this.label7 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.cbDelimitedTestRecord = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbTaggedTestRecord = new System.Windows.Forms.CheckBox();
            this.rbDelete = new System.Windows.Forms.RadioButton();
            this.rbChange = new System.Windows.Forms.RadioButton();
            this.rbAdd = new System.Windows.Forms.RadioButton();
            this.chkAutoTruncate = new System.Windows.Forms.CheckBox();
            this.valCycleDays = new System.Windows.Forms.Label();
            this.valComment = new System.Windows.Forms.Label();
            this.valPhone = new System.Windows.Forms.Label();
            this.valZip = new System.Windows.Forms.Label();
            this.valState = new System.Windows.Forms.Label();
            this.valCity = new System.Windows.Forms.Label();
            this.valAddress2 = new System.Windows.Forms.Label();
            this.valAddress1 = new System.Windows.Forms.Label();
            this.valLocName = new System.Windows.Forms.Label();
            this.valRxSys_StoreID = new System.Windows.Forms.Label();
            this.gbSwitches = new System.Windows.Forms.GroupBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtCycleDays = new System.Windows.Forms.TextBox();
            this.CycleDays = new System.Windows.Forms.Label();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtZip = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtState = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtCity = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtAddress2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAddress1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLocName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRxSys_StoreID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.valRxSys_LocID = new System.Windows.Forms.Label();
            this.txtRxSys_LocID = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.valCycleType = new System.Windows.Forms.Label();
            this.txtCycleType = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
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
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtFileName);
            this.groupBox2.Controls.Add(this.cbTaggedTestRecord);
            this.groupBox2.Controls.Add(this.cbDelimitedTestRecord);
            this.groupBox2.Location = new System.Drawing.Point(154, 513);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(731, 56);
            this.groupBox2.TabIndex = 289;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "[ Generate Test Record ]";
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
            this.rbDelete.CheckedChanged += new System.EventHandler(this.rbDelete_CheckedChanged);
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
            this.rbChange.CheckedChanged += new System.EventHandler(this.rbChange_CheckedChanged);
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
            this.rbAdd.CheckedChanged += new System.EventHandler(this.rbAdd_CheckedChanged);
            // 
            // chkAutoTruncate
            // 
            this.chkAutoTruncate.AutoSize = true;
            this.chkAutoTruncate.Location = new System.Drawing.Point(30, 23);
            this.chkAutoTruncate.Name = "chkAutoTruncate";
            this.chkAutoTruncate.Size = new System.Drawing.Size(161, 21);
            this.chkAutoTruncate.TabIndex = 0;
            this.chkAutoTruncate.Text = "Auto Truncate Fields";
            this.chkAutoTruncate.UseVisualStyleBackColor = true;
            this.chkAutoTruncate.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // valCycleDays
            // 
            this.valCycleDays.AutoSize = true;
            this.valCycleDays.Location = new System.Drawing.Point(692, 435);
            this.valCycleDays.Name = "valCycleDays";
            this.valCycleDays.Size = new System.Drawing.Size(16, 17);
            this.valCycleDays.TabIndex = 288;
            this.valCycleDays.Text = "0";
            // 
            // valComment
            // 
            this.valComment.AutoSize = true;
            this.valComment.Location = new System.Drawing.Point(692, 407);
            this.valComment.Name = "valComment";
            this.valComment.Size = new System.Drawing.Size(16, 17);
            this.valComment.TabIndex = 287;
            this.valComment.Text = "0";
            // 
            // valPhone
            // 
            this.valPhone.AutoSize = true;
            this.valPhone.Location = new System.Drawing.Point(692, 379);
            this.valPhone.Name = "valPhone";
            this.valPhone.Size = new System.Drawing.Size(16, 17);
            this.valPhone.TabIndex = 286;
            this.valPhone.Text = "0";
            // 
            // valZip
            // 
            this.valZip.AutoSize = true;
            this.valZip.Location = new System.Drawing.Point(692, 351);
            this.valZip.Name = "valZip";
            this.valZip.Size = new System.Drawing.Size(16, 17);
            this.valZip.TabIndex = 285;
            this.valZip.Text = "0";
            // 
            // valState
            // 
            this.valState.AutoSize = true;
            this.valState.Location = new System.Drawing.Point(692, 323);
            this.valState.Name = "valState";
            this.valState.Size = new System.Drawing.Size(16, 17);
            this.valState.TabIndex = 284;
            this.valState.Text = "0";
            // 
            // valCity
            // 
            this.valCity.AutoSize = true;
            this.valCity.Location = new System.Drawing.Point(692, 295);
            this.valCity.Name = "valCity";
            this.valCity.Size = new System.Drawing.Size(16, 17);
            this.valCity.TabIndex = 283;
            this.valCity.Text = "0";
            // 
            // valAddress2
            // 
            this.valAddress2.AutoSize = true;
            this.valAddress2.Location = new System.Drawing.Point(692, 265);
            this.valAddress2.Name = "valAddress2";
            this.valAddress2.Size = new System.Drawing.Size(16, 17);
            this.valAddress2.TabIndex = 282;
            this.valAddress2.Text = "0";
            // 
            // valAddress1
            // 
            this.valAddress1.AutoSize = true;
            this.valAddress1.Location = new System.Drawing.Point(692, 237);
            this.valAddress1.Name = "valAddress1";
            this.valAddress1.Size = new System.Drawing.Size(16, 17);
            this.valAddress1.TabIndex = 281;
            this.valAddress1.Text = "0";
            // 
            // valLocName
            // 
            this.valLocName.AutoSize = true;
            this.valLocName.Location = new System.Drawing.Point(692, 209);
            this.valLocName.Name = "valLocName";
            this.valLocName.Size = new System.Drawing.Size(16, 17);
            this.valLocName.TabIndex = 280;
            this.valLocName.Text = "0";
            // 
            // valRxSys_StoreID
            // 
            this.valRxSys_StoreID.AutoSize = true;
            this.valRxSys_StoreID.Location = new System.Drawing.Point(692, 151);
            this.valRxSys_StoreID.Name = "valRxSys_StoreID";
            this.valRxSys_StoreID.Size = new System.Drawing.Size(16, 17);
            this.valRxSys_StoreID.TabIndex = 279;
            this.valRxSys_StoreID.Text = "0";
            // 
            // gbSwitches
            // 
            this.gbSwitches.Controls.Add(this.chkAutoTruncate);
            this.gbSwitches.Location = new System.Drawing.Point(419, 21);
            this.gbSwitches.Name = "gbSwitches";
            this.gbSwitches.Size = new System.Drawing.Size(393, 63);
            this.gbSwitches.TabIndex = 278;
            this.gbSwitches.TabStop = false;
            this.gbSwitches.Text = "[ Options ]";
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(823, 21);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(189, 63);
            this.btnGo.TabIndex = 277;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtCycleDays
            // 
            this.txtCycleDays.Location = new System.Drawing.Point(359, 430);
            this.txtCycleDays.Name = "txtCycleDays";
            this.txtCycleDays.Size = new System.Drawing.Size(319, 22);
            this.txtCycleDays.TabIndex = 276;
            this.txtCycleDays.TextChanged += new System.EventHandler(this.txtCycleDays_TextChanged);
            // 
            // CycleDays
            // 
            this.CycleDays.AutoSize = true;
            this.CycleDays.Location = new System.Drawing.Point(208, 428);
            this.CycleDays.Name = "CycleDays";
            this.CycleDays.Size = new System.Drawing.Size(74, 17);
            this.CycleDays.TabIndex = 275;
            this.CycleDays.Text = "CycleDays";
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(359, 402);
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(319, 22);
            this.txtComment.TabIndex = 274;
            this.txtComment.TextChanged += new System.EventHandler(this.txtComment_TextChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(208, 402);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(74, 17);
            this.label19.TabIndex = 273;
            this.label19.Text = "Comments";
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(359, 374);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(319, 22);
            this.txtPhone.TabIndex = 272;
            this.txtPhone.TextChanged += new System.EventHandler(this.txtPhone_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(208, 374);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(49, 17);
            this.label18.TabIndex = 271;
            this.label18.Text = "Phone";
            // 
            // txtZip
            // 
            this.txtZip.Location = new System.Drawing.Point(359, 346);
            this.txtZip.Name = "txtZip";
            this.txtZip.Size = new System.Drawing.Size(319, 22);
            this.txtZip.TabIndex = 270;
            this.txtZip.TextChanged += new System.EventHandler(this.txtCity_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(208, 346);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(28, 17);
            this.label17.TabIndex = 269;
            this.label17.Text = "Zip";
            // 
            // txtState
            // 
            this.txtState.Location = new System.Drawing.Point(359, 318);
            this.txtState.Name = "txtState";
            this.txtState.Size = new System.Drawing.Size(319, 22);
            this.txtState.TabIndex = 268;
            this.txtState.TextChanged += new System.EventHandler(this.txtState_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(208, 318);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(41, 17);
            this.label16.TabIndex = 267;
            this.label16.Text = "State";
            // 
            // txtCity
            // 
            this.txtCity.Location = new System.Drawing.Point(359, 290);
            this.txtCity.Multiline = true;
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(319, 22);
            this.txtCity.TabIndex = 266;
            this.txtCity.TextChanged += new System.EventHandler(this.txtCity_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(208, 290);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(31, 17);
            this.label15.TabIndex = 265;
            this.label15.Text = "City";
            // 
            // txtAddress2
            // 
            this.txtAddress2.Location = new System.Drawing.Point(359, 260);
            this.txtAddress2.Name = "txtAddress2";
            this.txtAddress2.Size = new System.Drawing.Size(319, 22);
            this.txtAddress2.TabIndex = 264;
            this.txtAddress2.TextChanged += new System.EventHandler(this.txtAddress2_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(208, 260);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 263;
            this.label4.Text = "Address2";
            // 
            // txtAddress1
            // 
            this.txtAddress1.Location = new System.Drawing.Point(359, 232);
            this.txtAddress1.Name = "txtAddress1";
            this.txtAddress1.Size = new System.Drawing.Size(319, 22);
            this.txtAddress1.TabIndex = 262;
            this.txtAddress1.TextChanged += new System.EventHandler(this.txtAddress1_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(208, 232);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 17);
            this.label3.TabIndex = 261;
            this.label3.Text = "Address 1";
            // 
            // txtLocName
            // 
            this.txtLocName.Location = new System.Drawing.Point(359, 204);
            this.txtLocName.Name = "txtLocName";
            this.txtLocName.Size = new System.Drawing.Size(319, 22);
            this.txtLocName.TabIndex = 260;
            this.txtLocName.TextChanged += new System.EventHandler(this.txtLocName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(208, 204);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 17);
            this.label2.TabIndex = 259;
            this.label2.Text = "Location Name";
            // 
            // txtRxSys_StoreID
            // 
            this.txtRxSys_StoreID.Location = new System.Drawing.Point(359, 146);
            this.txtRxSys_StoreID.Name = "txtRxSys_StoreID";
            this.txtRxSys_StoreID.Size = new System.Drawing.Size(319, 22);
            this.txtRxSys_StoreID.TabIndex = 258;
            this.txtRxSys_StoreID.TextChanged += new System.EventHandler(this.txtRxSys_StoreID_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(208, 146);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 17);
            this.label1.TabIndex = 257;
            this.label1.Text = "RxSys_StoreID";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDelete);
            this.groupBox1.Controls.Add(this.rbChange);
            this.groupBox1.Controls.Add(this.rbAdd);
            this.groupBox1.Location = new System.Drawing.Point(20, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(393, 63);
            this.groupBox1.TabIndex = 256;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "[ Action ]";
            // 
            // valRxSys_LocID
            // 
            this.valRxSys_LocID.AutoSize = true;
            this.valRxSys_LocID.Location = new System.Drawing.Point(692, 179);
            this.valRxSys_LocID.Name = "valRxSys_LocID";
            this.valRxSys_LocID.Size = new System.Drawing.Size(16, 17);
            this.valRxSys_LocID.TabIndex = 292;
            this.valRxSys_LocID.Text = "0";
            // 
            // txtRxSys_LocID
            // 
            this.txtRxSys_LocID.Location = new System.Drawing.Point(359, 174);
            this.txtRxSys_LocID.Name = "txtRxSys_LocID";
            this.txtRxSys_LocID.Size = new System.Drawing.Size(319, 22);
            this.txtRxSys_LocID.TabIndex = 291;
            this.txtRxSys_LocID.TextChanged += new System.EventHandler(this.txtRxSys_LocID_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(208, 174);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 17);
            this.label6.TabIndex = 290;
            this.label6.Text = "RxSys_LocID";
            // 
            // valCycleType
            // 
            this.valCycleType.AutoSize = true;
            this.valCycleType.Location = new System.Drawing.Point(692, 463);
            this.valCycleType.Name = "valCycleType";
            this.valCycleType.Size = new System.Drawing.Size(16, 17);
            this.valCycleType.TabIndex = 295;
            this.valCycleType.Text = "0";
            // 
            // txtCycleType
            // 
            this.txtCycleType.Location = new System.Drawing.Point(359, 458);
            this.txtCycleType.Name = "txtCycleType";
            this.txtCycleType.Size = new System.Drawing.Size(319, 22);
            this.txtCycleType.TabIndex = 294;
            this.txtCycleType.TextChanged += new System.EventHandler(this.txtCycleType_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(208, 456);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 17);
            this.label8.TabIndex = 293;
            this.label8.Text = "CycleType";
            // 
            // frmLocationRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1036, 592);
            this.Controls.Add(this.valCycleType);
            this.Controls.Add(this.txtCycleType);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.valRxSys_LocID);
            this.Controls.Add(this.txtRxSys_LocID);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.valCycleDays);
            this.Controls.Add(this.valComment);
            this.Controls.Add(this.valPhone);
            this.Controls.Add(this.valZip);
            this.Controls.Add(this.valState);
            this.Controls.Add(this.valCity);
            this.Controls.Add(this.valAddress2);
            this.Controls.Add(this.valAddress1);
            this.Controls.Add(this.valLocName);
            this.Controls.Add(this.valRxSys_StoreID);
            this.Controls.Add(this.gbSwitches);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtCycleDays);
            this.Controls.Add(this.CycleDays);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.txtZip);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txtState);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.txtCity);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txtAddress2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtAddress1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtLocName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRxSys_StoreID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmLocationRecord";
            this.Text = "Location/Facility Record";
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
        private System.Windows.Forms.CheckBox chkAutoTruncate;
        private System.Windows.Forms.Label valCycleDays;
        private System.Windows.Forms.Label valComment;
        private System.Windows.Forms.Label valPhone;
        private System.Windows.Forms.Label valZip;
        private System.Windows.Forms.Label valState;
        private System.Windows.Forms.Label valCity;
        private System.Windows.Forms.Label valAddress2;
        private System.Windows.Forms.Label valAddress1;
        private System.Windows.Forms.Label valLocName;
        private System.Windows.Forms.Label valRxSys_StoreID;
        private System.Windows.Forms.GroupBox gbSwitches;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtCycleDays;
        private System.Windows.Forms.Label CycleDays;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtZip;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtState;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtCity;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtAddress2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAddress1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLocName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRxSys_StoreID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label valRxSys_LocID;
        private System.Windows.Forms.TextBox txtRxSys_LocID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label valCycleType;
        private System.Windows.Forms.TextBox txtCycleType;
        private System.Windows.Forms.Label label8;
    }
}