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
    public partial class frmDrugRecord : Form
    {
        Execute __execute;
        string __action = "Add";
        motDrugRecord Drug;

        public frmDrugRecord()
        {
            InitializeComponent();
            Drug = new motDrugRecord(__action, false);
            __execute = frmMainRecordTester.__execute;
        }

        private void txtRxSys_DrugID_TextChanged(object sender, EventArgs e)
        {
            valRxSys_DrugID.Text = txtRxSys_DrugID.Text.Length.ToString();
        }

        private void txtLblCode_TextChanged(object sender, EventArgs e)
        {
            valLblCode.Text = txtLblCode.Text.Length.ToString();
        }

        private void txtProdCode_TextChanged(object sender, EventArgs e)
        {
            valProdCode.Text = txtProdCode.Text.Length.ToString();
        }

        private void txtTradename_TextChanged(object sender, EventArgs e)
        {
            valTradename.Text = txtTradename.Text.Length.ToString();
        }

        private void txtStrength_TextChanged(object sender, EventArgs e)
        {
            valStrength.Text = txtStrength.Text.Length.ToString();
        }

        private void txtUnit_TextChanged(object sender, EventArgs e)
        {
            valUnit.Text = txtUnit.Text.Length.ToString();
        }

        private void txtRxOtc_TextChanged(object sender, EventArgs e)
        {
            valRxOtc.Text = txtRxOtc.Text.Length.ToString();
        }

        private void txtDoseForm_TextChanged(object sender, EventArgs e)
        {
            valDoseForm.Text = txtDoseForm.Text.Length.ToString();
        }

        private void txtRoute_TextChanged(object sender, EventArgs e)
        {
            valRoute.Text = txtRoute.Text.Length.ToString();
        }

        private void txtDrugSchedule_TextChanged(object sender, EventArgs e)
        {
            valDrugSchedule.Text = txtDrugSchedule.Text.Length.ToString();
        }

        private void txtVisualDescription_TextChanged(object sender, EventArgs e)
        {
            valVisualDescription.Text = txtVisualDescription.Text.Length.ToString();
        }

        private void txtDrugName_TextChanged(object sender, EventArgs e)
        {
            valDrugName.Text = txtDrugName.Text.Length.ToString();
        }

        private void txtShortName_TextChanged(object sender, EventArgs e)
        {
            valShortName.Text = txtShortName.Text.Length.ToString();
        }

        private void txtNDCNum_TextChanged(object sender, EventArgs e)
        {
            valNDCNum.Text = txtNDCNum.Text.Length.ToString();
        }

        private void txtSizeFactor_TextChanged(object sender, EventArgs e)
        {
            valSizeFactor.Text = txtSizeFactor.Text.Length.ToString();
        }

        private void txtTemplate_TextChanged(object sender, EventArgs e)
        {
            valTemplate.Text = txtTemplate.Text.Length.ToString();
        }

        private void txtDefaultIsolate_TextChanged(object sender, EventArgs e)
        {
            valDefaultIsolate.Text = txtDefaultIsolate.Text.Length.ToString();
        }

        private void txtConsultMsg_TextChanged(object sender, EventArgs e)
        {
            valConsultMsg.Text = txtConsultMsg.Text.Length.ToString();
        }

        private void txtGenericFor_TextChanged(object sender, EventArgs e)
        {
            valGenericFor.Text = txtGenericFor.Text.Length.ToString();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                Drug.__send_eof = chkSendEOF.Checked;

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
                        __key_data.Add(new KeyValuePair<string, string>("TableType", "D" + __action.ToUpper().Substring(0, 1)));
                    }

                    __key_data.Add(new KeyValuePair<string, string>("RxSys_DrugID", txtRxSys_DrugID.Text));
                    __key_data.Add(new KeyValuePair<string, string>("LabelCode", txtLblCode.Text));
                    __key_data.Add(new KeyValuePair<string, string>("ProductCode", txtProdCode.Text));
                    __key_data.Add(new KeyValuePair<string, string>("TradeName", txtTradename.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Strength", txtStrength.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Unit", txtUnit.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxOTC", txtRxOtc.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DoseForm", txtDoseForm.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Route", txtRoute.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DrugSchedule", txtDrugSchedule.Text));
                    __key_data.Add(new KeyValuePair<string, string>("VisualDescription", txtVisualDescription.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DrugName", txtDrugName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("ShortName", txtShortName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("NDCNum", txtNDCNum.Text));
                    __key_data.Add(new KeyValuePair<string, string>("SizeFactor", txtSizeFactor.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Template", txtTemplate.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DefaultIsolate", txtDefaultIsolate.Text));
                    __key_data.Add(new KeyValuePair<string, string>("ConsultMsg", txtConsultMsg.Text));
                    __key_data.Add(new KeyValuePair<string, string>("GenericFor", txtGenericFor.Text));

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

                        string __record = __output.WriteTaggedFile(@"C:\Records\" + txtFileName.Text, __key_data, "Drug", __action);

                        if (cbSendTaggedRecord.Checked)
                        {
                            __execute.__p.write(__record);
                        }
                    }
                }
                else
                {
                    Drug.RxSys_DrugID = txtRxSys_DrugID.Text;
                    Drug.LabelCode = txtLblCode.Text;
                    Drug.ProductCode = txtProdCode.Text;
                    Drug.TradeName = txtTradename.Text;
                    //Drug.Strength = Convert.ToInt32(txtStrength.Text);
                    Drug.Strength = txtStrength.Text;
                    Drug.Unit = txtUnit.Text;
                    Drug.RxOTC = txtRxOtc.Text;
                    Drug.DoseForm = txtDoseForm.Text;
                    Drug.Route = txtRoute.Text;
                    Drug.DrugSchedule = Convert.ToInt32(txtDrugSchedule.Text);
                    Drug.VisualDescription = txtVisualDescription.Text;
                    Drug.DrugName = txtDrugName.Text;
                    Drug.ShortName = txtShortName.Text;
                    Drug.NDCNum = txtNDCNum.Text;
                    Drug.SizeFactor = Convert.ToInt32(txtSizeFactor.Text);
                    Drug.Template = txtTemplate.Text;
                    Drug.DefaultIsolate = Convert.ToInt32(txtDefaultIsolate.Text);
                    Drug.ConsultMsg = txtConsultMsg.Text;
                    Drug.GenericFor = txtGenericFor.Text;

                    __execute.__update_event_ui("Drug field assignment complete ...");

                    try
                    {
                        Drug.Write(__execute.__p);
                    }
                    catch (Exception ex)
                    {
                        __execute.__update_error_ui(string.Format("Drug record write error: {0}", ex.Message));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                __execute.__update_error_ui(string.Format("Drug record error: {0}", ex.Message));
                return;
            }

            __execute.__update_event_ui("Drug write record complete ...");
        }

        private void rbChange_CheckedChanged(object sender, EventArgs e)
        {
            __action = "Change";
        }

        private void rbDelete_CheckedChanged(object sender, EventArgs e)
        {
            __action = "Delete";
        }

        private void rbAdd_CheckedChanged(object sender, EventArgs e)
        {
            __action = "Add";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            __execute.__auto_truncate = checkBox1.Checked;
        }
    }
}
