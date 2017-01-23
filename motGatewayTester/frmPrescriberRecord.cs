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
    public partial class frmPrescriberRecord : Form
    {
        motPrescriberRecord Doc;
        Execute __execute;
        string __action = "Add";

        public frmPrescriberRecord()
        {
            InitializeComponent();

            Doc = new motPrescriberRecord(__action, false);
            __execute = frmMainRecordTester.__execute;
        }

        private void txtRxSys_DocID_TextChanged(object sender, EventArgs e)
        {
            valRxSys_DocID.Text = txtRxSys_DocID.Text.Length.ToString();
        }
        private void txtLastName_TextChanged(object sender, EventArgs e)
        {
            valLastName.Text = txtLastName.Text.Length.ToString();
        }
        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            valFirstName.Text = txtFirstName.Text.Length.ToString();
        }
        private void txtMiddleInitial_TextChanged(object sender, EventArgs e)
        {
            valMiddleInitial.Text = txtMiddleInitial.Text.Length.ToString();
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

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            valComments.Text = txtComments.Text.Length.ToString();
        }

        private void txtDEA_ID_TextChanged(object sender, EventArgs e)
        {
            valDEA_ID.Text = txtDEA_ID.Text.Length.ToString();
        }
        private void txtTPID_TextChanged(object sender, EventArgs e)
        {
            valTPID.Text = txtTPID.Text.Length.ToString();
        }
        private void txtSpecialty_TextChanged(object sender, EventArgs e)
        {
            valSpecialty.Text = txtSpecialty.Text.Length.ToString();
        }
        private void txtFax_TextChanged(object sender, EventArgs e)
        {
            valFax.Text = txtFax.Text.Length.ToString();
        }
        private void txtPagerInfo_TextChanged(object sender, EventArgs e)
        {
            valPagerInfo.Text = txtPagerInfo.Text.Length.ToString();
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
            Doc.__auto_truncate = chkAutoTruncate.Checked;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                Doc.__send_eof = chkSendEOF.Checked;

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
                        __key_data.Add(new KeyValuePair<string, string>("TableType", "P" + __action.ToUpper().Substring(0, 1)));
                    }

                    __key_data.Add(new KeyValuePair<string, string>("LastName", txtLastName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("FirstName", txtFirstName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("MiddleInitial", txtMiddleInitial.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Address1", txtAddress1.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Address2", txtAddress2.Text));
                    __key_data.Add(new KeyValuePair<string, string>("City", txtCity.Text));
                    __key_data.Add(new KeyValuePair<string, string>("State", txtState.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Zip", txtZip.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Phone", txtPhone.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Comments", txtComments.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DEA_ID", txtDEA_ID.Text));
                    __key_data.Add(new KeyValuePair<string, string>("TPID", txtTPID.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Speciality", txtSpecialty.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Fax", txtFax.Text));
                    __key_data.Add(new KeyValuePair<string, string>("PagerInfo", txtPagerInfo.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_DocID", txtRxSys_DocID.Text));

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

                        string __record = __output.WriteTaggedFile(@"C:\Records\" + txtFileName.Text, __key_data, "Prescriber", __action);

                        if (cbSendTaggedRecord.Checked)
                        {
                            __execute.__p.write(__record);
                        }
                    }
                }
                else
                {
                    Doc.RxSys_DocID = txtRxSys_DocID.Text;
                    Doc.LastName = txtLastName.Text;
                    Doc.FirstName = txtFirstName.Text;
                    Doc.MiddleInitial = txtMiddleInitial.Text;
                    Doc.Address1 = txtAddress1.Text;
                    Doc.Address2 = txtAddress2.Text;
                    Doc.City = txtCity.Text;
                    Doc.State = txtState.Text;
                    Doc.PostalCode = txtZip.Text;
                    Doc.Phone = txtPhone.Text;
                    Doc.Comments = txtComments.Text;
                    Doc.DEA_ID = txtDEA_ID.Text;
                    Doc.TPID = txtTPID.Text;
                    Doc.Specialty = txtSpecialty.Text.Length > 0 ? Convert.ToInt32(txtSpecialty.Text) : 0;
                    Doc.Fax = txtFax.Text;
                    Doc.PagerInfo = txtPagerInfo.Text;

                    __execute.__update_event_ui("Prescriber field assignment complete ...");

                    try
                    {
                        Doc.Write(__execute.__p);
                        
                    }
                    catch (Exception ex)
                    {
                        __execute.__update_error_ui(string.Format("Prescriber record write error: {0}", ex.Message));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                __execute.__update_error_ui(string.Format("Prescriber record field assignment error: {0}", ex.Message));
                return;
            }

            __execute.__update_event_ui("Prescriber write record complete ...");
        }
    }
}
