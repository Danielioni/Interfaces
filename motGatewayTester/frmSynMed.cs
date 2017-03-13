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
using motMatchineInterface;

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

        public static bool useLegacy = true;

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

            __get_started();
        }

        private static async void __get_started()
        {
            try
            {
                DateTime __due;

               
                if (!useLegacy)
                {
                    motNextSynMed __synmed = new motNextSynMed(@"\motNext\SynmedFiles");

                    await __synmed.Login(__username, __password);

                    await __synmed.BuildPatientList(2000);

                    if (!string.IsNullOrEmpty(__start_date) && !string.IsNullOrEmpty(__patient_last_name))
                    {
                        await __synmed.WritePatient(__patient_last_name, __patient_first_name, __patient_middle_initial, DateTime.Parse(__start_date), 30);
                    }

                    else if (!string.IsNullOrEmpty(__patient_last_name))
                    {
                        await __synmed.WriteCycle(DateTime.Parse(__start_date), DateTime.Parse(__start_date));
                    }
                }
                else
                {
                    motLegacySynMed __old_synmed = new motLegacySynMed(@"\motNext\SynmedFiles");

                    await __old_synmed.Login(__username, __password);
                    await __old_synmed.WriteCycle(DateTime.Parse(__start_date), DateTime.Parse(__start_date));
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
    }
}
