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
    public partial class frmDoseSchedule : Form
    {
        Execute __execute;
        string __action = "Add";
        motTimeQtysRecord TQ;

        public frmDoseSchedule()
        {
            InitializeComponent();

            TQ = new motTimeQtysRecord(__action, motErrorlLevel.Info, false);
            __execute = frmMainRecordTester.__execute;
        }

        private void txtRxSys_LocID_TextChanged(object sender, EventArgs e)
        {
            valRxSys_LocID.Text = txtRxSys_LocID.Text.Length.ToString();
        }

        private void txtDoseScheduleName_TextChanged(object sender, EventArgs e)
        {
            valDoseScheduleName.Text = txtDoseScheduleName.Text.Length.ToString();
        }

        private void DoseTimesQtys_TextChanged(object sender, EventArgs e)
        {
            valDoseTimesQtys.Text = txtDoseTimesQtys.Text.Length.ToString();
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
                        __key_data.Add(new KeyValuePair<string, string>("TableType", "T" + __action.ToUpper().Substring(0, 1)));
                    }

                    __key_data.Add(new KeyValuePair<string, string>("RxSys_LocID", txtRxSys_LocID.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DoseScheduleName", txtDoseScheduleName.Text));
                    __key_data.Add(new KeyValuePair<string, string>("DoseTimesQtys", txtDoseTimesQtys.Text));

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

                        string __record = __output.WriteTaggedFile(@"C:\Records\" + txtFileName.Text, __key_data, "TimesQtys", __action);

                        if (cbSendTaggedRecord.Checked)
                        {
                            __execute.__p.Write(__record);
                        }
                    }
                }
                else
                {
                    TQ.RxSys_LocID = txtRxSys_LocID.Text;
                    TQ.DoseScheduleName = txtDoseScheduleName.Text;
                    TQ.DoseTimesQtys = txtDoseTimesQtys.Text;

                    __execute.__update_event_ui("Time and Quantities field assignment complete ...");

                    try
                    {
                        TQ.Write(__execute.__p);
                    }
                    catch (Exception ex)
                    {
                        __execute.__update_error_ui(string.Format("Time and Quantities record write error: {0}", ex.Message));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                __execute.__update_error_ui(string.Format("Time and Quantities record error: {0}", ex.Message));
                return;
            }

            __execute.__update_event_ui("Time and Quantities write record complete ...");
        }
    }
}
