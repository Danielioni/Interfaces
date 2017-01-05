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

namespace motGatewayTester
{
    public partial class frmSynMed : Form
    {
        public frmSynMed()
        {
            InitializeComponent();

            
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            __get_started();   
        }

        private static async void __get_started()
        {
            try
            {
                SynMed __synmed = new SynMed(@"\motNext\SynmedFiles");
                DateTime __due = DateTime.Parse("2/4/2017");

                await __synmed.Login("mot", "mot");

                //await __synmed.WriteCycle(__due);

                /*
                await __synmed.Write(@"Jenney",
                                     @"Peter",
                                     @"B",
                                    DateTime.Parse("09/17/1930"),
                                    __due,
                                     30);
                
    */
                await __synmed.Write(@"ALLEN",
                                     @"PRISCILLA",
                                     @"",
                                    DateTime.Parse("12/1/1960"),
                                    __due,
                                    30);
                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
