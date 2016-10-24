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
    public partial class frmRxRecord : Form
    {
        Execute __execute;
        string __action = "Add";

        motPrescriptionRecord Scrip;

        public frmRxRecord()
        {
            InitializeComponent();
            Scrip = new motPrescriptionRecord(__action, motErrorlLevel.Info, false);
            __execute = frmMainRecordTester.__execute;
        }

        private void txtAllergies_Enter(object sender, EventArgs e)
        {
            txtComments.Height *= 5;
            txtComments.BringToFront();
        }

        private void txtAllergies_Leave(object sender, EventArgs e)
        {
            txtComments.Height /= 5;
            txtComments.SendToBack();
        }


        private void txtComments_Enter(object sender, EventArgs e)
        {
            txtSig.Height *= 5;
            txtSig.BringToFront();
        }

        private void txtComments_Leave(object sender, EventArgs e)
        {
            txtSig.Height /= 5;
            txtSig.SendToBack();
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
                        __key_data.Add(new KeyValuePair<string, string>("TableType", "R" + __action.ToUpper().Substring(0, 1)));
                        //__key_data.Add(new KeyValuePair<string, string>("Action", __action.ToUpper().Substring(0, 1)));
                    }
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_PatID", txtRxSys_PatID.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_RxNum", txtRxNum.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_DocID", txtRxSys_DocID.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Sig", txtSig.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxStartDate", txtRxStartDate.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxStopDate", txtRxStopDate.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DoseScheduleName", txtDoseScheduleName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Comments", txtComments.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Refills", txtRefills.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_NewRxNum", txtRxSys_NewRxNum.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Isolate", txtIsolate.Text));
                    __key_data.Add(new KeyValuePair<string, string>("MDoMStart", txtMDOMStart.Text));
                    __key_data.Add(new KeyValuePair<string, string>("MDoMEnd", txtMDOMStop.Text));
                    __key_data.Add(new KeyValuePair<string, string>("QtyPerDose", txtQtyPerDose.Text));
                    __key_data.Add(new KeyValuePair<string, string>("QtyDispensed", txtQtyDispensed.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxType", txtRxType.Text));
                    __key_data.Add(new KeyValuePair<string, string>("Status", txtStatus.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DoW", txtDOW.Text));
                    __key_data.Add(new KeyValuePair<string, string>("SpecialDoses", txtSpecialDoses.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DoseTimesQtys", txtDoseTimesQtys.Text));
                    __key_data.Add(new KeyValuePair<string, string>("RxSys_DrugID", txtDrug_ID.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DiscontinueDate", txtDiscontinueDate.Text));

                    if (cbDelimitedTestRecord.Checked)
                    {
                        __output.WriteDelimitedFile(@"C:\Records\" + txtFileName.Text, __key_data);
                    }

                    if (cbTaggedTestRecord.Checked)
                    {
                        if (cbDelimitedTestRecord.Checked)
                        {
                            __key_data.RemoveAt(0);
                        }
                        __key_data.Add(new KeyValuePair<string, string>("ChartOnly", txtChartOnly.Text));
                        __key_data.Add(new KeyValuePair<string, string>("AnchorDate", txtAnchorDate.Text));

                        __output.WriteTaggedFile(@"C:\Records\" + txtFileName.Text, __key_data, "Rx", __action);
                    }
                }
                Scrip.setField("Action", __action);

                // Assign all the values and write
                Scrip.RxSys_PatID = txtRxSys_PatID.Text;
                Scrip.RxSys_RxNum = txtRxNum.Text;
                Scrip.RxSys_DrugID = txtDrug_ID.Text;
                Scrip.RxSys_DocID = txtRxSys_DocID.Text;
                Scrip.Sig = txtSig.Text;
                Scrip.Comments = txtComments.Text;
               
                Scrip.MDOMStart = txtMDOMStart.Text;
                Scrip.QtyPerDose = txtQtyPerDose.Text;
                Scrip.QtyDispensed = txtQtyDispensed.Text;

                Scrip.Status = txtStatus.Text;
                Scrip.DoW = txtDOW.Text;

                Scrip.RxStartDate = txtRxStartDate.Text;
                Scrip.RxStopDate = txtRxStopDate.Text;
                Scrip.DiscontinueDate = txtDiscontinueDate.Text;

                Scrip.DoseScheduleName = txtDoseScheduleName.Text;
                Scrip.Isolate = txtIsolate.Text;

                Scrip.SpecialDoses = txtSpecialDoses.Text;
                Scrip.DoseTimesQtys = txtDoseTimesQtys.Text;

                Scrip.ChartOnly = txtChartOnly.Text;
                Scrip.Refills = txtRefills.Text;
                Scrip.RxType = txtRxType.Text;
                Scrip.AnchorDate = txtAnchorDate.Text;

                __execute.__update_event_ui("Prescription field assignment complete ...");
            }
            catch(Exception ex)
            {
                __execute.__update_error_ui(string.Format("Prescription record field assignment error: {0}", ex.Message));
                return;
            }

            try
            {
                Scrip.Write(__execute.__p);
                __execute.__update_event_ui("Prescription write record complete ...");
            }
            catch(Exception ex)
            {
                __execute.__update_error_ui(string.Format("Prescription record write error: {0}", ex.Message));
                return;
            }        
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            __execute.__auto_truncate = checkBox1.Checked;
        }

        private void txtRxSys_PatID_TextChanged(object sender, EventArgs e)
        {
            valRxSys_PatID.Text = txtRxSys_PatID.Text.Length.ToString();
        }

        private void txtRxNum_TextChanged(object sender, EventArgs e)
        {
            valRxSys_RxNum.Text = txtRxNum.Text.Length.ToString();
        }

        private void txtRxSys_DocID_TextChanged(object sender, EventArgs e)
        {
            valRxSys_DocID.Text = txtRxSys_DocID.Text.Length.ToString();
        }

        private void txtMRxSys_DrugID_TextChanged(object sender, EventArgs e)
        {
            valRxSys_DrugID.Text = txtDrug_ID.Text.Length.ToString();
        }

        private void txtSig_TextChanged(object sender, EventArgs e)
        {
            valSig.Text = txtSig.Text.Length.ToString();
        }

        private void txtRxStartDate_TextChanged(object sender, EventArgs e)
        {
            valRxStartDate.Text = txtRxStartDate.Text.Length.ToString();
        }

        private void txtRxStopDate_TextChanged(object sender, EventArgs e)
        {
            valRxStopDate.Text = txtRxStopDate.Text.Length.ToString();
        }

        private void txtDiscontinueDate_TextChanged(object sender, EventArgs e)
        {
            valDiscontinueDate.Text = txtDiscontinueDate.Text.Length.ToString();
        }

        private void txtDoseScheduleName_TextChanged(object sender, EventArgs e)
        {
            valDoseScheduleName.Text = txtDoseScheduleName.Text.Length.ToString();
        }

        private void txtRefills_TextChanged(object sender, EventArgs e)
        {
            valRefills.Text = txtRefills.Text.Length.ToString();
        }

        private void txtChartOnlyc_TextChanged(object sender, EventArgs e)
        {
            valChartOnly.Text = txtChartOnly.Text.Length.ToString();
        }
        private void txtRxType_TextChanged(object sender, EventArgs e)
        {
            valRxType.Text = txtRxType.Text.Length.ToString();
        }

        private void txtIsolate_TextChanged(object sender, EventArgs e)
        {
            valIsolate.Text = txtIsolate.Text.Length.ToString();
        }

        private void txtComments_TextChanged(object sender, EventArgs e)
        {
            valComments.Text = txtComments.Text.Length.ToString();
        }

        private void txtAnchorDate_TextChanged(object sender, EventArgs e)
        {
            valAnchorDate.Text = txtAnchorDate.Text.Length.ToString();
        }

        private void txtMDOMStart_TextChanged(object sender, EventArgs e)
        {
            valMDOMStart.Text = txtMDOMStart.Text.Length.ToString();
        }

        private void txtMDOMStop_TextChanged(object sender, EventArgs e)
        {
            valMDOMStop.Text = txtMDOMStop.Text.Length.ToString();
        }

        private void txQtyPerDose_TextChanged(object sender, EventArgs e)
        {
            valQtyPerDose.Text = txtQtyPerDose.Text.Length.ToString();
        }

        private void txtQtyDispensed_TextChanged(object sender, EventArgs e)
        {
            valQtyDispensed.Text = txtQtyDispensed.Text.Length.ToString();
        }

        private void txtStatus_TextChanged(object sender, EventArgs e)
        {
            valStatus.Text = txtStatus.Text.Length.ToString();
        }

        private void txtMCaidNum_TextChanged(object sender, EventArgs e)
        {
            valDOW.Text = txtDOW.Text.Length.ToString();
        }

        private void txtAdmitDate_TextChanged(object sender, EventArgs e)
        {
            valAdmitDate.Text = txtSpecialDoses.Text.Length.ToString();
        }

        private void txtChartOnly_TextChanged(object sender, EventArgs e)
        {
            valDoseTimesQtys.Text = txtDoseTimesQtys.Text.Length.ToString();
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

        private void txtRxNum_TextChanged_1(object sender, EventArgs e)
        {
            valRxSys_RxNum.Text = txtRxNum.Text.Length.ToString();
        }
    }
}
