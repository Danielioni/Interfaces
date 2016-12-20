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
                
                await SynMed.Login();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
