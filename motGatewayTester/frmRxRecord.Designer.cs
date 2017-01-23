namespace motGatewayTester
{
    partial class frmRxRecord
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRxRecord));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbDelete = new System.Windows.Forms.RadioButton();
            this.rbChange = new System.Windows.Forms.RadioButton();
            this.rbAdd = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRxSys_PatID = new System.Windows.Forms.TextBox();
            this.txtRxNum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRxSys_DocID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDrug_ID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSig = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtRxStartDate = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtRxStopDate = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtDiscontinueDate = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtDoseScheduleName = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtRefills = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtChartOnly = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtRxType = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtIsolate = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.txtAnchorDate = new System.Windows.Forms.TextBox();
            this.txtMDOMStart = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.txtMDOMStop = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.txtQtyPerDose = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.txtQtyDispensed = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.txtDOW = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.txtSpecialDoses = new System.Windows.Forms.TextBox();
            this.label38 = new System.Windows.Forms.Label();
            this.txtDoseTimesQtys = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.btnGo = new System.Windows.Forms.Button();
            this.gbSwitches = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.valRxSys_PatID = new System.Windows.Forms.Label();
            this.valRxSys_RxNum = new System.Windows.Forms.Label();
            this.valRxSys_DrugID = new System.Windows.Forms.Label();
            this.valRxSys_DocID = new System.Windows.Forms.Label();
            this.valRxStartDate = new System.Windows.Forms.Label();
            this.valSig = new System.Windows.Forms.Label();
            this.valRefills = new System.Windows.Forms.Label();
            this.valDoseScheduleName = new System.Windows.Forms.Label();
            this.valDiscontinueDate = new System.Windows.Forms.Label();
            this.valRxStopDate = new System.Windows.Forms.Label();
            this.valDoseTimesQtys = new System.Windows.Forms.Label();
            this.valAdmitDate = new System.Windows.Forms.Label();
            this.valDOW = new System.Windows.Forms.Label();
            this.valStatus = new System.Windows.Forms.Label();
            this.valQtyDispensed = new System.Windows.Forms.Label();
            this.valQtyPerDose = new System.Windows.Forms.Label();
            this.valMDOMStop = new System.Windows.Forms.Label();
            this.valMDOMStart = new System.Windows.Forms.Label();
            this.valAnchorDate = new System.Windows.Forms.Label();
            this.valComments = new System.Windows.Forms.Label();
            this.valIsolate = new System.Windows.Forms.Label();
            this.valRxType = new System.Windows.Forms.Label();
            this.valChartOnly = new System.Windows.Forms.Label();
            this.valRxSys_NewRxNum = new System.Windows.Forms.Label();
            this.txtRxSys_NewRxNum = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbSendTaggedRecord = new System.Windows.Forms.CheckBox();
            this.cbSendDelimitedRecord = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.cbTaggedTestRecord = new System.Windows.Forms.CheckBox();
            this.cbDelimitedTestRecord = new System.Windows.Forms.CheckBox();
            this.chkSendEOF = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.gbSwitches.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDelete);
            this.groupBox1.Controls.Add(this.rbChange);
            this.groupBox1.Controls.Add(this.rbAdd);
            this.groupBox1.Location = new System.Drawing.Point(17, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(295, 51);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "[ Action ]";
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 82);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "RxSys_PatID";
            // 
            // txtRxSys_PatID
            // 
            this.txtRxSys_PatID.Location = new System.Drawing.Point(128, 82);
            this.txtRxSys_PatID.Margin = new System.Windows.Forms.Padding(2);
            this.txtRxSys_PatID.Name = "txtRxSys_PatID";
            this.txtRxSys_PatID.Size = new System.Drawing.Size(240, 20);
            this.txtRxSys_PatID.TabIndex = 1;
            this.txtRxSys_PatID.TextChanged += new System.EventHandler(this.txtRxSys_PatID_TextChanged);
            // 
            // txtRxNum
            // 
            this.txtRxNum.Location = new System.Drawing.Point(128, 105);
            this.txtRxNum.Margin = new System.Windows.Forms.Padding(2);
            this.txtRxNum.Name = "txtRxNum";
            this.txtRxNum.Size = new System.Drawing.Size(240, 20);
            this.txtRxNum.TabIndex = 2;
            this.txtRxNum.TextChanged += new System.EventHandler(this.txtRxNum_TextChanged_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 105);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "RxSys_RxNum";
            // 
            // txtRxSys_DocID
            // 
            this.txtRxSys_DocID.Location = new System.Drawing.Point(128, 128);
            this.txtRxSys_DocID.Margin = new System.Windows.Forms.Padding(2);
            this.txtRxSys_DocID.Name = "txtRxSys_DocID";
            this.txtRxSys_DocID.Size = new System.Drawing.Size(240, 20);
            this.txtRxSys_DocID.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 128);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "RxSys_DocID";
            // 
            // txtDrug_ID
            // 
            this.txtDrug_ID.Location = new System.Drawing.Point(128, 150);
            this.txtDrug_ID.Margin = new System.Windows.Forms.Padding(2);
            this.txtDrug_ID.Name = "txtDrug_ID";
            this.txtDrug_ID.Size = new System.Drawing.Size(240, 20);
            this.txtDrug_ID.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 150);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "RxSys_DrugID";
            // 
            // txtSig
            // 
            this.txtSig.Location = new System.Drawing.Point(128, 175);
            this.txtSig.Margin = new System.Windows.Forms.Padding(2);
            this.txtSig.Multiline = true;
            this.txtSig.Name = "txtSig";
            this.txtSig.Size = new System.Drawing.Size(240, 19);
            this.txtSig.TabIndex = 5;
            this.txtSig.Enter += new System.EventHandler(this.txtComments_Enter);
            this.txtSig.Leave += new System.EventHandler(this.txtComments_Leave);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(15, 175);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(22, 13);
            this.label15.TabIndex = 29;
            this.label15.Text = "Sig";
            // 
            // txtRxStartDate
            // 
            this.txtRxStartDate.Location = new System.Drawing.Point(128, 197);
            this.txtRxStartDate.Margin = new System.Windows.Forms.Padding(2);
            this.txtRxStartDate.Name = "txtRxStartDate";
            this.txtRxStartDate.Size = new System.Drawing.Size(240, 20);
            this.txtRxStartDate.TabIndex = 6;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(15, 197);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(71, 13);
            this.label16.TabIndex = 31;
            this.label16.Text = "Rx Start Date";
            // 
            // txtRxStopDate
            // 
            this.txtRxStopDate.Location = new System.Drawing.Point(128, 220);
            this.txtRxStopDate.Margin = new System.Windows.Forms.Padding(2);
            this.txtRxStopDate.Name = "txtRxStopDate";
            this.txtRxStopDate.Size = new System.Drawing.Size(240, 20);
            this.txtRxStopDate.TabIndex = 7;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(15, 220);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(71, 13);
            this.label17.TabIndex = 33;
            this.label17.Text = "Rx Stop Date";
            // 
            // txtDiscontinueDate
            // 
            this.txtDiscontinueDate.Location = new System.Drawing.Point(128, 243);
            this.txtDiscontinueDate.Margin = new System.Windows.Forms.Padding(2);
            this.txtDiscontinueDate.Name = "txtDiscontinueDate";
            this.txtDiscontinueDate.Size = new System.Drawing.Size(240, 20);
            this.txtDiscontinueDate.TabIndex = 8;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(15, 243);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(48, 13);
            this.label18.TabIndex = 35;
            this.label18.Text = "DC Date";
            // 
            // txtDoseScheduleName
            // 
            this.txtDoseScheduleName.Location = new System.Drawing.Point(128, 266);
            this.txtDoseScheduleName.Margin = new System.Windows.Forms.Padding(2);
            this.txtDoseScheduleName.Name = "txtDoseScheduleName";
            this.txtDoseScheduleName.Size = new System.Drawing.Size(240, 20);
            this.txtDoseScheduleName.TabIndex = 9;
            this.txtDoseScheduleName.TextChanged += new System.EventHandler(this.txtDoseScheduleName_TextChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(15, 266);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(111, 13);
            this.label19.TabIndex = 37;
            this.label19.Text = "Dose Schedule Name";
            // 
            // txtRefills
            // 
            this.txtRefills.Location = new System.Drawing.Point(128, 311);
            this.txtRefills.Margin = new System.Windows.Forms.Padding(2);
            this.txtRefills.Name = "txtRefills";
            this.txtRefills.Size = new System.Drawing.Size(240, 20);
            this.txtRefills.TabIndex = 11;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(15, 311);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(35, 13);
            this.label20.TabIndex = 39;
            this.label20.Text = "Refills";
            // 
            // txtChartOnly
            // 
            this.txtChartOnly.Location = new System.Drawing.Point(494, 290);
            this.txtChartOnly.Margin = new System.Windows.Forms.Padding(2);
            this.txtChartOnly.Name = "txtChartOnly";
            this.txtChartOnly.Size = new System.Drawing.Size(240, 20);
            this.txtChartOnly.TabIndex = 25;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(402, 294);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(56, 13);
            this.label21.TabIndex = 41;
            this.label21.Text = "Chart Only";
            // 
            // txtRxType
            // 
            this.txtRxType.Location = new System.Drawing.Point(494, 85);
            this.txtRxType.Margin = new System.Windows.Forms.Padding(2);
            this.txtRxType.Name = "txtRxType";
            this.txtRxType.Size = new System.Drawing.Size(240, 20);
            this.txtRxType.TabIndex = 16;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(402, 88);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(47, 13);
            this.label22.TabIndex = 43;
            this.label22.Text = "Rx Type";
            // 
            // txtIsolate
            // 
            this.txtIsolate.Location = new System.Drawing.Point(128, 357);
            this.txtIsolate.Margin = new System.Windows.Forms.Padding(2);
            this.txtIsolate.Name = "txtIsolate";
            this.txtIsolate.Size = new System.Drawing.Size(240, 20);
            this.txtIsolate.TabIndex = 15;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(15, 361);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(38, 13);
            this.label23.TabIndex = 45;
            this.label23.Text = "Isolate";
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(128, 288);
            this.txtComments.Margin = new System.Windows.Forms.Padding(2);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size(240, 19);
            this.txtComments.TabIndex = 10;
            this.txtComments.Enter += new System.EventHandler(this.txtAllergies_Enter);
            this.txtComments.Leave += new System.EventHandler(this.txtAllergies_Leave);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(15, 287);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(56, 13);
            this.label24.TabIndex = 47;
            this.label24.Text = "Comments";
            // 
            // txtAnchorDate
            // 
            this.txtAnchorDate.Location = new System.Drawing.Point(494, 313);
            this.txtAnchorDate.Margin = new System.Windows.Forms.Padding(2);
            this.txtAnchorDate.Name = "txtAnchorDate";
            this.txtAnchorDate.Size = new System.Drawing.Size(240, 20);
            this.txtAnchorDate.TabIndex = 26;
            // 
            // txtMDOMStart
            // 
            this.txtMDOMStart.Location = new System.Drawing.Point(494, 108);
            this.txtMDOMStart.Margin = new System.Windows.Forms.Padding(2);
            this.txtMDOMStart.Name = "txtMDOMStart";
            this.txtMDOMStart.Size = new System.Drawing.Size(240, 20);
            this.txtMDOMStart.TabIndex = 17;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(402, 108);
            this.label32.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(66, 13);
            this.label32.TabIndex = 63;
            this.label32.Text = "MDOM Start";
            // 
            // txtMDOMStop
            // 
            this.txtMDOMStop.Location = new System.Drawing.Point(494, 131);
            this.txtMDOMStop.Margin = new System.Windows.Forms.Padding(2);
            this.txtMDOMStop.Name = "txtMDOMStop";
            this.txtMDOMStop.Size = new System.Drawing.Size(240, 20);
            this.txtMDOMStop.TabIndex = 18;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(402, 131);
            this.label33.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(66, 13);
            this.label33.TabIndex = 65;
            this.label33.Text = "MDOM Stop";
            // 
            // txtQtyPerDose
            // 
            this.txtQtyPerDose.Location = new System.Drawing.Point(494, 154);
            this.txtQtyPerDose.Margin = new System.Windows.Forms.Padding(2);
            this.txtQtyPerDose.Name = "txtQtyPerDose";
            this.txtQtyPerDose.Size = new System.Drawing.Size(240, 20);
            this.txtQtyPerDose.TabIndex = 19;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(402, 154);
            this.label34.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(70, 13);
            this.label34.TabIndex = 67;
            this.label34.Text = "Qty Per Dose";
            // 
            // txtQtyDispensed
            // 
            this.txtQtyDispensed.Location = new System.Drawing.Point(494, 176);
            this.txtQtyDispensed.Margin = new System.Windows.Forms.Padding(2);
            this.txtQtyDispensed.Name = "txtQtyDispensed";
            this.txtQtyDispensed.Size = new System.Drawing.Size(240, 20);
            this.txtQtyDispensed.TabIndex = 20;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(402, 176);
            this.label35.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(76, 13);
            this.label35.TabIndex = 69;
            this.label35.Text = "Qty Dispensed";
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(494, 199);
            this.txtStatus.Margin = new System.Windows.Forms.Padding(2);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.Size = new System.Drawing.Size(240, 20);
            this.txtStatus.TabIndex = 21;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(402, 199);
            this.label36.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(37, 13);
            this.label36.TabIndex = 71;
            this.label36.Text = "Status";
            // 
            // txtDOW
            // 
            this.txtDOW.Location = new System.Drawing.Point(494, 222);
            this.txtDOW.Margin = new System.Windows.Forms.Padding(2);
            this.txtDOW.Name = "txtDOW";
            this.txtDOW.Size = new System.Drawing.Size(240, 20);
            this.txtDOW.TabIndex = 22;
            this.txtDOW.TextChanged += new System.EventHandler(this.txtMCaidNum_TextChanged);
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(402, 222);
            this.label37.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(70, 13);
            this.label37.TabIndex = 73;
            this.label37.Text = "Day of Week";
            // 
            // txtSpecialDoses
            // 
            this.txtSpecialDoses.Location = new System.Drawing.Point(494, 245);
            this.txtSpecialDoses.Margin = new System.Windows.Forms.Padding(2);
            this.txtSpecialDoses.Name = "txtSpecialDoses";
            this.txtSpecialDoses.Size = new System.Drawing.Size(240, 20);
            this.txtSpecialDoses.TabIndex = 23;
            this.txtSpecialDoses.TextChanged += new System.EventHandler(this.txtAdmitDate_TextChanged);
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(402, 245);
            this.label38.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(75, 13);
            this.label38.TabIndex = 75;
            this.label38.Text = "Special Doses";
            // 
            // txtDoseTimesQtys
            // 
            this.txtDoseTimesQtys.Location = new System.Drawing.Point(494, 267);
            this.txtDoseTimesQtys.Margin = new System.Windows.Forms.Padding(2);
            this.txtDoseTimesQtys.Name = "txtDoseTimesQtys";
            this.txtDoseTimesQtys.Size = new System.Drawing.Size(240, 20);
            this.txtDoseTimesQtys.TabIndex = 24;
            this.txtDoseTimesQtys.TextChanged += new System.EventHandler(this.txtChartOnly_TextChanged);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(402, 270);
            this.label39.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(89, 13);
            this.label39.TabIndex = 77;
            this.label39.Text = "Dose Times/Qtys";
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(620, 10);
            this.btnGo.Margin = new System.Windows.Forms.Padding(2);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(142, 51);
            this.btnGo.TabIndex = 81;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // gbSwitches
            // 
            this.gbSwitches.Controls.Add(this.chkSendEOF);
            this.gbSwitches.Controls.Add(this.checkBox1);
            this.gbSwitches.Location = new System.Drawing.Point(316, 10);
            this.gbSwitches.Margin = new System.Windows.Forms.Padding(2);
            this.gbSwitches.Name = "gbSwitches";
            this.gbSwitches.Padding = new System.Windows.Forms.Padding(2);
            this.gbSwitches.Size = new System.Drawing.Size(295, 51);
            this.gbSwitches.TabIndex = 82;
            this.gbSwitches.TabStop = false;
            this.gbSwitches.Text = "[ Options ]";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(22, 19);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(124, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Auto Truncate Fields";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // valRxSys_PatID
            // 
            this.valRxSys_PatID.AutoSize = true;
            this.valRxSys_PatID.Location = new System.Drawing.Point(378, 86);
            this.valRxSys_PatID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valRxSys_PatID.Name = "valRxSys_PatID";
            this.valRxSys_PatID.Size = new System.Drawing.Size(13, 13);
            this.valRxSys_PatID.TabIndex = 85;
            this.valRxSys_PatID.Text = "0";
            // 
            // valRxSys_RxNum
            // 
            this.valRxSys_RxNum.AutoSize = true;
            this.valRxSys_RxNum.Location = new System.Drawing.Point(378, 109);
            this.valRxSys_RxNum.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valRxSys_RxNum.Name = "valRxSys_RxNum";
            this.valRxSys_RxNum.Size = new System.Drawing.Size(13, 13);
            this.valRxSys_RxNum.TabIndex = 86;
            this.valRxSys_RxNum.Text = "0";
            // 
            // valRxSys_DrugID
            // 
            this.valRxSys_DrugID.AutoSize = true;
            this.valRxSys_DrugID.Location = new System.Drawing.Point(378, 154);
            this.valRxSys_DrugID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valRxSys_DrugID.Name = "valRxSys_DrugID";
            this.valRxSys_DrugID.Size = new System.Drawing.Size(13, 13);
            this.valRxSys_DrugID.TabIndex = 88;
            this.valRxSys_DrugID.Text = "0";
            // 
            // valRxSys_DocID
            // 
            this.valRxSys_DocID.AutoSize = true;
            this.valRxSys_DocID.Location = new System.Drawing.Point(378, 132);
            this.valRxSys_DocID.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valRxSys_DocID.Name = "valRxSys_DocID";
            this.valRxSys_DocID.Size = new System.Drawing.Size(13, 13);
            this.valRxSys_DocID.TabIndex = 87;
            this.valRxSys_DocID.Text = "0";
            // 
            // valRxStartDate
            // 
            this.valRxStartDate.AutoSize = true;
            this.valRxStartDate.Location = new System.Drawing.Point(378, 202);
            this.valRxStartDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valRxStartDate.Name = "valRxStartDate";
            this.valRxStartDate.Size = new System.Drawing.Size(13, 13);
            this.valRxStartDate.TabIndex = 100;
            this.valRxStartDate.Text = "0";
            // 
            // valSig
            // 
            this.valSig.AutoSize = true;
            this.valSig.Location = new System.Drawing.Point(378, 179);
            this.valSig.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valSig.Name = "valSig";
            this.valSig.Size = new System.Drawing.Size(13, 13);
            this.valSig.TabIndex = 99;
            this.valSig.Text = "0";
            // 
            // valRefills
            // 
            this.valRefills.AutoSize = true;
            this.valRefills.Location = new System.Drawing.Point(378, 315);
            this.valRefills.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valRefills.Name = "valRefills";
            this.valRefills.Size = new System.Drawing.Size(13, 13);
            this.valRefills.TabIndex = 104;
            this.valRefills.Text = "0";
            // 
            // valDoseScheduleName
            // 
            this.valDoseScheduleName.AutoSize = true;
            this.valDoseScheduleName.Location = new System.Drawing.Point(378, 270);
            this.valDoseScheduleName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valDoseScheduleName.Name = "valDoseScheduleName";
            this.valDoseScheduleName.Size = new System.Drawing.Size(13, 13);
            this.valDoseScheduleName.TabIndex = 103;
            this.valDoseScheduleName.Text = "0";
            // 
            // valDiscontinueDate
            // 
            this.valDiscontinueDate.AutoSize = true;
            this.valDiscontinueDate.Location = new System.Drawing.Point(378, 247);
            this.valDiscontinueDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valDiscontinueDate.Name = "valDiscontinueDate";
            this.valDiscontinueDate.Size = new System.Drawing.Size(13, 13);
            this.valDiscontinueDate.TabIndex = 102;
            this.valDiscontinueDate.Text = "0";
            // 
            // valRxStopDate
            // 
            this.valRxStopDate.AutoSize = true;
            this.valRxStopDate.Location = new System.Drawing.Point(378, 224);
            this.valRxStopDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valRxStopDate.Name = "valRxStopDate";
            this.valRxStopDate.Size = new System.Drawing.Size(13, 13);
            this.valRxStopDate.TabIndex = 101;
            this.valRxStopDate.Text = "0";
            // 
            // valDoseTimesQtys
            // 
            this.valDoseTimesQtys.AutoSize = true;
            this.valDoseTimesQtys.Location = new System.Drawing.Point(744, 271);
            this.valDoseTimesQtys.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valDoseTimesQtys.Name = "valDoseTimesQtys";
            this.valDoseTimesQtys.Size = new System.Drawing.Size(13, 13);
            this.valDoseTimesQtys.TabIndex = 123;
            this.valDoseTimesQtys.Text = "0";
            // 
            // valAdmitDate
            // 
            this.valAdmitDate.AutoSize = true;
            this.valAdmitDate.Location = new System.Drawing.Point(744, 249);
            this.valAdmitDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valAdmitDate.Name = "valAdmitDate";
            this.valAdmitDate.Size = new System.Drawing.Size(13, 13);
            this.valAdmitDate.TabIndex = 122;
            this.valAdmitDate.Text = "0";
            // 
            // valDOW
            // 
            this.valDOW.AutoSize = true;
            this.valDOW.Location = new System.Drawing.Point(744, 226);
            this.valDOW.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valDOW.Name = "valDOW";
            this.valDOW.Size = new System.Drawing.Size(13, 13);
            this.valDOW.TabIndex = 121;
            this.valDOW.Text = "0";
            // 
            // valStatus
            // 
            this.valStatus.AutoSize = true;
            this.valStatus.Location = new System.Drawing.Point(744, 203);
            this.valStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valStatus.Name = "valStatus";
            this.valStatus.Size = new System.Drawing.Size(13, 13);
            this.valStatus.TabIndex = 120;
            this.valStatus.Text = "0";
            // 
            // valQtyDispensed
            // 
            this.valQtyDispensed.AutoSize = true;
            this.valQtyDispensed.Location = new System.Drawing.Point(744, 180);
            this.valQtyDispensed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valQtyDispensed.Name = "valQtyDispensed";
            this.valQtyDispensed.Size = new System.Drawing.Size(13, 13);
            this.valQtyDispensed.TabIndex = 119;
            this.valQtyDispensed.Text = "0";
            // 
            // valQtyPerDose
            // 
            this.valQtyPerDose.AutoSize = true;
            this.valQtyPerDose.Location = new System.Drawing.Point(744, 158);
            this.valQtyPerDose.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valQtyPerDose.Name = "valQtyPerDose";
            this.valQtyPerDose.Size = new System.Drawing.Size(13, 13);
            this.valQtyPerDose.TabIndex = 118;
            this.valQtyPerDose.Text = "0";
            // 
            // valMDOMStop
            // 
            this.valMDOMStop.AutoSize = true;
            this.valMDOMStop.Location = new System.Drawing.Point(744, 135);
            this.valMDOMStop.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valMDOMStop.Name = "valMDOMStop";
            this.valMDOMStop.Size = new System.Drawing.Size(13, 13);
            this.valMDOMStop.TabIndex = 117;
            this.valMDOMStop.Text = "0";
            // 
            // valMDOMStart
            // 
            this.valMDOMStart.AutoSize = true;
            this.valMDOMStart.Location = new System.Drawing.Point(744, 112);
            this.valMDOMStart.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valMDOMStart.Name = "valMDOMStart";
            this.valMDOMStart.Size = new System.Drawing.Size(13, 13);
            this.valMDOMStart.TabIndex = 116;
            this.valMDOMStart.Text = "0";
            // 
            // valAnchorDate
            // 
            this.valAnchorDate.AutoSize = true;
            this.valAnchorDate.Location = new System.Drawing.Point(744, 315);
            this.valAnchorDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valAnchorDate.Name = "valAnchorDate";
            this.valAnchorDate.Size = new System.Drawing.Size(13, 13);
            this.valAnchorDate.TabIndex = 113;
            this.valAnchorDate.Text = "0";
            // 
            // valComments
            // 
            this.valComments.AutoSize = true;
            this.valComments.Location = new System.Drawing.Point(378, 292);
            this.valComments.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valComments.Name = "valComments";
            this.valComments.Size = new System.Drawing.Size(13, 13);
            this.valComments.TabIndex = 108;
            this.valComments.Text = "0";
            // 
            // valIsolate
            // 
            this.valIsolate.AutoSize = true;
            this.valIsolate.Location = new System.Drawing.Point(378, 359);
            this.valIsolate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valIsolate.Name = "valIsolate";
            this.valIsolate.Size = new System.Drawing.Size(13, 13);
            this.valIsolate.TabIndex = 107;
            this.valIsolate.Text = "0";
            // 
            // valRxType
            // 
            this.valRxType.AutoSize = true;
            this.valRxType.Location = new System.Drawing.Point(744, 89);
            this.valRxType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valRxType.Name = "valRxType";
            this.valRxType.Size = new System.Drawing.Size(13, 13);
            this.valRxType.TabIndex = 106;
            this.valRxType.Text = "0";
            // 
            // valChartOnly
            // 
            this.valChartOnly.AutoSize = true;
            this.valChartOnly.Location = new System.Drawing.Point(744, 294);
            this.valChartOnly.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valChartOnly.Name = "valChartOnly";
            this.valChartOnly.Size = new System.Drawing.Size(13, 13);
            this.valChartOnly.TabIndex = 105;
            this.valChartOnly.Text = "0";
            // 
            // valRxSys_NewRxNum
            // 
            this.valRxSys_NewRxNum.AutoSize = true;
            this.valRxSys_NewRxNum.Location = new System.Drawing.Point(378, 338);
            this.valRxSys_NewRxNum.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.valRxSys_NewRxNum.Name = "valRxSys_NewRxNum";
            this.valRxSys_NewRxNum.Size = new System.Drawing.Size(13, 13);
            this.valRxSys_NewRxNum.TabIndex = 127;
            this.valRxSys_NewRxNum.Text = "0";
            // 
            // txtRxSys_NewRxNum
            // 
            this.txtRxSys_NewRxNum.Location = new System.Drawing.Point(128, 334);
            this.txtRxSys_NewRxNum.Margin = new System.Windows.Forms.Padding(2);
            this.txtRxSys_NewRxNum.Name = "txtRxSys_NewRxNum";
            this.txtRxSys_NewRxNum.Size = new System.Drawing.Size(240, 20);
            this.txtRxSys_NewRxNum.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 338);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 13);
            this.label6.TabIndex = 125;
            this.label6.Text = "RxSys_NewRxNum";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(402, 315);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 128;
            this.label5.Text = "Anchor Date";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbSendTaggedRecord);
            this.groupBox2.Controls.Add(this.cbSendDelimitedRecord);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtFileName);
            this.groupBox2.Controls.Add(this.cbTaggedTestRecord);
            this.groupBox2.Controls.Add(this.cbDelimitedTestRecord);
            this.groupBox2.Location = new System.Drawing.Point(128, 393);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(548, 76);
            this.groupBox2.TabIndex = 204;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "[ Generate Test Record ]";
            // 
            // cbSendTaggedRecord
            // 
            this.cbSendTaggedRecord.AutoSize = true;
            this.cbSendTaggedRecord.Location = new System.Drawing.Point(190, 42);
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
            this.cbSendDelimitedRecord.Location = new System.Drawing.Point(89, 42);
            this.cbSendDelimitedRecord.Margin = new System.Windows.Forms.Padding(2);
            this.cbSendDelimitedRecord.Name = "cbSendDelimitedRecord";
            this.cbSendDelimitedRecord.Size = new System.Drawing.Size(69, 17);
            this.cbSendDelimitedRecord.TabIndex = 10;
            this.cbSendDelimitedRecord.Text = "Delimited";
            this.cbSendDelimitedRecord.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Send Record";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 21);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Create File";
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
            // cbTaggedTestRecord
            // 
            this.cbTaggedTestRecord.AutoSize = true;
            this.cbTaggedTestRecord.Location = new System.Drawing.Point(190, 21);
            this.cbTaggedTestRecord.Margin = new System.Windows.Forms.Padding(2);
            this.cbTaggedTestRecord.Name = "cbTaggedTestRecord";
            this.cbTaggedTestRecord.Size = new System.Drawing.Size(63, 17);
            this.cbTaggedTestRecord.TabIndex = 1;
            this.cbTaggedTestRecord.Text = "Tagged";
            this.cbTaggedTestRecord.UseVisualStyleBackColor = true;
            // 
            // cbDelimitedTestRecord
            // 
            this.cbDelimitedTestRecord.AutoSize = true;
            this.cbDelimitedTestRecord.Location = new System.Drawing.Point(89, 20);
            this.cbDelimitedTestRecord.Margin = new System.Windows.Forms.Padding(2);
            this.cbDelimitedTestRecord.Name = "cbDelimitedTestRecord";
            this.cbDelimitedTestRecord.Size = new System.Drawing.Size(69, 17);
            this.cbDelimitedTestRecord.TabIndex = 0;
            this.cbDelimitedTestRecord.Text = "Delimited";
            this.cbDelimitedTestRecord.UseVisualStyleBackColor = true;
            // 
            // chkSendEOF
            // 
            this.chkSendEOF.AutoSize = true;
            this.chkSendEOF.Checked = true;
            this.chkSendEOF.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSendEOF.Location = new System.Drawing.Point(177, 19);
            this.chkSendEOF.Name = "chkSendEOF";
            this.chkSendEOF.Size = new System.Drawing.Size(92, 17);
            this.chkSendEOF.TabIndex = 2;
            this.chkSendEOF.Text = "Send <EOF/>";
            this.chkSendEOF.UseVisualStyleBackColor = true;
            // 
            // frmRxRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 480);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.valRxSys_NewRxNum);
            this.Controls.Add(this.txtRxSys_NewRxNum);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.valDoseTimesQtys);
            this.Controls.Add(this.valAdmitDate);
            this.Controls.Add(this.valDOW);
            this.Controls.Add(this.valStatus);
            this.Controls.Add(this.valQtyDispensed);
            this.Controls.Add(this.valQtyPerDose);
            this.Controls.Add(this.valMDOMStop);
            this.Controls.Add(this.valMDOMStart);
            this.Controls.Add(this.valAnchorDate);
            this.Controls.Add(this.valComments);
            this.Controls.Add(this.valIsolate);
            this.Controls.Add(this.valRxType);
            this.Controls.Add(this.valChartOnly);
            this.Controls.Add(this.valRefills);
            this.Controls.Add(this.valDoseScheduleName);
            this.Controls.Add(this.valDiscontinueDate);
            this.Controls.Add(this.valRxStopDate);
            this.Controls.Add(this.valRxStartDate);
            this.Controls.Add(this.valSig);
            this.Controls.Add(this.valRxSys_DrugID);
            this.Controls.Add(this.valRxSys_DocID);
            this.Controls.Add(this.valRxSys_RxNum);
            this.Controls.Add(this.valRxSys_PatID);
            this.Controls.Add(this.gbSwitches);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtDoseTimesQtys);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.txtSpecialDoses);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.txtDOW);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.label36);
            this.Controls.Add(this.txtQtyDispensed);
            this.Controls.Add(this.label35);
            this.Controls.Add(this.txtQtyPerDose);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.txtMDOMStop);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.txtMDOMStart);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.txtAnchorDate);
            this.Controls.Add(this.txtComments);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.txtIsolate);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.txtRxType);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.txtChartOnly);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.txtRefills);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.txtDoseScheduleName);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.txtDiscontinueDate);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.txtRxStopDate);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txtRxStartDate);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.txtSig);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txtDrug_ID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtRxSys_DocID);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtRxNum);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRxSys_PatID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmRxRecord";
            this.Text = "Rx Record";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbSwitches.ResumeLayout(false);
            this.gbSwitches.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbDelete;
        private System.Windows.Forms.RadioButton rbChange;
        private System.Windows.Forms.RadioButton rbAdd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRxSys_PatID;
        private System.Windows.Forms.TextBox txtRxNum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRxSys_DocID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDrug_ID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSig;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtRxStartDate;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtRxStopDate;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtDiscontinueDate;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtDoseScheduleName;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtRefills;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtChartOnly;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtRxType;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtIsolate;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtAnchorDate;
        private System.Windows.Forms.TextBox txtMDOMStart;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox txtMDOMStop;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.TextBox txtQtyPerDose;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox txtQtyDispensed;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.TextBox txtDOW;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox txtSpecialDoses;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox txtDoseTimesQtys;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.GroupBox gbSwitches;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label valRxSys_PatID;
        private System.Windows.Forms.Label valRxSys_RxNum;
        private System.Windows.Forms.Label valRxSys_DrugID;
        private System.Windows.Forms.Label valRxSys_DocID;
        private System.Windows.Forms.Label valRxStartDate;
        private System.Windows.Forms.Label valSig;
        private System.Windows.Forms.Label valRefills;
        private System.Windows.Forms.Label valDoseScheduleName;
        private System.Windows.Forms.Label valDiscontinueDate;
        private System.Windows.Forms.Label valRxStopDate;
        private System.Windows.Forms.Label valDoseTimesQtys;
        private System.Windows.Forms.Label valAdmitDate;
        private System.Windows.Forms.Label valDOW;
        private System.Windows.Forms.Label valStatus;
        private System.Windows.Forms.Label valQtyDispensed;
        private System.Windows.Forms.Label valQtyPerDose;
        private System.Windows.Forms.Label valMDOMStop;
        private System.Windows.Forms.Label valMDOMStart;
        private System.Windows.Forms.Label valAnchorDate;
        private System.Windows.Forms.Label valComments;
        private System.Windows.Forms.Label valIsolate;
        private System.Windows.Forms.Label valRxType;
        private System.Windows.Forms.Label valChartOnly;
        private System.Windows.Forms.Label valRxSys_NewRxNum;
        private System.Windows.Forms.TextBox txtRxSys_NewRxNum;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.CheckBox cbTaggedTestRecord;
        private System.Windows.Forms.CheckBox cbDelimitedTestRecord;
        private System.Windows.Forms.CheckBox cbSendTaggedRecord;
        private System.Windows.Forms.CheckBox cbSendDelimitedRecord;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chkSendEOF;
    }
}