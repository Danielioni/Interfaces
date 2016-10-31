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
    public partial class frmLocationRecord : Form
    {
        Execute __execute;
        string __action = "Add";
        motLocationRecord Loc;

        public frmLocationRecord()
        {
            InitializeComponent();
            Loc = new motLocationRecord(__action,  false);
            __execute = frmMainRecordTester.__execute;
        }

        private void txtRxSys_StoreID_TextChanged(object sender, EventArgs e)
        {
            valRxSys_StoreID.Text = txtRxSys_StoreID.Text.Length.ToString();
        }

        private void txtRxSys_LocID_TextChanged(object sender, EventArgs e)
        {
            valRxSys_LocID.Text = txtRxSys_LocID.Text.Length.ToString();
        }

        private void txtLocName_TextChanged(object sender, EventArgs e)
        {
            valLocName.Text = txtLocName.Text.Length.ToString();
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

        private void txtComment_TextChanged(object sender, EventArgs e)
        {
            valComment.Text = txtComment.Text.Length.ToString();
        }

        private void txtCycleDays_TextChanged(object sender, EventArgs e)
        {
            valCycleDays.Text = txtCycleDays.Text.Length.ToString();
        }
        private void txtCycleType_TextChanged(object sender, EventArgs e)
        {
            valCycleDays.Text = txtCycleType.Text.Length.ToString();
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
            Loc.__auto_truncate = chkAutoTruncate.Checked;
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
                        __key_data.Add(new KeyValuePair<string, string>("TableType", "L" + __action.ToUpper().Substring(0, 1)));
                        //__key_data.Add(new KeyValuePair<string, string>("Action", __action.ToUpper().Substring(0, 1)));
                    }
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_StoreID", txtRxSys_StoreID.Text));
                    __key_data.Add(new KeyValuePair<string, string>("LocName", txtLocName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Address1", txtAddress1.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Address2", txtAddress2.Text));
                    __key_data.Add(new KeyValuePair<string, string>("City", txtCity.Text));
                    __key_data.Add(new KeyValuePair<string, string>("State", txtState.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Zip", txtZip.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Phone", txtPhone.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Comments", txtComment.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_StoreID", txtRxSys_LocID.Text));
                    __key_data.Add(new KeyValuePair<string, string>("CycleDays", txtCycleDays.Text));
                    __key_data.Add(new KeyValuePair<string, string>("CycleType", txtCycleType.Text));

                    if (cbDelimitedTestRecord.Checked)
                    {
                        byte[] __record = __output.WriteDelimitedFile(@"C:\Records\" + txtFileName.Text, __key_data);

                        if (cbSendDelimitedRecord.Checked)
                        {
                            __execute.__p.write(__record);
                        }
                    }

                    if (cbTaggedTestRecord.Checked)
                    {
                        if (cbDelimitedTestRecord.Checked)
                        {
                            __key_data.RemoveAt(0);
                        }

                        string __record = __output.WriteTaggedFile(@"C:\Records\" + txtFileName.Text, __key_data, "Location", __action);

                        if (cbSendTaggedRecord.Checked)
                        {
                            __execute.__p.write(__record);
                        }
                    }
                }
                else
                {
                    Loc.RxSys_StoreID = txtRxSys_StoreID.Text;
                    Loc.RxSys_LocID = txtRxSys_LocID.Text;
                    Loc.LocationName = txtLocName.Text;
                    Loc.Address1 = txtAddress1.Text;
                    Loc.Address2 = txtAddress2.Text;
                    Loc.City = txtCity.Text;
                    Loc.State = txtState.Text;
                    Loc.Zip = txtZip.Text;
                    Loc.Phone = txtPhone.Text;
                    Loc.Comments = txtComment.Text;
                    Loc.CycleDays = txtCycleDays.Text.Length > 0 ? Convert.ToInt32(txtCycleDays.Text) : 0;
                    Loc.CycleType = txtCycleType.Text.Length > 0 ? Convert.ToInt32(txtCycleType.Text) : 0;

                    __execute.__update_event_ui("Location field assignment complete ...");

                    try
                    {
                        Loc.Write(__execute.__p);
                        
                    }
                    catch (Exception ex)
                    {
                        __execute.__update_error_ui(string.Format("Location record write error: {0}", ex.Message));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                __execute.__update_error_ui(string.Format("Location record field assignment error: {0}", ex.Message));
                return;
            }

            __execute.__update_event_ui("Location write record complete ...");
        }
    }
}

