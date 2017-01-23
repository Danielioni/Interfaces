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
            this.cbSendTaggedRecord = new System.Windows.Forms.CheckBox();
            this.cbSendDelimitedRecord = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
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
            this.chkSendEOF = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.gbSwitches.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(306, 19);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Filename: ";
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(365, 18);
            this.txtFileName.Margin = new System.Windows.Forms.Padding(2);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(162, 20);
            this.txtFileName.TabIndex = 2;
            // 
            // cbDelimitedTestRecord
            // 
            this.cbDelimitedTestRecord.AutoSize = true;
            this.cbDelimitedTestRecord.Location = new System.Drawing.Point(93, 21);
            this.cbDelimitedTestRecord.Margin = new System.Windows.Forms.Padding(2);
            this.cbDelimitedTestRecord.Name = "cbDelimitedTestRecord";
            this.cbDelimitedTestRecord.Size = new System.Drawing.Size(69, 17);
            this.cbDelimitedTestRecord.TabIndex = 0;
            this.cbDelimitedTestRecord.Text = "Delimited";
            this.cbDelimitedTestRecord.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbSendTaggedRecord);
            this.groupBox2.Controls.Add(this.cbSendDelimitedRecord);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtFileName);
            this.groupBox2.Controls.Add(this.cbTaggedTestRecord);
            this.groupBox2.Controls.Add(this.cbDelimitedTestRecord);
            this.groupBox2.Location = new System.Drawing.Point(116, 406);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(548, 68);
            this.groupBox2.TabIndex = 289;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "[ Generate Test Record ]";
            // 
            // cbSendTaggedRecord
            // 
            this.cbSendTaggedRecord.AutoSize = true;
            this.cbSendTaggedRecord.Location = new System.Drawing.Point(185, 42);
            this.cbSendTaggedRecord.Margin = new System.Windows.Forms.Padding(2);
            this.cbSendTaggedRecord.Name = "cbSendTaggedRecord";
            this.cbSendTaggedRecord.Size = new System.Drawing.Size(63, 17);
            this.cbSendTaggedRecord.TabIndex = 11;
            this.cbSendTaggedRecord.Text = "Tagged";
            this.cbSendTaggedRecord.UseVisualStyleBackColor = true;
            // 
            // cbSendDelimitedRecord
            // 
            this.cbSendDelimitedRecord.AutoSize = true;
            this.cbSendDelimitedRecord.Location = new System.Drawing.Point(93, 42);
            this.cbSendDelimitedRecord.Margin = new System.Windows.Forms.Padding(2);
            this.cbSendDelimitedRecord.Name = "cbSendDelimitedRecord";
            this.cbSendDelimitedRecord.Size = new System.Drawing.Size(69, 17);
            this.cbSendDelimitedRecord.TabIndex = 10;
            this.cbSendDelimitedRecord.Text = "Delimited";
            this.cbSendDelimitedRecord.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Send Record";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Create File";
            // 
            // cbTaggedTestRecord
            // 
            this.cbTaggedTestRecord.AutoSize = true;
            this.cbTaggedTestRecord.Location = new System.Drawing.Point(185, 20);
            this.cbTaggedTestRecord.Margin = new System.Windows.Forms.Padding(2);
            this.cbTaggedTestRecord.Name = "cbTaggedTestRecord";
            this.cbTaggedTestRecord.Size = new System.Drawing.Size(63, 17);
            this.cbTaggedTestRecord.TabIndex = 1;
            this.cbTaggedTestRecord.Text = "Tagged";
            this.cbTaggedTestRecord.UseVisualStyleBackColor = true;
            // 
            // rbDelete
            // 
            this.rbDelete.AutoSize = true;
            this.rbDelete.Location = new System.Drawing.Point(181, 18);
            this.rbDelete.Margin = new System.Windows.Forms.Padding(2);
            this.rbDelete.Name = "rbDelete";
            this.rbDelete.Size = new System.Drawing.Size(56, 17);
            this.rbDelete.TabIndex = 2;
            this.rbDelete.TabStop = true;
            this.rbDelete.Text = "Delete";
            this.rbDelete.UseVisualStyleBackColor = true;
            this.rbDelete.CheckedChanged += new System.EventHandler(this.rbDelete_CheckedChanged);
            // 
            // rbChange
            // 
            this.rbChange.AutoSize = true;
            this.rbChange.Location = new System.Drawing.Point(100, 17);
            this.rbChange.Margin = new System.Windows.Forms.Padding(2);
            this.rbChange.Name = "rbChange";
            this.rbChange.Size = new System.Drawing.Size(62, 17);
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
            this.rbAdd.Location = new System.Drawing.Point(22, 18);
            this.rbAdd.Margin = new System.Windows.Forms.Padding(2);
            this.rbAdd.Name = "rbAdd";
            this.rbAdd.Size = new System.Drawing.Size(44, 17);
            this.rbAdd.TabIndex = 0;
            this.rbAdd.TabStop = true;
            this.rbAdd.Text = "Add";
            this.rbAdd.UseVisualStyleBackColor = true;
            this.rbAdd.CheckedChanged += new System.EventHandler(this.rbAdd_CheckedChanged);
            // 
            // chkAutoTruncate
            // 
            this.chkAutoTruncate.AutoSize = true;
            this.chkAutoTruncate.Location = new System.Drawing.Point(22, 19);
            this.chkAutoTruncate.Margin = new System.Windows.Forms.Padding(2);
            this.chkAutoTruncate.Name = "chkAutoTruncate";
            this.chkAutoTruncate.Size = new System.Drawing.Size(124, 17);
            this.chkAutoTruncate.TabIndex = 0;
            this.chkAutoTruncate.Text = "Auto Truncate Fields";
            this.chkAutoTruncate.UseVisualStyleBackColor = true;
            this.chkAutoTruncate.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // valCycleDays
            // 
            this.valCycleDays.AutoSize = true;
            this.valCycleDays.Location = new System.Drawing.Point(519, 353);
            this.valCycleDays.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valCycleDays.Name = "valCycleDays";
            this.valCycleDays.Size = new System.Drawing.Size(13, 13);
            this.valCycleDays.TabIndex = 288;
            this.valCycleDays.Text = "0";
            // 
            // valComment
            // 
            this.valComment.AutoSize = true;
            this.valComment.Location = new System.Drawing.Point(519, 331);
            this.valComment.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valComment.Name = "valComment";
            this.valComment.Size = new System.Drawing.Size(13, 13);
            this.valComment.TabIndex = 287;
            this.valComment.Text = "0";
            // 
            // valPhone
            // 
            this.valPhone.AutoSize = true;
            this.valPhone.Location = new System.Drawing.Point(519, 308);
            this.valPhone.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valPhone.Name = "valPhone";
            this.valPhone.Size = new System.Drawing.Size(13, 13);
            this.valPhone.TabIndex = 286;
            this.valPhone.Text = "0";
            // 
            // valZip
            // 
            this.valZip.AutoSize = true;
            this.valZip.Location = new System.Drawing.Point(519, 285);
            this.valZip.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valZip.Name = "valZip";
            this.valZip.Size = new System.Drawing.Size(13, 13);
            this.valZip.TabIndex = 285;
            this.valZip.Text = "0";
            // 
            // valState
            // 
            this.valState.AutoSize = true;
            this.valState.Location = new System.Drawing.Point(519, 262);
            this.valState.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valState.Name = "valState";
            this.valState.Size = new System.Drawing.Size(13, 13);
            this.valState.TabIndex = 284;
            this.valState.Text = "0";
            // 
            // valCity
            // 
            this.valCity.AutoSize = true;
            this.valCity.Location = new System.Drawing.Point(519, 240);
            this.valCity.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valCity.Name = "valCity";
            this.valCity.Size = new System.Drawing.Size(13, 13);
            this.valCity.TabIndex = 283;
            this.valCity.Text = "0";
            // 
            // valAddress2
            // 
            this.valAddress2.AutoSize = true;
            this.valAddress2.Location = new System.Drawing.Point(519, 215);
            this.valAddress2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valAddress2.Name = "valAddress2";
            this.valAddress2.Size = new System.Drawing.Size(13, 13);
            this.valAddress2.TabIndex = 282;
            this.valAddress2.Text = "0";
            // 
            // valAddress1
            // 
            this.valAddress1.AutoSize = true;
            this.valAddress1.Location = new System.Drawing.Point(519, 193);
            this.valAddress1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valAddress1.Name = "valAddress1";
            this.valAddress1.Size = new System.Drawing.Size(13, 13);
            this.valAddress1.TabIndex = 281;
            this.valAddress1.Text = "0";
            // 
            // valLocName
            // 
            this.valLocName.AutoSize = true;
            this.valLocName.Location = new System.Drawing.Point(519, 170);
            this.valLocName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valLocName.Name = "valLocName";
            this.valLocName.Size = new System.Drawing.Size(13, 13);
            this.valLocName.TabIndex = 280;
            this.valLocName.Text = "0";
            // 
            // valRxSys_StoreID
            // 
            this.valRxSys_StoreID.AutoSize = true;
            this.valRxSys_StoreID.Location = new System.Drawing.Point(519, 123);
            this.valRxSys_StoreID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valRxSys_StoreID.Name = "valRxSys_StoreID";
            this.valRxSys_StoreID.Size = new System.Drawing.Size(13, 13);
            this.valRxSys_StoreID.TabIndex = 279;
            this.valRxSys_StoreID.Text = "0";
            // 
            // gbSwitches
            // 
            this.gbSwitches.Controls.Add(this.chkSendEOF);
            this.gbSwitches.Controls.Add(this.chkAutoTruncate);
            this.gbSwitches.Location = new System.Drawing.Point(314, 17);
            this.gbSwitches.Margin = new System.Windows.Forms.Padding(2);
            this.gbSwitches.Name = "gbSwitches";
            this.gbSwitches.Padding = new System.Windows.Forms.Padding(2);
            this.gbSwitches.Size = new System.Drawing.Size(295, 51);
            this.gbSwitches.TabIndex = 278;
            this.gbSwitches.TabStop = false;
            this.gbSwitches.Text = "[ Options ]";
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(617, 17);
            this.btnGo.Margin = new System.Windows.Forms.Padding(2);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(142, 51);
            this.btnGo.TabIndex = 277;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtCycleDays
            // 
            this.txtCycleDays.Location = new System.Drawing.Point(269, 349);
            this.txtCycleDays.Margin = new System.Windows.Forms.Padding(2);
            this.txtCycleDays.Name = "txtCycleDays";
            this.txtCycleDays.Size = new System.Drawing.Size(240, 20);
            this.txtCycleDays.TabIndex = 276;
            this.txtCycleDays.TextChanged += new System.EventHandler(this.txtCycleDays_TextChanged);
            // 
            // CycleDays
            // 
            this.CycleDays.AutoSize = true;
            this.CycleDays.Location = new System.Drawing.Point(156, 348);
            this.CycleDays.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.CycleDays.Name = "CycleDays";
            this.CycleDays.Size = new System.Drawing.Size(57, 13);
            this.CycleDays.TabIndex = 275;
            this.CycleDays.Text = "CycleDays";
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(269, 327);
            this.txtComment.Margin = new System.Windows.Forms.Padding(2);
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(240, 20);
            this.txtComment.TabIndex = 274;
            this.txtComment.TextChanged += new System.EventHandler(this.txtComment_TextChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(156, 327);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(56, 13);
            this.label19.TabIndex = 273;
            this.label19.Text = "Comments";
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(269, 304);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(2);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(240, 20);
            this.txtPhone.TabIndex = 272;
            this.txtPhone.TextChanged += new System.EventHandler(this.txtPhone_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(156, 304);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(38, 13);
            this.label18.TabIndex = 271;
            this.label18.Text = "Phone";
            // 
            // txtZip
            // 
            this.txtZip.Location = new System.Drawing.Point(269, 281);
            this.txtZip.Margin = new System.Windows.Forms.Padding(2);
            this.txtZip.Name = "txtZip";
            this.txtZip.Size = new System.Drawing.Size(240, 20);
            this.txtZip.TabIndex = 270;
            this.txtZip.TextChanged += new System.EventHandler(this.txtCity_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(156, 281);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(22, 13);
            this.label17.TabIndex = 269;
            this.label17.Text = "Zip";
            // 
            // txtState
            // 
            this.txtState.Location = new System.Drawing.Point(269, 258);
            this.txtState.Margin = new System.Windows.Forms.Padding(2);
            this.txtState.Name = "txtState";
            this.txtState.Size = new System.Drawing.Size(240, 20);
            this.txtState.TabIndex = 268;
            this.txtState.TextChanged += new System.EventHandler(this.txtState_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(156, 258);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(32, 13);
            this.label16.TabIndex = 267;
            this.label16.Text = "State";
            // 
            // txtCity
            // 
            this.txtCity.Location = new System.Drawing.Point(269, 236);
            this.txtCity.Margin = new System.Windows.Forms.Padding(2);
            this.txtCity.Multiline = true;
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(240, 19);
            this.txtCity.TabIndex = 266;
            this.txtCity.TextChanged += new System.EventHandler(this.txtCity_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(156, 236);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(24, 13);
            this.label15.TabIndex = 265;
            this.label15.Text = "City";
            // 
            // txtAddress2
            // 
            this.txtAddress2.Location = new System.Drawing.Point(269, 211);
            this.txtAddress2.Margin = new System.Windows.Forms.Padding(2);
            this.txtAddress2.Name = "txtAddress2";
            this.txtAddress2.Size = new System.Drawing.Size(240, 20);
            this.txtAddress2.TabIndex = 264;
            this.txtAddress2.TextChanged += new System.EventHandler(this.txtAddress2_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(156, 211);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 263;
            this.label4.Text = "Address2";
            // 
            // txtAddress1
            // 
            this.txtAddress1.Location = new System.Drawing.Point(269, 188);
            this.txtAddress1.Margin = new System.Windows.Forms.Padding(2);
            this.txtAddress1.Name = "txtAddress1";
            this.txtAddress1.Size = new System.Drawing.Size(240, 20);
            this.txtAddress1.TabIndex = 262;
            this.txtAddress1.TextChanged += new System.EventHandler(this.txtAddress1_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(156, 188);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 261;
            this.label3.Text = "Address 1";
            // 
            // txtLocName
            // 
            this.txtLocName.Location = new System.Drawing.Point(269, 166);
            this.txtLocName.Margin = new System.Windows.Forms.Padding(2);
            this.txtLocName.Name = "txtLocName";
            this.txtLocName.Size = new System.Drawing.Size(240, 20);
            this.txtLocName.TabIndex = 260;
            this.txtLocName.TextChanged += new System.EventHandler(this.txtLocName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(156, 166);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 259;
            this.label2.Text = "Location Name";
            // 
            // txtRxSys_StoreID
            // 
            this.txtRxSys_StoreID.Location = new System.Drawing.Point(269, 119);
            this.txtRxSys_StoreID.Margin = new System.Windows.Forms.Padding(2);
            this.txtRxSys_StoreID.Name = "txtRxSys_StoreID";
            this.txtRxSys_StoreID.Size = new System.Drawing.Size(240, 20);
            this.txtRxSys_StoreID.TabIndex = 258;
            this.txtRxSys_StoreID.TextChanged += new System.EventHandler(this.txtRxSys_StoreID_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(156, 119);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 257;
            this.label1.Text = "RxSys_StoreID";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDelete);
            this.groupBox1.Controls.Add(this.rbChange);
            this.groupBox1.Controls.Add(this.rbAdd);
            this.groupBox1.Location = new System.Drawing.Point(15, 17);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(295, 51);
            this.groupBox1.TabIndex = 256;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "[ Action ]";
            // 
            // valRxSys_LocID
            // 
            this.valRxSys_LocID.AutoSize = true;
            this.valRxSys_LocID.Location = new System.Drawing.Point(519, 145);
            this.valRxSys_LocID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valRxSys_LocID.Name = "valRxSys_LocID";
            this.valRxSys_LocID.Size = new System.Drawing.Size(13, 13);
            this.valRxSys_LocID.TabIndex = 292;
            this.valRxSys_LocID.Text = "0";
            // 
            // txtRxSys_LocID
            // 
            this.txtRxSys_LocID.Location = new System.Drawing.Point(269, 141);
            this.txtRxSys_LocID.Margin = new System.Windows.Forms.Padding(2);
            this.txtRxSys_LocID.Name = "txtRxSys_LocID";
            this.txtRxSys_LocID.Size = new System.Drawing.Size(240, 20);
            this.txtRxSys_LocID.TabIndex = 291;
            this.txtRxSys_LocID.TextChanged += new System.EventHandler(this.txtRxSys_LocID_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(156, 141);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 290;
            this.label6.Text = "RxSys_LocID";
            // 
            // valCycleType
            // 
            this.valCycleType.AutoSize = true;
            this.valCycleType.Location = new System.Drawing.Point(519, 376);
            this.valCycleType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valCycleType.Name = "valCycleType";
            this.valCycleType.Size = new System.Drawing.Size(13, 13);
            this.valCycleType.TabIndex = 295;
            this.valCycleType.Text = "0";
            // 
            // txtCycleType
            // 
            this.txtCycleType.Location = new System.Drawing.Point(269, 372);
            this.txtCycleType.Margin = new System.Windows.Forms.Padding(2);
            this.txtCycleType.Name = "txtCycleType";
            this.txtCycleType.Size = new System.Drawing.Size(240, 20);
            this.txtCycleType.TabIndex = 294;
            this.txtCycleType.TextChanged += new System.EventHandler(this.txtCycleType_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(156, 370);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 293;
            this.label8.Text = "CycleType";
            // 
            // chkSendEOF
            // 
            this.chkSendEOF.AutoSize = true;
            this.chkSendEOF.Checked = true;
            this.chkSendEOF.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSendEOF.Location = new System.Drawing.Point(167, 19);
            this.chkSendEOF.Name = "chkSendEOF";
            this.chkSendEOF.Size = new System.Drawing.Size(92, 17);
            this.chkSendEOF.TabIndex = 2;
            this.chkSendEOF.Text = "Send <EOF/>";
            this.chkSendEOF.UseVisualStyleBackColor = true;
            // 
            // frmLocationRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 498);
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
            this.Margin = new System.Windows.Forms.Padding(2);
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
        private System.Windows.Forms.CheckBox cbSendTaggedRecord;
        private System.Windows.Forms.CheckBox cbSendDelimitedRecord;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chkSendEOF;
    }
}