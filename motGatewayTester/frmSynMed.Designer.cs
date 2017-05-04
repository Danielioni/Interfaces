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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Fill = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Print = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.locname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.locationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsFacilities = new motGatewayTester.dsFacilities();
            this.patientBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsPatients = new motGatewayTester.dsPatients();
            this.locationTableAdapter = new motGatewayTester.dsFacilitiesTableAdapters.LocationTableAdapter();
            this.patientTableAdapter = new motGatewayTester.dsPatientsTableAdapters.PatientTableAdapter();
            this.txtFacility = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.locationBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsFacilities)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.patientBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatients)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(501, 70);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(187, 89);
            this.btnGo.TabIndex = 0;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtPatientLastName
            // 
            this.txtPatientLastName.Location = new System.Drawing.Point(107, 29);
            this.txtPatientLastName.Name = "txtPatientLastName";
            this.txtPatientLastName.Size = new System.Drawing.Size(75, 20);
            this.txtPatientLastName.TabIndex = 1;
            // 
            // txtCycleStartDate
            // 
            this.txtCycleStartDate.Location = new System.Drawing.Point(107, 53);
            this.txtCycleStartDate.Name = "txtCycleStartDate";
            this.txtCycleStartDate.Size = new System.Drawing.Size(184, 20);
            this.txtCycleStartDate.TabIndex = 3;
            this.txtCycleStartDate.Text = "4/18/2017";
            this.txtCycleStartDate.TextChanged += new System.EventHandler(this.txtCycleStartDate_TextChanged);
            // 
            // txtCycleLength
            // 
            this.txtCycleLength.Location = new System.Drawing.Point(107, 79);
            this.txtCycleLength.Name = "txtCycleLength";
            this.txtCycleLength.Size = new System.Drawing.Size(184, 20);
            this.txtCycleLength.TabIndex = 4;
            // 
            // txtOutFile
            // 
            this.txtOutFile.Location = new System.Drawing.Point(107, 105);
            this.txtOutFile.Name = "txtOutFile";
            this.txtOutFile.Size = new System.Drawing.Size(184, 20);
            this.txtOutFile.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Patient Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Cycle Start Date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Cycle Length";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(59, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Out File";
            // 
            // txtPatientFirstName
            // 
            this.txtPatientFirstName.Location = new System.Drawing.Point(187, 29);
            this.txtPatientFirstName.Name = "txtPatientFirstName";
            this.txtPatientFirstName.Size = new System.Drawing.Size(75, 20);
            this.txtPatientFirstName.TabIndex = 11;
            // 
            // txtPatientMI
            // 
            this.txtPatientMI.Location = new System.Drawing.Point(268, 29);
            this.txtPatientMI.Name = "txtPatientMI";
            this.txtPatientMI.Size = new System.Drawing.Size(20, 20);
            this.txtPatientMI.TabIndex = 12;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(501, 27);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(2);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(204, 20);
            this.txtUserName.TabIndex = 13;
            this.txtUserName.Text = "mot";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(501, 49);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(2);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPassword.Size = new System.Drawing.Size(204, 20);
            this.txtPassword.TabIndex = 14;
            this.txtPassword.Text = "mot";
            this.txtPassword.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(426, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "User Name";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(426, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Password";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbUseLegacy);
            this.groupBox1.Controls.Add(this.rbUseNext);
            this.groupBox1.Location = new System.Drawing.Point(24, 268);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(150, 81);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "[ Database ]";
            // 
            // rbUseLegacy
            // 
            this.rbUseLegacy.AutoSize = true;
            this.rbUseLegacy.Location = new System.Drawing.Point(19, 48);
            this.rbUseLegacy.Margin = new System.Windows.Forms.Padding(2);
            this.rbUseLegacy.Name = "rbUseLegacy";
            this.rbUseLegacy.Size = new System.Drawing.Size(99, 17);
            this.rbUseLegacy.TabIndex = 1;
            this.rbUseLegacy.Text = "Use motLegacy";
            this.rbUseLegacy.UseVisualStyleBackColor = true;
            // 
            // rbUseNext
            // 
            this.rbUseNext.AutoSize = true;
            this.rbUseNext.Checked = true;
            this.rbUseNext.Location = new System.Drawing.Point(19, 25);
            this.rbUseNext.Margin = new System.Windows.Forms.Padding(2);
            this.rbUseNext.Name = "rbUseNext";
            this.rbUseNext.Size = new System.Drawing.Size(86, 17);
            this.rbUseNext.TabIndex = 0;
            this.rbUseNext.TabStop = true;
            this.rbUseNext.Text = "Use motNext";
            this.rbUseNext.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.Fill,
            this.Print,
            this.locname});
            this.dataGridView1.Location = new System.Drawing.Point(212, 214);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(836, 261);
            this.dataGridView1.TabIndex = 18;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick_1);
            // 
            // Select
            // 
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            // 
            // Fill
            // 
            this.Fill.HeaderText = "Fill";
            this.Fill.Name = "Fill";
            // 
            // Print
            // 
            this.Print.HeaderText = "Print";
            this.Print.Name = "Print";
            // 
            // locname
            // 
            this.locname.DataPropertyName = "locname";
            this.locname.HeaderText = "Facility";
            this.locname.Name = "locname";
            // 
            // locationBindingSource
            // 
            this.locationBindingSource.DataMember = "Location";
            this.locationBindingSource.DataSource = this.dsFacilities;
            // 
            // dsFacilities
            // 
            this.dsFacilities.DataSetName = "dsFacilities";
            this.dsFacilities.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // patientBindingSource
            // 
            this.patientBindingSource.DataMember = "Patient";
            this.patientBindingSource.DataSource = this.dsPatients;
            // 
            // dsPatients
            // 
            this.dsPatients.DataSetName = "dsPatients";
            this.dsPatients.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // locationTableAdapter
            // 
            this.locationTableAdapter.ClearBeforeFill = true;
            // 
            // patientTableAdapter
            // 
            this.patientTableAdapter.ClearBeforeFill = true;
            // 
            // txtFacility
            // 
            this.txtFacility.Location = new System.Drawing.Point(107, 131);
            this.txtFacility.Name = "txtFacility";
            this.txtFacility.Size = new System.Drawing.Size(184, 20);
            this.txtFacility.TabIndex = 19;
            this.txtFacility.Text = "CCDS-RIVERVIEW";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(31, 134);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Facility Name";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // frmSynMed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 487);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtFacility);
            this.Controls.Add(this.dataGridView1);
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
            this.Name = "frmSynMed";
            this.Text = "frmSynMed";
            this.Load += new System.EventHandler(this.frmSynMed_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.locationBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsFacilities)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.patientBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsPatients)).EndInit();
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
        private System.Windows.Forms.DataGridViewTextBoxColumn patientDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn facilityDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn iidDataGridViewTextBoxColumn;
        private dsFacilities dsFacilities;
        private System.Windows.Forms.BindingSource locationBindingSource;
        private dsFacilitiesTableAdapters.LocationTableAdapter locationTableAdapter;
        private dsPatients dsPatients;
        private System.Windows.Forms.BindingSource patientBindingSource;
        private dsPatientsTableAdapters.PatientTableAdapter patientTableAdapter;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Fill;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Print;
        private System.Windows.Forms.DataGridViewTextBoxColumn locname;
        private System.Windows.Forms.TextBox txtFacility;
        private System.Windows.Forms.Label label7;
    }
}