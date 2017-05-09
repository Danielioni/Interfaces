using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using motOutboundLib;
using motMachineInterface;

namespace motGatewayTester
{
    public partial class frmSynMed : Form
    {
        public static string __start_date = string.Empty;
        public static string __patient_last_name = string.Empty;
        public static string __patient_first_name = string.Empty;
        public static string __patient_middle_initial = string.Empty;
        public static string __cycle_start;
        public static string __facility_name = string.Empty;

        public static string __username = string.Empty;
        public static string __password = string.Empty;

        public static string _facility;

        public static bool useLegacy = true;

        public motNextSynMed __synmed;
        public motLegacySynMed __old_synmed;


        public frmSynMed()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            __start_date = txtCycleStartDate.Text;
            __patient_last_name = txtPatientLastName.Text;
            __patient_first_name = txtPatientFirstName.Text;
            __patient_middle_initial = txtPatientMI.Text;

            __username = txtUserName.Text;
            __password = txtPassword.Text;

            useLegacy = rbUseLegacy.Checked;

            __facility_name = txtFacility.Text;

            if (!useLegacy)
            {
                __startNext();
            }
            else
            {
                __startLegacy();
            }

        }

        public void __startLegacy()
        {
            motLegacySynMed __motLegacy_synmed = new motLegacySynMed(@"\motNext\SynmedFiles");
            __motLegacy_synmed.Login(__username, __password);

            __motLegacy_synmed.WriteCycle(DateTime.Parse(__start_date), DateTime.Parse(__start_date));

            this.locationTableAdapter.Fill(this.dsFacilities.Location);

            //           dataGridView1.DataSource = __motLegacy_synmed.motFacilities;

        }
        private async void __startNext()
        {
            try
            {
                DateTime __due;

                var __synmed = new motNextSynMed(@"\motNext\SynmedFiles");

                await __synmed.Login(__username, __password);

                //await __synmed.BuildPatientList(2000);

                //DataSet patient_list = await __synmed.GetPatientsAsDataSet();
                DataSet facility_list = await __synmed.GetFacilitiesAsDataSet();

                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = facility_list;
                dataGridView1.DataMember = "Facilities";

                if (!string.IsNullOrEmpty(__facility_name))
                {
                    await __synmed.WriteFacilityCycle(__facility_name, DateTime.Parse(__start_date), 0);
                }

                else if (!string.IsNullOrEmpty(__start_date) && string.IsNullOrEmpty(__patient_last_name))
                {
                    await __synmed.WriteCycle(DateTime.Parse(__start_date), DateTime.Parse(__start_date));
                }




                /*
                await __synmed.Write(@"Jenney",
                                     @"Peter",
                                     @"B",
                                    DateTime.Parse("09/17/1930"),
                                    __due,
                                     30);
                

                await __synmed.WritePatient(@"ALLEN",
                                     @"PRISCILLA",
                                     @"",
                                    DateTime.Parse("12/31/1960"),
                                    __due,
                                    30);
               */

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCycleStartDate_TextChanged(object sender, EventArgs e)
        {

        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = 0;
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            int i = 0;
        }

        private void frmSynMed_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dsPatients.Patient' table. You can move, or remove it, as needed.
            //this.patientTableAdapter.Fill(this.dsPatients.Patient);
            // TODO: This line of code loads data into the 'dsFacilities.Location' table. You can move, or remove it, as needed.
            //this.locationTableAdapter.Fill(this.dsFacilities.Location);


        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
