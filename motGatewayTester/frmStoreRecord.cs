using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using motCommonLib;
using motOutboundLib;

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
                if ((cbDelimitedTestRecord.Checked || cbTaggedTestRecord.Checked) && txtFileName.Text.Length > 0)
                {
                    List<KeyValuePair<string, string>> __key_data = new List<KeyValuePair<string, string>>();
                    var __output = new motFormattedFileOutput();

                    if (!Directory.Exists("C:/Records"))
                    {
                        Directory.CreateDirectory("C:/Records");
                    }

                    if (cbDelimitedTestRecord.Checked)
                    {
                        __key_data.Add(new KeyValuePair<string, string>("TableType", "S" + __action.ToUpper().Substring(0, 1)));
                    }
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_StoreID", txtRxSys_StoreID.Text));
                    __key_data.Add(new KeyValuePair<string, string>("StoreName", txtStoreName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Address1", txtAddress1.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Address2", txtAddress2.Text));
                    __key_data.Add(new KeyValuePair<string, string>("City", txtCity.Text));
                    __key_data.Add(new KeyValuePair<string, string>("State", txtState.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Zip", txtZip.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Phone", txtPhone.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Fax", txtFax.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DEANum", txtDEANum.Text));

                    if (cbDelimitedTestRecord.Checked)
                    {
                        byte[] __record = __output.WriteDelimitedFile(@"C:\Records\" + txtFileName.Text, __key_data);

                        if (cbSendDelimitedRecord.Checked)
                        {
                            __execute.__p.Write(__record);
                        }
                    }

                    if (cbTaggedTestRecord.Checked)
                    {
                        if (cbDelimitedTestRecord.Checked)
                        {
                            __key_data.RemoveAt(0);
                        }

                        string __record = __output.WriteTaggedFile(@"C:\Records\" + txtFileName.Text, __key_data, "Store", __action);

                        if (cbSendTaggedRecord.Checked)
                        {
                            __execute.__p.Write(__record);
                        }
                    }
                }
                else
                {
                    Store.RxSys_StoreID = txtRxSys_StoreID.Text;
                    Store.StoreName = txtStoreName.Text;
                    Store.Address1 = txtAddress1.Text;
                    Store.Address2 = txtAddress2.Text;
                    Store.City = txtCity.Text;
                    Store.State = txtState.Text;
                    Store.Zip = txtZip.Text;
                    Store.Phone = txtPhone.Text;
                    Store.Fax = txtFax.Text;
                    Store.DEANum = txtDEANum.Text;

                    __execute.__update_event_ui("Store field assignment complete ...");

                    try
                    {
                        Store.Write(__execute.__p);
                    }
                    catch (Exception ex)
                    {
                        __execute.__update_error_ui(string.Format("Store record write error: {0}", ex.Message));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                __execute.__update_error_ui(string.Format("Store record error: {0}", ex.Message));
                return;
            }

            __execute.__update_event_ui("Store write record complete ...");
        }
    }
}
