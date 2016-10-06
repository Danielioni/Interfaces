using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using motCommonLib;

namespace motGatewayTester
{
    public partial class frmStoreRecord : Form
    {
        Execute __execute;
        string __action = "Add";
        motStoreRecord Store;

        public frmStoreRecord()
        {
            InitializeComponent();
            Store = new motStoreRecord(__action, motErrorlLevel.Info, false);
            __execute = frmMainRecordTester.__execute;
        }

        private void txtRxSys_StoreID_TextChanged(object sender, EventArgs e)
        {
            valRxSys_StoreID.Text = txtRxSys_StoreID.Text.Length.ToString();
        }

        private void txtStoreName_TextChanged(object sender, EventArgs e)
        {
            valStoreName.Text = txtStoreName.Text.Length.ToString();
        }

        private void txtAddress1_TextChanged(object sender, EventArgs e)
        {
            valAddress1.Text = txtAddress2.Text.Length.ToString();
        }

        private void txtAddress2_TextChanged(object sender, EventArgs e)
        {
            valAddress2.Text = txtAddress2.Text.Length.ToString();
        }

        private void txtCity_TextChanged(object sender, EventArgs e)
        {
            valCity.Text = txtCity.Text.Length.ToString();
        }

        private void txtState_TextChanged(object sender, EventArgs e)
        {
            valState.Text = txtState.Text.Length.ToString();
        }

        private void txtZip_TextChanged(object sender, EventArgs e)
        {
            valZip.Text = txtZip.Text.Length.ToString();
        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {
            valPhone.Text = txtPhone.Text.Length.ToString();
        }

        private void txtFax_TextChanged(object sender, EventArgs e)
        {
            valFax.Text = txtFax.Text.Length.ToString();
        }

        private void txtDEANum_TextChanged(object sender, EventArgs e)
        {
            valDEANum.Text = txtDEANum.Text.Length.ToString();
        }

        private void rbAdd_CheckedChanged(object sender, EventArgs e)
        {
            __action = "Add";
        }

        private void rbChange_CheckedChanged(object sender, EventArgs e)
        {
            __action = "Change";
        }

        private void rbDelete_CheckedChanged(object sender, EventArgs e)
        {
            __action = "Delete";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Store.__auto_truncate = checkBox1.Checked;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                Store.RxSys_StoreID = txtRxSys_StoreID.Text;
                Store.StoreName = txtStoreName.Text;
                Store.Address1 = txtAddress1.Text;
                Store.Address2 = txtAddress2.Text;
                Store.City = txtCity.Text;
                Store.Zip = txtZip.Text;
                Store.Phone = txtPhone.Text;
                Store.Fax = txtFax.Text;
                Store.DEANum = txtDEANum.Text;

                __execute.__update_event_ui("Store field assignment complete ...");
            }
            catch (Exception ex)
            {
                __execute.__update_error_ui(string.Format("Store record field assignment error: {0}", ex.Message));
                return;
            }

            try
            {
                Store.Write(__execute.__p);
                __execute.__update_event_ui("Store write record complete ...");
            }
            catch (Exception ex)
            {
                __execute.__update_error_ui(string.Format("Store record write error: {0}", ex.Message));
                return;
            }
        }
    }
}
